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
    /// <summary>
    /// Dealer AND Checker Model
    /// </summary>
    public class DACModel : ContextBase
    {
        private Statement statement;
        private DataTable dt;
        public ResultAccess resAccess;
        private Functional func;
        private LineApiController api;
        private SMSModel sms;
        StateConfigs state = new StateConfigs();
        LineActionModel action;  
        
        public DACModel(IOptions<StateConfigs> configs) : base(configs)
        {
            statement = new Statement();
            dt = new DataTable();
            resAccess = new ResultAccess(configs);
            func = new Functional();
            api = new LineApiController();
            state = configs.Value;
            action = new LineActionModel(configs);
        }

        public DataTable CheckSecretCode(string SecretCode)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_CheckSecretCode @SecretCode");
            statement.AppendParameter("@SecretCode", SecretCode);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }
        public DataTable MatchingUser(string LineUserId, string SecretCode)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_MatchingUser @LineUserId, @SecretCode");
            statement.AppendParameter("@LineUserId", LineUserId);
            statement.AppendParameter("@SecretCode", SecretCode);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }
        public void SetupRichmenubyUser(string LineUserId, string RichMenuId)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_SetupRichmenubyUser @RichMenuId, @LineUserId");
            statement.AppendParameter("@RichMenuId", RichMenuId);
            statement.AppendParameter("@LineUserId", LineUserId);
            dt = resAccess.ExecuteDataTable(statement);
        }

        public DataTable GetUserInformation(string LineUserId)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_GetUserInformation @LineUserId");
            statement.AppendParameter("@LineUserId", LineUserId);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }          
        public DataTable REST_GetNeedHelpMessage(string LineUserId)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_GetNeedHelpMessage @LineUserId");
            statement.AppendParameter("@LineUserId", LineUserId);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }  
        public DataTable CheckApplicationNo(string AppNo)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_CheckApplicationNo @AppNo");
            statement.AppendParameter("@AppNo", AppNo);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }     
        public DataTable REST_GetApplicationInformation(string AppNo)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_GetApplicationInformation @AppNo");
            statement.AppendParameter("@AppNo", AppNo);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }        
        public DataTable REST_CheckHelperCase(string LineUserId)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_CheckHelperCase @LineUserId");
            statement.AppendParameter("@LineUserId", LineUserId);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }
        public DataTable REST_GetUserforNotice(string AppNo, string State)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_GetUserforNotice @AppNo, @State");
            statement.AppendParameter("@AppNo", AppNo);
            statement.AppendParameter("@State", State);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }       
        public DataTable REST_CheckAceptTaskExisting(string AppNo)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_CheckAceptTaskExisting @AppNo");
            statement.AppendParameter("@AppNo", AppNo);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }
        public DataTable REST_SelectPendingTaskByAppNo(string AppNo)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_SelectPendingTaskByAppNo @AppNo");
            statement.AppendParameter("@AppNo", AppNo);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }
        public DataTable REST_GetCheckerList(string AppNo)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_GetCheckerList @AppNo");
            statement.AppendParameter("@AppNo", AppNo);
            dt = resAccess.ExecuteDataTable(statement);

            return dt;
        }

        public DataTable REST_UpdateStatusApp(string UserLineId, string AppNo)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_UpdateStatusApp @UserLineId, @AppNo");
            statement.AppendParameter("@UserLineId", UserLineId);
            statement.AppendParameter("@AppNo", AppNo);
            dt = resAccess.ExecuteDataTable(statement);


            return dt;
        } 
        public DataTable REST_CheckStatustoFlexMessage(string UserLineId)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_CheckStatustoFlexMessage @UserLineId");
            statement.AppendParameter("@UserLineId", UserLineId);
            dt = resAccess.ExecuteDataTable(statement);
            
            return dt;
        }
    }
}