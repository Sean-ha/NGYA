using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
	BasicSquareEnemy,
	BasicStationarySquareShooter,
	RedTriangleFlyer
}

public enum Upgrade
{
	// Common upgrades
	MagicBullet = 0,
	FingerlessGloves = 1,
	Lipstick = 2,
	FlimsyString = 3,
	FannyPack = 4,
	Goggles = 5,
	LoveJar = 6,
	VultureClaw = 7,
	BustedToaster = 8,
	LastRegards = 9,
	CannedSoup = 10,
	AbyssalHead = 11,
	SinisterCharm = 12,
	DeadlyBananas = 13,
	Scissors = 14,
	CloakedDagger = 15,
	D6 = 16,
	Thumbtack = 17,
	SwirlyStraw = 18,
	RoyalShield = 19,
	PotLid = 20,
	DopeSunglasses = 21,
	RadCape = 22,


	// Rare upgrades
	StarFragment = 1000,
	GentleQuill = 1001,
	VoltHammer = 1002,
	JumboGeorge = 1003,
	WeirdEyeball = 1004,
	MoonlightScythe = 1005,
	ThunderWand = 1006,
	ShockCollar = 1007,
}

// Deprecated
public enum Upgrade_OLD
{
	Adrenaline = 0,
	Backbone = 1,
	Blowback = 2,
	Brawl = 3,
	DeepBreaths = 4,
	Defender = 5,
	HeavyBullets = 6,
	Inspire = 7,
	Love = 8,
	MagicBullets = 9,
	Resistance = 10,
	Sight = 11,
	Snipe = 12,
	TriggerFinger = 13,
	Unwavering = 14,
	Shrapnel = 15,
	Buddy = 16,
	Sharpness = 17,
	Supplies = 18,
}

public enum Tag
{
	PlayerProjectile, // Classic player projectile. Crit / OnHitEffects can be specified.
	ParticleHitEffects,  // Classic particles from player projectiles, or enemies being hit
	AmmoShell,  // Ammo shells for when player shoots
	EnemyProjectile,  // Classic enemy projectile
	CircleHitEffect, // Classic circle hit effect that shrinks upon enabling,
	SmallExpShell, // Exp shell that gives 1 exp
	CircularEnemyProjectile,   // Circular shaped enemy projectile
	TextObject, // A text object (not in canvas). Used for UI
	EnemyLaserProjectile,  // Enemy instant laser projectile attack
	LaserSight,	// Laser sight to indicate where a laser will shoot from
	CircleHitEffectBig,	// Like the regular one but bigger
	PlayerLaserAbility,	// Default character's special ability projectile (giant laser)
	Tendril,	// Bezier curve
	CritText,	// Text to appear when you crit an enemy -- DEPRECATED
	PlayerProjectileNoCrit_DEPRECATED,	// DEPRECATED
	HomingProjectile,		// Player projectile that follows enemies.
	SinisterCharmStart,	// Sinister charm projectile
	BananaProjectile,	// Homing banana projectile
	Electricity,	// ElectricityEffect
	ExplosionSmall,	// Small explosion effect
	ExplosionLarge,
	PlayerLaserProjectile,	// Any sort of player laser thing
	DodgeText,	// Same as crit text but for dodge instead. Doing it this way b/c it's easier
	GameTextObject,	// Text used for gameplay purposes (e.g. damage numbers) -- on default sorting layer
	ExpandingExplosion,	// Circular explosion with a mask in the center
}
