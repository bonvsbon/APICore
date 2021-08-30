using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using APICore.Models;
using APICore.Common;

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
            // List<string> headerAuth = new List<string>();
            // headerAuth = context.HttpContext.Request.Headers["Authorization"].ToList();
            // string currentToken = "";
            // string validationToken = "";
            // if(headerAuth.Count > 0)
            // {
            //     currentToken = context.HttpContext.Request.Headers["Authorization"][0].ToString();
            //     validationToken = TokenGenerator.ValidateJwtToken(currentToken);
            // }
            if (account == null)
            {
                // not logged in
                context.Result = new JsonResult(func.ResponseWithUnAuthorize());
            }
        }
    }
}