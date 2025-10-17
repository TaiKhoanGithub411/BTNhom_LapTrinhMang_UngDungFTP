# CẤU TRÚC DỰ ÁN FTP SERVER & CLIENT

## Tổng quan

Dự án được xây dựng theo kiến trúc 3-project trong cùng một Solution để tách biệt rõ ràng các thành phần chức năng:

-   **`FTP.Core` (Class Library)**: Chứa các logic chung, models (lớp dữ liệu), và các hằng số giao thức. Project này được cả Server và Client sử dụng lại, tránh việc lặp lại mã nguồn.
-   **`FTP.Server` (Windows Forms App)**: Tập trung vào việc xử lý logic và giao diện người dùng của phía Server.
-   **`FTP.Client` (Windows Forms App)**: Tập trung vào việc xử lý logic và giao diện người dùng của phía Client.

*Thiết kế này giúp việc mở rộng chức năng và sửa lỗi trở nên dễ dàng và có tổ chức hơn.*

---

## Chi tiết cấu trúc cây thư mục

### 📁 `FTP.Core`

Thư viện lõi chứa các thành phần dùng chung.

-   **`Models/`** `# Chứa các lớp dữ liệu (Data Transfer Objects - DTOs)`
    -   `User.cs` `# Thông tin user (Username, PasswordHash)`
    -   `UserSession.cs` `# Thông tin phiên làm việc của client`
    -   `FileSystemItem.cs` `# Đại diện cho một file hoặc thư mục trên server`
-   **`Protocols/`** `# Chứa các hằng số và enums liên quan đến giao thức FTP`
    -   `FtpCommands.cs` `# Chứa các hằng số cho lệnh FTP (USER, PASS, STOR, ...)`
    -   `FtpResponseCodes.cs` `# Chứa các hằng số cho mã phản hồi FTP (220, 230, 550, ...)`
-   **`Common/`** `# Chứa các lớp tiện ích dùng chung`
    -   `PasswordHasher.cs` `# Lớp để hash và xác minh mật khẩu`

### 📁 `FTP.Server`

Ứng dụng FTP Server.

-   **`Forms/`** `# Chứa các Form giao diện`
    -   `FtpServerForm.cs` `# Form chính của server, hiển thị logs, clients`
    -   `SettingsForm.cs` `# Form cấu hình server (port, thư mục root)`
-   **`Modules/`** `# Chứa các module xử lý logic chính của server`
    -   **`Authentication/`** `# Module xác thực người dùng`
        -   `AuthenticationService.cs` `# Xử lý các lệnh USER, PASS`
    -   **`CommandProcessor/`** `# Module xử lý và điều phối lệnh`
        -   `CommandHandler.cs` `# Lớp chính xử lý các lệnh từ client`
    -   **`FileManager/`** `# Module quản lý file`
        -   `FileTransferService.cs` `# Xử lý các lệnh LIST, RETR, STOR`
    -   **`SessionManagement/`** `# Module quản lý phiên`
        -   `SessionManager.cs` `# Quản lý các kết nối của client`
-   **`Network/`** `# Chứa các lớp xử lý mạng`
    -   `ServerListener.cs` `# Lớp lắng nghe và chấp nhận kết nối từ client`
-   `Program.cs` `# Điểm khởi đầu của ứng dụng`

### 📁 `FTP.Client`

Ứng dụng FTP Client.

-   **`Forms/`** `# Chứa các Form giao diện`
    -   `FtpClientForm.cs` `# Form chính của client, hiển thị danh sách file`
    -   `ConnectionForm.cs` `# Form nhập thông tin kết nối (host, user, pass)`
-   **`Services/`** `# Chứa các lớp xử lý logic của client`
    -   `FtpClientService.cs` `# Lớp xử lý kết nối, gửi lệnh và truyền file`
-   `Program.cs` `# Điểm khởi đầu của ứng dụng`

