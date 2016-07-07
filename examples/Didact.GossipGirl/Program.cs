using System;

namespace Didact.GossipGirl
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var didact = new DidactClient()
				.CliName("gossip-girl")
				.Name("Gossip Girl Cli Tool")
				.Version("1.0.0")
				.Command("add-gossip <word>", "Adds gossip to a word.", (arguments, options) =>
				{
					const string gossip = "xo";
					var word = $"{arguments["word"]} ";
					var gossips = int.Parse(options["c"]);

					for(var i = 0; i < gossips; i++)
						word += gossip;

					Console.WriteLine(word);
				})	.Option("-c, --count [count]", "Number of gossips to add.", validate: (val) => 
					{
						int gossips = 0;
						return Int32.TryParse(val, out gossips);
					}, defaultValue: "2");

			didact.Parse(args);
		}
	}
}
