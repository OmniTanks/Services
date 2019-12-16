using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Central.Models.Requests;
using Central.Models.Responces;

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

            using (var userDB = new Databases.UserDB())
            {
                string id = userDB.FindIDByMacAddress(request.MacAddress);
                if (id != string.Empty)
                {
                    var user = userDB.GetUserByID(id);
                    responce.Result = "OK";
                    responce.UserID = user.ID;
                    responce.UserName = user.Name;
                    responce.AccessKey = user.Hash;
                    responce.AuthenticationToken = "GenTOKEN"; // userDB.NewToken(user);
                }
                else
                {
                    string name = string.Empty;
                    while (name == string.Empty || userDB.NameExists(name))
                    {
                        name = userDB.GenerateName();
                    }

                    responce.UserName = name;
                }
            }
            return responce;
        }
    }
}
