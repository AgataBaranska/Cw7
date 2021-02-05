
using System.ComponentModel.DataAnnotations;

namespace Cw7.DTOs.Requests
{
    public class PromoteStudentsRequest
    {
        [Required(ErrorMessage = "Musisz podać nazwę kierunku")]
        [MaxLength(100)]
        public string Studies { get; set; }


        [Required(ErrorMessage = "Musisz podać numer semestru studiów ")]
        public int Semester { get; set; }
    }
}
