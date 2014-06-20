using System;
using System.ChesFrame.Utility.DataAccess.DataConfig;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.SiteMap
{
    public class MvcSitemapConfigManager
    {
        private MvcSiteMapConfig siteMapConfig = null;

        private string _SiteMapConfigDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public string SiteMapConfigDirectory
        {
            get { return _SiteMapConfigDirectory; }
            set { _SiteMapConfigDirectory = value; } 
        }

        private string _SiteMapFileName = "MvcWeb.siteMap";//默认名称
        public string SiteMapFileName { get {
            return _SiteMapFileName;
        }
            set {
                _SiteMapFileName = value;
            }
        }

        public void LoadSiteMap()
        {
            if (!Directory.Exists(_SiteMapConfigDirectory))
            {
                var errorMsg = string.Format("文件目录{0}不存在", _SiteMapConfigDirectory);
                throw new DirectoryNotFoundException(errorMsg);
            }

            string sitemapFilePath = _SiteMapConfigDirectory.TrimEnd(new char[] { '/' }) + "/"
                + _SiteMapFileName;

            if (!File.Exists(sitemapFilePath))
            {
                var errorMsg = string.Format("文件{0}不存在", sitemapFilePath);
                throw new FileNotFoundException(errorMsg);
            }

            var siteMap = SerializeHelper.
               XmlDeserialize<MvcSiteMapConfig>(sitemapFilePath);

            siteMapConfig = siteMap;
        }

        public MvcSiteMapConfig SiteMapConfig {
            get
            {
                if (siteMapConfig == null)
                {
                    LoadSiteMap();
                }
                return siteMapConfig;
            }
        }

        #region Single
        private MvcSitemapConfigManager() {
            LoadSiteMap();
        }

        private static MvcSitemapConfigManager _Instance = null;
        private static object objLock = new object();
        public static MvcSitemapConfigManager Instance
        {

            get {
                if (_Instance == null)
                {
                    lock (objLock)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new MvcSitemapConfigManager();
                        }
                    }
                }
                return _Instance;
            }
         }

        #endregion

    }
}
