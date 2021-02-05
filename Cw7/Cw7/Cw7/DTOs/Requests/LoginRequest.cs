

using System.ComponentModel.DataAnnotations;

namespace Cw7.DTOs.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Musisz podać nazwę użytkownika")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Musisz podać hasło")]
        public string Password { get; set; }
    }
}
