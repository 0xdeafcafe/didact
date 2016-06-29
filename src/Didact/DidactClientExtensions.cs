using Didact.Models;

namespace Didact
{
	public static class DidactClientExtensions
	{
		public static DidactClient Version(this DidactClient didact, string version)
		{
			didact.Metadata = didact.Metadata ?? new Metadata();
			didact.Metadata.Version = version;
			return didact;
		}
	}
}
