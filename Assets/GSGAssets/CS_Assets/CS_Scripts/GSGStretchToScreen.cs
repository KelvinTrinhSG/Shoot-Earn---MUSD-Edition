using UnityEngine;
using System.Collections;

namespace GunslingerGame
{
	/// <summary>
	/// This script stretches a sprite to the width and height of the screen. Good for backgrounds that need to fit all aspect ratios.
	/// </summary>
	public class GSGStretchToScreen:MonoBehaviour 
	{
		// Should the sprite be stretched vertical or horizontal
		public bool stretchVertical = true;
		public bool stretchHorizontal = true;

		// Limit the scale of the sprite, so we don't get sprites that are too destorted
		public Vector2 verticalScaleLimits = new Vector2( 0.8f, 1.2f);
		public Vector2 horizontalScaleLimits = new Vector2( 0.8f, 1.2f);

		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start() 
		{
			// If the sprite stretches both vertically and horizontally, also center it on screen
			if ( stretchVertical == true && stretchHorizontal == true )    transform.position = new Vector3( 0, 0, transform.position.z);

			// Reset the scale of the sprite
			transform.localScale = new Vector3(1,1,1);

			// Calculate the ratio between the the screen width and height
			float screenHeight = Camera.main.orthographicSize * 2.0f;
			float screenWidth = screenHeight/Screen.height * Screen.width;

			// Stretch the sprite vertically and horizontally to fit the screen
			if ( stretchVertical == true )    transform.localScale = new Vector3( transform.localScale.x, screenHeight/GetComponent<SpriteRenderer>().sprite.bounds.size.y, 0);
			if ( stretchHorizontal == true )    transform.localScale = new Vector3( screenWidth/GetComponent<SpriteRenderer>().sprite.bounds.size.x, transform.localScale.y, 0);

			// Limit the vertical scale of the sprite
			if ( transform.localScale.y < verticalScaleLimits.x )    transform.localScale = new Vector3( transform.localScale.x, verticalScaleLimits.x, 0);
			if ( transform.localScale.y > verticalScaleLimits.y )    transform.localScale = new Vector3( transform.localScale.x, verticalScaleLimits.y, 0);

			// Limit the horizontal scale of the sprite
			if ( transform.localScale.x < horizontalScaleLimits.x )    transform.localScale = new Vector3( horizontalScaleLimits.x, transform.localScale.y, 0);
			if ( transform.localScale.x > horizontalScaleLimits.y )    transform.localScale = new Vector3( horizontalScaleLimits.y, transform.localScale.y, 0);
		}
	}
}



