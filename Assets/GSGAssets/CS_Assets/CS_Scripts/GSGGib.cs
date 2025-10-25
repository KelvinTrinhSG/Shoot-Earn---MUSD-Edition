using UnityEngine;
using System.Collections;


namespace GunslingerGame
{
	[RequireComponent (typeof (Rigidbody2D))]

	/// <summary>
	/// This script defines a gib that falls off a target when it break
	/// </summary>
	public class GSGGib:MonoBehaviour 
	{
		// A random horizontal speed within this range
		public Vector2 speedRangeHorizontal = new Vector2(-2,2);

		// A random vertical speed within this range
		public Vector2 speedRangeVertical = new Vector2(2,4);

		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start() 
		{
			// A random horizontal speed within this range
			float speedHorizontal = Random.Range (speedRangeHorizontal.x, speedRangeHorizontal.y);

			// A random vertical speed within this range
			float speedVertical = Random.Range (speedRangeVertical.x, speedRangeVertical.y);

			// Set the velocity based on the random speeds we chose
			GetComponent<Rigidbody2D>().velocity = new Vector2( speedHorizontal, speedVertical);

			// Make the object spin based on the horizontal speed
			GetComponent<Rigidbody2D>().angularVelocity = speedHorizontal * -200;
		}
	}
}
