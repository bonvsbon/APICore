namespace APICore.Models
{
    #region Contract
    public class RequestModel
    {
        public string _token { get; set; }
        public RequestDataModel _data { get; set; }
        public string _sendfrom { get; set; }
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
        public string _token { get; set; }
        public string _sendfrom { get; set; }
        public string _files { get; set; }
    }
    #endregion
}