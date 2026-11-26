// 球果训练家 (plant_qgxlj.py): three forms (tap her to switch) summoning a dinosaur, a jellyfish or a horse;
// the ultimate depends on the form (dinosaur stampede, laser, gathering formation).
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public sealed partial class TavelBoard
{
	public List<TavelQgxlj.Dino> QgxljDinos = new List<TavelQgxlj.Dino>();

	public List<TavelQgxlj.Jellyfish> QgxljJellyfish = new List<TavelQgxlj.Jellyfish>();

	public List<TavelQgxlj.Laser> QgxljLasers = new List<TavelQgxlj.Laser>();

	public List<TavelQgxlj.Horse> QgxljHorses = new List<TavelQgxlj.Horse>();

	public List<TavelQgxlj.Formation> QgxljFormations = new List<TavelQgxlj.Formation>();

	public int QgxljLastUpdate = -1;
}

public sealed partial class TavelShot
{
	/// <summary>A jellyfish spike: 80 damage and a chance to charm.</summary>
	public bool QgxljSpike;
}

public static class TavelQgxlj
{
	public const int Id = 105;

	public static SeedType Seed => (SeedType)Id;

	public const int AttackFrames = 80;

	public const int SummonFrame = 60;

	public const int DinoWait = 180;

	public const int DinoSwallowRest = 1200;

	public const float DinoStepDistance = 40f;

	public const int DinoStepTime = 15;

	public const int GiantDamage = 400;

	public const int UltimateDamage = 800;

	public const float UltimateSpeed = 4f;

	public const float BodyScale = 1.7f;

	public const float BodyX = -35f;

	public const float BodyY = -45f;

	public const int JellyfishLifetime = 1200;

	public const int JellyfishFireInterval = 120;

	public const int SpikeDamage = 80;

	public const int SpikeCharmChance = 20;

	public const int LaserLifetime = 720;

	public const int LaserTickInterval = 12;

	public const int LaserDamage = 200;

	public const int HorseDamage = 300;

	public const int HorseExplosionDamage = 700;

	public const float HorseSpeed = 1.25f;

	public const float HorseYOffset = 30f;

	public const int HorseEndWait = 120;

	public const int FormationLifetime = 900;

	public const int FormationStun = 240;

	public sealed class Dino
	{
		public Plant Plant;

		public int Row;

		public float X;

		public float GroundY;

		public bool Ultimate;

		public int WaitUntil;

		public float StepStart;

		public float StepEnd;

		public int StepAge = DinoStepTime;

		public HashSet<Zombie> Hit = new HashSet<Zombie>();

		public int NextGiantTick;

		public bool Dead;
	}

	public sealed class Jellyfish
	{
		public Plant Plant;

		public float X;

		public float Y;

		public int Born;

		public int Expires;

		public int NextFire;

		public bool Dead;
	}

	public sealed class Laser
	{
		public Zombie Target;

		public int Expires;

		public int NextTick;

		public float X;

		public float BottomY;

		public bool Dead;
	}

	public sealed class Horse
	{
		public Plant Plant;

		public int Row;

		public float X;

		public float GroundY;

		public HashSet<Zombie> Hit = new HashSet<Zombie>();

		public int EndWait = -1;

		public bool Dead;
	}

	public sealed class Formation
	{
		public float TargetX;

		public float CenterX;

		public float CenterY;

		public int Born;

		public int Expires;

		public HashSet<Zombie> Known = new HashSet<Zombie>();

		public bool Dead;
	}

	public sealed class State
	{
		public int Form = 1;

		public string Mode = "stand";

		public int AttackForm = 1;

		public bool Summoned;

		public Dino Dino;

		public Jellyfish Jellyfish;

		public Horse Horse;

		public int UltimateUntil;
	}

	private static TavelSpec sSpec;

	public static State Get(Plant p) => p.mTavel as State ?? (State)(p.mTavel = new State());

	// the Spine origin is at her feet: scaling keeps the planted point fixed
	private static void Align(Plant p)
	{
		Reanimation body = Tavel.Body(p);
		if (body != null)
		{
			body.OverrideScale(BodyScale, BodyScale);
			body.SetPosition(BodyX, BodyY);
		}
	}

	public static TavelSpec Spec()
	{
		sSpec = new TavelSpec(Id, "qgxlj")
		{
			Name = "球果训练家",
			Cost = 300,
			Card = "tavel/qgxlj/card.png",
			AlmanacText = "球果训练家会召唤不同的伙伴协助作战。\n{SHORTLINE}\n{KEYWORD}特点：{STAT}种植后点击植物可以切换不同形态\n{KEYWORD}大招：{STAT}依据不同形态产生不同效果\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}短\n{KEYWORD}阳光：{STAT}300\n{SHORTLINE}\n{FLAVOR}在山里，我常常能听到动物伙伴的声音，芝士恐龙，芝士水母，芝士马",
			DefName = "QGXLJ",
			DefCost = 300,
			LaunchRate = 90,
			ReanimFile = "tavel/spanim/qgxlj",
			CustomBody = true,
			Preview = new TavelPreview("anim_stand1", 0.72f, -35f, -88f, BodyScale, -35f + BodyX, -88f + BodyY),
			OnPlanted = delegate(Plant p, int x, int y)
			{
				Get(p);
				if (p.mBoard != null)
				{
					Tavel.StartPacketCooldown(p.mBoard, Seed);
				}
				Tavel.DropNativeBody(p);
				Tavel.CreateBody(p, sSpec.Reanim, 1, "stand1");
				Align(p);
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
					UpdateBoard(p.mBoard);
				}
				Align(p);
			},
			OnDie = Cleanup,
			OnSquished = delegate(Plant p, object s)
			{
				Cleanup(p, s as State);
			},
			OnClick = delegate(Plant p)
			{
				State s = Get(p);
				s.Form = s.Form % 3 + 1;
				if (s.Mode != "attack")
				{
					Tavel.PlayAnim(p, "stand" + s.Form, ReanimLoopType.Loop, 30f);
				}
			},
			CanUltimate = (Plant p) => true,
			UltimateActive = (Plant p) => p.mTavel is State s && s.UltimateUntil > p.mBoard.mMainCounter,
			StartUltimate = StartUltimate,
			UpdateBoard = UpdateBoard,
			DrawBoard = DrawBoard
		};
		return sSpec;
	}

	private static void Cleanup(Plant p) => Cleanup(p, p.mTavel as State);

	private static void Cleanup(Plant p, State s)
	{
		if (p.mTavel is State)
		{
			p.mTavel = null;
		}
		if (s == null)
		{
			return;
		}
		if (s.Dino != null)
		{
			s.Dino.Dead = true;
		}
		if (s.Jellyfish != null)
		{
			s.Jellyfish.Dead = true;
		}
		if (s.Horse != null)
		{
			s.Horse.Dead = true;
		}
	}

	private static bool HasTarget(Plant p)
	{
		if (Get(p).Form == 2)
		{
			return CattailTarget(p.mBoard, p.mX + 40, p.mY + 40) != null;
		}
		return p.FindTargetZombie(p.mRow, PlantWeapon.Primary) != null;
	}

	private static Dino AddDino(Plant p, bool theUltimate, int theRow = -1)
	{
		if (theRow < 0)
		{
			theRow = p.mRow;
		}
		int col = theUltimate ? 0 : p.mPlantCol;
		float x = p.mBoard.GridToPixelX(col, theRow) + (theUltimate ? 40 : 60);
		Dino d = new Dino
		{
			Plant = p,
			Row = theRow,
			X = x,
			GroundY = p.mBoard.GetPosYBasedOnRow(x, theRow) + 20,
			Ultimate = theUltimate,
			WaitUntil = theUltimate ? 0 : p.mBoard.mMainCounter + DinoWait,
			StepStart = x,
			StepEnd = x,
			NextGiantTick = p.mBoard.mMainCounter
		};
		Tavel.State(p.mBoard).QgxljDinos.Add(d);
		return d;
	}

	private static Zombie CattailTarget(Board b, float theX, float theY)
	{
		Zombie target = null;
		float best = 1e9f;
		foreach (Zombie z in b.mZombies)
		{
			if (!Tavel.Alive(z) || z.mMindControlled || !z.mHasHead)
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			float cx = r.mX + r.mWidth * 0.5f;
			float cy = r.mY + r.mHeight * 0.5f;
			float score = (cx - theX) * (cx - theX) + (cy - theY) * (cy - theY);
			if (z.IsFlying())
			{
				score -= 1000000f;
			}
			if (target == null || score < best)
			{
				target = z;
				best = score;
			}
		}
		return target;
	}

	private static void FireSpike(Board b, Jellyfish j)
	{
		Zombie target = CattailTarget(b, j.X, j.Y);
		if (target == null)
		{
			return;
		}
		Projectile pr = b.AddProjectile((int)j.X, (int)j.Y, j.Plant.mRenderOrder + 2, target.mRow, ProjectileType.CattailSpike);
		pr.mDamageRangeFlags = j.Plant.GetDamageRangeFlags(PlantWeapon.Primary);
		pr.mFromPlant = Seed;
		pr.mMotionType = ProjectileMotion.Homing;
		pr.mVelX = 2f;
		pr.mTargetZombieID = b.ZombieGetID(target);
		TavelHooks.Shot(pr).QgxljSpike = true;
	}

	/// <summary>Projectile.DoImpact of a jellyfish spike: 80 in all, sometimes charms (true: handled).</summary>
	public static bool Impact(Projectile pr, TavelShot s, Zombie z)
	{
		if (!s.QgxljSpike)
		{
			return false;
		}
		s.QgxljSpike = false;
		// the game's spike deals 20 with its own impact
		pr.DoImpactCore(z);
		if (Tavel.Alive(z))
		{
			z.TakeDamage(SpikeDamage - 20, pr.GetDamageFlags(z));
			if (Tavel.Alive(z) && !Tavel.IsBoss(z) && Tavel.Rand(1, 100) <= SpikeCharmChance)
			{
				z.ApplyMindControl();
			}
		}
		return true;
	}

	private static Jellyfish AddJellyfish(Plant p)
	{
		float x = p.mBoard.GridToPixelX(p.mPlantCol, p.mRow) + 95;
		Jellyfish j = new Jellyfish
		{
			Plant = p,
			X = x,
			Y = p.mBoard.GetPosYBasedOnRow(x, p.mRow) - 15,
			Born = p.mBoard.mMainCounter,
			Expires = p.mBoard.mMainCounter + JellyfishLifetime,
			NextFire = p.mBoard.mMainCounter
		};
		Tavel.State(p.mBoard).QgxljJellyfish.Add(j);
		return j;
	}

	private static Zombie LaserTarget(Board b)
	{
		Zombie target = null;
		float left = 1000000f;
		foreach (Zombie z in b.mZombies)
		{
			if (!Tavel.Alive(z) || z.mMindControlled)
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			if (r.mX < left)
			{
				target = z;
				left = r.mX;
			}
		}
		return target;
	}

	private static Horse AddHorse(Plant p)
	{
		float x = p.mBoard.GridToPixelX(p.mPlantCol, p.mRow) + 75;
		Horse h = new Horse { Plant = p, Row = p.mRow, X = x, GroundY = p.mBoard.GetPosYBasedOnRow(x, p.mRow) + 55 };
		Tavel.State(p.mBoard).QgxljHorses.Add(h);
		return h;
	}

	private static void StartUltimate(Plant p)
	{
		State s = Get(p);
		s.UltimateUntil = p.mBoard.mMainCounter + 180;
		Board b = p.mBoard;
		TavelBoard bs = Tavel.State(b);
		if (s.Form == 3)
		{
			const int row = 2;
			float tx = b.GridToPixelX(8, row);
			float cx = tx + 40f;
			Formation f = new Formation { TargetX = tx, CenterX = cx, CenterY = b.GetPosYBasedOnRow(cx, row) + 35, Born = b.mMainCounter, Expires = b.mMainCounter + FormationLifetime };
			f.Known.UnionWith(b.mZombies);
			bs.QgxljFormations.Add(f);
			return;
		}
		if (s.Form == 2)
		{
			bs.QgxljLasers.Add(new Laser { Target = LaserTarget(b), Expires = b.mMainCounter + LaserLifetime, NextTick = b.mMainCounter, X = p.mX + 80, BottomY = p.mY + 80 });
			return;
		}
		int rows = b.StageHas6Rows() ? 6 : 5;
		for (int r = 0; r < rows; r++)
		{
			AddDino(p, theUltimate: true, r);
		}
	}

	private static void Update(Plant p)
	{
		State s = Get(p);
		float t = Tavel.Body(p) == null ? 1f : Tavel.AnimTime(p);
		if (s.Mode == "attack")
		{
			int frame = (int)(t * AttackFrames + 0.5f);
			if (!s.Summoned && frame >= SummonFrame)
			{
				if (s.AttackForm == 3)
				{
					s.Horse = AddHorse(p);
				}
				else if (s.AttackForm == 2)
				{
					s.Jellyfish = AddJellyfish(p);
				}
				else
				{
					s.Dino = AddDino(p, theUltimate: false);
				}
				s.Summoned = true;
			}
			if (t >= 0.999f)
			{
				s.Mode = s.AttackForm == 3 ? "waiting_horse" : s.AttackForm == 2 ? "waiting_jellyfish" : "waiting_dino";
				Tavel.PlayAnim(p, "stand" + s.Form, ReanimLoopType.Loop, 30f);
			}
			return;
		}
		if (s.Mode.StartsWith("waiting_", StringComparison.Ordinal))
		{
			return;
		}
		if (HasTarget(p))
		{
			s.Mode = "attack";
			s.AttackForm = s.Form;
			s.Summoned = false;
			Tavel.PlayAnim(p, "attack" + s.AttackForm, ReanimLoopType.PlayOnceAndHold, 30f);
		}
	}

	/// <summary>Summons keep moving after the trainer is gone (once per tick, from the board and from her).</summary>
	private static void UpdateBoard(Board b)
	{
		TavelBoard bs = b.mTavel;
		if (bs == null || bs.QgxljLastUpdate == b.mMainCounter)
		{
			return;
		}
		bs.QgxljLastUpdate = b.mMainCounter;
		UpdateDinos(b, bs);
		UpdateJellyfish(b, bs);
		UpdateLasers(b, bs);
		UpdateHorses(b, bs);
		UpdateFormations(b, bs);
	}

	private static void BackToStand(Plant p, Func<State, bool> theOwns, Action<State> theClear)
	{
		if (p?.mTavel is State s && theOwns(s))
		{
			theClear(s);
			s.Mode = "stand";
		}
	}

	private static void UpdateDinos(Board b, TavelBoard bs)
	{
		float rightEdge = b.GridToPixelX(9, 0) + 120;
		bool shake = false;
		foreach (Dino d in bs.QgxljDinos.ToArray())
		{
			if (d.Dead)
			{
				continue;
			}
			Plant p = d.Plant;
			// stampede dinosaurs finish their run even if the trainer is gone
			if (!d.Ultimate && (p == null || p.mDead || p.mSquished))
			{
				d.Dead = true;
				continue;
			}
			if (d.Ultimate)
			{
				d.X += UltimateSpeed;
				d.GroundY = b.GetPosYBasedOnRow(d.X, d.Row) + 20;
				foreach (Zombie z in b.mZombies.ToArray())
				{
					if (d.Hit.Contains(z) || !Tavel.Alive(z) || z.mRow != d.Row)
					{
						continue;
					}
					TRect r = z.GetZombieRect();
					if (r.mX - 30 <= d.X && d.X <= r.mX + r.mWidth + 30)
					{
						z.TakeDamage(UltimateDamage, 0u);
						d.Hit.Add(z);
					}
				}
				shake = true;
			}
			else if (b.mMainCounter >= d.WaitUntil)
			{
				if (d.StepAge >= DinoStepTime)
				{
					Zombie target = DinoTarget(b, d);
					if (target != null)
					{
						if (Tavel.IsGiantZombie(target))
						{
							if (b.mMainCounter >= d.NextGiantTick)
							{
								target.TakeDamage(GiantDamage, 0u);
								d.NextGiantTick = b.mMainCounter + 60;
							}
							continue;
						}
						target.DieNoLoot(false);
						d.WaitUntil = b.mMainCounter + DinoSwallowRest;
						continue;
					}
					d.StepStart = d.X;
					d.StepEnd = d.X + DinoStepDistance;
					d.StepAge = 0;
				}
				d.StepAge++;
				float progress = Math.Min(1f, d.StepAge / (float)DinoStepTime);
				d.X = d.StepStart + (d.StepEnd - d.StepStart) * progress;
				d.GroundY = b.GetPosYBasedOnRow(d.X, d.Row) + 20;
			}
			if (d.X > rightEdge)
			{
				d.Dead = true;
				if (!d.Ultimate)
				{
					BackToStand(p, (State s) => s.Dino == d, delegate(State s)
					{
						s.Dino = null;
					});
				}
			}
		}
		if (shake)
		{
			b.ShakeBoard(1, 4);
		}
		bs.QgxljDinos.RemoveAll((Dino d) => d.Dead);
	}

	private static Zombie DinoTarget(Board b, Dino d)
	{
		float front = d.X + 80f;
		Zombie best = null;
		float bestX = 1000000f;
		foreach (Zombie z in b.mZombies)
		{
			if (!Tavel.Alive(z) || Tavel.IsBoss(z) || z.mRow != d.Row)
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			float c = r.mX + r.mWidth * 0.5f;
			if (d.X <= c && c <= front && c < bestX)
			{
				best = z;
				bestX = c;
			}
		}
		return best;
	}

	private static void UpdateJellyfish(Board b, TavelBoard bs)
	{
		foreach (Jellyfish j in bs.QgxljJellyfish.ToArray())
		{
			if (j.Dead)
			{
				continue;
			}
			Plant p = j.Plant;
			if (p == null || p.mDead || p.mSquished)
			{
				j.Dead = true;
				continue;
			}
			int frame = b.mMainCounter;
			if (frame >= j.Expires)
			{
				j.Dead = true;
				BackToStand(p, (State s) => s.Jellyfish == j, delegate(State s)
				{
					s.Jellyfish = null;
				});
				continue;
			}
			if (frame >= j.NextFire)
			{
				FireSpike(b, j);
				FireSpike(b, j);
				j.NextFire = frame + JellyfishFireInterval;
			}
		}
		bs.QgxljJellyfish.RemoveAll((Jellyfish j) => j.Dead);
	}

	private static void UpdateLasers(Board b, TavelBoard bs)
	{
		foreach (Laser l in bs.QgxljLasers)
		{
			if (l.Dead)
			{
				continue;
			}
			int frame = b.mMainCounter;
			if (frame >= l.Expires)
			{
				l.Dead = true;
				continue;
			}
			Zombie target = l.Target;
			if (!Tavel.Alive(target) || target.mMindControlled)
			{
				target = l.Target = LaserTarget(b);
			}
			if (target == null)
			{
				continue;
			}
			TRect r = target.GetZombieRect();
			l.X = r.mX + r.mWidth * 0.5f;
			l.BottomY = r.mY + r.mHeight * 0.72f;
			if (frame < l.NextTick)
			{
				continue;
			}
			l.NextTick = frame + LaserTickInterval;
			const float half = 42f;
			foreach (Zombie z in b.mZombies.ToArray())
			{
				if (!Tavel.Alive(z) || z.mMindControlled)
				{
					continue;
				}
				TRect zr = z.GetZombieRect();
				if (zr.mX <= l.X + half && zr.mX + zr.mWidth >= l.X - half)
				{
					z.TakeDamage(LaserDamage, 0u);
				}
			}
		}
		bs.QgxljLasers.RemoveAll((Laser l) => l.Dead);
	}

	private static void UpdateHorses(Board b, TavelBoard bs)
	{
		float rightX = b.GridToPixelX(8, 0) + 40;
		foreach (Horse h in bs.QgxljHorses.ToArray())
		{
			if (h.Dead)
			{
				continue;
			}
			Plant p = h.Plant;
			if (p == null || p.mDead || p.mSquished)
			{
				h.Dead = true;
				continue;
			}
			if (h.EndWait < 0)
			{
				h.X = Math.Min(rightX, h.X + HorseSpeed);
				h.GroundY = b.GetPosYBasedOnRow(h.X, h.Row) + 55;
				foreach (Zombie z in b.mZombies.ToArray())
				{
					if (h.Hit.Contains(z) || !Tavel.Alive(z) || z.mRow != h.Row)
					{
						continue;
					}
					TRect r = z.GetZombieRect();
					if (r.mX - 35 <= h.X && h.X <= r.mX + r.mWidth + 35)
					{
						z.TakeDamage(HorseDamage, 0u);
						h.Hit.Add(z);
					}
				}
				if (h.X >= rightX)
				{
					h.EndWait = b.mMainCounter + HorseEndWait;
				}
			}
			else if (b.mMainCounter >= h.EndWait)
			{
				foreach (Zombie z in b.mZombies.ToArray())
				{
					if (!Tavel.Alive(z) || Math.Abs(z.mRow - h.Row) > 1)
					{
						continue;
					}
					TRect r = z.GetZombieRect();
					if (Math.Abs(r.mX + r.mWidth * 0.5f - h.X) <= 120f)
					{
						z.TakeDamage(HorseExplosionDamage, 0u);
					}
				}
				h.Dead = true;
				BackToStand(p, (State s) => s.Horse == h, delegate(State s)
				{
					s.Horse = null;
				});
			}
		}
		bs.QgxljHorses.RemoveAll((Horse h) => h.Dead);
	}

	private static void UpdateFormations(Board b, TavelBoard bs)
	{
		foreach (Formation f in bs.QgxljFormations)
		{
			if (f.Dead)
			{
				continue;
			}
			if (b.mMainCounter >= f.Expires)
			{
				f.Dead = true;
				continue;
			}
			foreach (Zombie z in b.mZombies.ToArray())
			{
				int row = Math.Max(0, Math.Min(4, z.mRow));
				bool outside = z.mPosX > b.GridToPixelX(8, row) + 80;
				if (f.Known.Contains(z) && !outside)
				{
					continue;
				}
				f.Known.Add(z);
				if (!Tavel.Alive(z) || Tavel.IsBoss(z) || Tavel.IsGiantZombie(z))
				{
					continue;
				}
				if (z.mRow != 2)
				{
					b.ZombieSwitchRow(z, 2);
					z.SetRow(2);
				}
				// mPosX is the zombie's anchor; roof height only after x and row are final
				z.mPosX = f.TargetX;
				z.mPosY = b.GetPosYBasedOnRow(z.mPosX + 40f, 2) - 30f;
				z.mX = (int)z.mPosX;
				z.mY = (int)z.mPosY;
				z.mOnHighGround = z.IsOnHighGround();
				z.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Zombie, 2, 4);
				z.ApplyButter();
				z.mButteredCounter = FormationStun;
			}
		}
		bs.QgxljFormations.RemoveAll((Formation f) => f.Dead);
	}

	/// <summary>Board.DrawBackdrop: the formation circle lies on the lawn under the zombies.</summary>
	public static void DrawFormations(Board b, Graphics g)
	{
		if (b.mTavel == null || b.mTavel.QgxljFormations.Count == 0)
		{
			return;
		}
		Image img = Tavel.LoadImage("tavel/qgxlj/formation.png");
		if (img == null)
		{
			return;
		}
		foreach (Formation f in b.mTavel.QgxljFormations)
		{
			if (f.Dead)
			{
				continue;
			}
			const float d = 160f;
			Tavel.Tinted(g, new Color(255, 255, 255, 150), delegate
			{
				TodCommon.TodDrawImageScaledF(g, img, f.CenterX - d * 0.5f, f.CenterY - d * 0.5f, d / img.mWidth, d / img.mHeight);
			});
		}
	}

	private static void DrawBoard(Board b, Graphics g)
	{
		TavelBoard bs = b.mTavel;
		if (bs == null)
		{
			return;
		}
		foreach (Dino d in bs.QgxljDinos)
		{
			if (d.Dead)
			{
				continue;
			}
			Image img = Tavel.LoadImage(d.Ultimate ? "tavel/qgxlj/ultimate_dino.png" : "tavel/qgxlj/dino.png");
			if (img == null)
			{
				continue;
			}
			float scale = d.Ultimate ? 0.18f : 0.16f;
			float yOff = d.Ultimate ? 64f : 80f;
			float period = d.Ultimate ? 12f : 18f;
			float bounce = Math.Abs((float)Math.Sin(b.mMainCounter * Math.PI / period)) * 16f;
			TodCommon.TodDrawImageScaledF(g, img, d.X - img.mWidth * scale * 0.5f, d.GroundY + yOff - bounce - img.mHeight * scale, scale, scale);
		}
		Image jelly = bs.QgxljJellyfish.Count > 0 ? Tavel.LoadImage("tavel/qgxlj/jellyfish.png") : null;
		if (jelly != null)
		{
			foreach (Jellyfish j in bs.QgxljJellyfish)
			{
				if (!j.Dead)
				{
					const float k = 0.18f;
					float bob = (float)Math.Sin((b.mMainCounter - j.Born) * Math.PI / 45.0) * 10f;
					TodCommon.TodDrawImageScaledF(g, jelly, j.X - jelly.mWidth * k * 0.5f, j.Y + bob - jelly.mHeight * k * 0.5f, k, k);
				}
			}
		}
		Image laser = bs.QgxljLasers.Count > 0 ? Tavel.LoadImage("tavel/qgxlj/laser.png") : null;
		if (laser != null)
		{
			foreach (Laser l in bs.QgxljLasers)
			{
				if (!l.Dead && l.Target != null)
				{
					const float top = -140f;
					float h = Math.Max(1f, l.BottomY - top);
					const float sx = 0.42f;
					TodCommon.TodDrawImageScaledF(g, laser, l.X - laser.mWidth * sx * 0.5f, top, sx, h / laser.mHeight);
				}
			}
		}
		Image horse = bs.QgxljHorses.Count > 0 ? Tavel.LoadImage("tavel/qgxlj/horse.png") : null;
		if (horse != null)
		{
			foreach (Horse h in bs.QgxljHorses)
			{
				if (!h.Dead)
				{
					const float k = 0.18f;
					float bob = (float)Math.Sin(b.mMainCounter * Math.PI / 90.0) * 8f;
					TodCommon.TodDrawImageScaledF(g, horse, h.X - horse.mWidth * k * 0.5f, h.GroundY + HorseYOffset - bob - horse.mHeight * k, k, k);
				}
			}
		}
	}
}
