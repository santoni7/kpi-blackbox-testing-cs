using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KPI3.BlackBoxTest
{
	public class InputHelper
	{

		private static Random rand = new Random();
		public static string GenerateLine()
		{
			return System.DateTime.UtcNow.ToFileTimeUtc().ToString() + rand.Next().ToString();
		}
		public static string[] GenerateLines(int lines)
		{
			string[] res = new string[lines];
			for (int i = 0; i < lines; i++)
			{
				res[i] = GenerateLine();
			}
			return res;
		}
		public static string GenerateData(int lines)
		{
			return JoinToString(GenerateLines(lines));
		}

		public static string JoinToString(string[] arr)
		{
			var sb = new StringWriter();
			for (int i = 0; i < arr.Length; i++)
			{
				sb.WriteLine(arr[i]);
			}
			return sb.ToString();
		}
	}
}
