using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FTP.Core.Models;
using FTP.Core.Client;
using FTP.Core.Enum;
using FTP.Core.Helpers;

namespace FTP.Core.DataTransfer
{
    // Quản lý upload operations - hỗ trợ upload nhiều file, folder
    public class UploadManager
    {
        private readonly FtpClient _ftpClient;
        private readonly TransferQueue _transferQueue;

        public UploadManager(FtpClient ftpClient, TransferQueue transferQueue)
        {
            _ftpClient = ftpClient ?? throw new ArgumentNullException(nameof(ftpClient));
            _transferQueue = transferQueue ?? throw new ArgumentNullException(nameof(transferQueue));
        }
        // Upload một file
        public TransferItem UploadFile(string localFilePath, string remoteDirectory)
        {
            if (string.IsNullOrWhiteSpace(localFilePath))
                throw new ArgumentException("Local file path is required", nameof(localFilePath));

            if (!File.Exists(localFilePath))
                throw new FileNotFoundException("Local file not found", localFilePath);

            // Tạo remote path
            string fileName = Path.GetFileName(localFilePath);
            string remotePath = PathHelper.Combine(remoteDirectory, fileName);

            // Tạo TransferItem
            FileInfo fileInfo = new FileInfo(localFilePath);
            TransferItem item = new TransferItem
            {
                Type = TransferType.Upload,
                LocalPath = localFilePath,
                RemotePath = remotePath,
                TotalBytes = fileInfo.Length,
                Status = TransferStatus.Waiting
            };

            // Thêm vào queue
            _transferQueue.AddTransfer(item);

            return item;
        }
        // Upload nhiều file
        public List<TransferItem> UploadFiles(IEnumerable<string> localFilePaths, string remoteDirectory)
        {
            if (localFilePaths == null)
                throw new ArgumentNullException(nameof(localFilePaths));

            List<TransferItem> items = new List<TransferItem>();

            foreach (string filePath in localFilePaths)
            {
                if (File.Exists(filePath))
                {
                    TransferItem item = CreateTransferItem(filePath, remoteDirectory);
                    items.Add(item);
                }
            }

            if (items.Count > 0)
            {
                _transferQueue.AddTransfers(items);
            }

            return items;
        }
        // Upload toàn bộ folder (recursive)
        public async Task<List<TransferItem>> UploadFolderAsync(string localFolderPath, string remoteDirectory)
        {
            if (string.IsNullOrWhiteSpace(localFolderPath))
                throw new ArgumentException("Local folder path is required", nameof(localFolderPath));

            if (!Directory.Exists(localFolderPath))
                throw new DirectoryNotFoundException($"Local folder not found: {localFolderPath}");

            List<TransferItem> allItems = new List<TransferItem>();

            // Tạo folder trên server trước
            string folderName = Path.GetFileName(localFolderPath.TrimEnd(Path.DirectorySeparatorChar));
            string remoteFolder = PathHelper.Combine(remoteDirectory, folderName);

            try
            {
                await _ftpClient.CreateDirectoryAsync(remoteFolder);
            }
            catch
            {
                // Folder có thể đã tồn tại, bỏ qua lỗi
            }

            // Upload tất cả files trong folder
            string[] files = Directory.GetFiles(localFolderPath);
            foreach (string filePath in files)
            {
                TransferItem item = CreateTransferItem(filePath, remoteFolder);
                allItems.Add(item);
            }

            // Recursively upload subfolders
            string[] subFolders = Directory.GetDirectories(localFolderPath);
            foreach (string subFolder in subFolders)
            {
                List<TransferItem> subItems = await UploadFolderAsync(subFolder, remoteFolder);
                allItems.AddRange(subItems);
            }

            // Thêm tất cả vào queue
            if (allItems.Count > 0)
            {
                _transferQueue.AddTransfers(allItems);
            }

            return allItems;
        }
        // Tạo TransferItem từ file path
        private TransferItem CreateTransferItem(string localFilePath, string remoteDirectory)
        {
            string fileName = Path.GetFileName(localFilePath);
            string remotePath = PathHelper.Combine(remoteDirectory, fileName);

            FileInfo fileInfo = new FileInfo(localFilePath);

            return new TransferItem
            {
                Type = TransferType.Upload,
                LocalPath = localFilePath,
                RemotePath = remotePath,
                TotalBytes = fileInfo.Length,
                Status = TransferStatus.Waiting
            };
        }
        // Ước tính thời gian upload (dựa trên kích thước file và tốc độ trung bình)
        public TimeSpan EstimateUploadTime(long fileSize, double averageSpeedBytesPerSecond = 1048576) // Default 1 MB/s
        {
            if (averageSpeedBytesPerSecond <= 0)
                averageSpeedBytesPerSecond = 1048576;

            double seconds = fileSize / averageSpeedBytesPerSecond;
            return TimeSpan.FromSeconds(seconds);
        }
        // Kiểm tra file local có tồn tại không
        public bool ValidateLocalFile(string localFilePath)
        {
            return !string.IsNullOrWhiteSpace(localFilePath) && File.Exists(localFilePath);
        }
        /// Lấy tổng kích thước của nhiều file
        public long GetTotalSize(IEnumerable<string> localFilePaths)
        {
            long totalSize = 0;

            foreach (string filePath in localFilePaths)
            {
                if (File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    totalSize += fileInfo.Length;
                }
            }

            return totalSize;
        }
    }
}
