using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;

namespace ProjectX
{
    public class Login
    {  
         public int? iduser { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }

        internal Database? Db { get; set; }

        public Login()
        {
        }

        internal Login(Database db)
        {
            Db = db;
        }

        public async Task<string> GetPassword(string username)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT  iduser, password  FROM  user  WHERE  username  = @username";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@username", 
                DbType = DbType.String,
                Value =  username,
            });

            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@iduser", 
                DbType = DbType.Int32,
                Value =  iduser,
            });

            var result = await ReturnPassword(await cmd.ExecuteReaderAsync());
            return result;
        }

        private async Task<string> ReturnPassword(DbDataReader reader)
        {
            var loginUser = new Login();
            using (reader)
            {
                int iduser;
                while (await reader.ReadAsync())
                {
                    var user = new Login(Db)
                    {
                        iduser = reader.GetInt32(0),
                        password = reader.GetString(1) 
                    };
                    loginUser=user;
                }
            }
            return loginUser.password;
        }

        public async Task<User> FindOneAsyncLoginUser(string username)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  User  WHERE  username = @username";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@username", 
                DbType = DbType.String,
                Value =  username,
            });

            var result = await ReturnAllAsync(await cmd.ExecuteReaderAsync());
            
            if(result.Count > 0){
                return result[0];
            }
            else {
                return null;
            }
        }        
        
        private async Task<List<User>> ReturnAllAsync(DbDataReader reader)
        {
            var posts = new List<User>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new User(Db)
                    {
                        iduser = reader.GetInt32(0),
                        username = reader.GetString(1),
                        password = reader.GetString(2),
                        bio = reader.GetString(3),
                    };
                    posts.Add(post);
                }
            }
            return posts;
        }

    }
}