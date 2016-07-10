using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Didact.Models;

namespace Didact
{
	public static partial class DidactClientExtensions
	{
		public static DidactClient CommandAsync(this DidactClient didact, string commandStr, string description,
			Func<Dictionary<string, string>, Dictionary<string, string>, Task> action)
		{
			if (action == null)
				throw new ArgumentNullException($"The {nameof(action)} argument cannot be null", nameof(action));

			return didact.Command(commandStr, description, action: null, actionAsync: action);
		}

		public static DidactClient Command(this DidactClient didact, string commandStr, string description,
			Action<Dictionary<string, string>, Dictionary<string, string>> action)
		{
			if (action == null)
				throw new ArgumentNullException($"The {nameof(action)} argument cannot be null", nameof(action));

			return didact.Command(commandStr, description, action: action, actionAsync: null);
		}

		private static DidactClient Command(this DidactClient didact, string commandStr, string description,
			Action<Dictionary<string, string>, Dictionary<string, string>> action = null,
			Func<Dictionary<string, string>, Dictionary<string, string>, Task> actionAsync = null)
		{
			var command = new Command
			{
				CommandStr = commandStr,
				Action = action,
				ActionAsync = actionAsync
			};

			var match = Regex.Match(commandStr, @"([a-z]+[a-z0-9\-]+)[ ]?([\<\>\[\]a-z0-9 ]+)?", RegexOptions.IgnoreCase);
			if (!match.Success || match.Groups.Count != 3)
				throw new ArgumentException($"The Command format is malformed for '{commandStr}'.", nameof(commandStr));

			command.Name = match.Groups[1].Value;

			var arguments = match.Groups[2].Value.Trim().Split(' ');
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
			}

			didact.Commands.Add(command);
			return didact;
		}
	}
}
