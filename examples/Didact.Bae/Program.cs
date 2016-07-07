using System.Collections.Generic;
using System.Threading.Tasks;
using BaelorNet;
using Didact;
using static System.Console;

namespace ConsoleApplication
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var didact = new DidactClient()
				.Name("Baelor Cli Client")
				.Version("1.0.0")
				.Usage("get-song <song> [options]")
				.Option("-k, --api-key [api-key]", "The Api Key for your account on baelor.io.")
				.Command("get-lyrics <song>", "Gets the lyric of a Taylor Swift song.", GetLyricsToSong)
					.Option("-s, --show-timecodes [show-timecodes]", "Toggles the visibility of Timecodes", defaultValue: "t");

			didact.ParseAsync(args).Wait();
		}

		public static async Task GetLyricsToSong(Dictionary<string, string> arguments, Dictionary<string, string> options)
		{
			var songSlug = arguments["song"];
			var showTimecodes = options["show-timecodes"];
			var apiKey = options["api-key"];

			var client = new BaelorClient(apiKey);
			var song = await client.Song(songSlug);
			WriteLine($"Lyrics for {song.Title}:");
			foreach(var lyric in song.Lyrics)
			{
				if (showTimecodes == "t") Write($"{lyric.TimeCode} - ");
				Write($"{lyric.Content}\n");
			}
		}
	}
}
