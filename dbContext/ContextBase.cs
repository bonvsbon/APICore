using System.Threading;
using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Options;
using DataTable = System.Data.DataTable;
using APICore.Models;
using APICore.Common;
using static APICore.Models.appSetting;
using static APICore.Models.ResponseModel;



namespace APICore.dbContext
{
    public class ContextBase : IDisposable
    {


  private string _connStr = "";
        private bool _isTrans = false;
        private bool _isSqlServer = false;
        private bool _isOracle = false;
        private bool _isPostgresQL = false;
        public bool isOpen = false;
        public int _commandTimeout = 15;
        private DBType _dbType;
        private OleDbTransaction _OleDb;
        private SqlConnection _SqlDb;
        private OleDbTransaction _OleDbTx;
        private SqlTransaction _SqlDbTx;
        private readonly IOptions<StateConfigs> _conf;
        public static StateConfigs _state = new StateConfigs();

        public ContextBase(IOptions<StateConfigs> options)
        {
            _conf = options;
            _state = options.Value;
        }

        public ContextBase(string ConString) : this(ConString, ConString.ToUpper().IndexOf(".MDB") > 1 || ConString.ToUpper().IndexOf(".XLS") > 1 || ConString.ToUpper().IndexOf(".CSV") > 1 ? DBType.OleDb : DBType.SqlServer)
        {
            
            _SqlDb = new SqlConnection(AESEncrypt.AESOperation.DecryptString(ConString));
        }
        public string BaseConnectionString()
        {
            if (bool.Parse(_state.ConnectionStrings.isProd))
            {
                return _state.ConnectionStrings._prod;
            }
            else
            { 
                return _state.ConnectionStrings._dev;
            }
        }
        public ContextBase(string connectionString, DBType databaseType)
        {
            this._connStr = connectionString;
            this._dbType = databaseType;
            this.isOpen = false;
            if (string.IsNullOrEmpty(this._connStr)) this._connStr = BaseConnectionString();
            switch (databaseType)
            {
                case DBType.SqlServer:
                    this._isSqlServer = true;
                    this._isOracle = false;
                    this._isPostgresQL = false;
                    break;
                case DBType.Oracle:
                    this._isOracle = true;
                    this._isSqlServer = false;
                    this._isPostgresQL = false;
                    break;
                case DBType.PostgresQL:
                    this._isPostgresQL = true;
                    this._isSqlServer = false;
                    this._isOracle = false;
                    break;
                default:
                    this._isOracle = false;
                    this._isSqlServer = this._connStr.ToUpper().IndexOf(".MDB") <= 1 && this._connStr.ToUpper().IndexOf(".XLS") <= 1;
                    break;
            }
        }

        #region State Connection
        public void Dispose()
        {
            _SqlDb.Dispose();
        }
        public void Open()
        {
            if (_SqlDb.State == ConnectionState.Closed)
            {
                _SqlDb.Open();
            }
        }

        public void Close()
        {
            _SqlDb.Close();
        }

        #endregion End State Connection

        public class Statement
        {
            private StringBuilder _statement = new StringBuilder();
            private SortedList hash = new SortedList();
            public Statement() { }

            #region Statement
            public void AppendStatement(string value) => this._statement.Append(value);
            public bool AppendParameter(string key, object value)
            {
                if (string.IsNullOrEmpty(key) || value == null)
                {
                    return false;
                }
                this.hash.Add(key, value);

                return true;
            }

            public SortedList GetParams()
            {
                return this.hash;
            }
            public string GetStatement()
            {
                return this._statement.ToString();
            }
            #endregion Statement

        }

        #region ส่วนการดึงข้อมูลจากฐานข้อมูล
        public class ResultAccess : ContextBase
        {
            public ContextBase _dal;
            public DataTable dt;
            public SqlCommand _command;
            public SqlDataAdapter _adapter;
            public ResponseModel responseModel;
            public Functional _func;

            public ResultAccess(IOptions<StateConfigs> option) : base(option.Value.ConnectionStrings._prod)
            {
                string cons = "";
                _dal = new ContextBase(option);
                cons = AESEncrypt.AESOperation.DecryptString(option.Value.ConnectionStrings._prod);
                _dal._SqlDb = new SqlConnection(cons);
                dt = new DataTable();
                responseModel = new ResponseModel();
                _func = new Functional();
            }
            public DataTable ExecuteDataTable(Statement sql)
            {
                ResponseModel resultSet = new ResponseModel();
                try
                {
                    _dal._SqlDb.Open();
                    _command = new SqlCommand(sql.GetStatement(), _dal._SqlDb);
                    AddParameter(sql);
                    _adapter = new SqlDataAdapter(_command);
                    _adapter.Fill(dt);
                    _dal._SqlDb.Close();
                }
                catch (Exception e)
                {
                    resultSet = _func.SerializeObject(dt, StatusHttp.InternalError, e.Message);
                }
                finally
                {
                    resultSet = _func.SerializeObject(dt, StatusHttp.OK, "");
                }

                return dt;
            }
            public ResponseModel ExecutenonResult(Statement sql)
            {
                ResponseModel resultSet = new ResponseModel();
                try
                {
                    _dal._SqlDb.Open();
                    _command = new SqlCommand(sql.GetStatement(), _dal._SqlDb);
                    AddParameter(sql);
                    _adapter = new SqlDataAdapter(_command);
                    _adapter.Fill(dt);
                    _dal._SqlDb.Close();
                }
                catch (Exception e)
                {
                    resultSet = _func.SerializeObject(dt, StatusHttp.InternalError, e.Message);
                }
                finally
                {
                    resultSet = _func.SerializeObject(dt, StatusHttp.OK, "");
                }

                return resultSet;
            }

            public object GetSingleValue(Statement sql)
            {
                _dal._SqlDb.Open();
                _command = new SqlCommand(sql.GetStatement(), _dal._SqlDb);
                AddParameter(sql);
                _adapter = new SqlDataAdapter(_command);
                _adapter.Fill(dt);
                _dal._SqlDb.Close();


                return dt.Rows.Count > 0 ? (object)dt.Rows[0]["ResultSet"] : null;
            }

            public void AddParameter(Statement sql)
            {
                SortedList list = sql.GetParams();
                if (list.Count > 0)
                {
                    foreach (object key in (IEnumerable)list.Keys)
                    {
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = (string)key;
                        param.Value = list[key];
                        this._command.Parameters.Add(param);
                    }
                }
            }

        }

        #endregion
        public enum DBType : byte
        {
            OleDb,
            SqlServer,
            Oracle,
            PostgresQL
        }
    }
}