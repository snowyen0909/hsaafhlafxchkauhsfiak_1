using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sexy;

public class MemoryImage : Image, IDisposable
{
	private static uint nextId;

	private RenderTarget2D renderTarget;

	private uint mId;

	public RenderTarget2D RenderTarget => renderTarget;

	public MemoryImage()
	{
		renderTarget = null;
		mId = 0u;
	}

	public void Clear()
	{
		GlobalStaticVars.g.Clear(new Color(0, 0, 0, 0));
	}

	public override void Dispose()
	{
		if (renderTarget != null)
		{
			renderTarget.Dispose();
		}
		base.Dispose();
	}

	public void Create(int theWidth, int theHeight, float theInvScale = 1f)
	{
		Create(theWidth, theHeight, PixelFormat.kPixelFormat_RGBA8888, theInvScale);
	}

	public void Create(int theWidth, int theHeight, PixelFormat thePixelFormat, float theInvScale = 1f)
	{
		mId = nextId++;
		mWidth = theWidth;
		mParentWidth = theWidth;
		mHeight = theHeight;
		mParentHeight = theHeight;
		int num = mWidth;
		int num2 = mHeight;
		mOwnsTexture = true;
		mMaxS = (float)mWidth / (float)num;
		mMaxT = (float)mHeight / (float)num2;
		renderTarget = new RenderTarget2D(GlobalStaticVars.g.GraphicsDevice, num, num2, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
		Texture = renderTarget;
		mInvScale = theInvScale;
	}
}
