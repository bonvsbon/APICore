using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;
using System.Text;
using APICore.dbContext;
using Newtonsoft.Json;
using static APICore.Models.appSetting;
using APICore.Common;
using Microsoft.Extensions.Options;

namespace APICore.Models {
    public class AuthorizeModel : ContextBase {
        string strErrMsg = "";
        StateConfigs config;
        public AuthorizeModel (IOptions<StateConfigs> configs) : base (configs) {
            config = configs.Value;
        }

        public string Authentication (string username, string password) {

            string DomainAndUsername = "";
            string strCommu;
            bool flgLogin = false;
            // strCommu = ("LDAP://" +
            //     (config.Ldap.server));
            strCommu = config.Ldap.server;//"LDAP://DC=BAF,DC=CO,DC=LOCAL";
            DomainAndUsername = (config.Ldap.shortDomainName + ("\\" + username));
            DirectoryEntry entry = new DirectoryEntry(strCommu, DomainAndUsername, password);
            object obj;
            //SearchResultCollection res;
            SearchResult res;
            if (entry.Properties.Values.Count == 0) {
                flgLogin = false;
                return "username of password incorrect";
            }
            obj = entry.NativeObject;
            DirectorySearcher search = new DirectorySearcher(entry);
            UserInformationModel response = new UserInformationModel();

            try {
                search.Filter = ("(SAMAccountName=" +
                    (username + ")"));
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("employeeID");
                res = search.FindOne();
                if ((res == null)) {
                    flgLogin = false;
                    return "Please check user / password";
                } else {
                    flgLogin = true;
                }
            } catch (Exception ex) {
                flgLogin = false;
                return ex.Message.ToString() + "Please check user / password";
            }
            if ((flgLogin == true)) {
                StringBuilder sb = new StringBuilder ();
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
            } else {
                strErrMsg = "Password In correct";
            }

            return strErrMsg;
        }

        public string AuthenticationbyPass(string username, string password)
        {
            UserInformationModel response = new UserInformationModel();
            if(username == "zwiz" && password == "securebyNextcapital")
            {
                response.EmployeeCode = "";
                response.EmployeeName = "LineBot";
                response.Token = TokenGenerator.GenerateToken(username);
                response.Username = username;
            }
            else
            {
                return "Invalid Username or Password";
            }

            return JsonConvert.SerializeObject(response);
        }

    }
}