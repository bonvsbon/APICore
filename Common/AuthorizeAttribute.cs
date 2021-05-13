using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using APICore.Models;

namespace APICore.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        Functional func;
        public AuthorizeAttribute()
        {
            func = new Functional();
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var account = context.HttpContext.Items["nameid"];
            if (account == null)
            {
                // not logged in
                context.Result = new JsonResult(ResponseModel.ResponseWithUnAuthorize());
            }
        }
    }
}