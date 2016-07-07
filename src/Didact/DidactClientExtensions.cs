using System;
using System.Linq;
using System.Text.RegularExpressions;
using Didact.Models;
using Didact.Models.Enums;
using static System.Console;

namespace Didact
{
	public static class DidactClientExtensions
	{
		public static DidactClient Name(this DidactClient didact, string name)
		{
			didact.Metadata.Name = name;
			return didact;
		}

		public static DidactClient Version(this DidactClient didact, string version)
		{
			didact.Metadata.Version = version;
			return didact;
		}

		public static DidactClient Usage(this DidactClient didact, string usage)
		{
			didact.Metadata.Usage = usage;
			return didact;
		}

		public static DidactClient Option(this DidactClient didact, string flags,
			string description, Func<string, string> parse = null, string defaultValue = default(string))
		{
			var parentName = didact.Commands.LastOrDefault()?.Name;
			var option = new Option<string>
			{
				DefaultValue = defaultValue,
				Description = description,
				Flags = flags,
				OptionType = parentName == null ? OptionType.Global : OptionType.Command,
				ParentName = parentName,
				Parse = parse,
				Type = typeof(string)
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
				var argRegex = Regex.Match(argument, @"(\<[a-z]+[a-z0-9\-]+\>|\[[a-z]+[a-z0-9]+\])", RegexOptions.IgnoreCase);
				if (!argRegex.Success)
					throw new ArgumentException($"The Flags format is malformed for the argument '{argument}'.", nameof(flags));

				option.Required = argument.StartsWith("<");
				option.ArgumentName = argument.Trim('<', '>', '[', ']');
			}

			didact.Options.Add(option);

			return didact;
		}

		public static DidactClient Parse(this DidactClient didact, string[] args)
		{
			return didact;
		}
	}
}


// TODO: move this to command parsing
// var index = 0;
// foreach(var argument in arguments)
// {
// 	var argRegex = new Regex(@"(\<[a-z]+[a-z0-9\.]+\>|\[[a-z]+[a-z0-9\.]+\])", RegexOptions.IgnoreCase);
// 	if (!argRegex.IsMatch(argument))
// 		throw new ArgumentException($"The Flags format is malformed for the argument '{argument}'.", nameof(flags));

// 	if (argument.Contains("."))
// 	{
// 		if (!argument.EndsWith("..."))
// 			throw new ArgumentException($"The Flags format is malformed for the argument '{argument}'. Arguments can only contain peroids if they are at the end of the argument name, and are in a group of three.", nameof(flags));
// 		if (index != argument.Count() - 1)
// 			throw new ArgumentException($"The Flags format is malformed for the argument '{argument}'. Array arguments can only ", nameof(flags));
// 	}

//     index++;
// }
