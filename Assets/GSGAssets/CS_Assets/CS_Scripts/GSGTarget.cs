using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GunslingerGame
{
	/// <summary>
	/// This script defines a target that can be shot and bounced and destroyed.
	/// </summary>
	public class GSGTarget:MonoBehaviour 
	{	
		internal GameObject GameController;

		// How much damage this target can take before it breaks
		public float health = 5;

		// The effect that is created at the location of this object when it is destroyed
		public Transform dieEffect;

		// How many points you get when destroying this target.
		public float scoreValue = 100;

		// The bonus effect that shows how much score we got when we destroyed a target
		public Transform scoreEffect;

		// Should this target give a hit bonus ( The hit bonus is the bonus that is given when hitting a target. 
		// The bonus is multiplied based on the number of targets on screen. The base value of the bonus defined in the game controller,
		public bool giveHitBonus = true;

		// Should this target be added to the target count? This allows special items to appear without preventing normal target from appearing.
		public bool addToTargetCount = true;

		// A function that runs when this target is destroyed
		public string dieFunctionName;
		public string dieFunctionTag;
		public float dieFunctionParameter;

		// The sound that plays when this object is hit
		public AudioClip soundHit;

		// The source from which sound for this object play
		public string soundSourceTag = "GameController";

		// The audiosource from which sounds play
		internal GameObject soundSource;

		// A random range for the pitch of the audio source, to make the sound more varied
		public Vector2 pitchRange = new Vector2( 0.9f, 1.1f);

		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start()
		{
			// Hold the gamcontroller object in a variable for quicker access
			GameController = GameObject.FindGameObjectWithTag("GameController");

			// Add to the count of targets on screen
			if ( addToTargetCount == true )    GameController.SendMessage("ChangeTargets", 1);

			//Assign the sound source for easier access
			if ( GameObject.FindGameObjectWithTag(soundSourceTag) )    soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);
		
			// Set the colider to trigger so it doesn't collide with anything
			GetComponent<Collider2D>().isTrigger = true;
		}

		void Update()
		{
			// Only activate the collider if the target is within the game area horizontally ( between leftEdge and rightEdge )
			if ( GetComponent<Collider2D>().isTrigger == true && transform.position.x > Camera.main.ScreenToWorldPoint(Vector3.zero).x && transform.position.x < Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x )
			{
				GetComponent<Collider2D>().isTrigger = false;
			}
		}

		/// <summary>
		/// Hits the target, giving hit bonus and playing a sound.
		/// </summary>
		/// <param name="hitSource">Hit source.</param>
		void HitTarget( Transform hitSource )
		{
			// Give hit bonus
			if ( giveHitBonus == true )    GameController.SendMessage("HitBonus", hitSource);

			// If there is a sound source and a sound assigned, play it
			if ( soundSourceTag != "" && soundHit )    
			{
				//Reset the pitch back to normal
				GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().pitch = Random.Range( pitchRange.x, pitchRange.y);;
				
				//Play the sound
				GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundHit);
			}

			// If there is a source and a sound, play it from the source
			if ( soundSource && soundHit )    soundSource.GetComponent<AudioSource>().PlayOneShot(soundHit);
		}

		/// <summary>
		/// Changes the health. If health reaches 0 the target dies.
		/// </summary>
		/// <param name="changeValue">Change value.</param>
		void ChangeHealth( float changeValue )
		{
			// Change the target's health
			health += changeValue;

			// If target's health is 0, kill it
			if ( health <= 0 )    Die();
		}

		/// <summary>
		/// Kills the target, adds to the score and creates a score effect, and runs a death function if there is one.
		/// </summary>
		void Die()
		{
			// Add to the score
			GameController.SendMessage("ChangeScore", scoreValue);

			// Create a score effect
			if ( scoreEffect )
			{
				// Create a new score effect at the position of the target
				Transform newScoreEffect = Instantiate(scoreEffect, transform.position, Quaternion.identity) as Transform;

				// Display the correct score
				newScoreEffect.Find("Text").GetComponent<Text>().text = "+" + scoreValue.ToString();
			}

			// Run a special function when this target dies
			if ( dieFunctionName != string.Empty && dieFunctionTag != string.Empty )    GameObject.FindGameObjectWithTag(dieFunctionTag).SendMessage(dieFunctionName, dieFunctionParameter);

			// Create a death effect at the position of the target
			Instantiate (dieEffect, transform.position, transform.rotation);

			// Reduce from the count of targets on screen
			if ( addToTargetCount == true )    GameController.SendMessage("ChangeTargets", -1);

			// Remove the object
			Destroy(gameObject);
		}
	}
}
