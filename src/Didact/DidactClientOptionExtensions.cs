using System;
using System.Linq;
using System.Text.RegularExpressions;
using Didact.Models;
using Didact.Models.Enums;

namespace Didact
{
	public static partial class DidactClientExtensions
	{
		public static DidactClient Option(this DidactClient didact, string flags,
			string description, Func<string, bool> validate = null, 
			Func<string, string> parse = null, string defaultValue = default(string))
		{
			var parentName = didact.Commands.LastOrDefault()?.Name;
			var option = new Option
			{
				DefaultValue = defaultValue,
				Description = description,
				Flags = flags,
				OptionType = parentName == null ? OptionType.Global : OptionType.Command,
				ParentName = parentName,
				Parse = parse,
				Validate = validate
			};

			// Check format of flags
			var match = Regex.Match(flags, @"(\-[a-z]),[ ]([\-]{2}[a-z\-]+)[ ]?([\<\>\[\]a-z0-9\- ]+)?", RegexOptions.IgnoreCase);
			if (!match.Success || match.Groups.Count != 4)
				throw new ArgumentException($"The Flags format is malformed for '{flags}'.", nameof(flags));

			option.ShortCommand = match.Groups[1].Value;
			option.LongCommand = match.Groups[2].Value;

			// Parse arguments out from flags
			var arguments = match.Groups[3].Value.Trim().Split(' ');
			if (arguments.Count() > 1)
				throw new ArgumentException($"The Flags format is malformed for the argument. You can have only one argument on an option.", nameof(flags));

			if (arguments.Any())
			{
				var argument = arguments.First();
				var argRegex = Regex.Match(argument, @"(\<[a-z]+[a-z0-9\-]+\>|\[[a-z]+[a-z0-9\-]+\])", RegexOptions.IgnoreCase);
				if (!argRegex.Success)
					throw new ArgumentException($"The Flags format is malformed for the argument '{argument}'.", nameof(flags));

				option.IsRequired = argument.StartsWith("<");
				option.Name = argument.Trim('<', '>', '[', ']');
			}

			didact.Options.Add(option);

			return didact;
		}
	}
}
