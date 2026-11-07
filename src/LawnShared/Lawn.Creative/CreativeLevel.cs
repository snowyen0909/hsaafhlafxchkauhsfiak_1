using System;
using System.Collections.Generic;
using System.Text.Json;
using Sexy;

namespace Lawn.Creative;

public class CreativeLevel : IDisposable
{
	private static readonly Dictionary<string, Func<int, IComponent>> sCtors;

	private readonly List<IComponent> mComponents;

	private bool disposedValue;

	public uint mCrc;

	public string mLevelId;

	public int mVersion;

	static CreativeLevel()
	{
		sCtors = new Dictionary<string, Func<int, IComponent>>();
		Regist<CSAutoZombiePoint>();
		Regist<CSBungeeBlitz>();
		Regist<CSColumnPlanting>();
		Regist<CSConveyorBelt>();
		Regist<CSDefaultPlantOnLawn>();
		Regist<CSInvisibleZombie>();
		Regist<CSIZombieLevel>();
		Regist<CSLevelProperty>();
		Regist<CSManualZombieCountDown>();
		Regist<CSManualZombiePoint>();
		Regist<CSMustHaveZombie>();
		Regist<CSOverridePlantProperty>();
		Regist<CSOverrideProjectileProperty>();
		Regist<CSOverrideZombieProperty>();
		Regist<CSRainingSeeds>();
		Regist<CSSeedBank>();
		Regist<CSSlotMachineLevel>();
		Regist<CSSpawnFog>();
		Regist<CSSpawnGraveStone>();
		Regist<CSSpawnLadder>();
		Regist<CSSpawnPortal>();
		Regist<CSSpawnSkySun>();
		Regist<CSSpeed>();
		Regist<CSSurvivalLevel>();
		Regist<CSVaseLevel>();
		Regist<CSWallnutBowlingLevel>();
		Regist<CSWeather>();
		Regist<CSGlove>();
		Regist<CSPause>();
		Regist<CSPlantSkill>();
		Regist<CSFusion>();
		Regist<CSExtendedPoolZombies>();
		Regist<CSTrashcan>();
		Regist<CSShovel>();
		Regist<CSWhackAZombieLevel>();
		static void Regist<T>() where T : class, IComponent, new()
		{
			string name = typeof(T).Name;
			Debug.ASSERT(name.StartsWith("CS"));
			Dictionary<string, Func<int, IComponent>> dictionary = sCtors;
			string text = name;
			dictionary.Add(text.Substring(2, text.Length - 2), (int v) => new T
			{
				Version = v
			});
		}
	}

	public CreativeLevel()
	{
		mComponents = new List<IComponent>();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void Init(JsonElement root)
	{
		if (!root.TryGetProperty("Version", out var value) || !value.TryGetInt32(out mVersion) || !IsValidVersion())
		{
			throw new Exception("Version checks failed");
		}
		if (!root.TryGetProperty("Components", out var value2) || value2.ValueKind != JsonValueKind.Array)
		{
			throw new Exception("Can not find any components");
		}
		foreach (JsonElement item in value2.EnumerateArray())
		{
			IComponent component = null;
			if (item.TryGetProperty("Type", out var value3) && value3.ValueKind == JsonValueKind.String)
			{
				string key = value3.GetString();
				if (sCtors.TryGetValue(key, out var value4))
				{
					component = value4(mVersion);
					component.Init(item);
					mComponents.Add(component);
				}
			}
		}
		bool flag = false;
		foreach (IComponent mComponent in mComponents)
		{
			flag |= mComponent is CSLevelProperty;
			try
			{
				mComponent.Check(this);
			}
			catch (Exception ex)
			{
				string name = mComponent.GetType().Name;
				string text = name;
				throw new Exception("Check error(" + text.Substring(2, text.Length - 2) + "): " + ex.Message);
			}
		}
		if (!flag)
		{
			throw new Exception("Check error: No LevelProperty Component!");
		}
	}

	public T GetComponent<T>() where T : class, IComponent
	{
		for (int i = 0; i < mComponents.Count; i++)
		{
			if (mComponents[i] is T)
			{
				return mComponents[i] as T;
			}
		}
		return null;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		foreach (IComponent mComponent in mComponents)
		{
			if (mComponent is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		mComponents.Clear();
		disposedValue = true;
	}

	public bool IsValidVersion()
	{
		int num = mVersion;
		if ((uint)(num - 1) <= 2u)
		{
			return true;
		}
		return false;
	}

	~CreativeLevel()
	{
		Dispose(disposing: false);
	}
}
