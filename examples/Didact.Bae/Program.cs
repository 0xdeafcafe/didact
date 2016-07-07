using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
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
				.CliName("baelor")
				.Name("Baelor Cli Client")
				.Version("1.0.0")
				.Option("-k, --api-key [api-key]", "The Api Key for your account on baelor.io.")
				.Command("get-album <album>", "Get's details of a Taylor Swift album.", GetAlbum)
				.Command("get-lyrics <song>", "Gets the lyric of a Taylor Swift song.", GetLyricsToSong)
					.Option("-s, --show-timecodes [show-timecodes]", "Toggles the visibility of Timecodes", validate: (val) => 
					{
						var lowerVal = val.ToLowerInvariant();
						return (lowerVal == "t" || lowerVal == "f");
					}, defaultValue: "t");

			try
			{
				didact.ParseAsync(args).Wait();
			}
			catch (AggregateException ex)
			{
				ExceptionDispatchInfo.Capture(ex.Flatten().InnerExceptions.First()).Throw();
			}
		}

		public static async Task GetLyricsToSong(Dictionary<string, string> arguments, Dictionary<string, string> options)
		{
			var songSlug = arguments["song"];
			var showTimecodes = options["show-timecodes"];
			var apiKey = options["k"];

			var client = new BaelorClient(apiKey);
			var song = await client.Song(songSlug);
			WriteLine($"Lyrics for {song.Title}:");
			foreach(var lyric in song.Lyrics)
			{
				if (showTimecodes == "t") Write($"{lyric.TimeCode} - ");
				Write($"{lyric.Content}\n");
			}
		}

		public static async Task GetAlbum(Dictionary<string, string> arguments, Dictionary<string, string> options)
		{
			var albumSlug = arguments["album"];
			var apiKey = options["k"];

			var client = new BaelorClient(apiKey);
			var album = await client.Album(albumSlug);
			WriteLine(album.Name);
			WriteLine($"- Produced By: {string.Join(", ", album.Producers)}");
			WriteLine($"- Genres: {string.Join(", ", album.Genres)}");
			WriteLine($"- Released: {album.ReleasedAt.ToString("dd MMM yyyy")}");
			WriteLine($"- Label: {album.Label}");
			WriteLine($"- Length: {album.Length.ToString("c")}");
			WriteLine($"- Songs:");
			foreach (var song in album.Songs)
				WriteLine($"  - {song.Title}");
		}
	}
}
