using log4net;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpTaskService
{
    public partial class HttpTaskService : ServiceBase
    {
        private readonly ILog logger;
        private IScheduler scheduler;

        public HttpTaskService()
        {
            InitializeComponent();

            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));
            logger = LogManager.GetLogger(GetType());

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler();

            scheduler.Start();
        }

        protected override void OnStart(string[] args)
        {
            scheduler.Start();
            logger.Info("Quartz服务成功启动");
        }

        protected override void OnStop()
        {
            scheduler.Shutdown();
            logger.Info("Quartz服务成功终止");
        }

        protected override void OnPause()
        {
            scheduler.PauseAll();
            logger.Info("Quartz服务暂停");
        }

        protected override void OnContinue()
        {
            scheduler.ResumeAll();
            logger.Info("Quartz服务恢复");

        }
    }
}
