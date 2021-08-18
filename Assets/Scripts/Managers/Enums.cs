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
}

public enum Tag
{
	PlayerProjectile, // Classic player projectile
	ParticleHitEffects,  // Classic particles from player projectiles, or enemies being hit
	AmmoShell,  // Ammo shells for when player shoots
	EnemyProjectile,  // Classic enemy projectile
	CircleHitEffect, // Classic circle hit effect that shrinks upon enabling,
	SmallExpShell, // Exp shell that gives 1 exp
	CircularEnemyProjectile,   // Circular shaped enemy projectile
	TextObject, // A text object (not in canvas)
	LaserProjectile,  // Instant laser projectile attack
	LaserSight,	// Laser sight to indicate where a laser will shoot from
	CircleHitEffectBig,	// Like the regular one but bigger
}
