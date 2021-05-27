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
        public string ReceiveFileFromClient(List<IFormFile> files, string username)
        {
            string fileName = "";
            string savePath = "";
            string localPath = "";
            string targetDirectory = "";
            string fileInput = "";
            string callDate = "";
            string callTime = "";
            string strTelephone = "";
            string fileforInsert = "";
            FileInfo info;
            List<string> fileformat = new List<string>();
            List<string> strInputFile = new List<string>();
            List<string> strOriginFile = new List<string>();
            List<string> strPhone = new List<string>();
            foreach (IFormFile file in files)
            { 
                try{
                    targetDirectory = _state.StoragePath.localPath;

                    fileName = file.FileName;

                    localPath = Path.Combine(targetDirectory, fileName);

                    fileformat = fileName.Split('_').ToList();
                    strPhone = fileformat[0].Split(" ").ToList();
                    callDate = DateTime.ParseExact(fileformat[1], "yyMMdd", new CultureInfo("en-US")).ToString("yyyy-MM-dd");
                    callTime = string.Format(
                        "{0}:{1}:{2}",
                        string.Concat(fileformat[2][0], fileformat[2][1]),
                        string.Concat(fileformat[2][2], fileformat[2][3]),
                        string.Concat(fileformat[2][4], fileformat[2][5])
                        );

                    if(strPhone.Count > 2)
                    {
                        for(int i = 1; i < strPhone.Count; i++)
                        {
                            if(!string.IsNullOrEmpty(strTelephone)) strTelephone += " ";
                            strTelephone += strPhone[i];
                        }
                    }
                    else
                    {
                        strTelephone = strPhone[1];
                    }

                    fileInput = string.Format("{0}_{1}", fileformat[1], fileformat[2]);

                    strInputFile = fileInput.Split(".").ToList();


                    if (!_func.CheckExistingDirectory(targetDirectory))
                    {
                        _func.DirectoryCreate(targetDirectory);
                    }
                    if (_func.CheckExistingFile(targetDirectory, fileName))
                    {
                        _func.DestroyFileFromName(targetDirectory, fileName);
                        // continue;
                    }
                    using (var fileStream = new FileStream(localPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    using (var client = new WebClient())
                    {
                        fileforInsert = string.Format("{0}_{1}_{2}", username, strTelephone, strInputFile[0]);
                        savePath = string.Format("{0}/{1}.m4a", _state.FtpConfig.ftpPath, fileforInsert);
                        client.Credentials = new NetworkCredential(_state.FtpConfig.username, _state.FtpConfig.password);
                        client.UploadFile(savePath, WebRequestMethods.Ftp.UploadFile, localPath);
                    }

                    info = _func.GetFileInformation(localPath);
                    // Log File for Other Process
                    strOriginFile = fileName.Split(".").ToList();
                    
                    _file.REST_InsertFilelog(
                            FileName : fileforInsert,
                            OriginalFile : strOriginFile[0],
                            FileSize : info.Length,
                            Extension : strInputFile[1],
                            PhoneNumber : strTelephone,
                            CallDate : callDate,
                            CallTime : callTime,
                            CreateBy : username
                        );
                        _func.DestroyFileFromName(targetDirectory, fileName);
                }
                catch (Exception e){
                    return e.Message;
                }
            }


            return username;
        }

    }
}
