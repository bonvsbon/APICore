using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace APICore.Models {
    public class appSetting {
        readonly IOptions<StateConfigs> _conf;
        public appSetting (IOptions<StateConfigs> config) {
            _conf = config;
        }
        public class StateConfigs {
            public ConnectionString ConnectionStrings { get; set; }
            public Storages StoragePath { get; set; }
            public FTP FtpConfig { get; set; }
            public Ldaps Ldap { get; set; }
            public Resource ResourceUrl { get; set; }
            
            
        }
        public class ConnectionString {
            public string isProd { get; set; }
            public string prod { get; set; }
            public string dev { get; set; }
        }

        public class Storages {
            public string ftpPath { get; set; }
            public string localPath { get; set; }
        }
        public class FTP {
            public string username { get; set; }
            public string password { get; set; }
            public string ftpPath { get; set; }
        }
        public class Ldaps {
            public string server { get; set; }

            public string shortDomainName { get; set; }

        }
        public class Resource {
            public string documentsUrl { get; set; }            
        }
    }
}