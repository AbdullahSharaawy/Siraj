namespace TheCharityBLL.Jobs.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QueueAttribute : Attribute
    {
        public string Name { get; }
        public QueueAttribute(string name) => Name = name;
    }
}
