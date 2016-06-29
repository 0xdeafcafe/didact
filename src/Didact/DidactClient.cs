using Didact.Models;

namespace Didact
{
	public class DidactClient
	{
		public Metadata Metadata { get; set; }

		public DidactClient()
		{
			Metadata = new Metadata();
		}
	}
}
