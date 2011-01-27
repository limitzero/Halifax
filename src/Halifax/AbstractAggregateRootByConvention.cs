using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Halifax.Exceptions;

namespace Halifax
{
    /// <summary>
    /// Base class that represents an aggregate root where the events that  
    /// are issued by the aggregate mapped by the convention "private void On{event name}(event)"
    /// or private {function name}(event)".
    /// </summary>
    [Serializable]
    public abstract class AbstractAggregateRootByConvention : AbstractAggregateRoot
    {
        public override void Apply<TEvent>(TEvent currentEvent, bool isLoadingFromHistory)
        {
            MethodInfo handler = GetMethodForEvent(currentEvent.GetType());
            if (handler == null)
                throw new UnregisteredEventHandlerOnAggregateForEventException(GetType(), currentEvent.GetType());

            handler.Invoke(this, new object[] {currentEvent});

            if (!isLoadingFromHistory)
            {
                // update the current event with the information from the aggregate:
                GetVersion();
                PrepareDomainEvent(currentEvent);
                Record(currentEvent);
            }
        }

        private MethodInfo GetMethodForEvent(Type domainEvent)
        {
            MethodInfo method = null;
            string pattern = "^(on|On|ON)" + domainEvent.Name;
            var regEx = new Regex(pattern);

            method = (from theMethod in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                      let parameters = theMethod.GetParameters()
                      where (parameters.Length == 1
                      && parameters[0].ParameterType == domainEvent)
                      || regEx.IsMatch(theMethod.Name) == true
                      select theMethod).FirstOrDefault();

            // This works:
            //foreach (MethodInfo item in GetType().GetMethods())
            //{
            //    if (!regEx.IsMatch(item.Name)) continue;
            //    method = item;
            //    break;
            //}

            return method;
        }
    }
}