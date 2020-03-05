using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using WorkerService.Job;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };

            StdSchedulerFactory factory = new StdSchedulerFactory(props);

            IScheduler sched = await factory.GetScheduler();
            await sched.Start();

            IJobDetail blinkJob = JobBuilder.Create<BlinkJob>()
                .WithIdentity("BlinkJob", "FirstGroup")
                .UsingJobData("Message", "Blink your eyes")
                .UsingJobData("DateTimeNow", DateTime.Now.ToString())
                .Build();

            ITrigger triggerEvryFiveMinutes = TriggerBuilder.Create()
                .WithIdentity("Trigger", "FirstGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever())
                .Build();

            await sched.ScheduleJob(blinkJob, triggerEvryFiveMinutes);

            IJobDetail goodMorningJob = JobBuilder.Create<GoodMorningJob>()
                .WithIdentity("MorningJob", "FirstGroup")
                .UsingJobData("Message", "Good morning")
                .UsingJobData("YourLuckyNumber", new Random().Next(1, 100))
                .Build();

            ITrigger triggerDailyAtNine = TriggerBuilder.Create()
                .WithIdentity("TriggerDailyAtNine")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(9, 0))
                .Build();

            await sched.ScheduleJob(goodMorningJob, triggerDailyAtNine);
        }
    }
}
