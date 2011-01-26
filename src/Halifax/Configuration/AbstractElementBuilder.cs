using Castle.Core.Configuration;
using Castle.MicroKernel;

namespace Halifax.Configuration
{
    public abstract class AbstractElementBuilder
    {
        public IKernel Kernel { get; set; }

        public string WorkingDirectory { get; set; }

        /// <summary>
        /// This will inspect the configuration element by 
        /// name to see if the concrete element builder 
        /// can build the element based on configuration.
        /// </summary>
        /// <param name="name">Name of the configuration element.</param>
        /// <returns></returns>
        public abstract bool IsMatchFor(string name);

        /// <summary>
        /// This will build the element based on the configuration
        /// definition and store it to the model repository.
        /// </summary>
        /// <param name="configuration"></param>
        public abstract void Build(IConfiguration configuration);
    }
}