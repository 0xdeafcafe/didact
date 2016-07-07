using System;
using Didact.Models.Enums;

namespace Didact.Models
{
	public abstract class Option
	{
		public string Flags { get; set; }

		public string Description { get; set; }

		public string ArgumentName { get; set; }

		public string ShortCommand { get; set; }

		public string LongCommand { get; set; }

		public Type Type { get; set; }

		public bool Required { get; set; }

		public OptionType OptionType { get; set; }

		public string ParentName { get; set; }

		public string Value { get; set; }
	}

	public class Option<T> : Option
	{
		public T DefaultValue { get; set; }

		public Func<string, T> Parse { get; set; }
	}
}
