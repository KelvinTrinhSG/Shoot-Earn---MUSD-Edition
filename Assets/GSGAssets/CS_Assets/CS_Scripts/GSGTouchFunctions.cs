using UnityEngine;
using System.Collections;
using GunslingerGame.Types;

namespace GunslingerGame
{
	/// <summary>
	/// This script defines an object that will run a function when touching another object
	/// </summary>
	public class GSGTouchFunctions:MonoBehaviour 
	{
		// The tag of the object that will be touched
		public string targetTag = "Target";

		// A list of the functions that run when the object is touched
		public TouchFunction[] touchFunctions;

		/// <summary>
		/// Raises the trigger enter2d event. Works only with 2D physics.
		/// </summary>
		/// <param name="other"> The other collider that this object touches</param>
		void OnTriggerEnter2D(Collider2D other) 
		{
			// If we hit the correct target, run the functions
			if ( other.tag == targetTag ) 
			{
				//Go through the list of functions and run them on the target we touch
				foreach( TouchFunction touchFunction in touchFunctions )
				{
					//Check that we have a target tag and function name before running
					if ( touchFunction.targetTag != string.Empty && touchFunction.functionName != string.Empty )
					{
						//Run the function
						GameObject.FindGameObjectWithTag(touchFunction.targetTag).SendMessage(touchFunction.functionName, touchFunction.functionParameter);
					}
				}
			}
		}
	}
}
