namespace TheCharityBLL.Jobs.Abstraction
{
    public interface IJobResult
    {
        bool IsSuccess { get; }
        string Message { get; }
        Exception? Error { get; }
        DateTime CompletedAt { get; }
        object? Data { get; }
    }
}
