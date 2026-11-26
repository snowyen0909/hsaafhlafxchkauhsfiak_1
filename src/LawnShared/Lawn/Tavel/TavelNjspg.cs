// 娜迦蛇皮果 (plant_njspg.py): drawn from sprite sheets. Lashes her tail to send water tornadoes down the row
// (hurting and freezing what they touch); the ultimate throws a water orb that bursts into a whirlpool.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public sealed partial class TavelBoard
{
	public List<TavelNjspg.Tornado> NjspgTornadoes = new List<TavelNjspg.Tornado>();

	public List<TavelNjspg.Orb> NjspgOrbs = new List<TavelNjspg.Orb>();

	public List<TavelNjspg.Vortex> NjspgVortexes = new List<TavelNjspg.Vortex>();
}

public static class TavelNjspg
{
	public const int Id = 112;

	public static SeedType Seed => (SeedType)Id;

	private const int Tps = Tavel.TicksPerSecond;

	public const int Cost = 200;

	public const int RefreshTime = 750;

	public const int Health = 300;

	public const float FeetX = 46f;

	public const float FeetY = 72f;

	/// <summary>Lawn units per pixel of the source clips (she is 410 px, 100 units tall).</summary>
	public const float ClipUnit = 100f / 410f;

	private static readonly TavelSpriteSheet IdleSheet = new TavelSpriteSheet("tavel/njspg/idle.png", 10, 220, 258, 60, 161.6f, 252.8f);

	public const float IdleScale = ClipUnit / 0.6f;

	public const int IdleFps = 12;

	private static readonly TavelSpriteSheet AttackSheet = new TavelSpriteSheet("tavel/njspg/attack.png", 7, 328, 382, 48, 215.6f, 378.2f);

	private const string AttackBack = "tavel/njspg/attack_back.png";

	public const float AttackScale = ClipUnit / 0.6f;

	public const int AttackFps = 24;

	public const int AttackHitFrame = 19;

	private static readonly TavelSpriteSheet SaSheet = new TavelSpriteSheet("tavel/njspg/sa.png", 10, 394, 386, 60, 142f, 381.5f);

	public const float SaScale = ClipUnit / 0.5f;

	public const int SaFps = 15;

	public const int SaThrowFrame = 30;

	public const float OrbStartDX = 11f;

	public const float OrbStartDY = -140f;

	public const float OrbStartSize = 86f;

	public const float OrbEndSize = 56f;

	public const float OrbSpeed = 7f;

	public const int AttackInterval = 240;

	public const int DoubleChance = 60;

	public const float DoubleGap = 22f;

	public const float TornadoSpeed = 160f / Tps;

	public const float TornadoContactSpeed = 80f / Tps;

	public const int TornadoDamage = 20;

	public const int TornadoDamageInterval = 15;

	public const int TornadoFreeze = 50;

	public const float TornadoHalfWidth = 34f;

	public const float TornadoStartDX = 34f;

	public const int TornadoGrowTicks = 25;

	public const float TornadoOverrun = 70f;

	private static readonly TavelSpriteSheet TornadoSheet = new TavelSpriteSheet("tavel/njspg/tornado.png", 6, 154, 204, 30, 77f, 199f);

	public const float TornadoScale = 0.66f;

	public const int TornadoAlpha = 225;

	public const int UltPlantChance = 50;

	public const int VortexDuration = 600;

	public const int VortexExtension = 300;

	public const int VortexMaxAbsorb = 3;

	public const float VortexRadiusX = 200f;

	public const float VortexRadiusY = 250f;

	public const int VortexOpenTicks = 35;

	public const int VortexCloseTicks = 45;

	public const int VortexGiantDamage = 1800;

	public const float VortexTurn = 2f * (float)Math.PI / 240f;

	public const float VortexPull = 1f / 160f;

	public const float VortexCore = 0.1f;

	private static readonly TavelSpriteSheet VortexSheet = new TavelSpriteSheet("tavel/njspg/vortex.png", 4, 320, 320, 16, 160f, 160f);

	public const int VortexFps = 20;

	public sealed class Tornado
	{
		public Plant Owner;

		public int Row;

		public float X;

		public float Y;

		public int Direction;

		public int Born;

		public int NextHit;
	}

	public sealed class Orb
	{
		public float X;

		public float Y;

		public Zombie Target;

		public Vector2 Fallback;

		public float Travel;

		public int Born;
	}

	public sealed class Held
	{
		public Zombie Zombie;

		public float Angle;

		public float Reach;

		public bool Giant;

		public bool Hit;

		public float OffsetX;

		public float OffsetY;
	}

	public sealed class Vortex
	{
		public float X;

		public float Y;

		public int Start;

		public int Until;

		public int Absorbed;

		public bool Closing;

		public Dictionary<Zombie, Held> Held = new Dictionary<Zombie, Held>();
	}

	public sealed class State
	{
		public int IdleStart;

		public string Mode = "idle";

		public int ModeStart;

		public int Frame;

		public int Direction = 1;

		public bool Acted;

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
		return new TavelSpec(Id, "njspg")
		{
			Name = "娜迦蛇皮果",
			Cost = Cost,
			RefreshTime = RefreshTime,
			Card = "tavel/njspg/card.png",
			AlmanacText = "娜迦蛇皮果甩动蛇尾卷起水龙卷前进，对僵尸造成持续伤害和冻结效果。\n{SHORTLINE}\n{KEYWORD}范围：{STAT}整行\n{KEYWORD}特点：{STAT}水龙卷穿透所有僵尸，接触期间持续伤害并冻结；有概率甩出双水龙卷；会自动转向身后的僵尸\n{KEYWORD}大招：{STAT}抛出水球，在最近的僵尸处形成5×5的大漩涡，把僵尸卷向中心后消灭中小型僵尸并重创大型僵尸；漩涡吸收水龙卷可延长时间\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}短\n{KEYWORD}阳光：{STAT}200\n{SHORTLINE}\n{FLAVOR}“感受深海的力量吧！”娜迦蛇皮果甩动蛇尾，卷起水龙卷席卷前方所有敌人。",
			DefName = "NJSPG",
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
			// tornadoes, orbs and whirlpools already out carry on without her
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
			DrawBoard = delegate(Board b, Graphics g)
			{
				DrawTornadoes(b, g);
				DrawOrbs(b, g);
			}
		};
	}

	private static int IdleFrame(int theTicks) => theTicks * IdleFps / Tps;

	private static int PreviewFrame() => IdleFrame(GlobalStaticVars.gLawnApp.mAppCounter);

	private static void DrawSprite(Graphics g, int theFrame, float x, float y, float theUnitX = 1f, float theUnitY = 1f)
	{
		IdleSheet.Draw(g, theFrame, x, y, IdleScale * theUnitX, IdleScale * theUnitY);
	}

	/// <summary>A frame of the attack sheet, or of its mirrored twin (the whole picture flipped left to right).</summary>
	private static void DrawAttackFrame(Graphics g, int theFrame, float x, float y, float theScale, bool theMirror)
	{
		TavelSpriteSheet sh = AttackSheet;
		if (!theMirror)
		{
			sh.Draw(g, theFrame, x, y, theScale);
			return;
		}
		Image img = Tavel.LoadImage(AttackBack);
		if (img == null)
		{
			return;
		}
		int i = theFrame % sh.FrameCount;
		TRect src = new TRect((sh.Columns - 1 - i % sh.Columns) * sh.FrameWidth, i / sh.Columns * sh.FrameHeight, sh.FrameWidth, sh.FrameHeight);
		TRect dst = new TRect((int)Math.Round(x - (sh.FrameWidth - sh.AnchorX) * theScale), (int)Math.Round(y - sh.AnchorY * theScale), (int)Math.Round(sh.FrameWidth * theScale), (int)Math.Round(sh.FrameHeight * theScale));
		g.DrawImage(img, dst, src);
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
		if (s.Mode == "attack")
		{
			DrawAttackFrame(g, s.Frame, FeetX, FeetY, AttackScale, s.Direction < 0);
		}
		else if (s.Mode == "ultimate")
		{
			SaSheet.Draw(g, s.Frame, FeetX, FeetY, SaScale);
		}
		else
		{
			DrawSprite(g, IdleFrame(p.mBoard.mMainCounter - s.IdleStart), FeetX, FeetY);
		}
	}

	private static bool CanTarget(Zombie z)
	{
		return Tavel.Alive(z) && !z.mMindControlled && (!Tavel.IsBoss(z) || Tavel.BossCanBeHit(z));
	}

	private static (float Left, float Right) LawnEdges(Board b, int theRow)
	{
		return (b.GridToPixelX(0, theRow), b.GridToPixelX(8, theRow) + 80);
	}

	/// <summary>Keeps the zombie frozen for at least theTicks more ticks.</summary>
	private static void Freeze(Zombie z, int theTicks)
	{
		if (z.mIceTrapCounter > 0)
		{
			if (z.mIceTrapCounter < theTicks)
			{
				z.mIceTrapCounter = theTicks;
			}
			return;
		}
		if (!z.CanBeFrozen())
		{
			return;
		}
		int chilled = z.mChilledCounter;
		z.HitIceTrap();
		if (z.mIceTrapCounter > 0)
		{
			// the game's freeze is long and leaves a long chill; this one is short, renewed by every hit
			z.mIceTrapCounter = theTicks;
			z.mChilledCounter = Math.Max(chilled, theTicks + 100);
		}
	}

	/// <summary>1: a zombie ahead in the row, -1: only behind her, 0: none.</summary>
	private static int AttackDirection(Plant p)
	{
		Board b = p.mBoard;
		(float left, float right) = LawnEdges(b, p.mRow);
		float feet = p.mX + FeetX;
		bool behind = false;
		foreach (Zombie z in b.mZombies)
		{
			if (!Tavel.ZombieIsInRow(z, p.mRow) || !CanTarget(z))
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			if (r.mX > right + TornadoOverrun)
			{
				continue;
			}
			if (r.mX + r.mWidth >= feet)
			{
				return 1;
			}
			if (r.mX + r.mWidth >= left - TornadoOverrun)
			{
				behind = true;
			}
		}
		return behind ? -1 : 0;
	}

	private static void SpawnTornadoes(Plant p, State s)
	{
		Board b = p.mBoard;
		int count = Tavel.Rand(0, 99) < DoubleChance ? 2 : 1;
		float x = p.mX + FeetX + TornadoStartDX * s.Direction;
		for (int i = 0; i < count; i++)
		{
			Tavel.State(b).NjspgTornadoes.Add(new Tornado
			{
				Owner = p,
				Row = p.mRow,
				X = x - i * DoubleGap * s.Direction,
				Y = p.mY + FeetY + 6f,
				Direction = s.Direction,
				Born = b.mMainCounter,
				// the second one strikes between the first one's hits
				NextHit = b.mMainCounter + i * (TornadoDamageInterval / 2)
			});
		}
		p.mApp.PlayFoley(FoleyType.Swing);
		p.mApp.PlayFoley(FoleyType.Zombiesplash);
	}

	private static float VortexReach(Vortex v, float x, float y)
	{
		float dx = (x - v.X) / VortexRadiusX;
		float dy = (y - v.Y) / VortexRadiusY;
		return (float)Math.Sqrt(dx * dx + dy * dy);
	}

	private static Vortex VortexAt(TavelBoard bs, float x, float y)
	{
		return bs.NjspgVortexes.FirstOrDefault((Vortex v) => !v.Closing && v.Absorbed < VortexMaxAbsorb && VortexReach(v, x, y) < 0.8f);
	}

	private static void UpdateTornadoes(Board b, TavelBoard bs)
	{
		int now = b.mMainCounter;
		List<Tornado> alive = new List<Tornado>();
		foreach (Tornado t in bs.NjspgTornadoes)
		{
			Vortex v = VortexAt(bs, t.X, t.Y - 20f);
			if (v != null)
			{
				v.Absorbed++;
				v.Until += VortexExtension;
				continue;
			}
			float left = t.X - TornadoHalfWidth;
			float right = t.X + TornadoHalfWidth;
			List<Zombie> touching = new List<Zombie>();
			foreach (Zombie z in b.mZombies)
			{
				if (!Tavel.ZombieIsInRow(z, t.Row) || !CanTarget(z))
				{
					continue;
				}
				TRect r = z.GetZombieRect();
				if (r.mX < right && r.mX + r.mWidth > left)
				{
					touching.Add(z);
				}
			}
			if (touching.Count > 0 && now >= t.NextHit)
			{
				t.NextHit = now + TornadoDamageInterval;
				foreach (Zombie z in touching)
				{
					z.TakeDamage(TornadoDamage, 0u);
					if (CanTarget(z))
					{
						Freeze(z, TornadoFreeze);
					}
				}
			}
			t.X += (touching.Count > 0 ? TornadoContactSpeed : TornadoSpeed) * t.Direction;
			(float ll, float lr) = LawnEdges(b, t.Row);
			if (t.X > lr + TornadoOverrun || t.X < ll - TornadoOverrun)
			{
				continue;
			}
			alive.Add(t);
		}
		bs.NjspgTornadoes = alive;
	}

	private static void DrawTornadoes(Board b, Graphics g)
	{
		if (b.mTavel == null || b.mTavel.NjspgTornadoes.Count == 0)
		{
			return;
		}
		int now = b.mMainCounter;
		bool oldColorize = g.GetColorizeImages();
		Color oldColor = g.GetColor();
		g.SetColorizeImages(colorizeImages: true);
		try
		{
			// back rows first, so a nearer tornado overlaps a farther one
			foreach (Tornado t in b.mTavel.NjspgTornadoes.OrderBy((Tornado q) => q.Y))
			{
				int age = now - t.Born;
				float grow = Math.Min(1f, 0.35f + 0.65f * age / TornadoGrowTicks);
				(float ll, float lr) = LawnEdges(b, t.Row);
				float past = Math.Max(t.X - lr, ll - t.X);
				float fade = Math.Max(0f, Math.Min(1f, 1f - (past - 20f) / (TornadoOverrun - 20f)));
				g.SetColor(new Color(255, 255, 255, (int)(TornadoAlpha * fade)));
				TornadoSheet.Draw(g, age * 12 / Tps, t.X, t.Y, TornadoScale * grow);
			}
		}
		finally
		{
			g.SetColor(oldColor, premultiply: false);
			g.SetColorizeImages(oldColorize);
		}
	}

	private static Zombie NearestZombie(Board b, float x, float y)
	{
		Zombie best = null;
		double bestD = 0;
		foreach (Zombie z in b.mZombies)
		{
			if (!CanTarget(z) || z.mZombieType == ZombieType.Bungee || z.mZombieType == ZombieType.Boss)
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			(float left, float right) = LawnEdges(b, Math.Max(0, z.mRow));
			float cx = r.mX + r.mWidth / 2f;
			if (cx > right + 20f || cx < left - 40f)
			{
				continue;
			}
			double d = Math.Sqrt((cx - x) * (cx - x) + Math.Pow((r.mY + r.mHeight / 2f - y) * 0.8f, 2));
			if (best == null || d < bestD)
			{
				best = z;
				bestD = d;
			}
		}
		return best;
	}

	private static float RowCentreY(Board b, float x, int theRow)
	{
		int col = Math.Max(0, Math.Min(8, b.PixelToGridXKeepOnBoard((int)x, 100)));
		return b.GridToPixelY(col, theRow) + 55f;
	}

	private static void ThrowOrb(Plant p)
	{
		Board b = p.mBoard;
		float x = p.mX + FeetX + OrbStartDX;
		float y = p.mY + FeetY + OrbStartDY;
		(float left, float right) = LawnEdges(b, p.mRow);
		// with nothing to hit it lands three columns ahead of her
		float fx = Math.Min(right - 40f, p.mX + FeetX + 240f);
		Tavel.State(b).NjspgOrbs.Add(new Orb
		{
			X = x,
			Y = y,
			Target = NearestZombie(b, p.mX + FeetX, p.mY + FeetY - 40f),
			Fallback = new Vector2(fx, RowCentreY(b, fx, p.mRow)),
			Born = b.mMainCounter
		});
		p.mApp.PlayFoley(FoleyType.Throw);
	}

	private static Vector2 OrbGoal(Board b, Orb o)
	{
		Zombie target = o.Target;
		if (target != null && !CanTarget(target))
		{
			target = o.Target = NearestZombie(b, o.X, o.Y);
		}
		if (target == null)
		{
			return o.Fallback;
		}
		TRect r = target.GetZombieRect();
		float x = r.mX + r.mWidth / 2f;
		// if it dies now, burst where it stood
		o.Fallback = new Vector2(x, RowCentreY(b, x, Math.Max(0, target.mRow)));
		return o.Fallback;
	}

	private static void UpdateOrbs(Board b, TavelBoard bs)
	{
		List<Orb> flying = new List<Orb>();
		foreach (Orb o in bs.NjspgOrbs)
		{
			Vector2 goal = OrbGoal(b, o);
			float dx = goal.X - o.X;
			float dy = goal.Y - o.Y;
			float d = (float)Math.Sqrt(dx * dx + dy * dy);
			if (d <= OrbSpeed)
			{
				OpenVortex(b, bs, goal.X, goal.Y);
				continue;
			}
			o.X += dx / d * OrbSpeed;
			o.Y += dy / d * OrbSpeed;
			o.Travel += OrbSpeed;
			flying.Add(o);
		}
		bs.NjspgOrbs = flying;
	}

	private static void DrawOrbs(Board b, Graphics g)
	{
		if (b.mTavel == null || b.mTavel.NjspgOrbs.Count == 0)
		{
			return;
		}
		Image img = Tavel.LoadImage("tavel/njspg/orb.png");
		if (img == null)
		{
			return;
		}
		foreach (Orb o in b.mTavel.NjspgOrbs)
		{
			float shrink = Math.Min(1f, o.Travel / 120f);
			float size = OrbStartSize + (OrbEndSize - OrbStartSize) * shrink;
			float wobble = 1f + 0.06f * (float)Math.Sin((b.mMainCounter - o.Born) * 0.4);
			float w = size * wobble;
			float h = size / wobble;
			g.DrawImage(img, new TRect((int)Math.Round(o.X - w / 2f), (int)Math.Round(o.Y - h / 2f), (int)Math.Round(w), (int)Math.Round(h)), new TRect(0, 0, img.mWidth, img.mHeight));
		}
	}

	private static void OpenVortex(Board b, TavelBoard bs, float x, float y)
	{
		int now = b.mMainCounter;
		bs.NjspgVortexes.Add(new Vortex { X = x, Y = y, Start = now, Until = now + VortexDuration });
		GlobalStaticVars.gLawnApp.PlayFoley(FoleyType.Zombiesplash);
		GlobalStaticVars.gLawnApp.PlayFoley(FoleyType.ZombieEnteringWater);
	}

	private static void SetPulled(Zombie z, bool theOn)
	{
		if (theOn)
		{
			TavelHooks.ZState(z).FjthPulled = true;
		}
		else if (z.mTavel != null)
		{
			z.mTavel.FjthPulled = false;
		}
	}

	private static void Catch(Board b, Vortex v)
	{
		foreach (Zombie z in b.mZombies)
		{
			if (v.Held.ContainsKey(z) || (z.mTavel != null && z.mTavel.FjthPulled) || !CanTarget(z) || z.mZombieType == ZombieType.Boss || z.mZombieType == ZombieType.Bungee)
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			float cx = r.mX + r.mWidth / 2f;
			float feet = r.mY + r.mHeight;
			float reach = VortexReach(v, cx, feet - 20f);
			if (reach > 1f)
			{
				continue;
			}
			// shares 伏僵塔黄's "being pulled" mark
			SetPulled(z, theOn: true);
			if (z.mIsEating)
			{
				z.StopEating();
			}
			v.Held[z] = new Held
			{
				Zombie = z,
				Angle = (float)Math.Atan2((feet - 20f - v.Y) / VortexRadiusY, (cx - v.X) / VortexRadiusX),
				Reach = Math.Max(0.05f, reach),
				Giant = Tavel.IsGiantZombie(z),
				OffsetX = cx - z.mPosX,
				OffsetY = feet - 20f - z.mPosY
			};
		}
	}

	/// <summary>The row under (x, y) becomes the zombie's real row (never a row of another kind, pool against lawn).</summary>
	private static void MoveToRow(Board b, Zombie z, float x, float y)
	{
		(float left, float right) = LawnEdges(b, 0);
		int row = b.PixelToGridYKeepOnBoard((int)Math.Max(left + 1f, Math.Min(right - 1f, x)), (int)y);
		if (row == z.mRow || row < 0 || b.mPlantRow[row] != b.mPlantRow[z.mRow])
		{
			return;
		}
		z.SetRow(row);
	}

	private static void Spin(Board b, Vortex v)
	{
		foreach (Zombie z in v.Held.Keys.ToArray())
		{
			Held h = v.Held[z];
			if (!Tavel.Alive(z))
			{
				SetPulled(z, theOn: false);
				v.Held.Remove(z);
				continue;
			}
			if (h.Reach > 0f)
			{
				// spirals inward, turning faster near the middle
				h.Reach = Math.Max(0f, h.Reach - VortexPull);
				h.Angle += VortexTurn * (1f + 2f * (1f - h.Reach));
			}
			if (!h.Giant)
			{
				if (h.Reach <= VortexCore)
				{
					SetPulled(z, theOn: false);
					v.Held.Remove(z);
					z.DieNoLoot(false);
					continue;
				}
			}
			else if (h.Reach <= 0f && !h.Hit)
			{
				// a giant is only wounded, then stays in the middle until the whirlpool closes
				h.Hit = true;
				z.TakeDamage(VortexGiantDamage, 0u);
				if (!Tavel.Alive(z))
				{
					SetPulled(z, theOn: false);
					v.Held.Remove(z);
					continue;
				}
			}
			float x = v.X + (float)Math.Cos(h.Angle) * h.Reach * VortexRadiusX;
			float y = v.Y + (float)Math.Sin(h.Angle) * h.Reach * VortexRadiusY;
			MoveToRow(b, z, x, y);
			z.mPosX = x - h.OffsetX;
			z.mPosY = y - h.OffsetY;
			z.mX = (int)z.mPosX;
			z.mY = (int)z.mPosY;
			Freeze(z, 30);
		}
	}

	private static void Finish(Board b, Vortex v)
	{
		foreach (Held h in v.Held.Values)
		{
			Zombie z = h.Zombie;
			SetPulled(z, theOn: false);
			if (!Tavel.Alive(z))
			{
				continue;
			}
			if (!Tavel.IsGiantZombie(z))
			{
				z.DieNoLoot(false);
				continue;
			}
			// survivors are set down on the ground of the row they ended up in
			(float left, float right) = LawnEdges(b, Math.Max(0, z.mRow));
			z.mPosX = Math.Max(left - 40f, Math.Min(right, z.mPosX));
			z.mPosY = z.GetPosYBasedOnRow(z.mRow);
			z.mX = (int)z.mPosX;
			z.mY = (int)z.mPosY;
			if (!h.Hit)
			{
				z.TakeDamage(VortexGiantDamage, 0u);
			}
		}
		v.Held.Clear();
	}

	private static void UpdateVortexes(Board b, TavelBoard bs)
	{
		int now = b.mMainCounter;
		List<Vortex> open = new List<Vortex>();
		foreach (Vortex v in bs.NjspgVortexes)
		{
			if (v.Closing)
			{
				if (now < v.Until + VortexCloseTicks)
				{
					open.Add(v);
				}
				continue;
			}
			if (now >= v.Until)
			{
				Finish(b, v);
				v.Closing = true;
				GlobalStaticVars.gLawnApp.PlayFoley(FoleyType.Zombiesplash);
				open.Add(v);
				continue;
			}
			Catch(b, v);
			Spin(b, v);
			open.Add(v);
		}
		bs.NjspgVortexes = open;
	}

	/// <summary>Board.DrawBackdrop: whirlpools lie on the ground under plants and zombies.</summary>
	public static void DrawBackdrop(Board b, Graphics g)
	{
		if (b.mTavel == null || b.mTavel.NjspgVortexes.Count == 0)
		{
			return;
		}
		int now = b.mMainCounter;
		bool oldColorize = g.GetColorizeImages();
		Color oldColor = g.GetColor();
		g.SetColorizeImages(colorizeImages: true);
		try
		{
			foreach (Vortex v in b.mTavel.NjspgVortexes)
			{
				int age = now - v.Start;
				float size = Math.Min(1f, 0.15f + 0.85f * age / VortexOpenTicks);
				int alpha = 255;
				if (v.Closing)
				{
					float left = Math.Max(0f, Math.Min(1f, 1f - (now - v.Until) / (float)VortexCloseTicks));
					size *= 0.4f + 0.6f * left;
					alpha = (int)(255f * left);
				}
				g.SetColor(new Color(255, 255, 255, alpha));
				VortexSheet.Draw(g, age * VortexFps / Tps, v.X, v.Y, VortexRadiusX * size / 160f, VortexRadiusY * size / 160f);
			}
		}
		finally
		{
			g.SetColor(oldColor, premultiply: false);
			g.SetColorizeImages(oldColorize);
		}
	}

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
			if (!s.Acted && frame >= SaThrowFrame)
			{
				s.Acted = true;
				ThrowOrb(p);
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
				SpawnTornadoes(p, s);
			}
			if (frame >= AttackSheet.FrameCount)
			{
				ReturnToIdle(p, s);
				return;
			}
			s.Frame = frame;
			return;
		}
		// the next lash waits for her last tornado to leave the row
		int now = p.mBoard.mMainCounter;
		if (now < s.NextAttack || (p.mBoard.mTavel != null && p.mBoard.mTavel.NjspgTornadoes.Any((Tornado t) => t.Owner == p)))
		{
			return;
		}
		int dir = AttackDirection(p);
		if (dir == 0)
		{
			return;
		}
		s.Direction = dir;
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
		if (bs.NjspgTornadoes.Count > 0)
		{
			UpdateTornadoes(b, bs);
		}
		if (bs.NjspgOrbs.Count > 0)
		{
			UpdateOrbs(b, bs);
		}
		if (bs.NjspgVortexes.Count > 0)
		{
			UpdateVortexes(b, bs);
		}
	}
}
