// Wardrobe hot swap: after putting a patch on / taking it off, update what the game has already loaded
// so the change shows immediately (own implementation of the idea used by the Android 时装柜 mod).
//  - images (xnb/png/jpg) loaded through ResourceManager: swap the texture inside the existing Image objects,
//    including atlas sub-images that share the texture, then re-render the cached plant thumbnails
//  - Spine page textures (spanim/*.png): swap the page texture in every live atlas
//  - Spine skeleton / atlas text (spanim/*.atlas|*.skel): rebuild the battle definitions (new plants use it)
//  - sound effects and plant voices: reload / drop caches
//  - ContentManager caches: forget the changed assets so the next Load reads the patch
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Sexy.TodLib;
using Spine;

namespace Lawn;

public sealed class HotReloadResult
{
	public int Images;

	public int SpinePages;

	public int SpineSkeletons;

	public int Sounds;

	public bool Voices;

	public readonly List<string> NeedRestart = new List<string>();

	public readonly List<string> Errors = new List<string>();

	public string Summary()
	{
		List<string> parts = new List<string>();
		if (Images > 0)
		{
			parts.Add(Images + " 张图");
		}
		if (SpinePages > 0)
		{
			parts.Add(SpinePages + " 页角色贴图");
		}
		if (SpineSkeletons > 0)
		{
			parts.Add(SpineSkeletons + " 个角色骨骼");
		}
		if (Sounds > 0)
		{
			parts.Add(Sounds + " 个音效");
		}
		if (Voices)
		{
			parts.Add("语音");
		}
		string s = parts.Count > 0 ? "已即时换上 " + string.Join("、", parts) : string.Empty;
		if (NeedRestart.Count > 0)
		{
			s += (s.Length > 0 ? "；" : string.Empty) + string.Join("、", NeedRestart.Distinct()) + "要重开游戏才生效";
		}
		return s;
	}
}

public static class WardrobeHotReload
{
	private static readonly List<WeakReference<Atlas>> sAtlases = new List<WeakReference<Atlas>>();

	private static readonly List<WeakReference<ReanimatorDefinition>> sSpineDefs = new List<WeakReference<ReanimatorDefinition>>();

	private static readonly List<WeakReference<OverlayContentManager>> sManagers = new List<WeakReference<OverlayContentManager>>();

	private static FieldInfo[] sImageFields;

	public static void RegisterAtlas(Atlas theAtlas)
	{
		Register(sAtlases, theAtlas);
	}

	public static void RegisterSpineDefinition(ReanimatorDefinition theDef)
	{
		lock (sSpineDefs)
		{
			foreach (WeakReference<ReanimatorDefinition> w in sSpineDefs)
			{
				if (w.TryGetTarget(out ReanimatorDefinition d) && d == theDef)
				{
					return;
				}
			}
		}
		Register(sSpineDefs, theDef);
	}

	public static void RegisterManager(OverlayContentManager theManager)
	{
		Register(sManagers, theManager);
	}

	private static void Register<T>(List<WeakReference<T>> theList, T theItem) where T : class
	{
		if (theItem == null)
		{
			return;
		}
		lock (theList)
		{
			theList.RemoveAll((WeakReference<T> w) => !w.TryGetTarget(out _));
			theList.Add(new WeakReference<T>(theItem));
		}
	}

	private static List<T> Alive<T>(List<WeakReference<T>> theList) where T : class
	{
		lock (theList)
		{
			List<T> list = new List<T>();
			foreach (WeakReference<T> w in theList)
			{
				if (w.TryGetTarget(out T t) && t != null)
				{
					list.Add(t);
				}
			}
			return list;
		}
	}

	/// <summary>theChanged: content-relative paths (normalised) whose effective source differs from before.</summary>
	public static HotReloadResult Apply(HashSet<string> theChanged)
	{
		HotReloadResult r = new HotReloadResult();
		if (theChanged == null || theChanged.Count == 0)
		{
			return r;
		}
		foreach (OverlayContentManager cm in Alive(sManagers))
		{
			cm.Forget(theChanged);
		}
		bool imagesTouched = false;
		Try(r, "图片", () => r.Images = SwapImages(theChanged));
		imagesTouched = r.Images > 0;
		Try(r, "角色贴图", () => r.SpinePages = SwapSpinePages(theChanged));
		Try(r, "角色骨骼", () => r.SpineSkeletons = RebuildSpineDefinitions(theChanged));
		Try(r, "音效", () => r.Sounds = GlobalStaticVars.gSexyAppBase?.mResourceManager?.WardrobeReloadSounds(theChanged) ?? 0);
		if (theChanged.Any((string p) => p.StartsWith("voices/", StringComparison.Ordinal)))
		{
			Try(r, "语音", () =>
			{
				PlantVoice.ClearCache();
				r.Voices = true;
			});
		}
		if (imagesTouched || r.SpinePages > 0 || r.SpineSkeletons > 0)
		{
			Try(r, "植物缩略图", RefreshPlantThumbnails);
		}
		foreach (string p in theChanged)
		{
			if (p.StartsWith("reanim/", StringComparison.Ordinal))
			{
				r.NeedRestart.Add("动画");
			}
			else if (p.StartsWith("fonts/", StringComparison.Ordinal))
			{
				r.NeedRestart.Add("字体");
			}
			else if (p.StartsWith("particles/", StringComparison.Ordinal))
			{
				r.NeedRestart.Add("粒子特效");
			}
			else if (p.StartsWith("music/", StringComparison.Ordinal))
			{
				r.NeedRestart.Add("音乐");
			}
		}
		return r;
	}

	private static void Try(HotReloadResult r, string what, Action a)
	{
		try
		{
			a();
		}
		catch (Exception ex)
		{
			r.Errors.Add(what + "：" + ex.Message);
			Console.Error.WriteLine("Wardrobe hot swap (" + what + "): " + ex);
		}
	}

	// ------------------------------------------------------------------ images

	private static IEnumerable<Image> AllStaticImages()
	{
		if (sImageFields == null)
		{
			List<FieldInfo> f = new List<FieldInfo>();
			foreach (Type t in new[] { typeof(AtlasResources), typeof(Resources) })
			{
				f.AddRange(t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where((FieldInfo x) => x.FieldType == typeof(Image)));
			}
			sImageFields = f.ToArray();
		}
		foreach (FieldInfo fi in sImageFields)
		{
			if (fi.GetValue(null) is Image img)
			{
				yield return img;
			}
		}
	}

	private static int SwapImages(HashSet<string> theChanged)
	{
		ResourceManager rm = GlobalStaticVars.gSexyAppBase?.mResourceManager;
		if (rm == null)
		{
			return 0;
		}
		int n = 0;
		List<ResourceManager.ImageRes> loaded = rm.WardrobeLoadedImages();
		List<Image> statics = null;
		foreach (ResourceManager.ImageRes res in loaded)
		{
			string ext = res.mFormat switch
			{
				ResourceManager.ImageRes.TextureFormat.Png => ".png",
				ResourceManager.ImageRes.TextureFormat.Jpg => ".jpg",
				_ => ".xnb",
			};
			if (!theChanged.Contains(WardrobeOverlay.Normalize(res.mLoadedPath + ext)))
			{
				continue;
			}
			Texture2D oldTex = res.mImage.Texture;
			Texture2D newTex = rm.WardrobeLoadTexture(res);
			if (newTex == null)
			{
				continue;
			}
			res.mImage.WardrobeSwapTexture(newTex, theResize: true);
			n++;
			if (oldTex == null)
			{
				continue;
			}
			// atlas sub-images and any other Image that shared the old texture
			statics ??= AllStaticImages().ToList();
			foreach (Image img in statics)
			{
				if (img != res.mImage && ReferenceEquals(img.Texture, oldTex))
				{
					img.WardrobeSwapTexture(newTex, theResize: false);
				}
			}
			foreach (ResourceManager.ImageRes other in loaded)
			{
				if (other != res && ReferenceEquals(other.mImage.Texture, oldTex))
				{
					other.mImage.WardrobeSwapTexture(newTex, theResize: false);
				}
			}
		}
		return n;
	}

	// ------------------------------------------------------------------ spine

	private static int SwapSpinePages(HashSet<string> theChanged)
	{
		if (!theChanged.Any((string p) => p.StartsWith("spanim/", StringComparison.Ordinal) && (p.EndsWith(".png", StringComparison.Ordinal) || p.EndsWith(".jpg", StringComparison.Ordinal))))
		{
			return 0;
		}
		GraphicsDevice device = GlobalStaticVars.g.GraphicsDevice;
		Dictionary<string, Texture2D> loadedNow = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
		int n = 0;
		foreach (Atlas atlas in Alive(sAtlases))
		{
			foreach (AtlasPage page in atlas.Pages)
			{
				if (string.IsNullOrEmpty(page.name))
				{
					continue;
				}
				string path = "Content/spanim/" + page.name.Replace('\\', '/');
				string key = WardrobeOverlay.Normalize(path);
				if (!theChanged.Contains(key))
				{
					continue;
				}
				if (!loadedNow.TryGetValue(key, out Texture2D tex))
				{
					using (Stream s = WardrobeOverlay.Open(path))
					{
						tex = Texture2D.FromStream(device, s);
					}
					loadedNow[key] = tex;
				}
				page.rendererObject = tex;
				page.width = tex.Width;
				page.height = tex.Height;
				n++;
			}
		}
		return n;
	}

	private static int RebuildSpineDefinitions(HashSet<string> theChanged)
	{
		int n = 0;
		foreach (ReanimatorDefinition def in Alive(sSpineDefs))
		{
			if (string.IsNullOrEmpty(def.mSpineAtlasPath))
			{
				continue;
			}
			string atlas = WardrobeOverlay.Normalize(Path.ChangeExtension(def.mSpineAtlasPath, "atlas"));
			string skel = WardrobeOverlay.Normalize(Path.ChangeExtension(def.mSpineAtlasPath, "skel"));
			if (!theChanged.Contains(atlas) && !theChanged.Contains(skel))
			{
				continue;
			}
			def.mSpineSkeletonData = null;
			def.ExtractImages();
			n++;
		}
		return n;
	}

	// ------------------------------------------------------------------ cached plant pictures (seed packets, cursor)

	private static void RefreshPlantThumbnails()
	{
		ReanimatorCache cache = GlobalStaticVars.gLawnApp?.mReanimatorCache;
		if (cache?.mPlantImages == null)
		{
			return;
		}
		for (int i = 0; i < cache.mPlantImages.Count; i++)
		{
			if (cache.mPlantImages[i] == null)
			{
				continue;
			}
			cache.mPlantImages[i] = null;
			try
			{
				cache.MakeCachedPlantFrame((SeedType)i, DrawVariation.Normal);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Wardrobe thumbnail " + (SeedType)i + ": " + ex.Message);
			}
		}
		if (cache.mImageVariationList != null)
		{
			foreach (ReanimatorCache.CachedPlantVariation key in cache.mImageVariationList.Keys.ToList())
			{
				cache.mImageVariationList.Remove(key);
				try
				{
					cache.MakeCachedPlantFrame(key.mSeedType, key.mDrawVariation);
				}
				catch
				{
				}
			}
		}
	}
}
