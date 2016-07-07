using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Didact.Models
{
	public class Command
	{
		public string Name { get; set; }

		public string CommandStr { get; set; }

		public Action<Dictionary<string, string>, Dictionary<string, string>> Action { get; set; }
		
		public Func<Dictionary<string, string>, Dictionary<string, string>, Task> ActionAsync { get; set; }

		public IList<Argument> Arguments { get; set; } = new List<Argument>();
	}
}
