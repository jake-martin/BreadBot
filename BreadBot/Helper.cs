using System;

namespace BreadBot
{
	public class Helper
	{
		public static string GetEnvironmentVariable(string name)
		{
			return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
		}
	}
}
