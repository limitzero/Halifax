using System;

namespace Halifax.StateMachine.Impl
{
	public class StateMachineDataToMessageDataCorrelation
	{
		public StateMachineDataToMessageDataCorrelation(
			Type stateMachineMessage,
			string stateMachineDataPropertyName,
			string messageDataPropertyName)
		{
			StateMachineMessage = stateMachineMessage;
			StateMachineDataPropertyName = stateMachineDataPropertyName;
			MessageDataPropertyName = messageDataPropertyName;
		}

		public Type StateMachineMessage { get; private set; }
		public string StateMachineDataPropertyName { get; private set; }
		public string MessageDataPropertyName { get; private set; }
		public object CorrelatedValue { get; private set; }

		public bool IsMatch(object stateMachineData, object message)
		{
			bool success = false;
			object dataPropertyValue = null;
			object messagePropertyValue = null;

			if (stateMachineData != null)
			{
				dataPropertyValue = stateMachineData.GetType()
					.GetProperty(StateMachineDataPropertyName)
					.GetValue(stateMachineData, null);
			}

			if (message != null)
			{
				messagePropertyValue = message.GetType()
					.GetProperty(MessageDataPropertyName)
					.GetValue(message, null);
			}

			this.CorrelatedValue = messagePropertyValue;

			if (this.CorrelatedValue != null)
			{
				 success = messagePropertyValue.Equals(dataPropertyValue);
			}

			return success;
		}
	}
}
