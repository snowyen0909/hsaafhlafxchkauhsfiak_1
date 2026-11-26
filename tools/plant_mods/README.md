# 植物模组（Mod 版）

游戏 → 中间层（`src/LawnShared/Lawn/Mods/ModPlants.cs`）→ 模组。

- 模组放在 `Documents/mods/植物模组/`（文件 App 里看得到），一个模组是一个 `.zip` 或一个文件夹，里面有 `mod.json`。
- 只在游戏启动时载入：加入、删除、启用、停用都要重开游戏。主菜单「模组」可以开关每个模组。
- 模组里只有图片、Spine 动画和数值，没有代码（iOS 不允许运行下载的代码）。
- 每种模组植物「借用」一个游戏植物的行为（`base`），换上自己的名字、卡片、身体和数值。
- 存档里模组植物用编号保存（`userdata/mod/plant_mods.json` 记住编号，从 100 开始，不会重复使用）。模组被删掉后，场上的那棵会变成豌豆射手。

## mod.json

```json
{
  "id": "tavel_super_gatling",
  "name": "超级机枪射手",
  "version": "1.0",
  "author": "塔维尔",
  "description": "模组说明",
  "plants": [
    {
      "id": "super_gatling",
      "name": "超级机枪射手",
      "description": "图鉴 / 提示文字，可用 \n 换行",
      "base": "Gatlingpea",
      "cost": 400,
      "recharge": 7.5,
      "health": 300,
      "fireInterval": 0.9,
      "damage": 1.5,
      "card": "cards/super_gatling.png",
      "spine": "spine/super_gatlingpea",
      "spineScale": 14.3,
      "scale": 1.375,
      "offset": [-28, -83],
      "animations": { "idle": "stand", "attack": "attack" }
    }
  ]
}
```

| 字段 | 意思 |
|---|---|
| `id` | 模组 id（英文、数字、`_`、`-`），不能和别的模组重复 |
| `plants[].base` | 借用行为的游戏植物：Peashooter、Sunflower、Cherrybomb、Wallnut、Potatomine、Snowpea、Chomper、Repeater、Squash、Threepeater、Jalapeno、Torchwood、Tallnut、Cactus、Splitpea、Starfruit、Cabbagepult、Kernelpult、Garlic、Melonpult、Gatlingpea、Twinsunflower、Wintermelon |
| `cost` | 阳光 |
| `recharge` | 卡片冷却（秒） |
| `health` | 血量（0 = 跟 base 一样） |
| `fireInterval` | 攻击间隔（秒，0 = 跟 base 一样） |
| `damage` | 子弹伤害倍数 |
| `card` | 卡片图（建议 162×212，和游戏卡片同样的构图：人物在上半，卡框在下半） |
| `spine` | Spine 4.2 文件（不含扩展名），要有 `.skel`、`.atlas` 和图片页；没有就用 base 的身体 |
| `spineScale` | 导出比例（塔维尔的模型是 14.3；游戏自己的大多是 16） |
| `scale` | 在草坪上再放大多少 |
| `offset` | 身体位置微调（游戏单位，x 向右、y 向下） |
| `animations` | Spine 里待机 / 攻击动画的名字 |

## 示例模组

`tavel_super_gatling.zip`：塔维尔的超级机枪射手（原作者委托移植）。素材来自 PC 版 `Payload/spanim/super_gatlingpea.*` 和 `assets/super_gatling/card.png`。

PC 版的特殊玩法（随机属性豌豆、大招）是 Python 代码，iOS 不能运行；这一版只用数值（更快的攻速、1.5 倍伤害）表现。

## 桌面测试

`PVZ_DORM_AUTOTEST=1 PVZ_DORM_SCENE=mods`（选卡、种下、存档读档）；加 `PVZ_MOD_UI=1` 测「模组」画面。

`menu_icon.py` 画主菜单的「模组」图标。
