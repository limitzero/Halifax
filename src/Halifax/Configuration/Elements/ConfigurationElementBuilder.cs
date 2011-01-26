using Castle.Core.Configuration;

namespace Halifax.Configuration.Elements
{
    /// <summary>
    /// Builder for the configuration element to 
    /// setup some defaults for the facility. 
    /// <configuration workingDirectory="{full directory to \bin for application hosting the context}" />
    /// </summary>
    public class ConfigurationElementBuilder : AbstractElementBuilder
    {
        private const string _element = "halifax-configuration";

        public override bool IsMatchFor(string name)
        {
            return _element.Trim() == name.Trim().ToLower();
        }

        public override void Build(IConfiguration configuration)
        {
            string workingDirectory = configuration.Attributes["workingDirectory"] ?? string.Empty;
            WorkingDirectory = workingDirectory;
        }
    }
}