// 宿舍 (dorm) test: one plant girl walking around the room, drawn from the game's own Spine skeleton
// (worn wardrobe patches apply on iOS, because loading goes through DormPlatform → WardrobeOverlay).
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
		/// <summary>Mushrooms napping on the spot (their own "sleep" animation).</summary>
		Sleep,
		/// <summary>Picked up by the player's finger.</summary>
		Held,
		/// <summary>Lying on a futon.</summary>
		Bed,
		/// <summary>Sitting on a cushion, at the kotatsu or on the swing.</summary>
		Seat
	}

	/// <summary>The furniture she sits on (DormDialog.Item) and which seat of it; or where she is walking to sit.</summary>
	public object SeatItem;

	public int SeatIdx;

	public object PendingSeat;

	public int PendingIdx;

	public enum SitKind
	{
		None,
		/// <summary>Kneeling on the floor (cushion, kotatsu): thighs and shins drawn short, knees on the ground.</summary>
		Floor,
		/// <summary>On a bench (swing): thighs drawn short, legs hanging down; drawn from her hips.</summary>
		Chair
	}

	/// <summary>Sitting poses layered instead of the standing one (Content/dorm/sit/&lt;skeleton&gt;_floor|_chair.json).</summary>
	private readonly Spine.Animation mSitFloor;

	private readonly Spine.Animation mSitChair;

	public SitKind Sitting { get; private set; }

	public bool CanSit => mSitFloor != null && mSitChair != null && !Glides;

	/// <summary>Floor line of the kneeling pose and hip height of the bench pose (skeleton units, y up), from anchor.json.</summary>
	private float mFloorY = float.NaN;

	private float mSeatY = float.NaN;

	private bool mYDown = true;

	/// <summary>Swing angle (degrees) while held, and its speed.</summary>
	public float Swing;

	public float SwingVel;

	/// <summary>The futon she sleeps on (DormDialog.Item), or null.</summary>
	public object BedItem;

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

	/// <summary>Dorm-only pose offsets kept on all the time (Content/dorm/pose/&lt;skeleton&gt;.json, e.g. the gatling pea standing normally).</summary>
	private readonly Spine.Animation mPose;

	/// <summary>Idle time where her eyes are closed (blink frame), used to sleep with closed eyes.</summary>
	private static readonly Dictionary<string, float> ClosedEyes = new Dictionary<string, float>
	{
		["sunflower"] = 0.667f,
		["marigold"] = 1.0f,
		["peashooter"] = 0.867f
	};

	/// <summary>Bones hidden only while in bed (a hand gun would poke out of the quilt).</summary>
	private static readonly Dictionary<string, string[]> BedHide = new Dictionary<string, string[]>
	{
		["peashooter"] = new[] { "1_shouq1" }
	};

	private readonly List<Slot> mBedHidden = new List<Slot>();

	private bool mInBed;

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
		mWalk ??= LoadKeys("Content/dorm/walk/" + theName + ".json", "walk", theData);
		mPose = LoadKeys("Content/dorm/pose/" + theName + ".json", "dorm_pose", theData);
		mSitFloor = LoadKeys("Content/dorm/sit/" + theName + "_floor.json", "dorm_sit_floor", theData);
		mSitChair = LoadKeys("Content/dorm/sit/" + theName + "_chair.json", "dorm_sit_chair", theData);
		foreach (Spine.Animation anim in new[] { mWalkIsNative ? null : mWalk, mPose, mSitFloor, mSitChair })
		{
			if (anim == null)
			{
				continue;
			}
			foreach (Timeline t in anim.Timelines)
			{
				if (t is IBoneTimeline bt && !mWalkBones.Contains(mSkeleton.Bones.Items[bt.BoneIndex]))
				{
					mWalkBones.Add(mSkeleton.Bones.Items[bt.BoneIndex]);
				}
			}
		}
		if (BedHide.TryGetValue(theName, out string[] bedHide))
		{
			foreach (Slot slot in mSkeleton.Slots)
			{
				for (Bone b = slot.Bone; b != null; b = b.Parent)
				{
					if (Array.IndexOf(bedHide, b.Data.Name) >= 0)
					{
						mBedHidden.Add(slot);
						break;
					}
				}
			}
		}
		if (mPose != null)
		{
			TrackEntry pe = mAnim.SetAnimation(2, mPose, loop: true);
			pe.MixBlend = MixBlend.Add;
		}
#if !__IOS__
		Console.WriteLine($"DORMTEST actor {theName} walk={(mWalk != null ? mWalk.Timelines.Count : -1)} native={mWalkIsNative} pose={(mPose != null ? mPose.Timelines.Count : -1)} hidden={mHidden.Count}");
#endif
		PlayIdle();
		mAnim.Apply(mSkeleton);
		HideSlots();
		mSkeleton.UpdateWorldTransform(Skeleton.Physics.Pose);
		MeasureBounds(theData);
		ApplyAnchor(theName);
		mRenderer = new SkeletonRenderer(GlobalStaticVars.g.GraphicsDevice);
		mRenderer.PremultipliedAlpha = true;
		Effect effect = DormPlatform.SpineEffect();
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
		if (mInBed)
		{
			foreach (Slot slot in mBedHidden)
			{
				slot.Attachment = null;
			}
		}
	}

	public static DormActor TryCreate(SeedType theSeed, string theSkeleton, string[] theHideBones = null)
	{
		Atlas atlas = null;
		try
		{
			atlas = DormPlatform.LoadAtlas("Content/spanim/" + theSkeleton + ".atlas");
			SkeletonData data = DormPlatform.ReadSkeleton(atlas, "Content/spanim/" + theSkeleton + ".skel");
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

	/// <summary>Girls that hover (sea-shroom, cattails) or glide on their chair (endoflame): no footsteps, no dust.</summary>
	public bool Glides;

	/// <summary>Tapped: her zen-garden reaction (garden1 / garden2) once, then idle. Returns its length in seconds.</summary>
	public float PlayGarden(int theWhich)
	{
		Spine.Animation r = mSkeleton.Data.FindAnimation(theWhich == 1 ? "garden1" : "garden2") ?? mSkeleton.Data.Animations.FirstOrDefault((Spine.Animation x) => x.Name.StartsWith(theWhich == 1 ? "garden1" : "garden2", StringComparison.Ordinal));
		Spine.Animation idle = mSkeleton.Data.FindAnimation("idle");
		if (r == null || idle == null)
		{
			return 0f;
		}
		float len = Math.Min(r.Duration, theWhich == 1 ? 4.5f : 6f);
		mCurrent = "idle";
		TrackEntry e = mAnim.SetAnimation(0, r, loop: false);
		e.MixDuration = 0.2f;
		mAnim.AddAnimation(0, idle, loop: true, len).MixDuration = 0.3f;
		return len;
	}

	/// <summary>
	/// Walk keys for skeletons that have no "walk" of their own: Content/dorm/walk/&lt;skeleton&gt;.json
	/// {"duration":0.8,"tracks":[{"bone":"2","type":"rotate"|"translate","keys":[[t,v]|[t,x,y],...]}]}.
	/// Values are offsets added on top of idle (Spine additive track).
	/// </summary>
	private static Spine.Animation LoadKeys(string thePath, string theAnimName, SkeletonData theData)
	{
		try
		{
			string json;
			using (System.IO.StreamReader r = new System.IO.StreamReader(DormPlatform.Open(thePath)))
			{
				json = r.ReadToEnd();
			}
			using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(json);
			System.Text.Json.JsonElement root = doc.RootElement;
			float dur = root.GetProperty("duration").GetSingle();
			// setup pose, to turn "space":"world" offsets (x right, y up) into each bone's parent space
			Skeleton setup = new Skeleton(theData);
			setup.UpdateWorldTransform(Skeleton.Physics.Pose);
			// the MonoGame runtime draws y-down (the root is mirrored): files say "up" as +y, so flip it here
			Bone sr = setup.RootBone;
			float upSign = sr.A * sr.D - sr.B * sr.C < 0f ? -1f : 1f;
			ExposedList<Timeline> timelines = new ExposedList<Timeline>();
			foreach (System.Text.Json.JsonElement tr in root.GetProperty("tracks").EnumerateArray())
			{
				string type = tr.GetProperty("type").GetString();
				List<float[]> keys = new List<float[]>();
				foreach (System.Text.Json.JsonElement k in tr.GetProperty("keys").EnumerateArray())
				{
					keys.Add(k.EnumerateArray().Select((System.Text.Json.JsonElement x) => x.GetSingle()).ToArray());
				}
				if (type == "ik" || type == "tc")
				{
					// constraint keys: ik [t, mix, bendDirection], tc [t, mix] (e.g. let a hand go of the hidden gun)
					string cname = tr.GetProperty("constraint").GetString();
					if (type == "ik")
					{
						IkConstraintData ikd = theData.IkConstraints.Find((IkConstraintData c) => c.Name == cname);
						if (ikd == null)
						{
							continue;
						}
						IkConstraintTimeline it = new IkConstraintTimeline(keys.Count, 0, theData.IkConstraints.IndexOf(ikd));
						for (int i = 0; i < keys.Count; i++)
						{
							it.SetFrame(i, keys[i][0], keys[i][1], 0f, (int)keys[i][2], compress: false, stretch: false);
						}
						timelines.Add(it);
					}
					else
					{
						TransformConstraintData tcd = theData.TransformConstraints.Find((TransformConstraintData c) => c.Name == cname);
						if (tcd == null)
						{
							continue;
						}
						TransformConstraintTimeline tt = new TransformConstraintTimeline(keys.Count, 0, theData.TransformConstraints.IndexOf(tcd));
						for (int i = 0; i < keys.Count; i++)
						{
							float m = keys[i][1];
							tt.SetFrame(i, keys[i][0], m, m, m, m, m, m);
						}
						timelines.Add(tt);
					}
					continue;
				}
				BoneData bone = theData.FindBone(tr.GetProperty("bone").GetString());
				if (bone == null)
				{
					continue;
				}
				if (type == "scale")
				{
					// stretch along the bone: a thigh pointing at the viewer is drawn short (sitting poses)
					ScaleXTimeline t = new ScaleXTimeline(keys.Count, 0, bone.Index);
					for (int i = 0; i < keys.Count; i++)
					{
						t.SetFrame(i, keys[i][0], keys[i][1]);
					}
					timelines.Add(t);
					continue;
				}
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
					bool world = tr.TryGetProperty("space", out System.Text.Json.JsonElement sp) && sp.GetString() == "world";
					Bone parent = setup.Bones.Items[bone.Index].Parent;
					for (int i = 0; i < keys.Count; i++)
					{
						float x = keys[i][1], y = keys[i][2];
						if (world && parent != null)
						{
							y *= upSign;
							float det = parent.A * parent.D - parent.B * parent.C;
							if (Math.Abs(det) > 1e-6f)
							{
								float lx = (parent.D * x - parent.B * y) / det;
								float ly = (-parent.C * x + parent.A * y) / det;
								x = lx;
								y = ly;
							}
						}
						t.SetFrame(i, keys[i][0], x, y);
					}
					timelines.Add(t);
				}
			}
			return timelines.Count > 0 ? new Spine.Animation(theAnimName, timelines, dur) : null;
		}
		catch (System.IO.FileNotFoundException)
		{
			return null;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm keys " + thePath + ": " + ex.Message);
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

	/// <summary>Picked up: play her garden reaction once, then back to idle.</summary>
	public void React()
	{
		Spine.Animation r = mSkeleton.Data.FindAnimation("garden1") ?? mSkeleton.Data.FindAnimation("garden2");
		Spine.Animation idle = mSkeleton.Data.FindAnimation("idle");
		if (r == null || idle == null)
		{
			return;
		}
		mCurrent = "idle";
		mAnim.SetAnimation(0, r, loop: false);
		mAnim.AddAnimation(0, idle, loop: true, Math.Min(r.Duration, 1.2f));
	}

	/// <summary>Lying in bed: own sleep animation, or idle frozen on the blink frame (eyes closed).</summary>
	public void EnterBed()
	{
		mInBed = true;
		if (mHasSleep)
		{
			PlaySleep();
			return;
		}
		Spine.Animation idle = mSkeleton.Data.FindAnimation("idle");
		if (idle == null)
		{
			return;
		}
		mCurrent = "bed";
		TrackEntry e = mAnim.SetAnimation(0, idle, loop: true);
		e.MixDuration = 0f;
		if (ClosedEyes.TryGetValue(Name, out float t))
		{
			e.TrackTime = t;
			e.TimeScale = 0f;
		}
	}

	public void LeaveBed()
	{
		mInBed = false;
		foreach (Slot slot in mBedHidden)
		{
			slot.SetToSetupPose();
		}
		mCurrent = null;
		PlayIdle();
	}

	private static Dictionary<string, float[]> sAnchors;

	private static readonly Dictionary<string, float[]> sSit = new Dictionary<string, float[]>();

	/// <summary>Sit down (kneeling or on a bench) or stand up again.</summary>
	public void SetSitting(SitKind theKind)
	{
		Spine.Animation sit = theKind == SitKind.Floor ? mSitFloor : theKind == SitKind.Chair ? mSitChair : null;
		if (theKind == Sitting || (theKind != SitKind.None && sit == null))
		{
			return;
		}
		Sitting = theKind;
		Spine.Animation pose = sit ?? mPose;
		if (pose != null)
		{
			TrackEntry e = mAnim.SetAnimation(2, pose, loop: true);
			e.MixBlend = MixBlend.Add;
			e.MixDuration = 0.25f;
		}
		else
		{
			mAnim.SetEmptyAnimation(2, 0.25f);
		}
	}

	/// <summary>
	/// Visible box measured offline from her standing dorm pose (Content/dorm/anchor.json: foot x, bottom, top, left,
	/// right in skeleton units, y up). Spine's own bounds also count invisible effect attachments, which pushed some
	/// girls off the middle of their cell and made them smaller.
	/// </summary>
	private void ApplyAnchor(string theName)
	{
		if (sAnchors == null)
		{
			sAnchors = new Dictionary<string, float[]>();
			try
			{
				using System.IO.StreamReader r = new System.IO.StreamReader(DormPlatform.Open("Content/dorm/anchor.json"));
				using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(r.ReadToEnd());
				foreach (System.Text.Json.JsonProperty p in doc.RootElement.EnumerateObject())
				{
					System.Text.Json.JsonElement e = p.Value;
					sAnchors[p.Name] = new[] { e.GetProperty("foot").GetSingle(), e.GetProperty("bottom").GetSingle(), e.GetProperty("top").GetSingle(), e.GetProperty("left").GetSingle(), e.GetProperty("right").GetSingle() };
					if (e.TryGetProperty("floor", out System.Text.Json.JsonElement fl) && e.TryGetProperty("chair", out System.Text.Json.JsonElement ch))
					{
						sSit[p.Name] = new[] { fl.GetSingle(), ch.GetSingle() };
					}
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Dorm anchors: " + ex.Message);
			}
		}
		if (!sAnchors.TryGetValue(theName, out float[] a) || a[2] <= a[1])
		{
			return;
		}
		Bone root = mSkeleton.RootBone;
		bool yDown = root.A * root.D - root.B * root.C < 0f;
		mYDown = yDown;
		if (sSit.TryGetValue(theName, out float[] sv))
		{
			mFloorY = sv[0];
			mSeatY = sv[1];
		}
		mBW = a[4] - a[3];
		mBH = a[2] - a[1];
		mBX = a[0] - mBW / 2f;
		mBY = yDown ? -a[2] : a[1];
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
			foreach (Bone b in mWalkBones)
			{
				b.SetToSetupPose();
			}
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
		// first frame: start the hair / skirt physics from the dorm pose, not from the setup pose
		mSkeleton.UpdateWorldTransform(mPhysicsReset ? Skeleton.Physics.Reset : Skeleton.Physics.Update);
		mPhysicsReset = false;
	}


	private bool mPhysicsReset = true;

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
		// standing: feet on the point; sitting: the lowest point of the sitting pose on the point (same scale)
		// standing / kneeling: the floor line on the point; on a bench: her hips on the point (legs hang below it)
		float sy = Sitting == SitKind.Chair ? mSeatY : Sitting == SitKind.Floor ? mFloorY : float.NaN;
		float ay = !float.IsNaN(sy) ? (mYDown ? -sy : sy) : mBY + mBH;
		Matrix world = Matrix.CreateTranslation(-(mBX + mBW / 2f), -ay, 0f) * Matrix.CreateScale(k * flip * theSX, k * theSY, 1f) * Matrix.CreateRotationZ(MathHelper.ToRadians(theTilt)) * Matrix.CreateTranslation(theFootX + g.mTransX, theFootY + g.mTransY, 0f) * (transform ?? Matrix.Identity);
		g.EndFrame();
		mRenderer.Effect.Parameters["Projection"].SetValue(DormPlatform.SpineProjection(g));
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
