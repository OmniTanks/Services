using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using CentralServices.Models;
using CentralServices.Models.Requests;
using CentralServices.Models.Responces;
using CentralServices.Databases;
using System.Web;

namespace Central.Controllers
{
    public class RegisterUserController : ApiController
    {
        // POST:RegisterUser
        public RegisterUserResponce Post([FromBody] RegisterUserRequest request)
        {
            RegisterUserResponce responce = new RegisterUserResponce();

            if (!ValidMac(request.MacAddress))
            {
                responce.Result = "ERROR";
            }
            else
            {
                using (var userDB = new UserDB())
                {
                    User user = null;
                    string id = userDB.FindIDByAddress(request.Email); // see if we think we know who they are
                    if (id != string.Empty)
                        responce.Result = "INVALID";
                    else if (!UserDB.ValidPassword(request.Password))
                        responce.Result = "BAD_CRED";
                    else if (!UserDB.ValidName(request.DesiredName))
                        responce.Result = "BAD_NAME";
                    else
                    {
                        user = userDB.FindByName(request.DesiredName);

                        if (user != null)
                        {
                            if (user.Temporary != 0 && request.TemporyID != string.Empty)
                            {
                                if (user.ID != request.TemporyID || user.Hash != request.TemporyID || user.Email != request.MacAddress)
                                {
                                    responce.Result = "BAD_TEMP_CONVERT";
                                    return responce;
                                }

                                user = userDB.ConvertUser(user, request);
                            }
                            else
                            {
                                responce.Result = "BAD_NAME";
                                return responce;
                            }
                        }
                        else
                        {
                            user = userDB.CreateUser(request.DesiredName,request.Email,request.Password);
                        }

                        responce.Result = "OK";
                        responce.UserID = user.ID;
                        responce.UserName = user.Name;
                        responce.AuthenticationToken = userDB.AuthenticateUser(user.ID, request.Password, HttpContext.Current.Request.UserHostAddress);
                        responce.Cosmetics = new CosmeticsGroup();
                        responce.Cosmetics.Deserialize(user.CosmeticsSettings);
                       // Mailer.SendRegistrationEmail(user);
                    }
                }
            }

            return responce;
        }

        public static bool ValidMac(string mac)
        {
            string[] parts = mac.Split('-');
            if (parts.Length != 6)
                return false;

            foreach (var part in parts)
            {
                if (!byte.TryParse(part, System.Globalization.NumberStyles.HexNumber, null, out byte b))
                    return false;
            }

            return true;
        }
    }
}
