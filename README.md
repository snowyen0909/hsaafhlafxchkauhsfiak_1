# 植物娘大战僵尸 iOS 移植（个人研究用）

> 私有仓库。游戏美术/音频版权归 EA/PopCap 与原 mod 作者「庄不纯团队」；代码引擎来自 MIT 授权的.

> [PlantsVsZombies.NET](https://github.com/Mewnojs/PlantsVsZombies.NET)。仅限自用，**不要公开或上架**。

## 结构

| 路径 | 内容 |
|---|---|
| `src/LawnShared/` | 由 Android 1.3.4 的 `Lawn.dll` 反编译出的游戏代码（已去除 Android / MonoMod / IronPython / Bugly） |
| `src/LawnShared/Platform/` | 新写的平台层：`PvZActivity`（替代 Android Activity）、对话框、震动、打开网址、iOS 软键盘输入 |
| `src/Lawn_iOS/` | iOS App 工程（MonoGame.Framework.iOS 3.8.4.1，net10.0-ios） |
| `src/Lawn.Check/` | 仅用于在 Linux/Windows 上快速检查能否编译（DesktopGL，不可运行） |
| `Content/` | 游戏资源（APK `assets/Content`，背景音乐已由 `tools/prepare_ios_content.py` 转为 m4a） |
| `lib/` | APK 中的纯托管库：spine-monogame、NVorbis、MersenneTwister |

## 与 Android 版的差异

- **Python mod 系统移除**（iOS 禁止运行时改代码）。`no_comic.py` 的效果已直接写进代码（`Edition.HasComic = false`）。
- **作弊菜单**原本靠 MonoMod 运行时 Hook，现改为在 `Coin.Update`、`Board.UpdateGame` 等方法内直接判断开关（`ZBCheatHooks.cs`）。
- **MO3 动态鼓点音乐**依赖原生 libopenmpt，iOS 版暂未内置 → 自动退回普通 ogg/m4a 背景音乐。
- 存档在 App 的「文件」目录（文件 App → 我的 iPhone → 植物娘大战僵尸），错误日志写到同目录 `error.log`。

## 编译

推送到 `main` 后 GitHub Actions（macOS 机器）自动编译，到 Actions 页面下载 `PVZ-iOS-unsigned-ipa`，
再用 Sideloadly / AltStore / SideStore / TrollStore 用你自己的 Apple ID 签名安装。
