using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FTP.Core.Models;
using FTP.Core.Client;
using FTP.Core.Enum;

namespace FTP.Core.DataTransfer
{
    // Quản lý hàng đợi upload/download - xử lý tuần tự các transfer
    public class TransferQueue
    {
        private readonly Queue<TransferItem> _queue;
        private readonly object _lock = new object();
        private bool _isProcessing = false;
        private FtpClient _ftpClient;
        private CancellationTokenSource _processingCancellation;
        // Danh sách tất cả transfer items (bao gồm đang chờ, đang xử lý, hoàn thành)
        public List<TransferItem> AllItems { get; private set; }
        // Số lượng items đang chờ trong queue
        public int PendingCount
        {
            get
            {
                lock (_lock)
                {
                    return _queue.Count;
                }
            }
        }
        // Có đang xử lý transfer không
        public bool IsProcessing
        {
            get { return _isProcessing; }
        }

        // Events
        public event EventHandler<TransferItem> TransferStarted;
        public event EventHandler<TransferItem> TransferCompleted;
        public event EventHandler<TransferItem> TransferFailed;
        public event EventHandler<TransferItem> TransferCancelled;
        public event EventHandler<TransferItem> TransferProgressChanged;
        public event EventHandler QueueCompleted;

        public TransferQueue(FtpClient ftpClient)
        {
            _ftpClient = ftpClient ?? throw new ArgumentNullException(nameof(ftpClient));
            _queue = new Queue<TransferItem>();
            AllItems = new List<TransferItem>();
        }
        // Thêm transfer item vào queue
        public void AddTransfer(TransferItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (_lock)
            {
                _queue.Enqueue(item);
                AllItems.Add(item);
                item.Status = TransferStatus.Waiting;
            }

            // Bắt đầu xử lý queue nếu chưa chạy
            if (!_isProcessing)
            {
                Task.Run(() => ProcessQueueAsync());
            }
        }
        // Thêm nhiều transfers cùng lúc
        public void AddTransfers(IEnumerable<TransferItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (TransferItem item in items)
            {
                lock (_lock)
                {
                    _queue.Enqueue(item);
                    AllItems.Add(item);
                    item.Status = TransferStatus.Waiting;
                }
            }

            if (!_isProcessing)
            {
                Task.Run(() => ProcessQueueAsync());
            }
        }
        // Xóa một transfer khỏi queue (chỉ xóa được nếu đang Waiting)
        public bool RemoveTransfer(Guid transferId)
        {
            lock (_lock)
            {
                TransferItem item = AllItems.FirstOrDefault(t => t.Id == transferId);
                if (item != null && item.Status == TransferStatus.Waiting)
                {
                    // Không thể remove trực tiếp từ Queue, phải đánh dấu Cancelled
                    item.Cancel();
                    return true;
                }
                return false;
            }
        }
        // Hủy transfer đang chạy
        public void CancelCurrent()
        {
            if (_processingCancellation != null && !_processingCancellation.IsCancellationRequested)
            {
                _processingCancellation.Cancel();
            }
        }
        // Dừng toàn bộ queue
        public void StopQueue()
        {
            _isProcessing = false;
            CancelCurrent();

            lock (_lock)
            {
                // Hủy tất cả items đang chờ
                while (_queue.Count > 0)
                {
                    TransferItem item = _queue.Dequeue();
                    if (item.Status == TransferStatus.Waiting)
                    {
                        item.Cancel();
                        OnTransferCancelled(item);
                    }
                }
            }
        }
        // Xóa tất cả transfer đã hoàn thành hoặc failed
        public void ClearCompleted()
        {
            lock (_lock)
            {
                AllItems.RemoveAll(t =>
                    t.Status == TransferStatus.Completed ||
                    t.Status == TransferStatus.Failed ||
                    t.Status == TransferStatus.Cancelled);
            }
        }
        // Xử lý queue - chạy tuần tự từng transfer
        private async Task ProcessQueueAsync()
        {
            _isProcessing = true;

            while (_queue.Count > 0)
            {
                TransferItem item = null;

                lock (_lock)
                {
                    if (_queue.Count > 0)
                    {
                        item = _queue.Dequeue();
                    }
                }

                if (item == null)
                    break;

                // Bỏ qua nếu đã bị cancel
                if (item.Status == TransferStatus.Cancelled)
                {
                    OnTransferCancelled(item);
                    continue;
                }

                // Bắt đầu transfer
                item.Start();
                OnTransferStarted(item);

                try
                {
                    // Tạo CancellationTokenSource cho transfer này
                    _processingCancellation = new CancellationTokenSource();
                    item.CancellationTokenSource = _processingCancellation;

                    // Progress reporter
                    Progress<TransferProgressEventArgs> progress = new Progress<TransferProgressEventArgs>(args =>
                    {
                        item.UpdateProgress(args.BytesTransferred);
                        OnTransferProgressChanged(item);
                    });

                    // Thực hiện transfer (Upload hoặc Download)
                    bool success = false;
                    if (item.Type == TransferType.Upload)
                    {
                        success = await _ftpClient.UploadFileAsync(
                            item.LocalPath,
                            item.RemotePath,
                            progress);
                    }
                    else // Download
                    {
                        success = await _ftpClient.DownloadFileAsync(
                            item.RemotePath,
                            item.LocalPath,
                            progress);
                    }

                    // Kiểm tra kết quả
                    if (_processingCancellation.IsCancellationRequested)
                    {
                        item.Cancel();
                        OnTransferCancelled(item);
                    }
                    else if (success)
                    {
                        item.Complete();
                        OnTransferCompleted(item);
                    }
                    else
                    {
                        item.Fail("Transfer failed");
                        OnTransferFailed(item);
                    }
                }
                catch (Exception ex)
                {
                    item.Fail(ex.Message);
                    OnTransferFailed(item);
                }
                finally
                {
                    _processingCancellation?.Dispose();
                    _processingCancellation = null;
                }
            }

            _isProcessing = false;
            OnQueueCompleted();
        }
        // Lấy tất cả transfers theo trạng thái
        public List<TransferItem> GetTransfersByStatus(TransferStatus status)
        {
            lock (_lock)
            {
                return AllItems.Where(t => t.Status == status).ToList();
            }
        }

        // Event triggers
        protected virtual void OnTransferStarted(TransferItem item)
        {
            TransferStarted?.Invoke(this, item);
        }

        protected virtual void OnTransferCompleted(TransferItem item)
        {
            TransferCompleted?.Invoke(this, item);
        }

        protected virtual void OnTransferFailed(TransferItem item)
        {
            TransferFailed?.Invoke(this, item);
        }

        protected virtual void OnTransferCancelled(TransferItem item)
        {
            TransferCancelled?.Invoke(this, item);
        }

        protected virtual void OnTransferProgressChanged(TransferItem item)
        {
            TransferProgressChanged?.Invoke(this, item);
        }

        protected virtual void OnQueueCompleted()
        {
            QueueCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
