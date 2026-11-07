using System.Collections.Generic;

namespace Sexy.TodLib;

public class TrailDefinition
{
	public Image mImage;

	public Image[] mImages;

	public int mNumImages;

	public string mImageName;

	public int mMaxPoints;

	public float mMinPointDistance;

	public int mTrailFlags;

	public FloatParameterTrack mTrailDuration = new FloatParameterTrack();

	public FloatParameterTrack mWidthOverLength = new FloatParameterTrack();

	public FloatParameterTrack mWidthOverTime = new FloatParameterTrack();

	public FloatParameterTrack mAlphaOverLength = new FloatParameterTrack();

	public FloatParameterTrack mAlphaOverTime = new FloatParameterTrack();

	public TrailDefinition()
	{
		mMaxPoints = 2;
		mMinPointDistance = 1f;
		mTrailFlags = 0;
		mImage = null;
		mImages = null;
		mNumImages = 0;
	}

	public void ExtractImages()
	{
		if (string.IsNullOrEmpty(mImageName))
		{
			return;
		}
		mImage = AtlasResources.GetImageInAtlasById(AtlasResources.GetAtlasIdByStringId(mImageName));
		if (mImage != null)
		{
			GlobalStaticVars.gLawnApp.mImagesToBeFiltered.Enqueue(mImage);
			return;
		}
		List<Image> list = new List<Image>();
		for (int i = 0; i < 100; i++)
		{
			Image imageInAtlasById = AtlasResources.GetImageInAtlasById(AtlasResources.GetAtlasIdByStringId($"{mImageName}_{i}"));
			if (imageInAtlasById == null)
			{
				break;
			}
			list.Add(imageInAtlasById);
		}
		mImages = list.ToArray();
		mNumImages = mImages.Length;
	}

	public void Dispose()
	{
	}
}
