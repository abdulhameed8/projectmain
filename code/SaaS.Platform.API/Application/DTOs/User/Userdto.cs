namespace SaaS.Platform.API.Application.DTOs.Users
{
    public class CreateUserdto
    {
        public Guid TenantId { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public string? PasswordSalt { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool? IsActive { get; set; } = true;

        public string? IsEmailVerified { get; set; }

        public string? EmailVerificationToken { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? FailedLoginAttempts { get; set; }

        public DateTime LogOutEndDate { get; set; }

        public string? RefreshToken { get; set; }

        public string? RefreshTokenExpiryTime { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

       
    }

    public class UpdateUserdto
    {
        public Guid TenantId { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public string? PasswordSalt { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool? IsActive { get; set; } = true;

        public string? IsEmailVerified { get; set; }

        public string? EmailVerificationToken { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? FailedLoginAttempts { get; set; }

        public DateTime LogOutEndDate { get; set; }

        public string? RefreshToken { get; set; }

        public string? RefreshTokenExpiryTime { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

        

    }

    // Dto for User Response

    public class  Userdto
    {
        public Guid TenantId { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public string? PasswordSalt { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool? IsActive { get; set; } = true;

        public string? IsEmailVerified { get; set; }

        public string? EmailVerificationToken { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? FailedLoginAttempts { get; set; }

        public DateTime LogOutEndDate { get; set; }

        public string? RefreshToken { get; set; }

        public string? RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

        public  DateTime? ModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

    }
}

    
    
    