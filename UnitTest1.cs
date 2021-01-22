using IIG.FileWorker;
using System;
using System.IO;
using Xunit;

namespace KPI3.BlackBoxTest
{
	public class UnitTest1
	{
		private const string OutputFilePath = "test_out.txt";

		[Fact]
		public void Test1()
		{
			var input_content = "test string";
			BaseFileWorker.Write(input_content, OutputFilePath);
			var out_fileName = BaseFileWorker.GetFileName(OutputFilePath);
			var out_contents = File.ReadAllText(OutputFilePath+"l");
			Console.WriteLine("out_contents: " + out_contents);
			Console.WriteLine("out_fileName: " + out_fileName);
			Assert.Equal(input_content, out_contents);
			Assert.Equal(OutputFilePath, out_fileName);
			//BaseFileWorker.Wr
		}
	}
}
