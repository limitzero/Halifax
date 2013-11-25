using System;

namespace Halifax.Events
{
    [Serializable]
    public abstract class Event
    {
		/// <summary>
		/// Gets or sets the identifier of the current aggregate root generating the event.
		/// </summary>
        public Guid EventSourceId { get; set; }

		/// <summary>
		/// Gets or sets the current version of the aggregate root that has been affected by the event.
		/// </summary>
        public int Version { get; set; }

		/// <summary>
		/// Gets or sets the current identity of the calling client that issued the domain change resulting in the event.
		/// </summary>
        public string Who { get; set; }

		/// <summary>
		/// Gets or sets the date and time the event was recorded on the domain aggregate root.
		/// </summary>
        public DateTime At { get; set; }

		/// <summary>
		/// Gets or sets the entity that emitted the event.
		/// </summary>
		public string From { get; set; }

		protected Event()
		{
			this.At = System.DateTime.Now;
			this.Who = System.Environment.UserName;
			this.From = string.Empty;
		}
    }
}