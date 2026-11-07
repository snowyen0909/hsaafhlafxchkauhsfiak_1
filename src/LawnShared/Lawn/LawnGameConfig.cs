using System.Text.Json.Serialization;
using Sexy;

namespace Lawn;

[JsonSerializable(typeof(LawnGameConfig))]
public class LawnGameConfig
{
	public string mStoragePath;

	public string mCustomConfigPath;

	public TPoint? mScreenSize;

	public bool? mFullscreen;

	public Constants.LanguageIndex? mLocale;

	public bool? mIronpythonEnabled;

	public short? mIronpythonPort;

	public string mCurrentUser;

	public static LawnGameConfig operator +(LawnGameConfig configOriginal, LawnGameConfig configOverwrite)
	{
		return new LawnGameConfig
		{
			mStoragePath = (configOverwrite.mStoragePath ?? configOriginal.mStoragePath),
			mCustomConfigPath = (configOverwrite.mCustomConfigPath ?? configOriginal.mCustomConfigPath),
			mScreenSize = (configOverwrite.mScreenSize ?? configOriginal.mScreenSize),
			mFullscreen = (configOverwrite.mFullscreen ?? configOriginal.mFullscreen),
			mLocale = (configOverwrite.mLocale ?? configOriginal.mLocale),
			mIronpythonEnabled = (configOverwrite.mIronpythonEnabled ?? configOriginal.mIronpythonEnabled),
			mIronpythonPort = (configOverwrite.mIronpythonPort ?? configOriginal.mIronpythonPort),
			mCurrentUser = (configOverwrite.mCurrentUser ?? configOriginal.mCurrentUser)
		};
	}

	public static LawnGameConfig operator -(LawnGameConfig configOriginal, LawnGameConfig configExclude)
	{
		LawnGameConfig lawnGameConfig = new LawnGameConfig
		{
			mStoragePath = ((configExclude.mStoragePath == null) ? configOriginal.mStoragePath : null),
			mCustomConfigPath = ((configExclude.mCustomConfigPath == null) ? configOriginal.mCustomConfigPath : null)
		};
		TPoint? tPoint = configExclude.mScreenSize;
		lawnGameConfig.mScreenSize = ((!tPoint.HasValue) ? configOriginal.mScreenSize : ((TPoint?)null));
		bool? flag = configExclude.mFullscreen;
		lawnGameConfig.mFullscreen = ((!flag.HasValue) ? configOriginal.mFullscreen : ((bool?)null));
		Constants.LanguageIndex? languageIndex = configExclude.mLocale;
		lawnGameConfig.mLocale = ((!languageIndex.HasValue) ? configOriginal.mLocale : ((Constants.LanguageIndex?)null));
		flag = configExclude.mIronpythonEnabled;
		lawnGameConfig.mIronpythonEnabled = ((!flag.HasValue) ? configOriginal.mIronpythonEnabled : ((bool?)null));
		short? num = configExclude.mIronpythonPort;
		lawnGameConfig.mIronpythonPort = ((!num.HasValue) ? configOriginal.mIronpythonPort : ((short?)null));
		lawnGameConfig.mCurrentUser = ((configExclude.mCurrentUser == null) ? configOriginal.mCurrentUser : null);
		return lawnGameConfig;
	}
}
