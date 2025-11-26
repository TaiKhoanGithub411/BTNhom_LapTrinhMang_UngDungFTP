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
    // Quản lý download operations - hỗ trợ download nhiều file, folder

    public class DownloadManager
    {
        private readonly FtpClient _ftpClient;
        private readonly TransferQueue _transferQueue;

        public DownloadManager(FtpClient ftpClient, TransferQueue transferQueue)
        {
            _ftpClient = ftpClient ?? throw new ArgumentNullException(nameof(ftpClient));
            _transferQueue = transferQueue ?? throw new ArgumentNullException(nameof(transferQueue));
        }
        // Download một file
    
        public TransferItem DownloadFile(string remotePath, string localDirectory)
        {
            if (string.IsNullOrWhiteSpace(remotePath))
                throw new ArgumentException("Remote path is required", nameof(remotePath));

            if (!Directory.Exists(localDirectory))
                Directory.CreateDirectory(localDirectory);

            // Tạo local path
            string fileName = PathHelper.GetFileName(remotePath);
            string localPath = Path.Combine(localDirectory, fileName);

            // Tạo TransferItem (TotalBytes sẽ cập nhật khi download)
            TransferItem item = new TransferItem
            {
                Type = TransferType.Download,
                LocalPath = localPath,
                RemotePath = remotePath,
                TotalBytes = 0, // Sẽ được cập nhật trong quá trình download
                Status = TransferStatus.Waiting
            };

            // Thêm vào queue
            _transferQueue.AddTransfer(item);

            return item;
        }
        // Download nhiều file
    
        public List<TransferItem> DownloadFiles(IEnumerable<FtpFileItem> remoteFiles, string localDirectory)
        {
            if (remoteFiles == null)
                throw new ArgumentNullException(nameof(remoteFiles));

            if (!Directory.Exists(localDirectory))
                Directory.CreateDirectory(localDirectory);

            List<TransferItem> items = new List<TransferItem>();

            foreach (FtpFileItem remoteFile in remoteFiles)
            {
                // Chỉ download file, không download folder
                if (!remoteFile.IsDirectory)
                {
                    TransferItem item = CreateTransferItem(remoteFile, localDirectory);
                    items.Add(item);
                }
            }

            if (items.Count > 0)
            {
                _transferQueue.AddTransfers(items);
            }

            return items;
        }
        // Download toàn bộ folder (recursive)    
        public async Task<List<TransferItem>> DownloadFolderAsync(string remoteFolderPath, string localDirectory)
        {
            if (string.IsNullOrWhiteSpace(remoteFolderPath))
                throw new ArgumentException("Remote folder path is required", nameof(remoteFolderPath));

            List<TransferItem> allItems = new List<TransferItem>();

            // Tạo local folder
            string folderName = PathHelper.GetFileName(remoteFolderPath);
            string localFolder = Path.Combine(localDirectory, folderName);

            if (!Directory.Exists(localFolder))
                Directory.CreateDirectory(localFolder);

            // List tất cả files và folders trong remote folder
            List<FtpFileItem> items = await _ftpClient.ListFilesAsync(remoteFolderPath);

            // Download tất cả files
            foreach (FtpFileItem item in items)
            {
                if (!item.IsDirectory)
                {
                    TransferItem transferItem = CreateTransferItem(item, localFolder);
                    allItems.Add(transferItem);
                }
            }

            // Recursively download subfolders
            foreach (FtpFileItem item in items)
            {
                if (item.IsDirectory)
                {
                    List<TransferItem> subItems = await DownloadFolderAsync(item.FullPath, localFolder);
                    allItems.AddRange(subItems);
                }
            }

            // Thêm tất cả vào queue
            if (allItems.Count > 0)
            {
                _transferQueue.AddTransfers(allItems);
            }

            return allItems;
        }
        // Tạo TransferItem từ FtpFileItem
    
        private TransferItem CreateTransferItem(FtpFileItem remoteFile, string localDirectory)
        {
            string localPath = Path.Combine(localDirectory, remoteFile.Name);

            return new TransferItem
            {
                Type = TransferType.Download,
                LocalPath = localPath,
                RemotePath = remoteFile.FullPath,
                TotalBytes = remoteFile.Size,
                Status = TransferStatus.Waiting
            };
        }
        // Ước tính thời gian download    
        public TimeSpan EstimateDownloadTime(long fileSize, double averageSpeedBytesPerSecond = 1048576) // Default 1 MB/s
        {
            if (averageSpeedBytesPerSecond <= 0)
                averageSpeedBytesPerSecond = 1048576;

            double seconds = fileSize / averageSpeedBytesPerSecond;
            return TimeSpan.FromSeconds(seconds);
        }
        // Kiểm tra local directory có tồn tại không, tạo mới nếu chưa có    
        public void EnsureLocalDirectory(string localDirectory)
        {
            if (!Directory.Exists(localDirectory))
            {
                Directory.CreateDirectory(localDirectory);
            }
        }
        // Tính tổng kích thước của nhiều remote files
        public long GetTotalSize(IEnumerable<FtpFileItem> remoteFiles)
        {
            long totalSize = 0;

            foreach (FtpFileItem file in remoteFiles)
            {
                if (!file.IsDirectory && file.Size > 0)
                {
                    totalSize += file.Size;
                }
            }

            return totalSize;
        }
        // Kiểm tra file local đã tồn tại chưa (để tránh overwrite)
        public bool LocalFileExists(string localPath)
        {
            return File.Exists(localPath);
        }
        // Tạo unique filename nếu file đã tồn tại (thêm (1), (2)...)
    
        public string GetUniqueLocalPath(string localPath)
        {
            if (!File.Exists(localPath))
                return localPath;

            string directory = Path.GetDirectoryName(localPath);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(localPath);
            string extension = Path.GetExtension(localPath);

            int counter = 1;
            string newPath;

            do
            {
                string newFileName = string.Format("{0} ({1}){2}", fileNameWithoutExt, counter, extension);
                newPath = Path.Combine(directory, newFileName);
                counter++;
            }
            while (File.Exists(newPath));

            return newPath;
        }
    }
}
