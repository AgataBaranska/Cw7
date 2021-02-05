
using Cw7.DAL;
using Cw7.DTOs.Requests;
using Cw7.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cw7.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public IConfiguration Configuration { get; set; }

        public StudentsController(IDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }


        [HttpGet("{index}")]
        [Authorize(Roles = "admin")]
        public IActionResult GetStudent([FromRoute] string index)
        {
            var student = _dbService.GetStudent(index);
            if (student == null) return NotFound($"W bazie nie ma studenta o id: {index}");
            return Ok(student);
        }
        [HttpPost]
        public IActionResult Login(LoginRequest log)
        {
            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,"1"),
                new Claim(ClaimTypes.Name, "jan123"),
                new Claim(ClaimTypes.Role,"admin"),
                new Claim(ClaimTypes.Role, "student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "Gakko",
                audience: "Students",
                claims: claim,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
                );
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            }
                );
        }




        [HttpDelete("{index}")]
        public IActionResult deleteStudent([FromRoute] string index)
        {
            var rowsAffected = _dbService.RemoveStudent(index);
            if (rowsAffected == 0) return NotFound($"Studenta o podanym id: {index} nie ma w bazie");
            return Ok("Usuwanie ukończone");
        }

        [HttpPut("{index}")]
        public IActionResult updateStudent([FromRoute] string index, [FromBody] Student newStudent)
        {

            newStudent.IndexNumber = index;
            var rowsAffected = _dbService.UpdateStudent(newStudent);
            if (rowsAffected == 0) return NotFound($"Student o podanym {index} nie znajduje się w bazie");
            return Ok($"Aktualizacja dokończona");
        }

        /*[HttpPost]
        public IActionResult AddStudent([FromBody] Student student)
        {
            var rowsAffected = _dbService.AddStudent(student);

            if (rowsAffected == 0) return NotFound("Nie dodano studenta do bazy");
            return Ok("Dodano studenta do bazy");
        }

        [HttpGet("enroll/{index}")]//api/students/enroll/index*/

        public IActionResult GetStudentsEnrollment([FromRoute] string index)
        {


            var studentsEnrollment = _dbService.GetStudentsEnrollment(index);
            if (studentsEnrollment == null) return NotFound($"Nie odnaleziono zapisów studenta {index}");
            return Ok(studentsEnrollment);

        }


    }
}
