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

namespace APICore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        Functional func;
        DataTable dt;
        AccountModel acc;
        CustomerModel cus;
        List<NoticeDue> due;
        List<NoticePayment> payment;
        Task<UserProfile> profile;
        LineApiController api;
        LineActionModel action; 
        List<CustomerData> data;
        public AccountController(IOptions<StateConfigs> configs)
        {
            func = new Functional();
            acc = new AccountModel(configs);
            dt = new DataTable();
            cus = new CustomerModel(configs);
            api = new LineApiController();
            action = new LineActionModel(configs);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Bind(AccountRequestModel request)
        {
            acc.REST_KeepLogRequest("", func.JsonSerialize(request));
            try
            {
                SMSResponse response = new SMSResponse();
                response = await acc.BindAccount(request);

                if(response.phoneNumber == "")
                {
                    return NotFound(response);
                }
                return Ok(response);
            } 
            catch (Exception e)
            {
                acc.REST_KeepLogRequest(e.Message, func.JsonSerialize(request));
                return BadRequest(e.Message);
            }
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UnBindOTP(AccountRequestModel request)
        {
            return NotFound();
            SMSResponse response = new SMSResponse();
            response = await acc.BindAccount(request, "UnBind");

            if(response.phoneNumber == "")
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UnBind(UnbindAccountRequestModel request)
        {
            acc.REST_KeepLogRequest("", func.JsonSerialize(request));
            OTPResultModel response = new OTPResultModel();
            try
            {
                await acc.Unbind(request);
                response.message = "success";
            }
            catch(Exception e)
            {
                acc.REST_KeepLogRequest(e.Message, func.JsonSerialize(request));
                response.message = "fail";
            }
            if(response.message == "fail")
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Register(AccountRequestModel request)
        {
            try
            {
                acc.REST_KeepLogRequest("", func.JsonSerialize(request));
                string result = await acc.Register(request);
                if(result == "NotFound")
                {
                    Dictionary<string, string> response = new Dictionary<string, string>();
                    response.Add("result", "OTP Mismatch");
                    return NotFound(response);
                }
                else if (result == "OTP Expire")
                {
                    Dictionary<string, string> response = new Dictionary<string, string>();
                    response.Add("result", result);
                    return NotFound(response);
                }
                data = cus.REST_GetAccountInformation(request.IDCard, request.BirthDay, "");
                profile = api.GetUserProfile(request.UserId);
                if(profile.Result != null)
                {
                    action.SP_InsertUserFollow(
                        request.UserId,
                        profile.Result.displayName,
                        profile.Result.pictureUrl,
                        profile.Result.statusMessage,
                        profile.Result.language
                    );
                }
                return Ok(data);
            } 
            catch (Exception e)
            {
                acc.REST_KeepLogRequest(e.Message, func.JsonSerialize(request));
                return BadRequest(e.Message);
            }
            
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> VerifyOTP(SMSResponse request)
        {
            OTPResultModel response = new OTPResultModel();
            dt = await acc.CheckOTP(request.phoneNumber, request.result);
            if(dt.Rows.Count == 0) 
            {
                response.message = "mismatch";
                return NotFound(response);
            }
            // await acc.DisableCurrentOTP(request.phoneNumber, request.OTP);
            response.message = "match";
            
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<object> Info(AccountInformationModel request)
        {
            try
            {
                acc.REST_KeepLogRequest("", func.JsonSerialize(request));
                CustomerData result = new CustomerData();
                data = new List<CustomerData>();
                data = cus.REST_GetAccountInformation(request.IDCard, request.BirthDay, "");
                if(data.Count == 0) 
                {
                    return NotFound(data);
                }
                // profile = api.GetUserProfile(request.UserId);
                // if(profile.Result != null)
                // {
                //     action.SP_InsertUserFollow(
                //         request.UserId,
                //         profile.Result.displayName,
                //         profile.Result.pictureUrl,
                //         profile.Result.statusMessage,
                //         profile.Result.language
                //     );
                // }
                // result = data[0];
                return Ok(data);
            } 
            catch (Exception e)
            {
                acc.REST_KeepLogRequest(e.Message, func.JsonSerialize(request));
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<object> InfoWithoutRegister(AccountRequestModel2 request)
        {
            try
            {
                acc.REST_KeepLogRequest("", func.JsonSerialize(request));
                List<CustomerDataWithoutRegister> data = new List<CustomerDataWithoutRegister>();
                data = cus.REST_CustomerInformationbyNationIDNotRegister(request.IDCard, request.BirthDay, request.NextCard);
                if(data.Count == 0)
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("message", "Not Found Data");
                    return NotFound(result);
                }
                return Ok(data[0]);
            } 
            catch (Exception e)
            {
                acc.REST_KeepLogRequest(e.Message, func.JsonSerialize(request));
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetDue(CustomerDue request)
        {
            try
            {
                acc.REST_KeepLogRequest("", func.JsonSerialize(request));
                dt = new DataTable();
                dt = await acc.CheckExistingRegister(request.IDCard);
                if(dt.Rows.Count == 0) return BadRequest(func.ResponseWithUnAuthorize("Not Register"));
                PaymentDue data = new PaymentDue();
                data = cus.REST_GetPaymentCurrentDue(request.IDCard, request.BirthDay, request.AgreementNo);
                if(data.DueDate == null) 
                {
                    return NotFound(data);
                }
                return Ok(data);
            } 
            catch (Exception e)
            {
                acc.REST_KeepLogRequest(e.Message, func.JsonSerialize(request));
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GenerateBarcode(RequestPaymentGatewayModel request)
        {
            acc.REST_KeepLogRequest("", func.JsonSerialize(request));
            string paymentFormat = "|010756300005301|{0}||{1}";
            string paymentFormatUI = "|010756300005301  {0}  {1}";
            string barcodePath = "barcode_{0}.png";
            string qrcodePath = "qrcode_{0}.png";
            byte[] bArray;
            PaymentModel response = new PaymentModel();
            dt = await acc.GetNextCard(request.IDCard, request.AgreementNo);
            if(dt.Rows.Count == 0)
            {
               return NotFound(response); 
            }
            // bBarcode b = new bBarcode()
            // {
            //     BarWidth = 10
            // };
            try {
            
            // Image img = b.Encode(BarcodeLib.TYPE.CODE128, string.Format(paymentFormat, dt.Rows[0]["NextCard"].ToString()), Color.Black, Color.White);
            // b.Width = 10;
            // img.Save(@"wwwroot/img/" + string.Format(barcodePath, dt.Rows[0]["NextCard"].ToString()));
            // b.SaveImage("wwwroot/img/barcodeFile.png", BarcodeLib.SaveTypes.PNG);

            // QRCodeGenerator qrGenerator = new QRCodeGenerator();
            // QRCodeData qrCodeData = qrGenerator.CreateQrCode(string.Format(paymentFormat, dt.Rows[0]["NextCard"].ToString()), QRCodeGenerator.ECCLevel.Q);
            // QRCode qrCode = new QRCode(qrCodeData);
            // Bitmap qrCodeImage = qrCode.GetGraphic(20);
            // Image img2 = qrCodeImage;
            // img2.Save(@"wwwroot/img/" + string.Format(qrcodePath, dt.Rows[0]["NextCard"].ToString()));
            // response.BarcodeUrl = @"https://synergy.nextcapital.co.th/WEBtest/APICore/wwwroot/img/" + string.Format(barcodePath, dt.Rows[0]["NextCard"].ToString());
            // response.QRCodeUrl = @"https://synergy.nextcapital.co.th/WEBtest/APICore/wwwroot/img/" + string.Format(qrcodePath, dt.Rows[0]["NextCard"].ToString());

            response.refCard = string.Format(paymentFormat, dt.Rows[0]["NextCard"].ToString(), dt.Rows[0]["Installment"].ToString());
            response.Prefix = dt.Rows[0]["Prefix"].ToString();
            response.FirstName = dt.Rows[0]["FirstName"].ToString();
            response.LastName = dt.Rows[0]["LastName"].ToString();
            response.refCardDisplay = string.Format(paymentFormatUI, dt.Rows[0]["NextCard"].ToString(), dt.Rows[0]["Installment"].ToString());

            return Ok(response);
            }
            catch(Exception e)
            {
                acc.REST_KeepLogRequest(e.Message, func.JsonSerialize(request));
                return BadRequest(e.Message);
            }
            
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> NoticeDue()
        {
            due = new List<NoticeDue>();
            due = cus.REST_NoticeDue();
            acc.REST_KeepLogRequest("Return", func.JsonSerialize(due));
            return Ok(due);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> NoticePayment()
        {
            payment = new List<NoticePayment>();
            payment = cus.REST_NoticePayment();
            acc.REST_KeepLogRequest("Return", func.JsonSerialize(payment));
            return Ok(payment);
        }

    }
}