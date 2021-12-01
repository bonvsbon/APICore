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
            statement = new Statement();
            statement.AppendStatement("EXEC REST_CustomerInformation @AgreementNo");
            statement.AppendParameter("@AgreementNo", agreementNo);
            
            return resAccess.ExecuteDataTable(statement);
        }
        public DataTable REST_CustomerInformationbyNationID(string idcard)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_CustomerInformationbyNationID @IDCard");
            statement.AppendParameter("@IDCard", idcard);

            return resAccess.ExecuteDataTable(statement);
        }
        public List<CustomerData> REST_CustomerInformationbyNationID(string idcard, string BirthDay, string NextCard)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_CustomerInformationbyIDCard @IDCard, @BirthDay, @NextCard");
            statement.AppendParameter("@IDCard", idcard);
            statement.AppendParameter("@BirthDay", BirthDay);
            statement.AppendParameter("@NextCard", NextCard);

            return ConvertDataTableToList(resAccess.ExecuteDataTable(statement));
        }
        public List<CustomerDataWithoutRegister> REST_CustomerInformationbyNationIDNotRegister(string idcard, string BirthDay, string NextCard)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_GetAccountInformationWithout @IDCard, @BirthDay, @NextCard");
            statement.AppendParameter("@IDCard", idcard);
            statement.AppendParameter("@BirthDay", BirthDay);
            statement.AppendParameter("@NextCard", NextCard);

            return ConvertDataTableToListNotRegister(resAccess.ExecuteDataTable(statement));
        }
        
        public PaymentDue REST_GetPaymentCurrentDue(string idcard, string BirthDay, string AgreementNo)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_GetPaymentCurrentDue @IDCard, @BirthDay, @AgreementNo");
            statement.AppendParameter("@IDCard", idcard);
            statement.AppendParameter("@BirthDay", BirthDay);
            statement.AppendParameter("@AgreementNo", AgreementNo);

            return ConvertDataTabletoObject(resAccess.ExecuteDataTable(statement));
        }

        public List<CustomerData> REST_GetAccountInformation(string idcard, string BirthDay, string AgreementNo)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_GetAccountInformation @IDCard, @BirthDay, @AgreementNo");
            statement.AppendParameter("@IDCard", idcard);
            statement.AppendParameter("@BirthDay", BirthDay);
            statement.AppendParameter("@AgreementNo", AgreementNo);

            return ConvertDataTableToList(resAccess.ExecuteDataTable(statement));
        }
        public DataTable REST_InstallmentTable(string agreementNo)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_InstallmentTable @AgreementNo");
            statement.AppendParameter("@AgreementNo", agreementNo);

            return resAccess.ExecuteDataTable(statement);
        }

        public DataTable REST_PurchaseHistory(string idcard)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_PurchaseHistory @NationID");
            statement.AppendParameter("@NationID", idcard);

            return resAccess.ExecuteDataTable(statement);
        }

        public void REST_InitialStep(string userId, string step, string value)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_InitialStep @userId, @step, @value");
            statement.AppendParameter("@userId", userId);
            statement.AppendParameter("@step", step);
            statement.AppendParameter("@value", value);

            resAccess.ExecutenonResult(statement);
        } 

        public DataTable REST_SelectFilterCondition(string userId)
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_SelectFilterCondition @userId");
            statement.AppendParameter("@userId", userId);

            return resAccess.ExecuteDataTable(statement);
        }

        public List<NoticeDue> REST_NoticeDue()
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_NoticeDue");
            
            return ConvertDataTable_RESTNoticeDue(resAccess.ExecuteDataTable(statement));
        }

        public List<NoticePayment> REST_NoticePayment()
        {
            statement = new Statement();
            statement.AppendStatement("EXEC REST_NoticePayment");

            return ConvertDataTable_RESTNoticePayment(resAccess.ExecuteDataTable(statement));
        }
    

    public List<NoticeDue> ConvertDataTable_RESTNoticeDue(DataTable dt)
    {
        List<NoticeDue> due = new List<NoticeDue>();
        due = dt.AsEnumerable().Select(t => new NoticeDue()
        {
            AgreementNo = t.Field<string>("AgreementNo"),
            Period = t.Field<string>("Period"),
            PaymentDue = t.Field<string>("PaymentDue"),
            PayDueDate = t.Field<DateTime>("PayDueDate"),
            UserLineId = t.Field<string>("UserLineId")
        }).ToList();

        return due;
    }

    public List<NoticePayment> ConvertDataTable_RESTNoticePayment(DataTable dt)
    {
        List<NoticePayment> payment = new List<NoticePayment>();
        payment = dt.AsEnumerable().Select(t => new NoticePayment()
        {
            Message = t.Field<string>("SMS"),
            UserLineId = t.Field<string>("UserLineId"),
            MobileNo = t.Field<string>("MobileNo")
        }).ToList();

        return payment;
    }
     public PaymentDue ConvertDataTabletoObject(DataTable dt)
     {
         PaymentDue data = new PaymentDue();
         if(dt.Rows.Count > 0)
         {
             data.AgreementNo = dt.Rows[0]["AgreementNo"].ToString();
             data.DueDate = dt.Rows[0]["DueDate"].ToString();
             data.Installment = decimal.Parse(dt.Rows[0]["Installment"].ToString());
             data.CollectionAmount = decimal.Parse(dt.Rows[0]["CollectionAmount"].ToString());
             data.PenaltyAmount = decimal.Parse(dt.Rows[0]["PenaltyAmount"].ToString());
         }

         return data;
     }
     public List<CustomerDataWithoutRegister> ConvertDataTableToListNotRegister(DataTable dt)
        {
            List<CustomerDataWithoutRegister> data = dt.AsEnumerable().Select(t => new CustomerDataWithoutRegister()
            {
                AgreementNo = t.Field<string>("AgreementNo"),
                Model = t.Field<string>("Model"),
                NetFinance = t.Field<decimal>("NetFinance"),
                InstallmentAmount = t.Field<decimal>("InstallmentAmount"),
                DueDate = t.Field<string>("DueDate"),
                LastDueDate = t.Field<DateTime>("LastDueDate"),
                OSBalance = t.Field<decimal>("OSBalance"),
                PeriodDue = t.Field<string>("PeriodDue"),
                CollectionAmount = t.Field<decimal>("CollectionAmount"),
                PenaltyAmount = t.Field<decimal>("PenaltyAmount"),
                OtherFee = t.Field<decimal>("OtherFee"),
                PaymentDue = t.Field<decimal>("PaymentDue"),
                PayDueDate = t.Field<DateTime>("PayDueDate"),
                ODPeriodDue = t.Field<string>("ODPeriodDue"),
                ODAmount = t.Field<decimal>("ODAmount"),
                isPastDue = t.Field<string>("isPastDue"),
                ContractStatus = t.Field<string>("ContractStatus"),
                CurrentInstallment = t.Field<decimal>("CurrentInstallment"),
                DiscountAmount = t.Field<decimal>("DiscountAmt"),
                SuspensionTenor = t.Field<string>("SuspensionTenor")
                // AgreementNo = t.Field<string>("AgreementNo"),
                // CustomerName = t.Field<string>("CustomerName"),
                // Email = t.Field<string>("Email"),
                // Model = t.Field<string>("Model"),
                // Color = t.Field<string>("Color"),
                // EngineNo = t.Field<string>("EngineNo"),
                // ChassisNo = t.Field<string>("ChassisNo"),
                // InstallmentAmount = t.Field<decimal>("InstallmentAmount"),
                // InstallmentPeriod = t.Field<int>("InstallmentPeriod"),
                // OSBalance = t.Field<decimal>("OSBalance"),
                // AccountStatus = t.Field<string>("AccountStatus"),
                // RegistrationNo = t.Field<string>("RegistrationNo"),
                // CollectionAmount = t.Field<decimal>("CollectionAmount"),
                // ODAmount = t.Field<decimal>("ODAmount"),
                // PenaltyAmount = t.Field<decimal>("PenaltyAmount")
            }).ToList();

            return data;
        }
    
     public List<CustomerData> ConvertDataTableToList(DataTable dt)
        {
            List<CustomerData> data = dt.AsEnumerable().Select(t => new CustomerData()
            {
                AgreementNo = t.Field<string>("AgreementNo"),
                Model = t.Field<string>("Model"),
                NetFinance = t.Field<decimal>("NetFinance"),
                InstallmentAmount = t.Field<decimal>("InstallmentAmount"),
                DueDate = t.Field<string>("DueDate"),
                LastDueDate = t.Field<DateTime>("LastDueDate"),
                OSBalance = t.Field<decimal>("OSBalance"),
                PeriodDue = t.Field<string>("PeriodDue"),
                CollectionAmount = t.Field<decimal>("CollectionAmount"),
                PenaltyAmount = t.Field<decimal>("PenaltyAmount"),
                OtherFee = t.Field<decimal>("OtherFee"),
                PaymentDue = t.Field<decimal>("PaymentDue"),
                PayDueDate = t.Field<DateTime>("PayDueDate"),
                ODPeriodDue = t.Field<string>("ODPeriodDue"),
                ODAmount = t.Field<decimal>("ODAmount"),
                isPastDue = t.Field<string>("isPastDue"),
                ContractStatus = t.Field<string>("ContractStatus"),
                CurrentInstallment = t.Field<decimal>("CurrentInstallment"),
                DiscountAmount = t.Field<decimal>("DiscountAmt"),
                SuspensionTenor = t.Field<string>("SuspensionTenor"),
                CollectionFC = t.Field<decimal>("CollectionFC")
                // AgreementNo = t.Field<string>("AgreementNo"),
                // CustomerName = t.Field<string>("CustomerName"),
                // Email = t.Field<string>("Email"),
                // Model = t.Field<string>("Model"),
                // Color = t.Field<string>("Color"),
                // EngineNo = t.Field<string>("EngineNo"),
                // ChassisNo = t.Field<string>("ChassisNo"),
                // InstallmentAmount = t.Field<decimal>("InstallmentAmount"),
                // InstallmentPeriod = t.Field<int>("InstallmentPeriod"),
                // OSBalance = t.Field<decimal>("OSBalance"),
                // AccountStatus = t.Field<string>("AccountStatus"),
                // RegistrationNo = t.Field<string>("RegistrationNo"),
                // CollectionAmount = t.Field<decimal>("CollectionAmount"),
                // ODAmount = t.Field<decimal>("ODAmount"),
                // PenaltyAmount = t.Field<decimal>("PenaltyAmount")
            }).ToList();

            return data;
        }
    }

    public class NoticeDue
    {
        public string AgreementNo { get; set; }
        public string Period { get; set; }
        public string PaymentDue { get; set; }
        public DateTime PayDueDate { get; set; }
        public string UserLineId { get; set; }
    }

    public class NoticePayment
    {
        public string Message { get; set; }
        public string MobileNo { get; set; }
        public string UserLineId { get; set; }
    }

    public class PaymentDue{
        public string AgreementNo { get; set; }        
        public string DueDate { get; set; }
        public decimal Installment { get; set; }
        public decimal CollectionAmount { get; set; }
        public decimal PenaltyAmount { get; set; }
    }
    public class CustomerDataWithoutRegister
    {
        public string AgreementNo { get; set; }
        // public string CustomerName { get; set; }
        // public string Email { get; set; }
        public string Model { get; set; }
        public decimal NetFinance { get; set; }
        public decimal InstallmentAmount { get; set; }
        public string DueDate { get; set; }
        public DateTime LastDueDate { get; set; }
        public decimal OSBalance { get; set; }
        public string PeriodDue { get; set; }
        public decimal CollectionAmount { get; set; }
        public decimal PenaltyAmount { get; set; }
        public decimal OtherFee { get; set; }
        public decimal PaymentDue { get; set; }
        public DateTime PayDueDate { get; set; }
        public string ODPeriodDue { get; set; }
        public decimal ODAmount { get; set; }
        public string isPastDue { get; set; }
        public string ContractStatus { get; set; }
        public decimal CurrentInstallment { get; set; }    
        public decimal DiscountAmount { get; set; }  
        public string SuspensionTenor { get; set; }
    }
    public class CustomerData
    {
        public string AgreementNo { get; set; }
        // public string CustomerName { get; set; }
        // public string Email { get; set; }
        public string Model { get; set; }
        public decimal NetFinance { get; set; }
        public decimal InstallmentAmount { get; set; }
        public string DueDate { get; set; }
        public DateTime LastDueDate { get; set; }
        public decimal OSBalance { get; set; }
        public string PeriodDue { get; set; }
        public decimal CollectionAmount { get; set; }
        public decimal PenaltyAmount { get; set; }
        public decimal OtherFee { get; set; }
        public decimal PaymentDue { get; set; }
        public DateTime PayDueDate { get; set; }
        public string ODPeriodDue { get; set; }
        public decimal ODAmount { get; set; }
        public string isPastDue { get; set; }
        public string ContractStatus { get; set; }
        public decimal CurrentInstallment { get; set; }    
        public decimal DiscountAmount { get; set; }  
        public string SuspensionTenor { get; set; }
        public decimal CollectionFC { get; set; }
        
        
        
    }
}