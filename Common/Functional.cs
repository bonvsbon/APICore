using Newtonsoft.Json;
using APICore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using static APICore.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Drawing;


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

       

        public ResponseModel ResponseWithUnAuthorize(string _errorMessage = "Unauthorized")
        {
            _response._data = new DataTable();
            _response._statusCode = _response.GetStateHttp(ResponseModel.StatusHttp.SecurityError);
            _response._errorMessage = _errorMessage;
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
        public string ToHexString(string str)
        {

            var s1 = string.Concat(str.Select(c => $"{(int)c:x4}"));  // left padded with 0 - "0030d835dfcfd835dfdad835dfe5d835dff0d835dffb"

            var sL = BitConverter.ToString(Encoding.Unicode.GetBytes(str)).Replace("-", "");       // Little Endian "300035D8CFDF35D8DADF35D8E5DF35D8F0DF35D8FBDF"
            var sB = BitConverter.ToString(Encoding.BigEndianUnicode.GetBytes(str)).Replace("-", ""); // Big Endian "0030D835DFCFD835DFDAD835DFE5D835DFF0D835DFFB"

            // no encodding "300035D8CFDF35D8DADF35D8E5DF35D8F0DF35D8FBDF"
            byte[] b = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, b, 0, b.Length);
            var sb = BitConverter.ToString(b).Replace("-", "");

            return sB.ToString();
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
        
        #region Line Message Dialog
        public string GetMessageCondition()
        {
            string result = "";
                result = 
@"เลขที่สัญญา : {0}
ยี่ห้อ / รุ่น : {1} 
ค่างวดทั้งหมด(บาท) : {2}
ผ่อนงวดละ(บาท) : {3}
กำหนดชำระทุกวันที่ : {4}
วันที่ครบกำหนดงวดสุดท้าย : {5}
ขยายอายุสัญญาจากพักชำระหนี้ : {19} 
สถานะสัญญา : {17}

ยอดค่างวดคงเหลือ(บาท) : {6}
งวดปัจจุบัน : {7}
ค่าธรรมเนียมติดตาม(บาท) : {8}
ค่าธรรมเนียมชำระล่าช้า(บาท) : {9}
ค่าธรรมเนียมอื่นๆ(บาท) : {10}
เงินส่วนลด(บาท) : {18}
ค่างวดค้างชำระ{14} : {13}
ค่างวดเดือนนี้(บาท) : {16}
ยอดรวมที่ต้องชำระ(บาท) : {11}

ชำระภายใน : {12} {15}
                ";

            return result;
        }
        #endregion
    }
}
