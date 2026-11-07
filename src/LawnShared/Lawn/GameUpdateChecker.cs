using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lawn;

internal static class GameUpdateChecker
{
	public static async Task<string> GetNewVersionAsync(string versionUrl, string currentVersion)
	{
		if (!Version.TryParse(currentVersion, out Version current))
		{
			return null;
		}
		try
		{
			using HttpClient client = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(10.0)
			};
			string text = (await client.GetStringAsync(versionUrl).ConfigureAwait(continueOnCapturedContext: false)).Trim();
			Version result;
			return (Version.TryParse(text, out result) && result > current) ? text : null;
		}
		catch (HttpRequestException)
		{
			return null;
		}
		catch (OperationCanceledException)
		{
			return null;
		}
	}
}
