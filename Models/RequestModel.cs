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