using System.Data;
using APICore.dbContext;
using Microsoft.Extensions.Options;
using static APICore.Models.appSetting;
using APICore.Common;

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


        public string REST_InsertFilelog(string FileName, string OriginalFile, long FileSize, string Extension,string PhoneNumber, string CallDate, string CallTime, string CreateBy)
        {
            _statement.AppendStatement("EXEC REST_InsertFilelog @FileName, @OriginalFile, @FileSize, @Extension, @PhoneNumber, @CallDate, @CallTime, @CreateBy");
            _statement.AppendParameter("@FileName", FileName.hasOrNull());
            _statement.AppendParameter("@OriginalFile", OriginalFile.hasOrNull());
            _statement.AppendParameter("@FileSize", FileSize.hasOrNull());
            _statement.AppendParameter("@Extension", Extension.hasOrNull());
            _statement.AppendParameter("@PhoneNumber", PhoneNumber.hasOrNull());
            _statement.AppendParameter("@CallDate", CallDate.hasOrNull());
            _statement.AppendParameter("@CallTime", CallTime.hasOrNull());
            _statement.AppendParameter("@CreateBy", CreateBy.hasOrNull());

            string result = _resAccess.ExecutenonResult(_statement);

            return result;
        }
    }
}