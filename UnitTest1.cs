using IIG.FileWorker;
using System;
using System.IO;
using Xunit;

namespace KPI3.BlackBoxTest
{
	public class UnitTest1
	{
		const string NON_EXISTENT_PATH_INVALID = "^$:/\\fd:\\fwe/wef\n\t\r\n";
		const string NON_EXISTENT_PATH_NULL = null;
		const string NON_EXISTENT_PATH_EMPTY = "";
		const string NON_EXISTENT_PATH_RELATIVE = "some-non-existing-file.dllx";
		const string NON_EXISTENT_PATH_VALID = "C:\\Windows\\Another-non-existing-file.dllx";

		const string TEMP_FILE_PATH = "_test_file.tmp";
		const string TEMP_FILE_PATH_IN = "_test_file.tmp";
		const string TEMP_FILE_PATH_OUT = "_test_file_out.tmp";

		string TOO_LONG_PATH = new string('a', 300); // must be too long to successfully create a file/dir



		const string OutputFilePath = "test_out.txt";
		private string CurrentDirectory = Directory.GetCurrentDirectory();


		[Theory]
		[InlineData("test_readfilename_1.txt")]
		[InlineData("test_readfilename_2.txt")]
		[InlineData("test_dir\\test_readfilename_2.txt")]
		[InlineData(NON_EXISTENT_PATH_INVALID)]
		[InlineData(NON_EXISTENT_PATH_NULL)]
		[InlineData(NON_EXISTENT_PATH_EMPTY)]
		[InlineData(NON_EXISTENT_PATH_RELATIVE)]
		[InlineData(NON_EXISTENT_PATH_VALID)]
		public void ReadFileName_ReturnsNull_NonExistFiles(string fileName)
		{
			try {
				File.Delete(fileName);
			} catch(Exception e) { }
			var path = $"{CurrentDirectory}\\{fileName}";
			Action codeToTest = () =>
			{
				var res_fileName = BaseFileWorker.GetFileName(fileName);
				var res_fileNameFromPath = BaseFileWorker.GetFileName(path);
				Assert.Null(res_fileName);
				Assert.Null(res_fileNameFromPath);
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
		}

		[Theory]
		[InlineData("test_readfilename_1.txt")]
		[InlineData("test_readfilename_2.txt", "test_dir_1")]
		[InlineData("test_readfilename_2.txt", "test_dir_2")]
		public void ReadFileName_ReadsExistingFileName(string fileName, string subdir = null)
		{
			if (subdir != null) Directory.CreateDirectory(subdir);
			string subdirOrEmpty = subdir != null ? subdir + "\\" : "";
			var path = $"{CurrentDirectory}\\{subdirOrEmpty}{fileName}";

			File.WriteAllText(path, "");

			string res_fileName = null;

			Action codeToTest = () =>
			{
				res_fileName = BaseFileWorker.GetFileName(path);
			};
			var ex = Record.Exception(codeToTest);

			Assert.Null(ex);
			Assert.NotNull(res_fileName);
			Assert.Equal(fileName, res_fileName);

			File.Delete(path); // cleanup
			if (subdir != null) Directory.Delete(subdir);
		}

		[Theory]
		[InlineData("test_readall_1.txt", "test content")]
		[InlineData("test_readall_2.txt", "multi\nline\ntext")]
		[InlineData("test_readall_3.txt", "")] // empty string
		[InlineData("test_readall_4.txt", "\r\n")] // linebreak + caret
		[InlineData("test_readall_5.txt", "\n")] // linebreak
		public void ReadAll_ReadsExistingFiles(string path, string content)
		{
			// Create file with text to be read using trusted APIs
			File.WriteAllText(path, content);

			string results = null;
			Action codeToTest = () =>
			{
				results = BaseFileWorker.ReadAll(path);
			};
			// Save exception in case code fails
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
			Assert.Equal(content, results);

			// Cleanup after test run
			File.Delete(path);
		}

		[Theory]
		[InlineData(TEMP_FILE_PATH)]
		[InlineData("test2.txt")]
		public void GetFullPath_ReadsExistingFilePath(string file)
		{
			File.WriteAllText(file, "");
			string expected = $"{CurrentDirectory}\\{file}";
			string results = null;
			Action codeToTest = () =>
			{
				results = BaseFileWorker.GetFullPath(file);
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
			Assert.NotNull(results);
			Assert.Equal(expected, results);
			Assert.EndsWith(file, results);
			File.Delete(file);
		}


		[Theory]
		[InlineData("test")]
		[InlineData("test2.txt")]
		[InlineData(NON_EXISTENT_PATH_EMPTY)]
		[InlineData(NON_EXISTENT_PATH_NULL)]
		[InlineData(NON_EXISTENT_PATH_INVALID)]
		[InlineData(NON_EXISTENT_PATH_RELATIVE)]
		[InlineData(NON_EXISTENT_PATH_VALID)]
		public void GetFullPath_ReturnsNullNonExistentFile(string file)
		{
			try { File.Delete(file); } // make sure file doesn't exist
			catch(Exception e) { }
			Action codeToTest = () =>
			{
				string results = BaseFileWorker.GetFullPath(file);
				Assert.Null(results);
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
		}

		[Fact]
		public void GetPath_ReturnsDirPath_NestedDir()
		{
			string filename = "filename.txt";
			Directory.CreateDirectory("dir1"); Directory.CreateDirectory("dir1\\dir2");
			string expected_dirPath = $"{CurrentDirectory}\\dir1\\dir2";
			string path = $"dir1\\dir2\\{filename}";
			File.WriteAllText(path, "");
			string results = null;
			Action codeToTest = () =>
			{
				results = BaseFileWorker.GetPath(path);
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
			Assert.Equal(expected_dirPath, results);
		}


		[Fact]
		public void GetPath_ReturnsDirPath_CurrentDir()
		{
			string path = "filename.txt";
			File.WriteAllText(path, "");
			string results = null;
			string expected = CurrentDirectory;
			Action codeToTest = () =>
			{
				results = BaseFileWorker.GetPath(path);
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
			Assert.Equal(expected, results);
			File.Delete(path);
		}


		[Theory]
		[InlineData(NON_EXISTENT_PATH_NULL)]
		[InlineData(NON_EXISTENT_PATH_EMPTY)]
		[InlineData(NON_EXISTENT_PATH_INVALID)]
		[InlineData(NON_EXISTENT_PATH_VALID)]
		public void GetPath_ReturnsNull_NonExist_Files(string file)
		{
			try { File.Delete(file); } // ensure file doesn't exist
			catch (Exception e) { }
			Action codeToTest = () =>
			{
				var results = BaseFileWorker.GetPath(file);
				Assert.Null(results);
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
		}

		[Theory]
		[InlineData("testDir1")]
		[InlineData("C:\\iig-file-writer-test-directory")]
		void MkDir_CreatesNewDirectory(string dirPath)
		{
			if (Directory.Exists(dirPath)) Directory.Delete(dirPath);
			string result = null;
			var ex = Record.Exception(() =>
			{
				result = BaseFileWorker.MkDir(dirPath);
			});
			Assert.Null(ex);
			Assert.NotNull(result);
			Assert.True(Directory.Exists(dirPath));
			Assert.True(Directory.Exists(result));
			Directory.Delete(dirPath);
		}

		[Fact]
		void MkDir_ReturnsFalse_PathTooLong()
		{
			string result = "";
			var ex = Record.Exception(() =>
			{
				result = BaseFileWorker.MkDir(TOO_LONG_PATH);
			});
			Assert.Null(ex);
			Assert.Null(result);
		}

		[Theory]
		[InlineData("testDir1")]
		[InlineData("C:\\iig-file-writer-test-directory")]
		void MkDir_Returns_ExistingDirectory(string dirPath)
		{
			if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
			string result = null;
			var ex = Record.Exception(() =>
			{
				result = BaseFileWorker.MkDir(dirPath);
			});
			Assert.Null(ex);
			Assert.NotNull(result);
			Assert.True(Directory.Exists(dirPath));
			Assert.True(Directory.Exists(result));
			Directory.Delete(dirPath);
		}

		[Theory]
		[InlineData(NON_EXISTENT_PATH_INVALID)]
		[InlineData(NON_EXISTENT_PATH_NULL)]
		[InlineData(NON_EXISTENT_PATH_EMPTY)]
		void MkDir_ReturnsNull_InvalidPath(string dirPath)
		{
			string result = "";
			var ex = Record.Exception(() =>
			{
				result = BaseFileWorker.MkDir(dirPath);
			});
			Assert.Null(ex);
			Assert.Null(result);
		}


		[Theory]
		[InlineData("read-all-test.txt")]
		[InlineData(TEMP_FILE_PATH, 1000)]
		void ReadAll_ReadsText_Correctly(string path, int lines = 100)
		{
			string data = InputHelper.GenerateData(lines);
			File.WriteAllText(path, data);
			string result = null;
			Action codeToTest = () =>
			{
				result = BaseFileWorker.ReadAll(path);
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
			Assert.NotNull(result);
			Assert.Equal(data, result);
			File.Delete(path);
		}


		[Theory]
		[InlineData(NON_EXISTENT_PATH_EMPTY)]
		[InlineData(NON_EXISTENT_PATH_INVALID)]
		[InlineData(NON_EXISTENT_PATH_NULL)]
		[InlineData(NON_EXISTENT_PATH_RELATIVE)]
		[InlineData(NON_EXISTENT_PATH_VALID)]
		void ReadAll_ReturnsNull_InvalidPath(string path)
		{
			string result = "";
			Action codeToTest = () =>
			{
				result = BaseFileWorker.ReadAll(path);
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
			Assert.Null(result);
		}

		[Theory]
		[InlineData(TEMP_FILE_PATH, 100)]
		[InlineData(TEMP_FILE_PATH, 1000)]
		[InlineData(TEMP_FILE_PATH, 1)]
		[InlineData(TEMP_FILE_PATH, 0)]
		void ReadLines_ReadsLinesCorrectly(string path, int lineCount= 100)
		{
			string[] lines = InputHelper.GenerateLines(lineCount);
			string joinedToString = InputHelper.JoinToString(lines);
			File.WriteAllText(path, joinedToString);
			string[] result = null;
			Action codeToTest = () =>
			{
				result = BaseFileWorker.ReadLines(path);
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
			Assert.NotNull(result);
			Assert.Equal(lines, result);
			File.Delete(path);
		}


		[Theory]
		[InlineData(NON_EXISTENT_PATH_EMPTY)]
		[InlineData(NON_EXISTENT_PATH_INVALID)]
		[InlineData(NON_EXISTENT_PATH_NULL)]
		[InlineData(NON_EXISTENT_PATH_RELATIVE)]
		[InlineData(NON_EXISTENT_PATH_VALID)]
		void ReadLines_ReturnsNull_InvalidPath(string path)
		{
			string[] result = new string[1];
			Action codeToTest = () =>
			{
				result = BaseFileWorker.ReadLines(path);
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
			Assert.Null(result);
		}

		[Theory]
		[InlineData(NON_EXISTENT_PATH_EMPTY)]
		[InlineData(NON_EXISTENT_PATH_INVALID)]
		[InlineData(NON_EXISTENT_PATH_NULL)]
		[InlineData(NON_EXISTENT_PATH_RELATIVE)]
		[InlineData(NON_EXISTENT_PATH_VALID)]
		// Same but With 100 retries
		[InlineData(NON_EXISTENT_PATH_EMPTY, 100)]
		[InlineData(NON_EXISTENT_PATH_INVALID, 100)]
		[InlineData(NON_EXISTENT_PATH_NULL, 100)]
		[InlineData(NON_EXISTENT_PATH_RELATIVE, 100)]
		[InlineData(NON_EXISTENT_PATH_VALID, 100)]
		void TryCopy_ReturnsFalse_From_NonExistentPath(string from, Nullable<int> tries = null)
		{
			if (File.Exists(TEMP_FILE_PATH)) File.Delete(TEMP_FILE_PATH);
			const string to = TEMP_FILE_PATH;
			Action codeToTest = () =>
			{
				bool result;
				if (!tries.HasValue)
				{
					result = BaseFileWorker.TryCopy(from, to, true);
				} else
				{
					result = BaseFileWorker.TryCopy(from, to, true, tries.Value);
				}
				Assert.False(result);
				Assert.False(File.Exists(to));
			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
		}

		[Theory]
		[InlineData(NON_EXISTENT_PATH_EMPTY)]
		[InlineData(NON_EXISTENT_PATH_INVALID)]
		[InlineData(NON_EXISTENT_PATH_NULL)]
		[InlineData(NON_EXISTENT_PATH_EMPTY, 10)]
		[InlineData(NON_EXISTENT_PATH_INVALID, 10)]
		[InlineData(NON_EXISTENT_PATH_NULL, 10)]
		void TryCopy_ReturnsFalse_To_NonExistentPath(string to, int maxTries = 1)
		{
			string from = TEMP_FILE_PATH;
			var data = InputHelper.GenerateData(100);
			File.WriteAllText(from, data);//create or overwrite
			Action<int?, bool> codeToTest = (int? x, bool overwrite) =>
			{
				bool result;
				if (!x.HasValue)
				{
					result = BaseFileWorker.TryCopy(from, to, overwrite);
				}
				else
				{
					result = BaseFileWorker.TryCopy(from, to, overwrite, x.Value);
				}
				Assert.False(result);
				Assert.False(File.Exists(to));
			};

			
			for (int i = 0; i < maxTries; ++i)
			{
				var _ex = Record.Exception(() => { codeToTest.Invoke(i, true); });
				Assert.Null(_ex);
				_ex = Record.Exception(() => { codeToTest.Invoke(i, false); });
				Assert.Null(_ex);
			}
			// Test with default 'tries' arg value
			var ex = Record.Exception(() => { codeToTest.Invoke(null, true); });
			Assert.Null(ex);
			ex = Record.Exception(() => { codeToTest.Invoke(null, false); });
			Assert.Null(ex);
			File.Delete(from);
		}


		[Theory]
		[InlineData(TEMP_FILE_PATH_OUT)]
		void TryCopy_ReturnsTrue_SuccessfullCopy_NewFile(string to, Nullable<int> tries = null)
		{
			string from = TEMP_FILE_PATH;
			var data = InputHelper.GenerateData(100);
			File.WriteAllText(from, data);//create or overwrite
			Action codeToTest = () =>
			{
				bool result;
				if (!tries.HasValue)
				{
					result = BaseFileWorker.TryCopy(from, to, true);
				}
				else
				{
					result = BaseFileWorker.TryCopy(from, to, true, tries.Value);
				}
				Assert.True(result);
				Assert.True(File.Exists(to));
				Assert.Equal(data, File.ReadAllText(to));

			};
			var ex = Record.Exception(codeToTest);
			Assert.Null(ex);
			File.Delete(from);
		}

		[Fact]
		public void Test_Write_GetFileName_ReadAllText()
		{
			var input_content = "test string";
			BaseFileWorker.Write(input_content, OutputFilePath);
			var out_fileName = BaseFileWorker.GetFileName(OutputFilePath);
			var out_contents = File.ReadAllText(OutputFilePath);
			Assert.Equal(input_content, out_contents);
			Assert.Equal(OutputFilePath, out_fileName);
		}
	}
}
