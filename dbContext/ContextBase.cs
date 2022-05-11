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
            if (_state.ConnectionStrings.isProd == "1")
            {
                return _state.ConnectionStrings.prod;
            }
            else
            { 
                return _state.ConnectionStrings.dev;
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

            public ResultAccess(IOptions<StateConfigs> option) : base(option.Value.ConnectionStrings.prod)
            {
                string cons = "";
                _dal = new ContextBase(option);
                cons = AESEncrypt.AESOperation.DecryptString(option.Value.ConnectionStrings.prod);
                _dal._SqlDb = new SqlConnection(cons);
                dt = new DataTable();
                _func = new Functional();
            }
            public DataTable ExecuteDataTable(Statement sql, string key = "Default")
            {
                try
                {
                     dt = new DataTable();
                    if(key != "Default")
                    {
                        _dal._SqlDb = new SqlConnection(AESEncrypt.AESOperation.DecryptString(key));
                        _dal._SqlDb.Open();
                        _command = new SqlCommand(sql.GetStatement(), _dal._SqlDb);
                        AddParameter(sql);
                        _adapter = new SqlDataAdapter(_command);
                        _adapter.Fill(dt);
                        _dal._SqlDb.Close();
                    }
                    else
                    {
                        _command = new SqlCommand(sql.GetStatement(), _dal._SqlDb);
                        _dal._SqlDb.Open();
                        AddParameter(sql);
                        _adapter = new SqlDataAdapter(_command);
                        _adapter.Fill(dt);
                        _dal._SqlDb.Close();
                        KeepLogDataBase("", sql.GetStatement());
                    }
                }
                catch (Exception e)
                {
                    KeepLogDataBase(e.Message, sql.GetStatement());
                    _func.SerializeObject(dt, StatusHttp.InternalError, e.Message);
                }
                finally
                {
                    if(dt.Rows.Count > 0)
                    {
                        _func.SerializeObject(dt, StatusHttp.OK, "");
                    }
                    else
                    {
                        _func.SerializeObject(dt, StatusHttp.NotFound, "");
                    }
                }

                return dt;
            }

            public void KeepLogDataBase(string error, string json = "")
            {
                Statement sql = new Statement();
                sql.AppendStatement("EXEC REST_KeepLogRequest @error, @json");
                sql.AppendParameter("@error", error);
                sql.AppendParameter("@json", json);
                _dal._SqlDb.Open();
                _command = new SqlCommand(sql.GetStatement(), _dal._SqlDb);
                AddParameter(sql);
                _adapter = new SqlDataAdapter(_command);
                _adapter.Fill(dt);
                _dal._SqlDb.Close();
                
            }

            public string ExecutenonResult(Statement sql, string key = "Default")
            {
                string result = "";
                if(key != "Default")
                {
                    _dal._SqlDb = new SqlConnection(AESEncrypt.AESOperation.DecryptString(key));
                    _dal._SqlDb.Open();
                    _command = new SqlCommand(sql.GetStatement(), _dal._SqlDb);
                    AddParameter(sql);
                    _command.ExecuteNonQuery();
                    _dal._SqlDb.Close();
                }
                else
                {
                    ResponseModel resultSet = new ResponseModel();
                    try
                    {
                        _dal._SqlDb.Open();
                        _command = new SqlCommand(sql.GetStatement(), _dal._SqlDb);
                        AddParameter(sql);
                        _command.ExecuteNonQuery();
                        _dal._SqlDb.Close();
                    }
                    catch (Exception e)
                    {
                        _dal._SqlDb.Close();
                        return e.Message;
                    }
                    finally
                    {
                        result = StatusHttp.OK.ToString();
                    }
                }

                return result;
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