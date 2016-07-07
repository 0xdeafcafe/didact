using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Didact.Models;

namespace Didact
{
	public static partial class DidactClientExtensions
	{
		public static DidactClient Command(this DidactClient didact, string commandStr, string description,
			Func<Dictionary<string, string>, Dictionary<string, string>, Task> action)
		{
			var command = new Command
			{
				CommandStr = commandStr,
				Action = action
			};

			var match = Regex.Match(commandStr, @"([a-z]+[a-z0-9\-]+)[ ]?([\<\>\[\]a-z0-9 ]+)?", RegexOptions.IgnoreCase);
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
				
				var argRegex = Regex.Match(argumentStr, @"(\<[a-z]+[a-z0-9\-]+\>|\[[a-z]+[a-z0-9\-]+\])", RegexOptions.IgnoreCase);
				if (!argRegex.Success)
					throw new ArgumentException($"The Command format is malformed for the argument '{argumentStr}'.", nameof(commandStr));

				command.Arguments.Add(argument);
				index++;
			}

			didact.Commands.Add(command);
			return didact;
		}
	}
}
