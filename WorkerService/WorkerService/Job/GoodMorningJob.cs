using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Job
{
    class GoodMorningJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobKey jobKey = context.JobDetail.Key;
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

            string message = jobDataMap.GetString("Message");
            int yourLuckyNumber = jobDataMap.GetInt("YourLuckyNumber");

            await Console.Out.WriteLineAsync($"Job [{jobKey}]: {message}, Your today lucky number is: {yourLuckyNumber}");
        }
    }
}
