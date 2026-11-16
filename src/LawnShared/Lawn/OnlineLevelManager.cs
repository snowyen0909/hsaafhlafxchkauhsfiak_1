using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Lawn.Creative;
using Sexy;

namespace Lawn;

public sealed class OnlineLevelManager
{
	private const int MaxIconCacheCount = 100;

	private const int MaxIconBytes = 2097152;

	private const int MaxCatalogBytes = 1048576;

	private const int MaxPayloadBytes = 4194304;

	private readonly HttpClient mHttpClient;

	private readonly string mIconCacheDir;

	private readonly string mLevelCacheDir;

	private readonly List<string> mPageCursors = new List<string>();

	private string mSort = "recommended";

	private static int PageSize
	{
		get
		{
			if (!Constants.Is500pMode)
			{
				return 20;
			}
			return 15;
		}
	}

	public bool IsInitialized { get; private set; }

	public int CurrentPage { get; private set; }

	public int TotalPages { get; private set; }

	public long TotalCount { get; private set; }

	public string LastError { get; private set; }

	public List<OnlineLevelPreview> CurrentLevels { get; private set; }

	public OnlineLevelManager()
	{
		mHttpClient = CreateHttpClient();
		mPageCursors.Add(null);
		LawnApp gLawnApp = GlobalStaticVars.gLawnApp;
		string path = ((gLawnApp != null) ? Path.Combine(gLawnApp.applicationStoragePath, GlobalStaticVars.GetDocumentsDir(), "cache", "online-levels-v2") : Path.Combine(Environment.CurrentDirectory, "docs", "cache", "online-levels-v2"));
		mIconCacheDir = Path.Combine(path, "icons");
		mLevelCacheDir = Path.Combine(path, "payloads");
		Directory.CreateDirectory(mIconCacheDir);
		Directory.CreateDirectory(mLevelCacheDir);
		CurrentLevels = new List<OnlineLevelPreview>();
	}

	public async Task InitializeAsync()
	{
		IsInitialized = await ResetAndLoadAsync("recommended");
	}

	public async Task<bool> ReloadAsync()
	{
		return await ResetAndLoadAsync(mSort);
	}

	public async Task<bool> LoadPageAsync(int page, string sortBy = "recommended")
	{
		string text = NormalizeSort(sortBy);
		if (!string.Equals(text, mSort, StringComparison.Ordinal))
		{
			bool flag = page == 1;
			bool flag2 = flag;
			if (flag2)
			{
				flag2 = await ResetAndLoadAsync(text);
			}
			return flag2;
		}
		if (!IsInitialized)
		{
			return await ResetAndLoadAsync(text);
		}
		if (page == CurrentPage)
		{
			return true;
		}
		if (page < 1 || page > TotalPages || page > mPageCursors.Count)
		{
			return false;
		}
		return await LoadCursorAsync(mPageCursors[page - 1], page);
	}

	public async Task<bool> DownloadLevelDataAsync(OnlineLevelPreview preview, GameMode gameMode)
	{
		LastError = null;
		if (preview?.Payload == null || string.IsNullOrEmpty(preview.Id))
		{
			LastError = "在线关卡元数据不完整";
			return false;
		}
		try
		{
			ValidatePayloadMetadata(preview.Payload);
			string cachePath = GetCachePath(mLevelCacheDir, preview.Payload.PayloadSha256, ".zlib");
			if (File.Exists(cachePath))
			{
				FileInfo fileInfo = new FileInfo(cachePath);
				if (fileInfo.Length == preview.Payload.CompressedSize && TryValidateAndDecompress(await File.ReadAllBytesAsync(cachePath), preview.Payload, out var decompressed, out var _) && TryLoadLevel(preview, gameMode, decompressed))
				{
					return true;
				}
				File.Delete(cachePath);
			}
			using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, CreateSameOriginUri(preview.Payload.Url));
			using HttpResponseMessage response = await mHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
			if (!response.IsSuccessStatusCode)
			{
				LastError = $"在线关卡下载失败：HTTP {(int)response.StatusCode}";
				return false;
			}
			byte[] array = await ReadBoundedAsync(response, 4194304);
			if (!TryValidateAndDecompress(array, preview.Payload, out var decompressed2, out var error2))
			{
				LastError = error2;
				return false;
			}
			if (!TryLoadLevel(preview, gameMode, decompressed2))
			{
				return false;
			}
			try
			{
				await File.WriteAllBytesAsync(cachePath, array);
			}
			catch (Exception ex)
			{
				Debug.Log(DebugType.Warn, "Cache online level failed: " + ex.Message);
			}
			return true;
		}
		catch (Exception ex2)
		{
			LastError = "在线关卡下载失败：" + ex2.Message;
			Debug.Log(DebugType.Error, "Download online level " + preview.Id + " failed: " + ex2.Message);
			return false;
		}
	}

	public void ClearCache()
	{
		ClearCacheDirectory(mIconCacheDir);
		ClearCacheDirectory(mLevelCacheDir);
	}

	private static HttpClient CreateHttpClient()
	{
		HttpClient httpClient = new HttpClient(new HttpClientHandler
		{
			AutomaticDecompression = DecompressionMethods.None
		})
		{
			Timeout = TimeSpan.FromSeconds(30.0)
		};
		httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PlantGirlsVsZombies.NET OnlineLevelClient");
		return httpClient;
	}

	private async Task<bool> ResetAndLoadAsync(string sort)
	{
		LastError = null;
		IsInitialized = false;
		CurrentPage = 0;
		TotalPages = 0;
		TotalCount = 0L;
		CurrentLevels = new List<OnlineLevelPreview>();
		mSort = NormalizeSort(sort);
		mPageCursors.Clear();
		mPageCursors.Add(null);
		return IsInitialized = await LoadCursorAsync(null, 1);
	}

	private async Task<bool> LoadCursorAsync(string cursor, int page)
	{
		int pageSize = PageSize;
		LastError = null;
		try
		{
			using HttpResponseMessage response = await mHttpClient.GetAsync(BuildCatalogUrl(cursor));
			if (!response.IsSuccessStatusCode)
			{
				LastError = $"加载在线关卡失败：HTTP {(int)response.StatusCode}";
				return false;
			}
			OnlineLevelCatalogResponse onlineLevelCatalogResponse = JsonSerializer.Deserialize<OnlineLevelCatalogResponse>(await ReadBoundedAsync(response, 1048576), new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});
			if (pageSize != PageSize)
			{
				return false;
			}
			if (onlineLevelCatalogResponse?.Items == null)
			{
				LastError = "在线关卡目录响应无效";
				return false;
			}
			CurrentLevels = onlineLevelCatalogResponse.Items;
			CurrentPage = page;
			TotalCount = onlineLevelCatalogResponse.TotalCount;
			TotalPages = Math.Max(1, (int)Math.Ceiling((double)onlineLevelCatalogResponse.TotalCount / (double)pageSize));
			UpdateNextPageCursor(page, onlineLevelCatalogResponse.NextCursor);
			DownloadIconsAsync(CurrentLevels);
			return true;
		}
		catch (Exception ex)
		{
			LastError = "加载在线关卡失败：" + ex.Message;
			Debug.Log(DebugType.Error, LastError);
			return false;
		}
	}

	private string BuildCatalogUrl(string cursor)
	{
		string text = $"{"https://levels.pgvz.top"}/api/v2/levels?clientVersion={99999}&limit={PageSize}&sort={Uri.EscapeDataString(mSort)}";
		if (!string.IsNullOrEmpty(cursor))
		{
			text = text + "&cursor=" + Uri.EscapeDataString(cursor);
		}
		return text;
	}

	private void UpdateNextPageCursor(int page, string nextCursor)
	{
		if (string.IsNullOrEmpty(nextCursor))
		{
			if (mPageCursors.Count > page)
			{
				mPageCursors.RemoveRange(page, mPageCursors.Count - page);
			}
		}
		else if (mPageCursors.Count == page)
		{
			mPageCursors.Add(nextCursor);
		}
		else if (mPageCursors.Count > page)
		{
			mPageCursors[page] = nextCursor;
		}
	}

	private async Task DownloadIconsAsync(List<OnlineLevelPreview> levels)
	{
		for (int i = 0; i < levels.Count; i++)
		{
			OnlineLevelPreview onlineLevelPreview = levels[i];
			if (onlineLevelPreview.Icon != null && !string.IsNullOrEmpty(onlineLevelPreview.Icon.Url) && !string.IsNullOrEmpty(onlineLevelPreview.Icon.Sha256))
			{
				OnlineLevelPreview onlineLevelPreview2 = onlineLevelPreview;
				onlineLevelPreview2.CachedIconBytes = await DownloadIconBytesAsync(onlineLevelPreview.Icon);
			}
		}
	}

	private async Task<byte[]> DownloadIconBytesAsync(OnlineLevelIconInfo icon)
	{
		_ = 3;
		try
		{
			ValidateSha256(icon.Sha256, "Sha256");
			string cachePath = GetCachePath(mIconCacheDir, icon.Sha256, ".png");
			if (File.Exists(cachePath))
			{
				FileInfo fileInfo = new FileInfo(cachePath);
				if (fileInfo.Length > 0 && fileInfo.Length <= 2097152)
				{
					byte[] array = await File.ReadAllBytesAsync(cachePath);
					if (HasExpectedHash(array, icon.Sha256))
					{
						return array;
					}
				}
				File.Delete(cachePath);
			}
			using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, CreateSameOriginUri(icon.Url));
			using HttpResponseMessage response = await mHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
			if (!response.IsSuccessStatusCode)
			{
				return null;
			}
			byte[] bytes = await ReadBoundedAsync(response, 2097152);
			if (!HasExpectedHash(bytes, icon.Sha256))
			{
				throw new InvalidDataException("在线关卡图标哈希与目录元数据不一致");
			}
			await File.WriteAllBytesAsync(cachePath, bytes);
			CleanIconCache();
			return bytes;
		}
		catch (Exception ex)
		{
			Debug.Log(DebugType.Error, "Download online level icon failed: " + ex.Message);
			return null;
		}
	}

	private bool TryLoadLevel(OnlineLevelPreview preview, GameMode gameMode, byte[] levelData)
	{
		try
		{
			CreativeLevelManager.LoadLevel(levelData, gameMode, preview.Id + ".json", preview.Id);
			preview.IsDataLoaded = true;
			return true;
		}
		catch (Exception ex)
		{
			LastError = CreativeLevelManager.mError ?? ex.Message;
			Debug.Log(DebugType.Error, "Load online level failed (" + preview.Id + "): " + LastError);
			return false;
		}
	}

	private static bool TryValidateAndDecompress(byte[] compressed, OnlineLevelPayloadInfo metadata, out byte[] decompressed, out string error)
	{
		decompressed = null;
		error = null;
		try
		{
			if (compressed == null || compressed.Length == 0 || compressed.Length != metadata.CompressedSize || !HasExpectedHash(compressed, metadata.PayloadSha256))
			{
				error = "在线关卡压缩数据与目录元数据不一致";
				return false;
			}
			decompressed = DecompressZlib(compressed, metadata.UncompressedSize);
			if (decompressed.Length != metadata.UncompressedSize || !HasExpectedHash(decompressed, metadata.ContentSha256))
			{
				decompressed = null;
				error = "在线关卡解压数据与目录元数据不一致";
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			decompressed = null;
			error = "在线关卡解压失败：" + ex.Message;
			return false;
		}
	}

	private static byte[] DecompressZlib(byte[] compressed, int expectedSize)
	{
		if (expectedSize <= 0 || expectedSize > 4194304)
		{
			throw new InvalidDataException("在线关卡解压大小无效");
		}
		using MemoryStream stream = new MemoryStream(compressed, writable: false);
		using ZLibStream zLibStream = new ZLibStream(stream, CompressionMode.Decompress);
		using MemoryStream memoryStream = new MemoryStream(expectedSize);
		byte[] array = new byte[16384];
		while (true)
		{
			int num = zLibStream.Read(array, 0, array.Length);
			if (num == 0)
			{
				break;
			}
			if (memoryStream.Length + num > expectedSize)
			{
				throw new InvalidDataException("在线关卡超过声明的解压大小");
			}
			memoryStream.Write(array, 0, num);
		}
		return memoryStream.ToArray();
	}

	private static async Task<byte[]> ReadBoundedAsync(HttpResponseMessage response, int maxBytes)
	{
		long? contentLength = response.Content.Headers.ContentLength;
		if (contentLength.HasValue && (contentLength.Value <= 0 || contentLength.Value > maxBytes))
		{
			throw new InvalidDataException("在线关卡服务器响应超过大小限制");
		}
		using Stream stream = await response.Content.ReadAsStreamAsync();
		using MemoryStream output = new MemoryStream((int)(contentLength.HasValue ? contentLength.Value : 0));
		byte[] buffer = new byte[16384];
		while (true)
		{
			int num = await stream.ReadAsync(buffer, 0, buffer.Length);
			if (num == 0)
			{
				break;
			}
			if (output.Length + num > maxBytes)
			{
				throw new InvalidDataException("在线关卡服务器响应超过大小限制");
			}
			output.Write(buffer, 0, num);
		}
		if (output.Length == 0L)
		{
			throw new InvalidDataException("在线关卡服务器响应为空");
		}
		return output.ToArray();
	}

	private static void ValidatePayloadMetadata(OnlineLevelPayloadInfo payload)
	{
		if (!string.Equals(payload.Encoding, "zlib", StringComparison.Ordinal) || payload.CompressedSize <= 0 || payload.CompressedSize > 4194304 || payload.UncompressedSize <= 0 || payload.UncompressedSize > 4194304)
		{
			throw new InvalidDataException("在线关卡压缩元数据无效");
		}
		ValidateSha256(payload.PayloadSha256, "PayloadSha256");
		ValidateSha256(payload.ContentSha256, "ContentSha256");
	}

	private static void ValidateSha256(string hash, string name)
	{
		if (hash == null || hash.Length != 64)
		{
			throw new InvalidDataException(name + " 不是有效的 SHA-256 哈希");
		}
		for (int i = 0; i < hash.Length; i++)
		{
			if (!Uri.IsHexDigit(hash[i]))
			{
				throw new InvalidDataException(name + " 不是有效的 SHA-256 哈希");
			}
		}
	}

	private static bool HasExpectedHash(byte[] data, string expectedHash)
	{
		string a = Convert.ToHexString(SHA256.HashData(data));
		return string.Equals(a, expectedHash, StringComparison.OrdinalIgnoreCase);
	}

	private static Uri CreateSameOriginUri(string relativeOrAbsoluteUrl)
	{
		Uri uri = new Uri("https://levels.pgvz.top", UriKind.Absolute);
		Uri uri2 = new Uri(uri, relativeOrAbsoluteUrl);
		if (uri2.Scheme != Uri.UriSchemeHttps || !string.Equals(uri2.Host, uri.Host, StringComparison.OrdinalIgnoreCase) || uri2.Port != uri.Port)
		{
			throw new InvalidDataException("在线关卡资源地址不可信");
		}
		return uri2;
	}

	private static string GetCachePath(string directory, string hash, string extension)
	{
		return Path.Combine(directory, hash.ToLowerInvariant() + extension);
	}

	private static string NormalizeSort(string sort)
	{
		if (string.Equals(sort, "downloads", StringComparison.OrdinalIgnoreCase))
		{
			return "popular";
		}
		switch (sort)
		{
		default:
			return "recommended";
		case "newest":
		case "popular":
		case "difficultyAsc":
		case "difficultyDesc":
			return sort;
		}
	}

	private void CleanIconCache()
	{
		try
		{
			string[] files = Directory.GetFiles(mIconCacheDir, "*.png");
			if (files.Length > 100)
			{
				Array.Sort(files, (string left, string right) => File.GetLastAccessTimeUtc(left).CompareTo(File.GetLastAccessTimeUtc(right)));
				for (int num = 0; num < files.Length - 100; num++)
				{
					File.Delete(files[num]);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log(DebugType.Error, "Clean online icon cache failed: " + ex.Message);
		}
	}

	private static void ClearCacheDirectory(string directory)
	{
		try
		{
			if (Directory.Exists(directory))
			{
				Directory.Delete(directory, recursive: true);
			}
			Directory.CreateDirectory(directory);
		}
		catch (Exception ex)
		{
			Debug.Log(DebugType.Error, "Clear online level cache failed: " + ex.Message);
		}
	}
}
