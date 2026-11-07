using System;
using System.Collections.Generic;
using Sexy;

namespace Lawn;

public sealed class OnlineLevelPreview
{
	public string Id { get; set; }

	public string RevisionId { get; set; }

	public string TitleZhCn { get; set; }

	public string TitleEn { get; set; }

	public string Author { get; set; }

	public int Difficulty { get; set; }

	public List<string> Tags { get; set; }

	public bool IsEndless { get; set; }

	public int MinClientVersion { get; set; }

	public int RecommendedRank { get; set; }

	public long PopularityScore { get; set; }

	public OnlineLevelPayloadInfo Payload { get; set; }

	public OnlineLevelIconInfo Icon { get; set; }

	public DateTime PublishedAt { get; set; }

	public string LevelId => Id;

	public string ChineseName => TitleZhCn;

	public string EnglishName => TitleEn;

	public string Name
	{
		get
		{
			if (string.IsNullOrEmpty(TitleEn))
			{
				return TitleZhCn;
			}
			return TitleEn;
		}
	}

	public int IconId => Icon?.IconId ?? (-1);

	public string IconUrl => Icon?.Url;

	public int Downloads
	{
		get
		{
			if (PopularityScore <= int.MaxValue)
			{
				return (int)PopularityScore;
			}
			return int.MaxValue;
		}
	}

	public bool IsRecommended => RecommendedRank > 0;

	public byte[] CachedIconBytes { get; set; }

	public Image CachedIcon { get; set; }

	public bool IsDataLoaded { get; set; }
}
