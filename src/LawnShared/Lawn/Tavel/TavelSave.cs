// What the game's save does not know about 塔维尔's plants (port of nineplants/save.py): a small text file next to
// the saved lawn ("<save>.tavel") with the ultimate stock, each plant's own state and each plant's lawn-wide state.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lawn;

public static class TavelSave
{
	private static string SidecarPath(string theFilePath) => theFilePath + ".tavel";

	private static string Esc(string t) => Uri.EscapeDataString(t ?? "");

	private static string Unesc(string t) => Uri.UnescapeDataString(t ?? "");

	public static void Save(Board b, string theFilePath)
	{
		if (!Tavel.Loaded)
		{
			return;
		}
		string path = SidecarPath(theFilePath);
		try
		{
			StringBuilder sb = new StringBuilder();
			bool any = false;
			if (b.mTavel != null)
			{
				sb.Append("U ").Append(b.mTavel.UltStock).Append(' ').Append(b.mTavel.UltCharge).Append('\n');
				any = true;
			}
			foreach (TavelSpec s in Tavel.Plants)
			{
				if (s.SaveBoard == null || (s.BoardHasData != null && !s.BoardHasData(b)))
				{
					continue;
				}
				string text = s.SaveBoard(b);
				if (text != null)
				{
					sb.Append("B ").Append(s.Code).Append(' ').Append(Esc(text)).Append('\n');
					any = true;
				}
			}
			foreach (Plant p in b.mPlants)
			{
				if (p.mDead)
				{
					continue;
				}
				TavelSpec s = Tavel.Get(p.mSeedType);
				if (s?.SaveState == null)
				{
					continue;
				}
				string text = s.SaveState(p);
				if (text != null)
				{
					sb.Append("P ").Append(p.mPlantCol).Append(' ').Append(p.mRow).Append(' ').Append((int)p.mSeedType).Append(' ').Append(Esc(text)).Append('\n');
					any = true;
				}
			}
			if (any)
			{
				File.WriteAllText(path, sb.ToString());
			}
			else if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
		catch (Exception ex)
		{
			Tavel.Log("save: " + ex.Message);
		}
	}

	/// <summary>Before the game reads the lawn: nothing remembered from the lawn being replaced applies.</summary>
	public static void BeforeLoad(Board b)
	{
		b.mTavel = null;
	}

	public static void AfterLoad(Board b, string theFilePath)
	{
		if (!Tavel.Loaded)
		{
			return;
		}
		string path = SidecarPath(theFilePath);
		if (!File.Exists(path))
		{
			return;
		}
		try
		{
			foreach (string line in File.ReadAllLines(path))
			{
				string[] f = line.Split(' ');
				if (f.Length >= 3 && f[0] == "U")
				{
					TavelBoard s = Tavel.State(b);
					s.UltStock = Math.Max(0, Math.Min(Tavel.UltimateMaxStock, int.Parse(f[1])));
					s.UltCharge = Math.Max(0, int.Parse(f[2]));
				}
				else if (f.Length >= 3 && f[0] == "B")
				{
					TavelSpec spec = Tavel.Plants.Find((TavelSpec q) => q.Code == f[1]);
					spec?.LoadBoard?.Invoke(b, Unesc(f[2]));
				}
				else if (f.Length >= 5 && f[0] == "P")
				{
					int col = int.Parse(f[1]);
					int row = int.Parse(f[2]);
					SeedType seed = (SeedType)int.Parse(f[3]);
					TavelSpec spec = Tavel.Get(seed);
					if (spec?.LoadState == null)
					{
						continue;
					}
					foreach (Plant p in b.mPlants)
					{
						if (!p.mDead && p.mPlantCol == col && p.mRow == row && p.mSeedType == seed)
						{
							spec.LoadState(p, Unesc(f[4]));
							break;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Tavel.Log("load: " + ex.Message);
		}
	}
}
