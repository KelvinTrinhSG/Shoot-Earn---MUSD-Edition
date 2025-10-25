using UnityEngine;
using System;

namespace GunslingerGame.Types
{
	/// <summary>
	/// This script defines a touch function. This is used to run functions when an object touches another object.
	/// </summary>
	[Serializable]
	public class TouchFunction
	{
		//The name of the function that will run
		public string functionName = "ChangeScore";
		
		//The tag of the target that the function will run on
		public string targetTag = "GameController";
		
		//A parameter that is passed along with the function
		public float functionParameter = 1;
	}
}