using System.Collections.Generic;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class ComicSelector : LawnDialog
{
	public record ComicImportDataEntry(Comics mComicId, string mName, Image mCoverImage, int mPageCount, string mLoadGroupPrefix, string mImagePrefix, SeedType mSeedType);

	public static class ComicImportData
	{
		public static Dictionary<Comics, ComicImportDataEntry> mComics;

		private static Image GetSeedPacketImage(SeedType seedType)
		{
			Image iMAGE_SEEDPACKETS = Resources.IMAGE_SEEDPACKETS;
			TRect celRect = iMAGE_SEEDPACKETS.GetCelRect((int)seedType);
			return new Image(iMAGE_SEEDPACKETS, celRect.mX, celRect.mY + celRect.mHeight / 2, celRect.mWidth, celRect.mHeight / 2);
		}

		static ComicImportData()
		{
			mComics = new Dictionary<Comics, ComicImportDataEntry>
			{
				[Comics.CartoonFumeshroom] = new ComicImportDataEntry(Comics.CartoonFumeshroom, "特别章", GetSeedPacketImage(SeedType.Fumeshroom), 37, "DelayLoad_Cartoon_Fumeshroom", "IMAGE_CARTOON_FUMESHROOM", SeedType.Fumeshroom),
				[Comics.CartoonJalapeno] = new ComicImportDataEntry(Comics.CartoonJalapeno, "第一章", GetSeedPacketImage(SeedType.Jalapeno), 19, "DelayLoad_Cartoon_Jalapeno", "IMAGE_CARTOON_JALAPENO", SeedType.Jalapeno),
				[Comics.CartoonScaredyshroom] = new ComicImportDataEntry(Comics.CartoonScaredyshroom, "第二章", GetSeedPacketImage(SeedType.Scaredyshroom), 15, "DelayLoad_Cartoon_Scaredyshroom", "IMAGE_CARTOON_SCAREDYSHROOM", SeedType.Scaredyshroom),
				[Comics.CartoonChomper] = new ComicImportDataEntry(Comics.CartoonChomper, "第三章", GetSeedPacketImage(SeedType.Chomper), 16, "DelayLoad_Cartoon_Chomper", "IMAGE_CARTOON_CHOMPER", SeedType.Chomper),
				[Comics.CartoonRepeater] = new ComicImportDataEntry(Comics.CartoonRepeater, "第四章", GetSeedPacketImage(SeedType.Repeater), 17, "DelayLoad_Cartoon_Repeater", "IMAGE_CARTOON_REPEATER", SeedType.Repeater),
				[Comics.CartoonGarlic] = new ComicImportDataEntry(Comics.CartoonGarlic, "第五章", GetSeedPacketImage(SeedType.Garlic), 15, "DelayLoad_Cartoon_Garlic", "IMAGE_CARTOON_GARLIC", SeedType.Garlic),
				[Comics.CartoonKernelpult] = new ComicImportDataEntry(Comics.CartoonKernelpult, "第六章", GetSeedPacketImage(SeedType.Kernelpult), 23, "DelayLoad_Cartoon_Kernelpult", "IMAGE_CARTOON_KERNELPULT", SeedType.Kernelpult),
				[Comics.CartoonLilypad] = new ComicImportDataEntry(Comics.CartoonLilypad, "第七章", GetSeedPacketImage(SeedType.Lilypad), 16, "DelayLoad_Cartoon_Lilypad", "IMAGE_CARTOON_LILYPAD", SeedType.Lilypad),
				[Comics.CartoonCattail] = new ComicImportDataEntry(Comics.CartoonCattail, "第八章", GetSeedPacketImage(SeedType.Cattail), 17, "DelayLoad_Cartoon_Cattail", "IMAGE_CARTOON_CATTAIL", SeedType.Cattail),
				[Comics.CartoonSunflower] = new ComicImportDataEntry(Comics.CartoonSunflower, "第九章", GetSeedPacketImage(SeedType.Sunflower), 19, "DelayLoad_Cartoon_Sunflower", "IMAGE_CARTOON_SUNFLOWER", SeedType.Sunflower),
				[Comics.CartoonPlantern] = new ComicImportDataEntry(Comics.CartoonPlantern, "第十章", GetSeedPacketImage(SeedType.Plantern), 21, "DelayLoad_Cartoon_Plantern", "IMAGE_CARTOON_PLANTERN", SeedType.Plantern),
				[Comics.CartoonGravebuster] = new ComicImportDataEntry(Comics.CartoonGravebuster, "第十一章", GetSeedPacketImage(SeedType.Gravebuster), 23, "DelayLoad_Cartoon_Gravebuster", "IMAGE_CARTOON_GRAVEBUSTER", SeedType.Gravebuster),
				[Comics.CartoonBejeweled] = new ComicImportDataEntry(Comics.CartoonBejeweled, "间章", GetSeedPacketImage(SeedType.SlotMachineDiamond), 21, "DelayLoad_Cartoon_Bejeweled", "IMAGE_CARTOON_BEJEWELED", SeedType.SlotMachineDiamond),
				[Comics.CartoonSpikeweed] = new ComicImportDataEntry(Comics.CartoonSpikeweed, "第十二章", GetSeedPacketImage(SeedType.Spikeweed), 18, "DelayLoad_Cartoon_Spikeweed", "IMAGE_CARTOON_SPIKEWEED", SeedType.Spikeweed),
				[Comics.CartoonAgave] = new ComicImportDataEntry(Comics.CartoonAgave, "第十三章", GetSeedPacketImage(SeedType.Agave), 18, "DelayLoad_Cartoon_Agave", "IMAGE_CARTOON_AGAVE", SeedType.Agave),
				[Comics.CartoonRose] = new ComicImportDataEntry(Comics.CartoonRose, "间章二", GetSeedPacketImage(SeedType.ZombiquariumTrophy), 21, "DelayLoad_Cartoon_Rose", "IMAGE_CARTOON_ROSE", SeedType.ZombiquariumTrophy),
				[Comics.CartoonPumpkin] = new ComicImportDataEntry(Comics.CartoonPumpkin, "第十四章", GetSeedPacketImage(SeedType.Pumpkinshell), 39, "DelayLoad_Cartoon_Pumpkin", "IMAGE_CARTOON_PUMPKIN", SeedType.Pumpkinshell),
				[Comics.CartoonSpikerock] = new ComicImportDataEntry(Comics.CartoonSpikerock, "第十五章", GetSeedPacketImage(SeedType.Spikerock), 28, "DelayLoad_Cartoon_Spikerock", "IMAGE_CARTOON_SPIKEROCK", SeedType.Spikerock),
				[Comics.CartoonImitater] = new ComicImportDataEntry(Comics.CartoonImitater, "第十六章", GetSeedPacketImage(SeedType.Imitater), 21, "DelayLoad_Cartoon_Imitater", "IMAGE_CARTOON_IMITATER", SeedType.Imitater),
				[Comics.CartoonSun] = new ComicImportDataEntry(Comics.CartoonSun, "间章三", GetSeedPacketImage(SeedType.SlotMachineSun), 12, "DelayLoad_Cartoon_Sun", "IMAGE_CARTOON_SUN", SeedType.SlotMachineSun),
				[Comics.CartoonHypnoshroom] = new ComicImportDataEntry(Comics.CartoonHypnoshroom, "第十七章", GetSeedPacketImage(SeedType.Hypnoshroom), 16, "DelayLoad_Cartoon_Hypnoshroom", "IMAGE_CARTOON_HYPNOSHROOM", SeedType.Hypnoshroom),
				[Comics.CartoonPuffshroom] = new ComicImportDataEntry(Comics.CartoonPuffshroom, "第十八章", GetSeedPacketImage(SeedType.Puffshroom), 23, "DelayLoad_Cartoon_Puffshroom", "IMAGE_CARTOON_PUFFSHROOM", SeedType.Puffshroom),
				[Comics.CartoonIceshroom] = new ComicImportDataEntry(Comics.CartoonIceshroom, "第十九章", GetSeedPacketImage(SeedType.Iceshroom), 22, "DelayLoad_Cartoon_Iceshroom", "IMAGE_CARTOON_ICESHROOM", SeedType.Iceshroom)
			};
		}
	}

	public const int COMIC_SELECTOR_ID = 3;

	public int mHalfDeltaWidth;

	public int mHalfDeltaHeight;

	public GameButton mCloseButton;

	public GameButton[] mComicButtons;

	public Widget mComicWidget;

	public AlmanacListener mListener;

	public ComicSelector(LawnApp theApp, AlmanacListener theListener)
		: base(theApp, null, 3, isModal: true, "[ALMANAC_HEADER]", "", "", 0)
	{
		mApp = theApp;
		mListener = theListener;
		mCloseButton = new GameButton(1001, this);
		mCloseButton.mButtonImage = AtlasResources.IMAGE_ALMANAC_CLOSEBUTTON;
		mCloseButton.mOverImage = null;
		mCloseButton.mDownImage = null;
		mCloseButton.SetFont(Resources.FONT_BRIANNETOD12);
		mCloseButton.mColors[0] = new SexyColor(42, 42, 90);
		mCloseButton.mColors[1] = new SexyColor(42, 42, 90);
		mCloseButton.Resize(Constants.Almanac_CloseButtonRect.mX + mHalfDeltaWidth, Constants.Almanac_CloseButtonRect.mY, Constants.Almanac_CloseButtonRect.mWidth, Constants.Almanac_CloseButtonRect.mHeight);
		mCloseButton.mParentWidget = this;
		mCloseButton.mTextOffsetX = -8;
		mCloseButton.mTextOffsetY = 1;
		mComicButtons = new GameButton[23];
		for (Comics comics = Comics.CartoonJalapeno; comics < Comics.ComicsCount; comics++)
		{
			ref GameButton reference = ref mComicButtons[(int)comics];
			reference = new GameButton((int)(2000 + comics), this);
			Image mCoverImage = ComicImportData.mComics[comics].mCoverImage;
			reference.mButtonImage = mCoverImage;
			reference.Resize(mHalfDeltaWidth + 100 + 100 - mCoverImage.mWidth / 2 + (int)comics % 7 * 100, Constants.Almanac_PlantScrollRect.mY + (int)comics / 7 * 100, mCoverImage.mWidth, mCoverImage.mHeight);
			reference.mParentWidget = this;
		}
	}

	public override void Draw(Graphics g)
	{
		LawnCommon.DrawImageBox(g, new TRect(0, 0, mWidth, mHeight), AtlasResources.IMAGE_ALMANAC_ROUNDED_OUTLINE);
		string theText = "植物娘大战僵尸 系列漫画";
		TodCommon.TodDrawString(g, theText, mWidth / 2, Constants.AlmanacHeaderY + 8 + 10, Resources.FONT_BRIANNETOD16, new SexyColor(40, 50, 90), DrawStringJustification.Center);
		mCloseButton.Draw(g);
		GameButton[] array = mComicButtons;
		foreach (GameButton gameButton in array)
		{
			ComicImportDataEntry comicImportDataEntry = ComicImportData.mComics[(Comics)(gameButton.mId - 2000)];
			SeedPacket.DrawSmallSeedPacket(g, gameButton.mX, gameButton.mY, comicImportDataEntry.mSeedType, SeedType.None, 0f, 255, theDrawCost: false, theUseCurrentCost: false, theDrawBackground: true, theDrawCostBackground: false, canShowName: false);
			TodCommon.TodDrawString(g, comicImportDataEntry.mName, gameButton.mX + gameButton.mWidth / 2, gameButton.mY + gameButton.mHeight, Resources.FONT_BRIANNETOD12, new SexyColor(40, 50, 90), DrawStringJustification.Center);
		}
	}

	public override void Update()
	{
		UpdateScreen();
		base.Update();
		mCloseButton.Update();
		GameButton[] array = mComicButtons;
		foreach (GameButton gameButton in array)
		{
			gameButton.Update();
		}
	}

	public override void Dispose()
	{
		mCloseButton.Dispose();
		base.Dispose();
	}

	public void UpdateScreen()
	{
		mHalfDeltaWidth = (mApp.mScreenScales.mVirtualWidth - Constants.BOARD_WIDTH) / 2;
		mHalfDeltaHeight = (mApp.mScreenScales.mVirtualHeight - Constants.BOARD_HEIGHT) / 2;
		Resize(-mHalfDeltaWidth, -mHalfDeltaHeight, mApp.mScreenScales.mVirtualWidth, mApp.mScreenScales.mVirtualHeight);
		UpdateUIPosition();
	}

	public void UpdateUIPosition()
	{
		mCloseButton.Resize(Constants.Almanac_CloseButtonRect.mX + mHalfDeltaWidth, Constants.Almanac_CloseButtonRect.mY, Constants.Almanac_CloseButtonRect.mWidth, Constants.Almanac_CloseButtonRect.mHeight);
		if (mComicWidget != null)
		{
			mComicWidget.Resize(0, 0, mWidth, mHeight);
		}
		GameButton[] array = mComicButtons;
		foreach (GameButton gameButton in array)
		{
			gameButton.Resize(mHalfDeltaWidth + 100 + 100 - gameButton.mButtonImage.mWidth / 2 + (gameButton.mId - 2000) % 7 * 100, Constants.Almanac_PlantScrollRect.mY + (gameButton.mId - 2000) / 7 * 100, gameButton.mButtonImage.mWidth, gameButton.mButtonImage.mHeight);
		}
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (mCloseButton.IsMouseOver())
		{
			mApp.KillDialog(3);
			if (mListener != null)
			{
				mListener.BackFromAlmanac();
			}
			return;
		}
		GameButton[] array = mComicButtons;
		foreach (GameButton gameButton in array)
		{
			if (gameButton.IsMouseOver())
			{
				ComicImportDataEntry comicImportDataEntry = ComicImportData.mComics[(Comics)(gameButton.mId - 2000)];
				ComicReaderWidget comicReaderWidget = new ComicReaderWidget(GenerateComicPages(comicImportDataEntry.mLoadGroupPrefix, comicImportDataEntry.mPageCount), GenerateComicPages(comicImportDataEntry.mImagePrefix, comicImportDataEntry.mPageCount), LoadComicThumbnails(comicImportDataEntry.mComicId), comicImportDataEntry.mPageCount);
				comicReaderWidget.Resize(0, 0, Constants.BackBufferSize.Y, Constants.BackBufferSize.X);
				AddWidget(comicReaderWidget);
				BringToFront(comicReaderWidget);
				mApp.mWidgetManager.SetFocus(comicReaderWidget);
				comicReaderWidget.mLastFocusedWidget = this;
				mComicWidget = comicReaderWidget;
			}
		}
		static string[] GenerateComicPages(string prefix, int count)
		{
			string[] array2 = new string[count];
			for (int j = 0; j < count; j++)
			{
				array2[j] = prefix + (j + 1);
				if (array2[j] == null)
				{
					return null;
				}
			}
			return array2;
		}
	}

	public Image[] LoadComicThumbnails(Comics theComic)
	{
		ComicImportDataEntry comicImportDataEntry = ComicImportData.mComics[theComic];
		mApp.DelayLoadComicThumbnailsResource(comicImportDataEntry.mLoadGroupPrefix);
		Image[] array = new Image[comicImportDataEntry.mPageCount];
		for (int i = 0; i < comicImportDataEntry.mPageCount; i++)
		{
			array[i] = AtlasResources.GetImageInAtlasById(AtlasResources.GetAtlasIdByStringId(comicImportDataEntry.mImagePrefix.Replace("IMAGE_", "IMAGE_THUMBNAIL_") + (i + 1)));
			if (array[i] == null)
			{
				return null;
			}
		}
		return array;
	}

	public override bool BackButtonPress()
	{
		return true;
	}
}
