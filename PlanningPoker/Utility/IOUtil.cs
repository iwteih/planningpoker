using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using Newtonsoft.Json;

namespace PlanningPoker.Utility
{
    public class IOUtil
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region IsolatedStorageFile

        private static ApplicationConfig appConfig;
        private static readonly string CONFIG_FILE_NAME = "PlanningPoker.NET.settings" ;

        public static ApplicationConfig LoadIsolatedData()
        {
            if (appConfig == null)
            {
                if(File.Exists(CONFIG_FILE_NAME))
                {
                    string content = File.ReadAllText(CONFIG_FILE_NAME, Encoding.UTF8);
                    appConfig = JsonConvert.DeserializeObject<ApplicationConfig>(content);
                }

                appConfig = appConfig == null ? new ApplicationConfig() : appConfig;
            }
            return appConfig;
        }

        public static void SaveIsolatedData(ApplicationConfig config)
        {
            string contect = JsonConvert.SerializeObject(appConfig, Formatting.Indented);

            File.WriteAllText(CONFIG_FILE_NAME, contect);
        }
        #endregion

        public static byte[] SerializeToByteArray<T>(T obj) where T : class
        {
            if (obj == null)
            {
                return null;
            }

            byte[] byteContent = null;

            IFormatter formatter = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    formatter.Serialize(ms, obj);
                }
                catch (Exception exp)
                {
                    logger.Error(string.Format("SerializeToByteArray error: {0}", exp.Message));
                }

                byteContent = ms.GetBuffer();

                byteContent = ms.ToArray();
            }

            return byteContent;
        }

        public static T DeserializeFromByteArray<T>(byte[] byteArray) where T : class
        {
            if (byteArray == null
                || byteArray.Length == 0)
            {
                return default(T);
            }

            object obj = null;

            IFormatter formatter = new BinaryFormatter();
    
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                try
                {
                    obj = formatter.Deserialize(ms);
                }
                catch (Exception exp)
                {
                    logger.Error(string.Format("Deserialize error: {0}", exp.Message));
                }
            }

            T target = obj as T;

            return target;
        }
    }
}
