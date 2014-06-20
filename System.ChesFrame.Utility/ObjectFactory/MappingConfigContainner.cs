using System;
using System.ChesFrame.Utility.DataAccess.DataConfig;
using System.ChesFrame.Utility.ObjectFactory.Config;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.ObjectFactory
{
    public class MappingConfigContainner
    {
        private SafeDictionary<string, Type> typeMapping = new SafeDictionary<string, Type>();

        //private SafeDictionary<Type, Func<object>> dic = new SafeDictionary<Type, Func<object>>();

       // private SafeDictionary<string, Type> typeMapping = new SafeDictionary<string, Type>();

        private bool loaded = false;

        private string directorStr = "";

        private MappingConfigContainner() { }

        public static MappingConfigContainner containner = null;
        private static object objLockKey = new object();

        public static MappingConfigContainner Instance
        {
            get{
                if (containner == null)
                {
                    lock (objLockKey)
                    {
                        if (containner == null)
                        {
                            containner = new MappingConfigContainner();
                        }
                    }
                }
                return containner;
            }
        }

        public string DirectorStr
        {
            get { return directorStr; }
            set { directorStr = value; }
        }

        public void Load()
        {
            if (string.IsNullOrEmpty(directorStr))
            {
                 var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                 directorStr = string.Format(@"{0}\Config\Mapping",baseDirectory.TrimEnd(new char[]{'\\'}));
            }

            var filePath = string.Format("{0}\\{1}", directorStr, "MappingConfig.Config");

            var mappingConfig = SerializeHelper.XmlDeserialize<MappingConfig>(filePath);

            //Dictionary<string, Type> typeMapping = new Dictionary<string, Type>();
            SafeDictionary<string, Type> typeMapping = new SafeDictionary<string, Type>();
            foreach (var sub in mappingConfig.Items)
            {
                Func<string, string> dllPath = (p) =>
                {
                    return string.Format("{0}bin\\{1}", System.AppDomain.CurrentDomain.BaseDirectory,p);
                };

                Type[] typesAbstract = Assembly.LoadFrom(dllPath(sub.Abstract)).GetTypes();//并自定加载程序集的依赖
                Type[] typesImplement = Assembly.LoadFrom(dllPath(sub.Implement)).GetTypes();
                //所有的接口
                for (int i = 0; i < typesAbstract.Count(); i++)
                {
                    Type curentType = typesAbstract[i];
                    if (!(curentType.IsInterface || curentType.IsAbstract))
                    {
                        continue; ;
                    }
                    for (int j = 0; j < typesImplement.Count(); j++)
                    {
                        if (curentType.IsAssignableFrom(typesImplement[j]))
                        {
                            typeMapping.Add(curentType.FullName, typesImplement[j]);
                        }
                    }
                }
                this.typeMapping = typeMapping;
            }
            loaded = true;
        }

        public T GetTypeByAbstract<T>()
        {
            var abstractType = typeof(T);
            if (loaded == false)
            {
                Load();
            }

            if (typeMapping.ContainsKey(abstractType.FullName))
            {
                var type= typeMapping[abstractType.FullName] ;
                var newExpression = Expression.New(type);
                return Expression.Lambda<Func<T>>(newExpression, null).Compile()();
            }
            else
            {
                if (abstractType.IsClass &&
                    abstractType.GetConstructor(null) != null)
                {
                    var newExpression = Expression.New(abstractType);
                    return Expression.Lambda<Func<T>>(newExpression, null).Compile()();
                }
                else
                {
                    var errorMsg = string.Format("没能找到类型{0}的实现类型", abstractType.FullName);
                    throw new KeyNotFoundException(errorMsg);
                }
            }

        }
    }
}
