using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;
using Newtonsoft.Json;
using System.Text;
using APICore.dbContext;
using static APICore.Models.appSetting;
using Microsoft.Extensions.Options;
using APICore.Common;

namespace APICore.Models
{
    public class AuthorizeModel : ContextBase
    {
        string strErrMsg = "";
        StateConfigs config;
        public AuthorizeModel(IOptions<StateConfigs> configs) : base (configs)
        {
            config = configs.Value;
        }

        public string Authentication(string username, string password){

            string DomainAndUsername = "";
            string strCommu;
            bool flgLogin = false;
            strCommu = ("LDAP://"
                        + (config.Ldap._server));
            DomainAndUsername = (config.Ldap._shortDomainName + ("\\" + username));
            DirectoryEntry entry = new DirectoryEntry(strCommu, DomainAndUsername, password);
            object obj;
            // SearchResultCollection result;
            SearchResult res;
            if (entry.Properties.Values.Count == 0)
            {
                flgLogin = false;
                return "username of password incorrect";
            }
            obj = entry.NativeObject;
            DirectorySearcher search = new DirectorySearcher(entry);
            UserInformationModel response = new UserInformationModel();
            
            try
            {
                search.Filter = ("(SAMAccountName="
                            + (username + ")"));
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("employeeID");
                res = search.FindOne();
                if ((res == null))
                {
                    flgLogin = false;
                    return "Please check user / password";
                }
                else
                {
                    flgLogin = true;
                }
            }
            catch (Exception ex)
            {
                flgLogin = false;
                return ex.Message.ToString() + "Please check user / password";
            }
            if ((flgLogin == true))
            {
                StringBuilder sb = new StringBuilder();
                res = search.FindOne();
                DirectoryEntry de = res.GetDirectoryEntry();

                /*
                   * cn => CustomerName
                   * sn => SurName
                   * title => Department
                   * EmployeeID
                */

                response.EmployeeCode = de.Properties["employeeID"].Value != null ? de.Properties["employeeID"].Value.ToString() : "";
                response.EmployeeName = username;
                response.Token = TokenGenerator.GenerateToken(username);
                response.Username = username;
                return JsonConvert.SerializeObject(response);
                //return "OK";
            }
            else
            {
                strErrMsg = "Password In correct";
            }

            return strErrMsg;
        }
    }
}