using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using APICore.Common;
using APICore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using APICore.Models;
using static APICore.Models.appSetting;
 
namespace Services.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        StateConfigs _state = new StateConfigs();
        Functional _func;
        FileModel _file;
        private bool result;
        public FileController(IOptions<StateConfigs> config)
        {
            _state = config.Value;
            _func = new Functional();
            _file = new FileModel(config);
        }

        [HttpPost]
        public string ReceiveFileFromClient(List<IFormFile> files)
        {
            string fileName = "";
            string savePath = "";
            string localPath = "";
            string targetDirectory = "";
            string fileInput = "";
            string callDate = "";
            string callTime = "";
            FileInfo info;
            List<string> fileformat = new List<string>();

            foreach (IFormFile file in files)
            { 
            
                targetDirectory = _state.StoragePath._localPath;

                fileName = file.FileName;

                localPath = Path.Combine(targetDirectory, fileName);

                if (!_func.CheckExistingDirectory(targetDirectory))
                {
                    _func.DirectoryCreate(targetDirectory);
                }
                if (_func.CheckExistingFile(targetDirectory, fileName))
                {
                    continue;
                    //_func.DestroyFileFromName(targetDirectory, fileName);
                }
                using (var fileStream = new FileStream(localPath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                using (var client = new WebClient())
                {
                    savePath = string.Format("{0}/{1}", _state.FtpConfig._ftpPath, fileName);
                    client.Credentials = new NetworkCredential(_state.FtpConfig._username, _state.FtpConfig._password);
                    client.UploadFile(savePath, WebRequestMethods.Ftp.UploadFile, localPath);
                }

                _func.DestroyFileFromName(targetDirectory, fileName);
                info = _func.GetFileInformation(fileName);
                // Log File for Other Process
                fileformat = fileName.Split('_').ToList();

                fileInput = string.Format("{0}_{1}", fileformat[1], fileformat[2]);

                callDate = DateTime.ParseExact(fileformat[1], "yyMMdd", new CultureInfo("en-US")).ToString("yyyy-MM-dd");
                callTime = string.Format(
                    "{0}:{1}:{2}",
                    string.Concat(fileformat[2][0], fileformat[2][1]),
                    string.Concat(fileformat[2][2], fileformat[2][3]),
                    string.Concat(fileformat[2][4], fileformat[2][5])
                    );



                // _file.REST_InsertFilelog(
                //         FileName : fileInput,
                //         OriginalFile : fileName,
                //         fileSize : info.Length,
                //         PhoneNumber : "0941610031",
                //         CallDate : callDate,
                //         CallTime : callTime,
                //         CreateBy : "0941610031"
                //     );

            }


            return fileName;
        }

    }
}
