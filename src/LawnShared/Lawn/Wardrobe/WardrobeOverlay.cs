// Wardrobe (时装柜) asset overlay for the iOS mod edition.
// The Android mod injected this with MonoMod detours on TitleContainer.OpenStream; iOS cannot patch code
// at runtime, so every place the game opens a content file goes through WardrobeOverlay.Open instead.
// In the normal / comic editions the overlay map is always empty and Open() is a straight pass-through.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace Lawn;

public static class WardrobeOverlay
{
	/// <summary>content-relative path (lower case, '/' separators, no "Content/" prefix) -> patch file on disk.</summary>
	private static volatile Dictionary<string, string> sMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	public static int Count => sMap.Count;

	public static void SetMap(Dictionary<string, string> theMap)
	{
		sMap = theMap ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	}

	public static string Normalize(string thePath)
	{
		if (string.IsNullOrEmpty(thePath))
		{
			return string.Empty;
		}
		string p = thePath.Replace('\\', '/');
		while (p.StartsWith("./", StringComparison.Ordinal))
		{
			p = p.Substring(2);
		}
		while (p.Contains("//"))
		{
			p = p.Replace("//", "/");
		}
		p = p.Replace("/./", "/");
		p = p.TrimStart('/');
		if (p.StartsWith("Content/", StringComparison.OrdinalIgnoreCase))
		{
			p = p.Substring("Content/".Length);
		}
		return p.ToLowerInvariant();
	}

	public static bool TryGetPatch(string thePath, out string theSource)
	{
		theSource = null;
		Dictionary<string, string> map = sMap;
		if (map.Count == 0)
		{
			return false;
		}
		return map.TryGetValue(Normalize(thePath), out theSource) && File.Exists(theSource);
	}

	/// <summary>Opens a content file ("Content/..." path), preferring a worn wardrobe patch.</summary>
	public static Stream Open(string thePath)
	{
		if (TryGetPatch(thePath, out string source))
		{
			try
			{
				return new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, 81920);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Wardrobe: cannot open patch " + source + ": " + ex.Message);
			}
		}
		return TitleContainer.OpenStream(thePath);
	}

	/// <summary>Spine atlas whose text and page textures both go through the overlay.</summary>
	public static Atlas LoadAtlas(string theAtlasPath, GraphicsDevice theDevice)
	{
		string text;
		using (StreamReader r = new StreamReader(Open(theAtlasPath)))
		{
			text = r.ReadToEnd();
		}
		// the game's atlases are exported premultiplied; 塔维尔's are not (no "pma:true" line)
		bool premultiply = Normalize(theAtlasPath).StartsWith("tavel/", StringComparison.Ordinal) && !System.Text.RegularExpressions.Regex.IsMatch(text, @"(?m)^\s*pma\s*:\s*true");
		using StringReader reader = new StringReader(text);
		string dir = Path.GetDirectoryName(theAtlasPath) ?? string.Empty;
		Atlas atlas = new Atlas(reader, dir, new OverlayTextureLoader(theDevice, premultiply));
		WardrobeHotReload.RegisterAtlas(atlas);
		return atlas;
	}

	public static SkeletonData ReadSkeletonBinary(SkeletonBinary theReader, string theSkelPath)
	{
		using Stream s = Open(theSkelPath);
		using MemoryStream ms = new MemoryStream();
		s.CopyTo(ms);
		ms.Position = 0;
		return theReader.ReadSkeletonData(ms);
	}

	public static SkeletonData ReadSkeletonJson(SkeletonJson theReader, string theJsonPath)
	{
		using StreamReader reader = new StreamReader(Open(theJsonPath));
		return theReader.ReadSkeletonData(reader);
	}
}

/// <summary>Spine page loader that honours wardrobe patches (spanim/*.png).</summary>
public sealed class OverlayTextureLoader : TextureLoader
{
	private readonly GraphicsDevice mDevice;

	private readonly bool mPremultiply;

	public OverlayTextureLoader(GraphicsDevice theDevice, bool thePremultiply = false)
	{
		mDevice = theDevice;
		mPremultiply = thePremultiply;
	}

	public void Load(AtlasPage page, string path)
	{
		Texture2D texture;
		using (Stream stream = WardrobeOverlay.Open(path))
		{
			texture = mPremultiply ? Texture2D.FromStream(mDevice, stream, DefaultColorProcessors.PremultiplyAlpha) : Texture2D.FromStream(mDevice, stream);
		}
		page.width = texture.Width;
		page.height = texture.Height;
		page.rendererObject = texture;
	}

	public void Unload(object texture)
	{
		((Texture2D)texture).Dispose();
	}
}

/// <summary>ContentManager whose .xnb reads go through the wardrobe overlay.</summary>
public sealed class OverlayContentManager : ContentManager
{
	public OverlayContentManager(IServiceProvider theServices)
		: base(theServices)
	{
		WardrobeHotReload.RegisterManager(this);
	}

	public OverlayContentManager(IServiceProvider theServices, string theRoot)
		: base(theServices, theRoot)
	{
		WardrobeHotReload.RegisterManager(this);
	}

	/// <summary>Drops cached assets whose file changed, so the next Load reads the (new) effective file.</summary>
	public void Forget(HashSet<string> theChanged)
	{
		foreach (string key in LoadedAssets.Keys.ToList())
		{
			string rel = WardrobeOverlay.Normalize(Path.Combine(RootDirectory ?? string.Empty, key) + ".xnb");
			if (theChanged.Contains(rel))
			{
				LoadedAssets.Remove(key);
			}
		}
	}

	protected override Stream OpenStream(string assetName)
	{
		if (WardrobeOverlay.Count > 0)
		{
			string path = Path.Combine(RootDirectory ?? string.Empty, assetName) + ".xnb";
			if (WardrobeOverlay.TryGetPatch(path, out _))
			{
				return WardrobeOverlay.Open(path);
			}
		}
		return base.OpenStream(assetName);
	}
}
