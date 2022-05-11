using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System;
using System.Data;
using APICore.dbContext;
using Microsoft.Extensions.Options;
using static APICore.Models.appSetting;
using System.Text.Json;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using APICore.Common;

namespace APICore.Models
{
    public class MobileCheckerModel : ContextBase
    {
        private Statement statement;
        private DataTable dt;
        public ResultAccess resAccess;
        private Functional func;
        StateConfigs state = new StateConfigs();

        public MobileCheckerModel(IOptions<StateConfigs> configs) : base(configs)
        {
            statement = new Statement();
            dt = new DataTable();
            resAccess = new ResultAccess(configs);
            func = new Functional();
            state = configs.Value;
        }

        public void REST_CreateNotification(string EAppNo, string AlertTitle, string AlertRemark, string InboxTitle, string InboxRemark, string AppStatus, string AppSubStatus, string Key, string Checker, string CreateBy)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_CreateNotification @EAppNo, @AlertTitle, @AlertRemark, @InboxTitle, @InboxRemark, @AppStatus, @AppSubStatus, @Key, @Checker, @CreateBy");
            statement.AppendParameter("@EAppNo", EAppNo);
            statement.AppendParameter("@AlertTitle", AlertTitle);
            statement.AppendParameter("@AlertRemark", AlertRemark);
            statement.AppendParameter("@InboxTitle", InboxTitle);
            statement.AppendParameter("@InboxRemark", InboxRemark);
            statement.AppendParameter("@AppStatus", AppStatus);
            statement.AppendParameter("@AppSubStatus", AppSubStatus);
            statement.AppendParameter("@Key", Key);
            statement.AppendParameter("@Checker", Checker);
            statement.AppendParameter("@CreateBy", CreateBy);

            resAccess.ExecutenonResult(statement, state.ConnectionStrings.EApp);
        }

        public void REST_DeliveredProcess(string ApplicationNo, string ApplicationType, string ChassisNo, string EngineNo, string NextCard, string RequestBy)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_DeliveredProcess @ApplicationNo, @ApplicationType, @ChassisNo, @EngineNo, @NextCard, @RequestBy");
            statement.AppendParameter("@ApplicationNo", ApplicationNo);
            statement.AppendParameter("@ApplicationType", ApplicationType);
            statement.AppendParameter("@ChassisNo", ChassisNo);
            statement.AppendParameter("@EngineNo", EngineNo);
            statement.AppendParameter("@NextCard", NextCard);
            statement.AppendParameter("@RequestBy", RequestBy);

            resAccess.ExecutenonResult(statement);
        }
        public string SP_Get_SerialNoForChecker(string ApplicationNo)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC SP_Get_SerialNoForChecker @EApp_No");
            statement.AppendParameter("@EApp_No", ApplicationNo);
            dt = resAccess.ExecuteDataTable(statement, state.ConnectionStrings.EApp);

            return dt.Rows.Count > 0 ? dt.Rows[0]["SerialNo"].ToString() : "";
        }
        public void REST_CreateRequestLog(string FunctionName, string Data, string StatusCode, string Result, string CreateBy)
        {
            dt = new DataTable();
            statement = new Statement();
            statement.AppendStatement("EXEC REST_CreateRequestLog @FunctionName, @Data, @StatusCode, @Result, @CreateBy");
            statement.AppendParameter("@FunctionName", FunctionName);
            statement.AppendParameter("@Data", Data);
            statement.AppendParameter("@StatusCode", StatusCode);
            statement.AppendParameter("@Result", Result);
            statement.AppendParameter("@CreateBy", CreateBy);
            resAccess.ExecutenonResult(statement, state.ConnectionStrings.EApp);
        }
    }

}