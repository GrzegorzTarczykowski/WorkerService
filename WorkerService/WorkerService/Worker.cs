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
            try
            {
                _logger.LogInformation("Scheduler starting...");
                await CreateJobs(await GetScheduler(stoppingToken), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
        }

        private async Task CreateJobs(IScheduler scheduler, CancellationToken stoppingToken)
        {
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

            await scheduler.ScheduleJob(blinkJob, triggerEvryFiveMinutes, stoppingToken);

            IJobDetail goodMorningJob = JobBuilder.Create<GoodMorningJob>()
                .WithIdentity("MorningJob", "FirstGroup")
                .UsingJobData("Message", "Good morning")
                .UsingJobData("YourLuckyNumber", new Random().Next(1, 100))
                .Build();

            ITrigger triggerDailyAtNine = TriggerBuilder.Create()
                .WithIdentity("TriggerDailyAtNine")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(9, 0))
                .Build();

            await scheduler.ScheduleJob(goodMorningJob, triggerDailyAtNine, stoppingToken);
        }

        private async Task<IScheduler> GetScheduler(CancellationToken stoppingToken)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory(new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            });

            var scheduler = await factory.GetScheduler();
            await scheduler.Start(stoppingToken);
            return scheduler;
        }
    }
}
