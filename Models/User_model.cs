using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;

namespace ProjectX
{
    public class User
    {
        public int iduser { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string bio { get; set; }

        internal Database Db { get; set; }

        public User()
        {
        }

        internal User(Database db)
        {
            Db = db;
        }

        public async Task<List<User>> GetAllAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  user ;";
            var result=await ReturnAllAsync(await cmd.ExecuteReaderAsync());
           // Console.WriteLine(result);
            return await ReturnAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task<User> FindOneAsync(int iduser)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  User  WHERE  iduser  = @iduser";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@iduser",
                DbType = DbType.Int32,
                Value = iduser,
            });
            var result = await ReturnAllAsync(await cmd.ExecuteReaderAsync());
            //Console.WriteLine(result.Count);
            if(result.Count > 0){
                return result[0];
            }
            else {
                return null;
            }
            //return result.Count > 0 ? result[0] : null;
        }


        public async Task DeleteAllAsync()
        {
            using var txn = await Db.Connection.BeginTransactionAsync();
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  user ";
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }
    

        public async Task<int> InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText=@"insert into user(username,password,bio) 
            values(@username,@password,@bio);";
            BindParams(cmd);
            try
            {
                await cmd.ExecuteNonQueryAsync();
                //int lastInsertId = (int) cmd.LastInsertedId; 
                return 1;
            }
            catch (System.Exception)
            {   
                return 0; 
            } 
        }

        public async Task<int> UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            
            cmd.CommandText = @"UPDATE  user  SET  username  = @username,  password  = @password, bio = @bio WHERE  iduser  = @iduser;";
            BindParams(cmd);
            BindId(cmd);
            Console.WriteLine("id="+iduser);
            int returnValue=await cmd.ExecuteNonQueryAsync();
            return returnValue;
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  user  WHERE  iduser  = @iduser;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
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
        
        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@iduser",
                DbType = DbType.Int32,
                Value = iduser,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@username",
                DbType = DbType.String,
                Value = username,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@password",
                DbType = DbType.String,
                Value = password,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@bio",
                DbType = DbType.String,
                Value = bio,
            });
        }
    }
}