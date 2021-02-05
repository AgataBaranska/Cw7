using System;
using System.ComponentModel.DataAnnotations;


namespace Cw7.DTOs.Requests
{
    public class EnrollStudentRequest
    {

        [RegularExpression("^s[\\d]+$")]
        [Required(ErrorMessage = "Musisz podać numer indeksu")]
        [MaxLength(100)]
        public string IndexNumber { get; set; }

        [Required(ErrorMessage = "Muszisz podać imie")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwisko")]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Musisz podać datę urodzenia")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Musisz podać kierunek studiów")]
        public string Studies { get; set; }//studies name
    }
}
