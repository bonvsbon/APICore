namespace APICore.Models
{
    #region Contract
    public class RequestModel
    {
        public string token { get; set; }
        public RequestDataModel data { get; set; }
        public string sendfrom { get; set; }
    }
    public class RequestDataModel
    {
        public string AgreementNo { get; set; }
        public string Id { get; set; }

    }
    #endregion Contract

    #region Authentication
    public class RequestAuthorizeModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    #endregion
    
    #region ผูกบัญชี 
    public class AccountRequestModel 
    {
        public string UserId { get; set; }
        public string IDCard { get; set; }
        public string BirthDay { get; set; }
        public string OTP { get; set; }
    }
    public class AccountRequestModel2
    {
        public string IDCard { get; set; }
        public string BirthDay { get; set; }     
        public string NextCard { get; set; }
    }

    public class UnbindAccountRequestModel 
    {
        public string UserId { get; set; }
        public string IDCard { get; set; }    
    }

    public class AccountInformationModel 
    {
        public string UserId { get; set; }
        public string IDCard { get; set; }
        public string BirthDay { get; set; }
        public string AgreementNo { get; set; }
    }

    public class ExistingCustomer
    {
        public string LineUserId { get; set; }
        public string PhoneNumber { get; set; }
        // public string isConcent { get; set; }
    }
    public class CheckOTP
    {
        public string refOTP { get; set; }
        public string phoneNumber { get; set; }
    }

    public class MobileInformation
    {
        public string NewPhoneNumber { get; set; }
        public string LineUserID { get; set; }
        public bool isConsent { get; set; }
    }

    public class SingleValueModel
    {
        public string LineUserId { get; set; }
        public string AgreementNo { get; set; }
    }

    public class ResultModel
    {
        public string result { get; set; }
    }

    public class CustomerWithoutBind
    {
        public string IDCard { get; set; }
        public string BirthDay { get; set; }
        public string NextCard { get; set; }
        public string PhoneNumber { get; set; }
        public bool isConsent { get; set; }
    }
    public class InformationAddressWithoutBind
    {
        public string IDCard { get; set; }
        public string BirthDay { get; set; }
        public string NextCard { get; set; }
        public string AddressCode { get; set; }
        public string AddressNo { get; set; }
        public string Moo { get; set; }
        public string Soi { get; set; }
        public string RoomNo { get; set; }
        public string Floor { get; set; }
        public string Building { get; set; }
        public string Street { get; set; }
        public string District { get; set; }
        public string SubDistrict { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string isMaillingAddress { get; set; }
        public bool isConsent { get; set; }
    }
    #endregion
    
    public class CustomerDue
    {
        public string IDCard { get; set; }
        public string BirthDay { get; set; }
        public string AgreementNo { get; set; }     
    }

    #region FileManagement
    public class RequestFileModel
    {
        public string token { get; set; }
        public string sendfrom { get; set; }
        public string files { get; set; }
    }
    #endregion
}