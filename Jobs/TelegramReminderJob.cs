using daytask.Services;
using Quartz;

namespace daytask.Jobs
{
    public class TelegramReminderJob (ITelegramService telegramService) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var chatId = dataMap.GetString("ChatId");
            var message = dataMap.GetString("Message");

            await telegramService.SendMessageAsync(chatId, message);
        }
    }
}
