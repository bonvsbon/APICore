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
        private bool result = false;

        public string JsonSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public void JsonDeserialize<T>(T obj, string json)
        {
            obj = JsonConvert.DeserializeObject<T>(json);
        }
        public T JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string GetStateHttp(StatusHttp code)
        {
            string resultCode = "";
            switch (code)
            {
                case StatusHttp.Created:
                    resultCode = "201";
                    break;
                case StatusHttp.Accepted:
                    resultCode = "202";
                    break;
                case StatusHttp.InvalidToken:
                    resultCode = "400";
                    break;
                case StatusHttp.SecurityError:
                    resultCode = "401";
                    break;
                case StatusHttp.NotFound:
                    resultCode = "404";
                    break;
                case StatusHttp.InternalError:
                    resultCode = "500";
                    break;
                default:
                    resultCode = "200";
                    break;
            }

            return resultCode;
        }

        public enum StatusHttp
        {
            OK,
            Created,
            Accepted,
            NotFound,
            InternalError,
            InvalidToken,
            SecurityError
        }
        public void SetResponseHeader(HttpContext context, string key = "Content-Type", string value = "application/json")
        {
            context.Response.Headers.Add(key, value);
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
