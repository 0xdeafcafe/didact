using System.Linq;
using System.Text;
using Didact.Models.Enums;
using static System.Console;

namespace Didact
{
	public static partial class DidactClientExtensions
	{
		internal static void PrintHelp(DidactClient client)
		{
			if (client.Metadata.Usage == null)
			{
				// Generate Usage
				var sb = new StringBuilder();
				sb.Append(client.Metadata.CliName);
				if (client.Commands.Any())
					sb.Append(" [command]");
				if (client.Commands.Any(c => c.Arguments.Any()))
					sb.Append(" [arguments]");
				if (client.Options.Any(o => o.OptionType == OptionType.Command))
					sb.Append(" [command-options]");
				if (client.Options.Any(o => o.OptionType == OptionType.Global))
					sb.Append(" [global-options]");

				client.Metadata.Usage = sb.ToString();
			}

			WriteLine();
			Write(client.Metadata.Name);
			if (client.Metadata.Version != null) Write($" - ({client.Metadata.Version})");
			Write("\n");
			WriteLine();
			WriteLine($"Usage: {client.Metadata.Usage}");
			WriteLine();

			// Print Global Options
			var options = client.Options.Where(o => o.OptionType == OptionType.Global);
			if (options.Any())
			{
				WriteLine("Global Options:");
				var largestLinePosition = 4 + options.OrderByDescending(o => o.ShortCommand.Length).First().ShortCommand.Length + options.OrderByDescending(o => o.LongCommand.Length).First().LongCommand.Length;
				foreach (var option in options)
				{
					var line = $"  {option.ShortCommand}, {option.LongCommand}";
					var padding = largestLinePosition - line.Length;
					WriteLine($"{line.PadRight(padding, ' ')} {option.Description}");
				}
			}

			// TODO: print commands with their respective options
		}

		#if DEBUG

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

		#endif
	}
}
