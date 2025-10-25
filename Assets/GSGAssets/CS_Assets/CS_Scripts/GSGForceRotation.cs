using UnityEngine;
using System.Collections;

namespace GunslingerGame.Types
{
	/// <summary>
	/// This script forces an object to rotate to a certain angle.
	/// This is used to make sure the popups ( "+Life", "Mirror", "SlowMo", etc ) always show up correctly
	/// </summary>
	public class GSGForceRotation:MonoBehaviour 
	{
		// The correct rotation
		public Vector3 rotation = Vector3.zero;

		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start() 
		{
			// Set the correct rotation
			transform.eulerAngles = rotation;
		}
	}
}
