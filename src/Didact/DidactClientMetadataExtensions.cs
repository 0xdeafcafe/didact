namespace Didact
{
	public static partial class DidactClientExtensions
	{
		public static DidactClient Name(this DidactClient didact, string name)
		{
			didact.Metadata.Name = name;
			return didact;
		}
		
		public static DidactClient CliName(this DidactClient didact, string cliName)
		{
			didact.Metadata.CliName = cliName;
			return didact;
		}

		public static DidactClient Version(this DidactClient didact, string version)
		{
			didact.Metadata.Version = version;
			return didact;
		}

		public static DidactClient Usage(this DidactClient didact, string usage)
		{
			didact.Metadata.Usage = usage;
			return didact;
		}
	}
}
