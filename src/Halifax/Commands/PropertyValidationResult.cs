using System;
using System.Collections.Generic;

namespace Halifax.Commands
{
	[Serializable]
	public class PropertyValidatorResult
	{
		public IList<Tuple<string,string>> InputValidations { get; set; }

		public PropertyValidatorResult()
		{
			this.InputValidations = new List<Tuple<string, string>>();
		}

		public void RecordValidation(string property, string message)
		{
				this.InputValidations.Add(new Tuple<string, string>(property, message));
		}
	}
}