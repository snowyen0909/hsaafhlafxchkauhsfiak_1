// 植物模组 (plant mods): extra plants from resource packs, loaded once at start-up (adding / enabling one needs a
// restart, like Minecraft mods). A pack is a folder or a .zip in  Documents/mods/植物模组/  holding mod.json, the card
// pictures and Spine files. A plant borrows the behaviour of a game plant ("base": shooter, wall-nut, sunflower ...)
// with its own name, numbers, card and Spine body. No code is loaded: iOS allows data only.
//
// mod.json:
//   { "id": "tavel", "name": "...", "version": "1.0", "author": "...", "description": "...",
//     "plants": [ { "id": "super_gatling", "name": "超级机枪射手", "description": "...",
//                   "base": "Gatlingpea", "cost": 250, "recharge": 7.5, "health": 300, "fireInterval": 1.5,
//                   "damage": 1.5, "card": "cards/super_gatling.png",
//                   "spine": "spine/super_gatlingpea", "spineScale": 14.3, "offset": [0, 0],
//                   "animations": { "idle": "stand", "attack": "attack" } } ] }
//
// Seed numbers: every plant ever seen gets a number from 100 up, kept in userdata/mod/plant_mods.json, so saved games
// and seed banks keep pointing at the right plant even when packs are added or removed.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Sexy;
using Sexy.TodLib;
using Spine;

namespace Lawn;

public sealed class ModPlantSpec
{
	public ModInfo Mod;

	/// <summary>"modid:plantid", the key of its seed number.</summary>
	public string Key;

	public string Name;

	public string Description;

	public SeedType Base;

	public SeedType Seed;

	public ReanimationType Reanim = ReanimationType.None;

	public int Cost;

	/// <summary>Packet cooldown in game ticks (100 per second).</summary>
	public int Refresh;

	public int Health;

	/// <summary>Ticks between shots (the plant's launch rate); 0 keeps the base plant's.</summary>
	public int LaunchRate;

	public float Damage = 1f;

	public string CardFile;

	/// <summary>Content path of the Spine body, "mods/&lt;mod&gt;/&lt;file&gt;" without extension; null: the base plant's body.</summary>
	public string Spine;

	public float SpineScale = 16f;

	/// <summary>Extra size of the body on the lawn (1: as exported).</summary>
	public float Scale = 1f;

	public float OffsetX;

	public float OffsetY;

	public string IdleAnim = "idle";

	public string AttackAnim;

	private Image mCard;

	private bool mCardTried;

	/// <summary>The card picture (any size; drawn into the packet cell).</summary>
	public Image Card
	{
		get
		{
			if (!mCardTried)
			{
				mCardTried = true;
				try
				{
					if (CardFile != null && File.Exists(CardFile))
					{
						using FileStream s = File.OpenRead(CardFile);
						Texture2D tex = Texture2D.FromStream(GlobalStaticVars.g.GraphicsDevice, s, DefaultColorProcessors.PremultiplyAlpha);
						// 100 layout units wide, texture kept at full resolution (like DormPlatform.LoadIcon)
						float k = tex.Width / 100f;
						mCard = new Image(tex, 0, 0, 100, Math.Max(1, (int)Math.Round(tex.Height / k)));
						mCard.mInvScale = k;
					}
				}
				catch (Exception ex)
				{
					ModPlants.Log("card " + CardFile + ": " + ex.Message);
				}
			}
			return mCard;
		}
	}
}

public sealed class ModInfo
{
	public string Id;

	public string Name;

	public string Version = "";

	public string Author = "";

	public string Description = "";

	/// <summary>Folder the pack was read from (a zip is unpacked into the cache first).</summary>
	public string Folder;

	/// <summary>File or folder name inside 植物模组 (what the player sees in the Files app).</summary>
	public string Source;

	public bool Enabled = true;

	/// <summary>Enabled and registered in this run (changes only take effect after a restart).</summary>
	public bool Loaded;

	public string Error;

	public readonly List<ModPlantSpec> Plants = new List<ModPlantSpec>();
}

public static class ModPlants
{
	public const string FolderName = "植物模组";

	public const int FirstSeed = 100;

	/// <summary>Cells in the seed chooser where mod plants start (after the game's 53 + the Imitater, on a new row).</summary>
	public const int FirstChooserCell = 56;

	private static readonly List<ModInfo> sMods = new List<ModInfo>();

	private static readonly Dictionary<int, ModPlantSpec> sBySeed = new Dictionary<int, ModPlantSpec>();

	private static readonly List<ModPlantSpec> sOrder = new List<ModPlantSpec>();

	private static Dictionary<string, int> sSeedIds = new Dictionary<string, int>();

	private static HashSet<string> sDisabled = new HashSet<string>();

	private static bool sScanned;

	private static bool sRegistered;

	/// <summary>Enabled / disabled since start-up: the change shows after a restart.</summary>
	public static bool RestartNeeded;

	public static IReadOnlyList<ModInfo> Mods => sMods;

	/// <summary>Mod plants of this run, in chooser order.</summary>
	public static IReadOnlyList<ModPlantSpec> Plants => sOrder;

	public static int Count => sOrder.Count;

	/// <summary>Highest seed number in use (game seeds if no mod plant).</summary>
	public static int MaxSeed => sOrder.Count == 0 ? (int)SeedType.SeedTypeCount - 1 : sOrder.Max((ModPlantSpec p) => (int)p.Seed);

	public static string Root => Path.Combine(LawnPlatform.PvZActivity.DataRoot, "mods", FolderName);

	private static string CacheRoot => Path.Combine(DormPlatform.ModSaveDir, "plant_mod_cache");

	private static string StatePath => Path.Combine(DormPlatform.ModSaveDir, "plant_mods.json");

	public static bool IsMod(SeedType theSeed) => (int)theSeed >= FirstSeed && sBySeed.ContainsKey((int)theSeed);

	public static ModPlantSpec Get(SeedType theSeed) => (int)theSeed >= FirstSeed && sBySeed.TryGetValue((int)theSeed, out ModPlantSpec p) ? p : null;

	/// <summary>The seed number was given to a mod plant at some point (its pack may be missing now).</summary>
	public static bool IsModNumber(int theSeed) => theSeed >= FirstSeed && theSeed < FirstSeed + 10000;

	/// <summary>Chooser cell of a mod plant, or -1.</summary>
	public static int ChooserCell(SeedType theSeed)
	{
		ModPlantSpec p = Get(theSeed);
		return p == null ? -1 : FirstChooserCell + sOrder.IndexOf(p);
	}

	public static SeedType SeedAtCell(int theCell)
	{
		int k = theCell - FirstChooserCell;
		return k >= 0 && k < sOrder.Count ? sOrder[k].Seed : SeedType.None;
	}

	public static void Log(string theText)
	{
		Console.Error.WriteLine("PlantMods: " + theText);
	}

	// ------------------------------------------------------------------ scanning

	/// <summary>Reads every pack (enabled or not) so the manager can list them; registers nothing.</summary>
	public static void Scan()
	{
		if (sScanned)
		{
			return;
		}
		sScanned = true;
		ReadState();
		try
		{
			Directory.CreateDirectory(Root);
			WriteReadme();
		}
		catch (Exception ex)
		{
			Log("cannot create " + Root + ": " + ex.Message);
		}
		List<string> sources = new List<string>();
		try
		{
			sources.AddRange(Directory.GetDirectories(Root).Where((string d) => !Path.GetFileName(d).StartsWith(".", StringComparison.Ordinal)));
			sources.AddRange(Directory.GetFiles(Root, "*.zip"));
		}
		catch (Exception ex)
		{
			Log("cannot list " + Root + ": " + ex.Message);
		}
		sources.Sort(StringComparer.Ordinal);
		HashSet<string> ids = new HashSet<string>();
		foreach (string src in sources)
		{
			ModInfo mod = ReadPack(src);
			if (mod == null)
			{
				continue;
			}
			if (!ids.Add(mod.Id))
			{
				mod.Error = "和另一个模组的 id 重复（" + mod.Id + "）";
				mod.Enabled = false;
			}
			sMods.Add(mod);
		}
		WriteState();
	}

	private static ModInfo ReadPack(string theSource)
	{
		ModInfo mod = new ModInfo { Source = Path.GetFileName(theSource) };
		try
		{
			string folder = theSource;
			if (File.Exists(theSource) && theSource.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
			{
				folder = UnpackZip(theSource);
			}
			string manifest = FindManifest(folder);
			if (manifest == null)
			{
				if (Directory.Exists(theSource) && !Directory.EnumerateFileSystemEntries(theSource).Any())
				{
					return null;
				}
				mod.Id = mod.Source;
				mod.Name = mod.Source;
				mod.Error = "找不到 mod.json";
				mod.Enabled = false;
				return mod;
			}
			folder = Path.GetDirectoryName(manifest);
			mod.Folder = folder;
			JObject j = JObject.Parse(File.ReadAllText(manifest));
			mod.Id = Clean((string)j["id"]) ?? Clean(mod.Source);
			mod.Name = (string)j["name"] ?? mod.Id;
			mod.Version = (string)j["version"] ?? "";
			mod.Author = (string)j["author"] ?? "";
			mod.Description = (string)j["description"] ?? "";
			mod.Enabled = !sDisabled.Contains(mod.Id);
			if (j["plants"] is JArray plants)
			{
				foreach (JToken t in plants)
				{
					if (t is JObject pj)
					{
						ModPlantSpec spec = ReadPlant(mod, pj, out string error);
						if (spec != null)
						{
							mod.Plants.Add(spec);
						}
						else
						{
							mod.Error = (mod.Error == null ? "" : mod.Error + "；") + error;
						}
					}
				}
			}
			if (mod.Plants.Count == 0 && mod.Error == null)
			{
				mod.Error = "没有植物";
			}
		}
		catch (Exception ex)
		{
			mod.Id ??= mod.Source;
			mod.Name ??= mod.Source;
			mod.Error = "读取失败：" + ex.Message;
			mod.Enabled = false;
		}
		return mod;
	}

	private static string FindManifest(string theFolder)
	{
		if (!Directory.Exists(theFolder))
		{
			return null;
		}
		string direct = Path.Combine(theFolder, "mod.json");
		if (File.Exists(direct))
		{
			return direct;
		}
		// a zip often holds one top folder
		foreach (string sub in Directory.GetDirectories(theFolder))
		{
			string inner = Path.Combine(sub, "mod.json");
			if (File.Exists(inner))
			{
				return inner;
			}
		}
		return null;
	}

	/// <summary>Unpacks a pack zip into the cache (again only when the zip changed).</summary>
	private static string UnpackZip(string theZip)
	{
		FileInfo fi = new FileInfo(theZip);
		string stamp = fi.Length + "_" + fi.LastWriteTimeUtc.Ticks;
		string dest = Path.Combine(CacheRoot, Clean(Path.GetFileNameWithoutExtension(theZip)) ?? "pack");
		string stampFile = Path.Combine(dest, ".stamp");
		if (File.Exists(stampFile) && File.ReadAllText(stampFile) == stamp)
		{
			return dest;
		}
		if (Directory.Exists(dest))
		{
			Directory.Delete(dest, recursive: true);
		}
		Directory.CreateDirectory(dest);
		string root = Path.GetFullPath(dest) + Path.DirectorySeparatorChar;
		using (ZipArchive zip = ZipFile.OpenRead(theZip))
		{
			foreach (ZipArchiveEntry e in zip.Entries)
			{
				if (string.IsNullOrEmpty(e.Name))
				{
					continue;
				}
				string target = Path.GetFullPath(Path.Combine(dest, e.FullName.Replace('\\', '/')));
				if (!target.StartsWith(root, StringComparison.Ordinal))
				{
					continue; // entries pointing outside the pack are skipped
				}
				Directory.CreateDirectory(Path.GetDirectoryName(target));
				e.ExtractToFile(target, overwrite: true);
			}
		}
		File.WriteAllText(stampFile, stamp);
		return dest;
	}

	private static ModPlantSpec ReadPlant(ModInfo theMod, JObject j, out string theError)
	{
		theError = null;
		string id = Clean((string)j["id"]);
		if (id == null)
		{
			theError = "有植物缺少 id";
			return null;
		}
		string baseName = (string)j["base"] ?? "Peashooter";
		if (!Enum.TryParse(baseName, ignoreCase: true, out SeedType baseSeed) || baseSeed < SeedType.Peashooter || baseSeed >= SeedType.SeedTypeCount || !AllowedBase(baseSeed))
		{
			theError = id + "：不支持的 base「" + baseName + "」";
			return null;
		}
		ModPlantSpec p = new ModPlantSpec
		{
			Mod = theMod,
			Key = theMod.Id + ":" + id,
			Name = (string)j["name"] ?? id,
			Description = (string)j["description"] ?? "",
			Base = baseSeed,
			Cost = Math.Max(0, (int?)j["cost"] ?? 100),
			Refresh = (int)Math.Round(100.0 * ((double?)j["recharge"] ?? 7.5)),
			Health = Math.Max(0, (int?)j["health"] ?? 0),
			LaunchRate = (int)Math.Round(100.0 * ((double?)j["fireInterval"] ?? 0.0)),
			Damage = Math.Max(0f, (float?)j["damage"] ?? 1f),
			SpineScale = (float?)j["spineScale"] ?? 16f,
			Scale = Math.Max(0.05f, (float?)j["scale"] ?? 1f)
		};
		string card = (string)j["card"];
		if (card != null)
		{
			p.CardFile = SafePath(theMod.Folder, card);
		}
		string spine = (string)j["spine"];
		if (spine != null)
		{
			string skel = SafePath(theMod.Folder, spine + ".skel");
			string atlas = SafePath(theMod.Folder, spine + ".atlas");
			if (skel == null || atlas == null || !File.Exists(skel) || !File.Exists(atlas))
			{
				theError = id + "：找不到 Spine 文件 " + spine + ".skel / .atlas";
				return null;
			}
			p.Spine = "mods/" + theMod.Id + "/" + spine.Replace('\\', '/').TrimStart('/');
		}
		if (j["offset"] is JArray off && off.Count >= 2)
		{
			p.OffsetX = (float)off[0];
			p.OffsetY = (float)off[1];
		}
		if (j["animations"] is JObject anims)
		{
			p.IdleAnim = (string)anims["idle"] ?? p.IdleAnim;
			p.AttackAnim = (string)anims["attack"];
		}
		return p;
	}

	/// <summary>Game plants a mod plant may act as: daytime land plants without special planting rules.</summary>
	private static bool AllowedBase(SeedType theSeed)
	{
		switch (theSeed)
		{
		case SeedType.Peashooter:
		case SeedType.Sunflower:
		case SeedType.Cherrybomb:
		case SeedType.Wallnut:
		case SeedType.Potatomine:
		case SeedType.Snowpea:
		case SeedType.Chomper:
		case SeedType.Repeater:
		case SeedType.Squash:
		case SeedType.Threepeater:
		case SeedType.Jalapeno:
		case SeedType.Torchwood:
		case SeedType.Tallnut:
		case SeedType.Cactus:
		case SeedType.Splitpea:
		case SeedType.Starfruit:
		case SeedType.Cabbagepult:
		case SeedType.Kernelpult:
		case SeedType.Garlic:
		case SeedType.Melonpult:
		case SeedType.Gatlingpea:
		case SeedType.Twinsunflower:
		case SeedType.Wintermelon:
			return true;
		default:
			return false;
		}
	}

	/// <summary>A file inside the pack folder (null if the path leaves it).</summary>
	private static string SafePath(string theFolder, string theRelative)
	{
		string root = Path.GetFullPath(theFolder) + Path.DirectorySeparatorChar;
		string full = Path.GetFullPath(Path.Combine(theFolder, theRelative.Replace('\\', '/')));
		return full.StartsWith(root, StringComparison.Ordinal) ? full : null;
	}

	/// <summary>Ids: letters, digits, '_' and '-' only.</summary>
	private static string Clean(string theId)
	{
		if (string.IsNullOrWhiteSpace(theId))
		{
			return null;
		}
		string s = new string(theId.Trim().Where((char c) => char.IsLetterOrDigit(c) || c == '_' || c == '-').ToArray());
		return s.Length == 0 ? null : s;
	}

	// ------------------------------------------------------------------ registration

	/// <summary>
	/// Gives the enabled packs' plants their seed numbers, plant definitions and Spine bodies. Called right after the
	/// game's reanimation table is loaded (again if the game reloads it).
	/// </summary>
	public static void Register()
	{
		Scan();
		if (!sRegistered)
		{
			sRegistered = true;
			foreach (ModInfo mod in sMods.Where((ModInfo m) => m.Enabled && m.Error == null))
			{
				foreach (ModPlantSpec p in mod.Plants)
				{
					if (!sSeedIds.TryGetValue(p.Key, out int seed))
					{
						seed = sSeedIds.Count == 0 ? FirstSeed : Math.Max(FirstSeed, sSeedIds.Values.Max() + 1);
						sSeedIds[p.Key] = seed;
					}
					p.Seed = (SeedType)seed;
					sBySeed[seed] = p;
					sOrder.Add(p);
				}
				mod.Loaded = true;
			}
			WriteState();
			MountFiles();
		}
		RegisterDefinitions();
		Log(sOrder.Count + " plant(s) from " + sMods.Count((ModInfo m) => m.Loaded) + " pack(s)");
	}

	/// <summary>Pack files become content paths "mods/&lt;id&gt;/..." (Spine atlases, pages and skeletons are read through the overlay).</summary>
	private static void MountFiles()
	{
		Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (ModInfo mod in sMods.Where((ModInfo m) => m.Loaded))
		{
			foreach (string file in Directory.EnumerateFiles(mod.Folder, "*", SearchOption.AllDirectories))
			{
				string rel = Path.GetRelativePath(mod.Folder, file).Replace('\\', '/');
				map[WardrobeOverlay.Normalize("mods/" + mod.Id + "/" + rel)] = file;
			}
		}
		WardrobeOverlay.SetModMap(map);
	}

	private static void RegisterDefinitions()
	{
		if (sOrder.Count == 0)
		{
			return;
		}
		int maxSeed = MaxSeed;
		PlantDefinition[] defs = GameConstants.gPlantDefs;
		if (defs.Length <= maxSeed)
		{
			Array.Resize(ref defs, maxSeed + 1);
			GameConstants.gPlantDefs = defs;
		}
		// holes stay valid, inert definitions (some code walks the whole table)
		PlantDefinition filler = defs[0];
		for (int i = 0; i < defs.Length; i++)
		{
			if (defs[i] == null)
			{
				defs[i] = new PlantDefinition(SeedType.None, null, filler.mReanimationType, 0, 0, 0, PlantSubClass.Normal, 0, "UNUSED_MOD_SEED_SLOT");
			}
		}
		int firstReanim = GameConstants.gLawnReanimationArray.Length;
		int nextReanim = firstReanim;
		foreach (ModPlantSpec p in sOrder)
		{
			PlantDefinition baseDef = defs[(int)p.Base];
			if (p.Spine != null)
			{
				p.Reanim = (ReanimationType)nextReanim++;
			}
			defs[(int)p.Seed] = new PlantDefinition(p.Seed, baseDef.mPlantImage, baseDef.mReanimationType, baseDef.mPacketIndex, p.Cost, p.Refresh, baseDef.mSubClass, p.LaunchRate > 0 ? p.LaunchRate : baseDef.mLaunchRate, baseDef.mPlantName);
		}
		if (nextReanim == firstReanim)
		{
			return;
		}
		// Spine bodies get reanimation numbers after the game's own (never saved: the body is made again on load)
		ReanimationParams[] reanims = GameConstants.gLawnReanimationArray;
		Array.Resize(ref reanims, nextReanim);
		foreach (ModPlantSpec p in sOrder.Where((ModPlantSpec q) => q.Spine != null))
		{
			reanims[(int)p.Reanim] = new ReanimationParams(p.Reanim, p.Spine, 0, isSpine: true, null, p.SpineScale);
		}
		GameConstants.gLawnReanimationArray = reanims;
		ReanimatorXnaHelpers.gReanimationParamArray = reanims;
		if (ReanimatorXnaHelpers.gReanimatorDefArray.Length < nextReanim)
		{
			ReanimatorDefinition[] d = ReanimatorXnaHelpers.gReanimatorDefArray;
			Array.Resize(ref d, nextReanim);
			ReanimatorXnaHelpers.gReanimatorDefArray = d;
		}
		foreach (ModPlantSpec p in sOrder.Where((ModPlantSpec q) => q.Spine != null))
		{
			try
			{
				ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(p.Reanim, theIsPreloading: false);
				SkeletonData data = ReanimatorXnaHelpers.gReanimatorDefArray[(int)p.Reanim]?.mSpineSkeletonData;
				if (data == null)
				{
					throw new Exception("no skeleton");
				}
				if (data.FindAnimation(p.IdleAnim) == null)
				{
					string first = data.Animations.Count > 0 ? data.Animations.Items[0].Name : null;
					Log(p.Key + ": no animation \"" + p.IdleAnim + "\", using \"" + first + "\"");
					p.IdleAnim = first ?? throw new Exception("no animations");
				}
				if (p.AttackAnim != null && data.FindAnimation(p.AttackAnim) == null)
				{
					Log(p.Key + ": no animation \"" + p.AttackAnim + "\"");
					p.AttackAnim = null;
				}
			}
			catch (Exception ex)
			{
				Log(p.Key + " spine: " + ex.Message);
				p.Reanim = ReanimationType.None;
			}
		}
	}

	// ------------------------------------------------------------------ the plant on the lawn

	/// <summary>After the base plant was set up: the mod plant's own numbers and body.</summary>
	public static void Apply(Plant thePlant, ModPlantSpec theSpec)
	{
		thePlant.mModSeed = theSpec.Seed;
		if (theSpec.Health > 0)
		{
			thePlant.mPlantHealth = theSpec.Health;
			thePlant.mPlantMaxHealth = theSpec.Health;
		}
		if (theSpec.LaunchRate > 0)
		{
			thePlant.mLaunchRate = theSpec.LaunchRate;
		}
		EnsureBody(thePlant, theSpec);
	}

	/// <summary>
	/// The Spine body drawn instead of the base plant's. It lives outside the effect system (not saved, not updated by
	/// it): the plant updates and draws it, and makes it again after a saved game is loaded.
	/// </summary>
	public static Reanimation EnsureBody(Plant thePlant, ModPlantSpec theSpec)
	{
		if (theSpec.Reanim == ReanimationType.None)
		{
			return null;
		}
		Reanimation r = thePlant.mModReanim;
		if (r != null && !r.mDead)
		{
			return r;
		}
		try
		{
			r = Reanimation.GetNewReanimation();
			r.ReanimationInitializeType(0f, 0f, theSpec.Reanim);
			r.mActive = true;
			r.mRenderOrder = thePlant.mRenderOrder;
			r.PlayReanim("anim_" + theSpec.IdleAnim, ReanimLoopType.Loop, 0, 0f);
			r.mAnimTime = TodCommon.RandRangeFloat(0f, 0.99f);
			thePlant.mModReanim = r;
			return r;
		}
		catch (Exception ex)
		{
			Log(theSpec.Key + " body: " + ex.Message);
			theSpec.Reanim = ReanimationType.None;
			return null;
		}
	}

	/// <summary>Draws the mod body in place of the base plant's body reanimation (false: draw the base plant).</summary>
	public static bool DrawBody(Plant thePlant, Graphics g, Reanimation theBaseBody)
	{
		ModPlantSpec spec = Get(thePlant.mModSeed);
		Reanimation r = spec == null ? null : EnsureBody(thePlant, spec);
		if (r == null)
		{
			return false;
		}
		r.mOverlayMatrix = theBaseBody.mOverlayMatrix;
		r.mOverlayMatrix.mMatrix.M11 *= spec.Scale;
		r.mOverlayMatrix.mMatrix.M22 *= spec.Scale;
		Microsoft.Xna.Framework.Vector3 t = theBaseBody.mOverlayMatrix.mMatrix.Translation;
		r.SetPosition(t.X + spec.OffsetX * Constants.S, t.Y + spec.OffsetY * Constants.S);
		r.mColorOverride = theBaseBody.mColorOverride;
		r.mExtraAdditiveColor = theBaseBody.mExtraAdditiveColor;
		r.mEnableExtraAdditiveDraw = theBaseBody.mEnableExtraAdditiveDraw;
		r.mExtraOverlayColor = theBaseBody.mExtraOverlayColor;
		r.mEnableExtraOverlayDraw = theBaseBody.mEnableExtraOverlayDraw;
		r.mFilterEffect = theBaseBody.mFilterEffect;
		r.Draw(g);
		return true;
	}

	/// <summary>Draws a mod plant's card fitted (and centred) into a packet cel at (x, y) of the given size.</summary>
	public static bool DrawCard(Graphics g, SeedType theSeed, float theX, float theY, float theWidth, float theHeight)
	{
		Image card = Get(theSeed)?.Card;
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

	/// <summary>Every tick: animate, and go back to the idle loop once the attack has played.</summary>
	public static void Update(Plant thePlant)
	{
		ModPlantSpec spec = Get(thePlant.mModSeed);
		Reanimation r = spec == null ? null : EnsureBody(thePlant, spec);
		if (r == null)
		{
			return;
		}
		r.Update();
		if (r.mLoopType == ReanimLoopType.PlayOnceAndHold && r.mLoopCount > 0)
		{
			r.PlayReanim("anim_" + spec.IdleAnim, ReanimLoopType.Loop, 10, 0f);
		}
	}

	/// <summary>A shot left the plant: attack animation, and the pack's damage factor on the projectile.</summary>
	public static void OnFire(Plant thePlant, Projectile theProjectile)
	{
		ModPlantSpec spec = Get(thePlant.mModSeed);
		if (spec == null)
		{
			return;
		}
		if (theProjectile != null)
		{
			theProjectile.mDamageScale = spec.Damage;
		}
		Reanimation r = thePlant.mModReanim;
		if (r != null && !r.mDead && spec.AttackAnim != null && r.mLoopType == ReanimLoopType.Loop)
		{
			r.PlayReanim("anim_" + spec.AttackAnim, ReanimLoopType.PlayOnceAndHold, 5, 0f);
		}
	}

	public static void OnDie(Plant thePlant)
	{
		Reanimation r = thePlant.mModReanim;
		thePlant.mModReanim = null;
		r?.ReanimationDie();
	}

	// ------------------------------------------------------------------ state

	private static void ReadState()
	{
		try
		{
			if (!File.Exists(StatePath))
			{
				return;
			}
			JObject j = JObject.Parse(File.ReadAllText(StatePath));
			if (j["seeds"] is JObject seeds)
			{
				sSeedIds = seeds.Properties().ToDictionary((JProperty p) => p.Name, (JProperty p) => (int)p.Value);
			}
			if (j["disabled"] is JArray off)
			{
				sDisabled = new HashSet<string>(off.Select((JToken t) => (string)t));
			}
		}
		catch (Exception ex)
		{
			Log("state: " + ex.Message);
		}
	}

	private static void WriteState()
	{
		try
		{
			Directory.CreateDirectory(DormPlatform.ModSaveDir);
			JObject j = new JObject
			{
				["version"] = 1,
				["seeds"] = new JObject(sSeedIds.OrderBy((KeyValuePair<string, int> kv) => kv.Value).Select((KeyValuePair<string, int> kv) => new JProperty(kv.Key, kv.Value))),
				["disabled"] = new JArray(sDisabled.OrderBy((string s) => s))
			};
			string tmp = StatePath + ".tmp";
			File.WriteAllText(tmp, j.ToString());
			File.Copy(tmp, StatePath, overwrite: true);
			File.Delete(tmp);
		}
		catch (Exception ex)
		{
			Log("state write: " + ex.Message);
		}
	}

	public static void SetEnabled(ModInfo theMod, bool theOn)
	{
		if (theMod.Error != null && theOn)
		{
			return;
		}
		theMod.Enabled = theOn;
		if (theOn)
		{
			sDisabled.Remove(theMod.Id);
		}
		else
		{
			sDisabled.Add(theMod.Id);
		}
		WriteState();
		RestartNeeded = sMods.Any((ModInfo m) => m.Enabled != m.Loaded && m.Error == null);
	}

	private static void WriteReadme()
	{
		string path = Path.Combine(Root, "说明.txt");
		if (File.Exists(path))
		{
			return;
		}
		File.WriteAllText(path,
			"植物模组\n" +
			"把模组的 .zip 或解压后的文件夹放在这里，重开游戏就会加载。\n" +
			"在主菜单的「模组」里可以开关每个模组（改完要重开游戏）。\n" +
			"模组只能放图片、动画和数值，请只安装你信任的作者做的模组。\n");
	}
}
