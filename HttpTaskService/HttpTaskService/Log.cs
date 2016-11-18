using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]
namespace HttpTaskService
{
    public class Log
    {
        static ILog logger;
        public static ILog Logger
        {
            get
            {
                return logger;
            }
        }
        static Log()
        {
            logger = LogManager.GetLogger("LogHelper");
        }
    }
}
