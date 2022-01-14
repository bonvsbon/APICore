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
using APICore.Models.DataClass;
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
    [Route("/api/[controller]/[action]")]
    [Produces("application/json")]
    public class ManagementController : ControllerBase
    {        
        Functional func;
        DataTable dt;
        AccountModel acc;
        ManagementModel management;
        StateConfigs state;
        ResultUpdate result;
        List<InformationAddress> address;
        Dictionary<string, string> dc;
        public ManagementController(IOptions<StateConfigs> configs)
        {
            func = new Functional();
            acc = new AccountModel(configs);
            dt = new DataTable();
            management = new ManagementModel(configs);
            state = configs.Value;
            address = new List<InformationAddress>();
        }

    #region Bind Account
    // /api/management/SendOTP
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendOTP(ExistingCustomer request)
    {
        dt = new DataTable();
        dc = new Dictionary<string, string>();
        dt = await acc.CheckExistingRegister("", request.LineUserId);
        string agreementNo = "";
        string PhoneNumber = "";
        PhoneNumber = request.PhoneNumber.Remove(0,1);
        PhoneNumber = "66" + PhoneNumber;
        if(dt.Rows.Count == 0)
        {
            dc.Add("refCode", "");
            dc.Add("result", "ไม่พบบัญชีนี้ในระบบ");
            return NotFound(dc);
        }
        agreementNo = dt.Rows[0]["Agreement No_"].ToString();
        dt = new DataTable();
        dt = management.REST_GenerateOTP(request.PhoneNumber, agreementNo, "UpdateMobile");
        string urlData = string.Format(state.SMSConfigs.UrlBase + "user={0}&pass={1}&type={2}&to={3}&from={4}&text={5}&servid={6}", state.SMSConfigs.User, state.SMSConfigs.Pass, state.SMSConfigs.Type, PhoneNumber, state.SMSConfigs.From, func.ToHexString(dt.Rows[0]["Message"].ToString()), state.SMSConfigs.ServID);
        acc.CallAPI(urlData);
        dc = new Dictionary<string, string>();
        dc.Add("refCode", dt.Rows[0]["OTP_Reference"].ToString());
        dc.Add("result", "Success");
        return Ok(dc);
    }
    // /api/management/CheckOTP
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CheckOTP(CheckOTP request)
    {
        dt = new DataTable();
        dt = await acc.CheckOTP(request.phoneNumber, request.refOTP);
        OTPResultModel response = new OTPResultModel();
        
        if(dt.Rows.Count == 0) 
        {
            response.message = "mismatch";
            return NotFound(response);
        }
        if(dt.Rows[0]["Result"].ToString() == "NotFound")
        {
            dc = new Dictionary<string, string>();
            dc.Add("result", "OTP Mismatch");
            return NotFound(dc);
        }
        else if (dt.Rows[0]["Result"].ToString() == "OTP Expire")
        {
            dc = new Dictionary<string, string>();
            dc.Add("result", dt.Rows[0]["Result"].ToString());
            return NotFound(dc);
        }
        // await acc.DisableCurrentOTP(request.phoneNumber, request.OTP);
        response.message = "match";
        
        return Ok(response);
    }
    // /api/management/UpdatePhoneNumber
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdatePhoneNumber(MobileInformation request)
    {
        // if(request.isConsent != true)
        // {
        //     dc.Add("refCode", "");
        //     dc.Add("result", "กรุณายืนยันความถูกต้องของข้อมูล");
        //     return BadRequest(dc);
        // }
        string msg = "";
        result = new ResultUpdate();
        acc.REST_KeepLogRequest("request", func.JsonSerialize(request));
        if(string.IsNullOrEmpty(request.NewPhoneNumber) || string.IsNullOrEmpty(request.LineUserID))
        {
            result.phoneNumber = "";
            result.result = "Data is Empty";
            acc.REST_KeepLogRequest("Data is Empty", func.JsonSerialize(request));
            return NotFound(result);
        }
        msg = management.REST_UpdateMobileNumber(request.NewPhoneNumber, request.LineUserID);//, request.isConsent);
        result.phoneNumber = request.NewPhoneNumber;
        result.result = msg;
        return Ok(result);
    }
    // /api/management/GetAddress
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> GetAddress(SingleValueModel request)
    {
        address = new List<InformationAddress>();
        dc = new Dictionary<string, string>();
        if(string.IsNullOrEmpty(request.LineUserId))
        {
            dc.Add("result", "Line User ID is Empty");
            acc.REST_KeepLogRequest("Line User ID is Empty", func.JsonSerialize(request));
            return NotFound(dc);
        }
        address = management.REST_GetAddress(request.LineUserId, request.AgreementNo);
        return Ok(address);
    }

    // /api/management/UpdateAddress
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateAddress(InformationAddress request)
    {
        // if(request.isConsent != true)
        // {
        //     dc.Add("refCode", "");
        //     dc.Add("result", "กรุณายืนยันความถูกต้องของข้อมูล");
        //     return BadRequest(dc);
        // }
        
        dt = new DataTable();
        dc = new Dictionary<string, string>();
        address = new List<InformationAddress>();
        if (string.IsNullOrEmpty(request.LineUserId))
        {
            dc.Add("result", "Line User ID is Empty");
            acc.REST_KeepLogRequest("Line User ID is Empty", func.JsonSerialize(request));
            return NotFound(dc);
        }
        acc.REST_KeepLogRequest("request", func.JsonSerialize(request));
        dt = management.REST_UpdateAddress(
            request.AgreementNo,
            request.LineUserId,
            request.AddressCode,
            request.isMaillingAddress,
            request.AddressNo,
            request.Moo,
            request.Soi,
            request.RoomNo,
            request.Floor,
            request.Building,
            request.Street,
            request.District,
            request.SubDistrict,
            request.City,
            request.PostCode
            // ,
            // request.isConsent
        );

        if(dt.Rows.Count > 0)
        {
            dc.Add("result", dt.Rows[0]["Result"].ToString());
            acc.REST_KeepLogRequest(dt.Rows[0]["Result"].ToString(), func.JsonSerialize(request));
        }
        address = management.REST_GetAddress(request.LineUserId, request.AgreementNo);
        acc.REST_KeepLogRequest("return", func.JsonSerialize(address));
        return Ok(address);
    }
    #endregion
    
    #region Not Bind Account
    // /api/management/CheckisCustomer
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CheckisCustomer(CustomerWithoutBind request)
    {
        ResultModel result = new ResultModel();
        dt = new DataTable();
        dt = management.REST_CheckisCustomer(request.IDCard, request.BirthDay, request.NextCard);

        if(dt.Rows.Count > 0)
        {
            result.result = "found";
        }
        else
        {
            result.result = "notfound";
            acc.REST_KeepLogRequest("Not Found Contract", func.JsonSerialize(request));
            return NotFound(result);
        }

        return Ok(result);
    }

    // /api/management/SendOTPNoRegister
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendOTPNoRegister(CustomerWithoutBind request)
    {
        result = new ResultUpdate();
        dt = new DataTable();
        dt = management.REST_CheckisCustomer(request.IDCard, request.BirthDay, request.NextCard);
        string agreementNo = "";
        string PhoneNumber = "";
        PhoneNumber = request.PhoneNumber.Remove(0,1);
        PhoneNumber = "66" + PhoneNumber;
        if(dt.Rows.Count == 0)
        {
            result.result = "Not Found Customer Data";
            result.phoneNumber = "";
            acc.REST_KeepLogRequest("Not Found Customer Data", func.JsonSerialize(request));
            return NotFound(result);
        }
        else
        {
            agreementNo = dt.Rows[0]["Agreement No_"].ToString();
        }
        if(string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.BirthDay) || string.IsNullOrEmpty(request.NextCard) || string.IsNullOrEmpty(request.IDCard))
        {
            result.result = "Data is Empty";
            result.phoneNumber = "";
            acc.REST_KeepLogRequest("Data is Empty", func.JsonSerialize(request));
            return NotFound(result);
        }

        dt = new DataTable();
        dt = management.REST_GenerateOTP(request.PhoneNumber, agreementNo, "UpdateMobile");


        string urlData = string.Format(state.SMSConfigs.UrlBase + "user={0}&pass={1}&type={2}&to={3}&from={4}&text={5}&servid={6}", state.SMSConfigs.User, state.SMSConfigs.Pass, state.SMSConfigs.Type, PhoneNumber, state.SMSConfigs.From, func.ToHexString(dt.Rows[0]["Message"].ToString()), state.SMSConfigs.ServID);
        acc.CallAPI(urlData);
        dc = new Dictionary<string, string>();
        dc.Add("refCode", dt.Rows[0]["OTP_Reference"].ToString());
        dc.Add("result", "Success");
        return Ok(dc);
    }

    // /api/management/UpdatePhoneNumberNoRegister
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdatePhoneNumberNoRegister(CustomerWithoutBind request)
    {
        // if(request.isConsent != true)
        // {
        //     dc.Add("refCode", "");
        //     dc.Add("result", "กรุณายืนยันความถูกต้องของข้อมูล");
        //     return BadRequest(dc);
        // }
        string msg = "";
        result = new ResultUpdate();
        acc.REST_KeepLogRequest("request", func.JsonSerialize(request));
        if(string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.BirthDay) || string.IsNullOrEmpty(request.IDCard))
        {
            result.phoneNumber = "";
            result.result = "Data is Empty";
            acc.REST_KeepLogRequest("Data is Empty", func.JsonSerialize(request));
            return NotFound(result);
        }
        msg = management.REST_UpdateMobileNotRegister(request.IDCard, request.BirthDay, request.PhoneNumber, request.TrackingID, request.ApprovalName);//, request.isConsent);
        result.phoneNumber = request.PhoneNumber;
        result.result = msg;
        return Ok(result);
    }
    
    // /api/management/GetAddressNoRegister
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> GetAddressNoRegister(AccountRequestModel2 request)
    {
        dt = new DataTable();
        dc = new Dictionary<string, string>();
        dt = management.REST_CheckisCustomer(request.IDCard, request.BirthDay, request.NextCard);
        List<InformationAddressNoRegister> address = new List<InformationAddressNoRegister>();
        if(dt.Rows.Count == 0)
        {
            dc.Add("result","Not Found Customer Data");
            acc.REST_KeepLogRequest("Not Found Customer Data", func.JsonSerialize(request));
            return NotFound(dc);            
        }
        
        address = management.REST_GetAddressNotRegister(request.IDCard, request.BirthDay, request.NextCard);

        return Ok(address);
    }

    // /api/management/UpdateAddressNoRegister
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateAddressNoRegister(InformationAddressWithoutBind request)
    {
        // if(request.isConsent != true)
        // {
        //     dc.Add("refCode", "");
        //     dc.Add("result", "กรุณายืนยันความถูกต้องของข้อมูล");
        //     return BadRequest(dc);
        // }
        dt = new DataTable();
        dc = new Dictionary<string, string>();
        List<InformationAddressNoRegister> address = new List<InformationAddressNoRegister>();
        if (string.IsNullOrEmpty(request.IDCard) || string.IsNullOrEmpty(request.BirthDay) || string.IsNullOrEmpty(request.NextCard))
        {
            dc.Add("result", "Data is Empty");
            acc.REST_KeepLogRequest("Data is Empty", func.JsonSerialize(request));
            return NotFound(dc);
        }

        dt = management.REST_UpdateAddressNotRegister(
            request.IDCard,
            request.BirthDay,
            request.NextCard,
            request.AddressCode,
            request.isMaillingAddress,
            request.AddressNo,
            request.Moo,
            request.Soi,
            request.RoomNo,
            request.Floor,
            request.Building,
            request.Street,
            request.District,
            request.SubDistrict,
            request.City,
            request.PostCode,
            request.TrackingID,
            request.ApprovalName
            // ,
            // request.isConsent
        );

        if(dt.Rows.Count > 0)
        {
            dc.Add("result", dt.Rows[0]["Result"].ToString());
            acc.REST_KeepLogRequest(dt.Rows[0]["Result"].ToString(), func.JsonSerialize(request));
        }

        address = management.REST_GetAddressNotRegister(request.IDCard, request.BirthDay, request.NextCard);
        acc.REST_KeepLogRequest("return", func.JsonSerialize(address));

        return Ok(address);
    }
    #endregion

    [HttpPost]
    public async Task<IActionResult> TestConnection()
    {
        return Ok("Running");
    }

    }
}