using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos
{
    public class RegisterDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(4)]
        public string Password { get; set; }
    }
}
