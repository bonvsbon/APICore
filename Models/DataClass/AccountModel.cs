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

namespace APICore.Models
{
    public class AccountModel : ContextBase
    {
        private Statement statement;
        private DataTable dt;
        public ResultAccess resAccess;
        private Functional func;
        private LineApiController api;
        private SMSModel sms;
        StateConfigs state = new StateConfigs();
        LineActionModel action; 
        
        public AccountModel(IOptions<StateConfigs> configs) : base (configs)
        {
            statement = new Statement();
            dt = new DataTable();
            resAccess = new ResultAccess(configs);
            func = new Functional();
            api = new LineApiController();
            state = configs.Value;
            action = new LineActionModel(configs);
        }

        public void REST_KeepLogRequest(string error, string Json)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_KeepLogRequest @error, @json");
            statement.AppendParameter("@error", error);
            statement.AppendParameter("@json", Json);
            resAccess.ExecutenonResult(statement);
        }

        public async Task<SMSResponse> BindAccount(AccountRequestModel request, string OTPType = "Bind")
        {
            SMSResponse response = new SMSResponse();
            
                statement = new Statement(); 
                statement.AppendStatement("EXEC REST_CheckPhoneNumber @IDCard, @BirthDay, @OTPType"); // , @UserId
                statement.AppendParameter("@IDCard", request.IDCard);
                statement.AppendParameter("@BirthDay", request.BirthDay);
                statement.AppendParameter("@OTPType", OTPType);
                // statement.AppendParameter("@UserId", request.UserId);

                dt = resAccess.ExecuteDataTable(statement);
                if(dt.Rows.Count > 0)
                {
                    string urlData = string.Format(state.SMSConfigs.UrlBase + "user={0}&pass={1}&type={2}&to={3}&from={4}&text={5}&servid={6}", state.SMSConfigs.User, state.SMSConfigs.Pass, state.SMSConfigs.Type, dt.Rows[0]["PhoneNumber"].ToString(), state.SMSConfigs.From, func.ToHexString(dt.Rows[0]["Message"].ToString()), state.SMSConfigs.ServID);
                    CallAPI(urlData);
                    response.phoneNumber = dt.Rows[0]["OriginalPhoneNumber"].ToString();
                    response.result = dt.Rows[0]["result"].ToString();
                    response.refOTP = dt.Rows[0]["OTP_Reference"].ToString();
                }
                else
                {
                    response.phoneNumber = "";
                    response.result = "";
                    response.refOTP = "";
                    return response;
                }
    
            return response;
        }

        public async Task Unbind(UnbindAccountRequestModel request)
        {
            statement = new Statement(); 
            statement.AppendStatement("EXEC REST_UnbindAccount @UserId, @IDCard");
            statement.AppendParameter("@UserId", request.UserId);
            statement.AppendParameter("@IDCard", request.IDCard);
            dt = resAccess.ExecuteDataTable(statement);
        }
        

    public async Task<string> Register(AccountRequestModel request)
    {
        dt = new DataTable();
        string result = "";
        Task<UserProfile> profile;
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
        statement = new Statement();
        statement.AppendStatement("EXEC REST_AccountRegister @UserId, @IDCard, @BirthDay, @OTP");
        statement.AppendParameter("@UserId", request.UserId);
        statement.AppendParameter("@IDCard", request.IDCard);
        statement.AppendParameter("@BirthDay", request.BirthDay);
        statement.AppendParameter("@OTP", request.OTP);

        dt = resAccess.ExecuteDataTable(statement);
        if (dt.Rows.Count > 0)
        {
            result = dt.Rows[0]["Result"].ToString();
        }
        return result;
    }

    public async Task<DataTable> CheckOTP(string phoneNumber, string OTP)
    {
        
        statement = new Statement();
        dt = new DataTable();
        statement.AppendStatement("EXEC REST_VerifyOTP @phoneNumber, @OTP_Number");
        statement.AppendParameter("@phoneNumber", phoneNumber);
        statement.AppendParameter("@OTP_Number", OTP);

        dt = resAccess.ExecuteDataTable(statement);

        return dt;
    } 
    public async Task<DataTable> GetNextCard(string idCard, string agreementNo)
    {
        statement = new Statement();
        dt = new DataTable();
        statement.AppendStatement("EXEC REST_SelectNextCard @idCard, @agreementNo");
        statement.AppendParameter("@idCard", idCard);
        statement.AppendParameter("@agreementNo", agreementNo);

        dt = resAccess.ExecuteDataTable(statement);

        return dt;
    } 
    public async Task<DataTable> DisableCurrentOTP(string phoneNumber, string OTP)
    {
        statement = new Statement();
        dt = new DataTable();
        statement.AppendStatement("EXEC REST_UpdateOTP @phoneNumber, @OTP_Number");
        statement.AppendParameter("@phoneNumber", phoneNumber);
        statement.AppendParameter("@OTP_Number", OTP);

        dt = resAccess.ExecuteDataTable(statement);

        return dt;
    } 

    public async Task<DataTable> CheckExistingRegister(string IDCard)
    {
        statement = new Statement();
        dt = new DataTable();
        statement.AppendStatement("EXEC REST_CheckExistingRegister @IDCard");
        statement.AppendParameter("@IDCard", IDCard);

        dt = resAccess.ExecuteDataTable(statement);

        return dt;
    }

    #region Services
        static async void CallAPI(string CallURL)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            string result = "";
            string json = "";
            //json = JsonConvert.SerializeObject(reqSMS);
            using (var client = new HttpClient())
            {
                StringContent strContent = new StringContent(json);
                strContent.Headers.ContentType.MediaType = "application/json";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.BaseAddress = new Uri(GetAppSetting("UrlBase"));

                var response = client.GetAsync(CallURL);


                result = response.Result.Content.ReadAsStringAsync().Result;

                //ResponseModel res = new ResponseModel();
                //res = JsonConvert.DeserializeObject<ResponseModel>(result);
            }
        }
    }

    #endregion
}