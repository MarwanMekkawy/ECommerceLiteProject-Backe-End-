namespace Gateway.API.DTOs
{ 
        // Authentication requests
        public class RegisterRequestDto
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
            public string? ConfirmPassword { get; set; }
            public string? PhoneNumber { get; set; }
        }

        public class LoginRequestDto
        {
            public bool RememberMe { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        // Email requests
        public class ChangeEmailRequestDto
        {
            public string? NewEmail { get; set; }
            public string? Password { get; set; }
        }

        // Password requests
        public class ForgotPasswordRequestDto
        {
            public string? Email { get; set; }
        }

        public class ResetPasswordRequestDto
        {
            public string? NewPassword { get; set; }
            public string? ConfirmPassword { get; set; }
        }

        public class ChangePasswordRequestDto
        {
            public string? CurrentPassword { get; set; }
            public string? NewPassword { get; set; }
            public string? ConfirmNewPassword { get; set; }
        }

        // Current user
        public class UpdateUserRequestDto
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? PhoneNumber { get; set; }
        }

        // Admin requests
        public class ChangeUserRoleRequestDto
        {
            public RoleType Role { get; set; }
        }

        public class PaginationQueryDto
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
        }

        public enum RoleType
        {
            Buyer,
            Admin
        }
}
