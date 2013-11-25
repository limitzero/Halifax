using System;
using System.Linq.Expressions;

namespace Halifax.Commands
{
	public class PropertyValidator<TEntity> where TEntity : class
	{
		private string message;
		private Exception exception;
		private Func<TEntity, bool> validation_criteria;
		private readonly TEntity command;
		private readonly string property;

		public PropertyValidator(TEntity command, Expression<Func<TEntity, object>> property)
		{
			this.command = command;
			this.property = GetPropertyName(property);
		}

		public PropertyValidator<TEntity> When(Func<TEntity, bool> validationCriteria)
		{
			this.validation_criteria = validationCriteria;
			return this;
		}

		public PropertyValidator<TEntity> ShowMessage(string message)
		{
			if (this.exception != null)
			{
				throw new InvalidOperationException("You have already set the exception for the validator, the exception or the error message can be set on validation but not both.");
			}

			this.message = message;

			return this;
		}

		public PropertyValidator<TEntity> ShowException(Exception exception)
		{
			if (string.IsNullOrEmpty(this.message) == false)
			{
				throw new InvalidOperationException("You have already set the error message for the validator, the exception or the error message can be set on validation but not both.");
			}
			this.exception = exception;
			return this;
		}

		public PropertyValidatorResult Evaluate()
		{
			var result = new PropertyValidatorResult();

			if (this.command == null)
				throw new InvalidOperationException("The command supplied to the validator must not be null");

			if (this.validation_criteria == null)
			{
				throw new InvalidOperationException("No criteria have been provided for this validation. (Use the 'When(..)' method to specify the criteria.)");
			}

			if (string.IsNullOrEmpty(this.message) == true && this.exception != null)
			{
				throw new InvalidOperationException("The error message or the exception object must be set on the validator for evaluation.");
			}

			if (this.validation_criteria(this.command) == false)
			{
				if(string.IsNullOrEmpty(message) == false)
				{
					result.RecordValidation(this.property, this.message);
					throw new InvalidOperationException(this.message);
				}
				else
				{
					result.RecordValidation(this.property, this.exception.Message);
					throw new InvalidOperationException(this.exception.Message);
				}
			}

			return result;
		}

		private static string GetPropertyName(Expression<Func<TEntity, object>> expression)
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