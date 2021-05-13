using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using APICore.Common;

namespace APICore.Models
{
    public class ResponseModel
    {
        static Functional func;
        public ResponseModel()
        {
            func = new Functional();
        }
        public string _errorMessage { get; set; }
        public string _statusCode { get; set; }
            
        
        public static ResponseModel ResponseWithUnAuthorize()
        {
            ResponseModel _response = new ResponseModel();
            _response._statusCode = func.GetStateHttp(Functional.StatusHttp.SecurityError);
            _response._errorMessage = "Unauthorized";
            return _response;
        }

        #region "ResponseCustomerInformation"
        public class ResponseCustomerInformation
        {
            static ResponseCustomerInformation _response = new ResponseCustomerInformation();
            public string _errorMessage { get; set; }
            public CustomerModel.CustomerInformation _data { get; set; }
            public string _statusCode { get; set; }

            
            public static ResponseModel.ResponseCustomerInformation SerializeObject(DataTable _data, Functional.StatusHttp _statusCode, string _errorMessage)
            {
                _response._data = CustomerModel.CustomerInformation.ConvertDataTable(_data);
                _response._statusCode = func.GetStateHttp(_statusCode);
                _response._errorMessage = _errorMessage;

                return _response;
            }
        }
        #endregion

        #region "ResponseCustomerInformationbyNationID"
        public class ResponseCustomerInformationbyNationID
        {
            static ResponseCustomerInformationbyNationID _response = new ResponseCustomerInformationbyNationID();
            public string _errorMessage { get; set; }
            public List<CustomerModel.CustomerInformationbyNationID> _data { get; set; }
            public string _statusCode { get; set; }

            
            public static ResponseModel.ResponseCustomerInformationbyNationID SerializeObject(DataTable _data, Functional.StatusHttp _statusCode, string _errorMessage)
            {
                _response._data = CustomerModel.CustomerInformationbyNationID.ConvertDataTable(_data);
                _response._statusCode = func.GetStateHttp(_statusCode);
                _response._errorMessage = _errorMessage;

                return _response;
            }
        }
        #endregion

        #region "ResponseInstallmentTable"
        public class ResponseInstallmentTable
        {
            static ResponseInstallmentTable _response = new ResponseInstallmentTable();
            public string _errorMessage { get; set; }
            public List<CustomerModel.CustomerInstallmentTable> _data { get; set; }
            public string _statusCode { get; set; }

             public static ResponseInstallmentTable SerializeObject(DataTable _data, Functional.StatusHttp _statusCode, string _errorMessage)
            {
                _response._data = CustomerModel.CustomerInstallmentTable.ConvertDataTable(_data);
                _response._statusCode = func.GetStateHttp(_statusCode);
                _response._errorMessage = _errorMessage;

                return _response;
            }
        }
        #endregion

        
        #region "ResponsePurchaseHistory"
        public class ResponsePurchaseHistory
        {
            static ResponsePurchaseHistory _response = new ResponsePurchaseHistory();
            public string _errorMessage { get; set; }
            public List<CustomerModel.PurchaseHistory> _data { get; set; }
            public string _statusCode { get; set; }

             public static ResponsePurchaseHistory SerializeObject(DataTable _data, Functional.StatusHttp _statusCode, string _errorMessage)
            {
                _response._data = CustomerModel.PurchaseHistory.ConvertDataTable(_data);
                _response._statusCode = func.GetStateHttp(_statusCode);
                _response._errorMessage = _errorMessage;

                return _response;
            }
        }
        #endregion
    }
}