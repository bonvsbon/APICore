using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using APICore.Common;
using Microsoft.Extensions.Options;
using static APICore.Models.appSetting;
using APICore.dbContext;
namespace APICore.Models
{
    public class LineRequestModel
    {
        public string destination { get; set; }
        public List<EventsModel> events = new List<EventsModel>();      

    }
    public class EventsModel
    {
        public string type { get; set; }
        public MessageModel message { get; set; }
        public string timestamp { get; set; }
        public SourceModel source { get; set; }
        public string replyToken { get; set; }
        public string mode { get; set; } 
    }
    public class MessageModel{
        public string type { get; set; }
        public string id { get; set; }
        public string text { get; set; }
    }
    public class SourceModel{
        public string type { get; set; }
        public string userId { get; set; }
    }

    public class LineActionModel : ContextBase
    {
        private Statement statement;
        private DataTable dt;
        public ResultAccess resAccess;
        private Functional func;

        public LineActionModel(IOptions<StateConfigs> config) : base(config)
        {
            _state = config.Value;
            resAccess = new ResultAccess(config);
            statement = new Statement();
            func = new Functional();
        }
        public void SP_InsertLogRequestMessage(string Log_Type, string Log_Destination, string Log_Mode, string Log_Message, string Log_RawJson, DateTime Log_RequestDate, string Log_RequestBy, string Log_CreateBy)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC [LineDB]..[SP_InsertLogRequestMessage] @Log_Type, @Log_Destination, @Log_Mode, @Log_Message, @Log_RawJson, @Log_RequestDate, @Log_RequestBy, @Log_CreateBy");
            statement.AppendParameter("@Log_Type", Log_Type);
            statement.AppendParameter("@Log_Destination", Log_Destination);
            statement.AppendParameter("@Log_Mode", Log_Mode);
            statement.AppendParameter("@Log_Message", Log_Message);
            statement.AppendParameter("@Log_RawJson", Log_RawJson);
            statement.AppendParameter("@Log_RequestDate", Log_RequestDate);
            statement.AppendParameter("@Log_RequestBy", Log_RequestBy);
            statement.AppendParameter("@Log_CreateBy", Log_CreateBy);
            
            resAccess.ExecutenonResult(statement);
            // return 
        }

        public void SP_InsertUserFollow(string Profile_UserId, string Profile_DisplayName, string Profile_PictureUrl, string Profile_StatusMessage, string Profile_Language)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC [LineDB]..[SP_InsertUserFollow] @Profile_UserId, @Profile_DisplayName, @Profile_PictureUrl, @Profile_StatusMessage, @Profile_Language");
            statement.AppendParameter("@Profile_UserId", Profile_UserId);
            statement.AppendParameter("@Profile_DisplayName", Profile_DisplayName);
            statement.AppendParameter("@Profile_PictureUrl", Profile_PictureUrl);
            statement.AppendParameter("@Profile_StatusMessage", Profile_StatusMessage);
            statement.AppendParameter("@Profile_Language", Profile_Language);
            
            resAccess.ExecutenonResult(statement);
        }

        public void SP_GenerateRouteStep(string UserID, string Keyword)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC [LineDB]..[SP_GenerateRouteStep] @UserId, @Keyword");
            statement.AppendParameter("@UserId", UserID);
            statement.AppendParameter("@Keyword", Keyword);

            resAccess.ExecuteDataTable(statement);
        }

    }

}