namespace Halifax.Internals
{
    public interface IStartable
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
    }
}