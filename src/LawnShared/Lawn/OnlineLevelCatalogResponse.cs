using System.Collections.Generic;

namespace Lawn;

public sealed class OnlineLevelCatalogResponse
{
	public List<OnlineLevelPreview> Items { get; set; }

	public long TotalCount { get; set; }

	public string NextCursor { get; set; }

	public bool HasMore { get; set; }
}
