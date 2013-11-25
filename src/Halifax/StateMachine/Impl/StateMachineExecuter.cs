using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Halifax.Configuration;
using Halifax.Domain;
using Halifax.Events;
using Halifax.Read;

namespace Halifax.StateMachine.Impl
{
	public class StateMachineExecuter
	{
		private readonly IContainer container;

		public StateMachineExecuter(IContainer container)
		{
			this.container = container;
		}

		public void Execute(IStateMachine stateMachine, Event @event)
		{
			var bus = this.container.Resolve<IBus>();
			DispatchMessage(stateMachine, @event);

			bus.Publish(stateMachine.Events.ToArray());
			bus.Send(stateMachine.Commands.ToArray());

			// kill any local state for next invocation:
			stateMachine.Dispose();
		}

		private void DispatchMessage(IStateMachine stateMachine, Event @event)
		{
			object stateMachineData = null;
			StateMachineDataToMessageDataCorrelation correlation = null;

			if (TryRetrieveStateMachineData(stateMachine, @event, out stateMachineData, out correlation) == true)
			{
				stateMachine.GetType().GetProperty("Data")
					.SetValue(stateMachine, stateMachineData, new object[] { });

				stateMachine.CurrentState = new State { Name = ((IStateMachineData)stateMachineData).State  };
			}

			Delegate action = Delegate.CreateDelegate(typeof(Action<>)
				.MakeGenericType(new Type[] { @event.GetType() }),
				stateMachine, "Handle");
			action.DynamicInvoke(@event);

			this.SaveOrDeleteStateMachineData(stateMachine, correlation, stateMachineData == null);
		}

		private void SaveOrDeleteStateMachineData(IStateMachine stateMachine, 
			StateMachineDataToMessageDataCorrelation correlation, 
			bool isStarting = false)
		{
			var repository = GetReadModelRepositoryInstanceForStateMachineData(stateMachine);

			var data = ((dynamic)stateMachine).Data;

			if (repository != null)
			{
				if (stateMachine.IsCompleted == true)
				{
					repository.Delete(data);
				}
				else if (stateMachine.IsCompleted == false && isStarting == true)
				{
					((IStateMachineData)data).Id = CombGuid.NewGuid();

					// pass on the correlated value to the data that will be stored for the
					// state machine on the initial creation of the data from the initializing message, 
					// we will search by this correlated name and value on the next access of the state machine
					// (this is most likely a business key for the data that is used between access to the state machine):
					if (correlation != null)
					{
						((IStateMachineData) data).GetType()
							.GetProperty(correlation.StateMachineDataPropertyName)
							.SetValue(data, correlation.CorrelatedValue, new object[] {});
					}

					repository.Insert(data);
				}
				else
				{
					repository.Update(data);
				}
			}

		}

		private bool TryRetrieveStateMachineData(IStateMachine stateMachine, Event @event, 
			out object state_machine_data, 
			out StateMachineDataToMessageDataCorrelation correlation)
		{
			bool success = false;
			state_machine_data = null;
			correlation = null;

			var correlations = ((dynamic) stateMachine).Correlations as List<StateMachineDataToMessageDataCorrelation>;

			if (correlations == null) return success;

			correlation = correlations.Where(c => c.StateMachineMessage == @event.GetType()).FirstOrDefault();

			if (correlation != null)
			{
				var the_correlation = correlation;
				var repository = GetReadModelRepositoryInstanceForStateMachineData(stateMachine);

				var models = ((IEnumerable<IReadModel>) repository.All());

				if (models.Count() > 0)
				{
					state_machine_data = (from item in models
					                      where the_correlation.IsMatch(item, @event)
					                      select item).FirstOrDefault() as object;
				}
				else
				{
					// force a match on the correlation to get the value representing the correlation from the message:
					correlation.IsMatch(null, @event);
				}
			}

			return state_machine_data != null;
		}

		private dynamic GetReadModelRepositoryInstanceForStateMachineData(IStateMachine stateMachine)
		{
			var readModelType = stateMachine.GetType().GetProperty("Data").PropertyType;
			var readModelRepositoryType = typeof(IReadModelRepository<>).MakeGenericType(readModelType);
			var readModelRepository = this.container.Resolve(readModelRepositoryType);
			return readModelRepository as dynamic;
		}

		private static string GetEventPropertyName(Expression<Func<Event, object>> expression)
		{
			MemberExpression memberExpression;

			if (expression.Body is UnaryExpression)
			{
				memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
			}
			else
			{
				memberExpression = expression.Body as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new InvalidOperationException("You must specify a property!");
			}

			return memberExpression.Member.Name;
		}

		private static string GetReadModelPropertyName(Expression<Func<IReadModel, object>> expression)
		{
			MemberExpression memberExpression;

			if (expression.Body is UnaryExpression)
			{
				memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
			}
			else
			{
				memberExpression = expression.Body as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new InvalidOperationException("You must specify a property!");
			}

			return memberExpression.Member.Name;
		}

		private static string GetPropertyName(Expression<Func<object, object>> expression)
		{
			MemberExpression memberExpression;

			if (expression.Body is UnaryExpression)
			{
				memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
			}
			else
			{
				memberExpression = expression.Body as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new InvalidOperationException("You must specify a property!");
			}

			return memberExpression.Member.Name;
		}
	}
}