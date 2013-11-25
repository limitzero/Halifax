using System;
using Halifax.Domain;

namespace Halifax.Commands
{
    /// <summary>
    /// Contract for all commands that are issued by the caller to the domain for aggregate mutation.
    /// </summary>
    [Serializable]
    public abstract class Command
    {
		/// <summary>
		/// Gets or sets the identifier of the command as it pertains to an <seealso cref="AggregateRoot"/>
		/// for initiating a series of action(s).
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the version of the set of event(s) issued against the 
		/// aggregate potentially for concurrency conflicts. 
		/// </summary>
		public int Version { get; set; }

		/// <summary>
		/// (Read-Write). The date and time the comment was issued to the domain aggregate root.
		/// </summary>
		public DateTime At { get; set; }

		protected Command()
		{
			this.Id = Guid.Empty;
			this.At = System.DateTime.Now;
		}
    }

	[Serializable]
	public abstract class Command<TResult> : Command where TResult : class 
	{
		
	}
}