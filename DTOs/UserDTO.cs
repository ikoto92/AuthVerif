namespace AuthVerif.DTOs
{
    public class UserDTO
    {
        public class ResetPasswordRequest
        {
            public required string Email { get; set; }
        }

        public class VerifyCodeRequest
        {
            public required string Code { get; set; }
        }

        public class ResetPasswordRequests
        {
            public required string Code { get; set; }
            public required string NewPassword { get; set; }
        }
    }
}
