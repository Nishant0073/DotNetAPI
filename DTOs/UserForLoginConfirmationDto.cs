namespace DotnetAPI.DTOs
{
    partial class UserForLoginDto
    {
        public Byte[] PasswordHash{ get; set; } = [];
        public Byte[] PasswordSalt{ get; set; } = [];
    }
    
}