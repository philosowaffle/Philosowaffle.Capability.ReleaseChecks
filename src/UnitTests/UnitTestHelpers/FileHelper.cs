namespace UnitTests.UnitTestHelpers;
public static class FileHelper
{
	public static readonly string DataDirectory = Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "test_data");

	public static async Task<string> ReadTextFromFileAsync(string path)
	{
		using (var reader = new StreamReader(path))
		{
			var content = await reader.ReadToEndAsync();
			return content;
		}
	}
}
