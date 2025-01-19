using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.Tests;
public class BaseTest
{
	private static string _projectDir = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
	private static string _bianceApiOutputTestFolder = Path.Combine(_projectDir, "BianceAPI", "OutputTestContent");
	private static string _bianceClientOutputTestFolder = Path.Combine(_projectDir, "BinanceClient", "OutputTestContent");

	protected async Task<string> GetBianceApiTestFileContent(string filename)
	{
		var ext = Path.GetExtension(filename);
		if (string.IsNullOrEmpty(ext))
		{
			filename = filename + ".json";
		}
		var testDataPath = Path.Combine(_bianceApiOutputTestFolder, filename);
		return await File.ReadAllTextAsync(testDataPath);
	}
}
