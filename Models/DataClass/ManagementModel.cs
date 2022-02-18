using System.Security.Cryptography.X509Certificates;
using System;
using System.Data;
using System.Net;
using APICore.dbContext;
using Microsoft.Extensions.Options;
using static APICore.Models.appSetting;
using System.Text.Json;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using APICore.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APICore.Models.DataClass
{
    public class ManagementModel : ContextBase
    {
        private Statement statement;
        private DataTable dt;
        public ResultAccess resAccess;
        private Functional func;
        private LineApiController api;
        private SMSModel sms;
        StateConfigs state = new StateConfigs();
        LineActionModel action; 
        public ManagementModel(IOptions<StateConfigs> configs) : base (configs)
        {
            statement = new Statement();
            dt = new DataTable();
            resAccess = new ResultAccess(configs);
            func = new Functional();
            api = new LineApiController();
            state = configs.Value;
            action = new LineActionModel(configs);
        }

        public DataTable REST_GenerateOTP(string phoneNumber, string agreementNo, string OTPType)
        {
            statement = new Statement();
            DataTable dt = new DataTable();
            statement.AppendStatement("EXEC REST_GenerateOTP @phoneNumber, @agreementNo, @OTPType");
            statement.AppendParameter("@phoneNumber", phoneNumber);
            statement.AppendParameter("@agreementNo", agreementNo);
            statement.AppendParameter("@OTPType", OTPType);

            dt = resAccess.ExecuteDataTable(statement);

            return dt;

        }

        public string REST_UpdateMobileNumber(string MobileNumber, string UserLineID, bool isConsent)
        {
            string result = "";
            statement = new Statement();
            DataTable dt = new DataTable();
            statement.AppendStatement("EXEC REST_UpdateMobile @mobile, @UserLineID, @isConsent");
            statement.AppendParameter("@mobile", MobileNumber);
            statement.AppendParameter("@UserLineID", UserLineID);
            statement.AppendParameter("@isConsent", isConsent);

            dt = resAccess.ExecuteDataTable(statement);
            if(dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["Result"].ToString();
            }

            return result;
        }
        // public string REST_UpdateMobileNotRegister(string IDCard, string BirthDay, string PhoneNumber, string TrackingID, string ApprovalName, bool isConsent)
        public string REST_UpdateMobileNotRegister(string IDCard, string BirthDay, string PhoneNumber, bool isConsent)
        {
            string result = "";
            statement = new Statement();
            DataTable dt = new DataTable();
            // statement.AppendStatement("EXEC REST_UpdateMobileNotRegister @IDCard, @BirthDay, @mobile, @TrackingID, @ApprovalName, @isConsent");
            statement.AppendStatement("EXEC REST_UpdateMobileNotRegister @IDCard, @BirthDay, @mobile, @isConsent");
            statement.AppendParameter("@IDCard", IDCard);
            statement.AppendParameter("@BirthDay", BirthDay);
            statement.AppendParameter("@mobile", PhoneNumber);
            // statement.AppendParameter("@TrackingID", TrackingID);
            // statement.AppendParameter("@ApprovalName", ApprovalName);
            statement.AppendParameter("@isConsent", isConsent);

            dt = resAccess.ExecuteDataTable(statement);
            if(dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["Result"].ToString();
            }

            return result;
        }

        public DataTable REST_CheckisCustomer(string IDCard, string BirthDay, string NextCard)
        {
            statement = new Statement();
            DataTable dt = new DataTable();
            statement.AppendStatement("EXEC REST_CheckisCustomer @IDCard, @BirthDay, @NextCard");
            statement.AppendParameter("@IDCard", IDCard);
            statement.AppendParameter("@BirthDay", BirthDay);
            statement.AppendParameter("@NextCard", NextCard);
            
            dt = resAccess.ExecuteDataTable(statement);

            return dt;

        }

        public List<InformationAddress> REST_GetAddress(string LineUserId, string AgreementNo)
        {
            statement = new Statement();
            DataTable dt = new DataTable();
            statement.AppendStatement("EXEC REST_GetAddress @LineUserId, @AgreementNo");
            statement.AppendParameter("@LineUserId", LineUserId);
            statement.AppendParameter("@AgreementNo", AgreementNo);
            
            dt = resAccess.ExecuteDataTable(statement);

            return ConvertDataTableToListAddress(dt, LineUserId);
        }

        public List<InformationAddressNoRegister> REST_GetAddressNotRegister(string IDCardNo, string BirthDay, string NextCard)
        {
            statement = new Statement();
            DataTable dt = new DataTable();
            statement.AppendStatement("EXEC REST_GetAddressNotRegister @IDCardNo, @BirthDay, @NextCard");
            statement.AppendParameter("@IDCardNo", IDCardNo);
            statement.AppendParameter("@BirthDay", BirthDay);
            statement.AppendParameter("@NextCard", NextCard);
            
            dt = resAccess.ExecuteDataTable(statement);

            return ConvertDataTableToListAddress(dt);
        }

        public DataTable REST_UpdateAddress(string AgreementNo, string UserLineID, string AddressCode, string isMaillingAddress, string AddressNo, string Moo, string Soi, string RoomNo, string Floor, string Building, string Street, string District, string SubDistrict, string City, string PostCode, bool isConsent)
        {
            statement = new Statement();
            DataTable dt = new DataTable();
            statement.AppendStatement("EXEC REST_UpdateAddress @AgreementNo, @UserLineID, @AddressCode, @isMaillingAddress, @AddressNo, @Moo, @Soi, @RoomNo, @Floor, @Building, @Street, @District, @SubDistrict, @City, @PostCode, @isConsent");
            statement.AppendParameter("@AgreementNo", AgreementNo);
            statement.AppendParameter("@UserLineID", UserLineID);
            statement.AppendParameter("@AddressCode", AddressCode);
            statement.AppendParameter("@isMaillingAddress", isMaillingAddress);
            statement.AppendParameter("@AddressNo", AddressNo);
            statement.AppendParameter("@Moo", Moo);
            statement.AppendParameter("@Soi", Soi);
            statement.AppendParameter("@RoomNo", RoomNo);
            statement.AppendParameter("@Floor", Floor);
            statement.AppendParameter("@Building", Building);
            statement.AppendParameter("@Street", Street);
            statement.AppendParameter("@District", District);
            statement.AppendParameter("@SubDistrict", SubDistrict);
            statement.AppendParameter("@City", City);
            statement.AppendParameter("@PostCode", PostCode);
            statement.AppendParameter("@isConsent", isConsent);
            
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }

        // public DataTable REST_UpdateAddressNotRegister(string IDcard, string BirthDay, string NextCard, string AddressCode, string isMaillingAddress, string AddressNo, string Moo, string Soi, string RoomNo, string Floor, string Building, string Street, string District, string SubDistrict, string City, string PostCode, string TrackingID, string ApprovalName, bool isConsent)
        public DataTable REST_UpdateAddressNotRegister(string IDcard, string BirthDay, string NextCard, string AddressCode, string isMaillingAddress, string AddressNo, string Moo, string Soi, string RoomNo, string Floor, string Building, string Street, string District, string SubDistrict, string City, string PostCode, bool isConsent)
        {
            statement = new Statement();
            DataTable dt = new DataTable();
            // statement.AppendStatement("EXEC REST_UpdateAddressNotRegister @IDcard, @BirthDay, @NextCard, @AddressCode, @isMaillingAddress, @AddressNo, @Moo, @Soi, @RoomNo, @Floor, @Building, @Street, @District, @SubDistrict, @City, @PostCode, @TrackingID, @ApprovalName, @isConsent");
            statement.AppendStatement("EXEC REST_UpdateAddressNotRegister @IDcard, @BirthDay, @NextCard, @AddressCode, @isMaillingAddress, @AddressNo, @Moo, @Soi, @RoomNo, @Floor, @Building, @Street, @District, @SubDistrict, @City, @PostCode, @isConsent");
            statement.AppendParameter("@IDcard", IDcard);
            statement.AppendParameter("@BirthDay", BirthDay);
            statement.AppendParameter("@NextCard", NextCard);
            statement.AppendParameter("@AddressCode", AddressCode);
            statement.AppendParameter("@isMaillingAddress", isMaillingAddress);
            statement.AppendParameter("@AddressNo", AddressNo);
            statement.AppendParameter("@Moo", Moo);
            statement.AppendParameter("@Soi", Soi);
            statement.AppendParameter("@RoomNo", RoomNo);
            statement.AppendParameter("@Floor", Floor);
            statement.AppendParameter("@Building", Building);
            statement.AppendParameter("@Street", Street);
            statement.AppendParameter("@District", District);
            statement.AppendParameter("@SubDistrict", SubDistrict);
            statement.AppendParameter("@City", City);
            statement.AppendParameter("@PostCode", PostCode);
            // statement.AppendParameter("@TrackingID", TrackingID);
            // statement.AppendParameter("@ApprovalName", ApprovalName);
            statement.AppendParameter("@isConsent", isConsent);
            
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }

        public List<InformationAddress> ConvertDataTableToListAddress(DataTable dt, string LineUserId)
        {
            List<InformationAddress> result = dt.AsEnumerable().Select(t => new InformationAddress()
            {
                LineUserId = LineUserId,
                AgreementNo = t.Field<string>("AgreementNo"),
                AddressCode = t.Field<string>("AddressCode"),
                AddressNo = t.Field<string>("AddressNo"),
                Moo = t.Field<string>("Moo"),
                Soi = t.Field<string>("Soi"),
                RoomNo = t.Field<string>("RoomNo"),
                Floor = t.Field<string>("Floor"),
                Building = t.Field<string>("Building"),
                Street = t.Field<string>("Street"),
                District = t.Field<string>("District"),
                SubDistrict = t.Field<string>("SubDistrict"),
                City = t.Field<string>("City"),
                PostCode = t.Field<string>("PostCode"),
                isMaillingAddress = t.Field<string>("isMaillingAddress"),
                AddressType = t.Field<string>("AddressType")
            }).ToList();

            return result;
        }
        public List<InformationAddressNoRegister> ConvertDataTableToListAddress(DataTable dt)
        {
            List<InformationAddressNoRegister> result = dt.AsEnumerable().Select(t => new InformationAddressNoRegister()
            {
                AddressCode = t.Field<string>("AddressCode"),
                AddressNo = t.Field<string>("AddressNo"),
                Moo = t.Field<string>("Moo"),
                Soi = t.Field<string>("Soi"),
                RoomNo = t.Field<string>("RoomNo"),
                Floor = t.Field<string>("Floor"),
                Building = t.Field<string>("Building"),
                Street = t.Field<string>("Street"),
                District = t.Field<string>("District"),
                SubDistrict = t.Field<string>("SubDistrict"),
                City = t.Field<string>("City"),
                PostCode = t.Field<string>("PostCode"),
                isMaillingAddress = t.Field<string>("isMaillingAddress"),
                AddressType = t.Field<string>("AddressType")
            }).ToList();

            return result;
        }
    }
}