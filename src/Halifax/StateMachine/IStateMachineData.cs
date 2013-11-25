using Halifax.Read;

namespace Halifax.StateMachine
{
	/// <summary>
	/// Marker interface for data that can be persisted in-between calls 
	/// to a state machine for longer-running processes.
	/// </summary>
	public interface IStateMachineData : IReadModel
	{
		/// <summary>
		/// Gets or sets the state of the current state machine for exernal inspection:
		/// </summary>
		string State { get; set; }
	}
}