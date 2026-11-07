using System;
using System.Collections.Generic;
using FontStashSharp.Interfaces;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public sealed class LevelInfoRichText : ScrollWidget
{
	private sealed class RichTextDocument : Widget
	{
		private readonly RichTextRenderer mRenderer = new RichTextRenderer();

		public RichTextLayout TextLayout { get; }

		public RichTextDocument(RichTextLayout layout)
		{
			TextLayout = layout;
			mMouseVisible = false;
		}

		public override void Draw(Graphics g)
		{
			if (mWidth <= 0 || mHeight <= 0 || g.mClipRect.mWidth <= 0 || g.mClipRect.mHeight <= 0)
			{
				return;
			}
			g.HardwareClip();
			try
			{
				TextLayout.Draw(mRenderer, new Vector2(g.mTransX, g.mTransY), Color.White);
			}
			finally
			{
				g.EndHardwareClip();
			}
		}
	}

	private sealed class RichTextRenderer : IFontStashRenderer2
	{
		public GraphicsDevice GraphicsDevice => Graphics.spriteBatch.GraphicsDevice;

		public void DrawQuad(Texture2D texture, ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight, ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight)
		{
			int num = (int)Math.Round(topLeft.TextureCoordinate.X * (float)texture.Width);
			int num2 = (int)Math.Round(topLeft.TextureCoordinate.Y * (float)texture.Height);
			int num3 = (int)Math.Round(bottomRight.TextureCoordinate.X * (float)texture.Width);
			int num4 = (int)Math.Round(bottomRight.TextureCoordinate.Y * (float)texture.Height);
			Rectangle value = new Rectangle(num, num2, num3 - num, num4 - num2);
			if (value.Width > 0 && value.Height > 0)
			{
				Vector2 position = new Vector2(topLeft.Position.X, topLeft.Position.Y);
				Vector2 scale = new Vector2((topRight.Position.X - topLeft.Position.X) / (float)value.Width, (bottomLeft.Position.Y - topLeft.Position.Y) / (float)value.Height);
				Graphics.spriteBatch.Draw(texture, position, value, topLeft.Color, 0f, Vector2.Zero, scale, SpriteEffects.None, topLeft.Position.Z);
			}
		}
	}

	private sealed class PreviewImages : IDisposable
	{
		private sealed class TintedPreview : IRenderable
		{
			private readonly TextureFragment mImage;

			private readonly Color mTint;

			public Point Size => mImage.Size;

			public TintedPreview(TextureFragment image, Color tint)
			{
				mImage = image;
				mTint = tint;
			}

			public void Draw(FSRenderContext context, Vector2 position, Color color)
			{
				context.DrawImage(mImage.Texture, mImage.Region, position, mImage.Scale, new Color(color.ToVector4() * mTint.ToVector4()));
			}
		}

		private sealed class EmptyPreview : IRenderable
		{
			public static readonly EmptyPreview Instance = new EmptyPreview();

			public Point Size => Point.Zero;

			public void Draw(FSRenderContext context, Vector2 position, Color color)
			{
			}
		}

		private readonly ReanimatorCache mCache;

		private readonly Dictionary<string, IRenderable> mImages = new Dictionary<string, IRenderable>();

		private readonly List<MemoryImage> mAnimationFrames = new List<MemoryImage>();

		public PreviewImages(ReanimatorCache cache)
		{
			mCache = cache;
		}

		public IRenderable Resolve(string key)
		{
			if (mImages.TryGetValue(key, out var value))
			{
				return value;
			}
			Image image = null;
			Rectangle region = Rectangle.Empty;
			Color color = Color.White;
			float val = float.PositiveInfinity;
			ZombieType result2;
			ProjectileType result3;
			if (key.StartsWith("plant:", StringComparison.Ordinal) && Enum.TryParse<SeedType>(key.Substring(6), out var result) && result >= SeedType.Peashooter && (int)result < mCache.mPlantImages.Count)
			{
				image = mCache.mPlantImages[(int)result];
				if (image != null)
				{
					region = FindVisibleRegion(image.Texture);
				}
			}
			else if (key.StartsWith("zombie:", StringComparison.Ordinal) && Enum.TryParse<ZombieType>(key.Substring(7), out result2) && result2 >= ZombieType.Normal && (int)result2 < mCache.mZombieImages.Count)
			{
				image = mCache.mZombieImages[(int)result2];
				if (image != null && mCache.mZombieImageBounds.TryGetValue(result2, out var value2))
				{
					region = new Rectangle((int)((float)value2.mX * image.mInvScale), (int)((float)value2.mY * image.mInvScale), (int)((float)value2.mWidth * image.mInvScale), (int)((float)value2.mHeight * image.mInvScale));
				}
			}
			else if (key.StartsWith("projectile:", StringComparison.Ordinal) && Enum.TryParse<ProjectileType>(key.Substring(11), out result3))
			{
				image = GetProjectileImage(result3);
				if (image != null)
				{
					val = ((image is MemoryImage) ? 0.5f : (1f / image.mInvScale));
					int mImageRow = GameConstants.gProjectileDefinition[(int)result3].mImageRow;
					int width = (int)((float)image.GetCelWidth() * image.mInvScale);
					int num = (int)((float)image.GetCelHeight() * image.mInvScale);
					region = FindVisibleRegion(image.Texture, new Rectangle(image.mS, image.mT + mImageRow * num, width, num));
					if (result3 == ProjectileType.ZombiePeaMindControl)
					{
						color = GameConstants.ZOMBIE_MINDCONTROLLED_COLOR;
					}
				}
			}
			if (image == null || region.Width <= 0 || region.Height <= 0)
			{
				return EmptyPreview.Instance;
			}
			TextureFragment textureFragment = new TextureFragment(image.Texture, region)
			{
				Scale = new Vector2(Math.Min(val, 96f / (float)Math.Max(region.Width, region.Height)))
			};
			IRenderable renderable2;
			if (!(color == Color.White))
			{
				IRenderable renderable = new TintedPreview(textureFragment, color);
				renderable2 = renderable;
			}
			else
			{
				IRenderable renderable = textureFragment;
				renderable2 = renderable;
			}
			IRenderable renderable3 = renderable2;
			mImages.Add(key, renderable3);
			return renderable3;
		}

		private static Rectangle FindVisibleRegion(Texture2D texture)
		{
			return FindVisibleRegion(texture, new Rectangle(0, 0, texture.Width, texture.Height));
		}

		private static Rectangle FindVisibleRegion(Texture2D texture, Rectangle region)
		{
			int width = region.Width;
			int height = region.Height;
			Color[] array = TexturePixelReader.ReadRegion(texture, region);
			int num = width;
			int num2 = height;
			int num3 = -1;
			int num4 = -1;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (array[i * width + j].A != 0)
					{
						num = Math.Min(num, j);
						num2 = Math.Min(num2, i);
						num3 = Math.Max(num3, j);
						num4 = Math.Max(num4, i);
					}
				}
			}
			if (num3 >= num)
			{
				return new Rectangle(region.X + num, region.Y + num2, num3 - num + 1, num4 - num2 + 1);
			}
			return Rectangle.Empty;
		}

		private Image GetProjectileImage(ProjectileType type)
		{
			switch (type)
			{
			case ProjectileType.Pea:
			case ProjectileType.ZombiePea:
			case ProjectileType.ZombiePeaMindControl:
				return AtlasResources.IMAGE_PROJECTILEPEA;
			case ProjectileType.Snowpea:
				return AtlasResources.IMAGE_PROJECTILESNOWPEA;
			case ProjectileType.Cabbage:
				return AtlasResources.IMAGE_REANIM_CABBAGEPULT_CABBAGE;
			case ProjectileType.Melon:
				return AtlasResources.IMAGE_REANIM_MELONPULT_MELON;
			case ProjectileType.Wintermelon:
				return AtlasResources.IMAGE_REANIM_WINTERMELON_PROJECTILE;
			case ProjectileType.Puff:
				return AtlasResources.IMAGE_PUFFSHROOM_PUFF1;
			case ProjectileType.PuffGreen:
				return AtlasResources.IMAGE_SEASHROOM_PUFF_GREEN1;
			case ProjectileType.Star:
				return AtlasResources.IMAGE_PROJECTILE_STAR;
			case ProjectileType.Spike:
				return AtlasResources.IMAGE_PROJECTILECACTUS;
			case ProjectileType.Basketball:
				return AtlasResources.IMAGE_REANIM_ZOMBIE_CATAPULT_BASKETBALL;
			case ProjectileType.Kernel:
				return AtlasResources.IMAGE_REANIM_CORNPULT_KERNAL;
			case ProjectileType.Butter:
				return AtlasResources.IMAGE_REANIM_CORNPULT_BUTTER;
			case ProjectileType.CattailSpike:
				return AtlasResources.IMAGE_REANIM_PROJECTILECATTAIL;
			case ProjectileType.HypnoCattailSpike:
				return AtlasResources.IMAGE_REANIM_PROJECTILECATTAIL_HYPNO;
			case ProjectileType.Fireball:
				return MakeAnimationPreview(ReanimationType.FirePea, null);
			case ProjectileType.EndoflameSpike:
				return MakeAnimationPreview(ReanimationType.EndoflameSpike, "anim_normal");
			case ProjectileType.EndoflameFireball:
				return MakeAnimationPreview(ReanimationType.EndoflameSpike, "anim_burning");
			default:
				return null;
			}
		}

		private MemoryImage MakeAnimationPreview(ReanimationType type, string track)
		{
			MemoryImage memoryImage = new MemoryImage();
			memoryImage.Create(512, 512);
			lock (ResourceManager.DrawLocker)
			{
				Graphics graphics = Graphics.GetNew(memoryImage);
				try
				{
					graphics.BeginFrame();
					memoryImage.Clear();
					graphics.mScaleX = (graphics.mScaleY = 2f);
					mCache.DrawReanimatorFrame(graphics, 128f, 128f, type, track, DrawVariation.Normal);
					graphics.EndFrame();
				}
				catch
				{
					memoryImage.Dispose();
					throw;
				}
				finally
				{
					graphics.SetRenderTarget(null);
					graphics.PrepareForReuse();
				}
			}
			mAnimationFrames.Add(memoryImage);
			return memoryImage;
		}

		public void Dispose()
		{
			mImages.Clear();
			foreach (MemoryImage mAnimationFrame in mAnimationFrames)
			{
				mAnimationFrame.Dispose();
			}
			mAnimationFrames.Clear();
		}
	}

	private readonly RichTextDocument mDocument;

	private readonly PreviewImages mPreviews;

	private readonly int mColumns;

	private LawnApp mLevelApp;

	private bool mRawValues;

	public string Text
	{
		get
		{
			return mDocument.TextLayout.Text;
		}
		set
		{
			mLevelApp = null;
			SetDocumentText(value);
		}
	}

	private int TextWidth => Math.Max(1, mWidth - AtlasResources.IMAGE_SCROLL_INDICATOR.mWidth - 4);

	public LevelInfoRichText(ReanimatorCache cache, Font font, int columns = 1)
	{
		if (columns < 1)
		{
			throw new ArgumentOutOfRangeException("columns");
		}
		mColumns = columns;
		mPreviews = new PreviewImages(cache);
		mDocument = new RichTextDocument(new RichTextLayout(new RichTextSettings
		{
			ImageResolver = mPreviews.Resolve
		})
		{
			Font = ((StashFont)font).mFonts[0],
			VerticalSpacing = 16
		});
		ClipInput = true;
		AddWidget(mDocument);
		EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
	}

	public override void Dispose()
	{
		base.Dispose();
		mPreviews.Dispose();
	}

	public void SetLevel(LawnApp app, bool rawValues)
	{
		mLevelApp = app;
		mRawValues = rawValues;
		SetDocumentText(LevelOverrideText.Build(app, mDocument.TextLayout.Font, mPreviews.Resolve, rawValues, (mWidth > 0) ? TextWidth : 800, mColumns));
	}

	private void SetDocumentText(string text)
	{
		mDocument.TextLayout.Text = text ?? string.Empty;
		LayoutDocument();
		MarkDirty();
	}

	public override void Resize(int x, int y, int width, int height)
	{
		bool flag = width != mWidth || height != mHeight;
		base.Resize(x, y, width, height);
		if (flag)
		{
			if (mLevelApp != null)
			{
				SetLevel(mLevelApp, mRawValues);
			}
			else
			{
				LayoutDocument();
			}
		}
	}

	private void LayoutDocument()
	{
		if (mWidth > 0)
		{
			int textWidth = TextWidth;
			mDocument.TextLayout.Width = textWidth;
			mDocument.Resize(mDocument.mX, mDocument.mY, textWidth, mDocument.TextLayout.Size.Y);
			ClientSizeChanged();
			SetScrollOffset(new CGPoint(0f, Math.Clamp(GetScrollOffset().y, Math.Min(0, mHeight - mDocument.mHeight), 0f)), animated: false);
		}
	}
}
