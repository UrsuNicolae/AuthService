using System.Reflection.Metadata.Ecma335;

namespace AuthService.Models
{
    public class ErrorModel
    {
        public bool Success { get; set; }

        public string Error { get; set; }
    }
}
