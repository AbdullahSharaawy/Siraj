using TheCharityBLL.Jobs.Base;

namespace TheCharityBLL.Jobs.Registry.Abstraction
{
    public interface IJobRegistry
    {
        void RegisterAllRecurringJobs();
        void RegisterRecurringJob<T>(string jobId, string cronExpression) where T : BaseJob;
    }
}
