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
				var argRegex = Regex.Match(argument, @"(\<[a-z]+[a-z0-9\-]+\>|\[[a-z]+[a-z0-9\-]+\])", RegexOptions.IgnoreCase);
				if (!argRegex.Success)
					throw new ArgumentException($"The Flags format is malformed for the argument '{argument}'.", nameof(flags));

				option.Required = argument.StartsWith("<");
				option.ArgumentName = argument.Trim('<', '>', '[', ']');
			}

			didact.Options.Add(option);

			return didact;
		}

		public static DidactClient Command(this DidactClient didact, string commandStr, string description)
		{
			var command = new Command
			{
				CommandStr = commandStr
			};

			var match = Regex.Match(commandStr, @"([a-z]+[a-z0-9\-]+)[ ]?([\<\>\[\]a-z0-9\. ]+)?", RegexOptions.IgnoreCase);
			if (!match.Success || match.Groups.Count != 3)
				throw new ArgumentException($"The Command format is malformed for '{commandStr}'.", nameof(commandStr));

			command.Name = match.Groups[1].Value;

			var arguments = match.Groups[2].Value.Trim().Split(' ');
			var index = 0;
			foreach(var argumentStr in arguments)
			{
				var trimmedArgumentStr = argumentStr.Trim('<', '>', '[', ']');
				var argument = new Argument
				{
					Name = trimmedArgumentStr,
					IsRequired = argumentStr.StartsWith("<")
				};
				
				var argRegex = Regex.Match(argumentStr, @"(\<[a-z]+[a-z0-9\-\.]+\>|\[[a-z]+[a-z0-9\-\.]+\])", RegexOptions.IgnoreCase);
				if (!argRegex.Success)
					throw new ArgumentException($"The Command format is malformed for the argument '{argumentStr}'.", nameof(commandStr));

				if (trimmedArgumentStr.Contains("."))
				{
					if (!trimmedArgumentStr.EndsWith("..."))
						throw new ArgumentException($"The Command format is malformed for the argument '{argumentStr}'. Arguments can only contain peroids if they are at the end of the argument name, and are in a group of three.", nameof(commandStr));
					if (index != arguments.Count() - 1)
						throw new ArgumentException($"The Command format is malformed for the argument '{argumentStr}'. Array arguments can only ", nameof(commandStr));

					argument.IsArray = true;
				}

				command.Arguments.Add(argument);
				index++;
			}

			didact.Commands.Add(command);
			return didact;
		}

		public static DidactClient Parse(this DidactClient didact, string[] args)
		{
			if (args == null || !args.Any())
			{
				PrintHelp(didact);
				return didact;
			}
			PrintHelp(didact);

			return didact;
		}

		public static void DebugPrintValues(this DidactClient client)
		{
			WriteLine();
			WriteLine("Printing Internal Values");
			WriteLine();
			WriteLine($"  Global Options");
			foreach (var option in client.Options.Where(o => o.OptionType == OptionType.Global))
				WriteLine($"    {option.ShortCommand}, {option.LongCommand} - {option.Value}");
			WriteLine();
			WriteLine($"  Commands");
			foreach (var command in client.Commands)
			{
				WriteLine($"  {command.Name}'s Details");
				WriteLine($"    Arguments");
				foreach (var argument in command.Arguments)
					WriteLine($"      {argument.Name} - {argument.Value}");

				WriteLine($"    Arguments");
				foreach (var argument in command.Arguments)
					WriteLine($"      {argument.Name} - {argument.Value}");

				WriteLine($"    Options");
				foreach (var option in client.Options.Where(o => o.ParentName == command.Name))
				WriteLine($"      {option.ShortCommand}, {option.LongCommand} - {option.Value}");

				WriteLine();
			}
		}

		private static void PrintHelp(DidactClient client)
		{
			WriteLine();
			WriteLine(client.Metadata.Name);
			WriteLine();
			WriteLine($"  Version: {client.Metadata.Version}");
			WriteLine();
			WriteLine($"Usage: {client.Metadata.Usage}");
			WriteLine();
			WriteLine("Common Options:");

			// Print Global Options
			var options = client.Options.Where(o => o.OptionType == OptionType.Global);
			var largestLinePosition = 4 + options.OrderByDescending(o => o.ShortCommand.Length).First().ShortCommand.Length + options.OrderByDescending(o => o.LongCommand.Length).First().LongCommand.Length;
			foreach (var option in options)
			{
				var line = $"  {option.ShortCommand}, {option.LongCommand}";
				var padding = largestLinePosition - line.Length;
				WriteLine($"{line.PadRight(padding, ' ')} {option.Description}");
			}
		}
	}
}
