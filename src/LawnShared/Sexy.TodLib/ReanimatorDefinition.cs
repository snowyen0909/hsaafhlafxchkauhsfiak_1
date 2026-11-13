using System.IO;
using Spine;

namespace Sexy.TodLib;

public class ReanimatorDefinition
{
	public ReanimatorTrack[] mTracks;

	public short mTrackCount;

	public float mFPS;

	public ReanimAtlas mReanimAtlas;

	public bool mIsSpine;

	public string mSpineAtlasPath;

	public float mSpineScale;

	public Atlas mSpineAtlas;

	public SkeletonData mSpineSkeletonData;

	public ReanimatorDefinition()
	{
		mFPS = 12f;
		mTrackCount = 0;
		mReanimAtlas = null;
	}

	public void ExtractImages()
	{
		if (mIsSpine)
		{
			Lawn.WardrobeHotReload.RegisterSpineDefinition(this);
			if (mSpineSkeletonData == null)
			{
				mSpineAtlas = Lawn.WardrobeOverlay.LoadAtlas(Path.ChangeExtension(mSpineAtlasPath, "atlas"), GlobalStaticVars.g.GraphicsDevice);
				float num = 1f / mSpineScale * Constants.ISUI;
				SkeletonBinary skeletonBinary = new SkeletonBinary(mSpineAtlas);
				skeletonBinary.Scale *= num;
				mSpineSkeletonData = Lawn.WardrobeOverlay.ReadSkeletonBinary(skeletonBinary, Path.ChangeExtension(mSpineAtlasPath, "skel"));
				mFPS = mSpineSkeletonData.Fps;
			}
		}
		else
		{
			for (int i = 0; i < mTracks.Length; i++)
			{
				mTracks[i].ExtractImages();
			}
		}
	}
}
