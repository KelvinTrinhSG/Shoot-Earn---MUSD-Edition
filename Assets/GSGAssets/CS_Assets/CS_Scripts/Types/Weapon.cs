using UnityEngine;
using System;

namespace GunslingerGame.Types
{
	/// <summary>
	/// This script defines a weapon in the game. A weapon has a rate of fire, spread ( lack of accuracy ), and the number of pellets in a shot ( like in a shotgun ). You also define the shot object used here.
	/// This class is used in the Game Controller script
	/// </summary>
	[Serializable]
	public class Weapon
	{
		// The shot object that is created when shooting. This is what collides with items.
		public Transform shotObject;

		// How fast the weapon shoots
		public float rateOfFire = 0.5f;
		internal float rateCount = 0;

		// How much the shot spread around the center point ( 0 means perfect accuracy, and a bigger number means less accuracy )
		public float shotSpread = 1;

		// How many pellets are created when shooting. For example this is used for shotguns that shoot several pellets at a time
		public int pelletsPerShot = 1;
		internal int pelletCount = 0;

		// The sound that plays when shooting
		public AudioClip soundShot;
	}
}