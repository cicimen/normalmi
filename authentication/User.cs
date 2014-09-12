using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication
{
    public class User
    {
            public int ID { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Password { get; set; }
            public DateTime LastLogin { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime BirthDate { get; set; }
            public string LoginType { get; set; }
            public string About { get; set; }


            public static UserAuthResult Authenticate(string userName, string password, string providerKey)
            {
                string Auth_GetUserByCredentials =
                @"SELECT u.ID,u.Name,u.Surname,u.Email,u.Password,u.About,u.BirthDate,u.DateCreated,u.LastLogin,u.DateUpdated,ul.LoginProvider 
                FROM User AS u
                INNER JOIN UserLogin AS ul 
                ON u.ID = ul.UserID
                WHERE  u.Email = '{1}'

                WHERE  ul.Providerkey = '{1}'";

                string connStr = ConfigurationManager.AppSettings["MasterSQLConnection"];
                SqlDatabase db = new SqlDatabase(connStr);
                UserAuthResult result = new UserAuthResult();
                result.AuthSuccess = false;
                User user = new User();
                string dbPassword = string.Empty;
                try
                {
                    string query = String.Format(Auth_GetUserByCredentials, userName);
                    using (DbCommand command = db.GetSqlStringCommand(query))
                    {

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            if (reader.Read())
                            {
                                //Users.ID,Users.Name,Surname,IsAdmin,IsSuperAdmin,LoginType,Users.ActiveDirectoryDomain,Password
                                user.ID       = int.Parse(reader["ID"].ToString());
                                user.Password = reader["Password"].ToString();
                                user.Name     = reader["Name"].ToString();
                                user.Surname  = reader["Surname"].ToString();
                            }
                            else
                            {
                                result.AuthSuccess = false;
                                result.ErrorMsg = "Username or password is wrong";
                            }
                        }
                    }
                }
                finally
                {
                }


                if (!string.IsNullOrEmpty(password) && user.ID > 0 && password.Equals(user.Password))
                {
                    result.User = user;
                    result.AuthSuccess = true;
                }
                else
                {
                    result.ErrorMsg = "Username or password is wrong";
                }

                return result;

            }


            public static UserLoginResult Login(string organizationCode, string userName, string password)
            {
                UserAuthResult authResult = Authenticate(userName, password,"");
                UserLoginResult loginResult = new UserLoginResult();
                if (authResult.AuthSuccess == true)
                {
                    loginResult.LoginSuccess = true;
                    loginResult.User = authResult.User;
                }
                else
                {
                    loginResult.LoginSuccess = false;
                    loginResult.ErrorMsg = authResult.ErrorMsg;

                }
                return loginResult;
            }

            public bool Update()
            {

                bool success = false;
                string connStr = ConfigurationManager.AppSettings["MasterSQLConnection"];
                SqlDatabase db = new SqlDatabase(connStr);

                // şu an sadece users tablosunu update ediyorum
                // user profilleri SitesToUsers a dokunmadım. TODO 
                try
                {
                    using (DbCommand command = db.GetStoredProcCommand("Auth_UpdateUser"))
                    {
                        db.AddInParameter(command, "@ID", SqlDbType.VarChar, this.ID);
                        db.AddInParameter(command, "@Name", SqlDbType.VarChar, this.Name);
                        db.AddInParameter(command, "@Surname", SqlDbType.VarChar, this.Surname);
                        db.AddInParameter(command, "@Password", SqlDbType.VarChar, this.Password);
                        db.AddInParameter(command, "@LoginType", SqlDbType.VarChar, this.LoginType);
                        db.AddInParameter(command, "@LastLogin", SqlDbType.DateTime, this.LastLogin);


                        int rows = db.ExecuteNonQuery(command);
                        if (rows == 1)
                        {

                            success = true;
                        }
                    }

                }
                finally
                {
                }
                return success;




            }

            public static User GetUser(int userID)
            {
                return new User();
            }



        }

        public class UserAuthResult
        {
            public bool AuthSuccess { get; set; }
            public string ErrorMsg { get; set; }
            public User User { get; set; }
        }

        public class UserLoginResult
        {
            public bool LoginSuccess { get; set; }
            public string ErrorMsg { get; set; }
            public User User { get; set; }

        }

    
}
