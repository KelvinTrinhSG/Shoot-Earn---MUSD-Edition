using UnityEngine;
using System.Collections;

namespace GunslingerGame
{
	/// <summary>
	/// This script defines a shot that can hit a target
	/// </summary>
	public class GSGShot:MonoBehaviour 
	{
		// The target that the shot can hit
		public string targetTag = "Target";

		// The vertical and horizontal power of this shot when it hits a target. X is the sideways power, and Y is the bouncing up power 
		public Vector2 shotPower = new Vector2(8,6);

		// How much damage this shot does when it hits a target
		public float shotDamage = 1;

		/// <summary>
		/// Raises the trigger enter2d event. Works only with 2D physics.
		/// </summary>
		/// <param name="other"> The other collider that this object touches</param>
		void OnTriggerEnter2D(Collider2D other) 
		{
			// Check if we hit the correct target
			if ( other.tag == targetTag ) 
			{
				// Run the hit target function on the target ( runs on GSGTarget.cs )
				other.SendMessage("HitTarget", transform);

				// Run the change health function on the target ( runs on GSGTarget.cs )
				other.SendMessage("ChangeHealth", -shotDamage);

				// Bounce the target up and sideways based on shotPower. The target flies sideways based on how close the shot is to the center of the target
				other.GetComponent<Rigidbody2D>().velocity = new Vector2( (other.transform.position.x - transform.position.x) * shotPower.x, shotPower.y);
			
				// Make the target spin based its sideways speed
				other.GetComponent<Rigidbody2D>().angularVelocity = (transform.position.x - other.transform.position.x) * shotPower.x * 100;
			}
		}
	}
}
