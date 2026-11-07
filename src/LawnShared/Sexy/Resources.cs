using System;

namespace Sexy;

public static class Resources
{
	public enum ResourceId
	{
		IMAGE_POPCAP_LOGO_ID,
		IMAGE_POPCAP_LOGO_REGISTERED_ID,
		IMAGE_TITLESCREEN_ID,
		IMAGE_LOADING_ID,
		IMAGE_PVZ_LOGO_ID,
		SOUND_BUTTONCLICK_ID,
		SOUND_LOADINGBAR_FLOWER_ID,
		SOUND_LOADINGBAR_ZOMBIE_ID,
		FONT_BRIANNETOD16_ID,
		FONT_HOUSEOFTERROR16_ID,
		FONT_CONTINUUMBOLD14_ID,
		FONT_CONTINUUMBOLD14OUTLINE_ID,
		FONT_DWARVENTODCRAFT12_ID,
		FONT_DWARVENTODCRAFT15_ID,
		FONT_DWARVENTODCRAFT18_ID,
		FONT_PICO129_ID,
		FONT_BRIANNETOD12_ID,
		IMAGE_CHARREDZOMBIES_ID,
		IMAGE_ALMANACUI_ID,
		IMAGE_SEEDATLAS_ID,
		IMAGE_DAVE_ID,
		IMAGE_DIALOG_ID,
		IMAGE_CONVEYORBELT_BACKDROP_ID,
		IMAGE_CONVEYORBELT_BELT_ID,
		IMAGE_SPEECHBUBBLE_ID,
		IMAGE_LOC_EN_ID,
		IMAGE_ZOMBIE_NOTE_SMALL_ID,
		IMAGE_REANIM_ZOMBIESWON_ID,
		IMAGE_SCARY_POT_ID,
		SOUND_AWOOGA_ID,
		SOUND_BLEEP_ID,
		SOUND_BUZZER_ID,
		SOUND_CHOMP_ID,
		SOUND_CHOMP2_ID,
		SOUND_CHOMPSOFT_ID,
		SOUND_FLOOP_ID,
		SOUND_FROZEN_ID,
		SOUND_GULP_ID,
		SOUND_GROAN_ID,
		SOUND_GROAN2_ID,
		SOUND_GROAN3_ID,
		SOUND_GROAN4_ID,
		SOUND_GROAN5_ID,
		SOUND_GROAN6_ID,
		SOUND_LOSEMUSIC_ID,
		SOUND_MINDCONTROLLED_ID,
		SOUND_PAUSE_ID,
		SOUND_PLANT_ID,
		SOUND_PLANT2_ID,
		SOUND_POINTS_ID,
		SOUND_SEEDLIFT_ID,
		SOUND_SIREN_ID,
		SOUND_SLURP_ID,
		SOUND_SPLAT_ID,
		SOUND_SPLAT2_ID,
		SOUND_SPLAT3_ID,
		SOUND_SUKHBIR4_ID,
		SOUND_SUKHBIR5_ID,
		SOUND_SUKHBIR6_ID,
		SOUND_TAP_ID,
		SOUND_TAP2_ID,
		SOUND_THROW_ID,
		SOUND_THROW2_ID,
		SOUND_BLOVER_ID,
		SOUND_WINMUSIC_ID,
		SOUND_LAWNMOWER_ID,
		SOUND_BOING_ID,
		SOUND_JACKINTHEBOX_ID,
		SOUND_DIAMOND_ID,
		SOUND_DOLPHIN_APPEARS_ID,
		SOUND_DOLPHIN_BEFORE_JUMPING_ID,
		SOUND_POTATO_MINE_ID,
		SOUND_ZAMBONI_ID,
		SOUND_BALLOON_POP_ID,
		SOUND_THUNDER_ID,
		SOUND_ZOMBIESPLASH_ID,
		SOUND_BOWLING_ID,
		SOUND_BOWLINGIMPACT_ID,
		SOUND_BOWLINGIMPACT2_ID,
		SOUND_GRAVEBUSTERCHOMP_ID,
		SOUND_GRAVEBUTTON_ID,
		SOUND_LIMBS_POP_ID,
		SOUND_PLANTERN_ID,
		SOUND_POGO_ZOMBIE_ID,
		SOUND_SNOW_PEA_SPARKLES_ID,
		SOUND_PLANT_WATER_ID,
		SOUND_ZOMBIE_ENTERING_WATER_ID,
		SOUND_ZOMBIE_FALLING_1_ID,
		SOUND_ZOMBIE_FALLING_2_ID,
		SOUND_PUFF_ID,
		SOUND_FUME_ID,
		SOUND_HUGE_WAVE_ID,
		SOUND_SLOT_MACHINE_ID,
		SOUND_COIN_ID,
		SOUND_ROLL_IN_ID,
		SOUND_DIGGER_ZOMBIE_ID,
		SOUND_HATCHBACK_CLOSE_ID,
		SOUND_HATCHBACK_OPEN_ID,
		SOUND_KERNELPULT_ID,
		SOUND_KERNELPULT2_ID,
		SOUND_ZOMBAQUARIUM_DIE_ID,
		SOUND_BUNGEE_SCREAM_ID,
		SOUND_BUNGEE_SCREAM2_ID,
		SOUND_BUNGEE_SCREAM3_ID,
		SOUND_BUTTER_ID,
		SOUND_JACK_SURPRISE_ID,
		SOUND_JACK_SURPRISE2_ID,
		SOUND_NEWSPAPER_RARRGH_ID,
		SOUND_NEWSPAPER_RARRGH2_ID,
		SOUND_NEWSPAPER_RIP_ID,
		SOUND_SQUASH_HMM_ID,
		SOUND_SQUASH_HMM2_ID,
		SOUND_VASE_BREAKING_ID,
		SOUND_POOL_CLEANER_ID,
		SOUND_MAGNETSHROOM_ID,
		SOUND_LADDER_ZOMBIE_ID,
		SOUND_GARGANTUAR_THUMP_ID,
		SOUND_BASKETBALL_ID,
		SOUND_FIREPEA_ID,
		SOUND_IGNITE_ID,
		SOUND_IGNITE2_ID,
		SOUND_READYSETPLANT_ID,
		SOUND_DOOMSHROOM_ID,
		SOUND_EXPLOSION_ID,
		SOUND_FINALWAVE_ID,
		SOUND_REVERSE_EXPLOSION_ID,
		SOUND_RVTHROW_ID,
		SOUND_SHIELDHIT_ID,
		SOUND_SHIELDHIT2_ID,
		SOUND_BOSSEXPLOSION_ID,
		SOUND_CHERRYBOMB_ID,
		SOUND_BONK_ID,
		SOUND_SWING_ID,
		SOUND_RAIN_ID,
		SOUND_LIGHTFILL_ID,
		SOUND_PLASTICHIT_ID,
		SOUND_PLASTICHIT2_ID,
		SOUND_JALAPENO_ID,
		SOUND_BALLOONINFLATE_ID,
		SOUND_BIGCHOMP_ID,
		SOUND_MELONIMPACT_ID,
		SOUND_MELONIMPACT2_ID,
		SOUND_PLANTGROW_ID,
		SOUND_SHOOP_ID,
		SOUND_JUICY_ID,
		SOUND_COFFEE_ID,
		SOUND_WAKEUP_ID,
		SOUND_LOWGROAN_ID,
		SOUND_LOWGROAN2_ID,
		SOUND_PRIZE_ID,
		SOUND_YUCK_ID,
		SOUND_YUCK2_ID,
		SOUND_GRASSSTEP_ID,
		SOUND_SHOVEL_ID,
		SOUND_COBLAUNCH_ID,
		SOUND_WATERING_ID,
		SOUND_POLEVAULT_ID,
		SOUND_GRAVESTONE_RUMBLE_ID,
		SOUND_DIRT_RISE_ID,
		SOUND_FERTILIZER_ID,
		SOUND_PORTAL_ID,
		SOUND_SCREAM_ID,
		SOUND_PAPER_ID,
		SOUND_MONEYFALLS_ID,
		SOUND_IMP_ID,
		SOUND_IMP2_ID,
		SOUND_HYDRAULIC_SHORT_ID,
		SOUND_HYDRAULIC_ID,
		SOUND_GARGANTUDEATH_ID,
		SOUND_CERAMIC_ID,
		SOUND_BOSSBOULDERATTACK_ID,
		SOUND_CHIME_ID,
		SOUND_CRAZYDAVESHORT1_ID,
		SOUND_CRAZYDAVESHORT2_ID,
		SOUND_CRAZYDAVESHORT3_ID,
		SOUND_CRAZYDAVELONG1_ID,
		SOUND_CRAZYDAVELONG2_ID,
		SOUND_CRAZYDAVELONG3_ID,
		SOUND_CRAZYDAVEEXTRALONG1_ID,
		SOUND_CRAZYDAVEEXTRALONG2_ID,
		SOUND_CRAZYDAVEEXTRALONG3_ID,
		SOUND_CRAZYDAVECRAZY_ID,
		SOUND_DANCER_ID,
		SOUND_FINALFANFARE_ID,
		SOUND_CRAZYDAVESCREAM_ID,
		SOUND_CRAZYDAVESCREAM2_ID,
		SOUND_ACHIEVEMENT_ID,
		SOUND_BUGSPRAY_ID,
		SOUND_FERTILISER_ID,
		SOUND_PHONOGRAPH_ID,
		IMAGE_PILE_ID,
		IMAGE_PLANTSZOMBIES_ID,
		IMAGE_PARTICLES_ID,
		IMAGE_SLOTMACHINE_OVERLAY_ID,
		IMAGE_ZENGARDEN_ID,
		IMAGE_CACHED_ID,
		IMAGE_SELECTORSCREEN_ACHIEVEMENTS_TOP_BACKGROUND_ID,
		IMAGE_SELECTORSCREEN_MAIN_BACKGROUND_ID,
		IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE_ID,
		IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE_CHINA_ID,
		IMAGE_SELECTORSCREEN_QUICKPLAY_BACKGROUND_ID,
		IMAGE_GOODIES_ID,
		IMAGE_QUICKPLAY_ID,
		IMAGE_ACHIEVEMENT_GNOME_ID,
		IMAGE_MINIGAMES_ID,
		IMAGE_SELECTORSCREEN_MOREGAMES_BACKGROUND_ID,
		IMAGE_CREDITS_ZOMBIENOTE_ID,
		IMAGE_LEADERBOARDSCREEN_BACKGROUND_ID,
		IMAGE_BLACKHOLE_ID,
		IMAGE_EDGE_OF_SPACE_ID,
		IMAGE_STARS_1_ID,
		IMAGE_STARS_2_ID,
		IMAGE_STARS_3_ID,
		IMAGE_STARS_4_ID,
		IMAGE_BACKGROUND_GREENHOUSE_ID,
		IMAGE_AQUARIUM1_ID,
		IMAGE_BACKGROUND_MUSHROOMGARDEN_ID,
		IMAGE_BACKGROUND1_ID,
		IMAGE_BACKGROUND1_GAMEOVER_INTERIOR_OVERLAY_ID,
		IMAGE_BACKGROUND1_GAMEOVER_MASK_ID,
		IMAGE_BACKGROUND1UNSODDED_ID,
		IMAGE_BACKGROUND2_ID,
		IMAGE_BACKGROUND2_GAMEOVER_INTERIOR_OVERLAY_ID,
		IMAGE_BACKGROUND2_GAMEOVER_MASK_ID,
		IMAGE_BACKGROUND3_ID,
		IMAGE_BACKGROUND3_GAMEOVER_INTERIOR_OVERLAY_ID,
		IMAGE_BACKGROUND3_GAMEOVER_MASK_ID,
		IMAGE_BACKGROUND4_ID,
		IMAGE_BACKGROUND4_GAMEOVER_INTERIOR_OVERLAY_ID,
		IMAGE_BACKGROUND4_GAMEOVER_MASK_ID,
		IMAGE_FOG_ID,
		IMAGE_BACKGROUND5_ID,
		IMAGE_BACKGROUND5_GAMEOVER_MASK_ID,
		IMAGE_BACKGROUND6BOSS_ID,
		IMAGE_BACKGROUND6_GAMEOVER_MASK_ID,
		IMAGE_STORE_BACKGROUND_ID,
		IMAGE_STORE_BACKGROUNDNIGHT_ID,
		IMAGE_STORE_CAR_ID,
		IMAGE_STORE_CAR_NIGHT_ID,
		IMAGE_STORE_CARCLOSED_ID,
		IMAGE_STORE_CARCLOSED_NIGHT_ID,
		IMAGE_STORE_HATCHBACKOPEN_ID,
		IMAGE_ZOMBIE_NOTE_ID,
		IMAGE_ZOMBIE_NOTE1_ID,
		IMAGE_ZOMBIE_NOTE2_ID,
		IMAGE_ZOMBIE_NOTE3_ID,
		IMAGE_ZOMBIE_NOTE4_ID,
		IMAGE_ZOMBIE_FINAL_NOTE_ID,
		IMAGE_ZOMBIE_NOTE_HELP_ID,
		RESOURCE_ID_MAX
	}

	public static readonly Image LOAD_LOGO_IMAGE_DATA;

	public static Image IMAGE_POPCAP_LOGO;

	public static Image IMAGE_POPCAP_LOGO_REGISTERED;

	public static Image IMAGE_TITLESCREEN;

	public static Image IMAGE_LOADING;

	public static Image IMAGE_PVZ_LOGO;

	public static int SOUND_BUTTONCLICK;

	public static int SOUND_LOADINGBAR_FLOWER;

	public static int SOUND_LOADINGBAR_ZOMBIE;

	public static Font FONT_BRIANNETOD16;

	public static Font FONT_HOUSEOFTERROR16;

	public static Font FONT_CONTINUUMBOLD14;

	public static Font FONT_CONTINUUMBOLD14OUTLINE;

	public static Font FONT_DWARVENTODCRAFT12;

	public static Font FONT_DWARVENTODCRAFT15;

	public static Font FONT_DWARVENTODCRAFT18;

	public static Font FONT_PICO129;

	public static Font FONT_BRIANNETOD12;

	public static Image IMAGE_CHARREDZOMBIES;

	public static Image IMAGE_ALMANACUI;

	public static Image IMAGE_SEEDATLAS;

	public static Image IMAGE_DAVE;

	public static Image IMAGE_DIALOG;

	public static Image IMAGE_CONVEYORBELT_BACKDROP;

	public static Image IMAGE_CONVEYORBELT_BELT;

	public static Image IMAGE_SPEECHBUBBLE;

	public static Image IMAGE_LOC_EN;

	public static Image IMAGE_ZOMBIE_NOTE_SMALL;

	public static Image IMAGE_REANIM_ZOMBIESWON;

	public static Image IMAGE_SCARY_POT;

	public static Image IMAGE_SUNFLOWER_TROPHY_1;

	public static Image IMAGE_SUNFLOWER_TROPHY_2;

	public static int SOUND_AWOOGA;

	public static int SOUND_BLEEP;

	public static int SOUND_BUZZER;

	public static int SOUND_CHOMP;

	public static int SOUND_CHOMP2;

	public static int SOUND_CHOMPSOFT;

	public static int SOUND_FLOOP;

	public static int SOUND_FROZEN;

	public static int SOUND_GULP;

	public static int SOUND_GROAN;

	public static int SOUND_GROAN2;

	public static int SOUND_GROAN3;

	public static int SOUND_GROAN4;

	public static int SOUND_GROAN5;

	public static int SOUND_GROAN6;

	public static int SOUND_LOSEMUSIC;

	public static int SOUND_MINDCONTROLLED;

	public static int SOUND_PAUSE;

	public static int SOUND_PLANT;

	public static int SOUND_PLANT2;

	public static int SOUND_POINTS;

	public static int SOUND_SEEDLIFT;

	public static int SOUND_SIREN;

	public static int SOUND_SLURP;

	public static int SOUND_SPLAT;

	public static int SOUND_SPLAT2;

	public static int SOUND_SPLAT3;

	public static int SOUND_SUKHBIR4;

	public static int SOUND_SUKHBIR5;

	public static int SOUND_SUKHBIR6;

	public static int SOUND_TAP;

	public static int SOUND_TAP2;

	public static int SOUND_THROW;

	public static int SOUND_THROW2;

	public static int SOUND_BLOVER;

	public static int SOUND_WINMUSIC;

	public static int SOUND_LAWNMOWER;

	public static int SOUND_BOING;

	public static int SOUND_JACKINTHEBOX;

	public static int SOUND_DIAMOND;

	public static int SOUND_DOLPHIN_APPEARS;

	public static int SOUND_DOLPHIN_BEFORE_JUMPING;

	public static int SOUND_POTATO_MINE;

	public static int SOUND_ZAMBONI;

	public static int SOUND_BALLOON_POP;

	public static int SOUND_THUNDER;

	public static int SOUND_ZOMBIESPLASH;

	public static int SOUND_BOWLING;

	public static int SOUND_BOWLINGIMPACT;

	public static int SOUND_BOWLINGIMPACT2;

	public static int SOUND_GRAVEBUSTERCHOMP;

	public static int SOUND_GRAVEBUTTON;

	public static int SOUND_LIMBS_POP;

	public static int SOUND_PLANTERN;

	public static int SOUND_POGO_ZOMBIE;

	public static int SOUND_SNOW_PEA_SPARKLES;

	public static int SOUND_PLANT_WATER;

	public static int SOUND_ZOMBIE_ENTERING_WATER;

	public static int SOUND_ZOMBIE_FALLING_1;

	public static int SOUND_ZOMBIE_FALLING_2;

	public static int SOUND_PUFF;

	public static int SOUND_FUME;

	public static int SOUND_HUGE_WAVE;

	public static int SOUND_SLOT_MACHINE;

	public static int SOUND_COIN;

	public static int SOUND_ROLL_IN;

	public static int SOUND_DIGGER_ZOMBIE;

	public static int SOUND_HATCHBACK_CLOSE;

	public static int SOUND_HATCHBACK_OPEN;

	public static int SOUND_KERNELPULT;

	public static int SOUND_KERNELPULT2;

	public static int SOUND_ZOMBAQUARIUM_DIE;

	public static int SOUND_BUNGEE_SCREAM;

	public static int SOUND_BUNGEE_SCREAM2;

	public static int SOUND_BUNGEE_SCREAM3;

	public static int SOUND_BUTTER;

	public static int SOUND_JACK_SURPRISE;

	public static int SOUND_JACK_SURPRISE2;

	public static int SOUND_NEWSPAPER_RARRGH;

	public static int SOUND_NEWSPAPER_RARRGH2;

	public static int SOUND_NEWSPAPER_RIP;

	public static int SOUND_SQUASH_HMM;

	public static int SOUND_SQUASH_HMM2;

	public static int SOUND_VASE_BREAKING;

	public static int SOUND_POOL_CLEANER;

	public static int SOUND_MAGNETSHROOM;

	public static int SOUND_LADDER_ZOMBIE;

	public static int SOUND_GARGANTUAR_THUMP;

	public static int SOUND_BASKETBALL;

	public static int SOUND_FIREPEA;

	public static int SOUND_IGNITE;

	public static int SOUND_IGNITE2;

	public static int SOUND_READYSETPLANT;

	public static int SOUND_DOOMSHROOM;

	public static int SOUND_EXPLOSION;

	public static int SOUND_FINALWAVE;

	public static int SOUND_REVERSE_EXPLOSION;

	public static int SOUND_RVTHROW;

	public static int SOUND_SHIELDHIT;

	public static int SOUND_SHIELDHIT2;

	public static int SOUND_BOSSEXPLOSION;

	public static int SOUND_CHERRYBOMB;

	public static int SOUND_BONK;

	public static int SOUND_SWING;

	public static int SOUND_RAIN;

	public static int SOUND_LIGHTFILL;

	public static int SOUND_PLASTICHIT;

	public static int SOUND_PLASTICHIT2;

	public static int SOUND_JALAPENO;

	public static int SOUND_BALLOONINFLATE;

	public static int SOUND_BIGCHOMP;

	public static int SOUND_MELONIMPACT;

	public static int SOUND_MELONIMPACT2;

	public static int SOUND_PLANTGROW;

	public static int SOUND_SHOOP;

	public static int SOUND_JUICY;

	public static int SOUND_COFFEE;

	public static int SOUND_WAKEUP;

	public static int SOUND_LOWGROAN;

	public static int SOUND_LOWGROAN2;

	public static int SOUND_PRIZE;

	public static int SOUND_YUCK;

	public static int SOUND_YUCK2;

	public static int SOUND_GRASSSTEP;

	public static int SOUND_SHOVEL;

	public static int SOUND_COBLAUNCH;

	public static int SOUND_WATERING;

	public static int SOUND_POLEVAULT;

	public static int SOUND_GRAVESTONE_RUMBLE;

	public static int SOUND_DIRT_RISE;

	public static int SOUND_FERTILIZER;

	public static int SOUND_PORTAL;

	public static int SOUND_SCREAM;

	public static int SOUND_PAPER;

	public static int SOUND_MONEYFALLS;

	public static int SOUND_IMP;

	public static int SOUND_IMP2;

	public static int SOUND_HYDRAULIC_SHORT;

	public static int SOUND_HYDRAULIC;

	public static int SOUND_GARGANTUDEATH;

	public static int SOUND_CERAMIC;

	public static int SOUND_BOSSBOULDERATTACK;

	public static int SOUND_CHIME;

	public static int SOUND_CRAZYDAVESHORT1;

	public static int SOUND_CRAZYDAVESHORT2;

	public static int SOUND_CRAZYDAVESHORT3;

	public static int SOUND_CRAZYDAVELONG1;

	public static int SOUND_CRAZYDAVELONG2;

	public static int SOUND_CRAZYDAVELONG3;

	public static int SOUND_CRAZYDAVEEXTRALONG1;

	public static int SOUND_CRAZYDAVEEXTRALONG2;

	public static int SOUND_CRAZYDAVEEXTRALONG3;

	public static int SOUND_CRAZYDAVECRAZY;

	public static int SOUND_DANCER;

	public static int SOUND_FINALFANFARE;

	public static int SOUND_CRAZYDAVESCREAM;

	public static int SOUND_CRAZYDAVESCREAM2;

	public static int SOUND_ACHIEVEMENT;

	public static int SOUND_BUGSPRAY;

	public static int SOUND_FERTILISER;

	public static int SOUND_PHONOGRAPH;

	public static int SOUND_CATTAILSPLAT;

	public static int SOUND_ZOMBIE_MONK_SP;

	public static int SOUND_AGAVE_ATTACK;

	public static int SOUND_AGAVE_IDLE2PREPARE;

	public static int SOUND_AGAVE_POWERFUL;

	public static int SOUND_AGAVE_PREPARE2IDLE;

	public static int SOUND_PUMPKIN_SHIELDBROKEN;

	public static int SOUND_ENDOFLAME_POWERFUL;

	public static int SOUND_SPIKEROCK_ATTACK;

	public static int SOUND_TALISMAN_ATTACK_1;

	public static Image IMAGE_PILE;

	public static Image IMAGE_PLANTSZOMBIES;

	public static Image IMAGE_PARTICLES;

	public static Image IMAGE_SLOTMACHINE_OVERLAY;

	public static Image IMAGE_ZENGARDEN;

	public static Image IMAGE_CACHED;

	public static Image IMAGE_SELECTORSCREEN_ACHIEVEMENTS_TOP_BACKGROUND;

	public static Image IMAGE_SELECTORSCREEN_MAIN_BACKGROUND;

	public static Image IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE;

	public static Image IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE_CHINA;

	public static Image IMAGE_SELECTORSCREEN_QUICKPLAY_BACKGROUND;

	public static Image IMAGE_GOODIES;

	public static Image IMAGE_QUICKPLAY;

	public static Image IMAGE_ACHIEVEMENT_GNOME;

	public static Image IMAGE_MINIGAMES;

	public static Image IMAGE_MINIGAMES2;

	public static Image IMAGE_SELECTORSCREEN_MOREGAMES_BACKGROUND;

	public static Image IMAGE_SELECTORSCREEN_PUZZLE_BACKGROUND;

	public static Image IMAGE_SELECTORSCREEN_MINIGAME_BACKGROUND;

	public static Image IMAGE_CREDITS_ZOMBIENOTE;

	public static Image IMAGE_LEADERBOARDSCREEN_BACKGROUND;

	public static Image IMAGE_BLACKHOLE;

	public static Image IMAGE_EDGE_OF_SPACE;

	public static Image IMAGE_STARS_1;

	public static Image IMAGE_STARS_2;

	public static Image IMAGE_STARS_3;

	public static Image IMAGE_STARS_4;

	public static Image IMAGE_BACKGROUND_GREENHOUSE;

	public static Image IMAGE_BACKGROUND_GREENHOUSE_NIGHT;

	public static Image IMAGE_AQUARIUM1;

	public static Image IMAGE_BACKGROUND_MUSHROOMGARDEN;

	public static Image IMAGE_BACKGROUND1;

	public static Image IMAGE_BACKGROUND1_GAMEOVER_INTERIOR_OVERLAY;

	public static Image IMAGE_BACKGROUND1_GAMEOVER_MASK;

	public static Image IMAGE_BACKGROUND1UNSODDED;

	public static Image IMAGE_BACKGROUND2;

	public static Image IMAGE_BACKGROUND2_GAMEOVER_INTERIOR_OVERLAY;

	public static Image IMAGE_BACKGROUND2_GAMEOVER_MASK;

	public static Image IMAGE_BACKGROUND3;

	public static Image IMAGE_BACKGROUND3_GAMEOVER_INTERIOR_OVERLAY;

	public static Image IMAGE_BACKGROUND3_GAMEOVER_MASK;

	public static Image IMAGE_BACKGROUND4;

	public static Image IMAGE_BACKGROUND4_GAMEOVER_INTERIOR_OVERLAY;

	public static Image IMAGE_BACKGROUND4_GAMEOVER_MASK;

	public static Image IMAGE_FOG;

	public static Image IMAGE_BACKGROUND5;

	public static Image IMAGE_BACKGROUND5_GAMEOVER_MASK;

	public static Image IMAGE_BACKGROUND6BOSS;

	public static Image IMAGE_BACKGROUND6_GAMEOVER_MASK;

	public static Image IMAGE_BACKGROUND7;

	public static Image IMAGE_BACKGROUND8;

	public static Image IMAGE_ROOF_TREES;

	public static Image IMAGE_ROOF_POLE;

	public static Image IMAGE_ROOF_TREES_NIGHT;

	public static Image IMAGE_ROOF_POLE_NIGHT;

	public static Image IMAGE_STORE_BACKGROUND;

	public static Image IMAGE_STORE_BACKGROUNDNIGHT;

	public static Image IMAGE_STORE_CAR;

	public static Image IMAGE_STORE_CAR_NIGHT;

	public static Image IMAGE_STORE_CARCLOSED;

	public static Image IMAGE_STORE_CARCLOSED_NIGHT;

	public static Image IMAGE_STORE_HATCHBACKOPEN;

	public static Image IMAGE_ZOMBIE_NOTE;

	public static Image IMAGE_ZOMBIE_NOTE1;

	public static Image IMAGE_ZOMBIE_NOTE2;

	public static Image IMAGE_ZOMBIE_NOTE3;

	public static Image IMAGE_ZOMBIE_NOTE4;

	public static Image IMAGE_ZOMBIE_FINAL_NOTE;

	public static Image IMAGE_ZOMBIE_NOTE_HELP;

	public static Image IMAGE_CATTAIL;

	public static Image IMAGE_CATTAIL2;

	public static Image IMAGE_CATTAIL3;

	public static Image IMAGE_CATTAIL4;

	public static Image IMAGE_CATTAIL5;

	public static Image IMAGE_CATTAIL6;

	public static Image IMAGE_CATTAIL7;

	public static Image IMAGE_CATTAIL8;

	public static Image IMAGE_CATTAIL9;

	public static Image IMAGE_CATTAIL10;

	public static Image IMAGE_CATTAIL11;

	public static Image IMAGE_CATTAIL12;

	public static Image IMAGE_CATTAIL13;

	public static Image IMAGE_CATTAIL14;

	public static Image IMAGE_CHOMPER;

	public static Image IMAGE_CHOMPER2;

	public static Image IMAGE_CHOMPER3;

	public static Image IMAGE_CHOMPER4;

	public static Image IMAGE_CHOMPER5;

	public static Image IMAGE_CHOMPER6;

	public static Image IMAGE_CHOMPER7;

	public static Image IMAGE_CHOMPER8;

	public static Image IMAGE_CHOMPER9;

	public static Image IMAGE_CHOMPER10;

	public static Image IMAGE_CHOMPER11;

	public static Image IMAGE_SUNFLOWER;

	public static Image IMAGE_GARLIC;

	public static Image IMAGE_GARLIC2;

	public static Image IMAGE_GARLIC3;

	public static Image IMAGE_GARLIC4;

	public static Image IMAGE_JALAPENO;

	public static Image IMAGE_JALAPENO2;

	public static Image IMAGE_JALAPENO3;

	public static Image IMAGE_JALAPENO4;

	public static Image IMAGE_JALAPENO5;

	public static Image IMAGE_JALAPENO6;

	public static Image IMAGE_JALAPENO7;

	public static Image IMAGE_JALAPENO8;

	public static Image IMAGE_JALAPENO9;

	public static Image IMAGE_JALAPENO10;

	public static Image IMAGE_JALAPENO11;

	public static Image IMAGE_KERNALPLUT;

	public static Image IMAGE_KERNALPLUT2;

	public static Image IMAGE_PLANTERN;

	public static Image IMAGE_PLANTERN2;

	public static Image IMAGE_PLANTERN3;

	public static Image IMAGE_PLANTERN4;

	public static Image IMAGE_PLANTERN5;

	public static Image IMAGE_REPEATER;

	public static Image IMAGE_REPEATER2;

	public static Image IMAGE_REPEATER3;

	public static Image IMAGE_REPEATER4;

	public static Image IMAGE_REPEATER5;

	public static Image IMAGE_REPEATER6;

	public static Image IMAGE_REPEATER7;

	public static Image IMAGE_SCAREDYSHROOM;

	public static Image IMAGE_SCAREDYSHROOM2;

	public static Image IMAGE_SCAREDYSHROOM3;

	public static Image IMAGE_SCAREDYSHROOM4;

	public static Image IMAGE_GRAVEBUSTER;

	public static Image IMAGE_GRAVEBUSTER2;

	public static Image IMAGE_GRAVEBUSTER3;

	public static Image IMAGE_GRAVEBUSTER4;

	public static Image IMAGE_GRAVEBUSTER5;

	public static Image IMAGE_GRAVEBUSTER6;

	public static Image IMAGE_GRAVEBUSTER7;

	public static Image IMAGE_GRAVEBUSTER8;

	public static Image IMAGE_GRAVEBUSTER9;

	public static Image IMAGE_GRAVEBUSTER10;

	public static Image IMAGE_SPIKEWEED;

	public static Image IMAGE_SPIKEWEED2;

	public static Image IMAGE_SPIKEWEED3;

	public static Image IMAGE_SPIKEWEED4;

	public static Image IMAGE_SPIKEWEED5;

	public static Image IMAGE_SPIKEWEED6;

	public static Image IMAGE_SPIKEWEED7;

	public static Image IMAGE_SPIKEWEED8;

	public static Image IMAGE_SPIKEWEED9;

	public static Image IMAGE_SPIKEWEED10;

	public static Image IMAGE_SPIKEWEED11;

	public static Image IMAGE_SPIKEWEED12;

	public static Image IMAGE_SPIKEWEED13;

	public static Image IMAGE_SPIKEWEED14;

	public static Image IMAGE_ZOMBIESEXTRA;

	public static Image IMAGE_AGAVE;

	public static Image IMAGE_AGAVE2;

	public static Image IMAGE_AGAVE3;

	public static Image IMAGE_AGAVE4;

	public static Image IMAGE_AGAVE5;

	public static Image IMAGE_AGAVE6;

	public static Image IMAGE_AGAVE7;

	public static Image IMAGE_AGAVE8;

	public static Image IMAGE_AGAVE9;

	public static Image IMAGE_AGAVE10;

	public static Image IMAGE_AGAVE11;

	public static Image IMAGE_AGAVE12;

	public static Image IMAGE_AGAVE13;

	public static Image IMAGE_AGAVE_FULLATTACK;

	public static Image IMAGE_AGAVE_FULLATTACK2;

	public static Image IMAGE_AGAVE_FULLATTACK3;

	public static Image IMAGE_ENDOFLAME_FULLATTACK;

	public static Image IMAGE_ENDOFLAME_FULLATTACK2;

	public static Image IMAGE_ENDOFLAME_FULLATTACK3;

	public static Image IMAGE_ENDOFLAME_FULLATTACK4;

	public static Image IMAGE_ENDOFLAME_FULLATTACK5;

	public static Image IMAGE_ENDOFLAME_FULLATTACK6;

	public static Image IMAGE_ENDOFLAME_FULLATTACK7;

	public static Image IMAGE_SEEDPACKETS;

	public static Image IMAGE_SEEDPACKETS_ADD;

	public static Image IMAGE_PAINTING1_0;

	public static Image IMAGE_PAINTING1_1;

	public static Image IMAGE_PAINTING6_0;

	public static Image IMAGE_PAINTING6_1;

	public static Image IMAGE_PAINTING7_0;

	public static Image IMAGE_PAINTING7_1;

	public static Image IMAGE_PAINTING11_0;

	public static Image IMAGE_PAINTING11_1;

	public static Image IMAGE_PAINTING13_0;

	public static Image IMAGE_PAINTING13_1;

	public static Image IMAGE_PAINTING16_0;

	public static Image IMAGE_PAINTING16_1;

	public static Image IMAGE_PAINTING20_0;

	public static Image IMAGE_PAINTING20_1;

	public static Image IMAGE_PAINTING21_0;

	public static Image IMAGE_PAINTING21_1;

	public static Image IMAGE_PAINTING25_0;

	public static Image IMAGE_PAINTING25_1;

	public static Image IMAGE_PAINTING34_0;

	public static Image IMAGE_PAINTING34_1;

	public static Image IMAGE_PAINTING36_0;

	public static Image IMAGE_PAINTING36_1;

	public static Image IMAGE_PAINTING43_0;

	public static Image IMAGE_PAINTING43_1;

	public static Image IMAGE_PAINTING51_0;

	public static Image IMAGE_PAINTING51_1;

	public static Image IMAGE_PAINTING30_0;

	public static Image IMAGE_PAINTING30_1;

	public static Image IMAGE_PAINTING_HYPNOSHROOM_0;

	public static Image IMAGE_PAINTING_HYPNOSHROOM_1;

	public static Image IMAGE_PAINTING_IMITATER_0;

	public static Image IMAGE_PAINTING_IMITATER_1;

	public static Image IMAGE_PAINTING_SPIKEROCK_0;

	public static Image IMAGE_PAINTING_SPIKEROCK_1;

	public static Image IMAGE_PAINTING_PEASHOOTER_0;

	public static Image IMAGE_PAINTING_PEASHOOTER_1;

	public static Image IMAGE_PAINTING_SQUASH_0;

	public static Image IMAGE_PAINTING_SQUASH_1;

	public static Image IMAGE_PAINTING_TALLNUT_0;

	public static Image IMAGE_PAINTING_TALLNUT_1;

	public static Image IMAGE_PAINTING_BLOVER_0;

	public static Image IMAGE_PAINTING_BLOVER_1;

	public static Image IMAGE_PAINTING_PUFFSHROOM_0;

	public static Image IMAGE_PAINTING_PUFFSHROOM_1;

	public static Image IMAGE_PAINTING_GATLINGPEA_0;

	public static Image IMAGE_PAINTING_GATLINGPEA_1;

	public static Image IMAGE_PAINTING_FUMESHROOM_0;

	public static Image IMAGE_PAINTING_FUMESHROOM_1;

	public static Image IMAGE_PAINTING_SNOWPEA_0;

	public static Image IMAGE_PAINTING_SNOWPEA_1;

	public static Image IMAGE_PAINTING_SEASHROOM_0;

	public static Image IMAGE_PAINTING_SEASHROOM_1;

	public static Image IMAGE_PAINTING_ICESHROOM_0;

	public static Image IMAGE_PAINTING_ICESHROOM_1;

	public static Image IMAGE_PAINTING_UMBRELLA_0;

	public static Image IMAGE_PAINTING_UMBRELLA_1;

	public static Image IMAGE_PAINTING_SPLITPEA_0;

	public static Image IMAGE_PAINTING_SPLITPEA_1;

	public static Image IMAGE_THUMBNAIL_CARTOON_SUNFLOWER;

	public static Image IMAGE_THUMBNAIL_CARTOON_JALAPENO;

	public static Image IMAGE_THUMBNAIL_CARTOON_SCAREDYSHROOM;

	public static Image IMAGE_THUMBNAIL_CARTOON_CHOMPER;

	public static Image IMAGE_THUMBNAIL_CARTOON_REPEATER;

	public static Image IMAGE_THUMBNAIL_CARTOON_GARLIC;

	public static Image IMAGE_THUMBNAIL_CARTOON_KERNELPULT;

	public static Image IMAGE_THUMBNAIL_CARTOON_LILYPAD;

	public static Image IMAGE_THUMBNAIL_CARTOON_CATTAIL;

	public static Image IMAGE_THUMBNAIL_CARTOON_PLANTERN;

	public static Image IMAGE_THUMBNAIL_CARTOON_GRAVEBUSTER;

	public static Image IMAGE_THUMBNAIL_CARTOON_BEJEWELED;

	public static Image IMAGE_THUMBNAIL_CARTOON_SPIKEWEED;

	public static Image IMAGE_THUMBNAIL_CARTOON_AGAVE;

	public static Image IMAGE_THUMBNAIL_CARTOON_PUMPKIN;

	public static Image IMAGE_THUMBNAIL_CARTOON_ROSE;

	public static Image IMAGE_THUMBNAIL_CARTOON_SPIKEROCK;

	public static Image IMAGE_THUMBNAIL_CARTOON_IMITATER;

	public static Image IMAGE_THUMBNAIL_CARTOON_SUN;

	public static Image IMAGE_THUMBNAIL_CARTOON_HYPNOSHROOM;

	public static Image IMAGE_THUMBNAIL_CARTOON_FUMESHROOM;

	public static Image IMAGE_THUMBNAIL_CARTOON_PUFFSHROOM;

	public static Image IMAGE_THUMBNAIL_CARTOON_ICESHROOM;

	public static Image IMAGE_REANIM_CORNPULT_BUTTER_SPLAT;

	public static bool gNeedRecalcVariableToIdMap;

	public static object[] gResources;

	public static int SOUND_TAPGLASS => SOUND_TAP;

	public static bool ExtractResourcesByName(ResourceManager theManager, string theName)
	{
		return theName switch
		{
			"Init" => ExtractInitResources(theManager), 
			"InitRegistered" => ExtractInitRegisteredResources(theManager), 
			"LoaderBar" => ExtractLoaderBarResources(theManager), 
			"LoaderBarFont" => ExtractLoaderBarFontResources(theManager), 
			"LoadingFonts" => ExtractLoadingFontsResources(theManager), 
			"LoadingImages" => ExtractLoadingImagesResources(theManager), 
			"LoadingSounds" => ExtractLoadingSoundsResources(theManager), 
			"DelayLoad_Pile" => ExtractDelayLoad_PileResources(theManager), 
			"DelayLoad_GamePlay" => ExtractDelayLoad_GamePlayResources(theManager), 
			"DelayLoad_ZenGarden" => ExtractDelayLoad_ZenGardenResources(theManager), 
			"DelayLoad_Cached" => ExtractDelayLoad_CachedResources(theManager), 
			"DelayLoad_MainMenu" => ExtractDelayLoad_MainMenuResources(theManager), 
			"DelayLoad_Credits" => ExtractDelayLoad_CreditsResources(theManager), 
			"DelayLoad_Leaderboard_Background" => ExtractDelayLoad_Leaderboard_BackgroundResources(theManager), 
			"DelayLoad_Leaderboard" => ExtractDelayLoad_LeaderboardResources(theManager), 
			"DelayLoad_Stars" => ExtractDelayLoad_StarsResources(theManager), 
			"DelayLoad_GreenHouseGarden" => ExtractDelayLoad_GreenHouseGardenResources(theManager), 
			"DelayLoad_GreenHouseNight" => ExtractDelayLoad_GreenHouseNightResources(theManager), 
			"DelayLoad_Zombiquarium" => ExtractDelayLoad_ZombiquariumResources(theManager), 
			"DelayLoad_MushroomGarden" => ExtractDelayLoad_MushroomGardenResources(theManager), 
			"DelayLoad_Background1" => ExtractDelayLoad_Background1Resources(theManager), 
			"DelayLoad_BackgroundUnsodded" => ExtractDelayLoad_BackgroundUnsoddedResources(theManager), 
			"DelayLoad_Background2" => ExtractDelayLoad_Background2Resources(theManager), 
			"DelayLoad_Background3" => ExtractDelayLoad_Background3Resources(theManager), 
			"DelayLoad_Background4" => ExtractDelayLoad_Background4Resources(theManager), 
			"DelayLoad_Background5" => ExtractDelayLoad_Background5Resources(theManager), 
			"DelayLoad_Background6" => ExtractDelayLoad_Background6Resources(theManager), 
			"DelayLoad_Background7" => ExtractDelayLoad_Background7Resources(theManager), 
			"DelayLoad_Background8" => ExtractDelayLoad_Background8Resources(theManager), 
			"DelayLoad_Almanac" => ExtractDelayLoad_AlmanacResources(theManager), 
			"DelayLoad_Store" => ExtractDelayLoad_StoreResources(theManager), 
			"DelayLoad_ZombieNote" => ExtractDelayLoad_ZombieNoteResources(theManager), 
			"DelayLoad_ZombieNote1" => ExtractDelayLoad_ZombieNote1Resources(theManager), 
			"DelayLoad_ZombieNote2" => ExtractDelayLoad_ZombieNote2Resources(theManager), 
			"DelayLoad_ZombieNote3" => ExtractDelayLoad_ZombieNote3Resources(theManager), 
			"DelayLoad_ZombieNote4" => ExtractDelayLoad_ZombieNote4Resources(theManager), 
			"DelayLoad_ZombieFinalNote" => ExtractDelayLoad_ZombieFinalNoteResources(theManager), 
			"DelayLoad_ZombieNoteHelp" => ExtractDelayLoad_ZombieNoteHelpResources(theManager), 
			"DelayLoad_Cartoon_Sunflower" => ExtractDelayLoad_Cartoon_SunflowerResources(theManager), 
			"DelayLoad_Cartoon_Jalapeno" => ExtractDelayLoad_Cartoon_JalapenoResources(theManager), 
			"DelayLoad_Cartoon_Scaredyshroom" => ExtractDelayLoad_Cartoon_ScaredyshroomResources(theManager), 
			"DelayLoad_Cartoon_Chomper" => ExtractDelayLoad_Cartoon_ChomperResources(theManager), 
			"DelayLoad_Cartoon_Repeater" => ExtractDelayLoad_Cartoon_RepeaterResources(theManager), 
			"DelayLoad_Cartoon_Garlic" => ExtractDelayLoad_Cartoon_GarlicResources(theManager), 
			"DelayLoad_Cartoon_Kernelpult" => ExtractDelayLoad_Cartoon_KernelpultResources(theManager), 
			"DelayLoad_Cartoon_Lilypad" => ExtractDelayLoad_Cartoon_LilypadResources(theManager), 
			"DelayLoad_Cartoon_Cattail" => ExtractDelayLoad_Cartoon_CattailResources(theManager), 
			"DelayLoad_Cartoon_Plantern" => ExtractDelayLoad_Cartoon_PlanternResources(theManager), 
			"DelayLoad_Cartoon_Gravebuster" => ExtractDelayLoad_Cartoon_GravebusterResources(theManager), 
			"DelayLoad_Cartoon_Bejeweled" => ExtractDelayLoad_Cartoon_BejeweledResources(theManager), 
			"DelayLoad_Cartoon_Spikeweed" => ExtractDelayLoad_Cartoon_SpikeweedResources(theManager), 
			"DelayLoad_Cartoon_Agave" => ExtractDelayLoad_Cartoon_AgaveResources(theManager), 
			"DelayLoad_Cartoon_Pumpkin" => ExtractDelayLoad_Cartoon_PumpkinResources(theManager), 
			"DelayLoad_Cartoon_Rose" => ExtractDelayLoad_Cartoon_RoseResources(theManager), 
			"DelayLoad_Cartoon_Spikerock" => ExtractDelayLoad_Cartoon_SpikerockResources(theManager), 
			"DelayLoad_Cartoon_Imitater" => ExtractDelayLoad_Cartoon_ImitaterResources(theManager), 
			"DelayLoad_Cartoon_Sun" => ExtractDelayLoad_Cartoon_SunResources(theManager), 
			"DelayLoad_Cartoon_Hypnoshroom" => ExtractDelayLoad_Cartoon_HypnoshroomResources(theManager), 
			"DelayLoad_Cartoon_Fumeshroom" => ExtractDelayLoad_Cartoon_FumeshroomResources(theManager), 
			"DelayLoad_Cartoon_Puffshroom" => ExtractDelayLoad_Cartoon_PuffshroomResources(theManager), 
			"DelayLoad_Cartoon_Iceshroom" => ExtractDelayLoad_Cartoon_IceshroomResources(theManager), 
			_ => false, 
		};
	}

	internal static void ExtractResources(ResourceManager theManager, AtlasResources theRes)
	{
		ExtractInitResources(theManager);
		ExtractInitRegisteredResources(theManager);
		ExtractLoaderBarResources(theManager);
		ExtractLoaderBarFontResources(theManager);
		ExtractLoadingFontsResources(theManager);
		ExtractLoadingImagesResources(theManager);
		ExtractLoadingSoundsResources(theManager);
		ExtractDelayLoad_PileResources(theManager);
		ExtractDelayLoad_GamePlayResources(theManager);
		ExtractDelayLoad_ZenGardenResources(theManager);
		ExtractDelayLoad_CachedResources(theManager);
		ExtractDelayLoad_MainMenuResources(theManager);
		ExtractDelayLoad_CreditsResources(theManager);
		ExtractDelayLoad_Leaderboard_BackgroundResources(theManager);
		ExtractDelayLoad_LeaderboardResources(theManager);
		ExtractDelayLoad_StarsResources(theManager);
		ExtractDelayLoad_GreenHouseGardenResources(theManager);
		ExtractDelayLoad_GreenHouseNightResources(theManager);
		ExtractDelayLoad_ZombiquariumResources(theManager);
		ExtractDelayLoad_MushroomGardenResources(theManager);
		ExtractDelayLoad_Background1Resources(theManager);
		ExtractDelayLoad_BackgroundUnsoddedResources(theManager);
		ExtractDelayLoad_Background2Resources(theManager);
		ExtractDelayLoad_Background3Resources(theManager);
		ExtractDelayLoad_Background4Resources(theManager);
		ExtractDelayLoad_Background5Resources(theManager);
		ExtractDelayLoad_Background6Resources(theManager);
		ExtractDelayLoad_AlmanacResources(theManager);
		ExtractDelayLoad_StoreResources(theManager);
		ExtractDelayLoad_ZombieNoteResources(theManager);
		ExtractDelayLoad_ZombieNote1Resources(theManager);
		ExtractDelayLoad_ZombieNote2Resources(theManager);
		ExtractDelayLoad_ZombieNote3Resources(theManager);
		ExtractDelayLoad_ZombieNote4Resources(theManager);
		ExtractDelayLoad_ZombieFinalNoteResources(theManager);
		ExtractDelayLoad_ZombieNoteHelpResources(theManager);
		theRes.ExtractResources();
	}

	public static bool ExtractInitResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_POPCAP_LOGO = theManager.GetImageThrow("IMAGE_POPCAP_LOGO");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractInitRegisteredResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_POPCAP_LOGO_REGISTERED = theManager.GetImageThrow("IMAGE_POPCAP_LOGO_REGISTERED");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractLoaderBarResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_TITLESCREEN = theManager.GetImageThrow("IMAGE_TITLESCREEN");
			IMAGE_LOADING = theManager.GetImageThrow("IMAGE_LOADING");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractLoaderBarFontResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_PVZ_LOGO = theManager.GetImageThrow("IMAGE_PVZ_LOGO");
			SOUND_BUTTONCLICK = theManager.GetSoundThrow("SOUND_BUTTONCLICK");
			SOUND_LOADINGBAR_FLOWER = theManager.GetSoundThrow("SOUND_LOADINGBAR_FLOWER");
			SOUND_LOADINGBAR_ZOMBIE = theManager.GetSoundThrow("SOUND_LOADINGBAR_ZOMBIE");
			FONT_BRIANNETOD16 = theManager.GetFontThrow("FONT_BRIANNETOD16");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractLoadingFontsResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			FONT_HOUSEOFTERROR16 = theManager.GetFontThrow("FONT_HOUSEOFTERROR16");
			FONT_CONTINUUMBOLD14 = theManager.GetFontThrow("FONT_CONTINUUMBOLD14");
			FONT_CONTINUUMBOLD14OUTLINE = theManager.GetFontThrow("FONT_CONTINUUMBOLD14OUTLINE");
			FONT_DWARVENTODCRAFT12 = theManager.GetFontThrow("FONT_DWARVENTODCRAFT12");
			FONT_DWARVENTODCRAFT15 = theManager.GetFontThrow("FONT_DWARVENTODCRAFT15");
			FONT_DWARVENTODCRAFT18 = theManager.GetFontThrow("FONT_DWARVENTODCRAFT18");
			FONT_PICO129 = theManager.GetFontThrow("FONT_PICO129");
			FONT_BRIANNETOD12 = theManager.GetFontThrow("FONT_BRIANNETOD12");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractLoadingImagesResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_CHARREDZOMBIES = theManager.GetImageThrow("IMAGE_CHARREDZOMBIES");
			IMAGE_ALMANACUI = theManager.GetImageThrow("IMAGE_ALMANACUI");
			IMAGE_SEEDATLAS = theManager.GetImageThrow("IMAGE_SEEDATLAS");
			IMAGE_SEEDPACKETS = theManager.GetImageThrow("IMAGE_SEEDPACKETS");
			IMAGE_DAVE = theManager.GetImageThrow("IMAGE_DAVE");
			IMAGE_DIALOG = theManager.GetImageThrow("IMAGE_DIALOG");
			IMAGE_CONVEYORBELT_BACKDROP = theManager.GetImageThrow("IMAGE_CONVEYORBELT_BACKDROP");
			IMAGE_CONVEYORBELT_BELT = theManager.GetImageThrow("IMAGE_CONVEYORBELT_BELT");
			IMAGE_SPEECHBUBBLE = theManager.GetImageThrow("IMAGE_SPEECHBUBBLE");
			IMAGE_LOC_EN = theManager.GetImageThrow("IMAGE_LOC_EN");
			IMAGE_ZOMBIE_NOTE_SMALL = theManager.GetImageThrow("IMAGE_ZOMBIE_NOTE_SMALL");
			IMAGE_REANIM_ZOMBIESWON = theManager.GetImageThrow("IMAGE_REANIM_ZOMBIESWON");
			IMAGE_SCARY_POT = theManager.GetImageThrow("IMAGE_SCARY_POT");
			IMAGE_SEEDPACKETS_ADD = theManager.GetImageThrow("IMAGE_SEEDPACKETS_ADD");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractLoadingSoundsResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			SOUND_AWOOGA = theManager.GetSoundThrow("SOUND_AWOOGA");
			SOUND_BLEEP = theManager.GetSoundThrow("SOUND_BLEEP");
			SOUND_BUZZER = theManager.GetSoundThrow("SOUND_BUZZER");
			SOUND_CHOMP = theManager.GetSoundThrow("SOUND_CHOMP");
			SOUND_CHOMP2 = theManager.GetSoundThrow("SOUND_CHOMP2");
			SOUND_CHOMPSOFT = theManager.GetSoundThrow("SOUND_CHOMPSOFT");
			SOUND_FLOOP = theManager.GetSoundThrow("SOUND_FLOOP");
			SOUND_FROZEN = theManager.GetSoundThrow("SOUND_FROZEN");
			SOUND_GULP = theManager.GetSoundThrow("SOUND_GULP");
			SOUND_GROAN = theManager.GetSoundThrow("SOUND_GROAN");
			SOUND_GROAN2 = theManager.GetSoundThrow("SOUND_GROAN2");
			SOUND_GROAN3 = theManager.GetSoundThrow("SOUND_GROAN3");
			SOUND_GROAN4 = theManager.GetSoundThrow("SOUND_GROAN4");
			SOUND_GROAN5 = theManager.GetSoundThrow("SOUND_GROAN5");
			SOUND_GROAN6 = theManager.GetSoundThrow("SOUND_GROAN6");
			SOUND_LOSEMUSIC = theManager.GetSoundThrow("SOUND_LOSEMUSIC");
			SOUND_MINDCONTROLLED = theManager.GetSoundThrow("SOUND_MINDCONTROLLED");
			SOUND_PAUSE = theManager.GetSoundThrow("SOUND_PAUSE");
			SOUND_PLANT = theManager.GetSoundThrow("SOUND_PLANT");
			SOUND_PLANT2 = theManager.GetSoundThrow("SOUND_PLANT2");
			SOUND_POINTS = theManager.GetSoundThrow("SOUND_POINTS");
			SOUND_SEEDLIFT = theManager.GetSoundThrow("SOUND_SEEDLIFT");
			SOUND_SIREN = theManager.GetSoundThrow("SOUND_SIREN");
			SOUND_SLURP = theManager.GetSoundThrow("SOUND_SLURP");
			SOUND_SPLAT = theManager.GetSoundThrow("SOUND_SPLAT");
			SOUND_SPLAT2 = theManager.GetSoundThrow("SOUND_SPLAT2");
			SOUND_SPLAT3 = theManager.GetSoundThrow("SOUND_SPLAT3");
			SOUND_SUKHBIR4 = theManager.GetSoundThrow("SOUND_SUKHBIR4");
			SOUND_SUKHBIR5 = theManager.GetSoundThrow("SOUND_SUKHBIR5");
			SOUND_SUKHBIR6 = theManager.GetSoundThrow("SOUND_SUKHBIR6");
			SOUND_TAP = theManager.GetSoundThrow("SOUND_TAP");
			SOUND_TAP2 = theManager.GetSoundThrow("SOUND_TAP2");
			SOUND_THROW = theManager.GetSoundThrow("SOUND_THROW");
			SOUND_THROW2 = theManager.GetSoundThrow("SOUND_THROW2");
			SOUND_BLOVER = theManager.GetSoundThrow("SOUND_BLOVER");
			SOUND_WINMUSIC = theManager.GetSoundThrow("SOUND_WINMUSIC");
			SOUND_LAWNMOWER = theManager.GetSoundThrow("SOUND_LAWNMOWER");
			SOUND_BOING = theManager.GetSoundThrow("SOUND_BOING");
			SOUND_JACKINTHEBOX = theManager.GetSoundThrow("SOUND_JACKINTHEBOX");
			SOUND_DIAMOND = theManager.GetSoundThrow("SOUND_DIAMOND");
			SOUND_DOLPHIN_APPEARS = theManager.GetSoundThrow("SOUND_DOLPHIN_APPEARS");
			SOUND_DOLPHIN_BEFORE_JUMPING = theManager.GetSoundThrow("SOUND_DOLPHIN_BEFORE_JUMPING");
			SOUND_POTATO_MINE = theManager.GetSoundThrow("SOUND_POTATO_MINE");
			SOUND_ZAMBONI = theManager.GetSoundThrow("SOUND_ZAMBONI");
			SOUND_BALLOON_POP = theManager.GetSoundThrow("SOUND_BALLOON_POP");
			SOUND_THUNDER = theManager.GetSoundThrow("SOUND_THUNDER");
			SOUND_ZOMBIESPLASH = theManager.GetSoundThrow("SOUND_ZOMBIESPLASH");
			SOUND_BOWLING = theManager.GetSoundThrow("SOUND_BOWLING");
			SOUND_BOWLINGIMPACT = theManager.GetSoundThrow("SOUND_BOWLINGIMPACT");
			SOUND_BOWLINGIMPACT2 = theManager.GetSoundThrow("SOUND_BOWLINGIMPACT2");
			SOUND_GRAVEBUSTERCHOMP = theManager.GetSoundThrow("SOUND_GRAVEBUSTERCHOMP");
			SOUND_GRAVEBUTTON = theManager.GetSoundThrow("SOUND_GRAVEBUTTON");
			SOUND_LIMBS_POP = theManager.GetSoundThrow("SOUND_LIMBS_POP");
			SOUND_PLANTERN = theManager.GetSoundThrow("SOUND_PLANTERN");
			SOUND_POGO_ZOMBIE = theManager.GetSoundThrow("SOUND_POGO_ZOMBIE");
			SOUND_SNOW_PEA_SPARKLES = theManager.GetSoundThrow("SOUND_SNOW_PEA_SPARKLES");
			SOUND_PLANT_WATER = theManager.GetSoundThrow("SOUND_PLANT_WATER");
			SOUND_ZOMBIE_ENTERING_WATER = theManager.GetSoundThrow("SOUND_ZOMBIE_ENTERING_WATER");
			SOUND_ZOMBIE_FALLING_1 = theManager.GetSoundThrow("SOUND_ZOMBIE_FALLING_1");
			SOUND_ZOMBIE_FALLING_2 = theManager.GetSoundThrow("SOUND_ZOMBIE_FALLING_2");
			SOUND_PUFF = theManager.GetSoundThrow("SOUND_PUFF");
			SOUND_FUME = theManager.GetSoundThrow("SOUND_FUME");
			SOUND_HUGE_WAVE = theManager.GetSoundThrow("SOUND_HUGE_WAVE");
			SOUND_SLOT_MACHINE = theManager.GetSoundThrow("SOUND_SLOT_MACHINE");
			SOUND_COIN = theManager.GetSoundThrow("SOUND_COIN");
			SOUND_ROLL_IN = theManager.GetSoundThrow("SOUND_ROLL_IN");
			SOUND_DIGGER_ZOMBIE = theManager.GetSoundThrow("SOUND_DIGGER_ZOMBIE");
			SOUND_HATCHBACK_CLOSE = theManager.GetSoundThrow("SOUND_HATCHBACK_CLOSE");
			SOUND_HATCHBACK_OPEN = theManager.GetSoundThrow("SOUND_HATCHBACK_OPEN");
			SOUND_KERNELPULT = theManager.GetSoundThrow("SOUND_KERNELPULT");
			SOUND_KERNELPULT2 = theManager.GetSoundThrow("SOUND_KERNELPULT2");
			SOUND_ZOMBAQUARIUM_DIE = theManager.GetSoundThrow("SOUND_ZOMBAQUARIUM_DIE");
			SOUND_BUNGEE_SCREAM = theManager.GetSoundThrow("SOUND_BUNGEE_SCREAM");
			SOUND_BUNGEE_SCREAM2 = theManager.GetSoundThrow("SOUND_BUNGEE_SCREAM2");
			SOUND_BUNGEE_SCREAM3 = theManager.GetSoundThrow("SOUND_BUNGEE_SCREAM3");
			SOUND_BUTTER = theManager.GetSoundThrow("SOUND_BUTTER");
			SOUND_JACK_SURPRISE = theManager.GetSoundThrow("SOUND_JACK_SURPRISE");
			SOUND_JACK_SURPRISE2 = theManager.GetSoundThrow("SOUND_JACK_SURPRISE2");
			SOUND_NEWSPAPER_RARRGH = theManager.GetSoundThrow("SOUND_NEWSPAPER_RARRGH");
			SOUND_NEWSPAPER_RARRGH2 = theManager.GetSoundThrow("SOUND_NEWSPAPER_RARRGH2");
			SOUND_NEWSPAPER_RIP = theManager.GetSoundThrow("SOUND_NEWSPAPER_RIP");
			SOUND_SQUASH_HMM = theManager.GetSoundThrow("SOUND_SQUASH_HMM");
			SOUND_SQUASH_HMM2 = theManager.GetSoundThrow("SOUND_SQUASH_HMM2");
			SOUND_VASE_BREAKING = theManager.GetSoundThrow("SOUND_VASE_BREAKING");
			SOUND_POOL_CLEANER = theManager.GetSoundThrow("SOUND_POOL_CLEANER");
			SOUND_MAGNETSHROOM = theManager.GetSoundThrow("SOUND_MAGNETSHROOM");
			SOUND_LADDER_ZOMBIE = theManager.GetSoundThrow("SOUND_LADDER_ZOMBIE");
			SOUND_GARGANTUAR_THUMP = theManager.GetSoundThrow("SOUND_GARGANTUAR_THUMP");
			SOUND_BASKETBALL = theManager.GetSoundThrow("SOUND_BASKETBALL");
			SOUND_FIREPEA = theManager.GetSoundThrow("SOUND_FIREPEA");
			SOUND_IGNITE = theManager.GetSoundThrow("SOUND_IGNITE");
			SOUND_IGNITE2 = theManager.GetSoundThrow("SOUND_IGNITE2");
			SOUND_READYSETPLANT = theManager.GetSoundThrow("SOUND_READYSETPLANT");
			SOUND_DOOMSHROOM = theManager.GetSoundThrow("SOUND_DOOMSHROOM");
			SOUND_EXPLOSION = theManager.GetSoundThrow("SOUND_EXPLOSION");
			SOUND_FINALWAVE = theManager.GetSoundThrow("SOUND_FINALWAVE");
			SOUND_REVERSE_EXPLOSION = theManager.GetSoundThrow("SOUND_REVERSE_EXPLOSION");
			SOUND_RVTHROW = theManager.GetSoundThrow("SOUND_RVTHROW");
			SOUND_SHIELDHIT = theManager.GetSoundThrow("SOUND_SHIELDHIT");
			SOUND_SHIELDHIT2 = theManager.GetSoundThrow("SOUND_SHIELDHIT2");
			SOUND_BOSSEXPLOSION = theManager.GetSoundThrow("SOUND_BOSSEXPLOSION");
			SOUND_CHERRYBOMB = theManager.GetSoundThrow("SOUND_CHERRYBOMB");
			SOUND_BONK = theManager.GetSoundThrow("SOUND_BONK");
			SOUND_SWING = theManager.GetSoundThrow("SOUND_SWING");
			SOUND_RAIN = theManager.GetSoundThrow("SOUND_RAIN");
			SOUND_LIGHTFILL = theManager.GetSoundThrow("SOUND_LIGHTFILL");
			SOUND_PLASTICHIT = theManager.GetSoundThrow("SOUND_PLASTICHIT");
			SOUND_PLASTICHIT2 = theManager.GetSoundThrow("SOUND_PLASTICHIT2");
			SOUND_JALAPENO = theManager.GetSoundThrow("SOUND_JALAPENO");
			SOUND_BALLOONINFLATE = theManager.GetSoundThrow("SOUND_BALLOONINFLATE");
			SOUND_BIGCHOMP = theManager.GetSoundThrow("SOUND_BIGCHOMP");
			SOUND_MELONIMPACT = theManager.GetSoundThrow("SOUND_MELONIMPACT");
			SOUND_MELONIMPACT2 = theManager.GetSoundThrow("SOUND_MELONIMPACT2");
			SOUND_PLANTGROW = theManager.GetSoundThrow("SOUND_PLANTGROW");
			SOUND_SHOOP = theManager.GetSoundThrow("SOUND_SHOOP");
			SOUND_JUICY = theManager.GetSoundThrow("SOUND_JUICY");
			SOUND_COFFEE = theManager.GetSoundThrow("SOUND_COFFEE");
			SOUND_WAKEUP = theManager.GetSoundThrow("SOUND_WAKEUP");
			SOUND_LOWGROAN = theManager.GetSoundThrow("SOUND_LOWGROAN");
			SOUND_LOWGROAN2 = theManager.GetSoundThrow("SOUND_LOWGROAN2");
			SOUND_PRIZE = theManager.GetSoundThrow("SOUND_PRIZE");
			SOUND_YUCK = theManager.GetSoundThrow("SOUND_YUCK");
			SOUND_YUCK2 = theManager.GetSoundThrow("SOUND_YUCK2");
			SOUND_GRASSSTEP = theManager.GetSoundThrow("SOUND_GRASSSTEP");
			SOUND_SHOVEL = theManager.GetSoundThrow("SOUND_SHOVEL");
			SOUND_COBLAUNCH = theManager.GetSoundThrow("SOUND_COBLAUNCH");
			SOUND_WATERING = theManager.GetSoundThrow("SOUND_WATERING");
			SOUND_POLEVAULT = theManager.GetSoundThrow("SOUND_POLEVAULT");
			SOUND_GRAVESTONE_RUMBLE = theManager.GetSoundThrow("SOUND_GRAVESTONE_RUMBLE");
			SOUND_DIRT_RISE = theManager.GetSoundThrow("SOUND_DIRT_RISE");
			SOUND_FERTILIZER = theManager.GetSoundThrow("SOUND_FERTILIZER");
			SOUND_PORTAL = theManager.GetSoundThrow("SOUND_PORTAL");
			SOUND_SCREAM = theManager.GetSoundThrow("SOUND_SCREAM");
			SOUND_PAPER = theManager.GetSoundThrow("SOUND_PAPER");
			SOUND_MONEYFALLS = theManager.GetSoundThrow("SOUND_MONEYFALLS");
			SOUND_IMP = theManager.GetSoundThrow("SOUND_IMP");
			SOUND_IMP2 = theManager.GetSoundThrow("SOUND_IMP2");
			SOUND_HYDRAULIC_SHORT = theManager.GetSoundThrow("SOUND_HYDRAULIC_SHORT");
			SOUND_HYDRAULIC = theManager.GetSoundThrow("SOUND_HYDRAULIC");
			SOUND_GARGANTUDEATH = theManager.GetSoundThrow("SOUND_GARGANTUDEATH");
			SOUND_CERAMIC = theManager.GetSoundThrow("SOUND_CERAMIC");
			SOUND_BOSSBOULDERATTACK = theManager.GetSoundThrow("SOUND_BOSSBOULDERATTACK");
			SOUND_CHIME = theManager.GetSoundThrow("SOUND_CHIME");
			SOUND_CRAZYDAVESHORT1 = theManager.GetSoundThrow("SOUND_CRAZYDAVESHORT1");
			SOUND_CRAZYDAVESHORT2 = theManager.GetSoundThrow("SOUND_CRAZYDAVESHORT2");
			SOUND_CRAZYDAVESHORT3 = theManager.GetSoundThrow("SOUND_CRAZYDAVESHORT3");
			SOUND_CRAZYDAVELONG1 = theManager.GetSoundThrow("SOUND_CRAZYDAVELONG1");
			SOUND_CRAZYDAVELONG2 = theManager.GetSoundThrow("SOUND_CRAZYDAVELONG2");
			SOUND_CRAZYDAVELONG3 = theManager.GetSoundThrow("SOUND_CRAZYDAVELONG3");
			SOUND_CRAZYDAVEEXTRALONG1 = theManager.GetSoundThrow("SOUND_CRAZYDAVEEXTRALONG1");
			SOUND_CRAZYDAVEEXTRALONG2 = theManager.GetSoundThrow("SOUND_CRAZYDAVEEXTRALONG2");
			SOUND_CRAZYDAVEEXTRALONG3 = theManager.GetSoundThrow("SOUND_CRAZYDAVEEXTRALONG3");
			SOUND_CRAZYDAVECRAZY = theManager.GetSoundThrow("SOUND_CRAZYDAVECRAZY");
			SOUND_DANCER = theManager.GetSoundThrow("SOUND_DANCER");
			SOUND_FINALFANFARE = theManager.GetSoundThrow("SOUND_FINALFANFARE");
			SOUND_CRAZYDAVESCREAM = theManager.GetSoundThrow("SOUND_CRAZYDAVESCREAM");
			SOUND_CRAZYDAVESCREAM2 = theManager.GetSoundThrow("SOUND_CRAZYDAVESCREAM2");
			SOUND_ACHIEVEMENT = theManager.GetSoundThrow("SOUND_ACHIEVEMENT");
			SOUND_BUGSPRAY = theManager.GetSoundThrow("SOUND_BUGSPRAY");
			SOUND_FERTILISER = theManager.GetSoundThrow("SOUND_FERTILISER");
			SOUND_PHONOGRAPH = theManager.GetSoundThrow("SOUND_PHONOGRAPH");
			SOUND_CATTAILSPLAT = theManager.GetSoundThrow("SOUND_CATTAILSPLAT");
			SOUND_ZOMBIE_MONK_SP = theManager.GetSoundThrow("SOUND_ZOMBIE_MONK_SP");
			SOUND_AGAVE_ATTACK = theManager.GetSoundThrow("SOUND_AGAVE_ATTACK");
			SOUND_AGAVE_IDLE2PREPARE = theManager.GetSoundThrow("SOUND_AGAVE_IDLE2PREPARE");
			SOUND_AGAVE_POWERFUL = theManager.GetSoundThrow("SOUND_AGAVE_POWERFUL");
			SOUND_AGAVE_PREPARE2IDLE = theManager.GetSoundThrow("SOUND_AGAVE_PREPARE2IDLE");
			SOUND_PUMPKIN_SHIELDBROKEN = theManager.GetSoundThrow("SOUND_PUMPKIN_SHIELDBROKEN");
			SOUND_ENDOFLAME_POWERFUL = theManager.GetSoundThrow("SOUND_ENDOFLAME_POWERFUL");
			SOUND_SPIKEROCK_ATTACK = theManager.GetSoundThrow("SOUND_SPIKEROCK_ATTACK");
			SOUND_TALISMAN_ATTACK_1 = theManager.GetSoundThrow("SOUND_TALISMAN_ATTACK_1");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_PileResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_PILE = theManager.GetImageThrow("IMAGE_PILE");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_GamePlayResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_PLANTSZOMBIES = theManager.GetImageThrow("IMAGE_PLANTSZOMBIES");
			IMAGE_PARTICLES = theManager.GetImageThrow("IMAGE_PARTICLES");
			IMAGE_SLOTMACHINE_OVERLAY = theManager.GetImageThrow("IMAGE_SLOTMACHINE_OVERLAY");
			IMAGE_CATTAIL = theManager.GetImageThrow("IMAGE_CATTAIL");
			IMAGE_CATTAIL2 = theManager.GetImageThrow("IMAGE_CATTAIL2");
			IMAGE_CATTAIL3 = theManager.GetImageThrow("IMAGE_CATTAIL3");
			IMAGE_CATTAIL4 = theManager.GetImageThrow("IMAGE_CATTAIL4");
			IMAGE_CATTAIL5 = theManager.GetImageThrow("IMAGE_CATTAIL5");
			IMAGE_CATTAIL6 = theManager.GetImageThrow("IMAGE_CATTAIL6");
			IMAGE_CATTAIL7 = theManager.GetImageThrow("IMAGE_CATTAIL7");
			IMAGE_CATTAIL8 = theManager.GetImageThrow("IMAGE_CATTAIL8");
			IMAGE_CATTAIL9 = theManager.GetImageThrow("IMAGE_CATTAIL9");
			IMAGE_CATTAIL10 = theManager.GetImageThrow("IMAGE_CATTAIL10");
			IMAGE_CATTAIL11 = theManager.GetImageThrow("IMAGE_CATTAIL11");
			IMAGE_CATTAIL12 = theManager.GetImageThrow("IMAGE_CATTAIL12");
			IMAGE_CATTAIL13 = theManager.GetImageThrow("IMAGE_CATTAIL13");
			IMAGE_CATTAIL14 = theManager.GetImageThrow("IMAGE_CATTAIL14");
			IMAGE_CHOMPER = theManager.GetImageThrow("IMAGE_CHOMPER");
			IMAGE_CHOMPER2 = theManager.GetImageThrow("IMAGE_CHOMPER2");
			IMAGE_CHOMPER3 = theManager.GetImageThrow("IMAGE_CHOMPER3");
			IMAGE_CHOMPER4 = theManager.GetImageThrow("IMAGE_CHOMPER4");
			IMAGE_CHOMPER5 = theManager.GetImageThrow("IMAGE_CHOMPER5");
			IMAGE_CHOMPER6 = theManager.GetImageThrow("IMAGE_CHOMPER6");
			IMAGE_CHOMPER7 = theManager.GetImageThrow("IMAGE_CHOMPER7");
			IMAGE_CHOMPER8 = theManager.GetImageThrow("IMAGE_CHOMPER8");
			IMAGE_CHOMPER9 = theManager.GetImageThrow("IMAGE_CHOMPER9");
			IMAGE_CHOMPER10 = theManager.GetImageThrow("IMAGE_CHOMPER10");
			IMAGE_CHOMPER11 = theManager.GetImageThrow("IMAGE_CHOMPER11");
			IMAGE_SUNFLOWER = theManager.GetImageThrow("IMAGE_SUNFLOWER");
			IMAGE_GARLIC = theManager.GetImageThrow("IMAGE_GARLIC");
			IMAGE_GARLIC2 = theManager.GetImageThrow("IMAGE_GARLIC2");
			IMAGE_GARLIC3 = theManager.GetImageThrow("IMAGE_GARLIC3");
			IMAGE_GARLIC4 = theManager.GetImageThrow("IMAGE_GARLIC4");
			IMAGE_JALAPENO = theManager.GetImageThrow("IMAGE_JALAPENO");
			IMAGE_JALAPENO2 = theManager.GetImageThrow("IMAGE_JALAPENO2");
			IMAGE_JALAPENO3 = theManager.GetImageThrow("IMAGE_JALAPENO3");
			IMAGE_JALAPENO4 = theManager.GetImageThrow("IMAGE_JALAPENO4");
			IMAGE_JALAPENO5 = theManager.GetImageThrow("IMAGE_JALAPENO5");
			IMAGE_JALAPENO6 = theManager.GetImageThrow("IMAGE_JALAPENO6");
			IMAGE_JALAPENO7 = theManager.GetImageThrow("IMAGE_JALAPENO7");
			IMAGE_JALAPENO8 = theManager.GetImageThrow("IMAGE_JALAPENO8");
			IMAGE_JALAPENO9 = theManager.GetImageThrow("IMAGE_JALAPENO9");
			IMAGE_JALAPENO10 = theManager.GetImageThrow("IMAGE_JALAPENO10");
			IMAGE_JALAPENO11 = theManager.GetImageThrow("IMAGE_JALAPENO11");
			IMAGE_KERNALPLUT = theManager.GetImageThrow("IMAGE_KERNALPLUT");
			IMAGE_KERNALPLUT2 = theManager.GetImageThrow("IMAGE_KERNALPLUT2");
			IMAGE_PLANTERN = theManager.GetImageThrow("IMAGE_PLANTERN");
			IMAGE_PLANTERN2 = theManager.GetImageThrow("IMAGE_PLANTERN2");
			IMAGE_PLANTERN3 = theManager.GetImageThrow("IMAGE_PLANTERN3");
			IMAGE_PLANTERN4 = theManager.GetImageThrow("IMAGE_PLANTERN4");
			IMAGE_PLANTERN5 = theManager.GetImageThrow("IMAGE_PLANTERN5");
			IMAGE_REPEATER = theManager.GetImageThrow("IMAGE_REPEATER");
			IMAGE_REPEATER2 = theManager.GetImageThrow("IMAGE_REPEATER2");
			IMAGE_REPEATER3 = theManager.GetImageThrow("IMAGE_REPEATER3");
			IMAGE_REPEATER4 = theManager.GetImageThrow("IMAGE_REPEATER4");
			IMAGE_REPEATER5 = theManager.GetImageThrow("IMAGE_REPEATER5");
			IMAGE_REPEATER6 = theManager.GetImageThrow("IMAGE_REPEATER6");
			IMAGE_REPEATER7 = theManager.GetImageThrow("IMAGE_REPEATER7");
			IMAGE_SCAREDYSHROOM = theManager.GetImageThrow("IMAGE_SCAREDYSHROOM");
			IMAGE_SCAREDYSHROOM2 = theManager.GetImageThrow("IMAGE_SCAREDYSHROOM2");
			IMAGE_SCAREDYSHROOM3 = theManager.GetImageThrow("IMAGE_SCAREDYSHROOM3");
			IMAGE_SCAREDYSHROOM4 = theManager.GetImageThrow("IMAGE_SCAREDYSHROOM4");
			IMAGE_GRAVEBUSTER = theManager.GetImageThrow("IMAGE_GRAVEBUSTER");
			IMAGE_GRAVEBUSTER2 = theManager.GetImageThrow("IMAGE_GRAVEBUSTER2");
			IMAGE_GRAVEBUSTER3 = theManager.GetImageThrow("IMAGE_GRAVEBUSTER3");
			IMAGE_GRAVEBUSTER4 = theManager.GetImageThrow("IMAGE_GRAVEBUSTER4");
			IMAGE_GRAVEBUSTER5 = theManager.GetImageThrow("IMAGE_GRAVEBUSTER5");
			IMAGE_GRAVEBUSTER6 = theManager.GetImageThrow("IMAGE_GRAVEBUSTER6");
			IMAGE_GRAVEBUSTER7 = theManager.GetImageThrow("IMAGE_GRAVEBUSTER7");
			IMAGE_GRAVEBUSTER8 = theManager.GetImageThrow("IMAGE_GRAVEBUSTER8");
			IMAGE_GRAVEBUSTER9 = theManager.GetImageThrow("IMAGE_GRAVEBUSTER9");
			IMAGE_GRAVEBUSTER10 = theManager.GetImageThrow("IMAGE_GRAVEBUSTER10");
			IMAGE_SPIKEWEED = theManager.GetImageThrow("IMAGE_SPIKEWEED");
			IMAGE_SPIKEWEED2 = theManager.GetImageThrow("IMAGE_SPIKEWEED2");
			IMAGE_SPIKEWEED3 = theManager.GetImageThrow("IMAGE_SPIKEWEED3");
			IMAGE_SPIKEWEED4 = theManager.GetImageThrow("IMAGE_SPIKEWEED4");
			IMAGE_SPIKEWEED5 = theManager.GetImageThrow("IMAGE_SPIKEWEED5");
			IMAGE_SPIKEWEED6 = theManager.GetImageThrow("IMAGE_SPIKEWEED6");
			IMAGE_SPIKEWEED7 = theManager.GetImageThrow("IMAGE_SPIKEWEED7");
			IMAGE_SPIKEWEED8 = theManager.GetImageThrow("IMAGE_SPIKEWEED8");
			IMAGE_SPIKEWEED9 = theManager.GetImageThrow("IMAGE_SPIKEWEED9");
			IMAGE_SPIKEWEED10 = theManager.GetImageThrow("IMAGE_SPIKEWEED10");
			IMAGE_SPIKEWEED11 = theManager.GetImageThrow("IMAGE_SPIKEWEED11");
			IMAGE_SPIKEWEED12 = theManager.GetImageThrow("IMAGE_SPIKEWEED12");
			IMAGE_SPIKEWEED13 = theManager.GetImageThrow("IMAGE_SPIKEWEED13");
			IMAGE_SPIKEWEED14 = theManager.GetImageThrow("IMAGE_SPIKEWEED14");
			IMAGE_REANIM_CORNPULT_BUTTER_SPLAT = theManager.GetImageThrow("IMAGE_REANIM_CORNPULT_BUTTER_SPLAT");
			IMAGE_ZOMBIESEXTRA = theManager.GetImageThrow("IMAGE_ZOMBIESEXTRA");
			IMAGE_AGAVE = theManager.GetImageThrow("IMAGE_AGAVE");
			IMAGE_AGAVE2 = theManager.GetImageThrow("IMAGE_AGAVE2");
			IMAGE_AGAVE3 = theManager.GetImageThrow("IMAGE_AGAVE3");
			IMAGE_AGAVE4 = theManager.GetImageThrow("IMAGE_AGAVE4");
			IMAGE_AGAVE5 = theManager.GetImageThrow("IMAGE_AGAVE5");
			IMAGE_AGAVE6 = theManager.GetImageThrow("IMAGE_AGAVE6");
			IMAGE_AGAVE7 = theManager.GetImageThrow("IMAGE_AGAVE7");
			IMAGE_AGAVE8 = theManager.GetImageThrow("IMAGE_AGAVE8");
			IMAGE_AGAVE9 = theManager.GetImageThrow("IMAGE_AGAVE9");
			IMAGE_AGAVE10 = theManager.GetImageThrow("IMAGE_AGAVE10");
			IMAGE_AGAVE11 = theManager.GetImageThrow("IMAGE_AGAVE11");
			IMAGE_AGAVE12 = theManager.GetImageThrow("IMAGE_AGAVE12");
			IMAGE_AGAVE13 = theManager.GetImageThrow("IMAGE_AGAVE13");
			IMAGE_AGAVE_FULLATTACK = theManager.GetImageThrow("IMAGE_AGAVE_FULLATTACK");
			IMAGE_AGAVE_FULLATTACK2 = theManager.GetImageThrow("IMAGE_AGAVE_FULLATTACK2");
			IMAGE_AGAVE_FULLATTACK3 = theManager.GetImageThrow("IMAGE_AGAVE_FULLATTACK3");
			IMAGE_ENDOFLAME_FULLATTACK = theManager.GetImageThrow("IMAGE_ENDOFLAME_FULLATTACK");
			IMAGE_ENDOFLAME_FULLATTACK2 = theManager.GetImageThrow("IMAGE_ENDOFLAME_FULLATTACK2");
			IMAGE_ENDOFLAME_FULLATTACK3 = theManager.GetImageThrow("IMAGE_ENDOFLAME_FULLATTACK3");
			IMAGE_ENDOFLAME_FULLATTACK4 = theManager.GetImageThrow("IMAGE_ENDOFLAME_FULLATTACK4");
			IMAGE_ENDOFLAME_FULLATTACK5 = theManager.GetImageThrow("IMAGE_ENDOFLAME_FULLATTACK5");
			IMAGE_ENDOFLAME_FULLATTACK6 = theManager.GetImageThrow("IMAGE_ENDOFLAME_FULLATTACK6");
			IMAGE_ENDOFLAME_FULLATTACK7 = theManager.GetImageThrow("IMAGE_ENDOFLAME_FULLATTACK7");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_ZenGardenResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_ZENGARDEN = theManager.GetImageThrow("IMAGE_ZENGARDEN");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_CachedResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_CACHED = theManager.GetImageThrow("IMAGE_CACHED");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_MainMenuResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_SELECTORSCREEN_ACHIEVEMENTS_TOP_BACKGROUND = theManager.GetImageThrow("IMAGE_SELECTORSCREEN_ACHIEVEMENTS_TOP_BACKGROUND");
			IMAGE_SELECTORSCREEN_MAIN_BACKGROUND = theManager.GetImageThrow("IMAGE_SELECTORSCREEN_MAIN_BACKGROUND");
			IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE = theManager.GetImageThrow("IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE");
			IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE_CHINA = theManager.GetImageThrow("IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE_CHINA");
			IMAGE_SELECTORSCREEN_QUICKPLAY_BACKGROUND = theManager.GetImageThrow("IMAGE_SELECTORSCREEN_QUICKPLAY_BACKGROUND");
			IMAGE_GOODIES = theManager.GetImageThrow("IMAGE_GOODIES");
			IMAGE_QUICKPLAY = theManager.GetImageThrow("IMAGE_QUICKPLAY");
			IMAGE_ACHIEVEMENT_GNOME = theManager.GetImageThrow("IMAGE_ACHIEVEMENT_GNOME");
			IMAGE_MINIGAMES = theManager.GetImageThrow("IMAGE_MINIGAMES");
			IMAGE_MINIGAMES2 = theManager.GetImageThrow("IMAGE_MINIGAMES2");
			IMAGE_SELECTORSCREEN_MOREGAMES_BACKGROUND = theManager.GetImageThrow("IMAGE_SELECTORSCREEN_MOREGAMES_BACKGROUND");
			IMAGE_SELECTORSCREEN_PUZZLE_BACKGROUND = theManager.GetImageThrow("IMAGE_SELECTORSCREEN_PUZZLE_BACKGROUND");
			IMAGE_SELECTORSCREEN_MINIGAME_BACKGROUND = theManager.GetImageThrow("IMAGE_SELECTORSCREEN_MINIGAME_BACKGROUND");
			IMAGE_SUNFLOWER_TROPHY_1 = theManager.GetImageThrow("IMAGE_SUNFLOWER_TROPHY_1");
			IMAGE_SUNFLOWER_TROPHY_2 = theManager.GetImageThrow("IMAGE_SUNFLOWER_TROPHY_2");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_CreditsResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_CREDITS_ZOMBIENOTE = theManager.GetImageThrow("IMAGE_CREDITS_ZOMBIENOTE");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_Leaderboard_BackgroundResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_LEADERBOARDSCREEN_BACKGROUND = theManager.GetImageThrow("IMAGE_LEADERBOARDSCREEN_BACKGROUND");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_LeaderboardResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BLACKHOLE = theManager.GetImageThrow("IMAGE_BLACKHOLE");
			IMAGE_EDGE_OF_SPACE = theManager.GetImageThrow("IMAGE_EDGE_OF_SPACE");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_StarsResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_STARS_1 = theManager.GetImageThrow("IMAGE_STARS_1");
			IMAGE_STARS_2 = theManager.GetImageThrow("IMAGE_STARS_2");
			IMAGE_STARS_3 = theManager.GetImageThrow("IMAGE_STARS_3");
			IMAGE_STARS_4 = theManager.GetImageThrow("IMAGE_STARS_4");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_GreenHouseGardenResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND_GREENHOUSE = theManager.GetImageThrow("IMAGE_BACKGROUND_GREENHOUSE");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_GreenHouseNightResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND_GREENHOUSE_NIGHT = theManager.GetImageThrow("IMAGE_BACKGROUND_GREENHOUSE_NIGHT");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_ZombiquariumResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_AQUARIUM1 = theManager.GetImageThrow("IMAGE_AQUARIUM1");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_MushroomGardenResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND_MUSHROOMGARDEN = theManager.GetImageThrow("IMAGE_BACKGROUND_MUSHROOMGARDEN");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_Background1Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND1 = theManager.GetImageThrow("IMAGE_BACKGROUND1");
			IMAGE_BACKGROUND1_GAMEOVER_INTERIOR_OVERLAY = theManager.GetImageThrow("IMAGE_BACKGROUND1_GAMEOVER_INTERIOR_OVERLAY");
			IMAGE_BACKGROUND1_GAMEOVER_MASK = theManager.GetImageThrow("IMAGE_BACKGROUND1_GAMEOVER_MASK");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_BackgroundUnsoddedResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND1UNSODDED = theManager.GetImageThrow("IMAGE_BACKGROUND1UNSODDED");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_Background2Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND2 = theManager.GetImageThrow("IMAGE_BACKGROUND2");
			IMAGE_BACKGROUND2_GAMEOVER_INTERIOR_OVERLAY = theManager.GetImageThrow("IMAGE_BACKGROUND2_GAMEOVER_INTERIOR_OVERLAY");
			IMAGE_BACKGROUND2_GAMEOVER_MASK = theManager.GetImageThrow("IMAGE_BACKGROUND2_GAMEOVER_MASK");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_Background3Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND3 = theManager.GetImageThrow("IMAGE_BACKGROUND3");
			IMAGE_BACKGROUND3_GAMEOVER_INTERIOR_OVERLAY = theManager.GetImageThrow("IMAGE_BACKGROUND3_GAMEOVER_INTERIOR_OVERLAY");
			IMAGE_BACKGROUND3_GAMEOVER_MASK = theManager.GetImageThrow("IMAGE_BACKGROUND3_GAMEOVER_MASK");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_Background4Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND4 = theManager.GetImageThrow("IMAGE_BACKGROUND4");
			IMAGE_BACKGROUND4_GAMEOVER_INTERIOR_OVERLAY = theManager.GetImageThrow("IMAGE_BACKGROUND4_GAMEOVER_INTERIOR_OVERLAY");
			IMAGE_BACKGROUND4_GAMEOVER_MASK = theManager.GetImageThrow("IMAGE_BACKGROUND4_GAMEOVER_MASK");
			IMAGE_FOG = theManager.GetImageThrow("IMAGE_FOG");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_Background5Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND5 = theManager.GetImageThrow("IMAGE_BACKGROUND5");
			IMAGE_BACKGROUND5_GAMEOVER_MASK = theManager.GetImageThrow("IMAGE_BACKGROUND5_GAMEOVER_MASK");
			IMAGE_ROOF_POLE = theManager.GetImageThrow("IMAGE_ROOF_POLE");
			IMAGE_ROOF_TREES = theManager.GetImageThrow("IMAGE_ROOF_TREES");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_Background6Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND6BOSS = theManager.GetImageThrow("IMAGE_BACKGROUND6BOSS");
			IMAGE_BACKGROUND6_GAMEOVER_MASK = theManager.GetImageThrow("IMAGE_BACKGROUND6_GAMEOVER_MASK");
			IMAGE_ROOF_POLE_NIGHT = theManager.GetImageThrow("IMAGE_ROOF_POLE_NIGHT");
			IMAGE_ROOF_TREES_NIGHT = theManager.GetImageThrow("IMAGE_ROOF_TREES_NIGHT");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_Background7Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND7 = theManager.GetImageThrow("IMAGE_BACKGROUND7");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_Background8Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_BACKGROUND8 = theManager.GetImageThrow("IMAGE_BACKGROUND8");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_AlmanacResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_PAINTING1_0 = theManager.GetImageThrow("IMAGE_PAINTING1_0");
			IMAGE_PAINTING1_1 = theManager.GetImageThrow("IMAGE_PAINTING1_1");
			IMAGE_PAINTING6_0 = theManager.GetImageThrow("IMAGE_PAINTING6_0");
			IMAGE_PAINTING6_1 = theManager.GetImageThrow("IMAGE_PAINTING6_1");
			IMAGE_PAINTING7_0 = theManager.GetImageThrow("IMAGE_PAINTING7_0");
			IMAGE_PAINTING7_1 = theManager.GetImageThrow("IMAGE_PAINTING7_1");
			IMAGE_PAINTING11_0 = theManager.GetImageThrow("IMAGE_PAINTING11_0");
			IMAGE_PAINTING11_1 = theManager.GetImageThrow("IMAGE_PAINTING11_1");
			IMAGE_PAINTING13_0 = theManager.GetImageThrow("IMAGE_PAINTING13_0");
			IMAGE_PAINTING13_1 = theManager.GetImageThrow("IMAGE_PAINTING13_1");
			IMAGE_PAINTING16_0 = theManager.GetImageThrow("IMAGE_PAINTING16_0");
			IMAGE_PAINTING16_1 = theManager.GetImageThrow("IMAGE_PAINTING16_1");
			IMAGE_PAINTING20_0 = theManager.GetImageThrow("IMAGE_PAINTING20_0");
			IMAGE_PAINTING20_1 = theManager.GetImageThrow("IMAGE_PAINTING20_1");
			IMAGE_PAINTING21_0 = theManager.GetImageThrow("IMAGE_PAINTING21_0");
			IMAGE_PAINTING21_1 = theManager.GetImageThrow("IMAGE_PAINTING21_1");
			IMAGE_PAINTING25_0 = theManager.GetImageThrow("IMAGE_PAINTING25_0");
			IMAGE_PAINTING25_1 = theManager.GetImageThrow("IMAGE_PAINTING25_1");
			IMAGE_PAINTING34_0 = theManager.GetImageThrow("IMAGE_PAINTING34_0");
			IMAGE_PAINTING34_1 = theManager.GetImageThrow("IMAGE_PAINTING34_1");
			IMAGE_PAINTING36_0 = theManager.GetImageThrow("IMAGE_PAINTING36_0");
			IMAGE_PAINTING36_1 = theManager.GetImageThrow("IMAGE_PAINTING36_1");
			IMAGE_PAINTING43_0 = theManager.GetImageThrow("IMAGE_PAINTING43_0");
			IMAGE_PAINTING43_1 = theManager.GetImageThrow("IMAGE_PAINTING43_1");
			IMAGE_PAINTING51_0 = theManager.GetImageThrow("IMAGE_PAINTING51_0");
			IMAGE_PAINTING51_1 = theManager.GetImageThrow("IMAGE_PAINTING51_1");
			IMAGE_PAINTING30_0 = theManager.GetImageThrow("IMAGE_PAINTING30_0");
			IMAGE_PAINTING30_1 = theManager.GetImageThrow("IMAGE_PAINTING30_1");
			IMAGE_PAINTING_HYPNOSHROOM_0 = theManager.GetImageThrow("IMAGE_PAINTING_HYPNOSHROOM_0");
			IMAGE_PAINTING_HYPNOSHROOM_1 = theManager.GetImageThrow("IMAGE_PAINTING_HYPNOSHROOM_1");
			IMAGE_PAINTING_IMITATER_0 = theManager.GetImageThrow("IMAGE_PAINTING_IMITATER_0");
			IMAGE_PAINTING_IMITATER_1 = theManager.GetImageThrow("IMAGE_PAINTING_IMITATER_1");
			IMAGE_PAINTING_SPIKEROCK_0 = theManager.GetImageThrow("IMAGE_PAINTING_SPIKEROCK_0");
			IMAGE_PAINTING_SPIKEROCK_1 = theManager.GetImageThrow("IMAGE_PAINTING_SPIKEROCK_1");
			IMAGE_PAINTING_PEASHOOTER_0 = theManager.GetImageThrow("IMAGE_PAINTING_PEASHOOTER_0");
			IMAGE_PAINTING_PEASHOOTER_1 = theManager.GetImageThrow("IMAGE_PAINTING_PEASHOOTER_1");
			IMAGE_PAINTING_SQUASH_0 = theManager.GetImageThrow("IMAGE_PAINTING_SQUASH_0");
			IMAGE_PAINTING_SQUASH_1 = theManager.GetImageThrow("IMAGE_PAINTING_SQUASH_1");
			IMAGE_PAINTING_TALLNUT_0 = theManager.GetImageThrow("IMAGE_PAINTING_TALLNUT_0");
			IMAGE_PAINTING_TALLNUT_1 = theManager.GetImageThrow("IMAGE_PAINTING_TALLNUT_1");
			IMAGE_PAINTING_BLOVER_0 = theManager.GetImageThrow("IMAGE_PAINTING_BLOVER_0");
			IMAGE_PAINTING_BLOVER_1 = theManager.GetImageThrow("IMAGE_PAINTING_BLOVER_1");
			IMAGE_PAINTING_PUFFSHROOM_0 = theManager.GetImageThrow("IMAGE_PAINTING_PUFFSHROOM_0");
			IMAGE_PAINTING_PUFFSHROOM_1 = theManager.GetImageThrow("IMAGE_PAINTING_PUFFSHROOM_1");
			IMAGE_PAINTING_GATLINGPEA_0 = theManager.GetImageThrow("IMAGE_PAINTING_GATLINGPEA_0");
			IMAGE_PAINTING_GATLINGPEA_1 = theManager.GetImageThrow("IMAGE_PAINTING_GATLINGPEA_1");
			IMAGE_PAINTING_FUMESHROOM_0 = theManager.GetImageThrow("IMAGE_PAINTING_FUMESHROOM_0");
			IMAGE_PAINTING_FUMESHROOM_1 = theManager.GetImageThrow("IMAGE_PAINTING_FUMESHROOM_1");
			IMAGE_PAINTING_SNOWPEA_0 = theManager.GetImageThrow("IMAGE_PAINTING_SNOWPEA_0");
			IMAGE_PAINTING_SNOWPEA_1 = theManager.GetImageThrow("IMAGE_PAINTING_SNOWPEA_1");
			IMAGE_PAINTING_SEASHROOM_0 = theManager.GetImageThrow("IMAGE_PAINTING_SEASHROOM_0");
			IMAGE_PAINTING_SEASHROOM_1 = theManager.GetImageThrow("IMAGE_PAINTING_SEASHROOM_1");
			IMAGE_PAINTING_ICESHROOM_0 = theManager.GetImageThrow("IMAGE_PAINTING_ICESHROOM_0");
			IMAGE_PAINTING_ICESHROOM_1 = theManager.GetImageThrow("IMAGE_PAINTING_ICESHROOM_1");
			IMAGE_PAINTING_UMBRELLA_0 = theManager.GetImageThrow("IMAGE_PAINTING_UMBRELLA_0");
			IMAGE_PAINTING_UMBRELLA_1 = theManager.GetImageThrow("IMAGE_PAINTING_UMBRELLA_1");
			IMAGE_PAINTING_SPLITPEA_0 = theManager.GetImageThrow("IMAGE_PAINTING_SPLITPEA_0");
			IMAGE_PAINTING_SPLITPEA_1 = theManager.GetImageThrow("IMAGE_PAINTING_SPLITPEA_1");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_StoreResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_STORE_BACKGROUND = theManager.GetImageThrow("IMAGE_STORE_BACKGROUND");
			IMAGE_STORE_BACKGROUNDNIGHT = theManager.GetImageThrow("IMAGE_STORE_BACKGROUNDNIGHT");
			IMAGE_STORE_CAR = theManager.GetImageThrow("IMAGE_STORE_CAR");
			IMAGE_STORE_CAR_NIGHT = theManager.GetImageThrow("IMAGE_STORE_CAR_NIGHT");
			IMAGE_STORE_CARCLOSED = theManager.GetImageThrow("IMAGE_STORE_CARCLOSED");
			IMAGE_STORE_CARCLOSED_NIGHT = theManager.GetImageThrow("IMAGE_STORE_CARCLOSED_NIGHT");
			IMAGE_STORE_HATCHBACKOPEN = theManager.GetImageThrow("IMAGE_STORE_HATCHBACKOPEN");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_ZombieNoteResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_ZOMBIE_NOTE = theManager.GetImageThrow("IMAGE_ZOMBIE_NOTE");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_ZombieNote1Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_ZOMBIE_NOTE1 = theManager.GetImageThrow("IMAGE_ZOMBIE_NOTE1");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_ZombieNote2Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_ZOMBIE_NOTE2 = theManager.GetImageThrow("IMAGE_ZOMBIE_NOTE2");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_ZombieNote3Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_ZOMBIE_NOTE3 = theManager.GetImageThrow("IMAGE_ZOMBIE_NOTE3");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_ZombieNote4Resources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_ZOMBIE_NOTE4 = theManager.GetImageThrow("IMAGE_ZOMBIE_NOTE4");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_ZombieFinalNoteResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_ZOMBIE_FINAL_NOTE = theManager.GetImageThrow("IMAGE_ZOMBIE_FINAL_NOTE");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_ZombieNoteHelpResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_ZOMBIE_NOTE_HELP = theManager.GetImageThrow("IMAGE_ZOMBIE_NOTE_HELP");
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_SunflowerResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_SUNFLOWER = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_SUNFLOWER");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonSunflowerAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_JalapenoResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_JALAPENO = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_JALAPENO");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonJalapenoAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_ScaredyshroomResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_SCAREDYSHROOM = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_SCAREDYSHROOM");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonScaredyshroomAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_ChomperResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_CHOMPER = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_CHOMPER");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonChomperAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_RepeaterResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_REPEATER = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_REPEATER");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonRepeaterAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_GarlicResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_GARLIC = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_GARLIC");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonGarlicAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_KernelpultResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_KERNELPULT = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_KERNELPULT");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonKernelpultAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_LilypadResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_LILYPAD = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_LILYPAD");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonLilypadAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_CattailResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_CATTAIL = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_CATTAIL");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonCattailAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_PlanternResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_PLANTERN = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_PLANTERN");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonPlanternAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_GravebusterResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_GRAVEBUSTER = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_GRAVEBUSTER");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonGravebusterAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_BejeweledResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_BEJEWELED = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_BEJEWELED");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonBejeweledAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_SpikeweedResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_SPIKEWEED = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_SPIKEWEED");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonSpikeweedAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_AgaveResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_AGAVE = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_AGAVE");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonAgaveAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_PumpkinResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_PUMPKIN = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_PUMPKIN");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonPumpkinAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_RoseResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_ROSE = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_ROSE");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonRoseAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_SpikerockResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_SPIKEROCK = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_SPIKEROCK");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonSpikerockAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_ImitaterResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_IMITATER = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_IMITATER");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonImitaterAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_SunResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_SUN = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_SUN");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonSunAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_HypnoshroomResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_HYPNOSHROOM = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_HYPNOSHROOM");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonHypnoshroomAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_FumeshroomResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_FUMESHROOM = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_FUMESHROOM");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonFumeshroomAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_PuffshroomResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_PUFFSHROOM = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_PUFFSHROOM");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonPuffshroomAtlasImages();
		return true;
	}

	public static bool ExtractDelayLoad_Cartoon_IceshroomResources(ResourceManager theManager)
	{
		gNeedRecalcVariableToIdMap = true;
		try
		{
			IMAGE_THUMBNAIL_CARTOON_ICESHROOM = theManager.GetImageThrow("IMAGE_THUMBNAIL_CARTOON_ICESHROOM");
		}
		catch
		{
			return false;
		}
		AtlasResources.mAtlasResources.UnpackThumbnailCartoonIceshroomAtlasImages();
		return true;
	}

	public static Image GetImageById(int theId)
	{
		if (theId < 0)
		{
			return null;
		}
		return (Image)gResources[theId];
	}

	public static Font GetFontById(int theId)
	{
		return (Font)gResources[theId];
	}

	public static int GetSoundById(int theId)
	{
		return (int)gResources[theId];
	}

	public static Image GetImageRefById(int theId)
	{
		return (Image)gResources[theId];
	}

	public static Font GetFontRefById(int theId)
	{
		return (Font)gResources[theId];
	}

	public static int GetSoundRefById(int theId)
	{
		return (int)gResources[theId];
	}

	public static ResourceId GetIdByImage(Image theImage)
	{
		return GetIdByVariable(theImage);
	}

	public static ResourceId GetIdByFont(Font theFont)
	{
		return GetIdByVariable(theFont);
	}

	public static ResourceId GetIdBySound(int theSound)
	{
		return GetIdByVariable((IntPtr)theSound);
	}

	public static ResourceId GetIdByStringId(string theStringId)
	{
		return ResourceId.RESOURCE_ID_MAX;
	}

	public static ResourceId GetIdByVariable(object theVariable)
	{
		return ResourceId.RESOURCE_ID_MAX;
	}

	public static void LinkUpResArray()
	{
		gResources = new object[250]
		{
			IMAGE_POPCAP_LOGO, IMAGE_POPCAP_LOGO_REGISTERED, IMAGE_TITLESCREEN, IMAGE_LOADING, IMAGE_PVZ_LOGO, FONT_BRIANNETOD16, SOUND_BUTTONCLICK, SOUND_LOADINGBAR_FLOWER, SOUND_LOADINGBAR_ZOMBIE, FONT_HOUSEOFTERROR16,
			FONT_CONTINUUMBOLD14, FONT_CONTINUUMBOLD14OUTLINE, FONT_DWARVENTODCRAFT12, FONT_DWARVENTODCRAFT15, FONT_DWARVENTODCRAFT18, FONT_PICO129, FONT_BRIANNETOD12, IMAGE_CHARREDZOMBIES, IMAGE_ALMANACUI, IMAGE_SEEDATLAS,
			IMAGE_DAVE, IMAGE_DIALOG, IMAGE_CONVEYORBELT_BACKDROP, IMAGE_CONVEYORBELT_BELT, IMAGE_SPEECHBUBBLE, IMAGE_LOC_EN, IMAGE_ZOMBIE_NOTE_SMALL, IMAGE_REANIM_ZOMBIESWON, IMAGE_SCARY_POT, SOUND_AWOOGA,
			SOUND_BLEEP, SOUND_BUZZER, SOUND_CHOMP, SOUND_CHOMP2, SOUND_CHOMPSOFT, SOUND_FLOOP, SOUND_FROZEN, SOUND_GULP, SOUND_GROAN, SOUND_GROAN2,
			SOUND_GROAN3, SOUND_GROAN4, SOUND_GROAN5, SOUND_GROAN6, SOUND_LOSEMUSIC, SOUND_MINDCONTROLLED, SOUND_PAUSE, SOUND_PLANT, SOUND_PLANT2, SOUND_POINTS,
			SOUND_SEEDLIFT, SOUND_SIREN, SOUND_SLURP, SOUND_SPLAT, SOUND_SPLAT2, SOUND_SPLAT3, SOUND_SUKHBIR4, SOUND_SUKHBIR5, SOUND_SUKHBIR6, SOUND_TAP,
			SOUND_TAP2, SOUND_THROW, SOUND_THROW2, SOUND_BLOVER, SOUND_WINMUSIC, SOUND_LAWNMOWER, SOUND_BOING, SOUND_JACKINTHEBOX, SOUND_DIAMOND, SOUND_DOLPHIN_APPEARS,
			SOUND_DOLPHIN_BEFORE_JUMPING, SOUND_POTATO_MINE, SOUND_ZAMBONI, SOUND_BALLOON_POP, SOUND_THUNDER, SOUND_ZOMBIESPLASH, SOUND_BOWLING, SOUND_BOWLINGIMPACT, SOUND_BOWLINGIMPACT2, SOUND_GRAVEBUSTERCHOMP,
			SOUND_GRAVEBUTTON, SOUND_LIMBS_POP, SOUND_PLANTERN, SOUND_POGO_ZOMBIE, SOUND_SNOW_PEA_SPARKLES, SOUND_PLANT_WATER, SOUND_ZOMBIE_ENTERING_WATER, SOUND_ZOMBIE_FALLING_1, SOUND_ZOMBIE_FALLING_2, SOUND_PUFF,
			SOUND_FUME, SOUND_HUGE_WAVE, SOUND_SLOT_MACHINE, SOUND_COIN, SOUND_ROLL_IN, SOUND_DIGGER_ZOMBIE, SOUND_HATCHBACK_CLOSE, SOUND_HATCHBACK_OPEN, SOUND_KERNELPULT, SOUND_KERNELPULT2,
			SOUND_ZOMBAQUARIUM_DIE, SOUND_BUNGEE_SCREAM, SOUND_BUNGEE_SCREAM2, SOUND_BUNGEE_SCREAM3, SOUND_BUTTER, SOUND_JACK_SURPRISE, SOUND_JACK_SURPRISE2, SOUND_NEWSPAPER_RARRGH, SOUND_NEWSPAPER_RARRGH2, SOUND_NEWSPAPER_RIP,
			SOUND_SQUASH_HMM, SOUND_SQUASH_HMM2, SOUND_VASE_BREAKING, SOUND_POOL_CLEANER, SOUND_MAGNETSHROOM, SOUND_LADDER_ZOMBIE, SOUND_GARGANTUAR_THUMP, SOUND_BASKETBALL, SOUND_FIREPEA, SOUND_IGNITE,
			SOUND_IGNITE2, SOUND_READYSETPLANT, SOUND_DOOMSHROOM, SOUND_EXPLOSION, SOUND_FINALWAVE, SOUND_REVERSE_EXPLOSION, SOUND_RVTHROW, SOUND_SHIELDHIT, SOUND_SHIELDHIT2, SOUND_BOSSEXPLOSION,
			SOUND_CHERRYBOMB, SOUND_BONK, SOUND_SWING, SOUND_RAIN, SOUND_LIGHTFILL, SOUND_PLASTICHIT, SOUND_PLASTICHIT2, SOUND_JALAPENO, SOUND_BALLOONINFLATE, SOUND_BIGCHOMP,
			SOUND_MELONIMPACT, SOUND_MELONIMPACT2, SOUND_PLANTGROW, SOUND_SHOOP, SOUND_JUICY, SOUND_COFFEE, SOUND_WAKEUP, SOUND_LOWGROAN, SOUND_LOWGROAN2, SOUND_PRIZE,
			SOUND_YUCK, SOUND_YUCK2, SOUND_GRASSSTEP, SOUND_SHOVEL, SOUND_COBLAUNCH, SOUND_WATERING, SOUND_POLEVAULT, SOUND_GRAVESTONE_RUMBLE, SOUND_DIRT_RISE, SOUND_FERTILIZER,
			SOUND_PORTAL, SOUND_SCREAM, SOUND_PAPER, SOUND_MONEYFALLS, SOUND_IMP, SOUND_IMP2, SOUND_HYDRAULIC_SHORT, SOUND_HYDRAULIC, SOUND_GARGANTUDEATH, SOUND_CERAMIC,
			SOUND_BOSSBOULDERATTACK, SOUND_CHIME, SOUND_CRAZYDAVESHORT1, SOUND_CRAZYDAVESHORT2, SOUND_CRAZYDAVESHORT3, SOUND_CRAZYDAVELONG1, SOUND_CRAZYDAVELONG2, SOUND_CRAZYDAVELONG3, SOUND_CRAZYDAVEEXTRALONG1, SOUND_CRAZYDAVEEXTRALONG2,
			SOUND_CRAZYDAVEEXTRALONG3, SOUND_CRAZYDAVECRAZY, SOUND_DANCER, SOUND_FINALFANFARE, SOUND_CRAZYDAVESCREAM, SOUND_CRAZYDAVESCREAM2, SOUND_ACHIEVEMENT, SOUND_BUGSPRAY, SOUND_FERTILISER, SOUND_PHONOGRAPH,
			IMAGE_PILE, IMAGE_PLANTSZOMBIES, IMAGE_PARTICLES, IMAGE_SLOTMACHINE_OVERLAY, IMAGE_ZENGARDEN, IMAGE_CACHED, IMAGE_SELECTORSCREEN_ACHIEVEMENTS_TOP_BACKGROUND, IMAGE_SELECTORSCREEN_MAIN_BACKGROUND, IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE, IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE_CHINA,
			IMAGE_SELECTORSCREEN_QUICKPLAY_BACKGROUND, IMAGE_GOODIES, IMAGE_QUICKPLAY, IMAGE_ACHIEVEMENT_GNOME, IMAGE_MINIGAMES, IMAGE_SELECTORSCREEN_MOREGAMES_BACKGROUND, IMAGE_CREDITS_ZOMBIENOTE, IMAGE_LEADERBOARDSCREEN_BACKGROUND, IMAGE_BLACKHOLE, IMAGE_EDGE_OF_SPACE,
			IMAGE_STARS_1, IMAGE_STARS_2, IMAGE_STARS_3, IMAGE_STARS_4, IMAGE_BACKGROUND_GREENHOUSE, IMAGE_AQUARIUM1, IMAGE_BACKGROUND_MUSHROOMGARDEN, IMAGE_BACKGROUND1, IMAGE_BACKGROUND1_GAMEOVER_INTERIOR_OVERLAY, IMAGE_BACKGROUND1_GAMEOVER_MASK,
			IMAGE_BACKGROUND1UNSODDED, IMAGE_BACKGROUND2, IMAGE_BACKGROUND2_GAMEOVER_INTERIOR_OVERLAY, IMAGE_BACKGROUND2_GAMEOVER_MASK, IMAGE_BACKGROUND3, IMAGE_BACKGROUND3_GAMEOVER_INTERIOR_OVERLAY, IMAGE_BACKGROUND3_GAMEOVER_MASK, IMAGE_BACKGROUND4, IMAGE_BACKGROUND4_GAMEOVER_INTERIOR_OVERLAY, IMAGE_BACKGROUND4_GAMEOVER_MASK,
			IMAGE_FOG, IMAGE_BACKGROUND5, IMAGE_BACKGROUND5_GAMEOVER_MASK, IMAGE_BACKGROUND6BOSS, IMAGE_BACKGROUND6_GAMEOVER_MASK, IMAGE_STORE_BACKGROUND, IMAGE_STORE_BACKGROUNDNIGHT, IMAGE_STORE_CAR, IMAGE_STORE_CAR_NIGHT, IMAGE_STORE_CARCLOSED,
			IMAGE_STORE_CARCLOSED_NIGHT, IMAGE_STORE_HATCHBACKOPEN, IMAGE_ZOMBIE_NOTE, IMAGE_ZOMBIE_NOTE1, IMAGE_ZOMBIE_NOTE2, IMAGE_ZOMBIE_NOTE3, IMAGE_ZOMBIE_NOTE4, IMAGE_ZOMBIE_FINAL_NOTE, IMAGE_ZOMBIE_NOTE_HELP, null
		};
	}

	public static string GetStringIdById(int theId)
	{
		return theId switch
		{
			0 => "IMAGE_POPCAP_LOGO", 
			1 => "IMAGE_POPCAP_LOGO_REGISTERED", 
			2 => "IMAGE_TITLESCREEN", 
			3 => "IMAGE_LOADING", 
			4 => "IMAGE_PVZ_LOGO", 
			8 => "FONT_BRIANNETOD16", 
			5 => "SOUND_BUTTONCLICK", 
			6 => "SOUND_LOADINGBAR_FLOWER", 
			7 => "SOUND_LOADINGBAR_ZOMBIE", 
			9 => "FONT_HOUSEOFTERROR16", 
			10 => "FONT_CONTINUUMBOLD14", 
			11 => "FONT_CONTINUUMBOLD14OUTLINE", 
			12 => "FONT_DWARVENTODCRAFT12", 
			13 => "FONT_DWARVENTODCRAFT15", 
			14 => "FONT_DWARVENTODCRAFT18", 
			15 => "FONT_PICO129", 
			16 => "FONT_BRIANNETOD12", 
			17 => "IMAGE_CHARREDZOMBIES", 
			18 => "IMAGE_ALMANACUI", 
			19 => "IMAGE_SEEDATLAS", 
			20 => "IMAGE_DAVE", 
			21 => "IMAGE_DIALOG", 
			22 => "IMAGE_CONVEYORBELT_BACKDROP", 
			23 => "IMAGE_CONVEYORBELT_BELT", 
			24 => "IMAGE_SPEECHBUBBLE", 
			25 => "IMAGE_LOC_EN", 
			26 => "IMAGE_ZOMBIE_NOTE_SMALL", 
			27 => "IMAGE_REANIM_ZOMBIESWON", 
			28 => "IMAGE_SCARY_POT", 
			29 => "SOUND_AWOOGA", 
			30 => "SOUND_BLEEP", 
			31 => "SOUND_BUZZER", 
			32 => "SOUND_CHOMP", 
			33 => "SOUND_CHOMP2", 
			34 => "SOUND_CHOMPSOFT", 
			35 => "SOUND_FLOOP", 
			36 => "SOUND_FROZEN", 
			37 => "SOUND_GULP", 
			38 => "SOUND_GROAN", 
			39 => "SOUND_GROAN2", 
			40 => "SOUND_GROAN3", 
			41 => "SOUND_GROAN4", 
			42 => "SOUND_GROAN5", 
			43 => "SOUND_GROAN6", 
			44 => "SOUND_LOSEMUSIC", 
			45 => "SOUND_MINDCONTROLLED", 
			46 => "SOUND_PAUSE", 
			47 => "SOUND_PLANT", 
			48 => "SOUND_PLANT2", 
			49 => "SOUND_POINTS", 
			50 => "SOUND_SEEDLIFT", 
			51 => "SOUND_SIREN", 
			52 => "SOUND_SLURP", 
			53 => "SOUND_SPLAT", 
			54 => "SOUND_SPLAT2", 
			55 => "SOUND_SPLAT3", 
			56 => "SOUND_SUKHBIR4", 
			57 => "SOUND_SUKHBIR5", 
			58 => "SOUND_SUKHBIR6", 
			59 => "SOUND_TAP", 
			60 => "SOUND_TAP2", 
			61 => "SOUND_THROW", 
			62 => "SOUND_THROW2", 
			63 => "SOUND_BLOVER", 
			64 => "SOUND_WINMUSIC", 
			65 => "SOUND_LAWNMOWER", 
			66 => "SOUND_BOING", 
			67 => "SOUND_JACKINTHEBOX", 
			68 => "SOUND_DIAMOND", 
			69 => "SOUND_DOLPHIN_APPEARS", 
			70 => "SOUND_DOLPHIN_BEFORE_JUMPING", 
			71 => "SOUND_POTATO_MINE", 
			72 => "SOUND_ZAMBONI", 
			73 => "SOUND_BALLOON_POP", 
			74 => "SOUND_THUNDER", 
			75 => "SOUND_ZOMBIESPLASH", 
			76 => "SOUND_BOWLING", 
			77 => "SOUND_BOWLINGIMPACT", 
			78 => "SOUND_BOWLINGIMPACT2", 
			79 => "SOUND_GRAVEBUSTERCHOMP", 
			80 => "SOUND_GRAVEBUTTON", 
			81 => "SOUND_LIMBS_POP", 
			82 => "SOUND_PLANTERN", 
			83 => "SOUND_POGO_ZOMBIE", 
			84 => "SOUND_SNOW_PEA_SPARKLES", 
			85 => "SOUND_PLANT_WATER", 
			86 => "SOUND_ZOMBIE_ENTERING_WATER", 
			87 => "SOUND_ZOMBIE_FALLING_1", 
			88 => "SOUND_ZOMBIE_FALLING_2", 
			89 => "SOUND_PUFF", 
			90 => "SOUND_FUME", 
			91 => "SOUND_HUGE_WAVE", 
			92 => "SOUND_SLOT_MACHINE", 
			93 => "SOUND_COIN", 
			94 => "SOUND_ROLL_IN", 
			95 => "SOUND_DIGGER_ZOMBIE", 
			96 => "SOUND_HATCHBACK_CLOSE", 
			97 => "SOUND_HATCHBACK_OPEN", 
			98 => "SOUND_KERNELPULT", 
			99 => "SOUND_KERNELPULT2", 
			100 => "SOUND_ZOMBAQUARIUM_DIE", 
			101 => "SOUND_BUNGEE_SCREAM", 
			102 => "SOUND_BUNGEE_SCREAM2", 
			103 => "SOUND_BUNGEE_SCREAM3", 
			104 => "SOUND_BUTTER", 
			105 => "SOUND_JACK_SURPRISE", 
			106 => "SOUND_JACK_SURPRISE2", 
			107 => "SOUND_NEWSPAPER_RARRGH", 
			108 => "SOUND_NEWSPAPER_RARRGH2", 
			109 => "SOUND_NEWSPAPER_RIP", 
			110 => "SOUND_SQUASH_HMM", 
			111 => "SOUND_SQUASH_HMM2", 
			112 => "SOUND_VASE_BREAKING", 
			113 => "SOUND_POOL_CLEANER", 
			114 => "SOUND_MAGNETSHROOM", 
			115 => "SOUND_LADDER_ZOMBIE", 
			116 => "SOUND_GARGANTUAR_THUMP", 
			117 => "SOUND_BASKETBALL", 
			118 => "SOUND_FIREPEA", 
			119 => "SOUND_IGNITE", 
			120 => "SOUND_IGNITE2", 
			121 => "SOUND_READYSETPLANT", 
			122 => "SOUND_DOOMSHROOM", 
			123 => "SOUND_EXPLOSION", 
			124 => "SOUND_FINALWAVE", 
			125 => "SOUND_REVERSE_EXPLOSION", 
			126 => "SOUND_RVTHROW", 
			127 => "SOUND_SHIELDHIT", 
			128 => "SOUND_SHIELDHIT2", 
			129 => "SOUND_BOSSEXPLOSION", 
			130 => "SOUND_CHERRYBOMB", 
			131 => "SOUND_BONK", 
			132 => "SOUND_SWING", 
			133 => "SOUND_RAIN", 
			134 => "SOUND_LIGHTFILL", 
			135 => "SOUND_PLASTICHIT", 
			136 => "SOUND_PLASTICHIT2", 
			137 => "SOUND_JALAPENO", 
			138 => "SOUND_BALLOONINFLATE", 
			139 => "SOUND_BIGCHOMP", 
			140 => "SOUND_MELONIMPACT", 
			141 => "SOUND_MELONIMPACT2", 
			142 => "SOUND_PLANTGROW", 
			143 => "SOUND_SHOOP", 
			144 => "SOUND_JUICY", 
			145 => "SOUND_COFFEE", 
			146 => "SOUND_WAKEUP", 
			147 => "SOUND_LOWGROAN", 
			148 => "SOUND_LOWGROAN2", 
			149 => "SOUND_PRIZE", 
			150 => "SOUND_YUCK", 
			151 => "SOUND_YUCK2", 
			152 => "SOUND_GRASSSTEP", 
			153 => "SOUND_SHOVEL", 
			154 => "SOUND_COBLAUNCH", 
			155 => "SOUND_WATERING", 
			156 => "SOUND_POLEVAULT", 
			157 => "SOUND_GRAVESTONE_RUMBLE", 
			158 => "SOUND_DIRT_RISE", 
			159 => "SOUND_FERTILIZER", 
			160 => "SOUND_PORTAL", 
			161 => "SOUND_SCREAM", 
			162 => "SOUND_PAPER", 
			163 => "SOUND_MONEYFALLS", 
			164 => "SOUND_IMP", 
			165 => "SOUND_IMP2", 
			166 => "SOUND_HYDRAULIC_SHORT", 
			167 => "SOUND_HYDRAULIC", 
			168 => "SOUND_GARGANTUDEATH", 
			169 => "SOUND_CERAMIC", 
			170 => "SOUND_BOSSBOULDERATTACK", 
			171 => "SOUND_CHIME", 
			172 => "SOUND_CRAZYDAVESHORT1", 
			173 => "SOUND_CRAZYDAVESHORT2", 
			174 => "SOUND_CRAZYDAVESHORT3", 
			175 => "SOUND_CRAZYDAVELONG1", 
			176 => "SOUND_CRAZYDAVELONG2", 
			177 => "SOUND_CRAZYDAVELONG3", 
			178 => "SOUND_CRAZYDAVEEXTRALONG1", 
			179 => "SOUND_CRAZYDAVEEXTRALONG2", 
			180 => "SOUND_CRAZYDAVEEXTRALONG3", 
			181 => "SOUND_CRAZYDAVECRAZY", 
			182 => "SOUND_DANCER", 
			183 => "SOUND_FINALFANFARE", 
			184 => "SOUND_CRAZYDAVESCREAM", 
			185 => "SOUND_CRAZYDAVESCREAM2", 
			186 => "SOUND_ACHIEVEMENT", 
			187 => "SOUND_BUGSPRAY", 
			188 => "SOUND_FERTILISER", 
			189 => "SOUND_PHONOGRAPH", 
			190 => "IMAGE_PILE", 
			191 => "IMAGE_PLANTSZOMBIES", 
			192 => "IMAGE_PARTICLES", 
			193 => "IMAGE_SLOTMACHINE_OVERLAY", 
			194 => "IMAGE_ZENGARDEN", 
			195 => "IMAGE_CACHED", 
			196 => "IMAGE_SELECTORSCREEN_ACHIEVEMENTS_TOP_BACKGROUND", 
			197 => "IMAGE_SELECTORSCREEN_MAIN_BACKGROUND", 
			198 => "IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE", 
			199 => "IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE_CHINA", 
			200 => "IMAGE_SELECTORSCREEN_QUICKPLAY_BACKGROUND", 
			201 => "IMAGE_GOODIES", 
			202 => "IMAGE_QUICKPLAY", 
			203 => "IMAGE_ACHIEVEMENT_GNOME", 
			204 => "IMAGE_MINIGAMES", 
			205 => "IMAGE_SELECTORSCREEN_MOREGAMES_BACKGROUND", 
			206 => "IMAGE_CREDITS_ZOMBIENOTE", 
			207 => "IMAGE_LEADERBOARDSCREEN_BACKGROUND", 
			208 => "IMAGE_BLACKHOLE", 
			209 => "IMAGE_EDGE_OF_SPACE", 
			210 => "IMAGE_STARS_1", 
			211 => "IMAGE_STARS_2", 
			212 => "IMAGE_STARS_3", 
			213 => "IMAGE_STARS_4", 
			214 => "IMAGE_BACKGROUND_GREENHOUSE", 
			215 => "IMAGE_AQUARIUM1", 
			216 => "IMAGE_BACKGROUND_MUSHROOMGARDEN", 
			217 => "IMAGE_BACKGROUND1", 
			218 => "IMAGE_BACKGROUND1_GAMEOVER_INTERIOR_OVERLAY", 
			219 => "IMAGE_BACKGROUND1_GAMEOVER_MASK", 
			220 => "IMAGE_BACKGROUND1UNSODDED", 
			221 => "IMAGE_BACKGROUND2", 
			222 => "IMAGE_BACKGROUND2_GAMEOVER_INTERIOR_OVERLAY", 
			223 => "IMAGE_BACKGROUND2_GAMEOVER_MASK", 
			224 => "IMAGE_BACKGROUND3", 
			225 => "IMAGE_BACKGROUND3_GAMEOVER_INTERIOR_OVERLAY", 
			226 => "IMAGE_BACKGROUND3_GAMEOVER_MASK", 
			227 => "IMAGE_BACKGROUND4", 
			228 => "IMAGE_BACKGROUND4_GAMEOVER_INTERIOR_OVERLAY", 
			229 => "IMAGE_BACKGROUND4_GAMEOVER_MASK", 
			230 => "IMAGE_FOG", 
			231 => "IMAGE_BACKGROUND5", 
			232 => "IMAGE_BACKGROUND5_GAMEOVER_MASK", 
			233 => "IMAGE_BACKGROUND6BOSS", 
			234 => "IMAGE_BACKGROUND6_GAMEOVER_MASK", 
			235 => "IMAGE_STORE_BACKGROUND", 
			236 => "IMAGE_STORE_BACKGROUNDNIGHT", 
			237 => "IMAGE_STORE_CAR", 
			238 => "IMAGE_STORE_CAR_NIGHT", 
			239 => "IMAGE_STORE_CARCLOSED", 
			240 => "IMAGE_STORE_CARCLOSED_NIGHT", 
			241 => "IMAGE_STORE_HATCHBACKOPEN", 
			242 => "IMAGE_ZOMBIE_NOTE", 
			243 => "IMAGE_ZOMBIE_NOTE1", 
			244 => "IMAGE_ZOMBIE_NOTE2", 
			245 => "IMAGE_ZOMBIE_NOTE3", 
			246 => "IMAGE_ZOMBIE_NOTE4", 
			247 => "IMAGE_ZOMBIE_FINAL_NOTE", 
			248 => "IMAGE_ZOMBIE_NOTE_HELP", 
			_ => "", 
		};
	}
}
