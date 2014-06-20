using System;
using System.ChesFrame.Utility.DataAccess;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.DataAccess.DataConfig
{
    /// <summary>
    /// 数据配置加载器
    /// </summary>
    public class DBConfigLoader
    {
        #region Property

        private string _ConfigDirectory;
        /// <summary>
        /// 配置文件的目录
        /// </summary>
        public string ConfigDirectory
        {
            get { return _ConfigDirectory; }
            set { _ConfigDirectory = value; }
        }

        private string _ServerConfigFileName = "DbServer.config";//默认值
        /// <summary>
        /// 连接字符串配置文件
        /// </summary>
        public string ServerConfigFileName
        {
            get { return _ServerConfigFileName; }
            set { _ServerConfigFileName = value; }
        }
        
        private string _DBCommandFilesConfigName = "DbCommandFiles.config";
        /// <summary>
        /// 脚本配置文件
        /// </summary>
        public string DBCommandFilesConfigName 
        {
            get { return _DBCommandFilesConfigName; }
            set { _DBCommandFilesConfigName = value; }

        }

        private ConnectionStringConfig _ConnectionStringConfig ;
        public ConnectionStringConfig ConnectionStringConfig {
            get { return _ConnectionStringConfig; }
            private set { _ConnectionStringConfig = value; }
        }

        private List<DBCommand> _DBCommandList;
        public List<DBCommand> DBCommandList
        {
            get { return _DBCommandList; }
            private set { _DBCommandList = value; }
        }

        #endregion

        #region Static Singleton Instance

        public static object objLockKey = new object();

        private static DBConfigLoader configLoader = null;

        public static DBConfigLoader Instance 
        {
            get
            {
                if (configLoader == null)
                {
                    lock (objLockKey)
                    {
                        if (configLoader == null)
                        {
                            configLoader = new DBConfigLoader();
                        }
                    }
                }
                return configLoader;
            }
        }

        #endregion

        #region Constructor
        private DBConfigLoader() { }

        #endregion

        #region  Methods
        public void ReLoad()
        {
            //加载连接字符串配置
            if (!Directory.Exists(_ConfigDirectory))
            {
                var errorMsg = string.Format("文件目录{0}不存在", _ConfigDirectory);
                throw new DirectoryNotFoundException(errorMsg);
            }

            string ConnectionStringConfigFilePath = _ConfigDirectory.TrimEnd(new char[] { '/' }) + "/"
                + _ServerConfigFileName;

            if (!File.Exists(ConnectionStringConfigFilePath))
            {
                var errorMsg = string.Format("文件{0}不存在", ConnectionStringConfigFilePath);
                throw new FileNotFoundException(errorMsg);
            }

            var connectionStringConfig = SerializeHelper.
                XmlDeserialize<ConnectionStringConfig>(ConnectionStringConfigFilePath);

            _ConnectionStringConfig = connectionStringConfig;

            //加载脚本文件配置
            string dbCommandFileConfigFilePath = _ConfigDirectory.TrimEnd(new char[] { '/' }) + "/"
                + _DBCommandFilesConfigName;

            if (!File.Exists(dbCommandFileConfigFilePath))
            {
                var errorMsg = string.Format("文件{0}不存在", dbCommandFileConfigFilePath);
                throw new FileNotFoundException(errorMsg);
            }

            var dbCommandFileConfig = SerializeHelper.
                XmlDeserialize<DBCommandFilesConfig>(dbCommandFileConfigFilePath);

            //加载脚本Command配置
            if(dbCommandFileConfig.CommandFiles==null
                ||dbCommandFileConfig.CommandFiles.Count<=0 )
            {
                var errorMsg = "无Command数据配置文件";
                throw new FileNotFoundException(errorMsg);
            }

            List<DBCommand> dBCommandList = new List<DBCommand>();
            foreach (var sub in dbCommandFileConfig.CommandFiles)
            {
                var dbCommandConfigFilePath = _ConfigDirectory.TrimEnd(new char[] { '/' }) + "/"
                + sub.FilePath;

                if (!File.Exists(dbCommandConfigFilePath))
                {
                    continue;
                    //var errorMsg = string.Format("文件{0}不存在", dbCommandConfigFilePath);
                    //throw new FileNotFoundException(errorMsg);
                }

                var dbCommandConfig = SerializeHelper.
                XmlDeserialize<DBCommandFileConfig>(dbCommandConfigFilePath);
                if (dbCommandConfig != null && dbCommandConfig.DBCommands.Count() > 0)
                {
                    dBCommandList.AddRange(dbCommandConfig.DBCommands);
                }
            }

            if (_DBCommandList != null
                && _DBCommandList.Count() > 0) 
            {
                _DBCommandList.Clear();
            }
            _DBCommandList = dBCommandList;
        }

        #endregion
    }
}
