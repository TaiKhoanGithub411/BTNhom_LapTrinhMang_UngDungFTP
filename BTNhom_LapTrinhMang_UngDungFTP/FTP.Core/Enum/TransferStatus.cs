using System;

namespace FTP.Core.Enum
{
    public enum TransferStatus
    {
        Waiting,       // Đang chờ trong queue
        Transferring,  // Đang transfer
        Completed,     // Hoàn thành
        Failed,        // Thất bại
        Cancelled      // Bị hủy bởi user
    }
}
