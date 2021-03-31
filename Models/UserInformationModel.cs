namespace APICore.Models{
    public class UserInformationModel
    {
        public UserInformationModel()
        {
            
        }
        private string _employeecode;
        public string EmployeeCode
        {
            get { return _employeecode; }
            set { _employeecode = value; }
        }
        private string _employeename;
        public string EmployeeName
        {
            get { return _employeename; }
            set { _employeename = value; }
        }
        
        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        
        //private string EmpPassword;
        //public string Password
        //{
        //    get { return EmpPassword; }
        //    set { EmpPassword = value; }
        //}

        private string _token;

        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }
    }
}