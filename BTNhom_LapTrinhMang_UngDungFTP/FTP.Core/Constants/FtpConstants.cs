using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTP.Core.Constants
{
    //Hằng số FTP theo chuẩn RFC 959. Định nghĩa mã phản hồi, cổng mặc định và cài đặt server
    public static class FtpConstants
    {
        #region Mã phản hồi FTP
        //Nhóm 1xx: phản hồi tích cực sơ bộ - Positive Preliminary reply
        //Server đã nhận lệnh, đang xử lý (ví dụ: đang chuẩn bị gửi file)

        public const int CODE_DATA_CONNECTION_OPEN = 150; //Trạng thái file OK, chuẩn bị mở kết nối dữ liệu.

        //Nhóm 2xx: Phản hồi hoàn thành tích cực - Positive Completion reply
        // Lệnh thực hiện thành công hoàn toàn

        public const int CODE_COMMAND_OK = 200; //Lệnh thực hiện thành công - Dùng cho các lệnh đơn giản như TYPE, MODE, STRU.
        public const int CODE_SERVICE_READY = 220; //Dịch vụ sẵn sàng cho người dùng mới - welcome message
        public const int CODE_CLOSING_CONTROL = 221; //Dịch vụ đóng kết nối điều khiển (control connection) - khi client gửi lệnh QUIT, ngắt kết nối
        public const int CODE_CLOSING_DATA = 226; //Đóng kết nối dữ liệu (data connection) - sau khi truyền file hoàn thành.
        public const int CODE_PASSIVE_MODE = 227; //Vào chế độ Passive
        public const int CODE_LOGGED_IN = 230; //Người dùng đăng nhập thành công
        public const int CODE_FILE_ACTION_OK = 250; //Thao tác với file hoàn thành - xóa file (DELE), xóa thư mực (RMD), đổi tên (RNFR/RNTO)
        public const int CODE_PATHNAME_CREATED = 257; //Đướng dẫn (Pathname) đã được tạo - hiển thị thư mục hiện tại (PWD) và tạo thư mục mới (MKD)

        //Nhóm 3xx: Phản hồi trung gian tích cực - Positive Intermediate reply
        //Lệnh OK nhưng cần thêm thông tin (ví dụ: cần password)
        public const int CODE_NEED_PASSWORD = 331; //Cần nhập mật khẩu - khi người dùng mới nhập tên đăng nhập chưa nhập mật khẩu.

        //Nhóm 4xx: Phản hồi tiêu cực tạm thời - Transient Negative Completion reply
        //Lỗi tạm thời (có thể thử lại sau)
        public const int CODE_SERVICE_NOT_AVAILABLE = 421; //Dùng khi server quá tải hay bảo trì
        public const int CODE_CANNOT_OPEN_DATA = 425; //Không thể mở kết nối dữ liệu - không thiết lập được data connection
        public const int CODE_CONNECTION_CLOSED = 426;//Kết nối bị đóng, truyền file bị hủy - việc transfer bị gián đoạn

        //Nhóm 5xx: Phản hồi tiêu cực vĩnh viễn - Permanent Negative Completion reply
        //Lỗi vĩnh viễn (không nên thử lại)
        public const int CODE_COMMAND_UNRECOGNIZED = 500;//Lỗi cú pháp - client gửi lệnh không tồn tại hay sai định dạng.
        public const int CODE_SYNTAX_ERROR = 501;//Lỗi cú pháp tham số - gửi đúng lệnh nhưng sai định dạng tham số
        public const int CODE_COMMAND_NOT_IMPLEMENTED = 502;//Các lệnh FTP mà server chưa hỗ trợ
        public const int CODE_BAD_SEQUENCE = 503;//Trình tự gửi lệnh không đúng
        public const int CODE_NOT_LOGGED_IN = 530;//Chưa đăng nhập - thao tác nhưng chưa có đăng nhập
        public const int CODE_FILE_UNAVAILABLE = 550;//Thao tác không thực hiện - file không tồn tại, không có quyền
        public const int CODE_ACTION_ABORTED = 551;//Thao tác bị hủy, loại không xác định
        public const int CODE_FILE_NAME_NOT_ALLOWED = 553;//Tên file không được chấp nhận
        #endregion
        #region Cài đặt mặc định
        public const int DEFAULT_PORT = 21;//Cổng lắng nghe mặc định
        public const int DEFAULT_BUFFER_SIZE = 8192;//Kích thước buffer cho đọc/ghi file (8KB).
        public const int DEFAULT_TIMEOUT = 300;//Thời gian timeout mặc định (300 giây = 5 phút) - Nếu client không hoạt động trong 5 phút, server sẽ ngắt kết nối.
        public const string DEFAULT_ROOT_FOLDER = @"C:\FTPRoot";//Thư mục gốc mặc định của FTP server - Tất cả file và thư mục FTP sẽ được lưu trong đây
        #endregion
        #region Thông điệp FTP
        public const string MSG_SERVICE_READY = "220 FTP Server Ready\r\n";//Thông điệp chào mừng cho client mới kết nối
        public const string MSG_GOODBYE = "221 Goodbye\r\n";//Thông điệp khi client ngắt kết nối
        public const string MSG_LOGGED_IN = "230 User logged in\r\n";//Client đăng nhập thành công
        public const string MSG_NEED_PASSWORD = "331 Password required\r\n";//Yêu cầu mật khẩu sau khi nhập username
        public const string MSG_LOGIN_FAILED = "530 Login incorrect\r\n";//Đăng nhập thất bại
        public const string MSG_COMMAND_OK = "200 Command okay\r\n";//Thực hiện lệnh thành công
        public const string MSG_COMMAND_UNKNOWN = "500 Command not recognized\r\n";//Không nhận diện được lệnh
        #endregion
    }
}
