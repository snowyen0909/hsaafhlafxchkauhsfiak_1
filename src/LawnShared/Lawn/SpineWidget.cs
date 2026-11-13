using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Sexy.TodLib;
using Spine;

namespace Lawn;

public class SpineWidget : Widget
{
	public enum SkeletonFormat
	{
		Json,
		Skel
	}

	private SkeletonRenderer skeletonRenderer;

	private Atlas atlas;

	private Skeleton skeleton;

	private AnimationState state;

	private GraphicsDevice graphicsDevice;

	private string animationTrack;

	private float mPosX;

	private float mPosY;

	public int mAppCounterLast;

	public SpineWidget(string atlasPath, string jsonPath, float scale, SkeletonFormat skeletonFormat = SkeletonFormat.Json)
	{
		atlasPath = Path.Combine("Content/spanim", atlasPath + ".atlas");
		switch (skeletonFormat)
		{
		case SkeletonFormat.Json:
			jsonPath = Path.Combine("Content/spanim", jsonPath + ".json");
			break;
		case SkeletonFormat.Skel:
			jsonPath = Path.Combine("Content/spanim", jsonPath + ".skel");
			break;
		}
		graphicsDevice = GlobalStaticVars.g.GraphicsDevice;
		skeletonRenderer = new SkeletonRenderer(graphicsDevice, scissorTest: true);
		skeletonRenderer.PremultipliedAlpha = true;
		Effect effect = GlobalStaticVars.gLawnApp.mContentManager.Load<Effect>("shaders\\SpineEffect");
		effect.Parameters["World"].SetValue(Matrix.Identity);
		effect.Parameters["View"].SetValue(Matrix.CreateLookAt(new Vector3(0f, 0f, 1f), Vector3.Zero, Vector3.Up));
		skeletonRenderer.Effect = effect;
		LoadAnimation(atlasPath, jsonPath, scale, skeletonFormat);
	}

	private void LoadAnimation(string atlasPath, string jsonPath, float scale, SkeletonFormat skeletonFormat)
	{
		atlas = WardrobeOverlay.LoadAtlas(atlasPath, GlobalStaticVars.g.GraphicsDevice);
		SkeletonLoader skeletonLoader = null;
		switch (skeletonFormat)
		{
		case SkeletonFormat.Json:
			skeletonLoader = new SkeletonJson(atlas);
			break;
		case SkeletonFormat.Skel:
			skeletonLoader = new SkeletonBinary(atlas);
			break;
		}
		skeletonLoader.Scale *= scale;
		SkeletonData data = (skeletonLoader is SkeletonBinary sb) ? WardrobeOverlay.ReadSkeletonBinary(sb, jsonPath) : WardrobeOverlay.ReadSkeletonJson((SkeletonJson)skeletonLoader, jsonPath);
		skeleton = new Skeleton(data);
		AnimationStateData data2 = new AnimationStateData(skeleton.Data);
		state = new AnimationState(data2);
		skeleton.X = mX;
		skeleton.Y = mY;
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		base.Resize(theX, theY, theWidth, theHeight);
		CGPoint absPos = GetAbsPos();
		skeleton.X = mPosY + absPos.X;
		skeleton.Y = mPosX + absPos.Y;
	}

	public void SetSkeletonPos(float x, float y)
	{
		CGPoint absPos = GetAbsPos();
		mPosY = y;
		mPosX = x;
		skeleton.X = absPos.X + x;
		skeleton.Y = absPos.Y + y;
	}

	public void SetAnimationTrack(string track, ReanimLoopType loopType)
	{
		if (loopType != ReanimLoopType.Loop && loopType != ReanimLoopType.PlayOnce)
		{
			throw new NotImplementedException("Only Loop and PlayOnce loop types are supported");
		}
		animationTrack = track;
		state.SetAnimation(0, animationTrack, loopType == ReanimLoopType.Loop);
	}

	public override void Draw(Graphics g)
	{
		skeleton.UpdateWorldTransform(Skeleton.Physics.Update);
		ScreenScales mScreenScales = GlobalStaticVars.gSexyAppBase.mScreenScales;
		float num = (float)mScreenScales.mTransX / mScreenScales.mScaleFactor;
		float num2 = (float)mScreenScales.mTransY / mScreenScales.mScaleFactorY;
		float num3 = (float)g.GraphicsDevice.PresentationParameters.BackBufferWidth / mScreenScales.mScaleFactor - (float)Constants.BackBufferSize.Y - num;
		float num4 = (float)g.GraphicsDevice.PresentationParameters.BackBufferHeight / mScreenScales.mScaleFactorY - (float)Constants.BackBufferSize.X - num2;
		skeletonRenderer.Effect.Parameters["Projection"].SetValue(Matrix.CreateOrthographicOffCenter(0f - num, (float)Constants.BackBufferSize.Y + num3, (float)Constants.BackBufferSize.X + num4, 0f - num2, 0f, 1f));
		if (g.mColorizeImages)
		{
			skeleton.R = (float)(int)g.mColor.R / 255f;
			skeleton.G = (float)(int)g.mColor.G / 255f;
			skeleton.B = (float)(int)g.mColor.B / 255f;
			skeleton.A = (float)(int)g.mColor.A / 255f;
		}
		else
		{
			skeleton.R = 1f;
			skeleton.G = 1f;
			skeleton.B = 1f;
			skeleton.A = 1f;
		}
		g.EndFrame();
		TRect inRect = new TRect(mX, mY, mWidth, mHeight).Intersection(g.GetClipRect());
		GlobalStaticVars.gSexyAppBase.mScreenScales.Scale(ref inRect);
		graphicsDevice.ScissorRectangle = inRect;
		skeletonRenderer.Begin();
		skeletonRenderer.Draw(skeleton);
		skeletonRenderer.End();
		g.EndHardwareClip();
	}

	public override void Update()
	{
		state.Update(0.01f);
		state.Apply(skeleton);
	}

	public void Update_FromDraw(int appCounter)
	{
		if (appCounter - mAppCounterLast > 0)
		{
			base.Update();
			state.Update(1f * (float)(appCounter - mAppCounterLast) / 100f);
		}
		mAppCounterLast = appCounter;
		state.Apply(skeleton);
	}
}
