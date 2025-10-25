#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GunslingerGame.Types;

namespace GunslingerGame
{
	/// <summary>
	/// This script controls the game, starting it, following game progress, and finishing it with game over.
	/// </summary>
	public class GSGGameController:MonoBehaviour 
	{
		// A list of all the normal targets used in the game
		public Target[] targets;
		internal Target[] targetsList;

		// This class defines an target, and its chance of appearance
		[System.Serializable]
		public class Target
		{
			public Transform targetObject;
			public int spawnChance = 1;
		}

		// The maximum number of targets allowed at the same time
		internal int maximumTargets = 0;

		// The number of targets currently on screen
		internal int targetCount = 0;

		// The height from which targets are thrown up
		public float spawnHeight = -7;

		// How long to wait before creating another target
		public float spawnDelay = 1;
		internal float spawnDelayCount;

		public Target[] specialTargets;
		internal Target[] specialTargetsList;

		// After how many columns does an item appear?
		public int specialTargetRate = 5;
		internal int specialTargetRateCount = 0;

		// A multiplier for the scale of targets
		internal float sizeMultiplier = 1;

		// The angle at which objects are thrown ( It's just a random side velocity given to the thrown object between -10 and 10 for example )
		public float throwAngle = 10;

		// How fast up the target is thrown
		public Vector2 throwSpeed = new Vector2(12,14);

		// How often a target which is twice as fast is thrown. It also has twice the throw angle
		public float fastThrowChance = 0.1f;

		// How often does the target get thrown from the sides rather than the bottom
		public float sideThrowChance = 0.1f;

		// The left and right edges of the game area. Targets bounce off these edges.
		public Transform leftEdge;
		public Transform rightEdge;

		// The number of lives the player has. When the player dies, it loses one life. When lives reach 0, it's game over.
		public int lives = 3;

		// The icon that represents the lives we have. This is animated when we lose a life, and it contains a text that shows how many lives are left
		public Transform livesIcon;

		// A list of weapons the player has access to
		public Weapon[] weapons;

		// The current weapon being used
		public int currentWeapon;

		// The camera that follows the player, and its speed
		public Transform cameraObject;

		// The point at which you aim when shooting. Used for mobile and gamepad/keyboard controls
		public Transform crosshair;
		
		// How fast the crosshair moves
		public float crosshairSpeed = 15;

		// The shoot button, click it or tap it to shoot
		public string shootButton = "Fire1";

		// Are we using the mouse now?
		internal bool usingMouse = false;

		// The position we are aiming at now
		internal Vector3 aimPosition;

		// How many points we get when we hit a target. This bonus is multiplied by the number of targets on screen
		public int hitTargetBonus = 10;
		
		// The bonus effect that shows how much bonus we got when we hit a target
		public Transform bonusEffect;

		// The score and score text of the player
		public int score = 0;
		public Transform scoreText;
		public string scoreTextSuffix = "$";
		internal int highScore = 0;
		internal int bonusMultiplier = 1;

		// The overall game speed
		internal float gameSpeed = 1;
		
		//How many points the player needs to collect before leveling up
		public Level[] levels;
		public int currentLevel = 0;

		// The text that shows the current level/round/stage/etc
		public Text LevelText;
		
		// The prefix of the number of the current level
		public string LevelNamePrefix = "ROUND";

		// Various canvases for the UI
		public Transform gameCanvas;
		public Transform pauseCanvas;
		public Transform gameOverCanvas;

		// Is the game over?
		internal bool  isGameOver = false;
		
		// The level of the main menu that can be loaded after the game ends
		public string mainMenuLevelName = "MainMenu";
		
		// Various sounds and their source
		public AudioClip soundLoseLife;
		public AudioClip soundLevelUp;
		public AudioClip soundGameOver;
		public string soundSourceTag = "GameController";
		internal GameObject soundSource;
		
		// The button that will restart the game after game over
		public string confirmButton = "Submit";
		
		// The button that pauses the game. Clicking on the pause button in the UI also pauses the game
		public string pauseButton = "Cancel";
		internal bool  isPaused = false;

		// A general use index
		internal int index = 0;

		public Transform slowMotionEffect;

		void Awake()
		{
			// Activate the pause canvas early on, so it can detect info about sound volume state
			if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(true);
		}

		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start()
		{
			// Check if we are running on a mobile device. If so, remove the crosshair as we don't need it for taps
			if ( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android )    
			{
				// If a crosshair is assigned, hide it
				if ( crosshair )    crosshair.gameObject.SetActive(false);
				
				crosshair = null;
			}

			//Update the score and lives at the start of the game
			UpdateScore();
			ChangeLives(0);
			
			//Hide the game over and pause screens
			if ( gameOverCanvas )    gameOverCanvas.gameObject.SetActive(false);
			if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(false);

			//Get the highscore for the player
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "HighScore", 0);
			#else
			highScore = PlayerPrefs.GetInt(Application.loadedLevelName + "HighScore", 0);
			#endif

//CALCULATING NORMAL TARGET CHANCES
			// Calculate the chances for the targets to spawn
			int totalTargets = 0;
			int totalTargetsIndex = 0;
			
			// Calculate the total number of targets with their chances
			for( index = 0; index < targets.Length; index++)
			{
				totalTargets += targets[index].spawnChance;
			}
			
			// Create a new list of the objects that can be dropped
			targetsList = new Target[totalTargets];
			
			// Go through the list again and fill out each type of drop based on its drop chance
			for( index = 0; index < targets.Length; index++)
			{
				int laneChanceCount = 0;
				
				while( laneChanceCount < targets[index].spawnChance )
				{
					targetsList[totalTargetsIndex] = targets[index];
					
					laneChanceCount++;
					
					totalTargetsIndex++;
				}
			}

//CALCULATING SPECIAL TARGET CHANCES
			// Calculate the chances for the special targets to spawn
			totalTargets = 0;
			totalTargetsIndex = 0;
			
			// Calculate the total number of special targets with their chances
			for( index = 0; index < specialTargets.Length; index++)
			{
				totalTargets += specialTargets[index].spawnChance;
			}
			
			// Create a new list of the objects that can be dropped
			specialTargetsList = new Target[totalTargets];
			
			// Go through the list again and fill out each type of drop based on its drop chance
			for( index = 0; index < specialTargets.Length; index++)
			{
				int laneChanceCount = 0;
				
				while( laneChanceCount < specialTargets[index].spawnChance )
				{
					specialTargetsList[totalTargetsIndex] = specialTargets[index];
					
					laneChanceCount++;
					
					totalTargetsIndex++;
				}
			}

			//If the player object is not already assigned, Assign it from the "Player" tag
			if ( cameraObject == null )    cameraObject = GameObject.FindGameObjectWithTag("MainCamera").transform;

			// Make sure the left and right edges are positioned correctly
			leftEdge.position = Camera.main.ScreenToWorldPoint(Vector3.zero);
			rightEdge.position = Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width);

			//Assign the sound source for easier access
			if ( GameObject.FindGameObjectWithTag(soundSourceTag) )    soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);

			// Reset the spawn delay
			spawnDelayCount = spawnDelay;

			// Check what level we are on
			UpdateLevel();
		}

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		void  Update()
		{
			//If the game is over, listen for the Restart and MainMenu buttons
			if ( isGameOver == true )
			{
				//The jump button restarts the game
				if ( Input.GetButtonDown(confirmButton) )
				{
					Restart();
				}
				
				//The pause button goes to the main menu
				if ( Input.GetButtonDown(pauseButton) )
				{
					MainMenu();
				}
			}
			else
			{
				// Keyboard and Gamepad controls
				if ( crosshair )
				{
					// If we move the mouse in any direction, then mouse controls take effect
					if ( Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0 || Input.GetMouseButtonDown(0) || Input.touchCount > 0 )    usingMouse = true;

					// We are using the mouse, hide the crosshair
					if ( usingMouse == true )
					{
						// Calculate the mouse/tap position
						aimPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						
						// Make sure it's 2D
						aimPosition.z = 0;
					}

					// If we press gamepad or keyboard arrows, then mouse controls are turned off
					if ( Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 )    
					{
						usingMouse = false;
					}

					// Checking wether the camera is flipped or not. 0 is not flipped, while 180 is flipped
					if ( Mathf.RoundToInt(cameraObject.eulerAngles.z) == 0 )    
					{
						// Move the crosshair based on gamepad/keyboard directions
						aimPosition += new Vector3( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), aimPosition.z) * crosshairSpeed * Time.deltaTime;
					
						// Limit the position of the crosshair to the edges of the screen
						// Limit to the left screen edge
						if ( aimPosition.x < Camera.main.ScreenToWorldPoint(Vector3.zero).x )    aimPosition = new Vector3( Camera.main.ScreenToWorldPoint(Vector3.zero).x, aimPosition.y, aimPosition.z);
						
						// Limit to the right screen edge
						if ( aimPosition.x > Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x )    aimPosition = new Vector3( Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x, aimPosition.y, aimPosition.z);
						
						// Limit to the bottom screen edge
						if ( aimPosition.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y )    aimPosition = new Vector3( aimPosition.x, Camera.main.ScreenToWorldPoint(Vector3.zero).y, aimPosition.z);
						
						// Limit to the top screen edge
						if ( aimPosition.y > Camera.main.ScreenToWorldPoint(Vector3.up * Screen.height).y )    aimPosition = new Vector3( aimPosition.x, Camera.main.ScreenToWorldPoint(Vector3.up * Screen.height).y, aimPosition.z);
					}
					else    
					{
						// Move the crosshair based on gamepad/keyboard directions
						aimPosition -= new Vector3( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), aimPosition.z) * crosshairSpeed * Time.deltaTime;

						// Limit the position of the crosshair to the edges of the screen
						// Limit to the left screen edge
						if ( aimPosition.x > Camera.main.ScreenToWorldPoint(Vector3.zero).x )    aimPosition = new Vector3( Camera.main.ScreenToWorldPoint(Vector3.zero).x, aimPosition.y, aimPosition.z);
						
						// Limit to the right screen edge
						if ( aimPosition.x < Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x )    aimPosition = new Vector3( Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x, aimPosition.y, aimPosition.z);
						
						// Limit to the bottom screen edge
						if ( aimPosition.y > Camera.main.ScreenToWorldPoint(Vector3.zero).y )    aimPosition = new Vector3( aimPosition.x, Camera.main.ScreenToWorldPoint(Vector3.zero).y, aimPosition.z);
						
						// Limit to the top screen edge
						if ( aimPosition.y < Camera.main.ScreenToWorldPoint(Vector3.up * Screen.height).y )    aimPosition = new Vector3( aimPosition.x, Camera.main.ScreenToWorldPoint(Vector3.up * Screen.height).y, aimPosition.z);
					}

					// Place the crosshair at the position of the mouse/tap, with an added offset
					crosshair.position = aimPosition;

					// If we press the shoot button, SHOOT!
					if ( usingMouse == false && Input.GetButtonDown(shootButton) )    Shoot();
				}

				// Make sure we haven't reached the maximum target count
				if ( targetCount < maximumTargets )
				{
					// Count down to the next target spawn
					if ( spawnDelayCount > 0 )    spawnDelayCount -= Time.deltaTime;
					else 
					{
						// Reset the spawn delay count
						spawnDelayCount = spawnDelay;

						// Count the speical target count
						specialTargetRateCount++;

						// Spawn either a normal target or a special target ( Special targets are spawned every X number of normal targets )
						if ( specialTargetRateCount >= specialTargetRate )
						{
							specialTargetRateCount = 0;

							SpawnTarget( specialTargetsList );
						}
						else
						{
							SpawnTarget( targetsList );
						}
					}
				}

				//Toggle pause/unpause in the game
				if ( Input.GetButtonDown(pauseButton) )
				{
					if ( isPaused == true )    Unpause();
					else    Pause();
				}

				// // Count down the rate of fire for all weapons
				for ( index = 0 ; index < weapons.Length ; index++ )
				{
					// Count down the rate of fire
					if ( weapons[currentWeapon].rateCount > 0 )    weapons[currentWeapon].rateCount -= Time.deltaTime;
				}
			}
		}

		/// <summary>
		/// Give a bonus when the target is hit. The bonus is multiplied by the number of targets on screen
		/// </summary>
		/// <param name="hitSource">The target that was hit</param>
		void HitBonus( Transform hitSource )
		{
			// If we have a bonus effect
			if ( bonusEffect )
			{
				// Create a new bonus effect at the hitSource position
				Transform newBonusEffect = Instantiate(bonusEffect, hitSource.position, Quaternion.identity) as Transform;

				// Display the bonus value
				newBonusEffect.Find("Text").GetComponent<Text>().text = "+" + (hitTargetBonus * targetCount * bonusMultiplier).ToString();

				// Rotate the bonus text slightly
				newBonusEffect.eulerAngles = Vector3.forward * Random.Range(-10,10);
			}

			// Add the bonus to the score
			ChangeScore(hitTargetBonus * targetCount * bonusMultiplier);
		}

		/// <summary>
		/// Change the score
		/// </summary>
		/// <param name="changeValue">Change value</param>
		void  ChangeScore( int changeValue )
		{
			score += changeValue;

			//Update the score
			UpdateScore();
		}
		
		/// <summary>
		/// Updates the score value and checks if we got to the next level
		/// </summary>
		void  UpdateScore()
		{
			//Update the score text
			if ( scoreText )    scoreText.GetComponent<Text>().text = score.ToString() + scoreTextSuffix;

			// If we reached the required number of points, level up!
			if ( currentLevel < levels.Length - 1 && score >= levels[currentLevel].scoreToNextLevel )
			{
				LevelUp();
			}

			// Update the progress bar to show how far we are from the next level
			if ( gameCanvas )
			{
				if ( currentLevel == 0 )    gameCanvas.Find("Progress").GetComponent<Image>().fillAmount = score * 1.0f/levels[currentLevel].scoreToNextLevel * 1.0f;
				else    gameCanvas.Find("Progress").GetComponent<Image>().fillAmount = (score - levels[currentLevel - 1].scoreToNextLevel) * 1.0f/(levels[currentLevel].scoreToNextLevel - levels[currentLevel - 1].scoreToNextLevel) * 1.0f;
			}
		}
		
		/// <summary>
		/// Levels up, and increases the difficulty of the game
		/// </summary>
		void  LevelUp()
		{
			currentLevel++;

			UpdateLevel();

			//Run the level up effect, displaying a sound
			LevelUpEffect();
		}

		/// <summary>
		/// Updates the level and sets some values like maximum targets, throw angle, and level text
		/// </summary>
		void UpdateLevel()
		{
			if ( LevelText ) 
			{
				// Display the current level text
				LevelText.text = LevelNamePrefix + " " + (currentLevel + 1).ToString();

				// Play the level animation
				if ( LevelText.GetComponent<Animation>() )    LevelText.GetComponent<Animation>().Play();

				if ( gameCanvas )    gameCanvas.Find("Progress/Text").GetComponent<Text>().text = (currentLevel + 1).ToString();
			}

			// Set the maximum number of targets
			maximumTargets = levels[currentLevel].maximumTargets;

			// Set the throw angle
			throwAngle = levels[currentLevel].throwAngle;
		}

		/// <summary>
		/// Shows the effect associated with leveling up ( a sound and text bubble )
		/// </summary>
		void  LevelUpEffect ()
		{
			//If there is a source and a sound, play it from the source
			if ( soundSource && soundLevelUp )    soundSource.GetComponent<AudioSource>().PlayOneShot(soundLevelUp);
		}

		/// <summary>
		/// Pause the game
		/// </summary>
		void  Pause()
		{
			isPaused = true;
			
			//Set timescale to 0, preventing anything from moving
			Time.timeScale = 0;
			
			//Show the pause screen and hide the game screen
			if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(true);
			if ( gameCanvas )    gameCanvas.gameObject.SetActive(false);
		}
		
		/// <summary>
		/// Resume the game
		/// </summary>
		void  Unpause()
		{
			isPaused = false;
			
			//Set timescale back to the current game speed
			Time.timeScale = gameSpeed;
			
			//Hide the pause screen and show the game screen
			if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(false);
			if ( gameCanvas )    gameCanvas.gameObject.SetActive(true);
		}
		
		/// <summary>
		/// Runs the game over event and shows the game over screen
		/// </summary>
		IEnumerator GameOver(float delay)
		{
			isGameOver = true;

			yield return new WaitForSeconds(delay);
			
			//If there is a source and a sound, play it from the source
			if ( soundSource && soundGameOver )    soundSource.GetComponent<AudioSource>().PlayOneShot(soundGameOver);

			//Remove the pause and game screens
			if ( pauseCanvas )    Destroy(pauseCanvas.gameObject);
			if ( gameCanvas )    Destroy(gameCanvas.gameObject);
			
			//Show the game over screen
			if ( gameOverCanvas )    
			{
				//Show the game over screen
				gameOverCanvas.gameObject.SetActive(true);

				// Tính điểm tổng cộng có cộng thêm VIP NFT bonus
				int finalScore = score + PlayerDataManager.Instance.vipNFT * 10;

				////Write the score text
				//gameOverCanvas.Find("TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();

				// Viết điểm số (score) lên màn hình
				gameOverCanvas.Find("TextScore").GetComponent<Text>().text = "SCORE " + finalScore.ToString();

				////Check if we got a high score
				//if ( score > highScore )    
				//{
				//	highScore = score;

				//	//Register the new high score
				//	#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				//	PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
				//	#else
				//	PlayerPrefs.SetInt(Application.loadedLevelName + "HighScore", score);
				//	#endif
				//}

				// Kiểm tra nếu đây là điểm cao nhất
				if (finalScore > highScore)
				{
					highScore = finalScore;

					// Ghi lại điểm cao mới
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
					PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name + "HighScore", finalScore);
#else
    PlayerPrefs.SetFloat(Application.loadedLevelName + "HighScore", finalScore);
#endif
				}

				//Write the high sscore text
				gameOverCanvas.Find("TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();

				// ✅ Cộng finalScore vào tổng điểm toàn game
				int oldTotal = PlayerPrefs.GetInt("TotalScore", 0);
				int newTotal = oldTotal + finalScore;
				PlayerPrefs.SetInt("TotalScore", newTotal);
				PlayerPrefs.Save();  // Lưu dữ liệu lại
			}
		}
		
		/// <summary>
		/// Restart the current level
		/// </summary>
		void  Restart()
		{
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			#else
			Application.LoadLevel(Application.loadedLevelName);
			#endif
		}
		
		/// <summary>
		/// Restart the current level
		/// </summary>
		void  MainMenu()
		{
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			SceneManager.LoadScene(mainMenuLevelName);
			#else
			Application.LoadLevel(mainMenuLevelName);
			#endif
		}

		/// <summary>
		/// Shoots the current weapon
		/// </summary>
		public void Shoot()
		{
			if ( weapons[currentWeapon].rateCount <= 0 && Time.deltaTime > 0 )
			{
				//Calculate the number of pellets per shot in this weapon
				weapons[currentWeapon].pelletCount = weapons[currentWeapon].pelletsPerShot;

				//Shoot all the pellets in the shot
				while ( weapons[currentWeapon].pelletCount > 0 )
				{
					weapons[currentWeapon].pelletCount--;
					
					// Reset the rate of fire
					weapons[currentWeapon].rateCount = weapons[currentWeapon].rateOfFire;
					
					// Create a new shot at the position of the mouse/tap
					Transform newShot = Instantiate( weapons[currentWeapon].shotObject ) as Transform;

					// If we are using the mouse, make sure we are aiming at the mouse position
					if ( crosshair == false )    aimPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

					// Make sure it's 2D
					aimPosition.z = 0;
					
					// Place the shot at the position of the click, and spread it randomly around the center
					newShot.transform.position = aimPosition + new Vector3( Random.Range(-weapons[currentWeapon].shotSpread,weapons[currentWeapon].shotSpread), Random.Range(-weapons[currentWeapon].shotSpread,weapons[currentWeapon].shotSpread), 0);
				}

				//If there is a source and a sound, play it from the source
				if ( soundSource && weapons[currentWeapon].soundShot )    
				{
					soundSource.GetComponent<AudioSource>().PlayOneShot(weapons[currentWeapon].soundShot);
				}
			}
		}

		/// <summary>
		/// Creates a new target at the bottom of the screen and throws it up 
		/// </summary>
		void SpawnTarget( Target[] currentTargetList )
		{
			// Create a new random target from the target list
			Transform newTarget = Instantiate( currentTargetList[Mathf.FloorToInt(Random.Range(0,currentTargetList.Length))].targetObject ) as Transform;

			// Set the scale of the created object
			newTarget.localScale *= sizeMultiplier;

			// There is a chance for the target to be thrown from the sides, rather than the bottom
			if ( Random.value < sideThrowChance )
			{
				// Choose either the left side or the right side
				if ( Random.value <= 0.5 )
				{
					newTarget.position = new Vector3( leftEdge.position.x - 1, leftEdge.position.y + 4, 0);

					// Set the side throw speed of the target
					newTarget.GetComponent<Rigidbody2D>().velocity = new Vector2( 8, 8);
					
					// And set the rotation speed accordingly
					newTarget.GetComponent<Rigidbody2D>().angularVelocity = throwAngle * -50;
				}
				else
				{
					newTarget.position = new Vector3( rightEdge.position.x + 1, rightEdge.position.y + 4, 0);

					// Set the side throw speed of the target
					newTarget.GetComponent<Rigidbody2D>().velocity = new Vector2( -8, 8);
					
					// And set the rotation speed accordingly
					newTarget.GetComponent<Rigidbody2D>().angularVelocity = throwAngle * -50;
				}
			}
			else
			{
				// Place the target at a random position along the throw height
				newTarget.position = new Vector3( Random.Range(leftEdge.position.x * 1.1f, rightEdge.position.x) * 0.9f, spawnHeight, 0);

				// Give the target a random initial rotation
				newTarget.eulerAngles = Vector3.forward * Random.Range( 0, 360);

				// Choose a random angle to throw the target at
				float tempThrowAngle = Random.Range( -throwAngle, throwAngle);

				// Choose a random speed to throw the target up
				float tempThrowSpeed = Random.Range(throwSpeed.x,throwSpeed.y);

				// There's a chance that the target throw speed and angle and are bigger
				if ( Random.value < fastThrowChance )    
				{
					if ( tempThrowAngle < 0 )    tempThrowAngle -= 4;
					else    tempThrowAngle += 4;

					tempThrowSpeed *= 1.1f;
				}

				// Set the side throw speed of the target
				newTarget.GetComponent<Rigidbody2D>().velocity = new Vector2( tempThrowAngle, tempThrowSpeed);

				// And set the rotation speed accordingly
				newTarget.GetComponent<Rigidbody2D>().angularVelocity = tempThrowAngle * -50;
			}
		}

		/// <summary>
		/// Changes the count of targets on screen
		/// </summary>
		/// <param name="changeValue">Change value.</param>
		void ChangeTargets( int changeValue )
		{
			targetCount += changeValue;
		}

		/// <summary>
		/// Changes the number of lives the player has
		/// </summary>
		/// <param name="changeValue">Change value.</param>
		void ChangeLives( int changeValue )
		{
			if ( isGameOver == false )
			{
				// Change the number of lives
				lives += changeValue;

				// Display the number of lives in text
				livesIcon.Find("Text").GetComponent<Text>().text = lives.ToString();

				// If we lose a life, play the lose life animation
				if ( changeValue < 0 ) 
				{
					if ( livesIcon.GetComponent<Animation>() )     livesIcon.GetComponent<Animation>().Play();

					//If there is a source and a sound, play it from the source
					if ( soundSource && soundLoseLife )    
					{
						soundSource.GetComponent<AudioSource>().pitch = 1;

						soundSource.GetComponent<AudioSource>().PlayOneShot(soundLoseLife);
					}
				}

				// If we have 0 lives left, it's game over
				if ( lives <= 0 ) 
				{
					StartCoroutine(GameOver(1));
				}
			}
		}

		/// <summary>
		/// Slows the game down to 0.5 speed for a few seconds
		/// </summary>
		/// <param name="duration">Duration of slowmotion effect</param>
		IEnumerator SlowMotion(float duration)
		{
			Transform newEffect = null;

			if ( slowMotionEffect )    
			{
				// Create a slow motion effect
				newEffect = Instantiate( slowMotionEffect, Vector3.zero, Quaternion.identity) as Transform;

				// Animate the effect
				if ( newEffect.GetComponent<Animation>() )
				{
					newEffect.GetComponent<Animation>()[newEffect.GetComponent<Animation>().clip.name].speed = 1; 
					newEffect.GetComponent<Animation>().Play(newEffect.GetComponent<Animation>().clip.name);
				}
			}

			// Set the game speed to half
			gameSpeed *= 0.5f;

			// Set the timescale accordingly
			Time.timeScale = gameSpeed;

			// This makes sure the game runs smoothly even in slowmotion. Otherwise you will get clunky physics
			Time.fixedDeltaTime = Time.timeScale * 0.02f;

			// Wait for some time
			yield return new WaitForSeconds(duration);

			// Reverse the slowmotion animation
			if ( slowMotionEffect && newEffect.GetComponent<Animation>() )    
			{
				newEffect.GetComponent<Animation>()[newEffect.GetComponent<Animation>().clip.name].speed = -1; 
				newEffect.GetComponent<Animation>()[newEffect.GetComponent<Animation>().clip.name].time = newEffect.GetComponent<Animation>()[newEffect.GetComponent<Animation>().clip.name].length; 
				newEffect.GetComponent<Animation>().Play(newEffect.GetComponent<Animation>().clip.name);
			}

			// Reset speed back to normal
			gameSpeed *= 2;

			// Set the timescale accordingly
			Time.timeScale = gameSpeed;

			Time.fixedDeltaTime = 0.02f;
		}

		/// <summary>
		/// Mirror the camera for the specified duration.
		/// </summary>
		/// <param name="duration">Duration.</param>
		IEnumerator Mirror(float duration)
		{
			// Set the flipping effect time
			float flipTime = 0.5f;

			// Animate the camera flipping
			while ( flipTime > 0 )
			{
				flipTime -= Time.deltaTime;
				
				cameraObject.eulerAngles = Vector3.Slerp( cameraObject.eulerAngles, Vector3.forward * 180, Time.deltaTime * 10);
				
				yield return new WaitForFixedUpdate();
			}

			// Set the new flipped camera angle
			cameraObject.eulerAngles = Vector3.forward * 180;

			// Wait for some time
			yield return new WaitForSeconds(duration);

			// Set the flipping effect time
			flipTime = 0.5f;

			// Animate the camera flipping back to normal
			while ( flipTime > 0 )
			{
				flipTime -= Time.deltaTime;
				
				cameraObject.eulerAngles = Vector3.Slerp( cameraObject.eulerAngles, Vector3.zero, Time.deltaTime * 10);
				
				yield return new WaitForFixedUpdate();
			}

			// Set the new camera angle back to normal
			cameraObject.eulerAngles = Vector3.zero;
		}

		/// <summary>
		/// Gives double bonus on hits for a few seconds
		/// </summary>
		/// <param name="duration">Duration of effect</param>
		IEnumerator DoubleBonus(float duration)
		{
			// Set the game speed to half
			bonusMultiplier = 2;

			// Wait for some time
			yield return new WaitForSeconds(duration);

			// Reset speed back to normal
			bonusMultiplier = 1;
		}

		/// <summary>
		/// Enlarges all thrown targets to double size
		/// </summary>
		/// <param name="duration">Duration of effect</param>
		IEnumerator Enlarge(float duration)
		{
			// Set the game speed to half
			sizeMultiplier = 2;
			
			// Wait for some time
			yield return new WaitForSeconds(duration);
			
			// Reset speed back to normal
			sizeMultiplier = 1;
		}

		/// <summary>
		/// Shrinks all thrown targets to double size
		/// </summary>
		/// <param name="duration">Duration of effect</param>
		IEnumerator Shrink(float duration)
		{
			// Set the game speed to half
			sizeMultiplier = 0.5f;
			
			// Wait for some time
			yield return new WaitForSeconds(duration);
			
			// Reset speed back to normal
			sizeMultiplier = 1;
		}

	}
}