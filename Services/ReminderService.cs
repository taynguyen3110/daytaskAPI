using daytask.Dtos;
using daytask.Exceptions;
using daytask.Jobs;
using daytask.Models;
using daytask.Repositories;
using Microsoft.AspNetCore.SignalR.Protocol;
using Quartz;
using System.Threading.Tasks;

namespace daytask.Services
{
    public class ReminderService (ISchedulerFactory schedulerFactory, IUserRepository userRepository, ILogger<ReminderService> logger) : IReminderService
    {
        // Create a reminder: schedule a new Quartz job
        public async Task CreateReminderAsync(UserTask task)
        {
            try
            {
                if (task.Completed || task.DueDate < DateTimeOffset.Now)
                {
                    return;
                }
                logger.LogInformation($"Start creating reminders");
                var chatId = await userRepository.GetChatIdByUserIdAsync(task.UserId);
                if (task.Reminder is not null)
                {
                    // Create job for reminder
                    var reminderMessage = $"Reminder: Don’t forget! Your task “{task.Title}” is scheduled for {task.DueDate.ToLocalTime().ToString("f")}. Stay on track and get it done!";
                    var reminder = new ReminderDto()
                    {
                        ChatId = chatId,
                        TaskId = task.Id.ToString(),
                        Message = reminderMessage,
                        ReminderGroup = "Reminder",
                        RemindAt = (DateTimeOffset)task.Reminder,
                        Recurrence = task.Recurrence is not null ? task.Recurrence : ""
                    };
                    logger.LogInformation($"Reminder data created");
                    await CreateReminderHelperAsync(reminder);
                }

                // Create job for due reminder
                var dueMessage = $"Task Due: Your task “{task.Title}” is now due. Time to wrap it up!";
                var dueReminder = new ReminderDto()
                {
                    ChatId = chatId,
                    TaskId = $"due_{task.Id}",
                    Message = dueMessage,
                    ReminderGroup = "Due_Reminder",
                    RemindAt = task.DueDate,
                    Recurrence = task.Recurrence is not null ? task.Recurrence : ""
                };
                await CreateReminderHelperAsync(dueReminder);

                logger.LogInformation($"Reminder for task: {task.Title} is created");
            }
            catch (Exception ex)
            {
                throw new AppException("Failed to create reminder");
            }
        }
        public async Task CreateRemindersAsync(IEnumerable<UserTask> tasks)
        {
            var scheduler = await schedulerFactory.GetScheduler();

            foreach (var task in tasks)
            {
                try
                {
                    await CreateReminderAsync(task);
                    logger.LogInformation($"Reminder for task [{task.Id.ToString()}] scheduled");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to create reminder for task [{task.Id.ToString()}]");
                }
            }
        }
        // Update reminder: reschedule existing job
        public async Task UpdateReminderAsync(UserTask task)
        {
            await DeleteReminderAsync(task.Id.ToString());
            await CreateReminderAsync(task);
            logger.LogInformation($"Reminder of task: {task.Id} is updated");
        }
        public async Task UpdateRemindersAsync(IEnumerable<UserTask> tasks)
        {
            foreach (var task in tasks)
            {
                await UpdateReminderAsync(task);
            }
        }
        // Delete reminder
        public async Task DeleteReminderAsync(string taskId)
        {
            try
            {
                var scheduler = await schedulerFactory.GetScheduler();
                var reminderJobKey = new JobKey($"reminder_{taskId}");
                var dueJobKey = new JobKey($"reminder_due_{taskId}");

                await scheduler.DeleteJob(reminderJobKey);
                await scheduler.DeleteJob(dueJobKey);
                logger.LogInformation($"Reminder of task: {taskId} deleted");
            }
            catch (Exception ex)
            {
                throw new AppException("Failed to delete reminder");
            }
        }

        private async Task CreateReminderHelperAsync(ReminderDto reminder)
        {
            logger.LogInformation($"Start scheduling reminder");
            int hour = reminder.RemindAt.Hour;
            int minute = reminder.RemindAt.Minute;
            int dayOfMonth = reminder.RemindAt.Day;
            DayOfWeek dayOfWeek = reminder.RemindAt.DayOfWeek;

            var scheduler = await schedulerFactory.GetScheduler();
            var job = JobBuilder.Create<TelegramReminderJob>()
                .WithIdentity(new JobKey($"reminder_{reminder.TaskId}", reminder.ReminderGroup))
                .WithDescription("This sends a reminder to the user")
                .UsingJobData("ChatId", reminder.ChatId)
                .UsingJobData("Message", reminder.Message)
                .Build();

            ITrigger trigger;

            if (reminder.Recurrence is not null)
            {
                var cronExpression = reminder.Recurrence switch
                {
                    "daily" => $"0 {minute} {hour} * * ?",
                    "weekly" => $"0 {minute} {hour} ? * {dayOfWeek.ToString().ToUpper()}",
                    "monthly" => $"0 {minute} {hour} {dayOfMonth} * ?",
                    _ => null // No recurrence or invalid recurrence
                };

                trigger = TriggerBuilder.Create()
                .WithIdentity(new TriggerKey($"trigger_{reminder.TaskId}", reminder.ReminderGroup))
                .WithCronSchedule(cronExpression!)
                .ForJob(job)
                .Build();
                logger.LogInformation($"Scheduling {reminder.Recurrence} reminder");
            }
            else
            {
                trigger = TriggerBuilder.Create()
                .WithIdentity(new TriggerKey($"trigger_{reminder.TaskId}", reminder.ReminderGroup))
                .StartAt(reminder.RemindAt)
                .WithSimpleSchedule(x => x.WithRepeatCount(0))
                .ForJob(job)
                .Build();
                logger.LogInformation($"Scheduling one time reminder");
            }
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
