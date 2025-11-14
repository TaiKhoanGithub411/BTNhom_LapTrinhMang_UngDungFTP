using FTP.Core.Constants;

namespace FTP.Core.Protocol
{
    //Lớp phục vụ tạo FTP reponse
    public static class FtpResponse
    {
        public static string Create(int code, string message)
        {
            return $"{code} {message}\r\n";
        }
        public static string ServiceReady() => FtpConstants.MSG_SERVICE_READY;
        public static string LoggedIn() => FtpConstants.MSG_LOGGED_IN;
        public static string NeedPassword() => FtpConstants.MSG_NEED_PASSWORD;
        public static string LoginFailed() => FtpConstants.MSG_LOGIN_FAILED;
        public static string CommandOK() => FtpConstants.MSG_COMMAND_OK;
        public static string CommandNotRecognized() => FtpConstants.MSG_COMMAND_UNKNOWN;
        public static string NotLoggedIn() => Create(FtpConstants.CODE_NOT_LOGGED_IN, "Not logged in");
        public static string FileUnavailable(string filename) => Create(FtpConstants.CODE_FILE_UNAVAILABLE, $"File unavailable: {filename}");
    }
}
