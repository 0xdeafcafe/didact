using Xunit;
using Didact;
using System;

namespace Tests
{
	public class OptionsTests
	{
		[Fact]
		public void Check_Valid_Flags_Format()
		{
			new DidactClient()
				.Option("-a, --test [test]", "Example Option A")
				.Option("-b, --test-b [test]", "Example Option B")
				.Option("-c, --test-C [test]", "Example Option C")
				.Option("-d, --testDddd [test]", "Example Option D")
				.Parse(new string[0]);

			Assert.True(true);
		}

		[Theory]
		[InlineData("-a -test")]
		[InlineData("-a, -test")]
		[InlineData("-a, --test")]
		[InlineData("-a, --test[test]")]
		[InlineData("-a")]
		public void Check_Invalid_Flags_Format()
		{
			try
			{
				new DidactClient()
					.Option("-a, --test [test]", "Invalid Flags Format")
					.Parse(new string[0]);
			}
			catch (FormatException)
			{
				Assert.True(true);
			}
		}

		[Fact]
		public void Check_Options_Validation()
		{
			try
			{
				new DidactClient()
					.Option("-t, --test [test]", "Validation Data", validate: (val) => { return (val == "data"); })
					.Command("test [arg]", "Test Command", (arg, opt) => { })
					.Parse(new [] { "test", "-t", "data" });
			}
			catch (FormatException ex)
			{
				if (ex.Message.EndsWith("validation."))
					Assert.True(false);
			}
		}

		[Fact]
		public void Check_Options_Parsing()
		{
			new DidactClient()
				.Option("-t, --test [test]", "Validation Data", parse: (val) => { return val + " :)"; })
				.Command("test [arg]", "Test Command", (arg, opt) =>
				{
					var x = opt["t"];
					Assert.True(x == "data :)");
				})
				.Parse(new [] { "test", "-t", "data" });
		}
	}
}
