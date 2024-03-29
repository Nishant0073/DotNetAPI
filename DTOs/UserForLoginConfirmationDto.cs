namespace DotnetAPI.DTOs
{
    public partial class UserForLoginConfirmation
    {
        public Byte[] PasswordHash{ get; set; } = [];
        public Byte[] PasswordSalt{ get; set; } = [];
    }
    
}