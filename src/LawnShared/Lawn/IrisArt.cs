using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;

namespace Lawn;

/// <summary>
/// iOS-port original plant art for the Iris Shooter, loaded from raw PNGs in Content/iris on first use.
/// Every image is optional: callers fall back to the prototype look when one is missing.
/// </summary>
public static class IrisArt
{
	// Battlefield sprite layout (iris_body.png is 420x420): drawn IRIS_SIZE board units square,
	// feet on board y = plant y + IRIS_FEET_Y, centred on the cell.
	public const float SIZE = 100f;

	public const float FEET_Y = 72f;

	public const float FEET_IN_IMAGE = 414f / 420f;

	// Hair-ornament anchors inside the sprite, as fractions of the image.
	public static readonly Vector2 LaserOrigin = new Vector2(296f / 420f, 108f / 420f);

	public static readonly Vector2 ScanOrigin = new Vector2(148f / 420f, 74f / 420f);

	public static Image Body;

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
			return Body != null;
		}
	}

	public static void EnsureLoaded()
	{
		if (sTried)
		{
			return;
		}
		sTried = true;
		Body = Load("iris_body");
		Card = Load("iris_card");
		Beam = Load("fx_beam");
		Muzzle = Load("fx_muzzle");
		Hit = Load("fx_hit");
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

	/// <summary>Draws the battlefield sprite with the plant's board origin at (theOriginX, theOriginY) in screen pixels.</summary>
	public static void DrawBody(Graphics g, float theOriginX, float theOriginY, float theScale, float theBreath)
	{
		float s = Constants.S * theScale;
		float w = SIZE * s;
		float h = SIZE * s * (1f + theBreath);
		float feetY = theOriginY + FEET_Y * s;
		float top = feetY - h * FEET_IN_IMAGE;
		float left = theOriginX + (40f - SIZE / 2f) * s;
		g.DrawImage(Body, new TRect((int)left, (int)top, (int)w, (int)h), new TRect(0, 0, Body.mWidth, Body.mHeight));
	}

	/// <summary>Anchor of a hair ornament in board units relative to the plant origin.</summary>
	public static Vector2 Anchor(Vector2 theFraction, float theHeightOffset)
	{
		float left = 40f - SIZE / 2f;
		float top = FEET_Y - SIZE * FEET_IN_IMAGE;
		return new Vector2(left + theFraction.X * SIZE, theHeightOffset + top + theFraction.Y * SIZE);
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
}
