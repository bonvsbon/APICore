using Newtonsoft.Json;
using APICore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using static APICore.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace APICore.Common
{
    public class Functional
    {
        private ResponseModel _response;
        private bool result = false;

        public Functional(){
            _response = new ResponseModel();
        }

        public string JsonSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public void JsonDeserialize<T>(T obj, string json)
        {
            obj = JsonConvert.DeserializeObject<T>(json);
        }
        public T JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

       

        public ResponseModel ResponseWithUnAuthorize()
        {
            _response._statusCode = _response.GetStateHttp(ResponseModel.StatusHttp.SecurityError);
            _response._errorMessage = "Unauthorized";
            return _response;
        }
        public ResponseModel SerializeObject(DataTable _data, StatusHttp _statusCode, string _errorMessage)
        {
            _response._data = _data;
            _response._statusCode = _response.GetStateHttp(_statusCode);
            _response._errorMessage = _errorMessage;
            return _response;
        }

        #region Directory & File
        public bool CheckExistingFile(string path, string filename)
        {
            List<string> files = Directory.GetFiles(path).ToList();
            string file = "";
            if (files.Count > 0)
            {
                file = files.Where(t => t.Contains(filename)).FirstOrDefault();
                if (file != null && !string.IsNullOrEmpty(file))
                {
                    this.result = true;
                    return this.result;
                }
            }
            this.result = false;
            return this.result;
        }

        public bool CheckExistingDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                this.result = true;
                return this.result;
            }
            this.result = false;
            return this.result;
        }

        public bool DirectoryCreate(string pathRoot)
        {
            if (!this.result)
            {
                Directory.CreateDirectory(pathRoot);
            }
            this.result = true;

            return this.result;
        }

        public bool DestroyFileFromName(string rootPath, string filename)
        {
            string file = "";
            if (!CheckExistingDirectory(rootPath))
            {
                this.result = false;
            }
            else
            {
                if (CheckExistingFile(rootPath, filename))
                {
                    file = Path.Combine(rootPath, filename);
                    File.Delete(file);
                }
            }
            return this.result;
        }

        public FileInfo GetFileInformation(string filename)
        {
            return new FileInfo(filename);
        }
        #endregion

    }
}
