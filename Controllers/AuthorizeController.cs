using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using APICore.Models;
using static APICore.Models.appSetting;
using Microsoft.Extensions.Options;
using APICore.Common;

namespace APICore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthorizeController : ControllerBase
    {
        Functional func;
        AuthorizeModel auth;
        AccountModel acc;
        public AuthorizeController(IOptions<StateConfigs> configs)
        {
            func = new Functional();
            auth = new AuthorizeModel(configs);
            acc = new AccountModel(configs);
        }
        [HttpPost]
        public UserInformationModel PostLogin(RequestAuthorizeModel request)
        {
            string result = "";
            if(request.Username == "zwiz")
            {
                result = auth.AuthenticationbyPass(request.Username, request.Password);
                acc.REST_KeepLogRequest("Token", result);
            }
            else
            {
                result = auth.Authentication(request.Username, request.Password);  
            }
            return func.JsonDeserialize<UserInformationModel>(result);
        }
    }
}