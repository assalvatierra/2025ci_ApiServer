using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Postgres.Repository
{
    public interface IAspNetUserRepo
    {
        // Create
        Task<int> CreateUserAsync(string username, string email, string passwordHash, string? phoneNumber = null);
        
        // Read
        Task<AspNetUser?> GetUserByIdAsync(string userId);
        Task<AspNetUser?> GetUserByUsernameAsync(string username);
        Task<AspNetUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<AspNetUser>> GetAllUsersAsync();
        
        // Update
        Task<bool> UpdateUserAsync(AspNetUser user);
        Task<bool> UpdatePasswordHashAsync(string userId, string newPasswordHash);
        Task<bool> UpdateEmailAsync(string userId, string newEmail);
        
        // Delete
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> SoftDeleteUserAsync(string userId);
        
        // Authentication
        Task<bool> ValidateUserCredentialsAsync(string username, string passwordHash);
        Task<bool> UserExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
    
    public class AspNetUser
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
