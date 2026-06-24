using TheCharityBLL.Jobs.Context;
using TheCharityBLL.Jobs.Result.Abstraction;

namespace TheCharityBLL.Jobs.Base
{
    public abstract class BaseJob
    {
        public abstract string JobName { get; }
        public virtual string Queue => "default";
        public virtual Task<bool> CanExecuteAsync(JobContext context)
        {
            return Task.FromResult(true);
        }
        public abstract Task<IJobResult> ExecuteAsync(JobContext context);
        public virtual Task OnSuccessAsync(JobContext context, IJobResult result)
        {
            return Task.CompletedTask;
        }
        public virtual Task OnFailureAsync(JobContext context, IJobResult result, Exception ex)
        {
            return Task.CompletedTask;
        }
    }
}
