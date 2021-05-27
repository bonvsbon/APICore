using System;
using System.Data;
using System.Net.Cache;
using Microsoft.AspNetCore.Mvc;
using static APICore.Models.appSetting;
using APICore.Common;
using APICore.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace APICore.Controllers {
    [ApiController]
    [Route ("api/[controller]/[action]")]
    [Produces("application/json")]
    public class CustomerController : ControllerBase {
        public class TempReceipt{
            public string AgreementNo { get; set; }            
            public string ReceiptUrl { get; set; }
            public int Period { get; set; }
            
        }
        StateConfigs state = new StateConfigs();
        Functional func;
        RequestDataModel reqModel;
        CustomerModel customer;
        public CustomerController(IOptions<StateConfigs> config) {
            func = new Functional();
            customer = new CustomerModel(config);
            reqModel = new RequestDataModel();
            state = config.Value;
        }

        // [Authorize]
        [HttpPost]
        public IActionResult index([FromBody] RequestModel request) {
            ResponseModel result = new ResponseModel();
            try
            {
                result._data = customer.REST_CustomerInformationbyNationID(request.data.Id);
                result._errorMessage = "";
                if(result._data.Rows.Count == 0)
                {
                    result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.NotFound);
                    return NotFound(result);
                }
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.OK);
                return Ok(result);
            } 
            catch (Exception e)
            {
                result._data = new DataTable();
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.InternalError);
                result._errorMessage = e.Message;
                return BadRequest(result);
            }

        }

        // [Authorize]
        [HttpPost]
        public IActionResult purchaseHistory([FromBody] RequestModel request) {
            ResponseModel result = new ResponseModel();
            try
            {
                result._data = customer.REST_PurchaseHistory(request.data.Id);
                result._errorMessage = "";
                if(result._data.Rows.Count == 0)
                {
                    result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.NotFound);
                    return NotFound(result);
                }
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.OK);
                return Ok(result);
            } 
            catch (Exception e)
            {
                result._data = new DataTable();
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.InternalError);
                result._errorMessage = e.Message;
                return BadRequest(result);
            }
        }

        // [Authorize]
        [HttpPost]
        public IActionResult installmentTable([FromBody] RequestModel request) {
            ResponseModel result = new ResponseModel();
            List<TempReceipt> dt = new List<TempReceipt>();
            try
            {
                result._data = customer.REST_InstallmentTable(request.data.AgreementNo);
                result._errorMessage = "";
                dt = result._data.AsEnumerable().Select(t => new TempReceipt()
                {
                   AgreementNo = t.Field<string>("Contno"),
                   ReceiptUrl =  t.Field<string>("ReceiptUrl"),
                   Period = t.Field<int>("Period")
                }).ToList();

                for(int i = 0; i < dt.Count; i++)
                {
                    if(!string.IsNullOrEmpty(dt[i].ReceiptUrl))
                    {
                        result._data.Rows[i]["ReceiptUrl"] = AESEncrypt.AESOperation.EncryptString(string.Format("{0}/{1}.pdf", state.ResourceUrl.documentsUrl,dt[i].ReceiptUrl));
                    }
                    else
                    {
                        result._data.Rows[i]["ReceiptUrl"] = dt[i].ReceiptUrl;
                    }
                    Console.WriteLine("Normal : " + string.Format("{0}/{1}.pdf", state.ResourceUrl.documentsUrl,dt[i].ReceiptUrl));
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine("Encrypt : " + AESEncrypt.AESOperation.EncryptString(string.Format("{0}.pdf", dt[i].ReceiptUrl)));
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine("Decrypt : " + AESEncrypt.AESOperation.DecryptString(AESEncrypt.AESOperation.EncryptString(string.Format("{0}.pdf", dt[i].ReceiptUrl))));
                    Console.WriteLine("----------------------------------");
                }

                if(result._data.Rows.Count == 0)
                {
                    result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.NotFound);
                    return NotFound(result);
                }
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.OK);
                return Ok(result);
            }
            catch (Exception e)
            {
                result._data = new DataTable();
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.InternalError);
                result._errorMessage = e.Message;
                return BadRequest(result);
            }
        }
    }
}