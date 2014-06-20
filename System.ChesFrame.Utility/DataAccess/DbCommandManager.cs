using System;
using System.ChesFrame.Utility.DataAccess.DataConfig;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.DataAccess
{
    public class DbCommandManager
    {
        //预留着对对象clone缓存
        private  ConcurrentDictionary<string, DbCommand> dbCommands =
            new ConcurrentDictionary<string, DbCommand>();

        private DBConfigLoader configLoader = DBConfigLoader.Instance;

        private DbCommandManager() { }

        private static  DbCommandManager signalDbCommandContainer = null;
        private static object lockKey = new object();

        public static DbCommandManager Instance
        {
            get
            {
                if (signalDbCommandContainer == null)
                {
                    lock (lockKey)
                    {
                        if (signalDbCommandContainer == null)
                        {
                            signalDbCommandContainer = new DbCommandManager();
                        }
                    }
                }
                return signalDbCommandContainer;
            }
        }

        //加载DbCommand到缓存(暂时有用到)
        private void Load(ConnectionStringConfig connectionStrConfig,List<DBCommand> dBCommandList)
        {
            foreach (var sub in dBCommandList)
            {
                var curentConnectionStringItem = connectionStrConfig.ConnectionStrings.ConnectionString.FirstOrDefault((m) => { return m.Name == sub.ServerName; });
                if (curentConnectionStringItem == null)
                {
                    continue;
                }

                var curentProvidorName = curentConnectionStringItem.DbProviderName ?? "System.Data.SqlClient";

                var dataAccessObjectCreater = DataAccessObjectCreater.Instance;
                dataAccessObjectCreater.ProvidorName = curentProvidorName;

                var dbConnection = dataAccessObjectCreater.CreateDbConnection();
                dbConnection.ConnectionString = curentConnectionStringItem.ConnectionString;

                var dbCommand = dataAccessObjectCreater.CreateDbCommand();
                dbCommand.Connection = dbConnection;

                dbCommand.CommandText = sub.CommandText;
                dbCommand.CommandType = string.IsNullOrEmpty(sub.CommandType) ? CommandType.Text : (CommandType)Enum.Parse(typeof(CommandType), sub.CommandType);

                foreach (var n in sub.Parameters)
                {
                    var curentParameter = dataAccessObjectCreater.CreateDbParameter();
                    curentParameter.DbType = (DbType)Enum.Parse(typeof(DbType), n.DbType);
                    curentParameter.ParameterName = n.Name;
                    curentParameter.Direction = string.IsNullOrEmpty(n.Direction) ? ParameterDirection.Input :
                        (ParameterDirection)Enum.Parse(typeof(ParameterDirection), n.Direction);
                    if (n.Size > 0)//部分类型不用设置类型 比如：int
                    {
                        curentParameter.Size = n.Size;
                    }
                    if (n.Value != null)
                    {
                        curentParameter.Value = n.Value;
                    }
                    dbCommand.Parameters.Add(curentParameter);
                }

                dbCommands.AddOrUpdate(sub.Name, dbCommand, (key, value1) => { return value1; });
            }
        }

        //根据配置信息创建DbCommand
        private DbCommand LoadByDbCommandName(ConnectionStringConfig connectionStrConfig, List<DBCommand> dBCommandList,
            string name)
        {
            DBCommand sub = null;
            sub = dBCommandList.FirstOrDefault(p => { return p.Name == name; });
            if (sub == null)
            {
                var errorMsg = string.Format("键{0}不存在", name);
                throw new KeyNotFoundException(errorMsg);
            }

            var curentConnectionStringItem = connectionStrConfig.ConnectionStrings.ConnectionString.FirstOrDefault((m) => { return m.Name == sub.ServerName; });
            if (curentConnectionStringItem == null)
            {
                var errorMsg = string.Format("无效的连接字符串关键字", sub.ServerName);
                throw new KeyNotFoundException(errorMsg);
            }

            var curentProvidorName = curentConnectionStringItem.DbProviderName ?? "System.Data.SqlClient";

            var dataAccessObjectCreater = DataAccessObjectCreater.Instance;
            dataAccessObjectCreater.ProvidorName = curentProvidorName;

            var dbConnection = dataAccessObjectCreater.CreateDbConnection();
            dbConnection.ConnectionString = curentConnectionStringItem.ConnectionString;

            var dbCommand = dataAccessObjectCreater.CreateDbCommand();
            dbCommand.Connection = dbConnection;

            dbCommand.CommandText = sub.CommandText;
            dbCommand.CommandType = string.IsNullOrEmpty(sub.CommandType) ? CommandType.Text : (CommandType)Enum.Parse(typeof(CommandType), sub.CommandType);

            foreach (var n in sub.Parameters)
            {
                var curentParameter = dataAccessObjectCreater.CreateDbParameter();
                curentParameter.DbType = (DbType)Enum.Parse(typeof(DbType), n.DbType);
                curentParameter.ParameterName = n.Name;
                curentParameter.Direction = string.IsNullOrEmpty(n.Direction) ? ParameterDirection.Input :
                    (ParameterDirection)Enum.Parse(typeof(ParameterDirection), n.Direction);
                if (n.Size > 0)//部分类型不用设置类型 比如：int
                {
                    curentParameter.Size = n.Size;
                }
                if (n.Value != null)
                {
                    curentParameter.Value = n.Value;
                }
                dbCommand.Parameters.Add(curentParameter);
            }
            return dbCommand;
        }

        //加载配置信息
        public void Load(string configDirectory)
        {
            configLoader.ConfigDirectory = configDirectory;
            configLoader.ReLoad();
        }
        
        public void Load()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var defaultDirectory = string.Format(@"{0}\Config\Data",baseDirectory.TrimEnd(new char[]{'\\'}));
            configLoader.ConfigDirectory = defaultDirectory;
            configLoader.ReLoad();
        }

        //获取DbCommand
        private DbCommand GetDbCommandByName(string name)
        {
            if (string.IsNullOrEmpty(configLoader.ConfigDirectory))
            {
                //configLoader.lo();
                this.Load();
            }

            return LoadByDbCommandName(configLoader.ConnectionStringConfig, configLoader.DBCommandList, name);
        }

        public static DbCommand GetDbCommand(string name)
        {
            return DbCommandManager.Instance.GetDbCommandByName(name);
        }
    }

    public class DataAccessObjectCreater
    {
        //线程安全
        private static ConcurrentDictionary<string, DbProviderFactory> dbProviderFactorys =
            new ConcurrentDictionary<string, DbProviderFactory>();

        private string _ProvidorName;

        public string ProvidorName
        {
            set { _ProvidorName = value; }
            get { return _ProvidorName; }
        }

        public static DataAccessObjectCreater Instance
        {
            get
            {
                return new DataAccessObjectCreater();
            }
        }

        private DataAccessObjectCreater() { }


        public DbCommand CreateDbCommand(string providorName)
        {
            return GetDbProviderFactory(providorName).CreateCommand();
        }

        public DbConnection CreateDbConnection(string providorName)
        {
            return GetDbProviderFactory(providorName).CreateConnection();
        }

        public DbParameter CreateDbParameter(string providorName)
        {
            return GetDbProviderFactory(providorName).CreateParameter();
        }

        private DbProviderFactory GetDbProviderFactory(string providorName)
        {
            if (dbProviderFactorys.ContainsKey(providorName))
            {
                return dbProviderFactorys[providorName];
            }
            else
            {
                var dbProviderFactory = DbProviderFactories.GetFactory(providorName);
                var dbProviderFactory2 = dbProviderFactorys.GetOrAdd(providorName, dbProviderFactory);
                return dbProviderFactory2;//可能与dbProviderFactory是一样的可能不一样
            }
        }

        public DbCommand CreateDbCommand()
        {
            return GetDbProviderFactory(_ProvidorName).CreateCommand();
        }

        public DbConnection CreateDbConnection()
        {
            return GetDbProviderFactory(_ProvidorName).CreateConnection();
        }

        public DbParameter CreateDbParameter()
        {
            return GetDbProviderFactory(_ProvidorName).CreateParameter();
        }
    }
}
