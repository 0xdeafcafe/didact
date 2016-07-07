using System;
using Didact.Models.Enums;

namespace Didact.Models
{
	public class Option
	{
		public string Flags { get; set; }

		public string Description { get; set; }

		public string Name { get; set; }

		public string ShortCommand { get; set; }

		public string LongCommand { get; set; }

		public Type Type { get; set; }

		public bool IsRequired { get; set; }

		public OptionType OptionType { get; set; }

		public string ParentName { get; set; }

		public string Value { get; set; }

		public string DefaultValue { get; set; }

		public Func<string, string> Parse { get; set; }

		public Func<string, bool> Validate { get; set; }
	}
}
