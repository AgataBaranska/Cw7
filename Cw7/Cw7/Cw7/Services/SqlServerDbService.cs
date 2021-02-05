 using Cw7.DTOs.Requests;
using Cw7.DTOs.Responses;
using Cw7.Exceptions;
using System;
using System.Data;
using System.Data.SqlClient;


namespace Cw7.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private const string ConString = "Data Source=DESKTOP-ENIT2G5\\" +
             "SQLEXPRESS;Initial Catalog=S19487;Integrated Security=True;MultipleActiveResultSets=true";

        public StudentsEnrollmentResponse StartEnrollStudent(EnrollStudentRequest request)
        {
            StudentsEnrollmentResponse response = null;
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var trans = con.BeginTransaction();
                com.Transaction = trans;


                try
                {
                    //Czy studia istnieja?
                    com.CommandText = "SELECT IdStudy FROM Studies WHERE Name = @name";
                    com.Parameters.AddWithValue("name", request.Studies);
                    var dr = com.ExecuteReader();

                    if (!dr.Read())
                    {
                        trans.Rollback();
                        throw new EnrollmentException("Dane studia nie istnieją");
                    }

                    int idStudy = int.Parse(dr["IdStudy"].ToString());
                    dr.Close();

                    //Czy index jest unikalny?
                    com.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber =@IndexNumber";
                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);

                    dr = com.ExecuteReader();

                    if (dr.HasRows)
                    {
                        trans.Rollback();
                        throw new EnrollmentException("Podany numer indexu jest już używany");
                    }
                    dr.Close();

                    //Czy jest już taki wpis w tabel Enrollments o danym IdEnrollments dla stówdiów o IdStudy

                    com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdStudy =@IdStudy AND Semester = 1 ORDER BY StartDate DESC";
                    com.Parameters.AddWithValue("IdStudy", idStudy);
                    int idEnrollment;
                    dr = com.ExecuteReader();

                    DateTime startDate = DateTime.Now;
                    if (dr.HasRows)
                    {
                        idEnrollment = (int)dr["IdEnrollment"];

                    }
                    else
                    {
                        //dodajemy wpis do Enorllment
                        //sprawdzamy maxymalny wpis w IdEnrollment
                        dr.Close();

                        com.CommandText = "SELECT Max(IdEnrollment) AS Max  FROM Enrollment";
                        dr = com.ExecuteReader();

                        if (dr.Read())
                        {
                            idEnrollment = (int)dr["Max"] + 1;
                        }
                        else
                        {
                            throw new EnrollmentException("Błąd w czytaniu max enrollment");
                        }
                        dr.Close();

                        com.CommandText = $"INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate) " +
                            $"VAUES (@IdEnrollment,@Semester,@IdStudy,@StartDate)";
                        com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                        com.Parameters.AddWithValue("Semester", "1");
                        com.Parameters.AddWithValue("IdStudy", idStudy);
                        com.Parameters.AddWithValue("StartDate", startDate);

                        com.BeginExecuteNonQuery();


                    }


                    //Dodajemy wpis do Students
                    com.CommandText = $"INSERT INTO Student(IndexNumber, FirstName,LastName,BirthDate, IdEnrollment)" +
                    $" VALUES ( @IndexNumber, @FirstName, @LastName, @Birthdate, @IdEnrollment)";

                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                    com.Parameters.AddWithValue("FirstName", request.FirstName);
                    com.Parameters.AddWithValue("LastName", request.LastName);
                    com.Parameters.AddWithValue("BirthDate", request.BirthDate);
                    com.Parameters.AddWithValue("IdEnrollment", idEnrollment);

                    com.ExecuteNonQuery();

                    response = new StudentsEnrollmentResponse
                    {
                        IndexNumber = request.IndexNumber,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        BirthDate = request.BirthDate,
                        Name = request.Studies,
                        Semester = 1,
                        StartDate = startDate

                    };

                    trans.Commit();


                }
                catch (SqlException e)
                {

                    trans.Rollback();
                    return null;
                }


            }
            return response;

        }



        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request)
        {
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {

                com.Connection = con;
                con.Open();




                //Procedura składowana - treść
                /*
                CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT, @NewSemester INT OUTPUT
                AS
                BEGIN
                DECLARE @IdStudy INT = (SELECT s.IdStudy FROM Studies s JOIN Enrollment e ON
                e.IdStudy = s.IdStudy WHERE s.Name = @Studies AND e.Semester = @Semester)
                IF ( @IdStudy IS NULL)
                BEGIN
                Raiserror  ('Studia o podanej nazwie i semestrze nie istnieją',1,1);
                RETURN;
                END

                DECLARE @NewIdEnrollment INT = (SELECT IdEnrollment FROM Enrollment e JOIN Studies s ON
                                    s.IdStudy = e.IdStudy WHERE e.Semester = (@Semester +1) AND
                                    s.Name = @Studies)
                IF (@NewIdEnrollment IS NULL)
                BEGIN

                INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate)
                VALUES (@@Identity+1,@Semester+1,@IdStudy,CURRENT_TIMESTAMP)
                SET @NewIdEnrollment =(SELECT IdEnrollment FROM Enrollment e JOIN Studies s ON
                                    s.IdStudy = e.IdStudy WHERE e.Semester = (@Semester +1) AND
                                    s.Name = @Studies)
                END

                UPDATE Student SET IdEnrollment = @NewIdEnrollment WHERE IdEnrollment = (SELECT IdEnrollment FROM Enrollment e JOIN Studies s ON
                                    s.IdStudy = e.IdStudy WHERE e.Semester = @Semester AND
                                    s.Name = @Studies);

                SET @NewSemester = @Semester +1;
                RETURN
                END;
                */

                com.CommandText = "PromoteStudents @Studies, @Semester, @NewSemseter OUT ";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.Parameters.AddWithValue("Studies", request.Studies);
                com.Parameters.AddWithValue("Semester", request.Semester);
                var resultParam = new SqlParameter();
                resultParam.ParameterName = "NewSemester";
                resultParam.SqlDbType = SqlDbType.Int;
                resultParam.Direction = ParameterDirection.Output;
                com.Parameters.Add(resultParam);
                int newSemester = -1;
                using (var dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        newSemester = int.Parse(dr["NewSemester"].ToString());
                    }
                }


                return new PromoteStudentsResponse
                {
                    Studies = request.Studies,
                    Semester = (int)newSemester

                };

            }
        }
    }



}
