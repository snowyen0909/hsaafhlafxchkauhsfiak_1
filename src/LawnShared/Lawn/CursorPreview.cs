using Lawn.Creative;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

public class CursorPreview : GameObject
{
	public int mGridX;

	public int mGridY;

	internal override void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.Sync(theContext);
		theContext.SyncInt(ref mGridX);
		theContext.SyncInt(ref mGridY);
	}

	public CursorPreview()
	{
		mX = 0;
		mY = 0;
		mWidth = 80;
		mHeight = 80;
		mGridX = 0;
		mGridY = 0;
		mVisible = false;
	}

	public void Update()
	{
		if (mApp.mGameScene == GameScenes.Playing && mApp.mWidgetManager.mMouseIn)
		{
			SeedType seedTypeInCursor = mBoard.GetSeedTypeInCursor();
			int mLastToolX = mBoard.mLastToolX;
			int mLastToolY = mBoard.mLastToolY;
			mGridX = mBoard.PlantingPixelToGridX((int)((float)mLastToolX * Constants.IS), (int)((float)mLastToolY * Constants.IS), seedTypeInCursor);
			mGridY = mBoard.PlantingPixelToGridY((int)((float)mLastToolX * Constants.IS), (int)((float)mLastToolY * Constants.IS), seedTypeInCursor);
			if (mGridX >= 0 && mGridX < Constants.GRIDSIZEX && mGridY >= 0 && mGridY <= Constants.MAX_GRIDSIZEY && (mBoard.IsPlantInCursor() || (mBoard.mCursorObject.mCursorType == CursorType.Wheeelbarrow && mApp.mZenGarden.GetPottedPlantInWheelbarrow() != null)) && mBoard.CanPlantAt(mGridX, mGridY, seedTypeInCursor) == PlantingReason.Ok)
			{
				mX = mBoard.GridToPixelX(mGridX, mGridY);
				mY = mBoard.GridToPixelY(mGridX, mGridY);
				mVisible = true;
				return;
			}
		}
		mVisible = false;
	}

	public override bool LoadFromFile(Buffer b)
	{
		return false;
	}

	public override bool SaveToFile(Buffer b)
	{
		return false;
	}

	public void Draw(Graphics g)
	{
		SeedType seedTypeInCursor = mBoard.GetSeedTypeInCursor();
		if (seedTypeInCursor == SeedType.None)
		{
			return;
		}
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new SexyColor(255, 255, 255, 132));
		if (Challenge.IsZombieSeedType(seedTypeInCursor))
		{
			ZombieType type = Challenge.IZombieSeedTypeToZombieType(seedTypeInCursor);
			bool mindControlled = (seedTypeInCursor & SeedType.MindControlledCardFlag) != 0;
			Vector2 placementPreviewOrigin = Zombie.GetPlacementPreviewOrigin(mBoard, type, mGridX, mGridY, mindControlled);
			TPoint zombieFrameOrigin = ReanimatorCache.GetZombieFrameOrigin(type);
			float mScaleX = g.mScaleX;
			if (Zombie.IsPlacementPreviewMirrored(type, mindControlled))
			{
				g.mScaleX = 0f - g.mScaleX;
			}
			float thePosX = (placementPreviewOrigin.X - (float)mX - (float)(zombieFrameOrigin.x - 20) * g.mScaleX) * Constants.S;
			float thePosY = (placementPreviewOrigin.Y - (float)mY - (float)(zombieFrameOrigin.y - 20) * g.mScaleY) * Constants.S;
			Plant.DrawSeedType(g, seedTypeInCursor, SeedType.None, DrawVariation.Normal, thePosX, thePosY);
			g.mScaleX = mScaleX;
			g.SetColorizeImages(colorizeImages: false);
			return;
		}
		CursorType mCursorType = mBoard.mCursorObject.mCursorType;
		PottedPlant pottedPlant = null;
		bool flag = true;
		switch (mCursorType)
		{
		case CursorType.PlantFromWheelBarrow:
		case CursorType.Wheeelbarrow:
			pottedPlant = mApp.mZenGarden.GetPottedPlantInWheelbarrow();
			break;
		case CursorType.PlantFromGlove:
			if (mBoard.mCursorObject.mGlovePlantID.mPottedPlantIndex != -1)
			{
				pottedPlant = mApp.GetPottedPlantByIndex(mBoard.mCursorObject.mGlovePlantID.mPottedPlantIndex);
				break;
			}
			pottedPlant = null;
			Plant.DrawSeedType(g, mBoard.mCursorObject.mType, mBoard.mCursorObject.mImitaterType, DrawVariation.Normal, 40f, 64f);
			flag = false;
			break;
		}
		if ((pottedPlant != null) & flag)
		{
			mApp.mZenGarden.DrawPottedPlant(g, 0f, 0f, pottedPlant, 1f, mBoard.mBackground != BackgroundType.MushroomGarden && mBoard.mBackground != BackgroundType.Zombiquarium);
		}
		else
		{
			float num = Plant.PlantDrawHeightOffset(mBoard, null, seedTypeInCursor, mGridX, mGridY);
			float num2 = 0f;
			Plant.DrawSeedType(g, mBoard.mCursorObject.mType, mBoard.mCursorObject.mImitaterType, DrawVariation.Normal, num2 + 40f, num + 64f);
		}
		if (mApp.IsColumnLevel() || mApp.mCreativeLevel?.GetComponent<CSColumnPlanting>() != null)
		{
			for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
			{
				if (i != mGridY && mBoard.CanPlantAt(mGridX, i, seedTypeInCursor) == PlantingReason.Ok)
				{
					Plant.DrawSeedType(g, mBoard.mCursorObject.mType, mBoard.mCursorObject.mImitaterType, DrawVariation.Normal, 40f, (float)(i - mGridY) * Constants.S * 85f + Plant.PlantDrawHeightOffset(mBoard, null, seedTypeInCursor, mGridX, i) + 64f);
				}
			}
		}
		g.SetColorizeImages(colorizeImages: false);
	}
}
