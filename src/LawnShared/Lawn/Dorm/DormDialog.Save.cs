// 宿舍 saves: one file per game profile, userdata/mod/profile<id>/dorm.json (+ dorm.json.bak).
//   { "version": 1,
//     "floors": [ { "size": 8, "wall": "...", "floor": "...", "layer": "-*60,floor_white*4" (tiles; "-" none; only when laid;
//                   shown on a plain-colour floor, put away under a whole floor picture),
//                   "items": [ {"id","i","j","rot","side"} ], "girls": ["sunflower", ...] } ],
//     "view": { "hideWalls": false },
//     "owned": { "sofa2": [500, 500], ... },          // what each piece cost (coins), so selling refunds half of that
//     "quiz": { "day", "today", "right", "seen": [...] }
//   }
// Written to dorm.json.tmp first, then swapped in (the previous file becomes dorm.json.bak); a damaged dorm.json falls
// back to the .bak. The old shared files (userdata/dorm.txt, dorm_owned.txt, dorm_girls.txt, dorm_quiz.txt) are read
// once into the first profile that opens the dorm and renamed to *.old.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lawn;

internal sealed partial class DormDialog
{
	private const int SaveVersion = 1;

	/// <summary>Price paid for each piece in the warehouse or the room, in coins (newest last).</summary>
	private readonly Dictionary<string, List<int>> mPaid = new Dictionary<string, List<int>>();

	/// <summary>Room layout read from the save, applied once the owned pieces are known.</summary>
	private string mLoadedLayout;

	/// <summary>No dorm.json yet for this profile: written once the room is set up.</summary>
	private bool mFreshSave;


	private static string ProfileDir(LawnApp app) => Path.Combine(DormPlatform.ModSaveDir, "profile" + DormPlatform.ProfileId(app));

	private string JsonPath => Path.Combine(ProfileDir(mLawn), "dorm.json");

	/// <summary>Room layout of the save, still in the old one-line-per-piece text (the dirty check compares these).</summary>
	internal static string SavePath => Path.Combine(DormPlatform.SaveDir, "dorm.txt");

	// ------------------------------------------------------------------ read

	private void ReadSave()
	{
		mOwned.Clear();
		mPaid.Clear();
		mChosen.Clear();
		mLoadedLayout = null;
		JObject root = TryRead(JsonPath) ?? TryRead(JsonPath + ".bak");
		bool fresh = root == null;
		if (fresh)
		{
			root = MigrateOldFiles();
		}
		if (root != null)
		{
			FromJson(root);
		}
		if (root == null || (fresh && root["floors"]?[0]?["girls"] == null))
		{
			mChosen.AddRange(new[] { "sunflower", "marigold", "peashooter", "gatlingpea" }.Where((string s) => Unlocked(FindGirl(s))));
		}
		mFreshSave = fresh;
		if (mFloors.Count == 0)
		{
			mFloors.Add(new FloorState());
		}
	}

	private static JObject TryRead(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				return JObject.Parse(File.ReadAllText(path));
			}
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm save " + Path.GetFileName(path) + ": " + ex.Message);
		}
		return null;
	}

	private void FromJson(JObject root)
	{
		if (root["owned"] is JObject owned)
		{
			foreach (JProperty p in owned.Properties())
			{
				if (FindDef(p.Name) == null || !(p.Value is JArray arr))
				{
					continue;
				}
				List<int> prices = arr.Select((JToken t) => (int)t).Where((int v) => v >= 0).ToList();
				if (prices.Count > 0)
				{
					mPaid[p.Name] = prices;
					mOwned[p.Name] = prices.Count;
				}
			}
		}
		mFloors.Clear();
		mFloor = 0;
		if (root["floors"] is JArray floors)
		{
			HashSet<string> seen = new HashSet<string>();
			foreach (JObject f in floors.OfType<JObject>().Take(MaxFloors))
			{
				int size = ClampSize((int?)f["size"] ?? 8);
				List<string> lines = new List<string> { "@size " + size, "@wall " + ((string)f["wall"] ?? "wall_sakura"), "@floor " + ((string)f["floor"] ?? "floor_tatami") };
				if (f["tiles"] is JValue tv && tv.Type == JTokenType.String)
				{
					lines.Add("@tiles " + (string)tv);
				}
				if (f["layer"] is JValue lv && lv.Type == JTokenType.String)
				{
					lines.Add("@layer " + (string)lv);
				}
				if (f["items"] is JArray items)
				{
					foreach (JObject it in items.OfType<JObject>())
					{
						lines.Add($"{(string)it["id"]} {(int?)it["i"] ?? 0} {(int?)it["j"] ?? 0} {(int?)it["rot"] ?? 0} {(int?)it["side"] ?? 0}");
					}
				}
				FloorState fs = new FloorState { Layout = string.Join("\n", lines) };
				if (f["girls"] is JArray girls)
				{
					foreach (string skel in girls.Select((JToken t) => (string)t))
					{
						GirlDef def = FindGirl(skel);
						if (def != null && Unlocked(def) && seen.Add(skel) && fs.Girls.Count < CapacityFor(size))
						{
							fs.Girls.Add(skel);
						}
					}
				}
				mFloors.Add(fs);
			}
			if (mFloors.Count > 0)
			{
				mLoadedLayout = mFloors[0].Layout;
				mChosen.AddRange(mFloors[0].Girls);
			}
		}
		mWallsHidden = root["view"] is JObject view && ((bool?)view["hideWalls"] ?? false);
		mTreeAlways = (bool?)root["test"]?["treeAlways"] ?? false;
		mTestTrophies = (bool?)root["test"]?["trophies"] ?? false;
		if (root["quiz"] is JObject q)
		{
			mQuizDay = (string)q["day"] ?? "";
			mQuizToday = (int?)q["today"] ?? 0;
			mQuizTotalRight = (int?)q["right"] ?? 0;
			mQuizSeen.Clear();
			if (q["seen"] is JArray seen)
			{
				foreach (JToken t in seen)
				{
					mQuizSeen.Add((string)t);
				}
			}
		}
	}

	/// <summary>The shared text saves of earlier test builds, turned into this profile's dorm.json (pieces count as bought at the test price).</summary>
	private JObject MigrateOldFiles()
	{
		string dir = DormPlatform.SaveDir;
		string[] names = { "dorm.txt", "dorm_owned.txt", "dorm_girls.txt", "dorm_quiz.txt" };
		if (!names.Any((string n) => File.Exists(Path.Combine(dir, n))))
		{
			return null;
		}
		JObject root = new JObject { ["version"] = SaveVersion };
		try
		{
			string owned = Path.Combine(dir, "dorm_owned.txt");
			JObject o = new JObject();
			if (File.Exists(owned))
			{
				foreach (string line in File.ReadAllLines(owned))
				{
					string[] p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
					if (p.Length >= 2 && FindDef(p[0]) != null && int.TryParse(p[1], out int n) && n > 0)
					{
						o[p[0]] = new JArray(Enumerable.Repeat(TestPrice, n));
					}
				}
			}
			root["owned"] = o;
			JObject floor = new JObject();
			string layout = Path.Combine(dir, "dorm.txt");
			if (File.Exists(layout))
			{
				FloorFromText(File.ReadAllText(layout), floor);
			}
			string girls = Path.Combine(dir, "dorm_girls.txt");
			if (File.Exists(girls))
			{
				floor["girls"] = new JArray(File.ReadAllLines(girls).Select((string s) => s.Trim()).Where((string s) => s.Length > 0));
			}
			root["floors"] = new JArray(floor);
			string quiz = Path.Combine(dir, "dorm_quiz.txt");
			if (File.Exists(quiz))
			{
				JObject q = new JObject();
				foreach (string line in File.ReadAllLines(quiz))
				{
					int eq = line.IndexOf('=');
					if (eq <= 0)
					{
						continue;
					}
					string k = line.Substring(0, eq), v = line.Substring(eq + 1);
					if (k == "seen")
					{
						q[k] = new JArray(v.Split(',', StringSplitOptions.RemoveEmptyEntries));
					}
					else if (k == "day")
					{
						q[k] = v;
					}
					else if (int.TryParse(v, out int n))
					{
						q[k] = n;
					}
				}
				root["quiz"] = q;
			}
			foreach (string n in names)
			{
				string f = Path.Combine(dir, n);
				if (File.Exists(f))
				{
					File.Move(f, f + ".old", overwrite: true);
				}
			}
			Console.WriteLine("Dorm: old saves moved into profile " + DormPlatform.ProfileId(mLawn));
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm migrate: " + ex.Message);
		}
		return root;
	}

	/// <summary>Test builds sold everything for $100: what an old piece counts as having cost.</summary>
	private const int TestPrice = 10;

	private static void FloorFromText(string text, JObject floor)
	{
		JArray items = new JArray();
		floor["size"] = 8;
		floor["wall"] = "wall_sakura";
		floor["floor"] = "floor_tatami";
		foreach (string line in text.Split('\n'))
		{
			string[] p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (p.Length == 2 && p[0] == "@size")
			{
				floor["size"] = ClampSize(int.TryParse(p[1], out int n) ? n : 8);
				continue;
			}
			if (p.Length == 2 && (p[0] == "@tiles" || p[0] == "@layer"))
			{
				floor[p[0] == "@tiles" ? "tiles" : "layer"] = p[1];
				continue;
			}
			if (p.Length == 2 && (p[0] == "@wall" || p[0] == "@floor"))
			{
				floor[p[0].Substring(1)] = p[1];
				continue;
			}
			if (p.Length < 5 || p[0].StartsWith("#") || !int.TryParse(p[1], out int i) || !int.TryParse(p[2], out int j) || !int.TryParse(p[3], out int rot) || !int.TryParse(p[4], out int side))
			{
				continue;
			}
			items.Add(new JObject { ["id"] = p[0], ["i"] = i, ["j"] = j, ["rot"] = rot, ["side"] = side });
		}
		floor["items"] = items;
	}

	// ------------------------------------------------------------------ write

	/// <summary>Everything to dorm.json: the room as last saved (unsaved arranging is not written), owned pieces, girls, quiz.</summary>
	private bool WriteSave()
	{
		try
		{
			SyncFloor();
			JArray floorArr = new JArray();
			foreach (FloorState fs in mFloors)
			{
				JObject floor = new JObject();
				FloorFromText(fs.Layout, floor);
				floor["girls"] = new JArray(fs.Girls);
				floorArr.Add(floor);
			}
			JObject owned = new JObject();
			foreach (KeyValuePair<string, List<int>> kv in mPaid.Where((KeyValuePair<string, List<int>> kv) => kv.Value.Count > 0))
			{
				owned[kv.Key] = new JArray(kv.Value);
			}
			JObject root = new JObject
			{
				["version"] = SaveVersion,
				["floors"] = floorArr,
				["owned"] = owned,
				["view"] = new JObject { ["hideWalls"] = mWallsHidden },
				["test"] = new JObject { ["treeAlways"] = mTreeAlways, ["trophies"] = mTestTrophies },
				["quiz"] = new JObject { ["day"] = mQuizDay, ["today"] = mQuizToday, ["right"] = mQuizTotalRight, ["seen"] = new JArray(mQuizSeen) }
			};
			string path = JsonPath;
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			string tmp = path + ".tmp";
			File.WriteAllText(tmp, root.ToString(Formatting.Indented));
			if (File.Exists(path))
			{
				File.Copy(path, path + ".bak", overwrite: true);
			}
			File.Move(tmp, path, overwrite: true);
			return true;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm save: " + ex.Message);
			return false;
		}
	}

	// ------------------------------------------------------------------ owned pieces

	private void AddOwned(Def def, int paid)
	{
		if (!mPaid.TryGetValue(def.Id, out List<int> list))
		{
			list = mPaid[def.Id] = new List<int>();
		}
		if (def.Kind == Kind.Style)
		{
			list.Clear();
		}
		list.Add(paid);
		mOwned[def.Id] = list.Count;
	}

	/// <summary>Half of what the newest piece of this kind cost (rounded down).</summary>
	private int SellPrice(Def d) => mPaid.TryGetValue(d.Id, out List<int> list) && list.Count > 0 ? list[list.Count - 1] / 2 : d.Price / 2;

	/// <summary>Takes the newest piece of this kind out of the owned list; returns its refund.</summary>
	private int RemoveOwned(Def d)
	{
		int refund = SellPrice(d);
		if (mPaid.TryGetValue(d.Id, out List<int> list) && list.Count > 0)
		{
			list.RemoveAt(list.Count - 1);
			if (list.Count == 0)
			{
				mPaid.Remove(d.Id);
			}
		}
		int n = Owned(d) - 1;
		if (n <= 0)
		{
			mOwned.Remove(d.Id);
		}
		else
		{
			mOwned[d.Id] = n;
		}
		return refund;
	}
}
