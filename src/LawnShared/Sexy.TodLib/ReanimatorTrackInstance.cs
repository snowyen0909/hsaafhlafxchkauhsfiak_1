using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Sexy.TodLib;

public class ReanimatorTrackInstance
{
	public byte mBlendCounter;

	public byte mBlendTime;

	public ReanimatorTransform mBlendTransform;

	public float mShakeOverride;

	public float mShakeX;

	public float mShakeY;

	public Attachment mAttachmentID;

	public int mAttachmentID_Save;

	public Image mImageOverride;

	public int mRenderGroup;

	public SexyColor mTrackColor;

	public bool mIgnoreClipRect;

	public bool mTruncateDisappearingFrames;

	public bool mIgnoreColorOverride;

	public bool mIgnoreExtraAdditiveColor;

	private static Stack<ReanimatorTrackInstance> unusedObjects = new Stack<ReanimatorTrackInstance>();

	public static void PreallocateMemory()
	{
		for (int i = 0; i < 10000; i++)
		{
			new ReanimatorTrackInstance().PrepareForReuse();
		}
	}

	public static ReanimatorTrackInstance GetNewReanimatorTrackInstance()
	{
		if (unusedObjects.Count > 0)
		{
			return unusedObjects.Pop();
		}
		return new ReanimatorTrackInstance();
	}

	public void PrepareForReuse()
	{
		Reset();
		unusedObjects.Push(this);
	}

	public override string ToString()
	{
		return $"Group: {mRenderGroup}";
	}

	private ReanimatorTrackInstance()
	{
		Reset();
	}

	private void Reset()
	{
		if (mBlendTransform != null)
		{
			mBlendTransform.PrepareForReuse();
		}
		mBlendTransform = null;
		mBlendCounter = 0;
		mBlendTime = 0;
		mShakeOverride = 0f;
		mShakeX = 0f;
		mShakeY = 0f;
		mAttachmentID = null;
		mAttachmentID_Save = -1;
		mRenderGroup = 0;
		mIgnoreClipRect = false;
		mTruncateDisappearingFrames = true;
		mImageOverride = null;
		mTrackColor = new SexyColor(Color.White);
		mIgnoreColorOverride = false;
		mIgnoreExtraAdditiveColor = false;
	}
}
