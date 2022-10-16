using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;

namespace ProjectX.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        public LoginController(Database db)
        {
            Db = db;
        }

        // POST api/login
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]User body)
        {
            try {
                if (body.password.Length == 0 || body.username.Length == 0) { return new OkObjectResult(false); }
                await Db.Connection.OpenAsync();
                var query = new Login(Db);
                var result = await query.GetPassword(body.username);
    
                var pass = BCrypt.Net.BCrypt.Verify(body.password, result);

                if (result is null || ! pass)
                {
                    // authentication failed          
                    return new OkObjectResult(false);
                }
                else
                {
                    // authentication successful

                    // get userinfo
                    var result2 = await query.FindOneAsyncLoginUser(body.username);

                    return new OkObjectResult(result2);             
                }
            } catch (InvalidCastException e) {
                return new OkObjectResult(false);
            }
        }

        public Database Db { get; }
    }
}