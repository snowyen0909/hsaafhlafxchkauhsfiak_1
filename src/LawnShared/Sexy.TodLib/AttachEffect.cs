using System.Collections.Generic;

namespace Sexy.TodLib;

public class AttachEffect
{
	public object mEffectID;

	public int mEffectID_Save;

	public EffectType mEffectType;

	public SexyTransform2D mOffset;

	public bool mDontDrawIfParentHidden;

	public bool mDontPropogateColor;

	private static Stack<AttachEffect> unusedObjects = new Stack<AttachEffect>(100);

	public static AttachEffect GetNewAttachEffect()
	{
		if (unusedObjects.Count > 0)
		{
			return unusedObjects.Pop();
		}
		return new AttachEffect();
	}

	public void PrepareForReuse()
	{
		Reset();
		unusedObjects.Push(this);
	}

	private AttachEffect()
	{
		Reset();
	}

	public void Reset()
	{
		mEffectID = null;
		mEffectType = EffectType.Particle;
		mOffset = default;
		mDontDrawIfParentHidden = false;
		mDontPropogateColor = false;
	}
}
