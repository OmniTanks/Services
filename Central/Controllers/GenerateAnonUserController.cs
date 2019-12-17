using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Central.Models.Requests;
using Central.Models.Responces;
using Central.Models;

namespace Central.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GenerateAnonUserController : ControllerBase
    {
        // POST: GenerateAnonUser
        [HttpPost]
        public GenerateAnonUserResponce Post([FromBody] GenerateAnonUserRequest request)
        {
            GenerateAnonUserResponce responce = new GenerateAnonUserResponce();

            if (!RegisterUserController.ValidMac(request.MacAddress))
            {
                responce.Result = "ERROR";
            }
            else
            {
                using (var userDB = new Databases.UserDB())
                {
                    User user = null;
                    string id = userDB.FindIDByAddress(request.MacAddress); // see if we think we know who they are
                    if (id != string.Empty)
                        user = userDB.GetUserByID(id);
                    else
                        user = userDB.CreateTemporaryUser(request.MacAddress);

                    responce.Result = "OK";
                    responce.UserID = user.ID;
                    responce.UserName = user.Name;
                    responce.AccessKey = user.Hash;
                    responce.AuthenticationToken = userDB.AuthenticateUser(user.ID, user.Hash, Request.HttpContext.Connection.RemoteIpAddress.ToString());
                }
            }
            return responce;
        }

       
    }
}
