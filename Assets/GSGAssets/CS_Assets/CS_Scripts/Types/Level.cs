using UnityEngine;
using System;

namespace GunslingerGame.Types
{
	/// <summary>
	/// This script defines a level in the game. When the player reaches a certain score, the level is increased and the difficulty is changed accordingly
	/// This class is used in the Game Controller script
	/// </summary>
	[Serializable]
	public class Level
	{
		// The score needed to win this level
		public int scoreToNextLevel = 500;

		// The sideways throw speed of the targets
		public float throwAngle = 10;

		// The maximum number of targets allowed on screen at once
		public int maximumTargets = 2;
	}
}