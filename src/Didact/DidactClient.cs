using System.Collections.Generic;
using Didact.Models;

namespace Didact
{
	public class DidactClient
	{
		internal Metadata Metadata { get; set; } = new Metadata();

		internal IList<Command> Commands { get; set; } = new List<Command>();

		internal IList<Option> Options { get; set; } = new List<Option>();

		public DidactClient() { }
	}
}
