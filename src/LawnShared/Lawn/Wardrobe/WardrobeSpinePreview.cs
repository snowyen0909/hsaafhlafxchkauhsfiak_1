// Wardrobe preview of a Spine character patch: assembles the skeleton with the pack's page textures
// (falling back to the game's own files for anything the pack does not replace) and plays its idle loop,
// instead of showing the raw atlas pages.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Sexy.TodLib;
using Spine;

namespace Lawn;

public sealed class WardrobeSpinePreview : IDisposable
{
	private static readonly Regex sPageSuffix = new Regex("_\\d+$", RegexOptions.Compiled);

	private readonly Atlas mAtlas;

	private readonly Skeleton mSkeleton;

	private readonly AnimationState mState;

	private readonly SkeletonRenderer mRenderer;

	private float mBX;

	private float mBY;

	private float mBW;

	private float mBH;

	private float[] mVertexBuffer = new float[64];

	private int mLastCounter = -1;

	public string Name { get; }

	private WardrobeSpinePreview(string theName, Atlas theAtlas, SkeletonData theData)
	{
		Name = theName;
		mAtlas = theAtlas;
		mSkeleton = new Skeleton(theData);
		mState = new AnimationState(new AnimationStateData(theData));
		Spine.Animation anim = theData.Animations.FirstOrDefault((Spine.Animation a) => a.Name == "idle")
			?? theData.Animations.FirstOrDefault((Spine.Animation a) => a.Name.IndexOf("idle", StringComparison.OrdinalIgnoreCase) >= 0)
			?? theData.Animations.FirstOrDefault();
		if (anim != null)
		{
			mState.SetAnimation(0, anim, loop: true);
		}
		mState.Apply(mSkeleton);
		mSkeleton.UpdateWorldTransform(Skeleton.Physics.Pose);
		MeasureBounds();
		GraphicsDevice device = GlobalStaticVars.g.GraphicsDevice;
		mRenderer = new SkeletonRenderer(device);
		mRenderer.PremultipliedAlpha = true;
		Effect effect = GlobalStaticVars.gLawnApp.mContentManager.Load<Effect>("shaders\\LawnSpineEffect").Clone();
		effect.Parameters["World"].SetValue(Matrix.Identity);
		effect.Parameters["View"].SetValue(Matrix.CreateLookAt(new Vector3(0f, 0f, 1f), Vector3.Zero, Vector3.Up));
		effect.Parameters["ExtraAdditiveColor"]?.SetValue(Vector4.Zero);
		mRenderer.Effect = effect;
	}

	/// <summary>Bounds over the first second of the loop so the whole motion fits the box.</summary>
	private void MeasureBounds()
	{
		float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
		for (int i = 0; i < 6; i++)
		{
			mSkeleton.GetBounds(out float x, out float y, out float w, out float h, ref mVertexBuffer);
			if (w > 0f && h > 0f && !float.IsInfinity(w) && !float.IsInfinity(h))
			{
				minX = Math.Min(minX, x);
				minY = Math.Min(minY, y);
				maxX = Math.Max(maxX, x + w);
				maxY = Math.Max(maxY, y + h);
			}
			mState.Update(0.2f);
			mState.Apply(mSkeleton);
			mSkeleton.UpdateWorldTransform(Skeleton.Physics.Pose);
		}
		if (maxX <= minX || maxY <= minY)
		{
			minX = -100f;
			minY = -100f;
			maxX = 100f;
			maxY = 100f;
		}
		mBX = minX;
		mBY = minY;
		mBW = maxX - minX;
		mBH = maxY - minY;
	}

	/// <summary>Builds a preview if the pack contains a Spine character (spanim pages / atlas / skeleton).</summary>
	public static WardrobeSpinePreview TryCreate(WardrobePack thePack)
	{
		Dictionary<string, string> packFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		List<string> stems = new List<string>();
		foreach (WardrobeFile f in thePack.Files)
		{
			if (!f.Relative.StartsWith("spanim/", StringComparison.Ordinal))
			{
				continue;
			}
			packFiles[f.Relative] = f.Source;
			string ext = Path.GetExtension(f.Relative);
			if (ext != ".png" && ext != ".jpg" && ext != ".atlas" && ext != ".skel" && ext != ".json")
			{
				continue;
			}
			string stem = f.Relative.Substring(0, f.Relative.Length - ext.Length);
			string baseStem = sPageSuffix.Replace(stem, string.Empty);
			if (!stems.Contains(baseStem))
			{
				stems.Add(baseStem);
			}
			if (!stems.Contains(stem))
			{
				stems.Add(stem);
			}
		}
		foreach (string stem in stems)
		{
			WardrobeSpinePreview p = TryBuild(stem, packFiles);
			if (p != null)
			{
				return p;
			}
		}
		return null;
	}

	private static Stream OpenPreferPack(string theRelative, Dictionary<string, string> thePack)
	{
		if (thePack.TryGetValue(theRelative, out string src) && File.Exists(src))
		{
			return new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.Read, 81920);
		}
		return TitleContainer.OpenStream("Content/" + theRelative);
	}

	private static bool Exists(string theRelative, Dictionary<string, string> thePack)
	{
		if (thePack.ContainsKey(theRelative))
		{
			return true;
		}
		try
		{
			using Stream s = TitleContainer.OpenStream("Content/" + theRelative);
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static WardrobeSpinePreview TryBuild(string theStem, Dictionary<string, string> thePack)
	{
		string atlasRel = theStem + ".atlas";
		bool binary = Exists(theStem + ".skel", thePack);
		if (!Exists(atlasRel, thePack) || (!binary && !Exists(theStem + ".json", thePack)))
		{
			return null;
		}
		Atlas atlas = null;
		try
		{
			string dir = Path.GetDirectoryName(atlasRel)?.Replace('\\', '/') ?? string.Empty;
			using (StreamReader reader = new StreamReader(OpenPreferPack(atlasRel, thePack)))
			{
				atlas = new Atlas(reader, dir, new PackTextureLoader(thePack));
			}
			SkeletonData data;
			if (binary)
			{
				using Stream s = OpenPreferPack(theStem + ".skel", thePack);
				using MemoryStream ms = new MemoryStream();
				s.CopyTo(ms);
				ms.Position = 0;
				data = new SkeletonBinary(atlas).ReadSkeletonData(ms);
			}
			else
			{
				using StreamReader r = new StreamReader(OpenPreferPack(theStem + ".json", thePack));
				data = new SkeletonJson(atlas).ReadSkeletonData(r);
			}
			return new WardrobeSpinePreview(Path.GetFileName(theStem), atlas, data);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Wardrobe spine preview " + theStem + ": " + ex.Message);
			atlas?.Dispose();
			return null;
		}
	}

	/// <summary>Draws the animated character fitted into theRect (dialog coordinates).</summary>
	public void Draw(Graphics g, TRect theRect, int theAppCounter)
	{
		float dt = mLastCounter < 0 ? 0f : Math.Clamp((theAppCounter - mLastCounter) / 100f, 0f, 0.1f);
		mLastCounter = theAppCounter;
		mSkeleton.X = 0f;
		mSkeleton.Y = 0f;
		mSkeleton.ScaleX = 1f;
		mSkeleton.ScaleY = 1f;
		mState.Update(dt);
		mState.Apply(mSkeleton);
		mSkeleton.Update(dt);
		mSkeleton.UpdateWorldTransform(Skeleton.Physics.Update);
		float k = Math.Min(theRect.mWidth / mBW, theRect.mHeight / mBH);
		float cx = theRect.mX + theRect.mWidth / 2f + g.mTransX;
		float cy = theRect.mY + theRect.mHeight / 2f + g.mTransY;
		Graphics.GetTransform(out Matrix? transform);
		Matrix world = Matrix.CreateTranslation(-(mBX + mBW / 2f), -(mBY + mBH / 2f), 0f) * Matrix.CreateScale(k, k, 1f) * Matrix.CreateTranslation(cx, cy, 0f) * (transform ?? Matrix.Identity);
		g.EndFrame();
		mRenderer.Effect.Parameters["Projection"].SetValue(g.SpineGetMatrices());
		mRenderer.Effect.Parameters["World"].SetValue(world);
		mSkeleton.R = mSkeleton.G = mSkeleton.B = mSkeleton.A = 1f;
		mRenderer.Begin();
		mRenderer.Draw(mSkeleton);
		mRenderer.End();
	}

	public void Dispose()
	{
		mAtlas?.Dispose();
		mRenderer?.Effect?.Dispose();
	}

	/// <summary>Page textures: the pack's file when it has one, otherwise the game's original.</summary>
	private sealed class PackTextureLoader : TextureLoader
	{
		private readonly Dictionary<string, string> mPack;

		public PackTextureLoader(Dictionary<string, string> thePack)
		{
			mPack = thePack;
		}

		public void Load(AtlasPage page, string path)
		{
			string rel = WardrobeOverlay.Normalize(path);
			Texture2D texture;
			using (Stream stream = OpenPreferPack(rel, mPack))
			{
				texture = Texture2D.FromStream(GlobalStaticVars.g.GraphicsDevice, stream);
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
}
