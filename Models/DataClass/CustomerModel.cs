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
public class CustomerModel : ContextBase
    {
        private Statement statement;
        private DataTable dt;
        public ResultAccess resAccess;
        private Functional func;

        public CustomerModel(IOptions<StateConfigs> config) : base(config)
        {
            _state = config.Value;
            resAccess = new ResultAccess(config);
            statement = new Statement();
            func = new Functional();
        }

        public DataTable REST_CustomerInformation(string agreementNo)
        {
            statement.AppendStatement("EXEC REST_CustomerInformation @AgreementNo");
            statement.AppendParameter("@AgreementNo", agreementNo);
            
            return resAccess.ExecuteDataTable(statement);
        }
        public DataTable REST_CustomerInformationbyNationID(string idcard)
        {
            statement.AppendStatement("EXEC REST_CustomerInformationbyNationID @IDCard");
            statement.AppendParameter("@IDCard", idcard);

            return resAccess.ExecuteDataTable(statement);
        }

        public DataTable REST_InstallmentTable(string agreementNo)
        {
            statement.AppendStatement("EXEC REST_InstallmentTable @AgreementNo");
            statement.AppendParameter("@AgreementNo", agreementNo);

            return resAccess.ExecuteDataTable(statement);
        }

        public DataTable REST_PurchaseHistory(string idcard)
        {
            statement.AppendStatement("EXEC REST_PurchaseHistory @NationID");
            statement.AppendParameter("@NationID", idcard);

            return resAccess.ExecuteDataTable(statement);
        }
    }
}