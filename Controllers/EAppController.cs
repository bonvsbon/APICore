using System.Net.Http;
using System.Diagnostics;
using System.Data;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using APICore.Models;
using static APICore.Models.appSetting;
using Microsoft.Extensions.Options;
using APICore.Common;
using bBarcode = BarcodeLib.Barcode;
using System.Drawing;
using QRCoder;
using System.IO;
using System.Net.Http.Headers;


namespace APICore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class EAppController : ControllerBase
    {
        LineApiController api;
        LineMessageTemplate template;
        DACModel DAC;
        Functional func;
        DataTable dt;
        StateConfigs option;
        MobileCheckerModel _checker;
        private static HttpClient client = new HttpClient();

        public EAppController(IOptions<StateConfigs> options)
        {
            option = options.Value;
            _checker = new MobileCheckerModel(options);
        }

        [HttpPost]
        public async Task<IActionResult> StartFlow(DataRequest request)
        {
            try
            {
                _checker.REST_CreateRequestLog("StartFlow", JsonConvert.SerializeObject(request), "", "Begin Function", request.Email);
                string json = @"
                {
                    'folio': '" + request.EAppNo + @"',
                    'dataFields': {
                        'EApp_No': '" + request.EAppNo + @"',
                        'Checker_Email': '" + request.Email + @"'
                    }
                } 
                ";
                StringContent content = new StringContent(json,
                System.Text.Encoding.UTF8, 
                "application/json");
                client.DefaultRequestHeaders.Authorization 
                            = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("baf\\admink2:K2@min2020")));
                var response = await client.PostAsync("https://devncap.nextcapital.co.th/Api/Workflow/Preview/workflows/132", content);
                var result = response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode)
                {
                    _checker.REST_CreateRequestLog("StartFlow", JsonConvert.SerializeObject(request), ((int)response.StatusCode).ToString(), JsonConvert.SerializeObject(result), request.Email);
                    return Ok(result);
                }
                else
                {
                    _checker.REST_CreateRequestLog("StartFlow", JsonConvert.SerializeObject(request), ((int)response.StatusCode).ToString(), JsonConvert.SerializeObject(result), request.Email);
                    return BadRequest(result);
                }
            }
            catch(Exception e)
            {
                _checker.REST_CreateRequestLog("StartFlow", JsonConvert.SerializeObject(request), "500", e.Message, request.Email);
                return BadRequest(e.Message);
            }
            
        } 
        [HttpPost]
        public async Task<IActionResult> ActionFlow(DataRequest request)
        {
            
            // เพิ่ม Log
            // EApp_No, actiontype
            // EApp_No => Call Stored return Example => 12426_307 as TasksID
            // SP_Get_SerialNoForChecker
            // Action Type => {"Complete", "Reject", "Pending"}

            // https://devncap.nextcapital.co.th/Api/Workflow/Preview/tasks/12426_307/actions/complete

            // https://devncap.nextcapital.co.th/Api/Workflow/Preview/tasks/{TasksID}/actions/{actiontype}
            try
            {
                _checker.REST_CreateRequestLog("ActionFlow", JsonConvert.SerializeObject(request), "", "Begin Function", request.Email);
                string TaskID = _checker.SP_Get_SerialNoForChecker(request.EAppNo);
                // string json = @"
                // {
                //     'folio': '" + request.EAppNo + @"',
                //     'dataFields': {
                //         'EApp_No': '" + request.EAppNo + @"',
                //         'Checker_Email': '" + request.Email + @"'
                //     }
                // } ";
                // StringContent content = new StringContent(json,
                // System.Text.Encoding.UTF8, 
                // "application/json");
                client.DefaultRequestHeaders.Authorization 
                            = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("baf\\admink2:K2@min2020")));
                // var response = await client.PostAsync("https://devncap.nextcapital.co.th/Api/Workflow/Preview/tasks/{TasksID}/actions/{actiontype}", content);
                var response = await client.PostAsync($"https://devncap.nextcapital.co.th/Api/Workflow/Preview/tasks/{TaskID}/actions/{request.Action}", null);
                var result = response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode)
                {
                    _checker.REST_CreateRequestLog("ActionFlow", JsonConvert.SerializeObject(request), ((int)response.StatusCode).ToString(), JsonConvert.SerializeObject(result), request.Email);
                    return Ok(result);
                }
                else
                {
                    _checker.REST_CreateRequestLog("ActionFlow", JsonConvert.SerializeObject(request), ((int)response.StatusCode).ToString(), JsonConvert.SerializeObject(result), request.Email);
                    return BadRequest(result);
                }
            }
            catch(Exception e)
            {
                _checker.REST_CreateRequestLog("ActionFlow", JsonConvert.SerializeObject(request), "500", e.Message, request.Email);
                return BadRequest(e.Message);
            }
        } 

        [HttpGet("/api/EApp/Notification/{external_user_id}/{message}/{parameter}/{eAppNo}/{header}/{remark}/{inboxTitle}/{inboxRemark}/{appStatus}/{appSubStatus}/{key}/{checker}/{createBy}")]
        public async Task<IActionResult> Notification(string external_user_id, string message, string parameter, string eAppNo, string header, string remark, string inboxTitle, string inboxRemark, string AppStatus, string AppSubStatus, string Key, string checker, string createBy)
        {
            K2RequestModel req = new K2RequestModel();
            req.app_id = "9050c8f4-8a8e-46fe-8cc0-889798383b1b";
            req.include_external_user_ids.Add(external_user_id);
            req.isChromeWeb = true;
            req.channel_for_external_user_ids = "push";
            req.contents.Add("en", message);
            req.data.Add("param1", parameter);
            req.headings.Add("en", $"Header of {header}");
            req.url = "https://onesignal.com";

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(req),
                System.Text.Encoding.UTF8, 
                "application/json");
                client.DefaultRequestHeaders.Authorization 
                            = new AuthenticationHeaderValue("Basic", "OTA1MGNlZjUtNTVlMS00ODcwLTk0ZTctNzlmYTgwMTY2ZjUw");
                var response = await client.PostAsync("https://onesignal.com/api/v1/notifications", content);
                var result = response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode)
                {
                    _checker.REST_CreateRequestLog("Notification", JsonConvert.SerializeObject(req), ((int)response.StatusCode).ToString(), JsonConvert.SerializeObject(result), createBy);
                    _checker.REST_CreateNotification(eAppNo, header, remark, inboxTitle, inboxRemark, AppStatus, AppSubStatus, Key, checker, createBy);
                    return Ok();
                }
                else
                {
                    _checker.REST_CreateRequestLog("Notification", JsonConvert.SerializeObject(req), ((int)response.StatusCode).ToString(), JsonConvert.SerializeObject(result), createBy);
                    return BadRequest();
                }
                
            }
            catch(Exception e)
            {
                _checker.REST_CreateRequestLog("Notification", JsonConvert.SerializeObject(req), "500", e.Message, createBy);
                return BadRequest(e.Message);
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> GetConnection(string ConnectionString, string Type = "Encrypt")
        // {
        //     string result = "";
        //     if(Type == "Encrypt")
        //     {
        //         result = AESEncrypt.AESOperation.EncryptString(ConnectionString);
        //     }
        //     else
        //     {
        //         result = AESEncrypt.AESOperation.DecryptString(ConnectionString);
        //     }
        //     return Ok(result);
        // }

        // [HttpPost]
        // public async Task<IActionResult> DeliveredBooking(DeliveredModel request)
        // {
        //     try
        //     {
        //         _checker.REST_CreateRequestLog("DeliveredBooking", JsonConvert.SerializeObject(request), "", "Begin Function", request.RequestBy);
        //         if(string.IsNullOrEmpty(request.NextCard))
        //         {
        //             return BadRequest($"กรุณาระบุ Next Card");
        //         }
        //         // NAV
        //         _checker.REST_DeliveredProcess(request.ApplicationNo, request.ApplicationType, request.ChassisNo, request.EngineNo, request.NextCard, request.RequestBy);
        //         // End NAV
        //         _checker.REST_CreateRequestLog("DeliveredBooking", JsonConvert.SerializeObject(request), "200", "Success", request.RequestBy);
        //         return Ok();
        //     }
        //     catch (Exception e)
        //     {
        //         _checker.REST_CreateRequestLog("DeliveredBooking", JsonConvert.SerializeObject(request), "500", e.Message, request.RequestBy);
        //         return BadRequest(e.Message);
        //     }
        // }

        public class DeliveredModel
        {
            public string ApplicationNo { get; set; }
            public string ApplicationType { get; set; }
            // public string DealerCode { get; set; }
            // public string Remark { get; set; }
            public string ChassisNo { get; set; }
            public string EngineNo { get; set; }
            public string NextCard { get; set; }
            public string RequestBy { get; set; }
            // public string ConfirmNextCard { get; set; }
            // public string DeliveredStatus { get; set; }
            // public DateTime DeliveredDate { get; set; }
            // public DateTime AppointmentDate { get; set; }
            // public string TypeofDealer { get; set; }
            // public string DealerBranch { get; set; }
            // public string TaxInvoiceDate { get; set; }
            // public string TaxInvoiceNo { get; set; }            
        }

        public class DataRequest
        {
            public string EAppNo { get; set; }
            public string Email { get; set; }
            public string Action { get; set; }
        }

        public class K2RequestModel
        {
            public string app_id { get; set; }
            public List<string> include_external_user_ids = new List<string>();
            public bool isChromeWeb { get; set; }
            public string channel_for_external_user_ids { get; set; }
            public Dictionary<string, string> contents = new Dictionary<string, string>();
            public Dictionary<string, string> data = new Dictionary<string, string>();
            public Dictionary<string, string> headings = new Dictionary<string, string>();
            public string url { get; set; }
        }
    }
}