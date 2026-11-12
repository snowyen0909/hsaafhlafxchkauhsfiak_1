using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;

namespace Lawn;

/// <summary>
/// iOS-port original plant art (Iris Shooter = "Iois" + floating apple cannon), loaded from raw PNGs in Content/iris.
/// The character is pre-baked from a Spine-style rig into sprite sheets (idle / raise / scan / fire / wink, 12 fps);
/// the cannon and all laser effects are drawn live so they can aim at the target.
/// Every image is optional: callers fall back to the prototype look when one is missing.
/// </summary>
public static class IrisArt
{
	// ---- character sheets (frame 185x260 px, 8 columns) ----
	public const int FRAME_W = 185;

	public const int FRAME_H = 260;

	public const int COLS = 8;

	public const int FPS = 12;

	public const int IDLE_FRAMES = 31;

	public const int RAISE_FRAMES = 6;

	public const int LOOP_FRAMES = 30;

	private const float FEET_IN_FRAME_X = 77.4f;

	private const float FEET_IN_FRAME_Y = 257.9f;

	/// <summary>Character height in board units and where her feet stand inside the 80-wide cell.</summary>
	public const float HEIGHT = 102f;

	public const float FEET_X = 38f;

	public const float FEET_Y = 78f;

	// ---- cannon sheet: 9 aim angles (-20..+20 deg, 5 deg steps), 200x200 frames ----
	public const int CANNON_FRAME = 200;

	public const int CANNON_ANGLES = 9;

	public const float CANNON_SIZE = 44f;

	private static readonly float[] sMuzzleX = { 166.1f, 168.2f, 169.9f, 171.0f, 171.6f, 171.6f, 171.1f, 170.0f, 168.4f };

	private static readonly float[] sMuzzleY = { 72.3f, 78.2f, 84.2f, 90.4f, 96.6f, 102.8f, 109.1f, 115.2f, 121.3f };

	public static Image Idle;

	public static Image Raise;

	public static Image ScanLoop;

	public static Image FireLoop;

	public static Image Wink;

	public static Image Cannon;

	public static Image Card;

	public static Image Beam;

	public static Image Muzzle;

	public static Image Hit;

	public static Image Scan;

	public static Image LockRed;

	public static Image LockGreen;

	public static Image Flash;

	private static bool sTried;

	public static bool Ready
	{
		get
		{
			EnsureLoaded();
			return Idle != null && Cannon != null;
		}
	}

	public static void EnsureLoaded()
	{
		if (sTried)
		{
			return;
		}
		sTried = true;
		Idle = Load("iois_idle");
		Raise = Load("iois_raise");
		ScanLoop = Load("iois_scan");
		FireLoop = Load("iois_fire");
		Wink = Load("iois_wink");
		Cannon = Load("iois_cannon");
		Card = Load("iris_card");
		Beam = Load("fxv_beam");
		Muzzle = Load("fxv_muzzle");
		Hit = Load("fxv_hit");
		Scan = Load("fx_scan");
		LockRed = Load("fx_lockred");
		LockGreen = Load("fx_lockgreen");
		Flash = Load("fx_flash");
	}

	private static Image Load(string theName)
	{
		try
		{
			using Stream stream = TitleContainer.OpenStream("Content/iris/" + theName + ".png");
			Texture2D theTexture = Texture2D.FromStream(GlobalStaticVars.g.GraphicsDevice, stream, DefaultColorProcessors.PremultiplyAlpha);
			return new Image(theTexture);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("IrisArt: cannot load " + theName + ": " + ex.Message);
			return null;
		}
	}

	/// <summary>Draws one character frame; (theOriginX, theOriginY) = plant board origin in screen pixels.</summary>
	public static void DrawCharacter(Graphics g, Image theSheet, int theFrame, float theOriginX, float theOriginY, float theScale)
	{
		if (theSheet == null)
		{
			theSheet = Idle;
			theFrame = 0;
		}
		float bu = HEIGHT / FRAME_H * Constants.S * theScale;
		float left = theOriginX + FEET_X * Constants.S * theScale - FEET_IN_FRAME_X * bu;
		float top = theOriginY + FEET_Y * Constants.S * theScale - FEET_IN_FRAME_Y * bu;
		int cols = Math.Min(COLS, theSheet.mWidth / FRAME_W);
		TRect src = new TRect(theFrame % cols * FRAME_W, theFrame / cols * FRAME_H, FRAME_W, FRAME_H);
		g.DrawImage(theSheet, new TRect((int)left, (int)top, (int)(FRAME_W * bu), (int)(FRAME_H * bu)), src);
	}

	/// <summary>Cannon centre in board units relative to the plant origin.</summary>
	public static Vector2 CannonCenter(int theCounter, float theEngage, float theHeightOffset)
	{
		float bob = 2.5f * (float)Math.Sin(theCounter * 2.0 * Math.PI / 210.0);
		return new Vector2(FEET_X + 34f + 9f * theEngage, FEET_Y - 70f + bob + theHeightOffset);
	}

	public static int AngleIndex(float theDegrees)
	{
		return Math.Clamp((int)Math.Round((theDegrees + 20f) / 5f), 0, CANNON_ANGLES - 1);
	}

	/// <summary>Muzzle position (board units, plant-relative) for a cannon at theCenter with aim frame theIndex.</summary>
	public static Vector2 MuzzleAt(Vector2 theCenter, int theIndex)
	{
		float k = CANNON_SIZE / CANNON_FRAME;
		return new Vector2(theCenter.X + (sMuzzleX[theIndex] - CANNON_FRAME / 2f) * k, theCenter.Y + (sMuzzleY[theIndex] - CANNON_FRAME / 2f) * k);
	}

	public static void DrawCannon(Graphics g, Vector2 theCenter, int theIndex, float theOriginX, float theOriginY)
	{
		float s = Constants.S;
		float size = CANNON_SIZE * s;
		float x = theOriginX + theCenter.X * s - size / 2f;
		float y = theOriginY + theCenter.Y * s - size / 2f;
		g.DrawImage(Cannon, new TRect((int)x, (int)y, (int)size, (int)size), new TRect(theIndex * CANNON_FRAME, 0, CANNON_FRAME, CANNON_FRAME));
	}

	/// <summary>Draws an image centred on (x, y) at the given size, tinted by alpha.</summary>
	public static void DrawCentered(Graphics g, Image theImage, float x, float y, float theWidth, float theHeight, int theAlpha)
	{
		if (theImage == null)
		{
			return;
		}
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new SexyColor(255, 255, 255, Math.Clamp(theAlpha, 0, 255)));
		g.DrawImage(theImage, new TRect((int)(x - theWidth / 2f), (int)(y - theHeight / 2f), (int)theWidth, (int)theHeight), new TRect(0, 0, theImage.mWidth, theImage.mHeight));
	}

	/// <summary>A straight band from p0 to p1 drawn as short horizontal slices of a horizontally-uniform strip (no rotation needed).</summary>
	public static void DrawBand(Graphics g, Image theStrip, Vector2 p0, Vector2 p1, float theThickness, int theAlpha)
	{
		if (theStrip == null)
		{
			return;
		}
		float dx = p1.X - p0.X;
		float dy = p1.Y - p0.Y;
		if (dx < 1f)
		{
			return;
		}
		int steps = Math.Max(1, (int)(Math.Abs(dy) / 1.5f));
		steps = Math.Min(steps, 120);
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new SexyColor(255, 255, 255, Math.Clamp(theAlpha, 0, 255)));
		float sliceW = dx / steps;
		for (int i = 0; i < steps; i++)
		{
			float x = p0.X + sliceW * i;
			float y = p0.Y + dy * (i + 0.5f) / steps;
			g.DrawImage(theStrip, new TRect((int)x, (int)(y - theThickness / 2f), (int)Math.Ceiling(sliceW) + 1, (int)theThickness), new TRect(0, 0, theStrip.mWidth, theStrip.mHeight));
		}
	}
}
