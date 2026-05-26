using TheCharityBLL.Jobs.Abstraction;

namespace TheCharityBLL.Jobs.Implementation
{
    public class JobResult : IJobResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public Exception? Error { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
        public object? Data { get; set; }
        public static JobResult Success(string message = "Job completed successfully", object? data = null)
            => new() { IsSuccess = true, Message = message, Data = data };
        public static JobResult Failure(string message, Exception? ex = null)
            => new() { IsSuccess = false, Message = message, Error = ex };
    }
}
