using UnityEngine;
using System.Collections;
using System;
using System.IO;


public class LoadLevel : MonoBehaviour 
{	
	protected static int levelIndex = 0;
	protected static string[][] levelData;
	protected static int[] levelSequence = {
		0, 1, 2, 0,
	};
	
	public GameObject SpinnerBall;
	public GameObject Darter;
	public GameObject Dolphin;
	public GameObject Langolier;
	public GameObject LittleChopperOrThePeriscopeThatCould;
	public GameObject FinalBoss;
	public GameObject rhDoor;
	public GameObject lhDoor;
	
	public GameObject[] Weapon;
	
	public GameObject Part1;
	public GameObject Part2;
	public GameObject Part3;
	public GameObject Part4;
	public GameObject Part5;
	
	protected GameObject[] parts;

	// Use this for initialization
	void Start () 
	{
		LoadLevelData();
		
		parts = new GameObject[8];
		
		// Hook up parts into an accessible list. IHFTP.
		parts[0] = Part1;
		parts[1] = Part2;
		parts[2] = Part3;
		parts[3] = Part4;
		levelIndex = 0;
	}
	
	public void loadNextLevel()
	{
		BoxCollider playerCollider = (BoxCollider)GameObject.FindWithTag("Player").GetComponent("BoxCollider");
		switch(levelIndex) {
			case 0:
				playerCollider.center = new Vector3(0f, 0f, 0);
				playerCollider.size = new Vector3(1.8f, 0.8f, 1);
				print("0");
				break;
			case 1:
				playerCollider.center = new Vector3(0f, 0.1f, 0);
				playerCollider.size = new Vector3(3f, 2f, 1);
				print("1");
				break;
			case 2:
				playerCollider.center = new Vector3(-0.5f, -0.5f, 0);
				playerCollider.size = new Vector3(4f, 3f, 1);
				break;
			case 3:
				// This is the size of piece 4
				playerCollider.center = new Vector3(-0.6f, 0.6f, 0);
				playerCollider.size = new Vector3(4f, 5.4f, 1);
				// piece 3
				// playerCollider.center = new Vector3(-0.4f, 0f, 0);
				// playerCollider.size = new Vector3(4f, 4.5f, 1);
				// piece 5
				// playerCollider.center = new Vector3(0f, 0.2f, 0);
				// playerCollider.size = new Vector3(5f, 6f, 1);
				break;
			default:
				// For the final boss, the player's collider is now only the vulnerable area.
				// FIXME: handle collisions differently then
				playerCollider.center = new Vector3(-0.8f, 0.3f, 0);
				playerCollider.size = new Vector3(1f, 2f, 1);
				break;
		}
		// Hack because the player is scaled.
		playerCollider.size *= 0.25f;
		
		if (levelIndex >= 4) {
			// Don't attempt to load a level; load a final boss fight instead.
			FinalBoss.SetActiveRecursively(true);
			
			PlayerBossGates bossGates = (PlayerBossGates)(GameObject.FindWithTag("Player").AddComponent("PlayerBossGates"));
			bossGates.finalBoss = FinalBoss;
			
			GateOpener gateOpener = (GateOpener)(GameObject.FindWithTag("Player").AddComponent("GateOpener"));
			gateOpener.leftGate = lhDoor;
			gateOpener.rightGate = rhDoor;
			
			Globals.CurrentBossLife = 100;
			
			return;
		}
		Globals.CurrentBossLife = -1;
		int nextLevel = levelSequence[levelIndex];
        string[] levelLines = levelData[nextLevel];
		
		UnityEngine.Object lastSpawnedEntity = null;
		int countEntitiesSpawned = 0;
		foreach (string text in levelLines) {
			lastSpawnedEntity = loadObject(text);
			countEntitiesSpawned++;
		}
		
		// Spawn the next part after this wave is over.
		Vector3 partPosition = new Vector3(((GameObject)lastSpawnedEntity).transform.position.x+40, 0, 0);
		if (levelIndex != 3) {
			Instantiate(parts[levelIndex], partPosition, Quaternion.identity);
		} else {
			parts[3].transform.position = new Vector3(((GameObject)lastSpawnedEntity).transform.position.x+40, 0.0f, 0.0f);
			parts[3].SetActiveRecursively(true);
		}
		
		// Swap weapons
		((PlayerWeaponControls)(GameObject.FindWithTag("Player").GetComponent("PlayerWeaponControls"))).currentPlayerWeapon = Weapon[levelIndex];

		// Update player health (they get one piece each level, and one bonus health each time)
		PlayerCollisionDetector playerDetector = ((PlayerCollisionDetector)(GameObject.FindWithTag("Player").GetComponent("PlayerCollisionDetector")));
		playerDetector.maxHealth = levelIndex + 1;

		// Next time, load the next level. Not this one. We just beat this one, so that would be extremely silly.
		// Unless it's a particularly good one.
		levelIndex++;
	}
	
	UnityEngine.Object loadObject(string enemyDetails)
	{
		string[] enemyAttributes = enemyDetails.Split(',');
		// Position in the x plane
		Vector3 spawnPosition = new Vector3 (Convert.ToSingle(enemyAttributes[1]), 0.0f, 0.0f);
		
		// Spawn the requested object
		UnityEngine.Object entity = null;
		if (enemyAttributes[0].Equals("enemy1")) {
			entity = Instantiate(SpinnerBall, spawnPosition, Quaternion.identity);
		} else if (enemyAttributes[0].Equals("enemy2")) {
			entity = Instantiate(Darter, spawnPosition, Quaternion.identity);
		} else if (enemyAttributes[0].Equals("enemy3") || true) {
			entity = Instantiate(Dolphin, spawnPosition, Quaternion.identity);
		} else if (enemyAttributes[0].Equals("enemy4")) {
//			entity = Instantiate(Langolier, spawnPosition, Quaternion.identity);
		} else if (enemyAttributes[0].Equals("enemy5") || true) {
//			entity = Instantiate(LittleChopperOrThePeriscopeThatCould, spawnPosition, Quaternion.identity);
		} 
		
		// Assign the specified (OH MY GOD P EEEEEYES) path and speed
		int yEncodedData = (int)(Convert.ToSingle(enemyAttributes[2]));
		int speed = (int)(((yEncodedData % 100)/10.0f) + 3.0f);
		int pathIndex = (yEncodedData / 100);
		
		PrePathPositioningWidget positioningWidget = ((PrePathPositioningWidget)(((GameObject)entity).GetComponent("PrePathPositioningWidget")));
		
		positioningWidget.SpeedOnPath = speed;
		positioningWidget.PathName = GetPathNameForIndex(pathIndex);
		
		return entity;
	}
	
	string GetPathNameForIndex(int index)
	{
		switch(index) 
		{
			case 0: return "SlantMiddle";
			case 1: return "SlantDiagTopLeft";
			case 2: return "SlantFromBehind";
			case 3: return "SlantFromFront";
			case 4: return "SlantDiagBottomLeft";
			case 5: return "StraightTopLeft";
			case 6: return "StraightTopRight";
			case 7: return "SlantDiagTopRight";
			default: 
				return "SlantDiagTopRight";
		}
	}
	
	void LoadLevelData() {
		levelData = new string[4][];
		
		// Level 0
		levelData[0] = new string[] {
			"enemy1,39.072,100.983333333,6.969422,NULL",
			"enemy1,48.72,601.0,6.969422,NULL",
			"enemy1,49.824,501.0,6.969422,NULL",
			"enemy1,55.92,300.633333333,6.969422,NULL",
			"enemy1,57.696,300.633333333,6.969422,NULL",
			"enemy1,59.568,300.633333333,6.969422,NULL",
			"enemy2,66.144,101.0,6.969422,NULL",
			"enemy2,68.448,701.033333333,6.969422,NULL",
			"enemy1,73.776,1.21666666667,6.969422,NULL",
			"enemy2,82.464,1.18333333333,6.969422,NULL",
			"enemy1,91.632,1.25,6.969422,NULL",
			"enemy1,113.52,301.5,6.969422,NULL",
			"enemy1,120.432,301.483333333,6.969422,NULL\n",
		};
		
		// Level 1
		levelData[1] = new string[] {
			"enemy1,31.872,601.4,6.969422,NULL",
			"enemy1,33.072,601.466666667,6.969422,NULL",
			"enemy1,33.984,500.75,6.969422,NULL",
			"enemy2,51.936,101.683333333,6.969422,NULL",
			"enemy2,48.24,101.6,6.969422,NULL",
			"enemy1,60.144,300.383333333,6.969422,NULL",
			"enemy2,65.328,601.25,6.969422,NULL",
			"enemy1,97.44,500.633333333,6.969422,NULL",
			"enemy2,118.8,702.05,6.969422,NULL",
			"enemy2,88.656,301.333333333,6.969422,NULL",
			"enemy1,135.696,500.333333333,6.969422,NULL",
			"enemy2,138.096,600.333333333,6.969422,NULL",
			"enemy2,157.776,701.383333333,6.969422,NULL",
			"enemy1,172.464,500.433333333,6.969422,NULL",
			"enemy1,174.192,500.933333333,6.969422,NULL",
			"enemy1,146.688,300.933333333,6.969422,NULL",
			"enemy2,165.264,601.083333333,6.969422,NULL",
			"enemy2,128.784,601.716666667,6.969422,NULL",
			"enemy2,126.528,501.866666667,6.969422,NULL",
			"enemy1,106.656,601.383333333,6.969422,NULL",
			"enemy1,122.64,301.333333333,6.969422,NULL",
			"enemy1,111.744,501.25,6.969422,NULL",
			"enemy1,114.96,300.533333333,6.969422,NULL",
			"enemy1,133.68,701.4,6.969422,NULL",
			"enemy1,142.32,500.733333333,6.969422,NULL",
			"enemy1,92.928,101.333333333,6.969422,NULL",
			"enemy1,126.192,100.483333333,6.969422,NULL",
			"enemy1,153.36,100.3,6.969422,NULL",
			"enemy1,151.296,101.383333333,6.969422,NULL",
			"enemy1,160.992,300.933333333,6.969422,NULL",
			"enemy1,169.824,101.0,6.969422,NULL",
			"enemy1,181.968,100.833333333,6.969422,NULL",
			"enemy1,177.264,700.716666667,6.969422,NULL",
			"enemy2,195.312,501.233333333,6.969422,NULL",
			"enemy2,199.44,600.5,6.969422,NULL",
			"enemy1,202.08,701.083333333,6.969422,NULL",
			"enemy1,206.688,701.116666667,6.969422,NULL",
			"enemy2,178.752,701.25,6.969422,NULL",
			"enemy2,172.848,301.45,6.969422,NULL",
			"enemy2,208.176,600.466666667,6.969422,NULL",
			"enemy1,209.28,600.466666667,6.969422,NULL",
			"enemy2,210.144,600.4,6.969422,NULL",
			"enemy1,211.344,600.433333333,6.969422,NULL",
			"enemy2,214.944,100.883333333,6.969422,NULL",
			"enemy2,216.624,100.983333333,6.969422,NULL",
			"enemy2,219.504,101.083333333,6.969422,NULL",
			"enemy2,220.752,101.033333333,6.969422,NULL",
			"enemy1,217.824,101.0,6.969422,NULL",
			"enemy1,222.576,501.883333333,6.969422,NULL",
			"enemy1,225.792,601.783333333,6.969422,NULL",
			"enemy1,229.344,701.05,6.969422,NULL",
			"enemy1,235.68,700.966666667,6.969422,NULL",
			"enemy1,239.088,701.433333333,6.969422,NULL",
			"enemy2,230.784,701.25,6.969422,NULL",
			"enemy2,234.048,701.233333333,6.969422,NULL",
			"enemy3,71.472,700.566666667,6.969422,NULL",
			"enemy3,75.744,600.916666667,6.969422,NULL",
			"enemy3,80.928,101.45,6.969422,NULL",
			"enemy3,106.464,700.316666667,6.969422,NULL",
			"enemy3,101.472,100.55,6.969422,NULL",
			"enemy3,187.44,301.716666667,6.969422,NULL",
			"enemy3,188.4,101.15,6.969422,NULL",
			"enemy3,189.648,100.65,6.969422,NULL",
			"enemy3,204.192,701.033333333,6.969422,NULL",
			"enemy3,232.368,701.65,6.969422,NULL",
			"enemy3,237.504,701.333333333,6.969422,NULL",
		};

		// Level 2
		levelData[2] = new string[] {
			"enemy2,54.048,500.866666667,6.969422,NULL",
			"enemy1,54.0,501.533333333,6.969422,NULL",
			"enemy2,56.112,500.9,6.969422,NULL",
			"enemy1,56.304,601.633333333,6.969422,NULL",
			"enemy2,74.256,1.58333333333,6.969422,NULL",
			"enemy1,77.616,501.733333333,6.969422,NULL",
			"enemy1,78.816,600.883333333,6.969422,NULL",
			"enemy3,89.424,101.35,6.969422,NULL",
			"enemy3,94.56,701.333333333,6.969422,NULL",
			"enemy3,99.648,101.35,6.969422,NULL",
			"enemy3,104.544,701.383333333,6.969422,NULL",
			"enemy3,109.344,101.333333333,6.969422,NULL",
			"enemy3,114.576,701.4,6.969422,NULL",
			"enemy2,121.008,301.366666667,6.969422,NULL",
			"enemy2,129.744,301.366666667,6.969422,NULL",
			"enemy1,122.784,600.333333333,6.969422,NULL",
			"enemy1,128.352,500.333333333,6.969422,NULL",
			"enemy1,136.08,500.733333333,6.969422,NULL",
			"enemy1,141.216,1.68333333333,6.969422,NULL",
			"enemy1,144.096,600.533333333,6.969422,NULL",
			"enemy3,150.288,701.716666667,6.969422,NULL",
			"enemy3,155.712,101.716666667,6.969422,NULL",
			"enemy1,159.6,500.65,6.969422,NULL",
			"enemy2,161.712,600.683333333,6.969422,NULL",
			"enemy2,84.0,1.33333333333,6.969422,NULL",
			"enemy3,171.792,500.9,6.969422,NULL",
			"enemy3,173.136,600.966666667,6.969422,NULL",
			"enemy1,175.008,500.9,6.969422,NULL",
			"enemy1,175.968,600.9,6.969422,NULL",
			"enemy2,178.512,500.95,6.969422,NULL",
			"enemy2,179.712,600.933333333,6.969422,NULL",
			"enemy3,187.584,700.983333333,6.969422,NULL",
			"enemy1,203.088,101.083333333,6.969422,NULL",
			"enemy1,204.48,701.083333333,6.969422,NULL",
			"enemy2,209.136,501.783333333,6.969422,NULL",
			"enemy2,211.872,601.783333333,6.969422,NULL",
			"enemy3,222.048,301.533333333,6.969422,NULL",
			"enemy2,224.544,301.433333333,6.969422,NULL",
			"enemy1,227.088,301.383333333,6.969422,NULL",
			"enemy4,42.912,100.633333333,6.969422,NULL",
			"enemy4,44.544,700.716666667,6.969422,NULL",
			"enemy4,46.128,100.666666667,6.969422,NULL",
			"enemy4,47.424,700.716666667,6.969422,NULL",
			"enemy4,48.864,100.666666667,6.969422,NULL",
			"enemy4,50.256,700.666666667,6.969422,NULL",
			"enemy3,63.312,300.95,6.969422,NULL",
			"enemy3,66.192,300.966666667,6.969422,NULL",
			"enemy3,69.36,300.983333333,6.969422,NULL",
			"enemy3,125.184,301.383333333,6.969422,NULL",
			"enemy4,193.824,300.933333333,6.969422,NULL",
			"enemy4,219.072,301.616666667,6.969422,NULL",
			"enemy4,231.936,701.2,6.969422,NULL",
		};

		// Level 3
		levelData[3] = new string[] {
			"enemy1,49.296,500.483333333,6.969422,NULL",
			"enemy1,50.4,600.466666667,6.969422,NULL",
			"enemy1,52.128,500.433333333,6.969422,NULL",
			"enemy1,52.944,600.4,6.969422,NULL",
			"enemy1,55.008,500.433333333,6.969422,NULL",
			"enemy1,56.112,600.433333333,6.969422,NULL",
			"enemy2,61.104,300.9,6.969422,NULL",
			"enemy2,65.424,300.933333333,6.969422,NULL",
			"enemy2,69.552,300.95,6.969422,NULL",
			"enemy2,83.184,101.85,6.969422,NULL",
			"enemy2,84.192,701.883333333,6.969422,NULL",
			"enemy1,87.024,101.966666667,6.969422,NULL",
			"enemy1,87.84,701.883333333,6.969422,NULL",
			"enemy3,90.144,101.983333333,6.969422,NULL",
			"enemy3,91.488,701.933333333,6.969422,NULL",
			"enemy3,101.184,500.7,6.969422,NULL",
			"enemy3,102.912,600.716666667,6.969422,NULL",
			"enemy1,107.808,500.583333333,6.969422,NULL",
			"enemy1,109.296,600.616666667,6.969422,NULL",
			"enemy2,113.04,500.683333333,6.969422,NULL",
			"enemy2,114.96,600.65,6.969422,NULL",
			"enemy1,130.56,1.23333333333,6.969422,NULL",
			"enemy1,134.736,1.25,6.969422,NULL",
			"enemy1,138.48,1.28333333333,6.969422,NULL",
			"enemy1,141.6,1.28333333333,6.969422,NULL",
			"enemy1,145.296,1.23333333333,6.969422,NULL",
			"enemy2,160.848,101.3,6.969422,NULL",
			"enemy2,164.88,101.333333333,6.969422,NULL",
			"enemy2,168.624,101.333333333,6.969422,NULL",
			"enemy2,179.184,701.366666667,6.969422,NULL",
			"enemy2,182.592,701.383333333,6.969422,NULL",
			"enemy2,185.76,701.333333333,6.969422,NULL",
			"enemy3,194.496,302.683333333,6.969422,NULL",
			"enemy3,209.76,302.783333333,6.969422,NULL",
			"enemy1,224.256,101.733333333,6.969422,NULL",
			"enemy1,226.992,701.733333333,6.969422,NULL",
			"enemy2,231.552,502.683333333,6.969422,NULL",
			"enemy2,233.952,602.683333333,6.969422,NULL",
			"enemy5,41.328,703.566666667,6.969422,NULL",
			"enemy5,42.384,103.583333333,6.969422,NULL",
			"enemy5,78.576,101.75,6.969422,NULL",
			"enemy5,80.112,701.75,6.969422,NULL",
			"enemy5,118.992,600.683333333,6.969422,NULL",
			"enemy5,97.104,500.716666667,6.969422,NULL",
			"enemy5,152.064,301.2,6.969422,NULL",
			"enemy5,202.8,302.516666667,6.969422,NULL",
			"enemy5,216.816,302.716666667,6.969422,NULL",
			"enemy2,229.392,101.733333333,6.969422,NULL",
			"enemy2,232.176,701.683333333,6.969422,NULL",
			"enemy3,234.816,101.633333333,6.969422,NULL",
			"enemy3,237.216,701.666666667,6.969422,NULL",
			"enemy4,239.472,101.566666667,6.969422,NULL",
			"enemy4,242.352,701.566666667,6.969422,NULL",
			"enemy1,126.72,1.26666666667,6.969422,NULL",
			"enemy1,123.072,1.28333333333,6.969422,NULL",
		};
	}
}
