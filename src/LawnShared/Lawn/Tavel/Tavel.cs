// 塔维尔的植物娘: the plant table, the 选项 switch, definitions, pictures, the shared ultimate and the 下一波 button.
// Port of nineplants/registry.py + base.py (塔维尔's PC pack, commissioned port). Mod edition only.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public static partial class Tavel
{
	/// <summary>First seed number of the pack (the game's own end at 90).</summary>
	public const int FirstSeed = 101;

	/// <summary>Seed chooser / almanac cell where the pack's cards start (a new row after the Imitater).</summary>
	public const int FirstChooserCell = 56;

	public const int TicksPerSecond = 100;

	public const int UltimateChargeTime = 1800;

	public const int UltimateMaxStock = 5;

	public const int NextWaveButtonId = 9100;

	/// <summary>All specs, in the pack's order (board effects are updated and drawn in it).</summary>
	public static readonly List<TavelSpec> Plants = new List<TavelSpec>();

	private static readonly Dictionary<int, TavelSpec> sById = new Dictionary<int, TavelSpec>();

	private static readonly List<TavelSpec> sClaiming = new List<TavelSpec>();

	private static bool sRegistered;

	private static bool? sEnabled;

	/// <summary>The pack is in this build (Mod edition) and its definitions are registered.</summary>
	public static bool Loaded => sRegistered;

	private static string SettingPath => Path.Combine(DormPlatform.ModSaveDir, "tavel_plants.txt");

	/// <summary>选项 → 塔维尔的植物娘: the plants can be picked (plants already on a saved lawn keep working either way).</summary>
	public static bool Enabled
	{
		get
		{
			if (!sEnabled.HasValue)
			{
				try
				{
					sEnabled = File.Exists(SettingPath) && File.ReadAllText(SettingPath).Trim() == "1";
				}
				catch
				{
					sEnabled = false;
				}
			}
			return sRegistered && sEnabled.Value;
		}
		set
		{
			sEnabled = value;
			sChooserSeeds = null;
			try
			{
				Directory.CreateDirectory(DormPlatform.ModSaveDir);
				File.WriteAllText(SettingPath, value ? "1" : "0");
			}
			catch (Exception ex)
			{
				Log("setting: " + ex.Message);
			}
		}
	}

	public static void Log(string theText)
	{
		Console.Error.WriteLine("Tavel: " + theText);
	}

	// ------------------------------------------------------------------ table

	private static void Add(TavelSpec theSpec)
	{
		Plants.Add(theSpec);
		sById[theSpec.SeedId] = theSpec;
		if (theSpec.Claims != null)
		{
			sClaiming.Add(theSpec);
		}
	}

	public static TavelSpec Get(SeedType theSeed)
	{
		return (int)theSeed >= FirstSeed && sById.TryGetValue((int)theSeed, out TavelSpec s) ? s : null;
	}

	public static bool IsCustom(SeedType theSeed) => Get(theSeed) != null;

	/// <summary>Like Get, but also finds game plants adopted by a pack plant.</summary>
	public static TavelSpec GetFor(Plant thePlant)
	{
		TavelSpec s = Get(thePlant.mSeedType);
		if (s == null && sClaiming.Count > 0 && thePlant.mTavel != null)
		{
			foreach (TavelSpec c in sClaiming)
			{
				if (c.Claims(thePlant))
				{
					return c;
				}
			}
		}
		return s;
	}

	/// <summary>A seed number in the pack's range (the plant may be unknown to this version).</summary>
	public static bool IsCustomNumber(int theSeed) => theSeed >= FirstSeed && theSeed < FirstSeed + 1000;

	public static int MaxSeed => Plants.Count == 0 ? (int)SeedType.SeedTypeCount - 1 : Plants.Max((TavelSpec p) => p.SeedId);

	public static TavelSpec EffectiveSpec(SeedType theSeed, SeedType theImitater)
	{
		return Get(theSeed) ?? (theSeed == SeedType.Imitater ? Get(theImitater) : null);
	}

	// ------------------------------------------------------------------ chooser

	private static List<TavelSpec> sChooserSeeds;

	/// <summary>The cards in the chooser and the almanac: the selectable plants, while the switch is on.</summary>
	public static List<TavelSpec> ChooserPlants => sChooserSeeds ??= Enabled ? Plants.Where((TavelSpec p) => p.Selectable).ToList() : new List<TavelSpec>();

	public static int Count => ChooserPlants.Count;

	public static int ChooserCell(SeedType theSeed)
	{
		int k = ChooserPlants.FindIndex((TavelSpec p) => p.Seed == theSeed);
		return k < 0 ? -1 : FirstChooserCell + k;
	}

	public static SeedType SeedAtCell(int theCell)
	{
		int k = theCell - FirstChooserCell;
		return k >= 0 && k < ChooserPlants.Count ? ChooserPlants[k].Seed : SeedType.None;
	}

	public static bool HasSeed(SeedType theSeed)
	{
		TavelSpec s = Get(theSeed);
		return s != null && s.Selectable && Enabled;
	}

	public static List<SeedType> ConveyorSeeds()
	{
		return Enabled ? Plants.Where((TavelSpec p) => p.Selectable && p.Conveyor).Select((TavelSpec p) => p.Seed).ToList() : new List<SeedType>();
	}

	public static int GetCost(SeedType theSeed, SeedType theImitater, Board theBoard)
	{
		foreach (SeedType s in new[] { theSeed, theImitater })
		{
			TavelSpec spec = Get(s);
			if (spec != null && spec.Cost >= 0)
			{
				return spec.CostFor != null ? spec.CostFor(theBoard) : spec.Cost;
			}
		}
		return 0;
	}

	public static int GetRefreshTime(SeedType theSeed, SeedType theImitater)
	{
		TavelSpec spec = EffectiveSpec(theSeed, theImitater);
		return spec?.RefreshTime ?? 750;
	}

	// ------------------------------------------------------------------ registration

	/// <summary>The pack's plants in its own registration order.</summary>
	private static void DefinePlants()
	{
		Add(TavelSuperGatling.Spec());
		Add(TavelFjth.Spec());
		Add(TavelDmxj.Spec());
		Add(TavelSsbps.Spec());
		Add(TavelQgxlj.Spec());
		Add(TavelNbjqs.Spec());
		Add(TavelCzlb.Spec());
		Add(TavelHjbl.Spec());
		foreach (TavelSpec s in TavelSwg.Specs())
		{
			Add(s);
		}
		Add(TavelTclc.Spec());
		Add(TavelNjspg.Spec());
		Add(TavelDmdf.Spec());
	}

	/// <summary>Plant definitions and Spine bodies for the pack; right after the game's reanimation table is loaded.</summary>
	public static void Register()
	{
		if (!sRegistered)
		{
			sRegistered = true;
			DefinePlants();
		}
		RegisterDefinitions();
		Log(Plants.Count + " plants, switch " + (Enabled ? "on" : "off"));
	}

	private static void RegisterDefinitions()
	{
		PlantDefinition[] defs = GameConstants.gPlantDefs;
		if (defs.Length <= MaxSeed)
		{
			Array.Resize(ref defs, MaxSeed + 1);
			GameConstants.gPlantDefs = defs;
		}
		ReanimationParams[] reanims = GameConstants.gLawnReanimationArray;
		int nextReanim = 199;
		foreach (TavelSpec p in Plants.Where((TavelSpec q) => q.ReanimFile != null))
		{
			if (p.Reanim == ReanimationType.None)
			{
				p.Reanim = (ReanimationType)nextReanim;
			}
			nextReanim = Math.Max(nextReanim, (int)p.Reanim + 1);
		}
		if (reanims.Length < nextReanim)
		{
			Array.Resize(ref reanims, nextReanim);
			GameConstants.gLawnReanimationArray = reanims;
		}
		foreach (TavelSpec p in Plants.Where((TavelSpec q) => q.ReanimFile != null))
		{
			reanims[(int)p.Reanim] = new ReanimationParams(p.Reanim, p.ReanimFile, 0, isSpine: true, null, 14.3f);
		}
		ReanimatorXnaHelpers.gReanimationParamArray = reanims;
		if (ReanimatorXnaHelpers.gReanimatorDefArray.Length < nextReanim)
		{
			ReanimatorDefinition[] d = ReanimatorXnaHelpers.gReanimatorDefArray;
			Array.Resize(ref d, nextReanim);
			ReanimatorXnaHelpers.gReanimatorDefArray = d;
		}
		foreach (TavelSpec p in Plants)
		{
			// sprite-drawn plants have no body of their own; the Peashooter's gives the game something valid to manage
			ReanimationType body = p.ReanimFile != null ? p.Reanim : ReanimationType.Peashooter;
			defs[p.SeedId] = new PlantDefinition(p.Seed, null, body, 54, p.DefCost, p.DefRefresh, PlantSubClass.Normal, p.LaunchRate, p.DefName);
		}
		PlantDefinition filler = defs[0];
		for (int i = 0; i < defs.Length; i++)
		{
			if (defs[i] == null)
			{
				defs[i] = new PlantDefinition(SeedType.None, null, filler.mReanimationType, 0, 0, 0, PlantSubClass.Normal, 0, "UNUSED_CUSTOM_SEED_SLOT");
			}
		}
		// the Spine bodies load when first used (ReanimationInitializeType / the save loader ensure them)
	}

	/// <summary>All reanimation types of the pack (the save game keeps them by number).</summary>
	public static IEnumerable<ReanimationType> ReanimTypes => Plants.Where((TavelSpec p) => p.ReanimFile != null).Select((TavelSpec p) => p.Reanim);

	// ------------------------------------------------------------------ pictures

	private static readonly Dictionary<string, Image> sImages = new Dictionary<string, Image>();

	/// <summary>A picture of the pack ("tavel/..." content path, premultiplied), loaded once when first drawn.</summary>
	public static Image LoadImage(string thePath)
	{
		if (thePath == null)
		{
			return null;
		}
		if (!sImages.TryGetValue(thePath, out Image img))
		{
			try
			{
				img = new Image(DormPlatform.LoadTexture("Content/" + thePath));
			}
			catch (Exception ex)
			{
				Log("image " + thePath + ": " + ex.Message);
				img = null;
			}
			sImages[thePath] = img;
		}
		return img;
	}

	/// <summary>Frees the big animation sheets (several hundred MB together) when a lawn is left; they load again when drawn.</summary>
	public static void ReleaseLargeImages()
	{
		foreach (string key in sImages.Keys.ToList())
		{
			Image img = sImages[key];
			if (img != null && (long)img.mWidth * img.mHeight > 1000000L)
			{
				sImages.Remove(key);
				try
				{
					img.Texture?.Dispose();
				}
				catch (Exception ex)
				{
					Log("release " + key + ": " + ex.Message);
				}
			}
		}
	}

	/// <summary>Draws a pack plant's card fitted (and centred) into a packet cel.</summary>
	public static bool DrawCard(Graphics g, SeedType theSeed, float theX, float theY, float theWidth, float theHeight)
	{
		Image card = LoadImage(Get(theSeed)?.Card);
		if (card == null)
		{
			return false;
		}
		float k = Math.Min(theWidth / card.mWidth, theHeight / card.mHeight);
		float w = card.mWidth * k;
		float h = card.mHeight * k;
		TodCommon.TodDrawImageScaledF(g, card, theX + (theWidth - w) / 2f, theY + (theHeight - h) / 2f, k, k);
		return true;
	}

	private static readonly Dictionary<int, Reanimation> sSeedPreviews = new Dictionary<int, Reanimation>();

	private static readonly Dictionary<int, Reanimation> sCursorPreviews = new Dictionary<int, Reanimation>();

	private static Reanimation PreviewReanim(Dictionary<int, Reanimation> theCache, TavelSpec theSpec)
	{
		if (!theCache.TryGetValue(theSpec.SeedId, out Reanimation r))
		{
			r = Reanimation.GetNewReanimation();
			r.ReanimationInitializeType(0f, 0f, theSpec.Reanim);
			r.PlayReanim(theSpec.Preview.Animation, ReanimLoopType.Loop, 0, 30f);
			theCache[theSpec.SeedId] = r;
		}
		return r;
	}

	/// <summary>The plant pictured where the game draws a plant that is not on the lawn (almanac, previews).</summary>
	public static bool DrawSeedType(Graphics g, SeedType theSeed, SeedType theImitater, float thePosX, float thePosY)
	{
		TavelSpec spec = EffectiveSpec(theSeed, theImitater);
		if (spec == null)
		{
			return false;
		}
		if (spec.DrawSeed != null)
		{
			spec.DrawSeed(g, thePosX, thePosY);
			return true;
		}
		if (spec.Preview == null || spec.Reanim == ReanimationType.None)
		{
			return true;
		}
		Reanimation r = PreviewReanim(sSeedPreviews, spec);
		r.Update();
		r.OverrideScale(spec.Preview.SeedScale * g.mScaleX, spec.Preview.SeedScale * g.mScaleY);
		r.SetPosition(thePosX + spec.Preview.SeedOffset.X * g.mScaleX, thePosY + spec.Preview.SeedOffset.Y * g.mScaleY);
		r.Draw(g);
		return true;
	}

	/// <summary>The see-through plant over the cell the cursor is on.</summary>
	public static bool DrawCursorPreview(Graphics g, SeedType theSeed, SeedType theImitater)
	{
		TavelSpec spec = EffectiveSpec(theSeed, theImitater);
		if (spec == null)
		{
			return false;
		}
		bool oldColorize = g.GetColorizeImages();
		Color oldColor = g.GetColor();
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new Color(255, 255, 255, 128));
		try
		{
			if (spec.DrawCursor != null)
			{
				spec.DrawCursor(g);
			}
			else if (spec.Preview != null && spec.Reanim != ReanimationType.None)
			{
				Reanimation r = PreviewReanim(sCursorPreviews, spec);
				r.Update();
				r.OverrideScale(spec.Preview.CursorScale, spec.Preview.CursorScale);
				r.SetPosition(spec.Preview.CursorOffset.X * Constants.S, spec.Preview.CursorOffset.Y * Constants.S);
				r.Draw(g);
			}
		}
		finally
		{
			g.SetColor(oldColor, premultiply: false);
			g.SetColorizeImages(oldColorize);
		}
		return true;
	}

	// ------------------------------------------------------------------ helpers shared by the plants

	public static void StartPacketCooldown(Board theBoard, SeedType theSeed)
	{
		SeedBank bank = theBoard?.mSeedBank;
		if (bank == null)
		{
			return;
		}
		for (int i = 0; i < bank.mNumPackets; i++)
		{
			SeedPacket p = bank.mSeedPackets[i];
			if (p.mPacketType != theSeed)
			{
				continue;
			}
			p.mRefreshing = true;
			p.mRefreshTime = GetRefreshTime(theSeed, SeedType.None);
			p.mRefreshCounter = 0;
			p.mActive = false;
			return;
		}
	}

	/// <summary>Removes the stand-in body a custom-body plant was initialised with.</summary>
	public static void DropNativeBody(Plant thePlant)
	{
		thePlant.mApp.ReanimationTryToGet(thePlant.mBodyReanimID)?.ReanimationDie();
		thePlant.mBodyReanimID = null;
	}

	public static Reanimation Body(Plant thePlant) => thePlant.mApp.ReanimationTryToGet(thePlant.mBodyReanimID);

	/// <summary>Makes a Spine body for a custom-body plant (drawn and updated by the plant, not the effect system).</summary>
	public static Reanimation CreateBody(Plant thePlant, ReanimationType theType, int theRenderOffset, string theAnim)
	{
		Reanimation body = thePlant.mApp.AddReanimation(0f, 0f, thePlant.mRenderOrder + theRenderOffset, theType);
		body.mIsAttachment = true;
		thePlant.mBodyReanimID = thePlant.mApp.ReanimationGetID(body);
		if (theAnim != null)
		{
			body.PlayReanim(theAnim.StartsWith("anim_", StringComparison.Ordinal) ? theAnim : "anim_" + theAnim, ReanimLoopType.Loop, 0, 30f);
		}
		return body;
	}

	public static void PlayAnim(Plant thePlant, string theName, ReanimLoopType theLoop, float theRate, byte theBlend = 0)
	{
		Reanimation body = Body(thePlant);
		body?.PlayReanim(theName.StartsWith("anim_", StringComparison.Ordinal) ? theName : "anim_" + theName, theLoop, theBlend, theRate);
	}

	public static void Align(Plant thePlant, float theScale, float theX, float theY)
	{
		Reanimation body = Body(thePlant);
		if (body != null)
		{
			body.OverrideScale(theScale, theScale);
			body.SetPosition(theX * Constants.S, theY * Constants.S);
		}
	}

	public static float AnimTime(Plant thePlant) => Body(thePlant)?.mAnimTime ?? 1f;

	public static bool IsGiantZombie(Zombie z)
	{
		ZombieType t = z.mZombieType;
		return t == ZombieType.Gargantuar || t == ZombieType.RedeyeGargantuar || t == ZombieType.RobotTitan || t == ZombieType.RedeyeRobotTitan;
	}

	public static bool IsBoss(Zombie z) => z.mZombieType == ZombieType.Boss;

	/// <summary>Dr. Zomboss has his head down within reach (the game's rule for its own plants).</summary>
	public static bool BossCanBeHit(Zombie z) => z.EffectedByDamage(1u);

	public static bool ZombieIsInRow(Zombie z, int theRow) => IsBoss(z) || z.mRow == theRow;

	public static int ZombieRowDistance(Zombie z, int theRow) => IsBoss(z) ? 0 : Math.Abs(z.mRow - theRow);

	public static bool Alive(Zombie z) => z != null && !z.mDead && !z.IsDeadOrDying();

	public static int ZombieMaximumHealth(Zombie z)
	{
		int h = Math.Max(0, z.mBodyMaxHealth);
		if (z.mHasHelm)
		{
			h += Math.Max(0, z.mHelmMaxHealth);
		}
		if (z.mHasShield)
		{
			h += Math.Max(0, z.mShieldMaxHealth);
		}
		if (z.mFlyingMaxHealth > 0)
		{
			h += z.mFlyingMaxHealth;
		}
		return h;
	}

	public static int ZombieEffectiveHealth(Zombie z)
	{
		int h = Math.Max(0, z.mBodyHealth);
		if (z.mHasHelm)
		{
			h += Math.Max(0, z.mHelmHealth);
		}
		if (z.mHasShield)
		{
			h += Math.Max(0, z.mShieldHealth);
		}
		if (z.mFlyingHealth > 0)
		{
			h += z.mFlyingHealth;
		}
		return h;
	}

	public static bool IsWearingMetalArmor(Zombie z)
	{
		if (z.mHasHelm && z.mHelmHealth > 0)
		{
			switch (z.mHelmType)
			{
			case HelmType.Pail:
			case HelmType.Football:
			case HelmType.Digger:
			case HelmType.Bobsled:
			case HelmType.FootballPremium:
			case HelmType.RobotTitanHead:
			case HelmType.RedeyeRobotTitanHead:
				return true;
			}
		}
		return z.mHasShield && z.mShieldHealth > 0 && (z.mShieldType == ShieldType.Door || z.mShieldType == ShieldType.Ladder);
	}

	public static bool RectsOverlap(TRect a, TRect b)
	{
		return !(a.mX + a.mWidth < b.mX || b.mX + b.mWidth < a.mX || a.mY + a.mHeight < b.mY || b.mY + b.mHeight < a.mY);
	}

	public static int Rand(int theMin, int theMax) => TodCommon.RandRangeInt(theMin, theMax);

	public static float RandF(float theMin, float theMax) => TodCommon.RandRangeFloat(theMin, theMax);

	/// <summary>Draws with colorize on, then restores the graphics colour.</summary>
	public static void Tinted(Graphics g, Color theColor, Action theDraw)
	{
		bool oldColorize = g.GetColorizeImages();
		Color oldColor = g.GetColor();
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(theColor);
		try
		{
			theDraw();
		}
		finally
		{
			g.SetColor(oldColor, premultiply: false);
			g.SetColorizeImages(oldColorize);
		}
	}

	// ------------------------------------------------------------------ the shared ultimate

	public static TavelBoard State(Board theBoard) => theBoard.mTavel ??= new TavelBoard();

	public static bool UltimateUiAllowed(Board b)
	{
		if (b == null || !Loaded || b.mApp.mGameScene != GameScenes.Playing)
		{
			return false;
		}
		GameMode m = b.mApp.mGameMode;
		if (m == GameMode.ChallengeZenGarden || m == GameMode.TreeOfWisdom || m == GameMode.Upsell || m == GameMode.Intro)
		{
			return false;
		}
		// the button is shown while the switch is on, or while pack plants are on this lawn
		return Enabled || b.mPlants.Any((Plant p) => !p.mDead && Get(p.mSeedType) != null);
	}

	public static void UpdateUltimateStock(Board b)
	{
		TavelBoard s = State(b);
		if (s.UltLastUpdate == b.mMainCounter)
		{
			return;
		}
		s.UltLastUpdate = b.mMainCounter;
		if (s.UltStock >= UltimateMaxStock)
		{
			s.UltCharge = 0;
			return;
		}
		s.UltCharge++;
		if (s.UltCharge >= UltimateChargeTime)
		{
			s.UltCharge = 0;
			s.UltStock++;
		}
	}

	public static void AddUltimateStock(Board b, int theAmount = 1)
	{
		if (b == null)
		{
			return;
		}
		TavelBoard s = State(b);
		s.UltStock = Math.Min(UltimateMaxStock, s.UltStock + theAmount);
		if (s.UltStock >= UltimateMaxStock)
		{
			s.UltCharge = 0;
		}
	}

	public static TRect UltimateRect(Board b)
	{
		TRect r = b.GetEndoflameSkillRect();
		return new TRect(r.mX + r.mWidth, r.mY, r.mWidth, r.mHeight);
	}

	public static bool CanUseUltimate(Plant p)
	{
		if (p == null || p.mDead || p.mBoard == null)
		{
			return false;
		}
		TavelSpec s = Get(p.mSeedType);
		return s?.CanUltimate != null && s.CanUltimate(p);
	}

	public static bool IsUltimateActive(Plant p)
	{
		if (p == null || p.mBoard == null || p.mDead)
		{
			return false;
		}
		TavelSpec s = Get(p.mSeedType);
		return s?.UltimateActive != null && s.UltimateActive(p);
	}

	/// <summary>A plant in this cell is using its ultimate: the cell cannot be hurt.</summary>
	public static bool IsCellProtected(Plant p)
	{
		if (p?.mBoard == null)
		{
			return false;
		}
		foreach (Plant o in p.mBoard.mPlants)
		{
			if (!o.mDead && o.mPlantCol == p.mPlantCol && o.mRow == p.mRow && IsUltimateActive(o))
			{
				return true;
			}
		}
		return false;
	}

	public static bool StartUltimate(Plant p, bool thePropagateTiles = true, string theSource = "manual")
	{
		if (!CanUseUltimate(p))
		{
			return false;
		}
		Get(p.mSeedType).StartUltimate(p);
		foreach (TavelSpec s in Plants)
		{
			s.OnAnyUltimate?.Invoke(p, thePropagateTiles, theSource);
		}
		return true;
	}

	private static Image sUltimateIcon;

	/// <summary>The framed button inside ultimate.png (100x100); the rest is its shadow.</summary>
	private static readonly TRect UltimateIconBox = new TRect(2, 2, 92, 93);

	public static void DrawUltimateButton(Board b, Graphics g)
	{
		if (!UltimateUiAllowed(b))
		{
			return;
		}
		TavelBoard s = State(b);
		TRect rect = UltimateRect(b);
		sUltimateIcon ??= LoadImage("tavel/ui/ultimate.png");
		Image image = sUltimateIcon;
		if (image != null)
		{
			bool dim = s.UltStock <= 0 || s.UltSelecting;
			if (dim)
			{
				int shade = s.UltSelecting ? 175 : 128;
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(new Color(shade, shade, shade, 255));
			}
			float kx = rect.mWidth / (float)UltimateIconBox.mWidth;
			float ky = rect.mHeight / (float)UltimateIconBox.mHeight;
			g.DrawImage(image, new TRect((int)Math.Round(rect.mX - UltimateIconBox.mX * kx), (int)Math.Round(rect.mY - UltimateIconBox.mY * ky), (int)Math.Round(image.mWidth * kx), (int)Math.Round(image.mHeight * ky)), new TRect(0, 0, image.mWidth, image.mHeight));
			g.SetColorizeImages(colorizeImages: false);
		}
		if (s.UltStock < UltimateMaxStock)
		{
			int inner = 4;
			int remaining = UltimateChargeTime - s.UltCharge;
			int h = (int)((rect.mHeight - inner * 2) * (float)remaining / UltimateChargeTime);
			g.SetColor(new Color(0, 0, 0, 150));
			g.FillRect(rect.mX + inner, rect.mY + inner, rect.mWidth - inner * 2, h);
		}
		TodCommon.TodDrawString(g, s.UltStock.ToString(), rect.mX + rect.mWidth - 3, rect.mY + rect.mHeight - 2, Resources.FONT_PICO129, SexyColor.White, DrawStringJustification.Right);
	}

	// ------------------------------------------------------------------ 下一波

	public static bool CanShowNextWaveButton(Board b)
	{
		if (!UltimateUiAllowed(b) || b.mCurrentWave >= b.mNumWaves || b.mHugeWaveCountDown > 0 || b.IsFlagWave(b.mCurrentWave))
		{
			return false;
		}
		int alive = 0;
		foreach (Zombie z in b.mZombies)
		{
			if (z.mDead || z.IsDeadOrDying())
			{
				continue;
			}
			alive++;
			if (IsBoss(z))
			{
				return false;
			}
		}
		return alive < 5;
	}

	public static GameButton NextWaveButton(Board b)
	{
		TavelBoard s = State(b);
		if (s.NextWave == null)
		{
			s.NextWave = new GameButton(NextWaveButtonId, b);
			s.NextWave.mDrawStoneButton = true;
			s.NextWave.SetLabel("下一波");
		}
		int width = 90;
		int height = AtlasResources.IMAGE_BUTTON_LEFT.mHeight;
		int lawnRight = b.mCamera != null ? b.mCamera.BoardToScreen(b.GridToPixelX(8, 2) + 80, 0).mX : b.GridToPixelX(8, 2) + 80;
		s.NextWave.Resize(Math.Min(lawnRight + 8, b.mWidth - width - 4), b.mHeight / 2 - height / 2, width, height);
		return s.NextWave;
	}

	public static bool TriggerNextWave(Board b)
	{
		if (!CanShowNextWaveButton(b))
		{
			return false;
		}
		b.NextWaveComing();
		b.SpawnZombieWave();
		b.mZombieHealthWaveStart = b.TotalZombiesHealthInWave(b.mCurrentWave - 1);
		bool bowlingOrLastStand = b.mApp.IsWallnutBowlingLevel() || b.mApp.mGameMode == GameMode.ChallengeLastStand;
		if (b.mCurrentWave == b.mNumWaves && b.mApp.IsSurvivalMode())
		{
			b.mZombieHealthToNextWave = 0;
			b.mZombieCountDown = 5500;
		}
		else if (b.IsFlagWave(b.mCurrentWave) && !bowlingOrLastStand)
		{
			b.mZombieHealthToNextWave = 0;
			b.mZombieCountDown = 4500;
		}
		else
		{
			b.mZombieHealthToNextWave = (int)(TodCommon.RandRangeFloat(0.5f, 0.65f) * b.mZombieHealthWaveStart);
			if (b.mApp.IsLittleTroubleLevel() || b.mApp.mGameMode == GameMode.ChallengeColumn || b.mApp.mGameMode == GameMode.ChallengeLastStand)
			{
				b.mZombieCountDown = 750;
			}
			else
			{
				b.mZombieCountDown = 2500 + TodCommon.RandRangeInt(0, 599);
			}
		}
		b.mZombieCountDownStart = b.mZombieCountDown;
		return true;
	}

	public static void DrawNextWaveButton(Board b, Graphics g)
	{
		if (!CanShowNextWaveButton(b))
		{
			return;
		}
		GameButton button = NextWaveButton(b);
		button.Update();
		button.Draw(g);
	}
}
