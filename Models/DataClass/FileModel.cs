using System.Data;
using APICore.dbContext;
using Microsoft.Extensions.Options;
using static APICore.Models.appSetting;


namespace APICore.Models
{
    public class FileModel: ContextBase
    {
        private ResponseModel _response;
        private Statement _statement;
        private DataTable dt;
        public ResultAccess _resAccess;

        public FileModel(IOptions<StateConfigs> config) : base(config)
        {
            _state = config.Value;

            _response = new ResponseModel();
            _resAccess = new ResultAccess(config);
            _statement = new Statement();
        }


        public string REST_InsertFilelog(string FileName, string OriginalFile, long fileSize, string PhoneNumber, string CallDate, string CallTime, string CreateBy)
        {
            _statement.AppendStatement("EXEC REST_InsertFilelog ");
            _statement.AppendParameter("@Test", 0);

            _resAccess.ExecutenonResult(_statement);

            return "Success";
        }
    }
}