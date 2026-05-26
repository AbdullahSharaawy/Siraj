using TheCharityBLL.Jobs.Base;

namespace TheCharityBLL.Jobs.Registry.Abstraction
{
    public interface IJobRegistry
    {
        void RegisterAllRecurringJobs();
    }
}
