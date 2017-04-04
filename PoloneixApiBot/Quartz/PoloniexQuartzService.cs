using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using PoloneixApiBot.Quartz.QuartzJob;

namespace PoloneixApiBot.Quartz
{
    public class PoloniexQuartzService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static IScheduler _scheduler;

        public void StartService()
        {
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();
            _scheduler.Start();
            Logger.Info("Starting PoloniexQuartzService Scheduler");
            AddJob();
        }

        private void AddJob()
        {
            var cronExpression = 0;
            cronExpression = Settings.GetConfigInt("PoloniexJob_IntervalInMinute", 10);
           
            IJobDetail job = JobBuilder.Create<TradingJob>().Build();

            ITrigger trigger = TriggerBuilder.Create().WithIdentity("trigger1", "group1").
                StartNow()
                .WithSimpleSchedule(s => s.WithIntervalInMinutes(cronExpression).
                RepeatForever())
                .Build();

            _scheduler.ScheduleJob(job, trigger);
        }
    }
}
