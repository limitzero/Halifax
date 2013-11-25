using System;

namespace Halifax.Read
{
	/// <summary>
	/// The read model marker interface signifies those view models that 
	/// should be projected from the behavioral model ("aggregate root")
	/// base on an event or series of events that occured within the aggregate root.
	/// </summary>
	public interface IReadModel
	{
		/// <summary>
		/// Gets or sets the identifer of the model data corresponding to an aggregate root.
		/// </summary>
		Guid Id { get; set; }
	}
}