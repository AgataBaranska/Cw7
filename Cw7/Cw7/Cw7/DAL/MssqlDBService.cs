using System;
using System.Collections.Generic;

using Cw7.DTOs.Responses;

using Cw7.Models;

using System.Data.SqlClient;

namespace Cw7.DAL
{

    public class MssqlDBService : IDbService
    {

        private const string ConString = "Data Source=DESKTOP-ENIT2G5\\" +
             "SQLEXPRESS;Initial Catalog=S19487;Integrated Security=True";

        public IEnumerable<Student> GetStudents()
        {
            string SelectQuery = "SELECT * FROM Student";
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = SelectQuery;
                con.Open();
                SqlDataReader dr = com.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(ReadToStudent(dr));
                }

            }

            return list;
        }

        private Student ReadToStudent(SqlDataReader dr)
        {
            var st = new Student();
            st.IndexNumber = dr["IndexNumber"].ToString();
            st.FirstName = dr["FirstName"].ToString();
            st.LastName = dr["LastName"].ToString();
            st.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
            st.IdEnrollment = int.Parse(dr["IdEnrollment"].ToString());
            return st;
        }

        public Student GetStudent(string index)
        {
            string SelectStudentQuery = "SELECT * FROM Student WHERE IndexNumber = @index";

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = SelectStudentQuery;
                com.Parameters.AddWithValue("index", index);
                con.Open();
                SqlDataReader dr = com.ExecuteReader();

                if (dr.Read()) { return ReadToStudent(dr); }
                return null;

            }
        }


        public int AddStudent(Student studentToAdd)
        {
            string InsertQuery = $"INSERT INTO Student(IndexNumber, FirstName,LastName,BirthDate, IdEnrollment)" +
                    $" VALUES ( @IndexNumber, @FirstName, @LastName, @Birthdate, @IdEnrollment)";
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = InsertQuery;
                com.Parameters.AddWithValue("IndexNumber", studentToAdd.IndexNumber);
                com.Parameters.AddWithValue("FirstName", studentToAdd.FirstName);
                com.Parameters.AddWithValue("LastName", studentToAdd.LastName);
                com.Parameters.AddWithValue("BirthDate", studentToAdd.BirthDate);
                com.Parameters.AddWithValue("IdEnrollment", studentToAdd.IdEnrollment);

                con.Open();
                return com.ExecuteNonQuery();

            }

        }


        public int RemoveStudent(string index)
        {
            string DeleteQuery = "DELETE FROM Student WHERE IndexNumber = @index";

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = DeleteQuery;
                com.Parameters.AddWithValue("index", index);
                con.Open();
                return com.ExecuteNonQuery();
            }
        }

        public int UpdateStudent(Student studentToUpdate)
        {
            string UpdateQuery = $"UPDATE Student SET  FirstName = @FirstName," +
                $"LastName = @LastName, BirthDate = @BirthDate, IdEnrollment = @IdEnrollment" +
                $" WHERE IndexNumber = @IndexNumber";


            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = UpdateQuery;
                com.Parameters.AddWithValue("IndexNumber", studentToUpdate.IndexNumber);
                com.Parameters.AddWithValue("FirstName", studentToUpdate.FirstName);
                com.Parameters.AddWithValue("LastName", studentToUpdate.LastName);
                com.Parameters.AddWithValue("BirthDate", studentToUpdate.BirthDate);
                com.Parameters.AddWithValue("IdEnrollment", studentToUpdate.IdEnrollment);

                con.Open();
                return com.ExecuteNonQuery();

            }
        }

        public StudentsEnrollmentResponse GetStudentsEnrollment(string index)
        {
            string EnrollmentQuery = "SELECT stu.IndexNumber,stu.FirstName, stu.LastName,BirthDate, s.Name, e.Semester, e.StartDate" +
                                   " FROM Student stu INNER JOIN Enrollment e on stu.IdEnrollment = e.IdEnrollment INNER JOIN" +
                                    " Studies s on e.IdStudy = s.IdStudy WHERE IndexNumber = @index";


            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = EnrollmentQuery;
                com.Parameters.AddWithValue("index", index);
                con.Open();
                SqlDataReader dr = com.ExecuteReader();


                var stEnrollment = new StudentsEnrollmentResponse();
                if (!dr.HasRows) return null;
                while (dr.Read())
                {
                    stEnrollment.IndexNumber = dr["IndexNumber"].ToString();
                    stEnrollment.FirstName = dr["FirstName"].ToString();
                    stEnrollment.LastName = dr["LastName"].ToString();
                    stEnrollment.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
                    stEnrollment.Name = dr["Name"].ToString();
                    stEnrollment.Semester = int.Parse(dr["Semester"].ToString());
                    stEnrollment.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                }

                return stEnrollment;

            }


        }


    }
}
