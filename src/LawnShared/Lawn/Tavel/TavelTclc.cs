// 贪吃龙草 (plant_tclc.py): drawn from sprite sheets. Snacks and bites fill her energy; full, she flies up and
// breathes fire; afterwards a fire clone shoots homing fireballs. The ultimate swallows up to three zombies in the row.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public sealed partial class TavelBoard
{
	public List<TavelTclc.Fireball> TclcFireballs = new List<TavelTclc.Fireball>();
}

public sealed partial class TavelZombie
{
	public bool TclcBurning;

	public int TclcBurnRemaining;

	public int TclcBurnNextTick;
}

public static class TavelTclc
{
	public const int Id = 111;

	public static SeedType Seed => (SeedType)Id;

	private const int Tps = Tavel.TicksPerSecond;

	public const int Cost = 200;

	public const int RefreshTime = 500;

	public const int Health = 300;

	public const int IdleFps = 12;

	public const float Scale = 0.42f;

	public const float FeetX = 40f;

	public const float FeetY = 70f;

	public const float EnergyMax = 100f;

	public const float EnergyStart = 50f;

	public const float EnergyPerSnack = 5f;

	public const float EnergyPerSwallow = 20f;

	public const int IdleLoopTicks = 500;

	public const int SnackTick = 200;

	public const int SaFps = 12;

	private static readonly (int, int) SaWindup = (0, 4);

	private static readonly (int, int) SaInhale = (6, 27);

	private static readonly (int, int) SaGulp = (29, 40);

	private static readonly (int, int) SaBlow = (41, 53);

	private static readonly (int, int) SaRecover = (54, 59);

	public const int SaSwallowFrame = 30;

	public const int UltMaxSwallow = 3;

	public const int UltGiantDamage = 1800;

	public const int UltPlantChance = 50;

	public const float UltPullSpeed = 6f;

	public const float UltBlowSpeed = 10f;

	public const int UltMinInhale = 90;

	public const int UltMaxInhale = 300;

	public const int WindStreaks = 9;

	public const float MouthDX = 47f;

	public const float MouthDY = -69f;

	public const int BiteFps = 15;

	public const int BiteFrames = 15;

	public const int BiteHitFrame = 8;

	public const int BiteInterval = 200;

	public const int BiteDamage = 60;

	public const float EnergyPerBite = 10f;

	private static readonly (int, int) FireWindup = (0, 16);

	private static readonly (int, int) FireHold = (18, 49);

	private static readonly (int, int) FireRecover = (50, 59);

	public const int FireWindupFps = 18;

	public const float FireAltitude = 46f;

	public const float FireDrainPerTick = 10f * 0.7f / Tps;

	public const int FireDamage = 60;

	public const int FireDamageInterval = 50;

	public const int FireBlueAfter = 300;

	public const int FirePurpleAfter = 600;

	public const float FireReach = 280f;

	public const int FireNoTargetGrace = 60;

	public const float FireMouthDX = 40f;

	public const float FireMouthDY = -56f;

	public const float CloneDurationRatio = 0.5f;

	public const int CloneMinDuration = 200;

	public const float CloneHeight = 92f;

	public const int CloneShotInterval = 100;

	public const float FireballSpeed = 5f;

	public const int FireballDamage = 40;

	public const int BurnDuration = 300;

	public const int BurnInterval = 50;

	public const int BurnDamage = 10;

	public const int FlameCell = 336;

	public const int FlameDrawSize = 230;

	public const int FlameColumns = 6;

	public const int FlameGrowFrames = 12;

	public const int FlameLoopFrames = 25;

	// idle.png: 160x244 cells, feet at x=88 with the soles at y=237; the action sheets line up with it
	private static readonly TavelSpriteSheet IdleSheet = new TavelSpriteSheet("tavel/tclc/idle.png", 10, 160, 244, 60, 88f, 237f);

	private static readonly TavelSpriteSheet SaSheet = new TavelSpriteSheet("tavel/tclc/sa.png", 10, 272, 248, 60, 110f, 245f);

	private static readonly TavelSpriteSheet FireSheet = new TavelSpriteSheet("tavel/tclc/fire.png", 10, 272, 248, 60, 110f, 245f);

	private static readonly TavelSpriteSheet BiteSheet = new TavelSpriteSheet("tavel/tclc/bite.png", 8, 272, 248, 15, 110f, 245f);

	private static readonly TavelSpriteSheet CloneSheet = new TavelSpriteSheet("tavel/tclc/clone.png", 10, 204, 186, 20, 82.5f, 183.75f);

	public sealed class Pull
	{
		public Zombie Zombie;

		public float X;

		public float StartX;

		public float StartY;
	}

	public sealed class Clone
	{
		public int Start;

		public int Until;

		public int NextShot;
	}

	public sealed class Fireball
	{
		public float X;

		public float Y;

		public Zombie Target;

		public int Born;
	}

	public sealed class State
	{
		public int IdleStart;

		public int Snacks;

		public float Energy = EnergyStart;

		public string Mode = "idle";

		public string Phase;

		public int PhaseStart;

		public int Frame;

		public int BiteReadyAt;

		public bool Bit;

		public float Altitude;

		public int BurnTicks;

		public int NextFireDamage;

		public int LastFireTarget;

		public Clone Clone;

		public bool Swallowed;

		public List<Pull> UltimateTargets = new List<Pull>();

		public List<Zombie> Giants = new List<Zombie>();

		public bool RollPlantUltimate;
	}

	public static State Get(Plant p)
	{
		if (p.mTavel is State s)
		{
			return s;
		}
		s = new State { IdleStart = p.mBoard?.mMainCounter ?? 0 };
		p.mTavel = s;
		return s;
	}

	public static TavelSpec Spec()
	{
		return new TavelSpec(Id, "tclc")
		{
			Name = "贪吃龙草",
			Cost = Cost,
			RefreshTime = RefreshTime,
			Card = "tavel/tclc/card.png",
			AlmanacText = "贪吃龙草靠吃零食和啃咬僵尸积攒能量，然后消耗能量喷出火焰。\n{SHORTLINE}\n{KEYWORD}范围：{STAT}前方1格或前方3×3\n{KEYWORD}特点：{STAT}吃零食或啃咬僵尸积攒能量，能量满后喷火\n{KEYWORD}大招：{STAT}吞下本行最多3只中小型僵尸并回复能量，重创大型僵尸并把它们吹回最右侧\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}短\n{KEYWORD}阳光：{STAT}200\n{SHORTLINE}\n{FLAVOR}作为龙草家族的老五，贪吃龙草有着惊人的大胃口，别说是零食，就算是一大堆僵尸也不在话下。",
			DefName = "TCLC",
			DefCost = Cost,
			DefRefresh = RefreshTime,
			DrawSeed = delegate(Graphics g, float x, float y)
			{
				DrawSprite(g, PreviewFrame(), x + FeetX * g.mScaleX, y + FeetY * g.mScaleY, g.mScaleX, g.mScaleY);
			},
			DrawCursor = delegate(Graphics g)
			{
				DrawSprite(g, PreviewFrame(), FeetX, FeetY);
			},
			Draw = DrawPlant,
			DrawSquished = delegate(Plant p, Graphics g)
			{
				DrawSprite(g, 0, FeetX, FeetY + TavelSpriteSheet.SquishedDrop, 1f, TavelSpriteSheet.SquishedHeight);
			},
			OnPlanted = delegate(Plant p, int x, int y)
			{
				p.mTavel = null;
				State s = Get(p);
				// a fresh planting may cast the ultimate for free
				s.RollPlantUltimate = p.mBoard != null;
				p.mPlantHealth = Health;
				p.mPlantMaxHealth = Health;
				if (p.mBoard != null)
				{
					Tavel.StartPacketCooldown(p.mBoard, Seed);
				}
			},
			OnUpdate = delegate(Plant p)
			{
				if (p.mDead || p.mSquished)
				{
					Cleanup(p);
					return;
				}
				if (p.mBoard != null)
				{
					Update(p);
				}
			},
			OnDie = Cleanup,
			CanUltimate = (Plant p) => Get(p).Mode != "ultimate",
			UltimateActive = (Plant p) => p.mTavel is State s && s.Mode == "ultimate",
			StartUltimate = StartUltimate,
			UpdateBoard = UpdateBoard,
			DrawBoard = DrawFireballs,
			// only her energy is kept; after loading she stands idle
			SaveState = (Plant p) => Get(p).Energy.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
			LoadState = delegate(Plant p, string t)
			{
				State s = Get(p);
				if (float.TryParse(t, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float e))
				{
					s.Energy = Math.Max(0f, Math.Min(EnergyMax, e));
				}
				s.RollPlantUltimate = false;
			}
		};
	}

	public static bool IsBurning(Zombie z) => z.mTavel != null && z.mTavel.TclcBurning;

	public static bool IsAirborne(Plant p) => p.mTavel is State s && s.Mode == "fire" && s.Altitude > FireAltitude * 0.3f;

	private static int IdleFrame(int theTicks) => theTicks * IdleFps / Tps;

	private static int PreviewFrame() => IdleFrame(GlobalStaticVars.gLawnApp.mAppCounter);

	private static void DrawSprite(Graphics g, int theFrame, float x, float y, float theUnitX = 1f, float theUnitY = 1f)
	{
		IdleSheet.Draw(g, theFrame, x, y, Scale * theUnitX, Scale * theUnitY);
	}

	private static bool IsPulled(Zombie z) => z.mTavel != null && z.mTavel.FjthPulled;

	private static void DrawWind(Plant p, Graphics g, State s)
	{
		bool inward;
		if (s.Phase == "inhale")
		{
			inward = true;
		}
		else if (s.Phase == "blow" && s.Frame > SaBlow.Item1)
		{
			inward = false;
		}
		else
		{
			return;
		}
		Image img = Tavel.LoadImage("tavel/tclc/wind.png");
		if (img == null)
		{
			return;
		}
		float mx = FeetX + MouthDX;
		float my = FeetY + MouthDY;
		float reach = Math.Max(160f, p.mBoard.mWidth - p.mX - mx);
		int ticks = p.mBoard.mMainCounter - s.PhaseStart;
		bool oldColorize = g.GetColorizeImages();
		Color oldColor = g.GetColor();
		g.SetColorizeImages(colorizeImages: true);
		try
		{
			for (int i = 0; i < WindStreaks; i++)
			{
				// each streak repeats its run every 60 ticks, offset from the rest
				float travel = (ticks + i * 60f / WindStreaks) % 60f / 60f;
				float distance = inward ? 1f - travel : travel;
				float lane = i * 3 % WindStreaks / (float)(WindStreaks - 1) - 0.5f;
				float x = mx + 10f + distance * reach;
				float y = my + 14f + lane * (16f + 60f * distance);
				int alpha = (int)(150f * Math.Min(1f, Math.Min(distance * 5f, (1f - distance) * 5f)));
				float sx = 0.4f + 0.6f * distance;
				float sy = 0.3f + 0.25f * distance;
				g.SetColor(new Color(255, 255, 255, Math.Max(0, alpha)));
				TodCommon.TodDrawImageScaledF(g, img, x - img.mWidth * sx / 2f, y - img.mHeight * sy / 2f, sx, sy);
			}
		}
		finally
		{
			g.SetColor(oldColor, premultiply: false);
			g.SetColorizeImages(oldColorize);
		}
	}

	private static void DrawEnergyBar(Graphics g, State s)
	{
		const int width = 36;
		int x = (int)FeetX - width / 2;
		int y = (int)FeetY + 6;
		Color old = g.GetColor();
		g.SetColor(new Color(30, 30, 30, 170));
		g.FillRect(x - 1, y - 1, width + 2, 6);
		int filled = (int)(width * s.Energy / EnergyMax);
		if (filled > 0)
		{
			g.SetColor(new Color(255, 150, 40, 255));
			g.FillRect(x, y, filled, 4);
		}
		g.SetColor(old, premultiply: false);
	}

	private static (string Colour, int Multiplier) FlameStage(int theBurnTicks)
	{
		if (theBurnTicks >= FirePurpleAfter)
		{
			return ("purple", 3);
		}
		if (theBurnTicks >= FireBlueAfter)
		{
			return ("blue", 2);
		}
		return ("orange", 1);
	}

	private static void DrawFlames(Graphics g, State s, float theMouthX, float theMouthY)
	{
		int burn = s.BurnTicks;
		int growTicks = FlameGrowFrames * Tps / 15;
		int frame = burn < growTicks ? burn * 15 / Tps : FlameGrowFrames + (burn - growTicks) * 12 / Tps % FlameLoopFrames;
		Image img = Tavel.LoadImage("tavel/tclc/flame_" + FlameStage(burn).Colour + ".png");
		if (img == null)
		{
			return;
		}
		TRect src = new TRect(frame % FlameColumns * FlameCell, frame / FlameColumns * FlameCell, FlameCell, FlameCell);
		// the jet leaves the cell's left edge a quarter of the way down
		g.DrawImage(img, new TRect((int)theMouthX, (int)theMouthY - FlameDrawSize / 4, FlameDrawSize, FlameDrawSize), src);
	}

	private static void DrawClone(Plant p, Graphics g, State s)
	{
		Clone c = s.Clone;
		if (c == null)
		{
			return;
		}
		int now = p.mBoard.mMainCounter;
		int frame = LoopFrame((0, CloneSheet.FrameCount - 1), now);
		int alpha = (int)(215f * Math.Min(1f, Math.Min((now - c.Start) / 50f, (c.Until - now) / 50f)));
		if (alpha <= 0)
		{
			return;
		}
		float bob = (float)Math.Sin(now * Math.PI / 60.0) * 3f;
		Tavel.Tinted(g, new Color(255, 255, 255, alpha), delegate
		{
			CloneSheet.Draw(g, frame, FeetX, FeetY - CloneHeight + bob, Scale);
		});
	}

	private static void DrawPlant(Plant p, Graphics g)
	{
		State s = Get(p);
		Image shadow = AtlasResources.IMAGE_PLANTSHADOW;
		if (shadow != null)
		{
			g.DrawImageF(shadow, FeetX - shadow.mWidth / 2f, FeetY - shadow.mHeight / 2f - 2f);
		}
		if (p.mBoard == null)
		{
			DrawSprite(g, PreviewFrame(), FeetX, FeetY);
			return;
		}
		DrawClone(p, g, s);
		switch (s.Mode)
		{
		case "ultimate":
			DrawWind(p, g, s);
			SaSheet.Draw(g, s.Frame, FeetX, FeetY, Scale);
			break;
		case "fire":
		{
			float feetY = FeetY - s.Altitude;
			FireSheet.Draw(g, s.Frame, FeetX, feetY, Scale);
			if (s.Phase == "burn")
			{
				DrawFlames(g, s, FeetX + FireMouthDX, feetY + FireMouthDY);
			}
			break;
		}
		case "bite":
			BiteSheet.Draw(g, s.Frame, FeetX, FeetY, Scale);
			break;
		default:
			DrawSprite(g, IdleFrame(p.mBoard.mMainCounter - s.IdleStart), FeetX, FeetY);
			break;
		}
		DrawEnergyBar(g, s);
	}

	private static void AddEnergy(State s, float theAmount)
	{
		s.Energy = Math.Max(0f, Math.Min(EnergyMax, s.Energy + theAmount));
	}

	/// <summary>Zombie mPosX that puts its body just in front of the puppet's mouth.</summary>
	private static float MouthTarget(Plant p) => p.mX + FeetX + MouthDX - 40f;

	private static void SetPhase(Plant p, State s, string thePhase)
	{
		s.Phase = thePhase;
		s.PhaseStart = p.mBoard.mMainCounter;
	}

	private static void StartUltimate(Plant p)
	{
		State s = Get(p);
		Board b = p.mBoard;
		List<Zombie> small = new List<Zombie>();
		List<Zombie> giants = new List<Zombie>();
		foreach (Zombie z in b.mZombies)
		{
			if (!Tavel.Alive(z) || z.mRow != p.mRow || z.mMindControlled || z.mPosX < p.mX - 40 || z.mPosX > b.mWidth || IsPulled(z) || z.mZombieType == ZombieType.Boss || z.mZombieType == ZombieType.Bungee)
			{
				continue;
			}
			(Tavel.IsGiantZombie(z) ? giants : small).Add(z);
		}
		s.UltimateTargets = small.OrderBy((Zombie z) => z.mPosX).Take(UltMaxSwallow).Select((Zombie z) => new Pull { Zombie = z, X = z.mPosX, StartX = z.mPosX, StartY = z.mPosY }).ToList();
		foreach (Pull item in s.UltimateTargets)
		{
			// shares 伏僵塔黄's "being pulled" mark: no eating, mowing or ice while dragged
			TavelHooks.ZState(item.Zombie).FjthPulled = true;
			if (item.Zombie.mIsEating)
			{
				item.Zombie.StopEating();
			}
		}
		s.Giants = giants;
		s.Swallowed = false;
		s.Mode = "ultimate";
		s.Altitude = 0f;
		s.Frame = SaWindup.Item1;
		SetPhase(p, s, "windup");
	}

	private static void ReleaseTargets(State s)
	{
		foreach (Pull item in s.UltimateTargets)
		{
			if (item.Zombie.mTavel != null)
			{
				item.Zombie.mTavel.FjthPulled = false;
			}
		}
		s.UltimateTargets = new List<Pull>();
		s.Giants = new List<Zombie>();
	}

	private static void Cleanup(Plant p)
	{
		if (p.mTavel is State s)
		{
			ReleaseTargets(s);
			p.mTavel = null;
		}
	}

	private static bool PullTargets(Plant p, State s)
	{
		float mx = MouthTarget(p);
		bool arrived = true;
		foreach (Pull item in s.UltimateTargets)
		{
			Zombie z = item.Zombie;
			if (z.mDead)
			{
				continue;
			}
			item.X = item.X > mx ? Math.Max(mx, item.X - UltPullSpeed) : Math.Min(mx, item.X + UltPullSpeed);
			if (item.X != mx)
			{
				arrived = false;
			}
			float progress = 1f - Math.Abs(item.X - mx) / Math.Max(1f, Math.Abs(item.StartX - mx));
			z.mPosX = item.X;
			z.mPosY = item.StartY - 20f * progress;
			z.mX = (int)z.mPosX;
			z.mY = (int)z.mPosY;
		}
		return arrived;
	}

	private static void Swallow(Plant p, State s)
	{
		int swallowed = 0;
		foreach (Pull item in s.UltimateTargets)
		{
			Zombie z = item.Zombie;
			if (z.mTavel != null)
			{
				z.mTavel.FjthPulled = false;
			}
			if (Tavel.Alive(z))
			{
				z.DieNoLoot(false);
				swallowed++;
			}
		}
		s.UltimateTargets = new List<Pull>();
		AddEnergy(s, EnergyPerSwallow * swallowed);
		foreach (Zombie z in s.Giants)
		{
			if (Tavel.Alive(z))
			{
				z.TakeDamage(UltGiantDamage, 0u);
			}
		}
		if (swallowed > 0)
		{
			p.mApp.PlayFoley(FoleyType.Bigchomp);
		}
		s.Swallowed = true;
	}

	private static void BlowGiants(Plant p, State s)
	{
		float lawnRight = p.mBoard.GridToPixelX(8, p.mRow) + 80;
		foreach (Zombie z in s.Giants)
		{
			if (Tavel.Alive(z) && z.mPosX < lawnRight)
			{
				z.mPosX = Math.Min(lawnRight, z.mPosX + UltBlowSpeed);
				z.mX = (int)z.mPosX;
			}
		}
	}

	/// <summary>Frame theTicks into a beat of an action sheet, or -1 once it is over.</summary>
	private static int SaFrame((int, int) theSpan, int theTicks, int theFps = SaFps)
	{
		int f = theSpan.Item1 + theTicks * theFps / Tps;
		return f <= theSpan.Item2 ? f : -1;
	}

	/// <summary>A beat played back and forth for as long as needed.</summary>
	private static int LoopFrame((int, int) theSpan, int theTicks)
	{
		int len = theSpan.Item2 - theSpan.Item1;
		int step = theTicks * SaFps / Tps % (2 * len);
		return theSpan.Item1 + (step <= len ? step : 2 * len - step);
	}

	private static void ReturnToIdle(Plant p, State s)
	{
		s.Mode = "idle";
		s.Phase = null;
		s.Altitude = 0f;
		s.IdleStart = p.mBoard.mMainCounter;
		s.Snacks = 0;
	}

	private static bool CanTarget(Zombie z)
	{
		return Tavel.Alive(z) && !z.mMindControlled && !IsPulled(z) && (!Tavel.IsBoss(z) || Tavel.BossCanBeHit(z));
	}

	private static List<Zombie> ZombiesBetween(Plant p, int[] theRows, float theLeft, float theRight)
	{
		List<Zombie> found = new List<Zombie>();
		foreach (Zombie z in p.mBoard.mZombies)
		{
			if (!theRows.Any((int r) => Tavel.ZombieIsInRow(z, r)) || !CanTarget(z))
			{
				continue;
			}
			TRect r2 = z.GetZombieRect();
			if (r2.mX < theRight && r2.mX + r2.mWidth > theLeft)
			{
				found.Add(z);
			}
		}
		return found;
	}

	private static List<Zombie> BiteTargets(Plant p) => ZombiesBetween(p, new[] { p.mRow }, p.mX + 30f, p.mX + 140f);

	private static List<Zombie> FireTargets(Plant p)
	{
		float left = p.mX + FeetX + FireMouthDX;
		return ZombiesBetween(p, new[] { p.mRow - 1, p.mRow, p.mRow + 1 }, left, left + FireReach);
	}

	private static void UpdateBite(Plant p, State s)
	{
		int ticks = p.mBoard.mMainCounter - s.PhaseStart;
		int frame = ticks * BiteFps / Tps;
		if (!s.Bit && frame >= BiteHitFrame)
		{
			s.Bit = true;
			List<Zombie> targets = BiteTargets(p);
			if (targets.Count > 0)
			{
				// one zombie per bite: the closest
				Zombie target = targets.OrderBy((Zombie z) => z.mPosX).First();
				target.TakeDamage(BiteDamage, 0u);
				AddEnergy(s, EnergyPerBite);
				p.mApp.PlayFoley(FoleyType.Chomp);
			}
		}
		if (frame >= BiteFrames)
		{
			ReturnToIdle(p, s);
			return;
		}
		s.Frame = frame;
	}

	private static void UpdateFire(Plant p, State s)
	{
		int now = p.mBoard.mMainCounter;
		int ticks = now - s.PhaseStart;
		if (s.Phase == "takeoff")
		{
			int frame = SaFrame(FireWindup, ticks, FireWindupFps);
			if (frame < 0)
			{
				s.Altitude = FireAltitude;
				s.NextFireDamage = now;
				s.LastFireTarget = now;
				s.Frame = FireHold.Item1;
				SetPhase(p, s, "burn");
				p.mApp.PlayFoley(FoleyType.Ignite);
				return;
			}
			int span = FireWindup.Item2 - FireWindup.Item1 + 1;
			s.Altitude = FireAltitude * (frame - FireWindup.Item1) / span;
			s.Frame = frame;
		}
		else if (s.Phase == "burn")
		{
			s.BurnTicks = ticks;
			s.Frame = LoopFrame(FireHold, ticks);
			s.Altitude = FireAltitude + (float)Math.Sin(ticks * Math.PI / 40.0) * 3f;
			AddEnergy(s, 0f - FireDrainPerTick);
			if (now >= s.NextFireDamage)
			{
				s.NextFireDamage = now + FireDamageInterval;
				List<Zombie> targets = FireTargets(p);
				if (targets.Count > 0)
				{
					s.LastFireTarget = now;
					int damage = FireDamage * FlameStage(ticks).Multiplier;
					foreach (Zombie z in targets)
					{
						z.TakeDamage(damage, 0u);
					}
				}
			}
			if (s.Energy <= 0f || now - s.LastFireTarget >= FireNoTargetGrace)
			{
				int duration = Math.Max(CloneMinDuration, (int)(s.BurnTicks * CloneDurationRatio));
				s.Clone = new Clone { Start = now, Until = now + duration, NextShot = now + CloneShotInterval / 2 };
				s.Frame = FireRecover.Item1;
				SetPhase(p, s, "land");
			}
		}
		else
		{
			int frame = SaFrame(FireRecover, ticks);
			if (frame < 0)
			{
				ReturnToIdle(p, s);
				return;
			}
			int span = FireRecover.Item2 - FireRecover.Item1 + 1;
			s.Altitude = FireAltitude * (1f - (frame - FireRecover.Item1 + 1) / (float)span);
			s.Frame = frame;
		}
	}

	private static void UpdateClone(Plant p, State s)
	{
		Clone c = s.Clone;
		if (c == null)
		{
			return;
		}
		int now = p.mBoard.mMainCounter;
		if (now >= c.Until)
		{
			s.Clone = null;
			return;
		}
		if (now < c.NextShot)
		{
			return;
		}
		c.NextShot = now + CloneShotInterval;
		float x = p.mX + FeetX + FireMouthDX * 0.8f;
		float y = p.mY + FeetY - CloneHeight + FireMouthDY * 0.8f;
		Zombie target = FireballTarget(p.mBoard, x, y);
		if (target != null)
		{
			Tavel.State(p.mBoard).TclcFireballs.Add(new Fireball { X = x, Y = y, Target = target, Born = now });
			p.mApp.PlayFoley(FoleyType.Firepea);
		}
	}

	/// <summary>The nearest zombie on the lawn that is not behind the shooter.</summary>
	private static Zombie FireballTarget(Board b, float x, float y)
	{
		Zombie best = null;
		double bestD = 0;
		foreach (Zombie z in b.mZombies)
		{
			if (!CanTarget(z))
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			float cx = r.mX + r.mWidth / 2f;
			if (cx < x - 40f || cx > b.mWidth)
			{
				continue;
			}
			double d = Math.Sqrt((cx - x) * (cx - x) + (r.mY + r.mHeight / 2f - y) * (r.mY + r.mHeight / 2f - y));
			if (best == null || d < bestD)
			{
				best = z;
				bestD = d;
			}
		}
		return best;
	}

	/// <summary>Fireballs in flight and the burns they leave; once per tick.</summary>
	private static void UpdateBoard(Board b)
	{
		if (b.mTavel == null)
		{
			return;
		}
		List<Fireball> flying = new List<Fireball>();
		foreach (Fireball f in b.mTavel.TclcFireballs)
		{
			Zombie target = f.Target;
			if (!CanTarget(target))
			{
				target = FireballTarget(b, f.X, f.Y);
				if (target == null)
				{
					continue;
				}
				f.Target = target;
			}
			TRect r = target.GetZombieRect();
			float dx = r.mX + r.mWidth / 2f - f.X;
			float dy = r.mY + r.mHeight / 2f - f.Y;
			float d = (float)Math.Sqrt(dx * dx + dy * dy);
			if (d <= FireballSpeed + 8f)
			{
				target.TakeDamage(FireballDamage, 0u);
				TavelZombie t = TavelHooks.ZState(target);
				t.TclcBurning = true;
				t.TclcBurnRemaining = BurnDuration;
				t.TclcBurnNextTick = BurnInterval;
				continue;
			}
			f.X += dx / d * FireballSpeed;
			f.Y += dy / d * FireballSpeed;
			flying.Add(f);
		}
		b.mTavel.TclcFireballs = flying;
		foreach (Zombie z in b.mZombies.ToArray())
		{
			TavelZombie t = z.mTavel;
			if (t == null || !t.TclcBurning)
			{
				continue;
			}
			if (!Tavel.Alive(z))
			{
				t.TclcBurning = false;
				continue;
			}
			t.TclcBurnRemaining--;
			t.TclcBurnNextTick--;
			if (t.TclcBurnNextTick <= 0)
			{
				t.TclcBurnNextTick = BurnInterval;
				z.TakeDamage(BurnDamage, 0u);
			}
			if (t.TclcBurnRemaining <= 0)
			{
				t.TclcBurning = false;
			}
		}
	}

	private static void DrawFireballs(Board b, Graphics g)
	{
		if (b.mTavel == null || b.mTavel.TclcFireballs.Count == 0)
		{
			return;
		}
		Image img = Tavel.LoadImage("tavel/tclc/fireball.png");
		if (img == null)
		{
			return;
		}
		foreach (Fireball f in b.mTavel.TclcFireballs)
		{
			float k = 0.8f + 0.15f * (float)Math.Sin((b.mMainCounter - f.Born) * 0.5);
			TodCommon.TodDrawImageScaledF(g, img, f.X - img.mWidth * k / 2f, f.Y - img.mHeight * k / 2f, k, k);
		}
	}

	private static void UpdateUltimate(Plant p, State s)
	{
		int ticks = p.mBoard.mMainCounter - s.PhaseStart;
		switch (s.Phase)
		{
		case "windup":
		{
			int frame = SaFrame(SaWindup, ticks);
			if (frame < 0)
			{
				SetPhase(p, s, "inhale");
				frame = SaInhale.Item1;
			}
			s.Frame = frame;
			break;
		}
		case "inhale":
		{
			bool arrived = PullTargets(p, s);
			s.Frame = LoopFrame(SaInhale, ticks);
			if ((arrived && ticks >= UltMinInhale) || ticks >= UltMaxInhale)
			{
				SetPhase(p, s, "gulp");
			}
			break;
		}
		case "gulp":
		{
			int frame = SaFrame(SaGulp, ticks);
			if (!s.Swallowed)
			{
				if (frame < 0 || frame >= SaSwallowFrame)
				{
					Swallow(p, s);
				}
				else
				{
					PullTargets(p, s);
				}
			}
			if (frame < 0)
			{
				SetPhase(p, s, "blow");
				frame = SaBlow.Item1;
			}
			s.Frame = frame;
			break;
		}
		case "blow":
		{
			int frame = SaFrame(SaBlow, ticks);
			if (frame < 0)
			{
				SetPhase(p, s, "recover");
				frame = SaRecover.Item1;
			}
			else
			{
				BlowGiants(p, s);
			}
			s.Frame = frame;
			break;
		}
		default:
		{
			int frame = SaFrame(SaRecover, ticks);
			if (frame < 0)
			{
				ReleaseTargets(s);
				ReturnToIdle(p, s);
				return;
			}
			s.Frame = frame;
			break;
		}
		}
	}

	private static void Update(Plant p)
	{
		State s = Get(p);
		if (s.RollPlantUltimate)
		{
			s.RollPlantUltimate = false;
			if (s.Mode == "idle" && Tavel.UltimateUiAllowed(p.mBoard) && Tavel.Rand(0, 99) < UltPlantChance)
			{
				StartUltimate(p);
			}
		}
		UpdateClone(p, s);
		switch (s.Mode)
		{
		case "ultimate":
			UpdateUltimate(p, s);
			return;
		case "fire":
			UpdateFire(p, s);
			return;
		case "bite":
			UpdateBite(p, s);
			return;
		}
		if (s.Energy >= EnergyMax && FireTargets(p).Count > 0)
		{
			s.Mode = "fire";
			s.Frame = FireWindup.Item1;
			s.BurnTicks = 0;
			SetPhase(p, s, "takeoff");
			return;
		}
		if (p.mBoard.mMainCounter >= s.BiteReadyAt && BiteTargets(p).Count > 0)
		{
			s.Mode = "bite";
			s.Bit = false;
			s.Frame = 0;
			SetPhase(p, s, null);
			s.BiteReadyAt = p.mBoard.mMainCounter + BiteInterval;
			return;
		}
		// one snack per idle loop, counted when she takes the bite
		int elapsed = p.mBoard.mMainCounter - s.IdleStart;
		int snacks = (elapsed + IdleLoopTicks - SnackTick) / IdleLoopTicks;
		if (snacks > s.Snacks)
		{
			AddEnergy(s, EnergyPerSnack * (snacks - s.Snacks));
			s.Snacks = snacks;
		}
	}
}
