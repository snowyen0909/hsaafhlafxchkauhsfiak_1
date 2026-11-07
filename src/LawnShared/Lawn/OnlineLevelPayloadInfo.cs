namespace Lawn;

public sealed class OnlineLevelPayloadInfo
{
	public string Url { get; set; }

	public string Encoding { get; set; }

	public string PayloadSha256 { get; set; }

	public string ContentSha256 { get; set; }

	public int CompressedSize { get; set; }

	public int UncompressedSize { get; set; }
}
