// 宿舍 ↔ the rest of the game. Every call the dorm makes into engine / platform code goes through this file,
// so porting the dorm to another build (Android, PC) means: copy the Dorm folder + ContentMod/dorm, then fix
// only what does not compile here. Each method says what it relies on and what to use if it is missing.
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Spine;

namespace Lawn;

internal static class DormPlatform
{
	// ------------------------------------------------------------------ files

	/// <summary>
	/// Opens a file shipped with the game ("Content/dorm/heart.png", "Content/spanim/x.skel").
	/// iOS build: WardrobeOverlay.Open (honours wardrobe patches, falls back to TitleContainer.OpenStream).
	/// Without the wardrobe: return TitleContainer.OpenStream(thePath). On Android the files must be packed as
	/// raw assets (not .xnb), on PC copied next to the exe; both are read by TitleContainer.
	/// </summary>
	public static Stream Open(string thePath)
	{
		return WardrobeOverlay.Open(thePath);
	}

	/// <summary>
	/// A PNG as a premultiplied texture (dorm sprites and UI). Needs MonoGame 3.8.1+ for DefaultColorProcessors;
	/// on older MonoGame: Texture2D.FromStream(device, s), then multiply RGB by A over GetData / SetData.
	/// </summary>
	public static Texture2D LoadTexture(string thePath)
	{
		using Stream s = Open(thePath);
		return Texture2D.FromStream(GlobalStaticVars.g.GraphicsDevice, s, DefaultColorProcessors.PremultiplyAlpha);
	}

	/// <summary>
	/// A PNG drawn at a given size in layout units (the texture is kept at its full resolution, like atlas art:
	/// mInvScale = texture pixels per unit). Used for the main menu's 宿舍 / 时装柜 icons. Null if missing.
	/// </summary>
	public static Image LoadIcon(string thePath, int theUnitHeight)
	{
		try
		{
			Texture2D tex = LoadTexture(thePath);
			float k = (float)tex.Height / theUnitHeight;
			Image img = new Image(tex, 0, 0, (int)Math.Round(tex.Width / k), theUnitHeight);
			img.mInvScale = k;
			return img;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm icon " + thePath + ": " + ex.Message);
			return null;
		}
	}

	/// <summary>Spine atlas of a plant girl (Content/spanim/&lt;skel&gt;.atlas). Without the wardrobe: new Atlas(reader, dir, a TextureLoader of your own).</summary>
	public static Atlas LoadAtlas(string theAtlasPath)
	{
		return WardrobeOverlay.LoadAtlas(theAtlasPath, GlobalStaticVars.g.GraphicsDevice);
	}

	/// <summary>Spine 4.2 binary skeleton. The .skel files are 4.2: the target build must use the same spine runtime (lib/spine-monogame.dll).</summary>
	public static SkeletonData ReadSkeleton(Atlas theAtlas, string theSkelPath)
	{
		return WardrobeOverlay.ReadSkeletonBinary(new SkeletonBinary(theAtlas), theSkelPath);
	}

	/// <summary>
	/// Folder for the dorm saves (dorm.txt, dorm_owned.txt, dorm_girls.txt): the game's own userdata folder.
	/// iOS needs applicationStoragePath (the app bundle is read-only). Android / PC: any writable folder, e.g.
	/// the folder the game keeps its user*.dat in.
	/// </summary>
	public static string SaveDir => Path.Combine(GlobalStaticVars.gSexyAppBase.applicationStoragePath, GlobalStaticVars.GetDocumentsDir() + "userdata");

	/// <summary>
	/// The mod's own saves, apart from the game's: userdata/mod/ (wardrobe.json, profile&lt;id&gt;/dorm.json). Still inside
	/// userdata, so the game's "export saves" zips them too.
	/// </summary>
	public static string ModSaveDir => Path.Combine(SaveDir, "mod");

	/// <summary>The current game profile (each one has its own coins, so each one has its own dorm). 0 when none.</summary>
	public static uint ProfileId(LawnApp app) => app.mPlayerInfo?.mId ?? 0;

	/// <summary>Test tools (连点标题 5 下). False in a public build: the title then does nothing.</summary>
	public const bool TestTools = true;

	// ------------------------------------------------------------------ screen

	/// <summary>
	/// The part of the virtual 1000-wide layout that is really on screen (wide phones show more, iPads less).
	/// iOS build: ScreenScales.Visible*. If the target has no such helpers: return (0, 0, app.mWidth, app.mHeight).
	/// </summary>
	public static TRect VisibleArea(LawnApp app)
	{
		ScreenScales ss = app.mScreenScales;
		return new TRect(ss.VisibleLeft, ss.VisibleTop, ss.VisibleWidth, ss.VisibleHeight);
	}

	/// <summary>
	/// Projection for drawing a Spine skeleton in widget coordinates (the same one the game's own Spine plants use).
	/// If the target has no Graphics.SpineGetMatrices, copy it from Sexy/Graphics.cs of this repo.
	/// </summary>
	public static Matrix SpineProjection(Graphics g)
	{
		return g.SpineGetMatrices();
	}

	/// <summary>Shader for the girls (Content/shaders/LawnSpineEffect.xnb, built per platform); the game's Spine plants use the same one.</summary>
	public static Effect SpineEffect()
	{
		return GlobalStaticVars.gLawnApp.mContentManager.Load<Effect>("shaders\\LawnSpineEffect").Clone();
	}

	// ------------------------------------------------------------------ sound

	public static void PlaySound(int theSound)
	{
		GlobalStaticVars.gLawnApp.PlaySample(theSound);
	}

	/// <summary>The girl's zen-garden voice line (1 or 2). Leave empty if the target has no PlantVoice.</summary>
	public static void PlayVoice(SeedType theSeed, int theWhich)
	{
		try
		{
			PlantVoice.Play(theSeed, SeedType.None, theWhich == 1 ? PlantVoice.VoiceType.Garden1 : PlantVoice.VoiceType.Garden2);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm voice: " + ex.Message);
		}
	}

	/// <summary>Zen-garden music while the dorm is open; returns the tune that was playing so it can be restored.</summary>
	public static MusicTune StartDormMusic(LawnApp app)
	{
		try
		{
			MusicTune prev = app.mMusic.mCurMusicTune;
			app.mMusic.MakeSureMusicIsPlaying(MusicTune.ZenGarden);
			return prev;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm music: " + ex.Message);
			return MusicTune.None;
		}
	}

	/// <summary>Record player: switch the dorm's music to one of the game's tunes (ZenGarden = back to the dorm's own).</summary>
	public static void PlayTune(LawnApp app, MusicTune theTune)
	{
		try
		{
			app.mMusic.MakeSureMusicIsPlaying(theTune);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm music: " + ex.Message);
		}
	}

	public static void StopDormMusic(LawnApp app, MusicTune thePrevious)
	{
		try
		{
			if (thePrevious != MusicTune.None && app.mMusic.mCurMusicTune == MusicTune.ZenGarden)
			{
				app.mMusic.MakeSureMusicIsPlaying(thePrevious);
			}
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm music: " + ex.Message);
		}
	}

	// ------------------------------------------------------------------ player

	/// <summary>Coins of the current profile (the game shows them × 10: 10 coins = $100).</summary>
	public static int Coins(LawnApp app) => app.mPlayerInfo?.mCoins ?? 0;

	/// <summary>Spends coins and saves the profile at once. False when there is no profile or not enough coins.</summary>
	public static bool SpendCoins(LawnApp app, int theCoins)
	{
		PlayerInfo pi = app.mPlayerInfo;
		if (pi == null || pi.mCoins < theCoins)
		{
			return false;
		}
		pi.AddCoins(-theCoins);
		try
		{
			app.WriteCurrentUserConfig();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm coins save: " + ex.Message);
		}
		return true;
	}

	/// <summary>Gives coins back (furniture sold at half price) and saves the profile.</summary>
	public static void AddCoins(LawnApp app, int theCoins)
	{
		PlayerInfo pi = app.mPlayerInfo;
		if (pi == null)
		{
			return;
		}
		pi.AddCoins(theCoins);
		try
		{
			app.WriteCurrentUserConfig();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm coins save: " + ex.Message);
		}
	}

	/// <summary>Sun cost of a plant, read from the game (智慧树 questions). Needs Plant.GetCost.</summary>
	public static int SunCost(SeedType theSeed)
	{
		try
		{
			return Plant.GetCost(theSeed, SeedType.None);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm sun cost: " + ex.Message);
			return 0;
		}
	}

	/// <summary>Has the player unlocked this plant (almanac / seed chooser)? Same rule as the seed chooser.</summary>
	public static bool HasPlant(LawnApp app, SeedType theSeed)
	{
		return app.mPlayerInfo != null && app.HasSeedType(theSeed);
	}
}
