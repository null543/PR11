using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ISProjectSchool.Classes;
using ISProjectSchool.Models;
using System.Data;




namespace ISProjectSchool.DataBase
{
    public class DatabaseHelper
    {
        SqlConnection sqlConnection = new SqlConnection("Server=WIN-N16HPOIVC32;Database=SchoolDB;Integrated Security=True;");


        public void openConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }

        public void closeConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public SqlConnection getConnection()
        {
            return sqlConnection;
        }
        private static string connectionString = "Server=WIN-N16HPOIVC32;Database=SchoolDB;Integrated Security=True;";

        public static User AuthenticateUser(string login, string passwordHash)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT u.UserID, u.FirstName, u.LastName, u.MiddleName, u.RoleID, r.RoleName 
            FROM Users u
            INNER JOIN Roles r ON u.RoleID = r.RoleID
            WHERE u.Email = @login AND u.Password = @passwordHash";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                MiddleName = reader.GetString(reader.GetOrdinal("MiddleName")),
                                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                Login = login
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static List<Student> LoadStudentsData()
        {
            List<Student> students = new List<Student>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT u.FirstName, u.LastName, u.MiddleName, u.Email, g.GroupName, u.UserID
FROM Users u
INNER JOIN Students s ON u.UserID = s.UserID
INNER JOIN Groups g ON s.GroupID = g.GroupID
";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Создание объекта Student и заполнение его данными из результата запроса
                            Student student = new Student
                            {
                                FirstName = reader.GetString(0),
                                LastName = reader.GetString(1),
                                MiddleName = reader.GetString(2),
                                Login = reader.GetString(3),
                                GroupID = reader.GetString(4),
                                UserID = reader.GetInt32(5)
                            };
                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }

        public bool DeleteUserandStudent(string iduser)
        {
            string studentdelete = "DELETE FROM [dbo].[Students] WHERE UserID = @ID";
            SqlCommand studentDeleteCommand = new SqlCommand(studentdelete, getConnection());
            studentDeleteCommand.Parameters.AddWithValue("@ID", iduser);
            openConnection();
            studentDeleteCommand.ExecuteNonQuery();
            closeConnection();

            string userdelete = "DELETE FROM [dbo].[Users] WHERE UserID = @ID";
            SqlCommand userDeleteCommand = new SqlCommand(userdelete, getConnection());
            userDeleteCommand.Parameters.AddWithValue("@ID", iduser);
            openConnection();
            userDeleteCommand.ExecuteNonQuery();
            closeConnection();
            return true;


        }

        public string GetStudentFullName(string iduser)
        {
            string query = "SELECT [LastName], [FirstName], [MiddleName] FROM [dbo].[Users] WHERE UserID = @ID";
            SqlCommand command = new SqlCommand(query, getConnection());
            command.Parameters.AddWithValue("@ID", iduser);

            openConnection();

            string fullName = "";
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read()) // если есть данные
                {
                    string lastName = reader["LastName"].ToString();
                    string firstName = reader["FirstName"].ToString();
                    string middleName = reader["MiddleName"].ToString();
                    fullName = lastName + " " + firstName + " " + middleName;
                }
            }

            closeConnection();
            return fullName;
        }

        public string GetUserFullNameToEmail(string iduser)
        {
            string query = "SELECT [LastName], [FirstName], [MiddleName] FROM [dbo].[Users] WHERE [Email] = @ID";
            SqlCommand command = new SqlCommand(query, getConnection());
            command.Parameters.AddWithValue("@ID", iduser);

            openConnection();

            string fullName = "";
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read()) // если есть данные
                {
                    string lastName = reader["LastName"].ToString();
                    string firstName = reader["FirstName"].ToString();
                    string middleName = reader["MiddleName"].ToString();
                    fullName = firstName + " " + lastName + " " + middleName;
                }
            }

            closeConnection();
            return fullName;
        }

        public string GetUserIDToEmail(string email)
        {
            string query = "SELECT UserID FROM [dbo].[Users] WHERE [Email] = @ID";
            SqlCommand command = new SqlCommand(query, getConnection());
            command.Parameters.AddWithValue("@ID", email);

            openConnection();

            string iduser = "";
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read()) // если есть данные
                {
                    iduser = reader["UserID"].ToString();

                }
            }

            closeConnection();
            return iduser;
        }

        public (string email, string firstName, string middleName) GetUserDetails(string userId)
        {
            string query = "SELECT [Email], [FirstName], [MiddleName] FROM [dbo].[Users] WHERE UserID = @ID";
            SqlCommand command = new SqlCommand(query, getConnection());
            command.Parameters.AddWithValue("@ID", userId);

            openConnection();

            string email = "";
            string firstName = "";
            string middleName = "";

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read()) // если есть данные
                {
                    email = reader["Email"].ToString();
                    firstName = reader["FirstName"].ToString();
                    middleName = reader["MiddleName"].ToString();
                }
            }

            closeConnection();

            return (email, firstName, middleName);
        }

        public static List<Teachers> LoadTeachersData()
        {
            List<Teachers> students = new List<Teachers>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
          WITH TeacherSubjects AS (
    SELECT 
        t.UserID,
        STRING_AGG(s.SubjectName, CHAR(13) + CHAR(10)) WITHIN GROUP (ORDER BY s.SubjectName) AS Subjects
    FROM 
        [dbo].[Teachers] t
        JOIN [dbo].[SubjectTeacher] st ON t.TeacherID = st.TeacherID
        JOIN [dbo].[Subjects] s ON st.SubjectID = s.SubjectID
    GROUP BY 
        t.UserID
),
TeacherGroups AS (
    SELECT 
        t.UserID,
        STRING_AGG(g.GroupName, CHAR(13) + CHAR(10)) WITHIN GROUP (ORDER BY g.GroupName) AS Groups
    FROM 
        [dbo].[Teachers] t
        JOIN [dbo].[TeacherGroups] tg ON t.TeacherID = tg.TeacherID
        JOIN [dbo].[Groups] g ON tg.GroupID = g.GroupID
    GROUP BY 
        t.UserID
)
SELECT 
    u.UserID,
    u.FirstName, 
    u.LastName, 
    u.MiddleName, 
    u.Email, 
    ts.Subjects,
    tg.Groups
FROM 
    [dbo].[Users] u
    JOIN [dbo].[Teachers] t ON u.UserID = t.UserID
    LEFT JOIN TeacherSubjects ts ON t.UserID = ts.UserID
    LEFT JOIN TeacherGroups tg ON t.UserID = tg.UserID
WHERE 
    u.RoleID = 2


";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Создание объекта Student и заполнение его данными из результата запроса
                            Teachers student = new Teachers
                            {
                                UserID = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                MiddleName = reader.GetString(3),
                                Login = reader.GetString(4),
                                SubjectsID = reader.GetString(5),
                                GroupsID = reader.GetString(6)
                            };
                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }



        public void DeleteUserAndRelatedData(string userIdToDelete)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Определить TeacherID по UserID из таблицы Teachers
                        int teacherIdToDelete = GetTeacherIdByUserId(connection, transaction, userIdToDelete);

                        // Если удаление учителя не найдено, прервать операцию
                        if (teacherIdToDelete == -1)
                        {
                            throw new Exception("Учитель с указанным UserID не найден.");
                        }

                        // Удалить записи из таблицы TeacherGroups, связанные с учителем
                        DeleteRecordsFromTable(connection, transaction, "TeacherGroups", "TeacherID", teacherIdToDelete);

                        // Удалить записи из таблицы SubjectTeacher, связанные с учителем
                        DeleteRecordsFromTable(connection, transaction, "SubjectTeacher", "TeacherID", teacherIdToDelete);

                        // Удалить записи из таблицы Teachers
                        DeleteRecordsFromTable(connection, transaction, "Teachers", "UserID", Convert.ToInt32(userIdToDelete));

                        // Удалить записи из таблицы Users
                        DeleteRecordsFromTable(connection, transaction, "Users", "UserID", Convert.ToInt32(userIdToDelete));

                        // Завершить транзакцию
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // В случае ошибки откатить транзакцию
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void EditUserAndRelatedData(string userIdToUpdate, Dictionary<string, string> updatedUserData, List<int> updatedGroupIds, List<int> updatedSubjectIds)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Определить TeacherID по UserID из таблицы Teachers
                        int teacherIdToUpdate = GetTeacherIdByUserId(connection, transaction, userIdToUpdate);

                        // Если учитель для обновления не найден, прервать операцию
                        if (teacherIdToUpdate == -1)
                        {
                            throw new Exception("Учитель с указанным UserID не найден.");
                        }

                        // Обновить записи в таблице Users
                        UpdateUserTable(connection, transaction, userIdToUpdate, updatedUserData);

                        // Обновить записи в таблице Teachers, если есть изменения
                        // (пример: UpdateTeacherTable(connection, transaction, teacherIdToUpdate, updatedTeacherData);)

                        // Удалить старые записи из таблицы TeacherGroups, связанные с учителем
                        DeleteRecordsFromTable(connection, transaction, "TeacherGroups", "TeacherID", teacherIdToUpdate);

                        // Добавить новые записи в таблицу TeacherGroups
                        InsertUpdatedGroups(connection, transaction, teacherIdToUpdate, updatedGroupIds);

                        // Удалить старые записи из таблицы SubjectTeacher, связанные с учителем
                        DeleteRecordsFromTable(connection, transaction, "SubjectTeacher", "TeacherID", teacherIdToUpdate);

                        // Добавить новые записи в таблицу SubjectTeacher
                        InsertUpdatedSubjects(connection, transaction, teacherIdToUpdate, updatedSubjectIds);

                        // Завершить транзакцию
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // В случае ошибки откатить транзакцию
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        private void UpdateUserTable(SqlConnection connection, SqlTransaction transaction, string userId, Dictionary<string, string> updatedUserData)
        {
            string query = "UPDATE Users SET ";
            foreach (var kvp in updatedUserData)
            {
                query += $"{kvp.Key} = @{kvp.Key}, ";
            }
            query = query.TrimEnd(' ', ','); // Удаляем последнюю запятую и пробел

            query += " WHERE UserID = @UserID";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                foreach (var kvp in updatedUserData)
                {
                    command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                }
                command.Parameters.AddWithValue("@UserID", userId);

                command.ExecuteNonQuery();
            }
        }

        private void DeleteRecordsFromTable(SqlConnection connection, SqlTransaction transaction, string tableName, string columnName, object value)
        {
            string query = $"DELETE FROM {tableName} WHERE {columnName} = @{columnName}";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue($"@{columnName}", value);

                command.ExecuteNonQuery();
            }
        }

        private void InsertUpdatedGroups(SqlConnection connection, SqlTransaction transaction, int teacherId, List<int> groupIds)
        {
            string query = "INSERT INTO TeacherGroups (TeacherID, GroupID) VALUES (@TeacherID, @GroupID)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@TeacherID", teacherId);

                foreach (int groupId in groupIds)
                {
                    command.Parameters.AddWithValue("@GroupID", groupId);
                    command.ExecuteNonQuery();
                    command.Parameters.RemoveAt("@GroupID"); // Удаление параметра для следующей итерации
                }
            }
        }

        private void InsertUpdatedSubjects(SqlConnection connection, SqlTransaction transaction, int teacherId, List<int> subjectIds)
        {
            string query = "INSERT INTO SubjectTeacher (TeacherID, SubjectID) VALUES (@TeacherID, @SubjectID)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@TeacherID", teacherId);

                foreach (int subjectId in subjectIds)
                {
                    command.Parameters.AddWithValue("@SubjectID", subjectId);
                    command.ExecuteNonQuery();
                    command.Parameters.RemoveAt("@SubjectID"); // Удаление параметра для следующей итерации
                }
            }
        }



        // Метод для получения TeacherID по UserID из таблицы Teachers
        private int GetTeacherIdByUserId(SqlConnection connection, SqlTransaction transaction, string userIdToDelete)
        {
            string query = "SELECT TeacherID FROM Teachers WHERE UserID = @UserID";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@UserID", userIdToDelete);
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return (int)result;
                }
                return -1; // Если не найдено, вернуть -1
            }
        }

        private void DeleteRecordsFromTable(SqlConnection connection, SqlTransaction transaction, string tableName, string columnName, int userIdToDelete)
        {
            using (SqlCommand command = new SqlCommand($"DELETE FROM {tableName} WHERE {columnName} = @UserID", connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userIdToDelete });
                command.ExecuteNonQuery();
            }
        }


       public static List<Groups> LoadGroupsData()
{
    List<Groups> groups = new List<Groups>();
    Dictionary<int, Groups> groupsMap = new Dictionary<int, Groups>();

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        string query = @"
            select g.[GroupID], g.[GroupName], s.[SubjectName]
            from [dbo].[Groups] g
            left join [dbo].[GroupSubjects] gs on g.[GroupID] = gs.[GroupID]
            left join [dbo].[Subjects] s on gs.[SubjectID] = s.[SubjectID]";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int groupId = reader.GetInt32(0);
                    if (!groupsMap.ContainsKey(groupId))
                    {
                        Groups group = new Groups
                        {
                            GroupID = groupId,
                            GroupName = reader.GetString(1),
                            Subjects = new List<string>()
                        };
                        groupsMap.Add(groupId, group);
                        groups.Add(group);
                    }
                    if (!reader.IsDBNull(2))
                    {
                        groupsMap[groupId].Subjects.Add(reader.GetString(2));
                    }
                }
            }
        }
    }

    // Convert list of subjects to comma-separated string
    foreach (var group in groups)
    {
        group.SubjectsString = string.Join(", ", group.Subjects);
    }

    return groups;
}


        public static List<Subjects> LoadSubjectData()
        {
            var subjects = new List<Subjects>();
            string connectionString1 = connectionString.ToString();

            using (SqlConnection connection = new SqlConnection(connectionString1))
            {
                connection.Open();
                string sql = "SELECT SubjectID, SubjectName FROM [dbo].[Subjects]";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var subject = new Subjects
                            {
                                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName"))
                            };
                            subjects.Add(subject);
                        }
                    }
                }
            }

            return subjects;
        }

        public static List<Subjects> GetTeacherSubjects(string email)
        {
            var subjects = new List<Subjects>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
SELECT sub.SubjectID, sub.SubjectName
FROM [dbo].[Subjects] sub
JOIN [dbo].[SubjectTeacher] st ON sub.SubjectID = st.SubjectID
JOIN [dbo].[Teachers] t ON st.TeacherID = t.TeacherID
JOIN [dbo].[Users] u ON t.UserID = u.UserID
WHERE u.Email = @Email";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var subject = new Subjects
                            {
                                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName"))
                            };
                            subjects.Add(subject);
                        }
                    }
                }
            }

            return subjects;
        }

        public static List<Groups> GetTeacherGroups(string email)
        {
            var groups = new List<Groups>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
SELECT g.GroupID, g.GroupName
FROM [dbo].[Groups] g
JOIN [dbo].[TeacherGroups] tg ON g.GroupID = tg.GroupID
JOIN [dbo].[Teachers] t ON tg.TeacherID = t.TeacherID
JOIN [dbo].[Users] u ON t.UserID = u.UserID
WHERE u.Email = @Email";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var group = new Groups
                            {
                                GroupID = reader.GetInt32(reader.GetOrdinal("GroupID")),
                                GroupName = reader.GetString(reader.GetOrdinal("GroupName"))
                            };
                            groups.Add(group);
                        }
                    }
                }
            }

            return groups;
        }

        public static List<ScheduleItem> LoadScheduleData()
        {
            var scheduleItems = new List<ScheduleItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Обновленный запрос SELECT, который соответствует вашей структуре таблицы
                string query = @"
SELECT s.StartTime, s.EndTime, subj.SubjectName, u.FirstName + ' ' + u.LastName AS TeacherName, r.RoomName, g.GroupName, s.DayOfWeek
FROM Schedules s
JOIN Subjects subj ON s.SubjectID = subj.SubjectID
JOIN Users u ON s.UserID = u.UserID
JOIN Rooms r ON s.RoomID = r.RoomID
JOIN Groups g ON s.GroupID = g.GroupID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var scheduleItem = new ScheduleItem
                            {
                                StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                                TeacherName = reader.IsDBNull(reader.GetOrdinal("TeacherName")) ? "" : reader.GetString(reader.GetOrdinal("TeacherName")),
                                RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                                GroupName = reader.GetString(reader.GetOrdinal("GroupName")),
                                DayOfWeek = reader.GetString(reader.GetOrdinal("DayOfWeek"))
                            };
                            scheduleItems.Add(scheduleItem);
                        }
                    }
                }
            }

            return scheduleItems;
        }

        public static List<ScheduleItem> GetTodaysLessons()
        {
            var lessons = new List<ScheduleItem>();
            string query = @"
SELECT s.StartTime, s.EndTime, subj.SubjectName, u.FirstName + ' ' + u.LastName AS TeacherName, r.RoomName, g.GroupName, DATENAME(dw, s.StartTime) AS DayOfWeek, ScheduleID
FROM Schedules s
JOIN Subjects subj ON s.SubjectID = subj.SubjectID
JOIN Users u ON s.UserID = u.UserID
JOIN Rooms r ON s.RoomID = r.RoomID
JOIN Groups g ON s.GroupID = g.GroupID
WHERE CAST(s.StartTime AS DATE) = CAST(GETDATE() AS DATE)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var lesson = new ScheduleItem
                            {
                                Date = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                                TeacherName = reader.GetString(reader.GetOrdinal("TeacherName")),
                                RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                                GroupName = reader.GetString(reader.GetOrdinal("GroupName")),
                                DayOfWeek = reader.GetString(reader.GetOrdinal("DayOfWeek")),
                                ScheduleID = reader.GetInt32(reader.GetOrdinal("ScheduleID")).ToString()
                            };
                            lessons.Add(lesson);
                        }
                    }
                }
            }

            return lessons;
        }

        public static List<ScheduleItem> GetLessonDetails(string idlesson)
        {
            var lessons = new List<ScheduleItem>();
            string query = $@"
SELECT s.StartTime, s.EndTime, subj.SubjectName, u.FirstName + ' ' + u.LastName AS TeacherName, r.RoomName, g.GroupName, DATENAME(dw, s.StartTime) AS DayOfWeek, ScheduleID
FROM Schedules s
JOIN Subjects subj ON s.SubjectID = subj.SubjectID
JOIN Users u ON s.UserID = u.UserID
JOIN Rooms r ON s.RoomID = r.RoomID
JOIN Groups g ON s.GroupID = g.GroupID
WHERE [ScheduleID]={idlesson}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var lesson = new ScheduleItem
                            {
                                Date = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                                TeacherName = reader.GetString(reader.GetOrdinal("TeacherName")),
                                RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                                GroupName = reader.GetString(reader.GetOrdinal("GroupName")),
                                DayOfWeek = reader.GetString(reader.GetOrdinal("DayOfWeek")),
                                ScheduleID = reader.GetInt32(reader.GetOrdinal("ScheduleID")).ToString()
                            };
                            lessons.Add(lesson);
                        }
                    }
                }
            }

            return lessons;
        }



    }
}