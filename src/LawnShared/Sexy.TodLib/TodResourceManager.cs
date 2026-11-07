using System;
using System.Collections.Generic;

namespace Sexy.TodLib;

internal class TodResourceManager : ResourceManager
{
	public TodResourceManager()
		: base(GlobalStaticVars.gSexyAppBase)
	{
	}

	public bool FindImagePath(Image theImage, ref string thePath)
	{
		foreach (KeyValuePair<string, BaseRes> item in mImageMap)
		{
			ImageRes imageRes = (ImageRes)item.Value;
			if (imageRes.mImage == theImage)
			{
				thePath = mImageMap.GetEnumerator().Current.Key;
				return true;
			}
		}
		return false;
	}

	public bool TodLoadNextResource()
	{
		bool flag = false;
		while (!flag)
		{
			flag = mCurResGroupListItr.MoveNext();
			BaseRes current = mCurResGroupListItr.Current;
			if (current.mFromProgram)
			{
				continue;
			}
			switch (current.mType)
			{
			case ResType.ResType_Image:
			{
				ImageRes imageRes = (ImageRes)current;
				imageRes.mPalletize = false;
				if (imageRes.mImage != null)
				{
					continue;
				}
				break;
			}
			case ResType.ResType_Sound:
			{
				SoundRes soundRes = (SoundRes)current;
				if (soundRes.mSoundId != -1)
				{
					continue;
				}
				break;
			}
			case ResType.ResType_Font:
			{
				FontRes fontRes = (FontRes)current;
				if (fontRes.mFont != null)
				{
					continue;
				}
				break;
			}
			}
			break;
		}
		if (!flag)
		{
			return LoadNextResource();
		}
		return false;
	}

	public new bool TodLoadResources(string theGroup, bool doUnpackAtlasImages)
	{
		if (IsGroupLoaded(theGroup))
		{
			return true;
		}
		PerfTimer perfTimer = default;
		perfTimer.Start();
		StartLoadResources(theGroup);
		while (!GlobalStaticVars.gSexyAppBase.mShutdown && TodLoadNextResource())
		{
		}
		if (GlobalStaticVars.gSexyAppBase.mShutdown)
		{
			return false;
		}
		if (HadError())
		{
			return false;
		}
		mLoadedGroups.Add(theGroup);
		Math.Max((int)perfTimer.GetDuration(), 0);
		return true;
	}
}
