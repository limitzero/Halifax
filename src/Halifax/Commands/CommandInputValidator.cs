using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Castle.Core;

namespace Halifax.Commands
{
	/// <summary>
	/// Marker interface for command validators.
	/// </summary>
	public interface ICommandValidatorFor
	{}

	/// <summary>
	/// Base class used to basic input validation on a command before dispatch to command consumer.
	/// </summary>
	public abstract class CommandInputValidator 
	{
		/// <summary>
		/// Base class to distinguish which command to validate input data on before passing it on to the command consumer.
		/// </summary>
		/// <typeparam name="T">Type of the command to validate</typeparam>
		public abstract class For<T> : ICommandValidatorFor where T : Command
		{
			private static List<PropertyValidator<T>> validationRules;

			protected For()
			{
				validationRules = new List<PropertyValidator<T>>();
			}

			/// <summary>
			/// This will validate the input data on the command for adherance to basic input rules.
			/// </summary>
			/// <typeparam name="T">Command to validate</typeparam>
			/// <param name="command"></param>
			public abstract void Validate(T command);

			protected class Inspect
			{
				public static void WithRules(params PropertyValidator<T>[] validators)
				{
					// TODO: clean this up!!!
					validationRules.AddRange(validators);
					validators.AsEnumerable().ForEach(v => v.Evaluate());
				}	
			}

			protected class Rule
			{
				public static PropertyValidator<T> For(T command, Expression<Func<T, object>> property)
				{
					return new PropertyValidator<T>(command, property);
				}
			}
		}
	}
}