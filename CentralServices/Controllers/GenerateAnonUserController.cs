using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CentralServices.Models.Requests;
using CentralServices.Models.Responces;
using CentralServices.Models;
using System.Web.Http;
using CentralServices.Databases;
using System.Web;

namespace Central.Controllers
{
    public class GenerateAnonUserController : ApiController
    {
        // POST: GenerateAnonUser
        public GenerateAnonUserResponce Post([FromBody] GenerateAnonUserRequest request)
        {
            GenerateAnonUserResponce responce = new GenerateAnonUserResponce();

            if (!RegisterUserController.ValidMac(request.MacAddress))
            {
                responce.Result = "ERROR";
            }
            else
            {
                using (var userDB = new UserDB())
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
                    responce.AuthenticationToken = userDB.AuthenticateUser(user.ID, user.Hash, HttpContext.Current.Request.UserHostAddress);
                }
            }
            return responce;
        }

       
    }
}
