using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Didact.Models.Enums;

namespace Didact
{
	public static partial class DidactClientExtensions
	{
		public static void Parse(this DidactClient didact, string[] args)
		{
			try
			{
				didact.ParseAsync(args).Wait();
			}
			catch (AggregateException ex)
			{
				ExceptionDispatchInfo.Capture(ex.Flatten().InnerExceptions.First()).Throw();
			}
		}

		public async static Task ParseAsync(this DidactClient didact, string[] args)
		{
			if (args == null || !args.Any())
			{
				PrintHelp(didact);
				return;
			}

			// Match all global options
			ParseOptions(ref didact, args, null);

			// Check if first command matches any commands
			var command = didact.Commands.FirstOrDefault(c => c.Name == args[0]);
			if (command != null)
			{
				// Parse the next few into commands arguments
				for(var i = 1; i < args.Length; i++)
				{
					var argument = command.Arguments.ElementAtOrDefault(i - 1);
					if (argument != null && args.Length >= i)
						argument.Value = args[i];
				}

				// Check if required arguments exist
				var missingArgument = command.Arguments.FirstOrDefault(a => a.Value == null && a.IsRequired);
				if (missingArgument != null)
					throw new ArgumentException($"The {missingArgument.Name} argument is required.", missingArgument.Name);

				// Parse all command specific options
				ParseOptions(ref didact, args, command.Name);

				// Populate null, non-required, non-validation-failed options with their default values, if they have them
				foreach(var option in didact.Options.Where(o => o.IsRequired && o.DefaultValue != null && o.Value == null))
					option.Value = option.DefaultValue;

				// Throw exception if a required command is missing
				var missingOption = didact.Options
					.FirstOrDefault(o => o.OptionType == OptionType.Global &&
									o.ParentName == command.Name &&
									o.IsRequired && o.Value == null);
									
				if (missingOption != null)
					throw new ArgumentNullException($"The {missingOption.Name} option is required for this command.");

				// Create dictionaries of arguments and options to pass into the action
				var arguments = command.Arguments
					.Select(a => new KeyValuePair<string, string>(a.Name, a.Value))
					.ToDictionary(d => d.Key, d => d.Value);

				var options = didact.Options
					.Where(o => o.ParentName == command.Name || o.OptionType == OptionType.Global)
					.Select(a => new KeyValuePair<string, string>(a.ShortCommand.Remove(0, 1), a.Value))
					.ToDictionary(d => d.Key, d => d.Value);

				if (command.Action != null)
					command.Action(arguments, options);
				else if (command.ActionAsync != null)
					await command.ActionAsync(arguments, options);
				else
					throw new InvalidOperationException("There is no action to call. Help.");
			}

			if (command == null)
				PrintHelp(didact);
		}

		private static void ParseOptions(ref DidactClient client, string[] args, string parentName)
		{
			for(var i = 1; i < args.Length; i++)
			{
				var option = client.Options.FirstOrDefault(o => args[i - 1] == o.ShortCommand || args[i - 1] == o.LongCommand && o.ParentName == parentName);
				if (option != null && args.Length >= i)
				{
					var val = args[i];

					// Parse
					if (option.Parse != null)
						val = option.Parse(val);

					// Validate
					if (option.Validate == null)
						option.Value = val;
					else
					{
						if (option.Validate(val))
							option.Value = val;
						else
							throw new FormatException($"The option {option.ShortCommand} failed data validation.");
					}

					if (option.IsRequired && option.Value == null)
						throw new FormatException($"The {option.Name} is required.");
				}
			}
		}
	}
}
