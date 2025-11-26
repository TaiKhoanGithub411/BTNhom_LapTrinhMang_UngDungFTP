using System;
using System.Threading;
using FTP.Core.Enum;

namespace FTP.Core.Models
{
    public class TransferItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();// ID duy nhất của transfer
        public TransferType Type { get; set; }// Loại transfer (Upload/Download)
        public string LocalPath { get; set; }// Đường dẫn file local (C:\Users\...)
        public string RemotePath { get; set; }// Đường dẫn file trên server (/folder/file.txt)
        public string FileName => System.IO.Path.GetFileName(Type == TransferType.Upload ? LocalPath : RemotePath);//Tên file
        public long TotalBytes { get; set; }//Tổng  kích thước file (bytes)
        public long TransferredBytes { get; set; }//Số bytes đã transfer
        public int ProgressPercentage// Phần trăm hoàn thành (0-100)
        {
            get
            {
                if (TotalBytes == 0) return 0;
                return (int)((TransferredBytes * 100) / TotalBytes);
            }
        }
        public TransferStatus Status { get; set; } = TransferStatus.Waiting;// Trạng thái hiện tại của transfer
        public DateTime? StartTime { get; set; }//Thời gian bắt đầu transfer
        public DateTime? EndTime { get; set; }//Thời gian kết thúc transfer
        public double TransferSpeed// Tốc độ transfer hiện tại (bytes/second)
        {
            get
            {
                if (StartTime == null) return 0;
                var elapsed = (DateTime.Now - StartTime.Value).TotalSeconds;
                if (elapsed <= 0) return 0;
                return TransferredBytes / elapsed;
            }
        }
        public string TransferSpeedFormatted// Tốc độ transfer dạng string (KB/s, MB/s)
        {
            get
            {
                double speed = TransferSpeed;
                if (speed < 1024) return $"{speed:F0} B/s";
                if (speed < 1024 * 1024) return $"{speed / 1024:F2} KB/s";
                return $"{speed / (1024 * 1024):F2} MB/s";
            }
        }
        public int EstimatedTimeRemaining// Thời gian còn lại ước tính (giây)
        {
            get
            {
                if (TransferSpeed == 0) return -1;
                long remainingBytes = TotalBytes - TransferredBytes;
                return (int)(remainingBytes / TransferSpeed);
            }
        }
        public string EstimatedTimeFormatted// Thời gian còn lại dạng string (HH:mm:ss)
        {
            get
            {
                int seconds = EstimatedTimeRemaining;
                if (seconds < 0) return "Calculating...";
                TimeSpan ts = TimeSpan.FromSeconds(seconds);
                return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
            }
        }
        public string ErrorMessage { get; set; }//Thông báo lỗi
        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();// CancellationTokenSource để hủy transfer
        public event EventHandler<long> ProgressChanged;// Event khi progress thay đổi
        public void UpdateProgress(long bytesTransferred)//Cập nhật progress
        {
            TransferredBytes = bytesTransferred;
            ProgressChanged?.Invoke(this, bytesTransferred);
        }
        public void Start()//Bắt đẩu transfer
        {
            StartTime = DateTime.Now;
            Status = TransferStatus.Transferring;
        }
        public void Complete()//Hoàn thành transfer
        {
            EndTime = DateTime.Now;
            Status = TransferStatus.Completed;
            TransferredBytes = TotalBytes;
        }
        public void Fail(string errorMessage)//Đánh dáu transfer là failed
        {
            EndTime = DateTime.Now;
            Status = TransferStatus.Failed;
            ErrorMessage = errorMessage;
        }
        public void Cancel()//Hủy transfer
        {
            CancellationTokenSource?.Cancel();
            EndTime = DateTime.Now;
            Status = TransferStatus.Cancelled;
        }

        public override string ToString()
        {
            return $"{Type}: {FileName} ({ProgressPercentage}% - {Status})";
        }
    }
}
