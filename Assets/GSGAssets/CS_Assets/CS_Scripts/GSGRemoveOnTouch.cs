using UnityEngine;
using System.Collections;

namespace GunslingerGame
{
	/// <summary>
	/// This script removes an object when it touches another object
	/// </summary>
	public class GSGRemoveOnTouch:MonoBehaviour 
	{
		// The tag of the object that can be touched
		public string touchTargetTag = "Target";

		// Remove the object we touched
		public bool removeTarget = true;

		// Remove yourself ( this object ) when it touches the target object
		public bool removeSelf = false;

		/// <summary>
		/// Raises the trigger enter2d event. Works only with 2D physics.
		/// </summary>
		/// <param name="other"> The other collider that this object touches</param>
		void OnTriggerEnter2D(Collider2D other) 
		{
			// Check if we touched the correct target
			if ( other.tag == touchTargetTag ) 
			{
				// Remove the object we touched
				if ( removeTarget == true )    Destroy(other.gameObject); 

				// Remove yourself ( this object ) when it touches the target object
				if ( removeSelf == true )    Destroy(gameObject);    
			}
		}
	}
}
