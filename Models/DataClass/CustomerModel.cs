using System;
using System.Data;
using APICore.dbContext;
using Microsoft.Extensions.Options;
using static APICore.Models.appSetting;
using System.Text.Json;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

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

        public ResponseModel.ResponseCustomerInformation REST_CustomerInformation(string agreementNo)
        {
            statement.AppendStatement("EXEC REST_CustomerInformation @AgreementNo");
            statement.AppendParameter("@AgreementNo", agreementNo);
            
            return ResponseModel.ResponseCustomerInformation.SerializeObject(resAccess.ExecuteDataTable(statement), Common.Functional.StatusHttp.OK, "");
        }
        public ResponseModel.ResponseCustomerInformationbyNationID REST_CustomerInformationbyNationID(string idcard)
        {
            statement.AppendStatement("EXEC REST_CustomerInformationbyNationID @IDCard");
            statement.AppendParameter("@IDCard", idcard);

            return ResponseModel.ResponseCustomerInformationbyNationID.SerializeObject(resAccess.ExecuteDataTable(statement), Common.Functional.StatusHttp.OK, "");
        }

        public ResponseModel.ResponseInstallmentTable REST_InstallmentTable(string agreementNo)
        {
            statement.AppendStatement("EXEC REST_InstallmentTable @AgreementNo");
            statement.AppendParameter("@AgreementNo", agreementNo);

            return ResponseModel.ResponseInstallmentTable.SerializeObject(resAccess.ExecuteDataTable(statement), Common.Functional.StatusHttp.OK, "");
        }

        public ResponseModel.ResponsePurchaseHistory REST_PurchaseHistory(string idcard)
        {
            statement.AppendStatement("EXEC REST_PurchaseHistory @NationID");
            statement.AppendParameter("@NationID", idcard);

            return ResponseModel.ResponsePurchaseHistory.SerializeObject(resAccess.ExecuteDataTable(statement), Common.Functional.StatusHttp.OK, "");
        }

        #region "CustomerInformationbyNationID" => "REST_CustomerInformationbyNationID"
        public class CustomerInformationbyNationID 
        {
            public string Prefix { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            [JsonProperty(PropertyName = "Citizen ID")]
            public string CitizenID { get; set; }
            [JsonProperty(PropertyName = "Application Number")]
            public string ApplicationNo { get; set; }            
            [JsonProperty(PropertyName = "Agreement Number")]
            public string AgreementNo { get; set; }
            [JsonProperty(PropertyName = "BAF Card Number")]
            public string NextCard { get; set; }
            public string Status { get; set; }
            
            public static List<CustomerInformationbyNationID> ConvertDataTable(DataTable dt){
                List<CustomerInformationbyNationID> result = new List<CustomerInformationbyNationID>();
                result = dt.AsEnumerable().Select(t => new CustomerInformationbyNationID(){
                    Prefix = t.Field<string>("Prefix"),
                    Firstname = t.Field<string>("Firstname"),
                    Lastname = t.Field<string>("Lastname"),
                    CitizenID = t.Field<string>("Citizen ID"),
                    ApplicationNo = t.Field<string>("Application Number"),
                    AgreementNo = t.Field<string>("Agreement Number"),
                    NextCard = t.Field<string>("BAF Card Number"),
                    Status = t.Field<string>("Status")
                }).ToList();

                return result;
            }
            
        }

        #endregion

        #region "CustomerInstallmentTable" => "REST_InstallmentTable"
        public class CustomerInstallmentTable {
            public DateTime DueDate { get; set; }
            public int Period { get; set; }
            public decimal PayAmount { get; set; }
            public decimal Balance { get; set; }
            public decimal Colamount { get; set; }
            public decimal ColRemainAmount { get; set; }
            public string state { get; set; }
            public int Due { get; set; }
            
            public static List<CustomerInstallmentTable> ConvertDataTable(DataTable dt)
            {
                List<CustomerInstallmentTable> result = new List<CustomerInstallmentTable>();
                result = dt.AsEnumerable().Select(t => new CustomerInstallmentTable(){
                    DueDate = t.Field<DateTime>("DueDate"),
                    Period = t.Field<int>("Period"),
                    PayAmount = t.Field<decimal>("PayAmount"),
                    Balance = t.Field<decimal>("Balance"),
                    Colamount = t.Field<decimal>("Colamount"),
                    ColRemainAmount = t.Field<decimal>("ColRemainAmount"),
                    state = t.Field<string>("state"),
                    Due = t.Field<int>("due"),
                }).ToList();
                return result;
            }
            
        }
        #endregion

        #region "CustomerInformation" => "REST_CustomerInformation"
        public class CustomerInformation {
            public string AgreementNo { get; set; }
            public string CustomerName { get; set; }
            public string Email { get; set; }
            public string Model { get; set; }
            public string Color { get; set; }
            public string EngineNo { get; set; }
            public string ChassisNo { get; set; }
            public decimal InstallmentAmount { get; set; }
            public int InstallmentPeriod { get; set; }
            public decimal OSBalance { get; set; }
            public string AccountStatus { get; set; }
            
            public static CustomerInformation ConvertDataTable(DataTable dt)
            {
                CustomerInformation result = new CustomerInformation();
                result = dt.AsEnumerable().Select(t => new CustomerInformation(){
                    AgreementNo = t.Field<string>("AgreementNo"),
                    CustomerName = t.Field<string>("CustomerName"),
                    Email = t.Field<string>("Email"),
                    Model = t.Field<string>("Model"),
                    Color = t.Field<string>("Color"),
                    EngineNo = t.Field<string>("EngineNo"),
                    ChassisNo = t.Field<string>("ChassisNo"),
                    InstallmentAmount = t.Field<decimal>("InstallmentAmount"),
                    InstallmentPeriod = t.Field<int>("InstallmentPeriod"),
                    OSBalance = t.Field<int>("OSBalance"),
                    AccountStatus = t.Field<string>("AccountStatus")
                }).FirstOrDefault();
                return result;
            }
            
        }
        #endregion        

        #region "PurchaseHistory" => "REST_PurchaseHistory"
        public class PurchaseHistory
        {
            [JsonProperty("Product Type")]
            public string ProductType { get; set; }
            [JsonProperty("Product Name")]
            public string ProductName { get; set; }
            [JsonProperty("Contract Start Date")]
            public DateTime AgreementDate { get; set; }
            [JsonProperty("Registration No")]
            public string RegistrationNo { get; set; }
            
            public static List<PurchaseHistory> ConvertDataTable(DataTable dt)
            {
                List<PurchaseHistory> result = new List<PurchaseHistory>();
                result = dt.AsEnumerable().Select(t => new PurchaseHistory(){
                    ProductType = t.Field<string>("Product Type"),
                    ProductName = t.Field<string>("Product Name"),
                    AgreementDate = t.Field<DateTime>("Contract Start Date"),
                    RegistrationNo = t.Field<string>("Registration No")
                }).ToList();

                return result;
            }                  
            
        }
        #endregion
    }
}