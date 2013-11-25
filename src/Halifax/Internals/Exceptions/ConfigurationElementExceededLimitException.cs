namespace Halifax.Internals.Exceptions
{
    public class ConfigurationElementExceededLimitException : HalifaxException
    {
        private const string _message =
            "The setting of '{0}' exceeded the threshold for configuration element of '{1}'. The top level value for this setting is '{2}'.";

        public ConfigurationElementExceededLimitException(string configurationSettingName, 
            string configurationSettingElementName,
            string threshHoldValue)
            : base(string.Format(_message, configurationSettingName, configurationSettingElementName, threshHoldValue))
        {
            
        }
    }
}
