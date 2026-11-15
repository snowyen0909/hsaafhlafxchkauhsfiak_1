using System;
using System.Collections.Generic;
using System.Text.Json;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

internal sealed class GameUpdate
{
	internal enum ReleaseType
	{
		Release,
		Development,
		Patch
	}

	internal readonly Version mVersion;

	internal readonly ReleaseType mReleaseType;

	internal readonly Dictionary<Constants.LanguageIndex, string> mChangeLog;

	internal readonly List<(CoinType mType, int mCount)> mRewards;

	private static readonly GameUpdate[] mUpdates = new GameUpdate[14]
	{
		new GameUpdate(new Version(0, 9, 26), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 雪橇区2出怪更改，增加螺旋桨僵尸频率/cd\n/c[cyan] - 仙人掌狂暴模式、支持屋顶高地/cd\n/c[cyan] - 修复部分植物名、僵尸名翻译/cd\n/c[cyan] - 泳池派对新增螺旋桨僵尸/cd\n/c[cyan] - 修复螺旋桨僵尸可能进入无敌状态的问题/cd\n/c[cyan] - 辣椒花园增加植物语音/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 0, 0), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 三叶草娘化/cd\n/c[cyan] - 画质更新：胆小菇、大蒜/cd\n/c[cyan] - 新僵尸：螺旋桨僵尸/cd\n/c[cyan] - 新关卡：空袭2 小游戏/cd\n/c[cyan] - 新关卡：雪橇区2 小游戏/cd\n/c[cyan] - 新关卡：大泳池 生存 普通难度/cd\n/c[cyan] - 新增植物卡片显示植物名的设置，可关闭/cd\n/c[cyan] - 增强仙人掌在屋顶场景和对空的强度/cd\n/c[cyan] - 修正部分植物名、僵尸名、关卡名的翻译/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 1, 1), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 新增间章三、魅惑菇漫画/cd\n/c[pink] - 希芙洛姆为香蒲猫猫设计的炫彩皮肤，现于商店热卖中/cd\n/c[orange] - 新关卡：融合！小游戏/cd\n/c[cyan] - 修复胆小菇裙子在加速时的问题/cd\n/c[cyan] - 启用花园放大互动、花园放大模式植物娘命名补齐/cd\n/c[cyan] - 修复显名模式文字出界的问题、开启显名模式导致部分游戏模式崩溃的问题/cd\n/c[cyan] - 修复传送带可以提前捡起种子包的问题/cd\n/c[cyan] - 仙人掌子弹索敌修复/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 1, 5), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 机枪射手娘化/cd\n/c[cyan] - 增加一些小游戏的花盆爆率/cd\n/c[cyan] - 现在手套移动植物时植物不会攻击/cd\n/c[cyan] - 修复作弊按键可被电脑版左Alt键意外开启的问题/cd\n/c[cyan] - 修复随机模仿者有概率崩溃的问题/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 1, 8), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 大喷菇娘化/cd\n/c[cyan] - 画质更新：路灯花（包含隐藏彩蛋）/cd\n/c[cyan] - 植物娘漫画：对16本漫画重新嵌字整理/cd\n/c[cyan] - 机枪射手现在不会对梯子上的僵尸采用近战攻击/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 2, 3), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 寒冰射手娘化/cd\n/c[cyan] - 商店新增大喷菇/cd /c[pink]爱的小屋/cd\n/c[cyan] - 略微增加大喷菇的大小/cd\n/c[orange] - 生存模式(地狱难度)新增手套功能/cd\n/c[orange] - 优化移动端暂停和加速按钮位置/cd\n/c[lime] - 修复机枪射手近战判定/cd\n/c[pink] - 修复香蒲皮肤/cd\n/c[orange] - 优化南瓜壳动画的性能/cd\n/c[lightgray] - 修复忍者僵尸的视觉问题与火炬树桩判定问题/cd\n/c[pink] - 大喷菇漫画/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 2, 5), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 寒冰菇娘化/cd\n/c[cyan] - 海蘑菇娘化/cd\n/c[cyan] - 替换了铲子和手套的皮肤/cd\n/c[cyan] - 使机枪射手近战攻击矿工僵尸/cd\n/c[cyan] - 修复胆小菇裙子的视觉问题/cd\n/c[cyan] - 修复传送带种子包等太久的问题/cd\n/c[cyan] - 修复用户选择对话框的显示问题/cd\n/c[cyan] - 修正部分漫画内容/cd\n/c[cyan] - 手机端主动退出按钮/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 2, 6), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 修复海蘑菇造成游戏出错的问题/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 3, 0), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 叶子保护伞娘化/cd\n/c[cyan] - 寒冰菇漫画/cd\n/c[cyan] - 新增冒险模式高难开关：盆栽爆率1.5倍，加入原创僵尸/cd\n/c[cyan] - 加入了10+可下载关卡，欢迎加入在线关卡社区一起制作关卡/cd\n/c[cyan] - 小游戏：占领高地/cd\n/c[cyan] - 小游戏：随机挑战/cd\n/c[cyan] - 电脑端全屏模式现在通过Alt+Enter键进入/cd\n/c[cyan] - 减小植物动画受到的风力幅度/cd\n/c[cyan] - 修复了仙人掌对僵王索敌的问题/cd\n/c[cyan] - 修复关卡列表的视觉问题/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 3, 1), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 补充叶子保护伞的图鉴介绍/cd\n/c[cyan] - 修复在花园中移动模仿者生成僵尸的问题/cd\n/c[cyan] - 修复安卓版无法进入部分在线关卡的问题/cd\n/c[cyan] - 修复随机挑战的卡槽数量/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 3, 2), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 裂荚射手娘化/cd\n/c[cyan] - 画质更新：玉米投手/cd\n/c[cyan] - 小喷菇漫画cd\n/c[lightgray] - 背景音乐动态鼓点/cd\n/c[cyan] - 调整了冒险模式高难的5-5关卡/cd\n/c[cyan] - 保存作弊菜单的一些设置/cd\n/c[cyan] - 修复手套移动火红莲造成的视觉问题/cd\n/c[cyan] - 修复传送带卡片出现时间过长的问题/cd\n/c[cyan] - 修复与大泳池香蒲有关的报错/cd\n/c[cyan] - 隐藏在线关卡的ID/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 3, 4), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 修复安卓版鼓点相关的报错问题/cd" }, null, ReleaseType.Release),
		new GameUpdate(new Version(1, 4, 0), new Dictionary<Constants.LanguageIndex, string> { [Constants.LanguageIndex.zh_cn] = "/c[cyan] - 金盏花娘化/cd\n/c[cyan] - 邮箱系统/cd\n/c[cyan] - 公开版更新奖励通过邮箱发放/cd\n/c[cyan] - 植物僵尸回归（重置计划暂缓）/cd\n/c[cyan] - 作弊菜单草坪放大功能/cd\n/c[cyan] - 修复花园剩余容量计算的问题/cd\n/c[cyan] - 修复暂停界面的僵尸动画/cd\n/c[cyan] - 删除屋顶冒险模式(困难)中的螺旋桨僵尸/cd" }, new List<(CoinType, int)>
		{
			(CoinType.Chocolate, 3),
			(CoinType.Diamond, 10),
			(CoinType.PresentPlant, 3)
		}),
		new GameUpdate(new Version(1, 4, 1), null, null, ReleaseType.Release)
	};

	private GameUpdate(Version theVersion, Dictionary<Constants.LanguageIndex, string> theChangeLog = null, List<(CoinType mType, int mCount)> theRewards = null, ReleaseType theReleaseType = ReleaseType.Development)
	{
		mVersion = theVersion;
		mReleaseType = theReleaseType;
		mChangeLog = theChangeLog;
		mRewards = theRewards;
	}

	internal static string GetAnnouncement(string theAppVersion)
	{
		string versionText = GetVersionText(theAppVersion);
		string value = string.Empty;
		if (Version.TryParse(versionText, out Version result))
		{
			for (int num = mUpdates.Length - 1; num >= 0; num--)
			{
				GameUpdate gameUpdate = mUpdates[num];
				if (gameUpdate.mVersion <= result && gameUpdate.mChangeLog != null)
				{
					if (!gameUpdate.mChangeLog.TryGetValue(Constants.Language, out value))
					{
						value = gameUpdate.mChangeLog[Constants.LanguageIndex.zh_cn];
					}
					break;
				}
			}
		}
		return TodStringFile.TodStringTranslate("[UPDATE_CHANGELOG_INTRO]").Replace("{VERSION}", versionText) + "\n" + value;
	}

	internal static bool ApplyVersion(PlayerInfo thePlayerInfo)
	{
		string versionText = GetVersionText(LawnApp.AppVersionNumber);
		if (!Version.TryParse(versionText, out Version result))
		{
			return false;
		}
		string mSaveStateAppVersionNumber = thePlayerInfo.mSaveStateAppVersionNumber;
		string mHighestAppVersionNumber = thePlayerInfo.mHighestAppVersionNumber;
		LocalMail localMail = null;
		bool flag = Version.TryParse(GetVersionText(mHighestAppVersionNumber), out Version result2);
		if (!flag || result > result2)
		{
			if (flag)
			{
				for (int num = mUpdates.Length - 1; num >= 0; num--)
				{
					GameUpdate gameUpdate = mUpdates[num];
					if (!(gameUpdate.mVersion > result))
					{
						List<(CoinType, int)> list = gameUpdate.mRewards;
						if (list != null && list.Count > 0)
						{
							if (gameUpdate.mVersion > result2)
							{
								localMail = new LocalMail(MailType.Text, JsonSerializer.SerializeToElement(new
								{
									TitleKey = "[MAIL_UPDATE_REWARD_TITLE]",
									BodyKey = "[MAIL_UPDATE_REWARD_BODY]",
									Version = versionText
								}), theDeleteOnlyAfterRead: true, gameUpdate.mRewards.ToArray());
								thePlayerInfo.mMail.Add(localMail);
							}
							break;
						}
					}
				}
			}
			thePlayerInfo.mHighestAppVersionNumber = LawnApp.AppVersionNumber;
		}
		thePlayerInfo.mSaveStateAppVersionNumber = LawnApp.AppVersionNumber;
		if (thePlayerInfo.SaveDetails())
		{
			return true;
		}
		thePlayerInfo.mSaveStateAppVersionNumber = mSaveStateAppVersionNumber;
		thePlayerInfo.mHighestAppVersionNumber = mHighestAppVersionNumber;
		if (localMail != null)
		{
			thePlayerInfo.mMail.Remove(localMail);
		}
		return false;
	}

	private static string GetVersionText(string theAppVersion)
	{
		int num = theAppVersion?.IndexOf("PGvZ ", StringComparison.Ordinal) ?? (-1);
		if (num < 0)
		{
			return string.Empty;
		}
		num += 5;
		int num2 = theAppVersion.IndexOf(' ', num);
		if (num2 >= 0)
		{
			return theAppVersion.Substring(num, num2 - num);
		}
		return theAppVersion.Substring(num);
	}
}
