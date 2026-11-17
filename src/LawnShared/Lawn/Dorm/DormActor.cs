// 宿舍 (dorm) test: one plant girl walking around the room, drawn from the game's own Spine skeleton
// (worn wardrobe patches apply, because loading goes through WardrobeOverlay).
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Spine;

namespace Lawn;

internal sealed class DormActor : IDisposable
{
	public enum State
	{
		Idle,
		Walk,
		Sleep
	}

	public readonly SeedType Seed;

	public readonly string Name;

	private readonly Atlas mAtlas;

	private readonly Skeleton mSkeleton;

	private readonly AnimationState mAnim;

	private readonly SkeletonRenderer mRenderer;

	private readonly bool mHasSleep;

	/// <summary>Leg/arm motion layered on top of idle while walking (skeleton "walk", or dorm/walk/&lt;name&gt;.json).</summary>
	private readonly Spine.Animation mWalk;

	public bool HasCustomWalk => mWalk != null;

	/// <summary>An animator-made "walk" inside the skeleton replaces idle; our JSON walk is layered on top of it.</summary>
	private readonly bool mWalkIsNative;

	/// <summary>Bones the walk keys: reset to setup before each apply, or the additive offsets pile up frame after frame.</summary>
	private readonly List<Bone> mWalkBones = new List<Bone>();

	/// <summary>Walk cycle length in seconds (two steps).</summary>
	public float WalkCycle => mWalk?.Duration ?? 0.5f;

	private float mBX;

	private float mBY;

	private float mBW;

	private float mBH;

	private float[] mVertexBuffer = new float[64];

	/// <summary>Position in grid space: cell centres are at (i + 0.5, j + 0.5).</summary>
	public float FI;

	public float FJ;

	public State Mode = State.Idle;

	public float Timer;

	public bool FaceLeft;

	public readonly List<Point> Path = new List<Point>();

	public float WalkPhase;

	public float Hop;

	/// <summary>Screen rectangle of the last draw (dialog coordinates), for tapping.</summary>
	public TRect LastRect;

	/// <summary>Slots left empty in the dorm (e.g. the gatling pea's gun).</summary>
	private readonly List<Slot> mHidden = new List<Slot>();

	private DormActor(SeedType theSeed, string theName, Atlas theAtlas, SkeletonData theData, string[] theHideBones)
	{
		Seed = theSeed;
		Name = theName;
		mAtlas = theAtlas;
		mSkeleton = new Skeleton(theData);
		AnimationStateData stateData = new AnimationStateData(theData);
		stateData.DefaultMix = 0.2f;
		mAnim = new AnimationState(stateData);
		mHasSleep = theData.FindAnimation("sleep") != null;
		if (theHideBones != null && theHideBones.Length > 0)
		{
			foreach (Slot slot in mSkeleton.Slots)
			{
				for (Bone b = slot.Bone; b != null; b = b.Parent)
				{
					if (Array.IndexOf(theHideBones, b.Data.Name) >= 0)
					{
						mHidden.Add(slot);
						break;
					}
				}
			}
		}
		mWalk = theData.FindAnimation("walk");
		mWalkIsNative = mWalk != null;
		mWalk ??= LoadWalk(theName, theData);
		if (mWalk != null && !mWalkIsNative)
		{
			foreach (Timeline t in mWalk.Timelines)
			{
				if (t is IBoneTimeline bt && !mWalkBones.Contains(mSkeleton.Bones.Items[bt.BoneIndex]))
				{
					mWalkBones.Add(mSkeleton.Bones.Items[bt.BoneIndex]);
				}
			}
		}
		PlayIdle();
		mAnim.Apply(mSkeleton);
		HideSlots();
		mSkeleton.UpdateWorldTransform(Skeleton.Physics.Pose);
		MeasureBounds(theData);
		mRenderer = new SkeletonRenderer(GlobalStaticVars.g.GraphicsDevice);
		mRenderer.PremultipliedAlpha = true;
		Effect effect = GlobalStaticVars.gLawnApp.mContentManager.Load<Effect>("shaders\\LawnSpineEffect").Clone();
		effect.Parameters["World"].SetValue(Matrix.Identity);
		effect.Parameters["View"].SetValue(Matrix.CreateLookAt(new Vector3(0f, 0f, 1f), Vector3.Zero, Vector3.Up));
		effect.Parameters["ExtraAdditiveColor"]?.SetValue(Vector4.Zero);
		mRenderer.Effect = effect;
	}

	private void HideSlots()
	{
		foreach (Slot slot in mHidden)
		{
			slot.Attachment = null;
		}
	}

	public static DormActor TryCreate(SeedType theSeed, string theSkeleton, string[] theHideBones = null)
	{
		Atlas atlas = null;
		try
		{
			atlas = WardrobeOverlay.LoadAtlas("Content/spanim/" + theSkeleton + ".atlas", GlobalStaticVars.g.GraphicsDevice);
			SkeletonData data = WardrobeOverlay.ReadSkeletonBinary(new SkeletonBinary(atlas), "Content/spanim/" + theSkeleton + ".skel");
			return new DormActor(theSeed, theSkeleton, atlas, data, theHideBones);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm actor " + theSkeleton + ": " + ex.Message);
			atlas?.Dispose();
			return null;
		}
	}

	public bool CanSleep => mHasSleep;

	/// <summary>
	/// Walk keys for skeletons that have no "walk" of their own: Content/dorm/walk/&lt;skeleton&gt;.json
	/// {"duration":0.8,"tracks":[{"bone":"2","type":"rotate"|"translate","keys":[[t,v]|[t,x,y],...]}]}.
	/// Values are offsets added on top of idle (Spine additive track).
	/// </summary>
	private static Spine.Animation LoadWalk(string theName, SkeletonData theData)
	{
		try
		{
			string json;
			using (System.IO.StreamReader r = new System.IO.StreamReader(WardrobeOverlay.Open("Content/dorm/walk/" + theName + ".json")))
			{
				json = r.ReadToEnd();
			}
			using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(json);
			System.Text.Json.JsonElement root = doc.RootElement;
			float dur = root.GetProperty("duration").GetSingle();
			ExposedList<Timeline> timelines = new ExposedList<Timeline>();
			foreach (System.Text.Json.JsonElement tr in root.GetProperty("tracks").EnumerateArray())
			{
				BoneData bone = theData.FindBone(tr.GetProperty("bone").GetString());
				if (bone == null)
				{
					continue;
				}
				List<float[]> keys = new List<float[]>();
				foreach (System.Text.Json.JsonElement k in tr.GetProperty("keys").EnumerateArray())
				{
					keys.Add(k.EnumerateArray().Select((System.Text.Json.JsonElement x) => x.GetSingle()).ToArray());
				}
				string type = tr.GetProperty("type").GetString();
				if (type == "rotate")
				{
					RotateTimeline t = new RotateTimeline(keys.Count, 0, bone.Index);
					for (int i = 0; i < keys.Count; i++)
					{
						t.SetFrame(i, keys[i][0], keys[i][1]);
					}
					timelines.Add(t);
				}
				else if (type == "translate")
				{
					TranslateTimeline t = new TranslateTimeline(keys.Count, 0, bone.Index);
					for (int i = 0; i < keys.Count; i++)
					{
						t.SetFrame(i, keys[i][0], keys[i][1], keys[i][2]);
					}
					timelines.Add(t);
				}
			}
			return timelines.Count > 0 ? new Spine.Animation("walk", timelines, dur) : null;
		}
		catch (System.IO.FileNotFoundException)
		{
			return null;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm walk " + theName + ": " + ex.Message);
			return null;
		}
	}

	private bool mWalking;

	public void StartWalk()
	{
		if (mWalking)
		{
			return;
		}
		mWalking = true;
		PlayIdle();
		if (mWalk != null)
		{
			TrackEntry e = mAnim.SetAnimation(1, mWalk, loop: true);
			e.MixBlend = mWalkIsNative ? MixBlend.Replace : MixBlend.Add;
			e.MixDuration = 0.15f;
		}
	}

	public void StopWalk()
	{
		if (!mWalking)
		{
			return;
		}
		mWalking = false;
		if (mWalk != null)
		{
			mAnim.SetEmptyAnimation(1, 0.2f);
		}
	}

	private string mCurrent;

	private void Play(string theName)
	{
		if (mCurrent == theName)
		{
			return;
		}
		Spine.Animation a = mSkeleton.Data.FindAnimation(theName);
		if (a == null)
		{
			return;
		}
		mCurrent = theName;
		mAnim.SetAnimation(0, a, loop: true);
	}

	public void PlayIdle()
	{
		Spine.Animation a = mSkeleton.Data.FindAnimation("idle") ?? mSkeleton.Data.Animations.FirstOrDefault((Spine.Animation x) => x.Name.IndexOf("idle", StringComparison.OrdinalIgnoreCase) >= 0) ?? mSkeleton.Data.FindAnimation("garden") ?? mSkeleton.Data.Animations.FirstOrDefault();
		if (a != null)
		{
			Play(a.Name);
		}
	}

	public void PlaySleep()
	{
		Play("sleep");
	}

	/// <summary>Bounds over the idle loop, so every girl can be scaled to the same height.</summary>
	private void MeasureBounds(SkeletonData theData)
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
			mAnim.Update(0.2f);
			mAnim.Apply(mSkeleton);
			HideSlots();
			mSkeleton.UpdateWorldTransform(Skeleton.Physics.Pose);
		}
		if (maxX <= minX || maxY <= minY)
		{
			minX = -50f;
			minY = -100f;
			maxX = 50f;
			maxY = 0f;
		}
		mBX = minX;
		mBY = minY;
		mBW = maxX - minX;
		mBH = maxY - minY;
	}

	public void Advance(float theDt)
	{
		mAnim.Update(theDt);
		foreach (Bone b in mWalkBones)
		{
			b.SetToSetupPose();
		}
		mAnim.Apply(mSkeleton);
		HideSlots();
		mSkeleton.Update(theDt);
		mSkeleton.UpdateWorldTransform(Skeleton.Physics.Update);
	}

	/// <summary>Draws the girl standing with her feet at (theFootX, theFootY), theHeight tall (dialog coordinates).</summary>
	public void Draw(Graphics g, float theFootX, float theFootY, float theHeight)
	{
		Draw(g, theFootX, theFootY, theHeight, 0f, 1f, 1f);
	}

	/// <summary>theTilt in degrees (around the feet), theSX / theSY squash and stretch.</summary>
	public void Draw(Graphics g, float theFootX, float theFootY, float theHeight, float theTilt, float theSX, float theSY)
	{
		float k = theHeight / mBH;
		float w = mBW * k;
		LastRect = new TRect((int)(theFootX - w / 2f), (int)(theFootY - theHeight), (int)w, (int)theHeight);
		float flip = FaceLeft ? -1f : 1f;
		Graphics.GetTransform(out Matrix? transform);
		Matrix world = Matrix.CreateTranslation(-(mBX + mBW / 2f), -(mBY + mBH), 0f) * Matrix.CreateScale(k * flip * theSX, k * theSY, 1f) * Matrix.CreateRotationZ(MathHelper.ToRadians(theTilt)) * Matrix.CreateTranslation(theFootX + g.mTransX, theFootY + g.mTransY, 0f) * (transform ?? Matrix.Identity);
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
}
