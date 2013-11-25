namespace Halifax.StateMachine
{
	public class State
	{
		public string Name { get; set; }

		public override bool Equals(object obj)
		{
			bool success = false;
			State other = obj as State;

			if (obj == null) return success;

			if (obj.GetType() != typeof(State) || other == null)
				return success;

			return this.Name.Equals(other.Name);
		}

		public static bool operator ==(State first, State second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(State first, State second)
		{
			return  !(first == second);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() + this.Name.GetHashCode();
		}
	}
}