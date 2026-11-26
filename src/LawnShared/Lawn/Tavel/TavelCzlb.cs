// 瓷砖萝卜 (plant_czlb.py): turns her cell into a tile; plants on tiles pass their ultimates along the tiles.
using System;
using System.Collections.Generic;
using System.Linq;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public sealed partial class TavelBoard
{
	public HashSet<(int, int)> CzlbTiles = new HashSet<(int, int)>();

	public HashSet<(int, int)> CzlbReserved = new HashSet<(int, int)>();

	public int CzlbPlantedCount;

	public List<(Plant Plant, int Due)> TileQueue = new List<(Plant, int)>();
}

public static class TavelCzlb
{
	public const int Id = 107;

	public static SeedType Seed => (SeedType)Id;

	public const int TileChainDelayPerGrid = 30;

	public sealed class State
	{
		public string Mode = "stand";

		public bool TileCreated;
	}

	private static TavelSpec sSpec;

	public static State Get(Plant p) => p.mTavel as State ?? (State)(p.mTavel = new State());

	private static void Align(Plant p) => Tavel.Align(p, 1.8f, -100f, -248f);

	public static bool CellTaken(Board b, int x, int y)
	{
		if (b?.mTavel == null)
		{
			return false;
		}
		return b.mTavel.CzlbTiles.Contains((x, y)) || b.mTavel.CzlbReserved.Contains((x, y));
	}

	public static int NextCost(Board b)
	{
		if (b?.mTavel == null || b.mTavel.CzlbPlantedCount <= 0)
		{
			return 0;
		}
		return 25 * (1 << Math.Min(20, b.mTavel.CzlbPlantedCount - 1));
	}

	/// <summary>The export calls its stand animation "atand".</summary>
	private static void Play(Plant p, string theName, ReanimLoopType theLoop)
	{
		Tavel.PlayAnim(p, theName == "stand" ? "atand" : theName, theLoop, 30f);
	}

	public static TavelSpec Spec()
	{
		sSpec = new TavelSpec(Id, "czlb")
		{
			Name = "瓷砖萝卜",
			Cost = 0,
			CostFor = NextCost,
			Card = "tavel/czlb/card.png",
			AlmanacText = "瓷砖萝卜会把所在格转化为能够传递大招的瓷砖。\n{SHORTLINE}\n{KEYWORD}特点：{STAT}可种在草坪、水池、屋顶或已有植物所在格\n{KEYWORD}瓷砖：{STAT}让其他瓷砖上的植物依距离依次释放大招\n{NORMAL}\n{KEYWORD}阳光：{STAT}每次种植后翻倍\n{SHORTLINE}\n{FLAVOR}排好瓷砖，力量就会沿着花纹一路传过去。",
			OnAlmanac = delegate(Plant p)
			{
				Play(p, "stand", ReanimLoopType.Loop);
			},
			DefName = "CZLB",
			DefCost = 0,
			LaunchRate = 60,
			ReanimFile = "tavel/spanim/czlb",
			CustomBody = true,
			Preview = new TavelPreview("anim_atand", 0.48f, -25f, -74f, 1.8f, -100f, -248f),
			OnPlanted = delegate(Plant p, int x, int y)
			{
				Get(p);
				if (p.mBoard != null)
				{
					TavelBoard bs = Tavel.State(p.mBoard);
					bs.CzlbPlantedCount++;
					bs.CzlbReserved.Add((x, y));
					Tavel.StartPacketCooldown(p.mBoard, Seed);
				}
				Tavel.DropNativeBody(p);
				Tavel.CreateBody(p, sSpec.Reanim, 2, null);
				Play(p, "stand", ReanimLoopType.PlayOnceAndHold);
				Align(p);
			},
			OnUpdate = delegate(Plant p)
			{
				if (p.mDead || p.mSquished)
				{
					DropState(p);
					return;
				}
				if (p.mBoard != null)
				{
					Update(p);
				}
				Align(p);
			},
			OnDie = DropState,
			OnSquished = delegate(Plant p, object s)
			{
				ReleaseCell(p);
			},
			UpdateBoard = UpdateTileUltimates,
			SaveBoard = SaveBoard,
			LoadBoard = LoadBoard,
			BoardHasData = (Board b) => b.mTavel != null && (b.mTavel.CzlbTiles.Count > 0 || b.mTavel.CzlbPlantedCount > 0),
			// a plant standing on a tile passes its ultimate along the tiles
			OnAnyUltimate = delegate(Plant p, bool thePropagate, string theSource)
			{
				if (thePropagate)
				{
					QueueTileUltimates(p);
				}
			}
		};
		return sSpec;
	}

	private static Plant FindUltimatePlantAt(Board b, int x, int y)
	{
		foreach (Plant p in b.mPlants)
		{
			if (!p.mDead && !p.mSquished && p.mPlantCol == x && p.mRow == y && p.mSeedType != Seed && Tavel.CanUseUltimate(p))
			{
				return p;
			}
		}
		return null;
	}

	private static void QueueTileUltimates(Plant source)
	{
		Board b = source.mBoard;
		if (b?.mTavel == null)
		{
			return;
		}
		TavelBoard bs = b.mTavel;
		(int, int) origin = (source.mPlantCol, source.mRow);
		if (!bs.CzlbTiles.Contains(origin))
		{
			return;
		}
		HashSet<Plant> queued = new HashSet<Plant>(bs.TileQueue.Select(((Plant Plant, int Due) q) => q.Plant));
		foreach ((int x, int y) in bs.CzlbTiles.ToArray())
		{
			if ((x, y) == origin)
			{
				continue;
			}
			Plant target = FindUltimatePlantAt(b, x, y);
			if (target == null || queued.Contains(target))
			{
				continue;
			}
			// the linked ultimates travel across the tiles, half a second per cell
			int distance = Math.Abs(x - origin.Item1) + Math.Abs(y - origin.Item2);
			bs.TileQueue.Add((target, b.mMainCounter + distance * TileChainDelayPerGrid));
		}
	}

	private static void UpdateTileUltimates(Board b)
	{
		if (b.mTavel == null || b.mTavel.TileQueue.Count == 0)
		{
			return;
		}
		List<(Plant, int)> remaining = new List<(Plant, int)>();
		foreach ((Plant p, int due) in b.mTavel.TileQueue.ToArray())
		{
			if (p == null || p.mDead || p.mBoard != b)
			{
				continue;
			}
			if (b.mMainCounter < due)
			{
				remaining.Add((p, due));
			}
			else if (Tavel.CanUseUltimate(p))
			{
				Tavel.StartUltimate(p, thePropagateTiles: false, "tile");
			}
			else
			{
				// only a running ultimate cannot be interrupted: wait for it
				remaining.Add((p, due));
			}
		}
		b.mTavel.TileQueue = remaining;
	}

	private static string SaveBoard(Board b)
	{
		TavelBoard bs = Tavel.State(b);
		return bs.CzlbPlantedCount + "|" + string.Join(";", bs.CzlbTiles.OrderBy(((int, int) c) => c).Select(((int, int) c) => c.Item1 + "," + c.Item2));
	}

	private static void LoadBoard(Board b, string theText)
	{
		string[] values = (theText ?? "").Split('|');
		if (values.Length < 2)
		{
			return;
		}
		TavelBoard bs = Tavel.State(b);
		int.TryParse(values[0], out bs.CzlbPlantedCount);
		bs.CzlbTiles.Clear();
		foreach (string cell in values[1].Split(';'))
		{
			string[] pair = cell.Split(',');
			if (pair.Length == 2 && int.TryParse(pair[0], out int x) && int.TryParse(pair[1], out int y))
			{
				bs.CzlbTiles.Add((x, y));
			}
		}
		bs.CzlbReserved.Clear();
		foreach (Plant p in b.mPlants)
		{
			if (!p.mDead && p.mSeedType == Seed)
			{
				bs.CzlbReserved.Add((p.mPlantCol, p.mRow));
			}
		}
	}

	private static void ReleaseCell(Plant p)
	{
		p.mBoard?.mTavel?.CzlbReserved.Remove((p.mPlantCol, p.mRow));
	}

	private static void DropState(Plant p)
	{
		if (p.mTavel is State)
		{
			p.mTavel = null;
		}
		ReleaseCell(p);
	}

	private static void Update(Plant p)
	{
		State s = Get(p);
		bool done = Tavel.Body(p) == null || Tavel.AnimTime(p) >= 0.999f;
		if (s.Mode == "stand")
		{
			if (done)
			{
				s.Mode = "attack";
				Play(p, "attack", ReanimLoopType.PlayOnceAndHold);
			}
			return;
		}
		if (s.Mode == "attack" && done)
		{
			s.Mode = "complete";
			if (!s.TileCreated && p.mBoard != null)
			{
				TavelBoard bs = Tavel.State(p.mBoard);
				bs.CzlbReserved.Remove((p.mPlantCol, p.mRow));
				bs.CzlbTiles.Add((p.mPlantCol, p.mRow));
				s.TileCreated = true;
				p.Die();
			}
		}
	}

	/// <summary>Board.DrawBackdrop: tiles lie on the lawn, under everything standing on it.</summary>
	public static void DrawTiles(Board b, Graphics g)
	{
		if (b.mTavel == null || b.mTavel.CzlbTiles.Count == 0)
		{
			return;
		}
		Image img = Tavel.LoadImage("tavel/czlb/tile.png");
		if (img == null)
		{
			return;
		}
		foreach ((int x, int y) in b.mTavel.CzlbTiles)
		{
			// a lawn cell is 80 x 100, roof rows included
			g.DrawImage(img, b.GridToPixelX(x, y), b.GridToPixelY(x, y), 80, 100);
		}
	}
}
