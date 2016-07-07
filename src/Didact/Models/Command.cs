using System;
using System.Collections.Generic;

namespace Didact.Models
{
	public class Command
	{
		public string Name { get; set; }

		public string CommandStr { get; set; }

		public Action<int> Action { get; set; }

		public IList<Argument> Arguments { get; set; } = new List<Argument>();
	}
}
