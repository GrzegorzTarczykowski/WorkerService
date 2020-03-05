using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Job
{
    class BlinkJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobKey jobKey = context.JobDetail.Key;
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

            string message = jobDataMap.GetString("Message");
            DateTime dateTimeNow = jobDataMap.GetDateTime("DateTimeNow");

            await Console.Out.WriteLineAsync($"Job [{jobKey}]: {message}, DateTime: {dateTimeNow}");
        }
    }
}
