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
    public class AuthenticateUserController : ControllerBase
    {
        // POST: api/AuthenticateUser
        [HttpPost]
        public AuthenticateUserResponce Post([FromBody] AuthenticateUserRequest request)
        {
            AuthenticateUserResponce responce = new AuthenticateUserResponce();

            if (!RegisterUserController.ValidMac(request.MacAddress))
            {
                responce.Result = "ERROR";
            }
            else
            {
                using (var userDB = new Databases.UserDB())
                {
                    User user = null;

                    if (!request.LoginName.Contains('@'))
                    {
                        // it's a temp user
                        user = userDB.FindByName(request.LoginName);
                    }
                    else
                    {
                        user = userDB.FindByAddress(request.LoginName);
                    }

                    if (user != null)
                    {
                        string token = userDB.AuthenticateUser(user.ID, request.Password, Request.HttpContext.Connection.RemoteIpAddress.ToString());
                        if (token == string.Empty)
                        {
                            if(request.Password2 != string.Empty)
                            {
                                if (userDB.UserHasActiveReset(user) != null)
                                {
                                    if (Databases.UserDB.ValidPassword(request.Password2) && userDB.ApplyUserReset(user, request.Password, request.Password2))
                                    {
                                        request.Password = request.Password2;
                                    }
                                    else
                                    {
                                        responce.Result = "BAD_RESET";
                                        return responce;
                                    }
                                }
                            }
                        }
                        responce.Result = "OK";
                        responce.UserID = user.ID;
                        responce.UserName = user.Name;
                        responce.AuthenticationToken = userDB.AuthenticateUser(user.ID, request.Password, Request.HttpContext.Connection.RemoteIpAddress.ToString());
                        responce.Cosmetics = new CosmeticsGroup();
                        responce.Cosmetics.Deserialize(user.CosmeticsSettings);
                    }
                }
            }
            return responce;
        }
    }
}
