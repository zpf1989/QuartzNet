using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpTaskService
{
    public class HttpJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {

            string url = "";
            try
            {
                url = context.JobDetail.Description;
                if (string.IsNullOrEmpty(url))
                {
                    logger.Error(string.Format("{0}error:{0}", "未找到url，请检查job节点下description是否设置了正确的url"));
                    return;
                }
                var response = HttpHelper.Get(url);
                logger.Info(string.Format("{0}request:{0}\turl,{1}{0}response:{0}\t{2}", Environment.NewLine, url, response));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("{0}request:{0}\turl,{1}{0}\t{1}{0}error:{0}\t{2}", Environment.NewLine, url, ex.Message));
            }
        }
    }
}
