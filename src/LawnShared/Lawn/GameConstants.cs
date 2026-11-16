using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class GameConstants
{
	public class CoverInfo
	{
		public float mX;

		public float mY;

		public float mScale;

		public CoverInfo(float mX, float mY, float mScale)
		{
			this.mX = mX;
			this.mY = mY;
			this.mScale = mScale;
		}
	}

	public const int SURVIVAL_NORMAL_FLAGS = 5;

	public const int SURVIVAL_HARD_FLAGS = 10;

	public const int SURVIVAL_HELL_FLAGS = 12;

	public const int LAST_STAND_FLAGS = 5;

	public const int FINAL_LEVEL = 50;

	public const int LEVELS_PER_AREA = 10;

	public const float SOUND_EFFECT_VOLUME_FACTOR = 0.65f;

	public const int NUM_CHALLENGE_MODES = 102;

	public const int PRICE_MULTIPLIER = 10;

	public const int MINI_GAME_COUNT = 20;

	public const int I_ZOMBIE_LEVEL_COUNT = 10;

	public const int VASEBREAKER_LEVEL_COUNT = 10;

	public const int VASEBREAKER_ACHIEVEMENT = 15;

	public const int IZOMBIE_ACHIEVEMENT = 10;

	public const int LAWN_IMAGE_AVE_MS_TO_LOAD = 9;

	public const int LAWN_SOUND_AVE_MS_TO_LOAD = 54;

	public const int LAWN_FONT_AVE_MS_TO_LOAD = 54;

	public const int LAWN_PARTICLE_AVE_MS_TO_LOAD = 6;

	public const int LAWN_REANIM_AVE_MS_TO_LOAD = 68;

	public const int CRAVE_DAVE_BLINK_RATE = 400;

	public const int MAX_ZOMBIE_WAVES = 100;

	public const int MAX_ZOMBIES_IN_WAVE = 50;

	public const int MAX_ZOMBIE_TYPES = 100;

	public const int FOG_BLOW_RETURN_TIME = 2000;

	public const int FOG_CUTSCENE_TIME = 200;

	public const int NUM_FLAMES_IN_FWOOSH = 12;

	public const int NUM_FOG_ROWS = 7;

	public const int PROGRESS_METER_COUNTER = 150;

	public const int NUM_LEVELS = 50;

	public const int BOARD_SHAKE_TIME = 12;

	public const int FLAG_RAISE_TIME = 100;

	public const int ZOMBIE_COUNTDOWN_FIRST_WAVE = 1800;

	public const int ZOMBIE_COUNTDOWN = 2500;

	public const int ZOMBIE_COUNTDOWN_RANGE = 600;

	public const int ZOMBIE_COUNTDOWN_BEFORE_FLAG = 4500;

	public const int ZOMBIE_COUNTDOWN_BEFORE_REPICK = 5499;

	public const int ZOMBIE_COUNTDOWN_MIN = 400;

	public const int SUN_COUNTDOWN = 425;

	public const int SUN_COUNTDOWN_RANGE = 275;

	public const int SUN_COUNTDOWN_MAX = 950;

	public const int LAWN_MOWER_COIN_START = 200;

	public const int LAWN_MOWER_COIN_DELAY = 40;

	public const int LAWN_MOWER_COIN_END = 150;

	public const int MAX_RENDER_ITEMS = 2048;

	public const int ROOF_SLOPE_PER_ROW = 20;

	public const int ROOF_SLOPE_PER_ROW_OFF = 10;

	public const int BLINK_RATE = 400;

	public const int BLINK_RATE_WALLNUT = 1000;

	public const int GRAVE_BUSTER_EAT_TIME = 400;

	public const int CRATER_TIME = 18000;

	public const int MAX_MAGNET_ITEMS = 5;

	public const int RENDER_GROUP_PUMPKIN_BACK = 1;

	public const int WAKE_UP_TIME = 100;

	public const int SEEDBANK_MAX = 10;

	public const int SLOT_MACHINE_TIME = 400;

	public const int CORN_ROULETTE_BTN_Y = 227;

	public const int CORN_ROULETTE_BTN_LEFT = 118;

	public const int CORN_ROULETTE_BTN_RIGHT = 519;

	public const int CONVEYOR_SPEED = 4;

	public const int CONVEYOR_PACKET_OFFSET_Y = 35;

	public const int CONVEYOR_PACKET_MIN_OFFSET = 40;

	public const int TICKS_PER_SECOND = 100;

	public const int REANIMATOR_LOAD_TASK_FACTOR = 0;

	public const int TAP_TAP_TO_PLANT = 1;

	public const int PARTNER_PREVIEW_MAX_DRIFT = 300;

	public const int NO_IMAGE_ID = 999999;

	public const int PRESENT_OFFSET_Y = 60;

	public const int MAX_CREDIT_SECTIONS = 57;

	public const int NUM_CLOUDS = 6;

	public const int NUM_FLOWERS = 3;

	public const int CLOUD_MOVE_TIME = 6000;

	public const int CLOUD_WAIT_TIME = 2000;

	public const int LEAF_WAIT_TIME = 200;

	public const int QUICKPLAY_SLIDE_COUNT = 30;

	public const int SLIDE_COUNT = 75;

	public const int MORE_GAMES_LIST_OFFSET = 52;

	public const int NUM_MOTION_TRAIL_FRAMES = 12;

	public const int CHOMP_TIME = 50;

	public const int MAX_MESSAGE_LENGTH = 128;

	public const int SLIDE_OFF_TIME = 100;

	public const int MIN_MESSAGE_TIME = 100;

	public const int MUSIC_ROW_FACTOR = 4;

	public const int BURST_FADE_IN_TIME = 400;

	public const int BURST_TIME_AFTER_START_TO_QUEUE_DRUMS = 300;

	public const int BURST_MIN_TIME = 800;

	public const int BURST_FADE_OUT_TIME = 800;

	public const int BURST_FOG_FADE_OUT_DELAY = 300;

	public const int DRUMS_FOG_FADE_OUT_TIME = 800;

	public const int DRUMS_FADE_OUT_TIME = 50;

	public const int LAWN_SONG_AVE_MS_TO_LOAD = 3500;

	public const int MAX_CHALLENGE_MODES = 200;

	public const int MAX_PURCHASES = 80;

	public const int NUM_PLACEHOLDER_INTS = 1;

	public const int MAX_POTTED_PLANTS = 200;

	public const int ITEM_GAP = 10;

	public const int MENU_BUTTON_TOP_OFFSET = 2;

	public const int STREET_GRID_SIZE_X = 5;

	public const int STREET_GRID_SIZE_Y = 5;

	public const int BEGHOULED_WINNING_SCORE = 75;

	public const int NUM_SQUIRRELS = 7;

	public const int SLOT_MACHINE_WINNING_SCORE = 2000;

	public const int ZOMBIQUARIUM_WINNING_SCORE = 1000;

	public const int IZOMBIE_WINNING_SCORE = 5;

	public const int NUM_TREE_CLOUDS = 6;

	public const int SLOT_MACHINE_COST = 25;

	public const int ART_CHALLEGE_SIZE_X = 9;

	public const int ART_CHALLEGE_SIZE_Y = 6;

	public const int STORM_FLASH_TIME = 150;

	public const int ICE_CHALLANGE_DELAY = 3000;

	public const int BEGHOULED_DELAY_BEFORE_HINT_FLASH = 1500;

	public const int BEGHOULED_DELAY_BEFORE_HINT_FLASH_AGAIN = 1500;

	public const int MAX_SCARY_POTS = 54;

	public const int RENDER_GROUP_TREE_BACKGROUND = 1;

	public const int RENDER_GROUP_TREE_TRUNK = 2;

	public const int RENDER_GROUP_TREE_GRASS = 3;

	public const int RENDER_GROUP_TREE_TOP = 4;

	public const int TREE_CLOUD_MOVE_TIME = 6000;

	public const int TREE_CLOUD_WAIT_TIME = 2000;

	public const int ZEN_GARDEN_FADE_DELAY = 3000;

	public const int STINKY_RENDER_ORDER_OFFSET = 30;

	public const float STINKY_COIN_OFFSET_Y = 30f;

	public static Achievement[] AchievementInfo;

	internal static int[,] gLawnDialogColors;

	public static SexyColor[] gGameButtonColors;

	public static ParticleParams[] gLawnParticleArray;

	public static ReanimationParams[] gLawnReanimationArray;

	public static TrailParams[] gLawnTrailArray;

	public static int gLawnStringFormatCount;

	public static TodStringListFormat[] gLawnStringFormats;

	public static int[] gZombieWaves;

	public static ZombieAllowedLevels[] gZombieAllowedLevels;

	public static PlantDefinition[] gPlantDefs;

	public static int NUM_BACKUP_DANCERS;

	public static int MAX_ZOMBIE_FOLLOWERS;

	public static int ZOMBIE_WAVE_DEBUG;

	public static int ZOMBIE_WAVE_CUTSCENE;

	public static int ZOMBIE_WAVE_UI;

	public static int ZOMBIE_WAVE_WINNER;

	public static int RENDER_GROUP_SHIELD;

	public static int RENDER_GROUP_ARMS;

	public static int RENDER_GROUP_OVER_SHIELD;

	public static int RENDER_GROUP_BOSS_BACK_LEG;

	public static int RENDER_GROUP_BOSS_FRONT_LEG;

	public static int RENDER_GROUP_BOSS_BACK_ARM;

	public static int RENDER_GROUP_BOSS_FIREBALL_ADDITIVE;

	public static int RENDER_GROUP_BOSS_FIREBALL_TOP;

	public static float CHILLED_SPEED_FACTOR;

	public static int ZOMBIE_LIMP_SPEED_FACTOR;

	public static int BUNGEE_ZOMBIE_HEIGHT;

	public static int DOG_WALKING_DISTANCE;

	public static int POGO_BOUNCE_TIME;

	public static int DOLPHIN_JUMP_TIME;

	public static float THOWN_ZOMBIE_GRAVITY;

	public static int TICKS_BETWEEN_EATS;

	public static float CLIP_HEIGHT_OFF;

	public static float CLIP_HEIGHT_LIMIT;

	public static SexyColor ZOMBIE_MINDCONTROLLED_COLOR;

	public static int ZOMBIE_BLINK_RATE;

	public static int BOSS_FLASH_HEALTH_FRACTION;

	public static float ZOMBIE_WALK_IN_FRONT_DOOR_Y;

	public static int BOBSLED_CRASH_TIME;

	public static int YUCKI_PAUSE_TIME;

	public static int YUCKI_SHORT_PAUSE_TIME;

	public static int YUCKI_HOLD_TIME;

	public static int YUCKI_WALK_TIME;

	public static float BOSS_BALL_OFFSET_Y;

	public static ZombieDefinition[] gZombieDefs;

	public static ZombieType[] gBossZombieList;

	public static ZombieType[] gBossZombieListHard;

	public static ZombieType[] gBossZombieList2;

	public static SeedType[] gFinalBoss2RainingSeeds;

	public static bool TESTING_LOAD_BAR;

	public static int BAR_FADE_TIME;

	public static int NUM_ALMANAC_REANIMS;

	public static int NUM_ALMANAC_SEEDS;

	public static int NUM_ALMANAC_ZOMBIES;

	public static int NUM_ALMANAC_ZOMBIES_PERF_TEST;

	public static int BUTTON_SIZE;

	public static int BUTTONS_PER_ROW;

	public static readonly LawnFPoint[] gFlowerCenter;

	public static double MUSIC_SLIDER_THRESHOLD;

	public static int gUserVersionTilJuly07;

	public static int gUserVersionTilFeb08;

	public static int gUserVersionTilApril08;

	public static int gUserVersionTilMay08;

	public static int gUserVersionTilSep08;

	public static int gUserVersionTilNov08;

	public static int gUserVersionTilJan09;

	public static int gUserVersionTilApr09;

	public static int gUserVersion;

	public static ProjectileDefinition[] gProjectileDefinition;

	public static CoverInfo[] gCoverInfos;

	public const bool ENABLE_CLASSIC_SEED_CHOOSER = false;

	internal static int SEED_BANK_OFFSET_X;

	internal static int SEED_BANK_OFFSET_X_END;

	public static void Init()
	{
		AchievementInfo = new Achievement[13]
		{
			new Achievement(10223, "[ACHIEVEMENT_HOME_SECURITY_TITLE]", "[ACHIEVEMENT_HOME_SECURITY_DESCRIPTION]"),
			new Achievement(10224, "[ACHIEVEMENT_SPUDOW_TITLE]", "[ACHIEVEMENT_SPUDOW_DESCRIPTION]"),
			new Achievement(10225, "[ACHIEVEMENT_EXPLODONATOR_TITLE]", "[ACHIEVEMENT_EXPLODONATOR_DESCRIPTION]"),
			new Achievement(10226, "[ACHIEVEMENT_MORTICULTURALIST_TITLE]", "[ACHIEVEMENT_MORTICULTURALIST_DESCRIPTION]"),
			new Achievement(10227, "[ACHIEVEMENT_DONT_PEA_IN_POOL_TITLE]", "[ACHIEVEMENT_DONT_PEA_IN_POOL_DESCRIPTION]"),
			new Achievement(10228, "[ACHIEVEMENT_ROLL_SOME_HEADS_TITLE]", "[ACHIEVEMENT_ROLL_SOME_HEADS_DESCRIPTION]"),
			new Achievement(10229, "[ACHIEVEMENT_GROUNDED_TITLE]", "[ACHIEVEMENT_GROUNDED_DESCRIPTION]"),
			new Achievement(10230, "[ACHIEVEMENT_ZOMBOLOGIST_TITLE]", "[ACHIEVEMENT_ZOMBOLOGIST_DESCRIPTION]"),
			new Achievement(10231, "[ACHIEVEMENT_PENNY_PINCHER_TITLE]", "[ACHIEVEMENT_PENNY_PINCHER_DESCRIPTION]"),
			new Achievement(10232, "[ACHIEVEMENT_SUNNY_DAYS_TITLE]", "[ACHIEVEMENT_SUNNY_DAYS_DESCRIPTION]"),
			new Achievement(10233, "[ACHIEVEMENT_POPCORN_PARTY_TITLE]", "[ACHIEVEMENT_POPCORN_PARTY_DESCRIPTION]"),
			new Achievement(10234, "[ACHIEVEMENT_GOOD_MORNING_TITLE]", "[ACHIEVEMENT_GOOD_MORNING_DESCRIPTION]"),
			new Achievement(10235, "[ACHIEVEMENT_NO_FUNGUS_AMONG_US_TITLE]", "[ACHIEVEMENT_NO_FUNGUS_AMONG_US_DESCRIPTION]")
		};
		gLawnDialogColors = new int[7, 3]
		{
			{ 255, 255, 255 },
			{ 255, 255, 0 },
			{ 255, 255, 255 },
			{ 255, 255, 255 },
			{ 255, 255, 255 },
			{ 80, 80, 80 },
			{ 255, 255, 255 }
		};
		gLawnParticleArray = new ParticleParams[107]
		{
			new ParticleParams(ParticleEffect.Melonsplash, "particles/melonimpact"),
			new ParticleParams(ParticleEffect.Wintermelon, "particles/WinterMelonImpact"),
			new ParticleParams(ParticleEffect.Fumecloud, "particles/FumeCloud"),
			new ParticleParams(ParticleEffect.Popcornsplash, "particles/PopcornSplash"),
			new ParticleParams(ParticleEffect.Powie, "particles/Powie"),
			new ParticleParams(ParticleEffect.Jackexplode, "particles/JackExplode"),
			new ParticleParams(ParticleEffect.ZombieHead, "particles/ZombieHead"),
			new ParticleParams(ParticleEffect.ZombieArm, "particles/ZombieArm"),
			new ParticleParams(ParticleEffect.ZombieTrafficCone, "particles/ZombieTrafficCone"),
			new ParticleParams(ParticleEffect.ZombiePail, "particles/ZombiePail"),
			new ParticleParams(ParticleEffect.ZombieHelmet, "particles/ZombieHelmet"),
			new ParticleParams(ParticleEffect.ZombieFlag, "particles/ZombieFlag"),
			new ParticleParams(ParticleEffect.ZombieDoor, "particles/ZombieDoor"),
			new ParticleParams(ParticleEffect.ZombieNewspaper, "particles/ZombieNewspaper"),
			new ParticleParams(ParticleEffect.ZombieHeadlight, "particles/ZombieHeadLight"),
			new ParticleParams(ParticleEffect.Pow, "particles/Pow"),
			new ParticleParams(ParticleEffect.ZombiePogo, "particles/ZombiePogo"),
			new ParticleParams(ParticleEffect.ZombieNewspaperHead, "particles/ZombieNewspaperHead"),
			new ParticleParams(ParticleEffect.ZombieBalloonHead, "particles/ZombieBalloonHead"),
			new ParticleParams(ParticleEffect.SodRoll, "particles/SodRoll"),
			new ParticleParams(ParticleEffect.GraveStoneRise, "particles/GraveStoneRise"),
			new ParticleParams(ParticleEffect.Planting, "particles/Planting"),
			new ParticleParams(ParticleEffect.PlantingPool, "particles/PlantingPool"),
			new ParticleParams(ParticleEffect.ZombieRise, "particles/ZombieRise"),
			new ParticleParams(ParticleEffect.GraveBuster, "particles/GraveBuster"),
			new ParticleParams(ParticleEffect.GraveBusterDie, "particles/GraveBusterDie"),
			new ParticleParams(ParticleEffect.PoolSplash, "particles/PoolSplash"),
			new ParticleParams(ParticleEffect.IceSparkle, "particles/IceSparkle"),
			new ParticleParams(ParticleEffect.SeedPacket, "particles/SeedPacket"),
			new ParticleParams(ParticleEffect.TallNutBlock, "particles/TallNutBlock"),
			new ParticleParams(ParticleEffect.Doom, "particles/Doom"),
			new ParticleParams(ParticleEffect.DiggerRise, "particles/DiggerRise"),
			new ParticleParams(ParticleEffect.DiggerTunnel, "particles/DiggerTunnel"),
			new ParticleParams(ParticleEffect.DancerRise, "particles/DancerRise"),
			new ParticleParams(ParticleEffect.PoolSparkly, "particles/PoolSparkly"),
			new ParticleParams(ParticleEffect.WallnutEatSmall, "particles/WallnutEatSmall"),
			new ParticleParams(ParticleEffect.WallnutEatLarge, "particles/WallnutEatLarge"),
			new ParticleParams(ParticleEffect.PeaSplat, "particles/PeaSplat"),
			new ParticleParams(ParticleEffect.ButterSplat, "particles/ButterSplat"),
			new ParticleParams(ParticleEffect.CabbageSplat, "particles/CabbageSplat"),
			new ParticleParams(ParticleEffect.PuffSplat, "particles/PuffSplat"),
			new ParticleParams(ParticleEffect.StarSplat, "particles/StarSplat"),
			new ParticleParams(ParticleEffect.IceTrap, "particles/IceTrap"),
			new ParticleParams(ParticleEffect.SnowpeaSplat, "particles/SnowPeaSplat"),
			new ParticleParams(ParticleEffect.SnowpeaPuff, "particles/SnowPeaPuff"),
			new ParticleParams(ParticleEffect.SnowpeaTrail, "particles/SnowPeaTrail"),
			new ParticleParams(ParticleEffect.LanternShine, "particles/LanternShine"),
			new ParticleParams(ParticleEffect.SeedPacketPickup, "particles/Award"),
			new ParticleParams(ParticleEffect.PotatoMine, "particles/PotatoMine"),
			new ParticleParams(ParticleEffect.PotatoMineRise, "particles/PotatoMineRise"),
			new ParticleParams(ParticleEffect.PuffshroomTrail, "particles/PuffShroomTrail"),
			new ParticleParams(ParticleEffect.PuffshroomMuzzle, "particles/PuffShroomMuzzle"),
			new ParticleParams(ParticleEffect.SeedPacketFlash, "particles/SeedPacketFlash"),
			new ParticleParams(ParticleEffect.WhackAZombieRise, "particles/WhackAZombieRise"),
			new ParticleParams(ParticleEffect.ZombieLadder, "particles/ZombieLadder"),
			new ParticleParams(ParticleEffect.UmbrellaReflect, "particles/UmbrellaReflect"),
			new ParticleParams(ParticleEffect.SeedPacketPick, "particles/SeedPacketPick"),
			new ParticleParams(ParticleEffect.IceTrapZombie, "particles/IceTrapZombie"),
			new ParticleParams(ParticleEffect.IceTrapRelease, "particles/IceTrapRelease"),
			new ParticleParams(ParticleEffect.ZamboniSmoke, "particles/ZamboniSmoke"),
			new ParticleParams(ParticleEffect.Gloomcloud, "particles/GloomCloud"),
			new ParticleParams(ParticleEffect.ZombiePogoHead, "particles/ZombiePogoHead"),
			new ParticleParams(ParticleEffect.ZamboniTire, "particles/ZamboniTire"),
			new ParticleParams(ParticleEffect.ZamboniExplosion, "particles/ZamboniExplosion"),
			new ParticleParams(ParticleEffect.ZamboniExplosion2, "particles/ZamboniExplosion2"),
			new ParticleParams(ParticleEffect.CatapultExplosion, "particles/CatapultExplosion"),
			new ParticleParams(ParticleEffect.MowerCloud, "particles/MowerCloud"),
			new ParticleParams(ParticleEffect.BossIceBall, "particles/BossIceBallTrail"),
			new ParticleParams(ParticleEffect.Blastmark, "particles/BlastMark"),
			new ParticleParams(ParticleEffect.CoinPickupArrow, "particles/CoinPickupArrow"),
			new ParticleParams(ParticleEffect.PresentPickup, "particles/PresentPickup"),
			new ParticleParams(ParticleEffect.ImitaterMorph, "particles/ImitaterMorph"),
			new ParticleParams(ParticleEffect.MoweredZombieHead, "particles/MoweredZombieHead"),
			new ParticleParams(ParticleEffect.MoweredZombieArm, "particles/MoweredZombieArm"),
			new ParticleParams(ParticleEffect.ZombieHeadPool, "particles/ZombieHeadPool"),
			new ParticleParams(ParticleEffect.ZombieBossFireball, "particles/Zombie_boss_fireball"),
			new ParticleParams(ParticleEffect.FireballDeath, "particles/FireBallDeath"),
			new ParticleParams(ParticleEffect.IceballDeath, "particles/IceBallDeath"),
			new ParticleParams(ParticleEffect.IceballTrail, "particles/Iceball_Trail"),
			new ParticleParams(ParticleEffect.FireballTrail, "particles/Fireball_Trail"),
			new ParticleParams(ParticleEffect.BossExplosion, "particles/BossExplosion"),
			new ParticleParams(ParticleEffect.ScreenFlash, "particles/ScreenFlash"),
			new ParticleParams(ParticleEffect.TrophySparkle, "particles/TrophySparkle"),
			new ParticleParams(ParticleEffect.PortalCircle, "particles/PortalCircle"),
			new ParticleParams(ParticleEffect.PortalSquare, "particles/PortalSquare"),
			new ParticleParams(ParticleEffect.PottedPlantGlow, "particles/PottedPlantGlow"),
			new ParticleParams(ParticleEffect.PottedWaterPlantGlow, "particles/PottedWaterPlantGlow"),
			new ParticleParams(ParticleEffect.PottedZenGlow, "particles/PottedZenGlow"),
			new ParticleParams(ParticleEffect.MindControl, "particles/MindControl"),
			new ParticleParams(ParticleEffect.VaseShatter, "particles/VaseShatter"),
			new ParticleParams(ParticleEffect.VaseShatterLeaf, "particles/VaseShatterLeaf"),
			new ParticleParams(ParticleEffect.VaseShatterZombie, "particles/VaseShatterZombie"),
			new ParticleParams(ParticleEffect.AwardPickupArrow, "particles/AwardPickupArrow"),
			new ParticleParams(ParticleEffect.ZombieSeaweed, "particles/Zombie_seaweed"),
			new ParticleParams(ParticleEffect.ZombieMustache, "particles/ZombieMustache"),
			new ParticleParams(ParticleEffect.ZombieFutureGlasses, "particles/ZombieFutureGlasses"),
			new ParticleParams(ParticleEffect.Pinata, "particles/Pinata"),
			new ParticleParams(ParticleEffect.DustSquash, "particles/Dust_Squash"),
			new ParticleParams(ParticleEffect.DustFoot, "particles/Dust_Foot"),
			new ParticleParams(ParticleEffect.Daisy, "particles/Daisy"),
			new ParticleParams(ParticleEffect.Starburst, "particles/Starburst"),
			new ParticleParams(ParticleEffect.UpsellArrow, "particles/UpsellArrow"),
			new ParticleParams(ParticleEffect.ZombieMonkBell, "particles/ZombieMonkBell"),
			new ParticleParams(ParticleEffect.RobotTitanExplosion, "particles/RobotTitanExplosion"),
			new ParticleParams(ParticleEffect.PoolSparklyFull, "particles/PoolSparklyFull"),
			new ParticleParams(ParticleEffect.WallnutNewEatLarge, "particles/WallnutNewEatLarge"),
			new ParticleParams(ParticleEffect.WallnutNewEatSmall, "particles/WallnutNewEatSmall")
		};
		gLawnReanimationArray = new ReanimationParams[199]
		{
			new ReanimationParams(ReanimationType.LoadbarSprout, "reanim/LoadBar_sprout", 1),
			new ReanimationParams(ReanimationType.LoadbarZombiehead, "reanim/LoadBar_Zombiehead", 1),
			new ReanimationParams(ReanimationType.Sodroll, "reanim/SodRoll", 0),
			new ReanimationParams(ReanimationType.FinalWave, "reanim/FinalWave", 1),
			new ReanimationParams(ReanimationType.Peashooter, "spanim/peashooter", 0, isSpine: true, "reanim/PeaShooterSingle"),
			new ReanimationParams(ReanimationType.Wallnut, "reanim/Wallnut", 0),
			new ReanimationParams(ReanimationType.Lilypad, "reanim/LilyPad", 0),
			new ReanimationParams(ReanimationType.Sunflower, "spanim/sunflower", 0, isSpine: true, "reanim/SunFlower"),
			new ReanimationParams(ReanimationType.Lawnmower, "reanim/LawnMower", 0),
			new ReanimationParams(ReanimationType.Readysetplant, "reanim/StartReadySetPlant", 1),
			new ReanimationParams(ReanimationType.Cherrybomb, "reanim/CherryBomb", 0),
			new ReanimationParams(ReanimationType.Squash, "spanim/squash", 0, isSpine: true, "reanim/Squash"),
			new ReanimationParams(ReanimationType.Doomshroom, "reanim/DoomShroom", 0),
			new ReanimationParams(ReanimationType.Snowpea, "spanim/snowpea", 0, isSpine: true, "reanim/SnowPea"),
			new ReanimationParams(ReanimationType.Repeater, "spanim/repeater", 0, isSpine: true, "reanim/PeaShooter"),
			new ReanimationParams(ReanimationType.Sunshroom, "reanim/SunShroom", 0),
			new ReanimationParams(ReanimationType.Tallnut, "spanim/tallnut", 0, isSpine: true, "reanim/Tallnut", 15f),
			new ReanimationParams(ReanimationType.Fumeshroom, "spanim/fumeshroom", 0, isSpine: true, "reanim/FumeShroom", 4.8f),
			new ReanimationParams(ReanimationType.Puffshroom, "spanim/puffshroom", 0, isSpine: true, "reanim/PuffShroom", 18.181818f),
			new ReanimationParams(ReanimationType.Hypnoshroom, "spanim/hypnoshroom", 0, isSpine: true, "reanim/HypnoShroom"),
			new ReanimationParams(ReanimationType.Chomper, "spanim/chomper", 0, isSpine: true, "reanim/Chomper"),
			new ReanimationParams(ReanimationType.Zombie, "reanim/Zombie", 0),
			new ReanimationParams(ReanimationType.Sun, "reanim/Sun", 0),
			new ReanimationParams(ReanimationType.Potatomine, "reanim/PotatoMine", 0),
			new ReanimationParams(ReanimationType.Spikeweed, "spanim/caltrop", 0, isSpine: true, "reanim/Caltrop", 18.181818f),
			new ReanimationParams(ReanimationType.Spikerock, "spanim/spikerock", 0, isSpine: true, "reanim/SpikeRock"),
			new ReanimationParams(ReanimationType.Threepeater, "reanim/ThreePeater", 0),
			new ReanimationParams(ReanimationType.Marigold, "spanim/marigold", 0, isSpine: true, "reanim/Marigold", 15f),
			new ReanimationParams(ReanimationType.Iceshroom, "spanim/iceshroom", 0, isSpine: true, "reanim/IceShroom"),
			new ReanimationParams(ReanimationType.ZombieFootball, "reanim/Zombie_football", 0),
			new ReanimationParams(ReanimationType.ZombieNewspaper, "reanim/Zombie_paper", 0),
			new ReanimationParams(ReanimationType.ZombieZamboni, "reanim/Zombie_zamboni", 0),
			new ReanimationParams(ReanimationType.Splash, "reanim/splash", 0),
			new ReanimationParams(ReanimationType.Jalapeno, "reanim/Jalapeno", 0),
			new ReanimationParams(ReanimationType.JalapenoFire, "reanim/fire", 0),
			new ReanimationParams(ReanimationType.CoinSilver, "reanim/Coin_silver", 0),
			new ReanimationParams(ReanimationType.ZombieCharred, "reanim/Zombie_charred", 0),
			new ReanimationParams(ReanimationType.ZombieCharredImp, "reanim/Zombie_charred_imp", 0),
			new ReanimationParams(ReanimationType.ZombieCharredDigger, "reanim/Zombie_charred_digger", 0),
			new ReanimationParams(ReanimationType.ZombieCharredZamboni, "reanim/Zombie_charred_zamboni", 0),
			new ReanimationParams(ReanimationType.ZombieCharredCatapult, "reanim/Zombie_charred_catapult", 0),
			new ReanimationParams(ReanimationType.ZombieCharredGargantuar, "reanim/Zombie_charred_gargantuar", 0),
			new ReanimationParams(ReanimationType.Scrareyshroom, "spanim/scaredyshroom", 0, isSpine: true, "reanim/ScaredyShroom", 8.888889f),
			new ReanimationParams(ReanimationType.Pumpkin, "spanim/pumpkin_body", 0, isSpine: true, "reanim/Pumpkin"),
			new ReanimationParams(ReanimationType.Plantern, "spanim/plantern", 0, isSpine: true, "reanim/Plantern"),
			new ReanimationParams(ReanimationType.Torchwood, "reanim/Torchwood", 0),
			new ReanimationParams(ReanimationType.Splitpea, "spanim/splitpea", 0, isSpine: true, "reanim/SplitPea"),
			new ReanimationParams(ReanimationType.Seashroom, "spanim/seashroom", 0, isSpine: true, "reanim/SeaShroom"),
			new ReanimationParams(ReanimationType.Blover, "spanim/blover", 0, isSpine: true, "reanim/Blover"),
			new ReanimationParams(ReanimationType.FlowerPot, "reanim/Pot", 0),
			new ReanimationParams(ReanimationType.Cactus, "reanim/Cactus", 0),
			new ReanimationParams(ReanimationType.Disco, "reanim/Zombie_disco", 0),
			new ReanimationParams(ReanimationType.Tanglekelp, "reanim/Tanglekelp", 0),
			new ReanimationParams(ReanimationType.Starfruit, "reanim/Starfruit", 0),
			new ReanimationParams(ReanimationType.Polevaulter, "reanim/Zombie_polevaulter", 0),
			new ReanimationParams(ReanimationType.Balloon, "reanim/Zombie_balloon", 0),
			new ReanimationParams(ReanimationType.Gargantuar, "reanim/Zombie_gargantuar", 0),
			new ReanimationParams(ReanimationType.Imp, "reanim/Zombie_imp", 0),
			new ReanimationParams(ReanimationType.Digger, "reanim/Zombie_digger", 0),
			new ReanimationParams(ReanimationType.DiggerDirt, "reanim/Digger_rising_dirt", 0),
			new ReanimationParams(ReanimationType.ZombieDolphinrider, "reanim/Zombie_dolphinrider", 0),
			new ReanimationParams(ReanimationType.Pogo, "reanim/Zombie_pogo", 0),
			new ReanimationParams(ReanimationType.BackupDancer, "reanim/Zombie_backup", 0),
			new ReanimationParams(ReanimationType.Bobsled, "reanim/Zombie_bobsled", 0),
			new ReanimationParams(ReanimationType.Jackinthebox, "reanim/Zombie_jackbox", 0),
			new ReanimationParams(ReanimationType.Snorkel, "reanim/Zombie_snorkle", 0),
			new ReanimationParams(ReanimationType.Bungee, "reanim/Zombie_bungi", 0),
			new ReanimationParams(ReanimationType.Catapult, "reanim/Zombie_catapult", 0),
			new ReanimationParams(ReanimationType.Ladder, "reanim/Zombie_ladder", 0),
			new ReanimationParams(ReanimationType.Puff, "reanim/Puff", 0),
			new ReanimationParams(ReanimationType.Sleeping, "reanim/Z", 0),
			new ReanimationParams(ReanimationType.GraveBuster, "spanim/gravebuster", 0, isSpine: true, "reanim/Gravebuster"),
			new ReanimationParams(ReanimationType.ZombiesWon, "reanim/ZombiesWon", 1),
			new ReanimationParams(ReanimationType.Magnetshroom, "reanim/Magnetshroom", 0),
			new ReanimationParams(ReanimationType.Boss, "reanim/Zombie_boss", 1),
			new ReanimationParams(ReanimationType.Cabbagepult, "reanim/Cabbagepult", 0),
			new ReanimationParams(ReanimationType.Kernelpult, "spanim/cornpult", 0, isSpine: true, "reanim/Cornpult"),
			new ReanimationParams(ReanimationType.Melonpult, "reanim/Melonpult", 0),
			new ReanimationParams(ReanimationType.Coffeebean, "reanim/Coffeebean", 1),
			new ReanimationParams(ReanimationType.Umbrellaleaf, "spanim/umbrellaleaf", 0, isSpine: true, "reanim/Umbrellaleaf"),
			new ReanimationParams(ReanimationType.Gatlingpea, "spanim/gatlingpea", 0, isSpine: true, "reanim/GatlingPea"),
			new ReanimationParams(ReanimationType.Cattail, "spanim/cattail", 0, isSpine: true, "reanim/Cattail"),
			new ReanimationParams(ReanimationType.Gloomshroom, "reanim/GloomShroom", 0),
			new ReanimationParams(ReanimationType.BossIceball, "reanim/Zombie_boss_iceball", 1),
			new ReanimationParams(ReanimationType.BossFireball, "reanim/Zombie_boss_fireball", 1),
			new ReanimationParams(ReanimationType.Cobcannon, "reanim/CobCannon", 0),
			new ReanimationParams(ReanimationType.Garlic, "spanim/garlic", 0, isSpine: true, "reanim/Garlic"),
			new ReanimationParams(ReanimationType.GoldMagnet, "reanim/GoldMagnet", 0),
			new ReanimationParams(ReanimationType.WinterMelon, "reanim/WinterMelon", 0),
			new ReanimationParams(ReanimationType.TwinSunflower, "reanim/TwinSunflower", 0),
			new ReanimationParams(ReanimationType.PoolCleaner, "reanim/PoolCleaner", 0),
			new ReanimationParams(ReanimationType.RoofCleaner, "reanim/RoofCleaner", 0),
			new ReanimationParams(ReanimationType.FirePea, "reanim/FirePea", 0),
			new ReanimationParams(ReanimationType.Imitater, "spanim/imitater", 0, isSpine: true, "reanim/Imitater"),
			new ReanimationParams(ReanimationType.Yeti, "reanim/Zombie_yeti", 0),
			new ReanimationParams(ReanimationType.BossDriver, "reanim/Zombie_Boss_driver", 0),
			new ReanimationParams(ReanimationType.LawnMoweredZombie, "reanim/LawnMoweredZombie", 0),
			new ReanimationParams(ReanimationType.CrazyDave, "reanim/CrazyDave", 1),
			new ReanimationParams(ReanimationType.TextFadeOn, "reanim/TextFadeOn", 0),
			new ReanimationParams(ReanimationType.Hammer, "reanim/Hammer", 0),
			new ReanimationParams(ReanimationType.SlotMachineHandle, "reanim/SlotMachine", 0),
			new ReanimationParams(ReanimationType.SelectorScreen, "reanim/SelectorScreen", 3),
			new ReanimationParams(ReanimationType.PortalCircle, "reanim/Portal_Circle", 0),
			new ReanimationParams(ReanimationType.PortalSquare, "reanim/Portal_Square", 0),
			new ReanimationParams(ReanimationType.ZengardenSprout, "reanim/ZenGarden_sprout", 0),
			new ReanimationParams(ReanimationType.ZengardenWateringcan, "reanim/ZenGarden_wateringcan", 1),
			new ReanimationParams(ReanimationType.ZengardenFertilizer, "reanim/ZenGarden_fertilizer", 1),
			new ReanimationParams(ReanimationType.ZengardenBugspray, "reanim/ZenGarden_bugspray", 1),
			new ReanimationParams(ReanimationType.ZengardenPhonograph, "reanim/ZenGarden_phonograph", 1),
			new ReanimationParams(ReanimationType.Diamond, "reanim/Diamond", 0),
			new ReanimationParams(ReanimationType.Stinky, "reanim/Stinky", 0),
			new ReanimationParams(ReanimationType.Rake, "reanim/Rake", 0),
			new ReanimationParams(ReanimationType.RainCircle, "reanim/Rain_circle", 0),
			new ReanimationParams(ReanimationType.RainSplash, "reanim/Rain_splash", 0),
			new ReanimationParams(ReanimationType.ZombieSurprise, "reanim/Zombie_surprise", 0),
			new ReanimationParams(ReanimationType.CoinGold, "reanim/Coin_gold", 0),
			new ReanimationParams(ReanimationType.ZombieFlagpole, "reanim/Zombie_flagpole"),
			new ReanimationParams(ReanimationType.Woodsign, "reanim/woodsign"),
			new ReanimationParams(ReanimationType.Astronaut, "reanim/astronaut"),
			new ReanimationParams(ReanimationType.ZombieRobotTitan, "reanim/Zombie_machinegargantuar", 0),
			new ReanimationParams(ReanimationType.ZombieMonk, "reanim/Zombie_monk", 0),
			new ReanimationParams(ReanimationType.ZombieFootballPremium, "reanim/Zombie_football_premium", 0),
			new ReanimationParams(ReanimationType.Bushes3, "reanim/bushes3", 0),
			new ReanimationParams(ReanimationType.Bushes4, "reanim/bushes4", 0),
			new ReanimationParams(ReanimationType.Bushes5, "reanim/bushes5", 0),
			new ReanimationParams(ReanimationType.NightBushes3, "reanim/night_bushes3", 0),
			new ReanimationParams(ReanimationType.NightBushes4, "reanim/night_bushes4", 0),
			new ReanimationParams(ReanimationType.NightBushes5, "reanim/night_bushes5", 0),
			new ReanimationParams(ReanimationType.ZombieNinja, "reanim/Zombie_ninja", 0),
			new ReanimationParams(ReanimationType.TreeFood, "reanim/treefood", 0),
			new ReanimationParams(ReanimationType.TreeOfWisdom, "reanim/treeofwisdom", 0),
			new ReanimationParams(ReanimationType.TreeOfWisdomCloud, "reanim/treeofwisdomclouds", 0),
			new ReanimationParams(ReanimationType.SuperChomper, "reanim/SuperChomper", 0),
			new ReanimationParams(ReanimationType.PickledPepper, "reanim/PickledPepper", 0),
			new ReanimationParams(ReanimationType.FireShroom, "reanim/FireShroom", 0),
			new ReanimationParams(ReanimationType.Agave, "spanim/agave", 0, isSpine: true, "reanim/Agave"),
			new ReanimationParams(ReanimationType.AgaveParticle, "reanim/AgaveParticle", 0),
			new ReanimationParams(ReanimationType.AgaveFullattack, "reanim/AgaveFullattack", 0),
			new ReanimationParams(ReanimationType.ZengardenWateringcanDiamond, "reanim/ZenGarden_wateringcan_diamond", 1),
			new ReanimationParams(ReanimationType.PumpkinUpper, "spanim/pumpkin_upper", 0, isSpine: true, null, 8f),
			new ReanimationParams(ReanimationType.PumpkinLower, "spanim/pumpkin_lower", 0, isSpine: true, null, 8f),
			new ReanimationParams(ReanimationType.Endoflame, "spanim/endoflame", 0, isSpine: true, null, 12.8f),
			new ReanimationParams(ReanimationType.EndoflameSpike, "spanim/endoflame_spike", 0, isSpine: true),
			new ReanimationParams(ReanimationType.EndoflameFullattack, "reanim/EndoflameFullattack"),
			new ReanimationParams(ReanimationType.SquashWater, "spanim/squash2", 0, isSpine: true),
			new ReanimationParams(ReanimationType.ZombieTalisman, "reanim/Zombie_talisman", 0),
			new ReanimationParams(ReanimationType.Talisman, "reanim/Talisman", 0),
			new ReanimationParams(ReanimationType.Zorrose, "spanim/zorrose", 0, isSpine: true),
			new ReanimationParams(ReanimationType.ZombiePropeller, "reanim/Zombie_propeller", 0),
			new ReanimationParams(ReanimationType.HypnoCattail, "spanim/hypno_cattail", 0, isSpine: true),
			new ReanimationParams(ReanimationType.PeashooterOriginal, "reanim/Orig_PeaShooterSingle", 0),
			new ReanimationParams(ReanimationType.WallnutOriginal, "reanim/Orig_Wallnut", 0),
			new ReanimationParams(ReanimationType.LilypadOriginal, "reanim/Orig_LilyPad", 0),
			new ReanimationParams(ReanimationType.SunflowerOriginal, "reanim/Orig_SunFlower", 0),
			new ReanimationParams(ReanimationType.CherrybombOriginal, "reanim/Orig_CherryBomb", 0),
			new ReanimationParams(ReanimationType.SquashOriginal, "reanim/Orig_Squash", 0),
			new ReanimationParams(ReanimationType.DoomshroomOriginal, "reanim/Orig_DoomShroom", 0),
			new ReanimationParams(ReanimationType.SnowpeaOriginal, "reanim/Orig_SnowPea", 0),
			new ReanimationParams(ReanimationType.RepeaterOriginal, "reanim/Orig_PeaShooter", 0),
			new ReanimationParams(ReanimationType.SunshroomOriginal, "reanim/Orig_SunShroom", 0),
			new ReanimationParams(ReanimationType.TallnutOriginal, "reanim/Orig_Tallnut", 0),
			new ReanimationParams(ReanimationType.FumeshroomOriginal, "reanim/Orig_FumeShroom", 0),
			new ReanimationParams(ReanimationType.PuffshroomOriginal, "reanim/Orig_PuffShroom", 0),
			new ReanimationParams(ReanimationType.HypnoshroomOriginal, "reanim/Orig_HypnoShroom", 0),
			new ReanimationParams(ReanimationType.ChomperOriginal, "reanim/Orig_Chomper", 0),
			new ReanimationParams(ReanimationType.PotatomineOriginal, "reanim/Orig_PotatoMine", 0),
			new ReanimationParams(ReanimationType.SpikeweedOriginal, "reanim/Orig_Caltrop", 0),
			new ReanimationParams(ReanimationType.SpikerockOriginal, "reanim/Orig_SpikeRock", 0),
			new ReanimationParams(ReanimationType.ThreepeaterOriginal, "reanim/Orig_ThreePeater", 0),
			new ReanimationParams(ReanimationType.MarigoldOriginal, "reanim/Orig_Marigold", 0),
			new ReanimationParams(ReanimationType.IceshroomOriginal, "reanim/Orig_IceShroom", 0),
			new ReanimationParams(ReanimationType.JalapenoOriginal, "reanim/Orig_Jalapeno", 0),
			new ReanimationParams(ReanimationType.ScrareyshroomOriginal, "reanim/Orig_ScaredyShroom", 0),
			new ReanimationParams(ReanimationType.PumpkinOriginal, "reanim/Orig_Pumpkin", 0),
			new ReanimationParams(ReanimationType.PlanternOriginal, "reanim/Orig_Plantern", 0),
			new ReanimationParams(ReanimationType.TorchwoodOriginal, "reanim/Orig_Torchwood", 0),
			new ReanimationParams(ReanimationType.SplitpeaOriginal, "reanim/Orig_SplitPea", 0),
			new ReanimationParams(ReanimationType.SeashroomOriginal, "reanim/Orig_SeaShroom", 0),
			new ReanimationParams(ReanimationType.BloverOriginal, "reanim/Orig_Blover", 0),
			new ReanimationParams(ReanimationType.FlowerPotOriginal, "reanim/Orig_Pot", 0),
			new ReanimationParams(ReanimationType.CactusOriginal, "reanim/Orig_Cactus", 0),
			new ReanimationParams(ReanimationType.TanglekelpOriginal, "reanim/Orig_Tanglekelp", 0),
			new ReanimationParams(ReanimationType.StarfruitOriginal, "reanim/Orig_Starfruit", 0),
			new ReanimationParams(ReanimationType.GraveBusterOriginal, "reanim/Orig_Gravebuster", 0),
			new ReanimationParams(ReanimationType.MagnetshroomOriginal, "reanim/Orig_Magnetshroom", 0),
			new ReanimationParams(ReanimationType.CabbagepultOriginal, "reanim/Orig_Cabbagepult", 0),
			new ReanimationParams(ReanimationType.KernelpultOriginal, "reanim/Orig_Cornpult", 0),
			new ReanimationParams(ReanimationType.MelonpultOriginal, "reanim/Orig_Melonpult", 0),
			new ReanimationParams(ReanimationType.CoffeebeanOriginal, "reanim/Orig_Coffeebean", 0),
			new ReanimationParams(ReanimationType.UmbrellaleafOriginal, "reanim/Orig_Umbrellaleaf", 0),
			new ReanimationParams(ReanimationType.GatlingpeaOriginal, "reanim/Orig_GatlingPea", 0),
			new ReanimationParams(ReanimationType.CattailOriginal, "reanim/Orig_Cattail", 0),
			new ReanimationParams(ReanimationType.GloomshroomOriginal, "reanim/Orig_GloomShroom", 0),
			new ReanimationParams(ReanimationType.CobcannonOriginal, "reanim/Orig_CobCannon", 0),
			new ReanimationParams(ReanimationType.GarlicOriginal, "reanim/Orig_Garlic", 0),
			new ReanimationParams(ReanimationType.GoldMagnetOriginal, "reanim/Orig_GoldMagnet", 0),
			new ReanimationParams(ReanimationType.WinterMelonOriginal, "reanim/Orig_WinterMelon", 0),
			new ReanimationParams(ReanimationType.TwinSunflowerOriginal, "reanim/Orig_TwinSunflower", 0),
			new ReanimationParams(ReanimationType.ImitaterOriginal, "reanim/Orig_Imitater", 0)
		};
		gLawnTrailArray = new TrailParams[4]
		{
			new TrailParams(TrailType.Ice, "particles/IceTrail"),
			new TrailParams(TrailType.Cattail, "particles/CattailTrail"),
			new TrailParams(TrailType.Endoflame, "particles/EndoflameTrail"),
			new TrailParams(TrailType.HypnoCattail, "particles/HypnoCattailTrail")
		};
		gLawnStringFormats = new TodStringListFormat[12]
		{
			new TodStringListFormat("NORMAL", null, new SexyColor(40, 50, 90), 0, 0u),
			new TodStringListFormat("FLAVOR", null, new SexyColor(143, 67, 27), 0, 1u),
			new TodStringListFormat("KEYWORD", null, new SexyColor(143, 67, 27), 0, 0u),
			new TodStringListFormat("NOCTURNAL", null, new SexyColor(136, 50, 170), 0, 0u),
			new TodStringListFormat("AQUATIC", null, new SexyColor(10, 151, 216), 0, 0u),
			new TodStringListFormat("STAT", null, new SexyColor(204, 36, 29), 0, 0u),
			new TodStringListFormat("METAL", null, new SexyColor(204, 36, 29), 0, 2u),
			new TodStringListFormat("KEYMETAL", null, new SexyColor(143, 67, 27), 0, 2u),
			new TodStringListFormat("SHORTLINE", null, new SexyColor(0, 0, 0, 0), 0, 0u),
			new TodStringListFormat("EXTRASHORTLINE", null, new SexyColor(0, 0, 0, 0), -14, 0u),
			new TodStringListFormat("CREDITS1", null, new SexyColor(0, 0, 0, 0), 3, 0u),
			new TodStringListFormat("CREDITS2", null, new SexyColor(0, 0, 0, 0), 2, 0u)
		};
		gLawnStringFormatCount = gLawnStringFormats.Length;
	}

	public static int GetRectOverlap(TRect rect1, TRect rect2)
	{
		int num;
		int mX2;
		int num2;
		if (rect1.mX < rect2.mX)
		{
			int mX = rect1.mX;
			num = rect1.mX + rect1.mWidth;
			mX2 = rect2.mX;
			num2 = rect2.mX + rect2.mWidth;
		}
		else
		{
			int mX3 = rect2.mX;
			num = rect2.mX + rect2.mWidth;
			mX2 = rect1.mX;
			num2 = rect1.mX + rect1.mWidth;
		}
		if (num <= mX2)
		{
			return num - mX2;
		}
		if (num <= num2)
		{
			return num - mX2;
		}
		return num2 - mX2;
	}

	public static bool GetCircleRectOverlap(int theCircleX, int theCircleY, int theRadius, TRect theRect)
	{
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		int num2 = 0;
		if (theCircleX < theRect.mX)
		{
			flag = true;
			num = theRect.mX - theCircleX;
		}
		else if (theCircleX > theRect.mX + theRect.mWidth)
		{
			flag = true;
			num = theCircleX - theRect.mX - theRect.mWidth;
		}
		if (theCircleY < theRect.mY)
		{
			flag2 = true;
			num2 = theRect.mY - theCircleY;
		}
		else if (theCircleY > theRect.mY + theRect.mHeight)
		{
			flag2 = true;
			num2 = theCircleY - theRect.mY - theRect.mHeight;
		}
		if (!flag2 && !flag)
		{
			return true;
		}
		if (flag & flag2)
		{
			int num3 = num * num + num2 * num2;
			return num3 <= theRadius * theRadius;
		}
		if (flag)
		{
			return num <= theRadius;
		}
		return num2 <= theRadius;
	}

	static GameConstants()
	{
		gGameButtonColors = new SexyColor[6]
		{
			new SexyColor(0, 0, 0),
			new SexyColor(0, 0, 0),
			new SexyColor(0, 0, 0),
			new SexyColor(255, 255, 255),
			new SexyColor(132, 132, 132),
			new SexyColor(212, 212, 212)
		};
		gZombieWaves = new int[50]
		{
			4, 6, 8, 10, 8, 10, 20, 10, 20, 20,
			10, 20, 10, 20, 10, 10, 20, 10, 20, 20,
			10, 20, 20, 30, 20, 20, 30, 20, 30, 30,
			10, 20, 10, 20, 20, 10, 20, 10, 20, 20,
			10, 20, 20, 30, 20, 20, 30, 20, 30, 30
		};
		gCoverInfos = new CoverInfo[6]
		{
			new CoverInfo((float)(-Constants.BoardOffsetChange - Constants.BoardPosChange) + 712f, -18f, 1f),
			new CoverInfo((float)(-Constants.BoardOffsetChange - Constants.BoardPosChange) + 724f, 110f, 1f),
			new CoverInfo((float)(-Constants.BoardOffsetChange - Constants.BoardPosChange) + 728f, 198f, 1f),
			new CoverInfo((float)(-Constants.BoardOffsetChange - Constants.BoardPosChange) + 734f, 320f, 1f),
			new CoverInfo((float)(-Constants.BoardOffsetChange - Constants.BoardPosChange) + 740f, 401f, 1f),
			new CoverInfo((float)(-Constants.BoardOffsetChange - Constants.BoardPosChange) + 740f, 450f, 1f)
		};
		gZombieAllowedLevels = new ZombieAllowedLevels[40]
		{
			new ZombieAllowedLevels(ZombieType.Normal, new int[50]
			{
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Flag, new int[50]
			{
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.TrafficCone, new int[50]
			{
				0, 0, 1, 1, 1, 1, 1, 1, 1, 1,
				0, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Polevaulter, new int[50]
			{
				0, 0, 0, 0, 0, 1, 1, 0, 1, 1,
				0, 0, 0, 1, 1, 0, 0, 0, 0, 0,
				0, 0, 0, 1, 0, 0, 0, 0, 1, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 1, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.Pail, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
				0, 1, 0, 0, 1, 0, 0, 0, 0, 0,
				0, 1, 0, 1, 0, 0, 1, 0, 1, 1,
				0, 0, 0, 0, 0, 0, 1, 0, 1, 1,
				0, 1, 0, 0, 1, 0, 0, 0, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Newspaper, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				1, 1, 0, 0, 1, 0, 0, 0, 0, 0,
				0, 1, 0, 1, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.Door, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 1, 1, 0, 0, 1, 0, 1, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.Football, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 1, 1, 0, 0, 1,
				0, 1, 0, 0, 1, 0, 0, 0, 0, 0,
				0, 1, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 1, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.Dancer, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.BackupDancer, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.DuckyTube, new int[50]),
			new ZombieAllowedLevels(ZombieType.Snorkel, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 1, 1, 1, 0, 1, 0, 0, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.Zamboni, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 1, 1, 0, 1, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.Bobsled, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 1, 1, 0, 1, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.DolphinRider, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
				0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.JackInTheBox, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				1, 1, 0, 0, 0, 0, 1, 0, 0, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Balloon, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 1, 1, 0, 0, 0, 0, 1, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.Digger, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 1, 1, 0, 0, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.Pogo, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
				0, 0, 0, 1, 0, 0, 0, 0, 0, 0
			}),
			new ZombieAllowedLevels(ZombieType.Yeti, new int[50]),
			new ZombieAllowedLevels(ZombieType.Bungee, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				1, 1, 0, 0, 0, 0, 1, 0, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Ladder, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 1, 1, 1, 0, 1, 0, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Catapult, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 1, 1, 0, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Gargantuar, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 1, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Imp, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 1, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Boss, new int[50]),
			new ZombieAllowedLevels(ZombieType.PeaHead, new int[50]),
			new ZombieAllowedLevels(ZombieType.WallnutHead, new int[50]),
			new ZombieAllowedLevels(ZombieType.JalapenoHead, new int[50]),
			new ZombieAllowedLevels(ZombieType.GatlingHead, new int[50]),
			new ZombieAllowedLevels(ZombieType.SquashHead, new int[50]),
			new ZombieAllowedLevels(ZombieType.TallnutHead, new int[50]),
			new ZombieAllowedLevels(ZombieType.RedeyeGargantuar, new int[50]),
			new ZombieAllowedLevels(ZombieType.RobotTitan, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
				0, 1, 0, 1, 0, 1, 1, 1, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.RedeyeRobotTitan, new int[50]),
			new ZombieAllowedLevels(ZombieType.Monk, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 1, 0, 1,
				0, 0, 0, 1, 0, 1, 0, 1, 1, 1,
				0, 1, 1, 0, 0, 0, 1, 0, 1, 1,
				0, 0, 0, 0, 0, 1, 0, 1, 0, 0,
				0, 0, 1, 0, 0, 1, 0, 1, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.FootballPremium, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 1, 0, 1, 1,
				0, 0, 0, 1, 1, 1, 0, 0, 1, 1,
				0, 0, 0, 0, 0, 0, 0, 1, 1, 0,
				0, 0, 0, 1, 0, 0, 1, 1, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Ninja, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
				0, 1, 0, 1, 0, 1, 1, 1, 1, 0,
				0, 1, 0, 0, 0, 1, 0, 0, 0, 1
			}),
			new ZombieAllowedLevels(ZombieType.Talisman, new int[50]
			{
				1, 1, 0, 1, 0, 1, 1, 0, 1, 1,
				0, 0, 0, 1, 0, 0, 0, 1, 1, 1,
				1, 0, 1, 1, 0, 0, 0, 1, 0, 1,
				0, 0, 0, 1, 0, 0, 0, 0, 1, 0,
				1, 1, 0, 0, 1, 0, 0, 0, 1, 1
			}),
			new ZombieAllowedLevels(ZombieType.Propeller, new int[50]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 1, 0, 0, 1, 0, 1, 1, 1,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			})
		};
		gPlantDefs = new PlantDefinition[(int)SeedType.SeedTypeCount]
		{
			new PlantDefinition(SeedType.Peashooter, null, ReanimationType.Peashooter, 0, 100, 750, PlantSubClass.Shooter, 150, "PEASHOOTER"),
			new PlantDefinition(SeedType.Sunflower, null, ReanimationType.Sunflower, 1, 50, 750, PlantSubClass.Normal, 2500, "SUNFLOWER"),
			new PlantDefinition(SeedType.Cherrybomb, null, ReanimationType.Cherrybomb, 3, 150, 5000, PlantSubClass.Normal, 0, "CHERRY_BOMB"),
			new PlantDefinition(SeedType.Wallnut, null, ReanimationType.Wallnut, 2, 50, 3000, PlantSubClass.Normal, 0, "WALL_NUT"),
			new PlantDefinition(SeedType.Potatomine, null, ReanimationType.Potatomine, 37, 25, 3000, PlantSubClass.Normal, 0, "POTATO_MINE"),
			new PlantDefinition(SeedType.Snowpea, null, ReanimationType.Snowpea, 4, 175, 750, PlantSubClass.Shooter, 150, "SNOW_PEA"),
			new PlantDefinition(SeedType.Chomper, null, ReanimationType.Chomper, 31, 150, 750, PlantSubClass.Normal, 0, "CHOMPER"),
			new PlantDefinition(SeedType.Repeater, null, ReanimationType.Repeater, 5, 200, 750, PlantSubClass.Shooter, 150, "REPEATER"),
			new PlantDefinition(SeedType.Puffshroom, null, ReanimationType.Puffshroom, 6, 0, 750, PlantSubClass.Shooter, 150, "PUFF_SHROOM"),
			new PlantDefinition(SeedType.Sunshroom, null, ReanimationType.Sunshroom, 7, 25, 750, PlantSubClass.Normal, 2500, "SUN_SHROOM"),
			new PlantDefinition(SeedType.Fumeshroom, null, ReanimationType.Fumeshroom, 9, 75, 750, PlantSubClass.Shooter, 150, "FUME_SHROOM"),
			new PlantDefinition(SeedType.Gravebuster, null, ReanimationType.GraveBuster, 40, 75, 750, PlantSubClass.Normal, 0, "GRAVE_BUSTER"),
			new PlantDefinition(SeedType.Hypnoshroom, null, ReanimationType.Hypnoshroom, 10, 75, 3000, PlantSubClass.Normal, 0, "HYPNO_SHROOM"),
			new PlantDefinition(SeedType.Scaredyshroom, null, ReanimationType.Scrareyshroom, 33, 25, 750, PlantSubClass.Shooter, 150, "SCAREDY_SHROOM"),
			new PlantDefinition(SeedType.Iceshroom, null, ReanimationType.Iceshroom, 36, 75, 5000, PlantSubClass.Normal, 0, "ICE_SHROOM"),
			new PlantDefinition(SeedType.Doomshroom, null, ReanimationType.Doomshroom, 20, 125, 5000, PlantSubClass.Normal, 0, "DOOM_SHROOM"),
			new PlantDefinition(SeedType.Lilypad, null, ReanimationType.Lilypad, 19, 25, 750, PlantSubClass.Normal, 0, "LILY_PAD"),
			new PlantDefinition(SeedType.Squash, null, ReanimationType.Squash, 21, 50, 3000, PlantSubClass.Normal, 0, "SQUASH"),
			new PlantDefinition(SeedType.Threepeater, null, ReanimationType.Threepeater, 12, 325, 750, PlantSubClass.Shooter, 150, "THREEPEATER"),
			new PlantDefinition(SeedType.Tanglekelp, null, ReanimationType.Tanglekelp, 17, 25, 3000, PlantSubClass.Normal, 0, "TANGLE_KELP"),
			new PlantDefinition(SeedType.Jalapeno, null, ReanimationType.Jalapeno, 11, 125, 5000, PlantSubClass.Normal, 0, "JALAPENO"),
			new PlantDefinition(SeedType.Spikeweed, null, ReanimationType.Spikeweed, 22, 100, 750, PlantSubClass.Normal, 0, "SPIKEWEED"),
			new PlantDefinition(SeedType.Torchwood, null, ReanimationType.Torchwood, 29, 175, 750, PlantSubClass.Normal, 0, "TORCHWOOD"),
			new PlantDefinition(SeedType.Tallnut, null, ReanimationType.Tallnut, 28, 125, 3000, PlantSubClass.Normal, 0, "TALL_NUT"),
			new PlantDefinition(SeedType.Seashroom, null, ReanimationType.Seashroom, 39, 0, 750, PlantSubClass.Shooter, 150, "SEA_SHROOM"),
			new PlantDefinition(SeedType.Plantern, null, ReanimationType.Plantern, 38, 25, 3000, PlantSubClass.Normal, 2500, "PLANTERN"),
			new PlantDefinition(SeedType.Cactus, null, ReanimationType.Cactus, 15, 125, 750, PlantSubClass.Shooter, 150, "CACTUS"),
			new PlantDefinition(SeedType.Blover, null, ReanimationType.Blover, 18, 100, 750, PlantSubClass.Normal, 0, "BLOVER"),
			new PlantDefinition(SeedType.Splitpea, null, ReanimationType.Splitpea, 32, 125, 750, PlantSubClass.Shooter, 150, "SPLIT_PEA"),
			new PlantDefinition(SeedType.Starfruit, null, ReanimationType.Starfruit, 30, 125, 750, PlantSubClass.Shooter, 150, "STARFRUIT"),
			new PlantDefinition(SeedType.Pumpkinshell, null, ReanimationType.Pumpkin, 25, 125, 3000, PlantSubClass.Normal, 0, "PUMPKIN"),
			new PlantDefinition(SeedType.Magnetshroom, null, ReanimationType.Magnetshroom, 35, 100, 750, PlantSubClass.Normal, 0, "MAGNET_SHROOM"),
			new PlantDefinition(SeedType.Cabbagepult, null, ReanimationType.Cabbagepult, 13, 100, 750, PlantSubClass.Shooter, 300, "CABBAGE_PULT"),
			new PlantDefinition(SeedType.Flowerpot, null, ReanimationType.FlowerPot, 33, 25, 750, PlantSubClass.Normal, 0, "FLOWER_POT"),
			new PlantDefinition(SeedType.Kernelpult, null, ReanimationType.Kernelpult, 13, 100, 750, PlantSubClass.Shooter, 300, "KERNEL_PULT"),
			new PlantDefinition(SeedType.InstantCoffee, null, ReanimationType.Coffeebean, 33, 75, 750, PlantSubClass.Normal, 0, "COFFEE_BEAN"),
			new PlantDefinition(SeedType.Garlic, null, ReanimationType.Garlic, 8, 50, 750, PlantSubClass.Normal, 0, "GARLIC"),
			new PlantDefinition(SeedType.Umbrella, null, ReanimationType.Umbrellaleaf, 23, 100, 750, PlantSubClass.Normal, 0, "UMBRELLA_LEAF"),
			new PlantDefinition(SeedType.Marigold, null, ReanimationType.Marigold, 24, 50, 3000, PlantSubClass.Normal, 2500, "MARIGOLD"),
			new PlantDefinition(SeedType.Melonpult, null, ReanimationType.Melonpult, 14, 300, 750, PlantSubClass.Shooter, 300, "MELON_PULT"),
			new PlantDefinition(SeedType.Gatlingpea, null, ReanimationType.Gatlingpea, 5, 250, 5000, PlantSubClass.Shooter, 150, "GATLING_PEA"),
			new PlantDefinition(SeedType.Twinsunflower, null, ReanimationType.TwinSunflower, 1, 150, 5000, PlantSubClass.Normal, 2500, "TWIN_SUNFLOWER"),
			new PlantDefinition(SeedType.Gloomshroom, null, ReanimationType.Gloomshroom, 27, 150, 5000, PlantSubClass.Shooter, 200, "GLOOM_SHROOM"),
			new PlantDefinition(SeedType.Cattail, null, ReanimationType.Cattail, 27, 200, 4700, PlantSubClass.Shooter, 150, "CATTAIL"),
			new PlantDefinition(SeedType.Wintermelon, null, ReanimationType.WinterMelon, 27, 200, 5000, PlantSubClass.Shooter, 300, "WINTER_MELON"),
			new PlantDefinition(SeedType.GoldMagnet, null, ReanimationType.GoldMagnet, 27, 50, 5000, PlantSubClass.Normal, 0, "GOLD_MAGNET"),
			new PlantDefinition(SeedType.Spikerock, null, ReanimationType.Spikerock, 27, 125, 5000, PlantSubClass.Normal, 0, "SPIKEROCK"),
			new PlantDefinition(SeedType.Cobcannon, null, ReanimationType.Cobcannon, 16, 500, 5000, PlantSubClass.Normal, 600, "COB_CANNON"),
			new PlantDefinition(SeedType.SuperChomper, null, ReanimationType.SuperChomper, 16, 125, 5000, PlantSubClass.Normal, 0, "SUPER_CHOMPER"),
			new PlantDefinition(SeedType.PickledPepper, null, ReanimationType.PickledPepper, 16, 150, 5000, PlantSubClass.Normal, 0, "PICKLED_PEPPER"),
			new PlantDefinition(SeedType.FireShroom, null, ReanimationType.FireShroom, 16, 50, 3000, PlantSubClass.Normal, 0, "FIRE_SHROOM"),
			new PlantDefinition(SeedType.Agave, null, ReanimationType.Agave, 16, 300, 750, PlantSubClass.Normal, 100, "AGAVE"),
			new PlantDefinition(SeedType.Endoflame, null, ReanimationType.Endoflame, 16, 225, 5000, PlantSubClass.Shooter, 150, "ENDOFLAME"),
			new PlantDefinition(SeedType.Imitater, null, ReanimationType.Imitater, 33, 0, 750, PlantSubClass.Normal, 0, "IMITATER"),
			new PlantDefinition(SeedType.ExplodeONut, null, ReanimationType.Wallnut, 2, 0, 3000, PlantSubClass.Normal, 0, "EXPLODE_O_NUT"),
			new PlantDefinition(SeedType.GiantWallnut, null, ReanimationType.Wallnut, 2, 0, 3000, PlantSubClass.Normal, 0, "GIANT_WALLNUT"),
			new PlantDefinition(SeedType.Sprout, null, ReanimationType.None, 33, 0, 3000, PlantSubClass.Normal, 0, "SPROUT"),
			new PlantDefinition(SeedType.Leftpeater, null, ReanimationType.Repeater, 5, 200, 750, PlantSubClass.Shooter, 150, "REPEATER"),
			new PlantDefinition(SeedType.ImitaterRandomPlant, null, ReanimationType.Imitater, 33, 100, 750, PlantSubClass.Normal, 0, "IMITATER"),
			new PlantDefinition(SeedType.ImitaterRandomZombie, null, ReanimationType.Imitater, 33, 0, 2250, PlantSubClass.Normal, 0, "IMITATER"),
			new PlantDefinition(SeedType.HypnoCattail, null, ReanimationType.HypnoCattail, 27, 275, 4700, PlantSubClass.Shooter, 150, "CATTAIL_HYPNO"),
			new PlantDefinition(SeedType.SunflowerPea, null, ReanimationType.Sunflower, 1, 150, 750, PlantSubClass.Normal, 2500, "FU_SUNFLOWER_PEASHOOTER"),
			new PlantDefinition(SeedType.PeaWallnut, null, ReanimationType.Wallnut, 0, 150, 750, PlantSubClass.Shooter, 150, "FU_PEASHOOTER_WALL_NUT"),
			new PlantDefinition(SeedType.SunflowerWallnut, null, ReanimationType.Sunflower, 1, 100, 750, PlantSubClass.Normal, 2500, "FU_SUNFLOWER_WALL_NUT"),
			new PlantDefinition(SeedType.IceWallnut, null, ReanimationType.Wallnut, 2, 50, 3000, PlantSubClass.Normal, 0, "FU_ICE_WALL_NUT")
		};
		NUM_BACKUP_DANCERS = 4;
		MAX_ZOMBIE_FOLLOWERS = 4;
		ZOMBIE_WAVE_DEBUG = -1;
		ZOMBIE_WAVE_CUTSCENE = -2;
		ZOMBIE_WAVE_UI = -3;
		ZOMBIE_WAVE_WINNER = -4;
		RENDER_GROUP_SHIELD = 1;
		RENDER_GROUP_ARMS = 2;
		RENDER_GROUP_OVER_SHIELD = 3;
		RENDER_GROUP_BOSS_BACK_LEG = 4;
		RENDER_GROUP_BOSS_FRONT_LEG = 5;
		RENDER_GROUP_BOSS_BACK_ARM = 6;
		RENDER_GROUP_BOSS_FIREBALL_ADDITIVE = 7;
		RENDER_GROUP_BOSS_FIREBALL_TOP = 8;
		CHILLED_SPEED_FACTOR = 0.4f;
		ZOMBIE_LIMP_SPEED_FACTOR = 2;
		BUNGEE_ZOMBIE_HEIGHT = 3000;
		DOG_WALKING_DISTANCE = 31;
		POGO_BOUNCE_TIME = 80;
		DOLPHIN_JUMP_TIME = 120;
		THOWN_ZOMBIE_GRAVITY = 0.05f;
		TICKS_BETWEEN_EATS = 4;
		CLIP_HEIGHT_OFF = -200f;
		CLIP_HEIGHT_LIMIT = -100f;
		ZOMBIE_MINDCONTROLLED_COLOR = new SexyColor(128, 64, 192);
		ZOMBIE_BLINK_RATE = 400;
		BOSS_FLASH_HEALTH_FRACTION = 10;
		ZOMBIE_WALK_IN_FRONT_DOOR_Y = 290f;
		BOBSLED_CRASH_TIME = 150;
		YUCKI_PAUSE_TIME = 70;
		YUCKI_SHORT_PAUSE_TIME = 20;
		YUCKI_HOLD_TIME = YUCKI_PAUSE_TIME + 100;
		YUCKI_WALK_TIME = YUCKI_HOLD_TIME + 100;
		BOSS_BALL_OFFSET_Y = -90f;
		gZombieDefs = new ZombieDefinition[40]
		{
			new ZombieDefinition(ZombieType.Normal, ReanimationType.Zombie, 1, 1, 1, 4000, "ZOMBIE"),
			new ZombieDefinition(ZombieType.Flag, ReanimationType.Zombie, 1, 1, 1, 0, "FLAG_ZOMBIE"),
			new ZombieDefinition(ZombieType.TrafficCone, ReanimationType.Zombie, 2, 3, 1, 4000, "CONEHEAD_ZOMBIE"),
			new ZombieDefinition(ZombieType.Polevaulter, ReanimationType.Polevaulter, 2, 6, 5, 2000, "POLE_VAULTING_ZOMBIE"),
			new ZombieDefinition(ZombieType.Pail, ReanimationType.Zombie, 4, 8, 1, 3000, "BUCKETHEAD_ZOMBIE"),
			new ZombieDefinition(ZombieType.Newspaper, ReanimationType.ZombieNewspaper, 2, 11, 1, 1000, "NEWSPAPER_ZOMBIE"),
			new ZombieDefinition(ZombieType.Door, ReanimationType.Zombie, 4, 13, 5, 3500, "SCREEN_DOOR_ZOMBIE"),
			new ZombieDefinition(ZombieType.Football, ReanimationType.ZombieFootball, 7, 16, 5, 2000, "FOOTBALL_ZOMBIE"),
			new ZombieDefinition(ZombieType.Dancer, ReanimationType.Disco, 5, 18, 5, 1000, "DANCING_ZOMBIE"),
			new ZombieDefinition(ZombieType.BackupDancer, ReanimationType.BackupDancer, 1, 18, 1, 0, "BACKUP_DANCER"),
			new ZombieDefinition(ZombieType.DuckyTube, ReanimationType.Zombie, 1, 21, 5, 0, "DUCKY_TUBE_ZOMBIE"),
			new ZombieDefinition(ZombieType.Snorkel, ReanimationType.Snorkel, 3, 23, 10, 2000, "SNORKEL_ZOMBIE"),
			new ZombieDefinition(ZombieType.Zamboni, ReanimationType.ZombieZamboni, 7, 26, 10, 2000, "ZOMBONI"),
			new ZombieDefinition(ZombieType.Bobsled, ReanimationType.Bobsled, 3, 26, 10, 2000, "ZOMBIE_BOBSLED_TEAM"),
			new ZombieDefinition(ZombieType.DolphinRider, ReanimationType.ZombieDolphinrider, 3, 28, 10, 1500, "DOLPHIN_RIDER_ZOMBIE"),
			new ZombieDefinition(ZombieType.JackInTheBox, ReanimationType.Jackinthebox, 3, 31, 10, 1000, "JACK_IN_THE_BOX_ZOMBIE"),
			new ZombieDefinition(ZombieType.Balloon, ReanimationType.Balloon, 2, 33, 10, 2000, "BALLOON_ZOMBIE"),
			new ZombieDefinition(ZombieType.Digger, ReanimationType.Digger, 4, 36, 10, 1000, "DIGGER_ZOMBIE"),
			new ZombieDefinition(ZombieType.Pogo, ReanimationType.Pogo, 4, 38, 10, 1000, "POGO_ZOMBIE"),
			new ZombieDefinition(ZombieType.Yeti, ReanimationType.Yeti, 4, 40, 1, 1, "ZOMBIE_YETI"),
			new ZombieDefinition(ZombieType.Bungee, ReanimationType.Bungee, 3, 41, 10, 1000, "BUNGEE_ZOMBIE"),
			new ZombieDefinition(ZombieType.Ladder, ReanimationType.Ladder, 4, 43, 10, 1000, "LADDER_ZOMBIE"),
			new ZombieDefinition(ZombieType.Catapult, ReanimationType.Catapult, 5, 46, 10, 1500, "CATAPULT_ZOMBIE"),
			new ZombieDefinition(ZombieType.Gargantuar, ReanimationType.Gargantuar, 10, 48, 15, 1500, "GARGANTUAR"),
			new ZombieDefinition(ZombieType.Imp, ReanimationType.Imp, 10, 48, 1, 0, "IMP"),
			new ZombieDefinition(ZombieType.Boss, ReanimationType.Boss, 10, 50, 1, 0, "BOSS"),
			new ZombieDefinition(ZombieType.PeaHead, ReanimationType.Zombie, 1, 99, 1, 4000, "ZOMBIE"),
			new ZombieDefinition(ZombieType.WallnutHead, ReanimationType.Zombie, 4, 99, 1, 3000, "ZOMBIE"),
			new ZombieDefinition(ZombieType.JalapenoHead, ReanimationType.Zombie, 3, 99, 10, 1000, "ZOMBIE"),
			new ZombieDefinition(ZombieType.GatlingHead, ReanimationType.Zombie, 3, 99, 10, 2000, "ZOMBIE"),
			new ZombieDefinition(ZombieType.SquashHead, ReanimationType.Zombie, 3, 99, 10, 2000, "ZOMBIE"),
			new ZombieDefinition(ZombieType.TallnutHead, ReanimationType.Zombie, 4, 99, 10, 2000, "ZOMBIE"),
			new ZombieDefinition(ZombieType.RedeyeGargantuar, ReanimationType.Gargantuar, 10, 48, 15, 6000, "REDEYED_GARGANTUAR"),
			new ZombieDefinition(ZombieType.RobotTitan, ReanimationType.ZombieRobotTitan, 10, 10, 15, 6000, "ROBOTTITAN"),
			new ZombieDefinition(ZombieType.RedeyeRobotTitan, ReanimationType.ZombieRobotTitan, 10, 99, 15, 6000, "REDEYED_ROBOTTITAN"),
			new ZombieDefinition(ZombieType.Monk, ReanimationType.ZombieMonk, 4, 8, 5, 3000, "ZOMBIE_MONK"),
			new ZombieDefinition(ZombieType.FootballPremium, ReanimationType.ZombieFootballPremium, 10, 17, 5, 2000, "ZOMBIE_FOOTBALL_PREMIUM"),
			new ZombieDefinition(ZombieType.Ninja, ReanimationType.ZombieNinja, 3, 28, 5, 3000, "ZOMBIE_NINJA"),
			new ZombieDefinition(ZombieType.Talisman, ReanimationType.ZombieTalisman, 2, 1, 10, 3000, "ZOMBIE_TALISMAN"),
			new ZombieDefinition(ZombieType.Propeller, ReanimationType.ZombiePropeller, 2, 33, 10, 3500, "PROPELLER_ZOMBIE")
		};
		gBossZombieList = new ZombieType[12]
		{
			ZombieType.TrafficCone,
			ZombieType.Pail,
			ZombieType.Football,
			ZombieType.Polevaulter,
			ZombieType.JackInTheBox,
			ZombieType.Ladder,
			ZombieType.Zamboni,
			ZombieType.Catapult,
			ZombieType.Pogo,
			ZombieType.Newspaper,
			ZombieType.Door,
			ZombieType.Gargantuar
		};
		gBossZombieListHard = new ZombieType[18]
		{
			ZombieType.TrafficCone,
			ZombieType.Pail,
			ZombieType.Football,
			ZombieType.Polevaulter,
			ZombieType.JackInTheBox,
			ZombieType.Ladder,
			ZombieType.Zamboni,
			ZombieType.Catapult,
			ZombieType.Pogo,
			ZombieType.Newspaper,
			ZombieType.Door,
			ZombieType.Talisman,
			ZombieType.Monk,
			ZombieType.Ninja,
			ZombieType.Propeller,
			ZombieType.FootballPremium,
			ZombieType.Gargantuar,
			ZombieType.RobotTitan
		};
		gBossZombieList2 = new ZombieType[16]
		{
			ZombieType.TrafficCone,
			ZombieType.Pail,
			ZombieType.Football,
			ZombieType.Polevaulter,
			ZombieType.JackInTheBox,
			ZombieType.Ladder,
			ZombieType.Zamboni,
			ZombieType.Catapult,
			ZombieType.Pogo,
			ZombieType.Newspaper,
			ZombieType.Door,
			ZombieType.Monk,
			ZombieType.FootballPremium,
			ZombieType.Ninja,
			ZombieType.Gargantuar,
			ZombieType.RobotTitan
		};
		gFinalBoss2RainingSeeds = new SeedType[5]
		{
			SeedType.Jalapeno,
			SeedType.PickledPepper,
			SeedType.Cherrybomb,
			SeedType.Squash,
			SeedType.Agave
		};
		TESTING_LOAD_BAR = false;
		BAR_FADE_TIME = 8;
		NUM_ALMANAC_REANIMS = 4;
		NUM_ALMANAC_SEEDS = 54;
		NUM_ALMANAC_ZOMBIES = AlmanacZombieDataContainer.mData.Length;
		NUM_ALMANAC_ZOMBIES_PERF_TEST = 400;
		BUTTON_SIZE = 40;
		BUTTONS_PER_ROW = 10;
		gFlowerCenter = new LawnFPoint[3]
		{
			new LawnFPoint(765f, 483f),
			new LawnFPoint(663f, 455f),
			new LawnFPoint(701f, 439f)
		};
		MUSIC_SLIDER_THRESHOLD = 0.01;
		gUserVersionTilJuly07 = 2;
		gUserVersionTilFeb08 = 3;
		gUserVersionTilApril08 = 4;
		gUserVersionTilMay08 = 5;
		gUserVersionTilSep08 = 8;
		gUserVersionTilNov08 = 9;
		gUserVersionTilJan09 = 10;
		gUserVersionTilApr09 = 11;
		gUserVersion = 13;
		gProjectileDefinition = new ProjectileDefinition[23]
		{
			new ProjectileDefinition(ProjectileType.Pea, 0, 20),
			new ProjectileDefinition(ProjectileType.Snowpea, 0, 20),
			new ProjectileDefinition(ProjectileType.Cabbage, 0, 40),
			new ProjectileDefinition(ProjectileType.Melon, 0, 80),
			new ProjectileDefinition(ProjectileType.Puff, 0, 20),
			new ProjectileDefinition(ProjectileType.Wintermelon, 0, 80),
			new ProjectileDefinition(ProjectileType.Fireball, 0, 40),
			new ProjectileDefinition(ProjectileType.Star, 0, 20),
			new ProjectileDefinition(ProjectileType.Spike, 0, 20),
			new ProjectileDefinition(ProjectileType.Basketball, 0, 75),
			new ProjectileDefinition(ProjectileType.Kernel, 0, 20),
			new ProjectileDefinition(ProjectileType.Cobbig, 0, 300),
			new ProjectileDefinition(ProjectileType.Butter, 0, 40),
			new ProjectileDefinition(ProjectileType.ZombiePea, 0, 20),
			new ProjectileDefinition(ProjectileType.ZombiePeaMindControl, 0, 20),
			new ProjectileDefinition(ProjectileType.CattailSpike, 0, 20),
			new ProjectileDefinition(ProjectileType.EndoflameSpike, 0, 20),
			new ProjectileDefinition(ProjectileType.EndoflameFireball, 0, 40),
			new ProjectileDefinition(ProjectileType.Talisman, 0, 0),
			new ProjectileDefinition(ProjectileType.TalismanMove, 0, 0),
			new ProjectileDefinition(ProjectileType.TalismanSeal, 0, 0),
			new ProjectileDefinition(ProjectileType.HypnoCattailSpike, 0, 20),
			new ProjectileDefinition(ProjectileType.PuffGreen, 0, 20)
		};
		SEED_BANK_OFFSET_X = 0;
		SEED_BANK_OFFSET_X_END = 10;
	}
}
