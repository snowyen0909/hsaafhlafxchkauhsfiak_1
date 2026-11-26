// 塔维尔的植物娘 (built-in for the Mod edition; switched on in 选项). Port of 塔维尔's PC plant pack (nineplants).
// One TavelSpec describes one plant; the shared hooks (TavelHooks) look the spec up by seed number.
using System;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

/// <summary>How a Spine plant is drawn on seed packets and under the cursor (offsets in lawn units).</summary>
public sealed class TavelPreview
{
	public string Animation;

	public float SeedScale;

	public Vector2 SeedOffset;

	public float CursorScale;

	public Vector2 CursorOffset;

	public TavelPreview(string theAnimation, float theSeedScale, float theSeedX, float theSeedY, float theCursorScale, float theCursorX, float theCursorY)
	{
		Animation = theAnimation;
		SeedScale = theSeedScale;
		SeedOffset = new Vector2(theSeedX, theSeedY);
		CursorScale = theCursorScale;
		CursorOffset = new Vector2(theCursorX, theCursorY);
	}
}

public sealed class TavelSpec
{
	public int SeedId;

	public SeedType Seed => (SeedType)SeedId;

	public string Code;

	// --- identity and seed packet
	public string Name;

	/// <summary>Sun cost (-1: cannot be bought).</summary>
	public int Cost = -1;

	public Func<Board, int> CostFor;

	public int RefreshTime = 750;

	public bool StartsReady;

	public bool Selectable = true;

	public bool Conveyor = true;

	/// <summary>Content path of the packet picture (162x212, the layout of a cell of the packet sheet).</summary>
	public string Card;

	public string AlmanacText;

	public Action<Plant> OnAlmanac;

	// --- the game's plant definition
	public string DefName;

	public int DefCost;

	public int DefRefresh = 750;

	public int LaunchRate;

	/// <summary>Spine body file, e.g. "tavel/spanim/hjbl" (null: drawn by the plant's code).</summary>
	public string ReanimFile;

	public ReanimationType Reanim = ReanimationType.None;

	/// <summary>Body made by OnPlanted, not by the game's initialiser.</summary>
	public bool CustomBody;

	/// <summary>Seed type the game's initialiser sees (default: the plant's own).</summary>
	public SeedType NativeSeed = SeedType.None;

	// --- drawing
	public TavelPreview Preview;

	public Action<Graphics, float, float> DrawSeed;

	public Action<Graphics> DrawCursor;

	public Action<Plant, Graphics> Draw;

	public Action<Plant, Graphics> DrawSquished;

	// --- life cycle
	/// <summary>A game plant acting as this one (the small guard is a pumpkin underneath).</summary>
	public Func<Plant, bool> Claims;

	public Action<Plant, int, int> OnPlanted;

	public Action<Plant> OnUpdate;

	public Action<Plant> OnDie;

	public Action<Plant, object> OnSquished;

	public Action<Plant> SquishOverride;

	public Action<Plant> OnClick;

	// --- ultimate
	public Func<Plant, bool> CanUltimate;

	public Func<Plant, bool> UltimateActive;

	public Action<Plant> StartUltimate;

	/// <summary>After ANY plant starts an ultimate: (plant, propagate tiles, source).</summary>
	public Action<Plant, bool, string> OnAnyUltimate;

	// --- board-wide effects (projectiles, summons, tiles)
	public Action<Board> UpdateBoard;

	public Action<Board, Graphics> DrawBoardUnder;

	public Action<Board, Graphics> DrawBoard;

	// --- saved games (kept in a file beside the save)
	public Func<Plant, string> SaveState;

	public Action<Plant, string> LoadState;

	public Func<Board, string> SaveBoard;

	public Action<Board, string> LoadBoard;

	public Func<Board, bool> BoardHasData;

	public TavelSpec(int theSeedId, string theCode)
	{
		SeedId = theSeedId;
		Code = theCode;
	}
}

/// <summary>A grid of equally sized frames in one picture (plants drawn from pre-rendered frames instead of Spine).</summary>
public sealed class TavelSpriteSheet
{
	public readonly string Path;

	public readonly int Columns;

	public readonly int FrameWidth;

	public readonly int FrameHeight;

	public readonly int FrameCount;

	public readonly float AnchorX;

	public readonly float AnchorY;

	/// <summary>How tall a plant flattened by a gargantuar is drawn, and how far below her feet it lies.</summary>
	public const float SquishedHeight = 0.5f;

	public const float SquishedDrop = 26f;

	public TavelSpriteSheet(string thePath, int theColumns, int theFrameWidth, int theFrameHeight, int theFrameCount, float theAnchorX, float theAnchorY)
	{
		Path = thePath;
		Columns = theColumns;
		FrameWidth = theFrameWidth;
		FrameHeight = theFrameHeight;
		FrameCount = theFrameCount;
		AnchorX = theAnchorX;
		AnchorY = theAnchorY;
	}

	public Image Image => Tavel.LoadImage(Path);

	/// <summary>One frame with its anchor at (x, y) in g's coordinates.</summary>
	public void Draw(Graphics g, float theFrame, float x, float y, float theScaleX, float theScaleY = float.NaN)
	{
		if (float.IsNaN(theScaleY))
		{
			theScaleY = theScaleX;
		}
		Image img = Image;
		if (img == null)
		{
			return;
		}
		int index = ((int)theFrame % FrameCount + FrameCount) % FrameCount;
		TRect src = new TRect(index % Columns * FrameWidth, index / Columns * FrameHeight, FrameWidth, FrameHeight);
		TRect dst = new TRect((int)Math.Round(x - AnchorX * theScaleX), (int)Math.Round(y - AnchorY * theScaleY), (int)Math.Round(FrameWidth * theScaleX), (int)Math.Round(FrameHeight * theScaleY));
		g.DrawImage(img, dst, src);
	}
}
