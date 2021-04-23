using System.Data;
using APICore.dbContext;
using Microsoft.Extensions.Options;
using static APICore.Models.appSetting;

namespace APICore.Models
{
public class CustomerModel : ContextBase
    {
        private ResponseModel response;
        private Statement statement;
        private DataTable dt;
        public ResultAccess resAccess;

        public CustomerModel(IOptions<StateConfigs> config) : base(config)
        {
            _state = config.Value;
            response = new ResponseModel();
            resAccess = new ResultAccess(config);
            statement = new Statement();
        }

        public ResponseModel REST_CustomerInformation(string agreementNo)
        {
            statement.AppendStatement("EXEC REST_CustomerInformation @AgreementNo");
            statement.AppendParameter("@AgreementNo", agreementNo);

            return resAccess.ExecuteDataTable(statement);
        }
        public ResponseModel REST_CustomerInformationbyNationID(string idcard)
        {
            statement.AppendStatement("EXEC REST_CustomerInformationbyNationID @IDCard");
            statement.AppendParameter("@IDCard", idcard);

            return resAccess.ExecuteDataTable(statement);
        }

        public ResponseModel REST_InstallmentTable(string agreementNo)
        {
            statement.AppendStatement("EXEC REST_InstallmentTable @AgreementNo");
            statement.AppendParameter("@AgreementNo", agreementNo);

            return resAccess.ExecuteDataTable(statement);
        }

    }
}