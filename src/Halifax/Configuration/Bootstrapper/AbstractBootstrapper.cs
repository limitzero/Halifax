using Castle.MicroKernel;

namespace Halifax.Configuration.Bootstrapper
{
    /// <summary>
    /// Base class for extending the configuration of the 
    /// framework via custom components.
    /// </summary>
    public abstract class AbstractBootstrapper
    {
        protected AbstractBootstrapper()
        {
            IsActive = true;
        }

        public bool IsActive { get; set; }
        public IKernel Kernel { get; set; }
        public string WorkingDirectory { get; set; }

        public abstract void Configure();
    }
}