// 暗物质火龙果 (plant_dmdf.py): a mage drawn from sprite sheets. Dark-matter shots deal cosmic damage, put zombies
// into the quantum state (straight projectiles pass through them and still hit) and burst behind the target; some are
// collapse bombs. The ultimate summons a dragon left of the lawn that fires at three random zombies.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public sealed partial class TavelBoard
{
	public List<TavelDmdf.Shot> DmdfShots = new List<TavelDmdf.Shot>();

	public List<TavelDmdf.BigShot> DmdfBigShots = new List<TavelDmdf.BigShot>();

	public List<TavelDmdf.Dragon> DmdfDragons = new List<TavelDmdf.Dragon>();

	public List<TavelDmdf.Flame> DmdfFlames = new List<TavelDmdf.Flame>();

	public List<TavelDmdf.Effect> DmdfEffects = new List<TavelDmdf.Effect>();
}

public sealed partial class TavelShot
{
	/// <summary>Quantum zombies this projectile has passed through.</summary>
	public List<Zombie> DmdfPierced;
}

public sealed partial class TavelZombie
{
	/// <summary>Board tick the quantum state ends (0: never had it).</summary>
	public int DmdfQuantumUntil;
}

public static class TavelDmdf
{
	public const int Id = 113;

	public static SeedType Seed => (SeedType)Id;

	private const int Tps = Tavel.TicksPerSecond;

	public const int Cost = 200;

	public const int RefreshTime = 1500;

	public const int Health = 300;

	public const float FeetX = 46f;

	public const float FeetY = 72f;

	public const float Height = 112f;

	public const float ActionUnit = Height / 422f;

	public const float IdleUnit = Height / 784f;

	private static readonly TavelSpriteSheet IdleSheet = new TavelSpriteSheet("tavel/dmdf/idle.png", 10, 212, 260, 60, 150.96f, 257.04f);

	public const float IdleScale = IdleUnit / 0.32f;

	public const int IdleFps = 12;

	private static readonly TavelSpriteSheet AttackSheet = new TavelSpriteSheet("tavel/dmdf/attack.png", 8, 187, 268, 32, 103.1f, 263.6f);

	public const float AttackScale = ActionUnit / 0.6f;

	public const int AttackFps = 24;

	public const int AttackOrbFrame = 2;

	public const int AttackHitFrame = 16;

	private static readonly TavelSpriteSheet SaSheet = new TavelSpriteSheet("tavel/dmdf/sa.png", 10, 319, 309, 60, 120.25f, 293f);

	public const float SaScale = ActionUnit / 0.5f;

	public const int SaFps = 15;

	public const int SaSummonFrame = 25;

	public const int AttackInterval = 240;

	public const int Damage = 80;

	public const float ShotSpeed = 3.7f;

	public const float ShotStartDX = 38f;

	public const float ShotStartDY = -82f;

	public const float ShotFlyDY = -52f;

	public const float ShotHalfWidth = 14f;

	public const int CollapseChance = 30;

	public const float CollapseCell = 45f;

	public const float BurstNear = -15f;

	public const float BurstFar = 115f;

	public const float BurstSideNear = 25f;

	public const int QuantumDuration = 1000;

	public const int QuantumCastSlowdown = 3;

	public const uint DamageRange = 1u;

	public const int UltPlantChance = 50;

	public const int UltShots = 3;

	public const int UltImpactDamage = 1200;

	public const float BurnHalfWidth = 160f;

	public const int BurnDuration = 300;

	public const int BurnInterval = 50;

	public const int BurnDamage = 100;

	public const float BigShotSpeed = 6f;

	public const float BigShotScale = 0.42f;

	private static readonly TavelSpriteSheet DragonSheet = new TavelSpriteSheet("tavel/dmdf/dragon.png", 6, 658, 424, 35, 70f, 330f);

	public const float DragonScale = 300f / 654f;

	public const int DragonFps = 15;

	private static readonly int[] DragonIntro = Enumerable.Range(0, 11).ToArray();

	private static readonly int[] DragonRoar = Enumerable.Range(11, 14).Concat(Enumerable.Range(12, 12).Reverse()).ToArray();

	private static readonly int[] DragonOutro = Enumerable.Range(25, 10).ToArray();

	private static readonly int DragonIntroTicks = DragonIntro.Length * Tps / DragonFps;

	private static readonly int DragonOutroTicks = DragonOutro.Length * Tps / DragonFps;

	public const int DragonDelay = 20;

	public const int DragonFade = 35;

	public const int DragonFireGap = 35;

	public const int DragonLinger = 30;

	public const float PortalDX = -75f;

	public const float PortalDY = -135f;

	public const float MouthDX = 238f;

	public const float MouthDY = -62f;

	private static readonly TavelSpriteSheet BallSheet = new TavelSpriteSheet("tavel/dmdf/ball.png", 4, 112, 72, 8, 78f, 36f);

	private static readonly TavelSpriteSheet CollapseSheet = new TavelSpriteSheet("tavel/dmdf/collapse.png", 4, 96, 96, 8, 48f, 48f);

	private static readonly TavelSpriteSheet BlastSheet = new TavelSpriteSheet("tavel/dmdf/blast.png", 5, 224, 224, 10, 112f, 112f);

	private static readonly TavelSpriteSheet HoleSheet = new TavelSpriteSheet("tavel/dmdf/hole.png", 4, 160, 160, 12, 80f, 80f);

	private static readonly TavelSpriteSheet PortalSheet = new TavelSpriteSheet("tavel/dmdf/portal.png", 4, 200, 320, 8, 100f, 160f);

	private static readonly TavelSpriteSheet RingSheet = new TavelSpriteSheet("tavel/dmdf/ring.png", 4, 176, 60, 12, 88f, 30f);

	private static readonly TavelSpriteSheet OrbSheet = new TavelSpriteSheet("tavel/dmdf/orb.png", 3, 64, 64, 6, 32f, 32f);

	private static readonly TavelSpriteSheet BigShotSheet = new TavelSpriteSheet("tavel/dmdf/bigshot.png", 4, 320, 220, 8, 214f, 110f);

	private static readonly TavelSpriteSheet FlameSheet = new TavelSpriteSheet("tavel/dmdf/flame.png", 4, 160, 64, 12, 0f, 60f);

	public const float BallScale = 0.55f;

	public const float CollapseScale = 0.6f;

	public const float RingScale = 0.4f;

	public const float RingDX = -6f;

	public const float RingDY = -48f;

	public const float OrbDX = 24f;

	public const float OrbDY = -70f;

	public const int EffectFps = 20;

	private static readonly Color QuantumTint = new Color(225, 190, 255, 205);

	/// <summary>The projectile whose impact must not end it (passing through a quantum zombie).</summary>
	private static Projectile sPiercing;

	public sealed class Shot
	{
		public int Row;

		public float X;

		public float Y;

		public float FlyY;

		public bool Collapse;

		public HashSet<Zombie> Hit = new HashSet<Zombie>();

		public int Born;
	}

	public sealed class BigShot
	{
		public float X;

		public float Y;

		public Zombie Target;

		public Vector2 Goal;

		public int Row;

		public int Born;
	}

	public sealed class Dragon
	{
		public float X;

		public float Y;

		public int Start;

		public int Shots;

		public int Ready;

		public int NextFire;

		public int LastFire;

		public int Closing = -1;

		public List<Zombie> Picked = new List<Zombie>();
	}

	public sealed class Flame
	{
		public int Row;

		public float X;

		public int Start;

		public int Until;

		public int NextHit;
	}

	public sealed class Effect
	{
		public bool Hole;

		public float X;

		public float Y;

		public float Scale;

		public int Start;
	}

	public sealed class State
	{
		public int IdleStart;

		public string Mode = "idle";

		public int ModeStart;

		public int Frame;

		public bool Acted;

		public bool Collapse;

		public bool Guaranteed;

		public int NextAttack;

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
		return new TavelSpec(Id, "dmdf")
		{
			Name = "暗物质火龙果",
			Cost = Cost,
			RefreshTime = RefreshTime,
			Card = "tavel/dmdf/card.png",
			AlmanacText = "暗物质火龙果施放暗物质弹，对命中的僵尸造成宇宙伤害并施加量子状态。\n{SHORTLINE}\n{KEYWORD}范围：{STAT}整行\n{KEYWORD}特点：{STAT}命中后在目标身后产生扇形爆炸；所有平射子弹可以穿透量子状态的僵尸并照常命中；被宇宙伤害击杀的僵尸直接湮灭；有概率施放坍缩弹，湮灭首个命中格内的僵尸（包括巨人，僵王除外）\n{KEYWORD}大招：{STAT}在草坪左侧召唤巨龙，向随机三只僵尸各吐出一发暗物质弹，命中后留下3×4的灼烧区域；之后的下一次攻击必定是坍缩弹\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}慢\n{KEYWORD}阳光：{STAT}200\n{SHORTLINE}\n{FLAVOR}他们的祖先被追杀至太空深处。如今，后代归来，只为一场冰冷的、水果味的复仇。",
			DefName = "DMDF",
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
					p.mTavel = null;
					return;
				}
				if (p.mBoard != null)
				{
					Update(p);
				}
			},
			// shots and a dragon already out carry on without her
			OnDie = delegate(Plant p)
			{
				p.mTavel = null;
			},
			CanUltimate = (Plant p) => Get(p).Mode != "ultimate",
			UltimateActive = (Plant p) => p.mTavel is State s && s.Mode == "ultimate",
			StartUltimate = delegate(Plant p)
			{
				StartAction(p, Get(p), "ultimate");
			},
			UpdateBoard = UpdateBoard,
			DrawBoard = DrawBoard,
			// only the promised collapse bomb is kept across a save
			SaveState = (Plant p) => Get(p).Guaranteed ? "1" : "0",
			LoadState = delegate(Plant p, string t)
			{
				State s = Get(p);
				s.Guaranteed = t.Trim() == "1";
				s.RollPlantUltimate = false;
			}
		};
	}

	// ------------------------------------------------------------------ drawing her

	private static int IdleFrame(int theTicks) => theTicks * IdleFps / Tps;

	private static int PreviewFrame() => IdleFrame(GlobalStaticVars.gLawnApp.mAppCounter);

	private static void DrawSprite(Graphics g, int theFrame, float x, float y, float theUnitX = 1f, float theUnitY = 1f)
	{
		IdleSheet.Draw(g, theFrame, x, y, IdleScale * theUnitX, IdleScale * theUnitY);
	}

	/// <summary>Half of her hip ring: the far half behind her, the near half in front.</summary>
	private static void DrawRingHalf(Graphics g, int theTicks, bool theFront)
	{
		TavelSpriteSheet sh = RingSheet;
		Image img = sh.Image;
		if (img == null)
		{
			return;
		}
		int i = theTicks * 12 / Tps % sh.FrameCount;
		int half = sh.FrameHeight / 2;
		TRect src = new TRect(i % sh.Columns * sh.FrameWidth, i / sh.Columns * sh.FrameHeight + (theFront ? half : 0), sh.FrameWidth, half);
		float left = FeetX + RingDX - sh.AnchorX * RingScale;
		float middle = FeetY + RingDY;
		float top = theFront ? middle : middle - half * RingScale;
		g.DrawImage(img, new TRect((int)Math.Round(left), (int)Math.Round(top), (int)Math.Round(sh.FrameWidth * RingScale), (int)Math.Round(half * RingScale)), src);
	}

	private static void DrawPlant(Plant p, Graphics g)
	{
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
		State s = Get(p);
		int now = p.mBoard.mMainCounter;
		if (s.Mode == "ultimate")
		{
			// she leans far forward here: no ring
			SaSheet.Draw(g, s.Frame, FeetX, FeetY, SaScale);
			return;
		}
		DrawRingHalf(g, now, theFront: false);
		if (s.Mode == "attack")
		{
			int frame = s.Frame;
			AttackSheet.Draw(g, frame, FeetX, FeetY, AttackScale);
			DrawRingHalf(g, now, theFront: true);
			if (AttackOrbFrame <= frame && frame < AttackHitFrame)
			{
				// the shot gathers before her chest until she pushes it out
				float grown = (frame - AttackOrbFrame + 1f) / (AttackHitFrame - AttackOrbFrame);
				if (s.Collapse)
				{
					CollapseSheet.Draw(g, now * 12 / Tps, FeetX + OrbDX, FeetY + OrbDY, 0.1f + 0.2f * grown);
				}
				else
				{
					OrbSheet.Draw(g, now * 12 / Tps, FeetX + OrbDX, FeetY + OrbDY, 0.12f + 0.2f * grown);
				}
			}
			return;
		}
		DrawSprite(g, IdleFrame(now - s.IdleStart), FeetX, FeetY);
		DrawRingHalf(g, now, theFront: true);
	}

	// ------------------------------------------------------------------ zombies

	private static bool CanHit(Zombie z) => Tavel.Alive(z) && !z.mMindControlled && z.EffectedByDamage(DamageRange);

	private static (float Left, float Right) LawnEdges(Board b, int theRow) => (b.GridToPixelX(0, theRow), b.GridToPixelX(8, theRow) + 80);

	private static int LastRow(Board b) => b.StageHas6Rows() ? 5 : 4;

	private static float RowCentreY(Board b, float x, int theRow)
	{
		int col = Math.Max(0, Math.Min(8, b.PixelToGridXKeepOnBoard((int)x, 100)));
		return b.GridToPixelY(col, theRow) + 55f;
	}

	private static Vector2 Centre(Zombie z)
	{
		TRect r = z.GetZombieRect();
		return new Vector2(r.mX + r.mWidth / 2f, r.mY + r.mHeight / 2f);
	}

	/// <summary>The row a zombie is in; for Dr. Zomboss the row his head is over.</summary>
	private static int ZombieRow(Board b, Zombie z)
	{
		if (!Tavel.IsBoss(z))
		{
			return Math.Max(0, z.mRow);
		}
		Vector2 c = Centre(z);
		(float left, float right) = LawnEdges(b, 0);
		int row = b.PixelToGridYKeepOnBoard((int)Math.Max(left + 1f, Math.Min(right - 1f, c.X)), (int)c.Y);
		return Math.Max(0, Math.Min(LastRow(b), row));
	}

	public static bool IsQuantum(Zombie z)
	{
		return z.mTavel != null && !z.mDead && z.mBoard != null && z.mTavel.DmdfQuantumUntil > z.mBoard.mMainCounter;
	}

	private static void MakeQuantum(Zombie z)
	{
		if (!Tavel.Alive(z) || Tavel.IsBoss(z))
		{
			return;
		}
		TavelHooks.ZState(z).DmdfQuantumUntil = z.mBoard.mMainCounter + QuantumDuration;
	}

	private static void AddEffect(Board b, bool theHole, float x, float y, float theScale, int theDelay = 0)
	{
		Tavel.State(b).DmdfEffects.Add(new Effect { Hole = theHole, X = x, Y = y, Scale = theScale, Start = b.mMainCounter + theDelay });
	}

	/// <summary>Removes the zombie outright: no corpse.</summary>
	private static void Annihilate(Zombie z)
	{
		Vector2 c = Centre(z);
		AddEffect(z.mBoard, theHole: false, c.X, c.Y, 0.3f);
		if (z.mTavel != null)
		{
			z.mTavel.DmdfQuantumUntil = 0;
		}
		z.DieWithLoot();
	}

	/// <summary>Cosmic damage: a zombie it would kill is annihilated instead.</summary>
	private static void CosmicDamage(Zombie z, int theDamage)
	{
		if (!Tavel.IsBoss(z) && Tavel.ZombieEffectiveHealth(z) <= theDamage)
		{
			Annihilate(z);
			return;
		}
		z.TakeDamage(theDamage, 0u);
	}

	private static void Strike(Zombie z, int theDamage)
	{
		CosmicDamage(z, theDamage);
		MakeQuantum(z);
	}

	// ------------------------------------------------------------------ her shots

	private static bool HasTarget(Plant p)
	{
		Board b = p.mBoard;
		(float left, float right) = LawnEdges(b, p.mRow);
		float feet = p.mX + FeetX;
		foreach (Zombie z in b.mZombies)
		{
			if (!Tavel.ZombieIsInRow(z, p.mRow) || !CanHit(z))
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			if (r.mX + r.mWidth >= feet && r.mX <= right + 20f)
			{
				return true;
			}
		}
		return false;
	}

	private static void SpawnShot(Plant p, State s)
	{
		Board b = p.mBoard;
		float feetY = p.mY + FeetY;
		Tavel.State(b).DmdfShots.Add(new Shot { Row = p.mRow, X = p.mX + FeetX + ShotStartDX, Y = feetY + ShotStartDY, FlyY = feetY + ShotFlyDY, Collapse = s.Collapse, Born = b.mMainCounter });
		p.mApp.PlayFoley(FoleyType.Fume);
	}

	/// <summary>The fan-shaped burst behind the zombie a shot has just hit.</summary>
	private static void Burst(Board b, Shot s, Zombie theTarget, float theHitX)
	{
		int row = s.Row;
		foreach (Zombie z in b.mZombies.ToArray())
		{
			if (z == theTarget || !CanHit(z))
			{
				continue;
			}
			float offset = Centre(z).X - theHitX;
			int away = Tavel.ZombieRowDistance(z, row);
			float near;
			if (away == 0)
			{
				near = BurstNear;
			}
			else if (away == 1)
			{
				near = BurstSideNear;
			}
			else
			{
				continue;
			}
			if (near <= offset && offset <= BurstFar)
			{
				Strike(z, Damage);
			}
		}
		float y = RowCentreY(b, theHitX, row);
		AddEffect(b, theHole: false, theHitX + 45f, y - 8f, 0.52f);
		foreach (int side in new[] { row - 1, row + 1 })
		{
			if (side >= 0 && side <= LastRow(b))
			{
				AddEffect(b, theHole: false, theHitX + 78f, RowCentreY(b, theHitX, side) - 8f, 0.4f, 5);
			}
		}
		GlobalStaticVars.gLawnApp.PlayFoley(FoleyType.Ignite);
	}

	/// <summary>A collapse bomb reaches its first zombie: that zombie's cell is emptied.</summary>
	private static void Collapse(Board b, Shot s, Zombie theFirst)
	{
		Vector2 c = Centre(theFirst);
		foreach (Zombie z in b.mZombies.ToArray())
		{
			if (z.mRow != s.Row || !CanHit(z) || Tavel.IsBoss(z))
			{
				continue;
			}
			if (Math.Abs(Centre(z).X - c.X) <= CollapseCell)
			{
				Annihilate(z);
			}
		}
		AddEffect(b, theHole: true, c.X, c.Y, 0.85f);
		GlobalStaticVars.gLawnApp.PlayFoley(FoleyType.Portal);
	}

	private static Zombie FirstZombieAt(Board b, Shot s)
	{
		float left = s.X - ShotHalfWidth;
		float right = s.X + ShotHalfWidth;
		Zombie best = null;
		int bestX = 0;
		foreach (Zombie z in b.mZombies)
		{
			if (!Tavel.ZombieIsInRow(z, s.Row) || !CanHit(z) || s.Hit.Contains(z))
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			if (r.mX < right && r.mX + r.mWidth > left && (best == null || r.mX < bestX))
			{
				best = z;
				bestX = r.mX;
			}
		}
		return best;
	}

	private static void UpdateShots(Board b, TavelBoard bs)
	{
		List<Shot> flying = new List<Shot>();
		foreach (Shot s in bs.DmdfShots)
		{
			s.X += ShotSpeed;
			s.Y += (s.FlyY - s.Y) * 0.12f;
			Zombie z = FirstZombieAt(b, s);
			if (z != null && s.Collapse)
			{
				Collapse(b, s, z);
				// from here on an ordinary shot: what the collapse spared is struck as usual
				s.Collapse = false;
				z = FirstZombieAt(b, s);
			}
			if (z != null)
			{
				bool wasQuantum = IsQuantum(z);
				float hitX = Centre(z).X;
				s.Hit.Add(z);
				Strike(z, Damage);
				Burst(b, s, z, hitX);
				if (!wasQuantum)
				{
					continue;
				}
			}
			if (s.X > LawnEdges(b, s.Row).Right + 80f)
			{
				continue;
			}
			flying.Add(s);
		}
		bs.DmdfShots = flying;
	}

	// ------------------------------------------------------------------ quantum zombies and piercing projectiles

	private static bool IsStraight(Projectile pr)
	{
		switch (pr.mMotionType)
		{
		case ProjectileMotion.Straight:
		case ProjectileMotion.Threepeater:
		case ProjectileMotion.Puff:
		case ProjectileMotion.Backwards:
		case ProjectileMotion.Star:
		case ProjectileMotion.YAwareStraight:
			return true;
		default:
			return false;
		}
	}

	/// <summary>Projectile.DoImpact on a quantum zombie: the impact happens, the projectile goes on (true: pierce).</summary>
	public static bool BeginPierce(Projectile pr, Zombie z)
	{
		if (z == null || !IsStraight(pr) || !IsQuantum(z))
		{
			return false;
		}
		TavelShot s = TavelHooks.Shot(pr);
		s.DmdfPierced ??= new List<Zombie>();
		s.DmdfPierced.Add(z);
		sPiercing = pr;
		return true;
	}

	public static void EndPierce()
	{
		sPiercing = null;
	}

	/// <summary>Projectile.Die: the piercing projectile survives its impact.</summary>
	public static bool KeepAlive(Projectile pr) => sPiercing == pr;

	/// <summary>Projectile.FindCollisionTarget: zombies it passed through look dead to the search.</summary>
	public static List<Zombie> HideForSearch(Projectile pr)
	{
		List<Zombie> list = pr.mTavel?.DmdfPierced;
		if (list == null || list.Count == 0)
		{
			return null;
		}
		List<Zombie> hidden = list.Where((Zombie z) => !z.mDead).ToList();
		foreach (Zombie z in hidden)
		{
			z.mDead = true;
		}
		return hidden;
	}

	public static bool DrawZombie(Zombie z, Graphics g)
	{
		if (!IsQuantum(z))
		{
			return false;
		}
		Tavel.Tinted(g, QuantumTint, delegate
		{
			z.DrawCore(g);
		});
		return true;
	}

	/// <summary>After Zombie.Update: a quantum summoner's timer advances on one tick in three.</summary>
	public static void AfterZombieUpdate(Zombie z, int theSummonBefore)
	{
		if (z.mZombieType != ZombieType.Dancer && z.mZombieType != ZombieType.Monk && z.mZombieType != ZombieType.Talisman)
		{
			return;
		}
		if (!IsQuantum(z))
		{
			return;
		}
		if (theSummonBefore > 0 && z.mSummonCounter == theSummonBefore - 1 && z.mBoard.mMainCounter % QuantumCastSlowdown != 0)
		{
			z.mSummonCounter = theSummonBefore;
		}
	}

	// ------------------------------------------------------------------ the dragon

	private static Vector2 DragonSpot(Board b)
	{
		float left = LawnEdges(b, 0).Left;
		float middle = (RowCentreY(b, left + 40f, 0) + RowCentreY(b, left + 40f, LastRow(b))) / 2f;
		return new Vector2(left + PortalDX, middle + PortalDY);
	}

	private static void SummonDragon(Plant p)
	{
		Board b = p.mBoard;
		TavelBoard bs = Tavel.State(b);
		int now = b.mMainCounter;
		// a dragon already out simply gets more shots
		Dragon out1 = bs.DmdfDragons.FirstOrDefault((Dragon d) => d.Closing < 0);
		if (out1 != null)
		{
			out1.Shots += UltShots;
			return;
		}
		Vector2 spot = DragonSpot(b);
		int ready = now + DragonDelay + DragonIntroTicks;
		bs.DmdfDragons.Add(new Dragon { X = spot.X, Y = spot.Y, Start = now, Shots = UltShots, Ready = ready, NextFire = ready, LastFire = ready });
		p.mApp.PlayFoley(FoleyType.Portal);
	}

	private static Zombie PickTarget(Board b, TavelBoard bs, Dragon d)
	{
		List<Zombie> busy = new List<Zombie>(d.Picked);
		busy.AddRange(bs.DmdfBigShots.Where((BigShot s) => s.Target != null).Select((BigShot s) => s.Target));
		List<Zombie> candidates = new List<Zombie>();
		foreach (Zombie z in b.mZombies)
		{
			if (!CanHit(z) || z.mZombieType == ZombieType.Bungee)
			{
				continue;
			}
			(float left, float right) = LawnEdges(b, Math.Max(0, z.mRow));
			float cx = Centre(z).X;
			if (left - 20f <= cx && cx <= right + 20f)
			{
				candidates.Add(z);
			}
		}
		List<Zombie> fresh = candidates.Where((Zombie z) => !busy.Contains(z)).ToList();
		List<Zombie> pool = fresh.Count > 0 ? fresh : candidates;
		if (pool.Count == 0)
		{
			return null;
		}
		Zombie t = pool[Tavel.Rand(0, pool.Count - 1)];
		d.Picked.Add(t);
		return t;
	}

	private static void FireDragon(Board b, TavelBoard bs, Dragon d)
	{
		Zombie target = PickTarget(b, bs, d);
		Vector2 goal;
		int row;
		if (target != null)
		{
			goal = Centre(target);
			row = ZombieRow(b, target);
		}
		else
		{
			// nothing to aim at: a random spot on the right half of the lawn
			row = Tavel.Rand(0, LastRow(b));
			float x = LawnEdges(b, row).Left + 40f + 80f * Tavel.Rand(4, 7);
			goal = new Vector2(x, RowCentreY(b, x, row) - 10f);
		}
		bs.DmdfBigShots.Add(new BigShot { X = d.X + MouthDX, Y = d.Y + MouthDY, Target = target, Goal = goal, Row = row, Born = b.mMainCounter });
		GlobalStaticVars.gLawnApp.PlayFoley(FoleyType.Gargantudeath);
		GlobalStaticVars.gLawnApp.PlayFoley(FoleyType.Fume);
	}

	private static void UpdateDragons(Board b, TavelBoard bs)
	{
		int now = b.mMainCounter;
		List<Dragon> present = new List<Dragon>();
		foreach (Dragon d in bs.DmdfDragons.ToArray())
		{
			if (d.Closing >= 0)
			{
				if (now < d.Closing + DragonOutroTicks + DragonDelay)
				{
					present.Add(d);
				}
				continue;
			}
			if (d.Shots > 0)
			{
				if (now >= d.NextFire)
				{
					d.Shots--;
					d.NextFire = now + DragonFireGap;
					d.LastFire = now;
					FireDragon(b, bs, d);
				}
			}
			else if (now >= d.LastFire + DragonLinger)
			{
				d.Closing = now;
			}
			present.Add(d);
		}
		bs.DmdfDragons = present;
	}

	private static void DrawDragons(Board b, TavelBoard bs, Graphics g)
	{
		if (bs.DmdfDragons.Count == 0)
		{
			return;
		}
		int now = b.mMainCounter;
		bool oldColorize = g.GetColorizeImages();
		Color oldColor = g.GetColor();
		g.SetColorizeImages(colorizeImages: true);
		try
		{
			foreach (Dragon d in bs.DmdfDragons)
			{
				int age = now - d.Start;
				float opened = Math.Min(1f, age / 25f);
				if (d.Closing >= 0)
				{
					float left = d.Closing + DragonOutroTicks + DragonDelay - now;
					opened = Math.Max(0f, Math.Min(opened, left / 25f));
				}
				g.SetColor(new Color(255, 255, 255, (int)(255f * opened)));
				PortalSheet.Draw(g, age * 10 / Tps, d.X, d.Y, 0.5f * opened, 0.5f * (0.6f + 0.4f * opened));
				int ticks = age - DragonDelay;
				if (ticks < 0)
				{
					continue;
				}
				// it grows out of the portal, and shrinks back into it
				float outAmount = Math.Min(1f, ticks / (float)DragonFade);
				int cell;
				if (d.Closing >= 0)
				{
					int since = now - d.Closing;
					if (since >= DragonOutroTicks)
					{
						continue;
					}
					cell = DragonOutro[Math.Min(DragonOutro.Length - 1, since * DragonFps / Tps)];
					outAmount = Math.Min(outAmount, (DragonOutroTicks - since) / (float)DragonFade);
				}
				else if (ticks < DragonIntroTicks)
				{
					cell = DragonIntro[Math.Min(DragonIntro.Length - 1, ticks * DragonFps / Tps)];
				}
				else
				{
					int step = (ticks - DragonIntroTicks) * DragonFps / Tps;
					cell = DragonRoar[step % DragonRoar.Length];
				}
				g.SetColor(new Color(255, 255, 255, (int)(255f * Math.Max(0f, outAmount))));
				DragonSheet.Draw(g, cell, d.X, d.Y, DragonScale * (0.4f + 0.6f * outAmount));
			}
		}
		finally
		{
			g.SetColor(oldColor, premultiply: false);
			g.SetColorizeImages(oldColorize);
		}
	}

	private static bool InBurnArea(Zombie z, float x, int theRow)
	{
		return Tavel.ZombieRowDistance(z, theRow) <= 1 && Math.Abs(Centre(z).X - x) <= BurnHalfWidth;
	}

	private static void BigShotLands(Board b, TavelBoard bs, float x, float y, int theRow)
	{
		int now = b.mMainCounter;
		foreach (Zombie z in b.mZombies.ToArray())
		{
			if (CanHit(z) && InBurnArea(z, x, theRow))
			{
				Strike(z, UltImpactDamage);
			}
		}
		bs.DmdfFlames.Add(new Flame { Row = theRow, X = x, Start = now, Until = now + BurnDuration, NextHit = now + BurnInterval });
		AddEffect(b, theHole: false, x, y, 0.75f);
		GlobalStaticVars.gLawnApp.PlayFoley(FoleyType.JalapenoIgnite);
		b.ShakeBoard(2, -3);
	}

	private static void UpdateBigShots(Board b, TavelBoard bs)
	{
		int now = b.mMainCounter;
		List<BigShot> flying = new List<BigShot>();
		foreach (BigShot s in bs.DmdfBigShots.ToArray())
		{
			if (s.Target != null)
			{
				if (!Tavel.Alive(s.Target))
				{
					// lands where its zombie was last seen
					s.Target = null;
				}
				else
				{
					s.Goal = Centre(s.Target);
					s.Row = ZombieRow(b, s.Target);
				}
			}
			float dx = s.Goal.X - s.X;
			float dy = s.Goal.Y - s.Y;
			float d = (float)Math.Sqrt(dx * dx + dy * dy);
			if (d <= BigShotSpeed)
			{
				BigShotLands(b, bs, s.Goal.X, s.Goal.Y, s.Row);
				continue;
			}
			s.X += dx / d * BigShotSpeed;
			s.Y += dy / d * BigShotSpeed;
			flying.Add(s);
		}
		bs.DmdfBigShots = flying;
		List<Flame> burning = new List<Flame>();
		foreach (Flame f in bs.DmdfFlames)
		{
			if (now >= f.NextHit)
			{
				f.NextHit += BurnInterval;
				foreach (Zombie z in b.mZombies.ToArray())
				{
					if (CanHit(z) && InBurnArea(z, f.X, f.Row))
					{
						CosmicDamage(z, BurnDamage);
					}
				}
			}
			if (now < f.Until)
			{
				burning.Add(f);
			}
		}
		bs.DmdfFlames = burning;
	}

	private static void DrawFlames(Board b, TavelBoard bs, Graphics g)
	{
		if (bs.DmdfFlames.Count == 0)
		{
			return;
		}
		int now = b.mMainCounter;
		bool oldColorize = g.GetColorizeImages();
		Color oldColor = g.GetColor();
		g.SetColorizeImages(colorizeImages: true);
		try
		{
			foreach (Flame f in bs.DmdfFlames)
			{
				float shown = Math.Max(0f, Math.Min(1f, Math.Min((now - f.Start) / 20f, (f.Until - now) / 60f)));
				g.SetColor(new Color(255, 255, 255, (int)(215f * shown)));
				float left = f.X - BurnHalfWidth;
				for (int row = f.Row - 1; row <= f.Row + 1; row++)
				{
					if (row < 0 || row > LastRow(b))
					{
						continue;
					}
					for (int tile = 0; tile < 4; tile++)
					{
						float x = left + tile * 80f;
						// neighbouring tiles out of step with each other
						FlameSheet.Draw(g, now * 14 / Tps + tile * 5 + row * 3, x, RowCentreY(b, x + 40f, row) + 30f, 0.5f);
					}
				}
			}
		}
		finally
		{
			g.SetColor(oldColor, premultiply: false);
			g.SetColorizeImages(oldColorize);
		}
	}

	private static void UpdateEffects(Board b, TavelBoard bs)
	{
		int now = b.mMainCounter;
		bs.DmdfEffects.RemoveAll((Effect e) => (now - e.Start) * EffectFps / Tps >= (e.Hole ? HoleSheet : BlastSheet).FrameCount);
	}

	private static void DrawBoard(Board b, Graphics g)
	{
		TavelBoard bs = b.mTavel;
		if (bs == null)
		{
			return;
		}
		int now = b.mMainCounter;
		DrawFlames(b, bs, g);
		DrawDragons(b, bs, g);
		foreach (Shot s in bs.DmdfShots)
		{
			int frame = (now - s.Born) * 14 / Tps;
			if (s.Collapse)
			{
				CollapseSheet.Draw(g, frame, s.X, s.Y, CollapseScale);
			}
			else
			{
				BallSheet.Draw(g, frame, s.X, s.Y, BallScale);
			}
		}
		foreach (BigShot s in bs.DmdfBigShots)
		{
			BigShotSheet.Draw(g, (now - s.Born) * 12 / Tps, s.X, s.Y, BigShotScale);
		}
		foreach (Effect e in bs.DmdfEffects)
		{
			if (now < e.Start)
			{
				continue;
			}
			TavelSpriteSheet sh = e.Hole ? HoleSheet : BlastSheet;
			int frame = (now - e.Start) * EffectFps / Tps;
			if (frame < sh.FrameCount)
			{
				sh.Draw(g, frame, e.X, e.Y, e.Scale);
			}
		}
	}

	// ------------------------------------------------------------------ the plant

	private static void StartAction(Plant p, State s, string theMode)
	{
		s.Mode = theMode;
		s.ModeStart = p.mBoard.mMainCounter;
		s.Frame = 0;
		s.Acted = false;
	}

	private static void ReturnToIdle(Plant p, State s)
	{
		s.Mode = "idle";
		s.IdleStart = p.mBoard.mMainCounter;
	}

	private static void Update(Plant p)
	{
		State s = Get(p);
		if (s.RollPlantUltimate)
		{
			s.RollPlantUltimate = false;
			if (Tavel.UltimateUiAllowed(p.mBoard) && Tavel.Rand(0, 99) < UltPlantChance)
			{
				StartAction(p, s, "ultimate");
			}
		}
		int ticks = p.mBoard.mMainCounter - s.ModeStart;
		if (s.Mode == "ultimate")
		{
			int frame = ticks * SaFps / Tps;
			if (!s.Acted && frame >= SaSummonFrame)
			{
				s.Acted = true;
				SummonDragon(p);
				// her next shot after an ultimate is always a collapse bomb
				s.Guaranteed = true;
			}
			if (frame >= SaSheet.FrameCount)
			{
				ReturnToIdle(p, s);
				return;
			}
			s.Frame = frame;
			return;
		}
		if (s.Mode == "attack")
		{
			int frame = ticks * AttackFps / Tps;
			if (!s.Acted && frame >= AttackHitFrame)
			{
				s.Acted = true;
				SpawnShot(p, s);
			}
			if (frame >= AttackSheet.FrameCount)
			{
				ReturnToIdle(p, s);
				return;
			}
			s.Frame = frame;
			return;
		}
		int now = p.mBoard.mMainCounter;
		if (now < s.NextAttack || !HasTarget(p))
		{
			return;
		}
		s.Collapse = s.Guaranteed || Tavel.Rand(0, 99) < CollapseChance;
		s.Guaranteed = false;
		s.NextAttack = now + AttackInterval;
		StartAction(p, s, "attack");
	}

	private static void UpdateBoard(Board b)
	{
		TavelBoard bs = b.mTavel;
		if (bs == null)
		{
			return;
		}
		if (bs.DmdfShots.Count > 0)
		{
			UpdateShots(b, bs);
		}
		if (bs.DmdfDragons.Count > 0)
		{
			UpdateDragons(b, bs);
		}
		if (bs.DmdfBigShots.Count > 0 || bs.DmdfFlames.Count > 0)
		{
			UpdateBigShots(b, bs);
		}
		if (bs.DmdfEffects.Count > 0)
		{
			UpdateEffects(b, bs);
		}
	}
}
