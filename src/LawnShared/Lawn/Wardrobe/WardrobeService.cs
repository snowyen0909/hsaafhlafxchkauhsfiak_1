// Wardrobe (时装柜) for the iOS mod edition: an independent re-implementation that reads the same
// patch-folder layout as the Android 时装柜 mod (patch/<name>/images/600x1000/*.xnb, spanim/, voices/, ...).
// Players import patch folders with the Files app into  Documents/mods/时装柜/patches/.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Sexy.TodLib;

namespace Lawn;

public sealed class WardrobeFile
{
	public string Relative;   // content-relative, lower case

	public string Source;     // absolute path

	public long Size;
}

public sealed class WardrobePack
{
	public string Id;

	public string Name;

	public string Title;

	public string Author = string.Empty;

	public string Dir;

	public string Kind = string.Empty;

	public readonly List<WardrobeFile> Files = new List<WardrobeFile>();

	public readonly List<string> PlantKeys = new List<string>();

	public readonly List<WardrobeFile> PreviewImages = new List<WardrobeFile>();

	public long Bytes;

	public bool IsGlobal => PlantKeys.Count == 0;
}

public static class WardrobeService
{
	public const string MOD_NAME = "时装柜";

	private static readonly HashSet<string> ContentDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "images", "spanim", "reanim", "sounds", "voices", "fonts", "particles", "shaders", "music" };

	private static readonly HashSet<string> SkipNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "desktop.ini", "thumbs.db", ".ds_store", "coin.txt", "readme.txt", "readme.md", "说明.txt", "使用说明.txt", "license.txt" };

	private static readonly Regex NamedPainting = new Regex("^painting_([a-z0-9_]+?)_([01])\\.xnb$", RegexOptions.IgnoreCase);

	private static readonly Regex NumberedPainting = new Regex("^painting(\\d+)_([01])\\.xnb$", RegexOptions.IgnoreCase);

	private static readonly Regex ComicPage = new Regex("^cartoon_([a-z_]+?)\\d+\\.xnb$", RegexOptions.IgnoreCase);

	private static readonly Regex Author = new Regex("作者\\s*[-:：]\\s*([^）)]+)");

	private static readonly Dictionary<string, string> NumberedKeys = new Dictionary<string, string>
	{
		["1"] = "sunflower", ["6"] = "chomper", ["7"] = "repeater", ["11"] = "gravebuster", ["13"] = "scaredyshroom",
		["16"] = "lilypad", ["20"] = "jalapeno", ["21"] = "spikeweed", ["25"] = "plantern", ["30"] = "pumpkinshell",
		["34"] = "kernelpult", ["36"] = "garlic", ["43"] = "cattail", ["51"] = "agave"
	};

	private static readonly Dictionary<string, string> KeyAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		["pumpkin"] = "pumpkinshell", ["fume"] = "fumeshroom", ["scaredy"] = "scaredyshroom", ["kernel"] = "kernelpult",
		["cat"] = "cattail", ["caltrop"] = "spikeweed", ["190306"] = "cattail"
	};

	private static Dictionary<string, SeedType> sSeedByKey;

	public static List<WardrobePack> Packs { get; private set; } = new List<WardrobePack>();

	/// <summary>Worn pack ids, in the order they were put on.</summary>
	public static List<string> Worn { get; private set; } = new List<string>();

	public static readonly List<string> Warnings = new List<string>();

	public static bool Initialized { get; private set; }

	public static string ModDir => Path.Combine(LawnPlatform.PvZActivity.DataRoot, "mods", MOD_NAME);

	public static string PatchDir => Path.Combine(ModDir, "patches");

	/// <summary>What is worn: shared by every profile, kept with the mod's other saves (userdata/mod/).</summary>
	private static string StatePath => Path.Combine(LawnPlatform.PvZActivity.DataRoot, "docs", "userdata", "mod", "wardrobe.json");

	/// <summary>Where earlier builds kept it; read once, then left as wardrobe.json.old.</summary>
	private static string OldStatePath => Path.Combine(LawnPlatform.PvZActivity.DataRoot, "docs", "userdata", "wardrobe.json");

	private sealed class State
	{
		public List<string> worn = new List<string>();
	}

	private sealed class Config
	{
		public List<string> patchRoots = new List<string> { "patches" };
	}

	/// <summary>Called once at startup (before any content loads) in the mod edition.</summary>
	public static void Startup()
	{
		try
		{
			Directory.CreateDirectory(PatchDir);
			string hint = Path.Combine(PatchDir, "把补丁文件夹放这里.txt");
			if (!File.Exists(hint))
			{
				File.WriteAllText(hint, "把每个补丁文件夹整个放进这个 patches 文件夹（用「文件」App 拷贝或解压过来）。\n补丁文件夹里应该能看到 images / spanim / voices 等子文件夹。\n放好后在游戏主菜单点「时装柜」→「刷新」，选中补丁点「穿上」。\n");
			}
			Rescan();
			LoadState();
			ApplyMap();
		}
		catch (Exception ex)
		{
			Warnings.Add("时装柜初始化失败：" + ex.Message);
			Console.Error.WriteLine("Wardrobe startup: " + ex);
		}
		Initialized = true;
	}

	public static List<string> PatchRoots()
	{
		List<string> roots = new List<string>();
		Config cfg = null;
		string cfgPath = Path.Combine(ModDir, "config.json");
		try
		{
			if (File.Exists(cfgPath))
			{
				cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText(cfgPath));
			}
		}
		catch (Exception ex)
		{
			Warnings.Add("config.json 读取失败：" + ex.Message);
		}
		foreach (string r in (cfg?.patchRoots != null && cfg.patchRoots.Count > 0) ? cfg.patchRoots : new List<string> { "patches" })
		{
			if (string.IsNullOrWhiteSpace(r))
			{
				continue;
			}
			string full = Path.IsPathRooted(r) ? r : Path.Combine(ModDir, r);
			if (!roots.Contains(full, StringComparer.OrdinalIgnoreCase))
			{
				roots.Add(full);
			}
		}
		if (!roots.Contains(PatchDir, StringComparer.OrdinalIgnoreCase))
		{
			roots.Insert(0, PatchDir);
		}
		return roots;
	}

	public static void Rescan()
	{
		Warnings.Clear();
		List<WardrobePack> packs = new List<WardrobePack>();
		HashSet<string> ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string root in PatchRoots())
		{
			if (!Directory.Exists(root))
			{
				continue;
			}
			IEnumerable<string> dirs;
			if (HasContentDir(root))
			{
				dirs = new[] { root };
			}
			else
			{
				dirs = Directory.EnumerateDirectories(root).Where((string d) => !Path.GetFileName(d).StartsWith(".", StringComparison.Ordinal)).OrderBy((string d) => d, StringComparer.CurrentCultureIgnoreCase);
			}
			foreach (string dir in dirs)
			{
				try
				{
					WardrobePack pack = ScanPack(dir);
					if (pack == null)
					{
						continue;
					}
					string id = pack.Name;
					int n = 2;
					while (!ids.Add(id))
					{
						id = pack.Name + " (" + n++ + ")";
					}
					pack.Id = id;
					packs.Add(pack);
				}
				catch (Exception ex)
				{
					Warnings.Add("扫描失败 " + Path.GetFileName(dir) + "：" + ex.Message);
				}
			}
		}
		packs.Sort((WardrobePack a, WardrobePack b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
		Packs = packs;
		Worn.RemoveAll((string id) => !packs.Any((WardrobePack p) => p.Id == id));
	}

	private static bool HasContentDir(string dir)
	{
		return ContentDirs.Any((string d) => Directory.Exists(Path.Combine(dir, d))) || Directory.Exists(Path.Combine(dir, "Content"));
	}

	/// <summary>The folder whose children are images/ spanim/ ...; tolerates one extra wrapper folder (common after unzipping).</summary>
	private static string FindContentBase(string dir)
	{
		if (Directory.Exists(Path.Combine(dir, "Content")) && !ContentDirs.Any((string d) => Directory.Exists(Path.Combine(dir, d))))
		{
			return Path.Combine(dir, "Content");
		}
		if (HasContentDir(dir))
		{
			return dir;
		}
		string[] subs = Directory.GetDirectories(dir).Where((string d) => !Path.GetFileName(d).StartsWith(".", StringComparison.Ordinal)).ToArray();
		if (subs.Length == 1)
		{
			return FindContentBase(subs[0]);
		}
		return null;
	}

	private static WardrobePack ScanPack(string dir)
	{
		string baseDir = FindContentBase(dir);
		if (baseDir == null)
		{
			return null;
		}
		string name = Path.GetFileName(dir.TrimEnd('/', '\\'));
		WardrobePack pack = new WardrobePack { Name = name, Dir = dir, Title = name };
		int paren = name.IndexOfAny(new[] { '（', '(' });
		if (paren > 0)
		{
			pack.Title = name.Substring(0, paren).Trim();
			Match m = Author.Match(name.Substring(paren));
			if (m.Success)
			{
				pack.Author = m.Groups[1].Value.Trim();
			}
		}
		HashSet<string> kinds = new HashSet<string>();
		foreach (string file in Directory.EnumerateFiles(baseDir, "*", SearchOption.AllDirectories))
		{
			string rel = Path.GetRelativePath(baseDir, file).Replace('\\', '/');
			string fn = Path.GetFileName(rel);
			if (SkipNames.Contains(fn) || fn.StartsWith(".", StringComparison.Ordinal))
			{
				continue;
			}
			int slash = rel.IndexOf('/');
			if (slash <= 0 || !ContentDirs.Contains(rel.Substring(0, slash)))
			{
				continue;
			}
			FileInfo fi = new FileInfo(file);
			if (fi.Length == 0)
			{
				continue;
			}
			WardrobeFile wf = new WardrobeFile { Relative = rel.ToLowerInvariant(), Source = file, Size = fi.Length };
			pack.Files.Add(wf);
			pack.Bytes += fi.Length;
			string key = Classify(wf.Relative, out string kind, out bool previewable);
			if (kind != null)
			{
				kinds.Add(kind);
			}
			if (key != null && !pack.PlantKeys.Contains(key))
			{
				pack.PlantKeys.Add(key);
			}
			if (previewable)
			{
				pack.PreviewImages.Add(wf);
			}
		}
		if (pack.Files.Count == 0)
		{
			return null;
		}
		if (name.Contains("地图") || name.Contains("选卡界面") || name.Contains("加载界面"))
		{
			pack.PlantKeys.Clear();
		}
		pack.PreviewImages.Sort((WardrobeFile a, WardrobeFile b) => string.CompareOrdinal(a.Relative, b.Relative));
		pack.Kind = kinds.Count == 0 ? "其他" : string.Join(" / ", kinds.OrderBy((string k) => k));
		return pack;
	}

	private static string NormalizeKey(string key)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			return null;
		}
		key = key.Trim().ToLowerInvariant();
		return KeyAliases.TryGetValue(key, out string a) ? a : key;
	}

	/// <summary>Plant key (or null) for a patch file, plus a short type label for the list.</summary>
	private static string Classify(string rel, out string kind, out bool previewable)
	{
		kind = null;
		previewable = false;
		string fn = Path.GetFileName(rel);
		if (rel.StartsWith("images/", StringComparison.Ordinal))
		{
			previewable = rel.EndsWith(".xnb", StringComparison.Ordinal) || rel.EndsWith(".png", StringComparison.Ordinal) || rel.EndsWith(".jpg", StringComparison.Ordinal);
			Match m = NamedPainting.Match(fn);
			if (m.Success)
			{
				kind = m.Groups[2].Value == "1" ? "满好感立绘" : "立绘";
				return NormalizeKey(m.Groups[1].Value);
			}
			m = NumberedPainting.Match(fn);
			if (m.Success)
			{
				kind = m.Groups[2].Value == "1" ? "满好感立绘" : "立绘";
				return NumberedKeys.TryGetValue(m.Groups[1].Value, out string k) ? k : null;
			}
			m = ComicPage.Match(fn);
			if (m.Success)
			{
				kind = "漫画";
				return NormalizeKey(m.Groups[1].Value);
			}
			if (fn.StartsWith("seedpacket", StringComparison.Ordinal))
			{
				kind = "选卡界面";
			}
			else if (fn.StartsWith("background", StringComparison.Ordinal))
			{
				kind = "地图";
			}
			else
			{
				kind = "图片";
			}
			return null;
		}
		if (rel.StartsWith("spanim/", StringComparison.Ordinal))
		{
			kind = "角色";
			previewable = rel.EndsWith(".png", StringComparison.Ordinal);
			string n = Path.GetFileNameWithoutExtension(fn);
			if (n.EndsWith("_tachie", StringComparison.Ordinal))
			{
				n = n.Substring(0, n.Length - 7);
			}
			int us = n.IndexOf('_');
			string key = NormalizeKey(us > 0 ? n.Substring(0, us) : n);
			return ResolveSeed(key, out _) ? key : null;
		}
		if (rel.StartsWith("voices/", StringComparison.Ordinal))
		{
			kind = "语音";
			string n = Path.GetFileNameWithoutExtension(fn);
			int dash = n.IndexOf('-');
			string key = NormalizeKey(dash > 0 ? n.Substring(0, dash) : n);
			return ResolveSeed(key, out _) ? key : null;
		}
		if (rel.StartsWith("sounds/", StringComparison.Ordinal))
		{
			kind = "音效";
		}
		else if (rel.StartsWith("reanim/", StringComparison.Ordinal))
		{
			kind = "动画";
		}
		else if (rel.StartsWith("fonts/", StringComparison.Ordinal))
		{
			kind = "字体";
		}
		else
		{
			kind = "其他";
		}
		return null;
	}

	public static bool ResolveSeed(string key, out SeedType seed)
	{
		if (sSeedByKey == null)
		{
			Dictionary<string, SeedType> d = new Dictionary<string, SeedType>(StringComparer.OrdinalIgnoreCase);
			foreach (SeedType s in Enum.GetValues(typeof(SeedType)))
			{
				d[s.ToString()] = s;
			}
			d["pumpkinshell"] = SeedType.Pumpkinshell;
			d["spikeweed"] = SeedType.Spikeweed;
			sSeedByKey = d;
		}
		seed = SeedType.None;
		return key != null && sSeedByKey.TryGetValue(key, out seed) && seed >= 0 && seed < SeedType.SeedTypeCount;
	}

	public static string PlantTitle(string key)
	{
		if (ResolveSeed(key, out SeedType seed))
		{
			try
			{
				string name = Plant.GetPlantDefinition(seed).mPlantName;
				string t = TodStringFile.TodStringTranslate("[" + name + "]");
				if (!string.IsNullOrEmpty(t) && !t.StartsWith("[", StringComparison.Ordinal))
				{
					return t;
				}
			}
			catch
			{
			}
		}
		return key;
	}

	public static WardrobePack Find(string id)
	{
		return Packs.FirstOrDefault((WardrobePack p) => p.Id == id);
	}

	public static bool IsWorn(WardrobePack pack)
	{
		return pack != null && Worn.Contains(pack.Id);
	}

	/// <summary>Puts a pack on; packs that write any of the same files come off. Returns the titles that were replaced.</summary>
	public static List<string> Wear(WardrobePack pack)
	{
		List<string> replaced = new List<string>();
		if (pack == null || IsWorn(pack))
		{
			return replaced;
		}
		HashSet<string> mine = new HashSet<string>(pack.Files.Select((WardrobeFile f) => f.Relative));
		foreach (string id in Worn.ToList())
		{
			WardrobePack other = Find(id);
			if (other != null && other.Files.Any((WardrobeFile f) => mine.Contains(f.Relative)))
			{
				Worn.Remove(id);
				replaced.Add(other.Title);
			}
		}
		Worn.Add(pack.Id);
		Commit();
		return replaced;
	}

	public static void TakeOff(WardrobePack pack)
	{
		if (pack != null && Worn.Remove(pack.Id))
		{
			Commit();
		}
	}

	public static void TakeOffAll()
	{
		Worn.Clear();
		Commit();
	}

	/// <summary>Result of the last immediate swap (shown in the wardrobe screen).</summary>
	public static HotReloadResult LastReload { get; private set; }

	private static void Commit()
	{
		SaveState();
		Dictionary<string, string> before = sCurrentMap;
		ApplyMap();
		HashSet<string> changed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (KeyValuePair<string, string> kv in before)
		{
			if (!sCurrentMap.TryGetValue(kv.Key, out string now) || !string.Equals(now, kv.Value, StringComparison.Ordinal))
			{
				changed.Add(kv.Key);
			}
		}
		foreach (string k in sCurrentMap.Keys)
		{
			if (!before.ContainsKey(k))
			{
				changed.Add(k);
			}
		}
		LastReload = WardrobeHotReload.Apply(changed);
	}

	private static Dictionary<string, string> sCurrentMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private static void ApplyMap()
	{
		Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (string id in Worn)
		{
			WardrobePack pack = Find(id);
			if (pack == null)
			{
				continue;
			}
			foreach (WardrobeFile f in pack.Files)
			{
				map[f.Relative] = f.Source;
			}
		}
		sCurrentMap = map;
		WardrobeOverlay.SetMap(map);
	}

	private static void LoadState()
	{
		try
		{
			if (!File.Exists(StatePath) && File.Exists(OldStatePath))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(StatePath));
				File.Copy(OldStatePath, StatePath);
				File.Move(OldStatePath, OldStatePath + ".old", overwrite: true);
			}
			if (File.Exists(StatePath))
			{
				State st = JsonConvert.DeserializeObject<State>(File.ReadAllText(StatePath));
				Worn = (st?.worn ?? new List<string>()).Where((string id) => Find(id) != null).Distinct().ToList();
			}
		}
		catch (Exception ex)
		{
			Warnings.Add("wardrobe.json 读取失败：" + ex.Message);
			Worn = new List<string>();
		}
	}

	private static void SaveState()
	{
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(StatePath));
			string tmp = StatePath + ".tmp";
			File.WriteAllText(tmp, JsonConvert.SerializeObject(new State { worn = Worn }, Formatting.Indented));
			File.Move(tmp, StatePath, overwrite: true);
		}
		catch (Exception ex)
		{
			Warnings.Add("wardrobe.json 保存失败：" + ex.Message);
		}
	}
}
