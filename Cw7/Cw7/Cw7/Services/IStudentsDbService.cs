using Cw7.DTOs.Requests;
using Cw7.DTOs.Responses;


namespace Cw7.Services
{
    public interface IStudentsDbService
    {
        public StudentsEnrollmentResponse StartEnrollStudent(EnrollStudentRequest request);

        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request);

    }
}
