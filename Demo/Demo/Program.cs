using Common.Logging;
using Common.Logging.Simple;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter { Level = LogLevel.Info };

                // Grab the Scheduler instance from the Factory 
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();
                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                //Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "groups")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                scheduler.ScheduleJob(job, trigger);

                // some sleep to show what's happening
                Thread.Sleep(TimeSpan.FromSeconds(60));

                //and last shut down the scheduler when you are ready to close your program
                scheduler.Shutdown();

            }
            catch (SchedulerException ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }
    }
}
