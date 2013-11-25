using System;
using System.Collections.Generic;
using Halifax.Events;

namespace Halifax.Commands
{
	[Serializable]
	public class CommandResult
	{
		/// <summary>
		/// Gets or sets the command that was issued to the behavioral model
		/// </summary>
		public Command IssuedCommand { get; set; }

		/// <summary>
		/// Gets or sets the collection of events triggered as a result of the command 
		/// being issued against the behavioral model and state changes model as 
		/// events are issued.
		/// </summary>
		public IEnumerable<Event> Events { get; set; }

		/// <summary>
		/// Gets or set the collection of input validation messages on the issued command.
		/// </summary>
		public IEnumerable<string> ValidationMessages { get; set; }

		public Exception Exception { get; set; }
	}
}