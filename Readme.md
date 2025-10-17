CẤU TRÚC CÂY THƯ MỤC CHO BÀI TẬP NHÓM
FTP_Solution/
├── FTP.Server/      (Windows Forms App) 
├── FTP.Client/      (Windows Forms App)
└── 	/        (Class Library)
FTP.Server, FTP.Client, FTP.Core là một project riêng trong solution
- FTP.Core: Chứa các logic chung, models (lớp dữ liệu), và các hằng số giao thức. Project này có thể được cả Server và Client sử dụng lại, tránh việc lặp lại mã nguồn.
- FTP.Server: Tập trung vào việc xử lý logic của server và giao diện người dùng của server. Nó sẽ tham chiếu đến FPT.Core.
- FTP.Client: Tập trung vào việc xử lý logic của client và giao diện người dùng của client. Nó cũng sẽ tham chiếu đến FTP.Core.
--> Dễ mở rông chức năng và sửa lỗi.
📁 FTP.Core/
│
├── 📁 Models/                   # Chứa các lớp dữ liệu (Data Transfer Objects - DTOs)
│   ├── 📄 User.cs               # Thông tin user (Username, PasswordHash)
│   ├── 📄 UserSession.cs        # Thông tin phiên làm việc của client
│   └── 📄 FileSystemItem.cs     # Đại diện cho một file hoặc thư mục trên server
│
├── 📁 Protocols/                # Chứa các hằng số và enums liên quan đến giao thức FTP
│   ├── 📄 FtpCommands.cs        # Chứa các hằng số cho lệnh FTP (USER, PASS, STOR, ...)
│   └── 📄 FtpResponseCodes.cs    # Chứa các hằng số cho mã phản hồi FTP (220, 230, 550, ...)
│
└── 📁 Common/                   # Chứa các lớp tiện ích dùng chung
    └── 📄 PasswordHasher.cs      # Lớp để hash và xác minh mật khẩu

📁 FTP.Server/
│
├── 📁 Forms/                    # Chứa các Form giao diện
│   ├── 📄 FtpServerForm.cs      # Form chính của server, hiển thị logs, clients
│   └── 📄 SettingsForm.cs       # Form cấu hình server (port, thư mục root)
│
├── 📁 Modules/                  # Chứa các module xử lý logic chính của server
│   │
│   ├── 📁 Authentication/        # Module xác thực người dùng
│   │   └── 📄 AuthenticationService.cs # Xử lý các lệnh USER, PASS
│   │
│   ├── 📁 CommandProcessor/      # Module xử lý và điều phối lệnh
│   │   └── 📄 CommandHandler.cs    # Lớp chính xử lý các lệnh từ client
│   │
│   ├── 📁 FileManager/           # Module quản lý file
│   │   └── 📄 FileTransferService.cs # Xử lý các lệnh LIST, RETR, STOR
│   │
│   └── 📁 SessionManagement/     # Module quản lý phiên
│       └── 📄 SessionManager.cs    # Quản lý các kết nối của client
│
├── 📁 Network/                  # Chứa các lớp xử lý mạng
│   └── 📄 ServerListener.cs       # Lớp lắng nghe và chấp nhận kết nối từ client
│
└── 📄 Program.cs                 # Điểm khởi đầu của ứng dụng

📁 FTP.Client/
│
├── 📁 Forms/                    # Chứa các Form giao diện
│   ├── 📄 FtpClientForm.cs      # Form chính của client, hiển thị danh sách file
│   └── 📄 ConnectionForm.cs     # Form nhập thông tin kết nối (host, user, pass)
│
├── 📁 Services/                 # Chứa các lớp xử lý logic của client
│   └── 📄 FtpClientService.cs    # Lớp xử lý kết nối, gửi lệnh và truyền file
│
└── 📄 Program.cs                 # Điểm khởi đầu của ứng dụng


