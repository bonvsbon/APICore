using System.Net;
using Microsoft.AspNetCore.Mvc;
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

namespace APICore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizeController : ControllerBase
    {
        AuthorizeModel auth;
        public AuthorizeController(IOptions<StateConfigs> configs)
        {
            auth = new AuthorizeModel(configs);
        }
        [HttpPost]
        public string PostLogin(RequestAuthorizeModel request)
        {
            return auth.Authentication(request.Username, request.Password);
        }
    }
}