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

    #region FileManagement
    public class RequestFileModel
    {
        public string token { get; set; }
        public string sendfrom { get; set; }
        public string files { get; set; }
    }
    #endregion
}