using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Sexy;

namespace Lawn.Creative;

public static class CreativeLevelManager
{
	private static readonly Dictionary<GameMode, CreativeLevel> mLevels = new Dictionary<GameMode, CreativeLevel>(64);

	public static string mError;

	public static object readStateLock = new object();

	public static void Init()
	{
		lock (readStateLock)
		{
			mLevels.Clear();
		}
	}

	public static string ReloadDIYLevel()
	{
		LawnApp gLawnApp = GlobalStaticVars.gLawnApp;
		string text = Path.Combine(gLawnApp.applicationStoragePath, GlobalStaticVars.GetDocumentsDir() + "levels");
		lock (readStateLock)
		{
			int i;
			for (i = 0; mLevels.Remove((GameMode)(40000 + i)); i++)
			{
			}
			string[] files;
			try
			{
				Directory.CreateDirectory(text);
				files = Directory.GetFiles(text, "*.json", SearchOption.TopDirectoryOnly);
			}
			catch (Exception)
			{
				return "Can not access levels folder " + text;
			}
			Array.Sort(files, StringComparer.OrdinalIgnoreCase);
			string text2 = null;
			i = 0;
			string[] array = files;
			foreach (string path in array)
			{
				string fileName = Path.GetFileName(path);
				GameMode gameMode = (GameMode)(40000 + i);
				try
				{
					byte[] array2 = File.ReadAllBytes(path);
					ReadOnlyMemory<byte> json = array2;
					if (array2.Length >= 3 && array2[0] == 239 && array2[1] == 187 && array2[2] == 191)
					{
						json = json.Slice(3, json.Length - 3);
					}
					LoadLevel(json, gameMode, fileName);
					i++;
				}
				catch (Exception)
				{
					if (text2 == null)
					{
						text2 = "";
					}
					text2 = text2 + (mError ?? ("Can not access file " + fileName)) + "\n";
					mLevels.Remove(gameMode);
				}
			}
			return text2;
		}
	}

	public static void LoadLevel(ReadOnlyMemory<byte> json, GameMode gameMode, string file, string levelId = null)
	{
		mError = null;
		JsonDocument jsonDocument = null;
		try
		{
			jsonDocument = JsonDocument.Parse(json);
			CreativeLevel creativeLevel = new CreativeLevel();
			creativeLevel.Init(jsonDocument.RootElement);
			creativeLevel.mLevelId = levelId;
			mLevels[gameMode] = creativeLevel;
			SecretCrc secretCrc = default;
			secretCrc.Init();
			secretCrc.Update(json.Span);
			creativeLevel.mCrc = secretCrc.Value;
		}
		catch (Exception ex)
		{
			mError = ((file != null) ? (file + ":\n" + ex.Message) : ex.Message);
			throw;
		}
		finally
		{
			jsonDocument?.Dispose();
		}
	}

	public static bool IsCreativeMode(GameMode gameMode)
	{
		return mLevels.ContainsKey(gameMode);
	}

	public static CreativeLevel GetLevelDefine(GameMode gameMode)
	{
		if (!mLevels.TryGetValue(gameMode, out var value))
		{
			return null;
		}
		return value;
	}

	public static string GetLevelName(GameMode gameMode)
	{
		if (!mLevels.TryGetValue(gameMode, out var value) || value == null)
		{
			return null;
		}
		CSLevelProperty component = value.GetComponent<CSLevelProperty>();
		string result = component.mName;
		if (Constants.Language == Constants.LanguageIndex.zh_cn && component.mChineseName != null)
		{
			result = component.mChineseName;
		}
		if (Constants.Language == Constants.LanguageIndex.en && component.mEnglishName != null)
		{
			result = component.mEnglishName;
		}
		return result;
	}
}
