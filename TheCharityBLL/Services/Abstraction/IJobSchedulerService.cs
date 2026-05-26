using TheCharityBLL.Jobs.Base;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Services.Abstraction
{
    public interface IJobSchedulerService
    {
        // Immediate execution
        string EnqueueJob<T>(object? parameters = null) where T : BaseJob;
        // Delayed execution
        string ScheduleJob<T>(DateTimeOffset executeAt, object? parameters = null) where T : BaseJob;
        // Recurring jobs
        void AddOrUpdateRecurringJob<T>(string recurringJobId, string cronExpression, object? parameters = null) where T : BaseJob;
        void RemoveRecurringJob(string recurringJobId);
        // Cancellation
        bool CancelJob(string jobId);
        // Status
        JobStatus GetJobStatus(string jobId);
        // Continuation (runs after parent job completes)
        string ScheduleContinuationJob<T>(string parentJobId, object? parameters = null) where T : BaseJob;
    }
}
