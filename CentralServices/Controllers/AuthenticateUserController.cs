using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using CentralServices.Models.Requests;
using CentralServices.Models.Responces;
using CentralServices.Models;
using CentralServices.Databases;
using System.Web;

namespace Central.Controllers
{
    public class AuthenticateUserController : ApiController
    {
        // POST: AuthenticateUser
        public AuthenticateUserResponce Post([FromBody] AuthenticateUserRequest request)
        {
            AuthenticateUserResponce responce = new AuthenticateUserResponce();

            if (!RegisterUserController.ValidMac(request.MacAddress))
            {
                responce.Result = "ERROR";
            }
            else
            {
                using (var userDB = new UserDB())
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
                        string token = userDB.AuthenticateUser(user.ID, request.Password, HttpContext.Current.Request.UserHostAddress);
                        if (token == string.Empty)
                        {
                            if(request.Password2 != string.Empty)
                            {
                                if (userDB.UserHasActiveReset(user) != null)
                                {
                                    if (UserDB.ValidPassword(request.Password2) && userDB.ApplyUserReset(user, request.Password, request.Password2))
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
                        responce.AuthenticationToken = userDB.AuthenticateUser(user.ID, request.Password, HttpContext.Current.Request.UserHostAddress);
                        responce.Cosmetics = new CosmeticsGroup();
                        responce.Cosmetics.Deserialize(user.CosmeticsSettings);
                    }
                }
            }
            return responce;
        }
    }
}
