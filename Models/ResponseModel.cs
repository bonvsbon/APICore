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
        public DataTable _data { get; set; }
        public string _errorMessage { get; set; }
        public string _statusCode { get; set; }
         public string GetStateHttp(StatusHttp code)
        {
            string resultCode = "";
            switch (code)
            {
                case StatusHttp.Created:
                    resultCode = "201";
                    break;
                case StatusHttp.Accepted:
                    resultCode = "202";
                    break;
                case StatusHttp.InvalidToken:
                    resultCode = "400";
                    break;
                case StatusHttp.SecurityError:
                    resultCode = "401";
                    break;
                case StatusHttp.NotFound:
                    resultCode = "404";
                    break;
                case StatusHttp.InternalError:
                    resultCode = "500";
                    break;
                default:
                    resultCode = "200";
                    break;
            }

            return resultCode;
        }



        public enum StatusHttp
        {
            OK,
            Created,
            Accepted,
            NotFound,
            InternalError,
            InvalidToken,
            SecurityError
        }
    }

    
    public class OTPResultModel
    {
        public string message { get; set; }
    }

    public class RequestPaymentGatewayModel
    {
        public string IDCard { get; set; }
        public string AgreementNo { get; set; }
    }

    public class AccountBarcodeModel
    {
        public string AgreementNo { get; set; }
        public string NextCard { get; set; }
    }

    public class PaymentModel
    {
        public string refCard { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class SMSResponse 
    {
        public string phoneNumber { get; set; }
        // public string OTP { get; set; }   
        public string result { get; set; }     
    }
    
    public class SMSModel
    {
        public string User { get; set; }
        public string Pass { get; set; }
        public string Type { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Text { get; set; }
        public string ServID { get; set; }

    }
}