using UnityEngine;
using System.Collections;

public class GameControllerScript2 : MonoBehaviour
{
	
	//GameObjects
	public GameObject aCube;
	public GameObject pusherCube1;
	public GameObject pusherCube2;
	public Color[] startingColors;
	public int numberOfColors = 6;
	public bool generatingColors = false;
	//CubeArray
	public GameObject[,] cubeArray;
	public int gridWidth = 8;
	public int gridHeight = 5;
	static int gridSpacing = 2;
	//calling other scripts
	public PusherCube1Script pushercube1script;
	public PusherCube2Script pushercube2script;
	public CubeBehaviorScript cubebehaviorscript;
	//pusherCube relat\ed
	public int[] pusherArrayX, pusherArrayY;
	public int pusherCube1Location, pusherCube2Location;
	public int pusherCube1X, pusherCube1Y, pusherCube2X, pusherCube2Y;
	public Vector3[,] pusherArrayVector3;
	public GameObject number1, number2;
	//GUI Related
	public GUIStyle actionButton;
	public GUIStyle timerButton;
	public GUIStyle clickButton;
	public GUIStyle scoreButton;
	//RAYCASTING UGH
	RaycastHit hit;
	public float xToShootRayFrom1,xToShootRayFrom3;
	public float yToShootRayFrom1,yToShootRayFrom3;
	public float xToShootRayFrom2,xToShootRayFrom4;
	public float yToShootRayFrom2,yToShootRayFrom4;
	public Vector3 directionToShoot;
	public int numberOfSameColorCubes;
	public Color colorOfLastCubeHit;
	public bool firstRay1 = true;
	public bool firstRay2 = true;
	public float hitX, hitY;
	public int numCubesHit;
	public LayerMask cyan=8;
	public LayerMask black=9;
	public LayerMask white=10;
	public LayerMask red=11;
	public LayerMask green=12;
	public LayerMask yellow=13;
	public int anotherX, anotherY;
	//Game Phase Related
	public float timer;
	public float timeActionPhase = 4f;
	public float timeLooting = 1f;
	public float timeScoredCubeFadeOut = 0.5f;
	public bool itIsNowThePlanningPhase;
	public bool itIsNowTheActionPhase;
	public bool itIsNowTheResolutionPhase;
	public bool thereAreThingsToLoot;
	public bool canClickCubes;
	public int score;
	//Scoring Related
	public Color thisColor, colorAbove, colorBelow, colorToTheRight, colorToTheLeft, fadeColor;
	public GameObject thisCube, cubeAbove, cubeBelow, cubeToRight, cubeToLeft;
	
	void Start ()
	{
		
		itIsNowTheActionPhase = false;
		itIsNowTheResolutionPhase = false;
		itIsNowThePlanningPhase = true;
		pusherArrayVector3 = new Vector3 [26, 26];
		startingColors = new Color[numberOfColors];
		cubeArray = new GameObject [gridWidth, gridHeight];
		cubebehaviorscript = aCube.GetComponent<CubeBehaviorScript> ();
		for (int x =0; x<gridWidth; x++) {
			for (int y =0; y<gridHeight; y++) {
				cubeArray [x, y] = (GameObject)Instantiate (aCube, new Vector3 (x * gridSpacing - 14, y * gridSpacing - 8, 10), Quaternion.identity);
				cubeArray [x, y].GetComponent<CubeBehaviorScript> ().x = x * gridSpacing - 14;
				cubeArray [x, y].GetComponent<CubeBehaviorScript> ().y = y * gridSpacing - 8;
				startingColors [0] = Color.cyan;
				startingColors [1] = Color.black;
				startingColors [2] = Color.white;
				startingColors [3] = Color.red;
				startingColors [4] = Color.green;
				startingColors [5] = Color.yellow;
				cubeArray [x, y].renderer.material.color = startingColors [(Random.Range (0, 5))];
				//ForLoop to run ColorCrossCheck
//				CheckForSameColorCrossV1(x,y);
//				if(CheckForSameColorCrossV1(x,y)){
//					cubeArray[x,y].renderer.material.color= startingColors[(Random.Range(0,5))];
//				
//					
//					
//				}
				
			}
		}
		generatingColors = false;
		for (int x =1; x<gridWidth-1; x++) {
			for (int y =1; y<gridHeight-1; y++) {
				generatingColors = true;
				ProcessScoring (x, y);
				
			}
		}

		//pusherCube1 begins in the horizontal orientation
		pusherCube1 = (GameObject)Instantiate (pusherCube1, new Vector3 (0, 2, 10), Quaternion.identity);
		pusherCube1Location = 0;
		pusherCube1.renderer.material.color = startingColors [(Random.Range (0, 5))];
		number1 = (GameObject)Instantiate (number1, new Vector3 (0, 2, 8), Quaternion.identity);
		//pusherCube2 begins in the vertical orientation
		pusherCube2 = (GameObject)Instantiate (pusherCube2, new Vector3 (2, 0, 10), Quaternion.identity);
		pusherCube2.renderer.material.color = startingColors [(Random.Range (0, 5))];
		number2 = (GameObject)Instantiate (number2, new Vector3 (2, 0, 8), Quaternion.identity);
		pusherCube2Location = 1;
		
		for (int x =1; x<gridWidth-1; x++) {
			for (int y =1; y<gridHeight-1; y++) {
				generatingColors = true;
				ProcessScoring (x, y);
			}
		}
		generatingColors = false;
	}

	void Update ()
	{
		timer += Time.deltaTime;
		UpdatePusherCubes ();
		ProcessKeyboardInput ();
		ShootDemRays();
//		if(itIsNowTheActionPhase){
//			canClickCubes=true;
//		}
//		if(!itIsNowTheActionPhase){
//			canClickCubes=false;
//		}
		if ((timer > timeActionPhase) && itIsNowTheActionPhase) {
			ProcessResolutionPhase ();
				
		}
		if ((itIsNowTheResolutionPhase)) {
			timer = 0;
			
			
			
			//Check Colors	
			for (int x =1; x<gridWidth-1; x++) {
				for (int y =1; y<gridHeight-1; y++) {
					generatingColors = false;
					fadeColor = Color.magenta;
					ProcessScoring (x, y);
				}
			}
			
			
			//AFTER DONE DOING THINGS
			
			
			ProcessPlanningPhase ();
		}
	}

	void ColorsForScoring (int x, int y)
	{
		thisColor = cubeArray [x, y].renderer.material.color;
		colorAbove = cubeArray [x, y + 1].renderer.material.color;
		colorBelow = cubeArray [x, y - 1].renderer.material.color;
		colorToTheRight = cubeArray [x + 1, y].renderer.material.color;
		colorToTheLeft = cubeArray [x - 1, y].renderer.material.color;
		
	}

	void ProcessScoring (int x, int y)
	{
		ColorsForScoring (x, y);
		VerticalMatch (x, y);
		if (VerticalMatch (x, y)) {
			
			if (generatingColors) {
				cubeArray [x, y].renderer.material.color = startingColors [(Random.Range (0, 5))]/*FadeOut*/;
				cubeArray [x, y + 1].renderer.material.color = startingColors [(Random.Range (0, 5))];
				cubeArray [x, y - 1].renderer.material.color = startingColors [(Random.Range (0, 5))];	
				cubeArray [x + 1, y].renderer.material.color = startingColors [(Random.Range (0, 5))];
				cubeArray [x - 1, y].renderer.material.color = startingColors [(Random.Range (0, 5))];	
			} else if (!generatingColors && !cubebehaviorscript.scored) {
				cubeArray [x, y].renderer.enabled = false/*material.color= fadeColor;*//*FadeOut*/;
				cubeArray [x, y + 1].renderer.enabled = false;/*material.color= fadeColor;*/
				cubeArray [x, y - 1].renderer.enabled = false;/*material.color= fadeColor;*/
				;
				Debug.Log ("VertMatch");
				//Give Points!
				score += 10;	
				cubeArray [x, y].GetComponent<CubeBehaviorScript> ().scored = true;
			
			
				//Shoot a RayCast Up and return the amount of cubes that are the same color. 
				//If the ray hits a color that is not the same, stop it;
				//Shoot a RayCast Down and return the amount of cubes that are the same color. 
				//If the ray hits a color that is not the same, stop it;
			}
		}
		HorizontalMatch (x, y);
		if (HorizontalMatch (x, y)) {
			
			if (generatingColors) {
				cubeArray [x, y].renderer.material.color = startingColors [(Random.Range (0, 5))]/*FadeOut*/;
				cubeArray [x + 1, y].renderer.material.color = startingColors [(Random.Range (0, 5))];
				cubeArray [x - 1, y].renderer.material.color = startingColors [(Random.Range (0, 5))];	
				cubeArray [x, y + 1].renderer.material.color = startingColors [(Random.Range (0, 5))];
				cubeArray [x, y - 1].renderer.material.color = startingColors [(Random.Range (0, 5))];	
			} else if (!generatingColors && !cubebehaviorscript.scored) {
				cubeArray [x, y].renderer.enabled = false/*material.color= fadeColor;*/;
				cubeArray [x + 1, y].renderer.enabled = false/*material.color= fadeColor;*/;
				cubeArray [x - 1, y].renderer.enabled = false/*material.color= fadeColor;*/;
				//Give Points!
				Debug.Log ("HoriMatch");
				score += 10;	
				cubeArray [x, y].GetComponent<CubeBehaviorScript> ().scored = true;
			}
			
			
			//Shoot a RayCast Right and return the amount of cubes that are the same color. 
			//If the ray hits a color that is not the same, stop it;
			//Shoot a RayCast Left and return the amount of cubes that are the same color. 
			//If the ray hits a color that is not the same, stop it;
		}
//		thisCube = cubeArray[x,y];
//		cubeAbove= cubeArray[x,y+1];
//		cubeBelow= cubeArray[x,y-1];
//		cubeToRight= cubeArray[x+1,y];
//		cubeToLeft= cubeArray [x-1,y];
		
	}

	public bool VerticalMatch (int x, int y)
	{
		
		if ((thisColor ==
			colorAbove) && 
			thisColor ==
			colorBelow) {
//			Ray rayUp = new Ray (cubeArray[x,y].transform.position, Vector3.up);
//			Ray rayDown = new Ray (cubeArray[x,y].transform.position,Vector3.down);
//			if(Physics.Raycast (rayUp,100f)/*number of active cubes in direction times 2 */){
//				Debug.DrawRay(transform.position, transform.TransformDirection (Vector3.forward) * hit.distance, Color.yellow);
//			}
//			if(Physics.Raycast (rayDown,100f)){
//				Debug.DrawRay(transform.position, transform.TransformDirection (Vector3.forward) * hit.distance, Color.yellow);
//			}
			return true;
			
		} else {
			return false;
		}
	}

	public bool HorizontalMatch (int x, int y)
	{
		
		if ((thisColor ==
			colorToTheRight) && 
			thisColor ==
			colorToTheRight) {
			return true;
			
		} else {
			return false;
		}
	}

	void ProcessKeyboardInput ()
	{
		if (Input.GetKeyDown (KeyCode.DownArrow) && itIsNowThePlanningPhase) {
			if (pusherCube2Location == 25) {	
				pusherCube2Location = 0;
				if (pusherCube2Location == pusherCube1Location) {
					pusherCube2Location = pusherCube1Location + 1;
				}			
			} else if (pusherCube2Location < 25) {
				pusherCube2Location++;
				if (pusherCube2Location == pusherCube1Location) {
					pusherCube2Location = pusherCube1Location + 1;
				}	
				
			}
				
		}


		if (Input.GetKeyDown (KeyCode.UpArrow) && itIsNowThePlanningPhase) {
			if (pusherCube2Location == 0) {	
				pusherCube2Location = 25;
								
			} else if (pusherCube2Location > 0) {
				pusherCube2Location--;
				if (pusherCube2Location == pusherCube1Location) {
					pusherCube2Location = 25;
				}	
			}
				
		}	
			
		if (Input.GetKeyDown (KeyCode.LeftArrow) && itIsNowThePlanningPhase) {
			if (pusherCube1Location == 0) {	
				pusherCube1Location = 25;
								
			} else if (pusherCube1Location > 0) {
				pusherCube1Location--;
				if (pusherCube1Location == pusherCube2Location) {
					pusherCube1Location = pusherCube2Location - 1;
				}	
			}
				
		
		}

		if (Input.GetKeyDown (KeyCode.RightArrow) && itIsNowThePlanningPhase) {
			if (pusherCube1Location == 25) {	
				pusherCube1Location = 0;
				if (pusherCube1Location == pusherCube2Location) {
					pusherCube1Location = pusherCube2Location + 1;
				}				
			} else if (pusherCube1Location < 25) {
				pusherCube1Location++;
				if (pusherCube1Location == pusherCube2Location) {
					pusherCube1Location = pusherCube2Location + 1;
				}	
				
			}

                
					

		}
				

		if (Input.GetKeyDown (KeyCode.Space) && itIsNowThePlanningPhase == true) {
			ProcessActionPhase ();
		} else if (Input.GetKeyDown (KeyCode.Space) && itIsNowTheActionPhase == true) {
			ProcessResolutionPhase ();
		} else if (Input.GetKeyDown (KeyCode.Space) && itIsNowTheResolutionPhase == true) {
			ProcessPlanningPhase ();
		} 
		
			
				

	}

	void OnGUI ()
	{
		
		if (itIsNowThePlanningPhase) {	
			if ((GUI.Button (new Rect (75, 10, 100f, 25f), "Action Phase", actionButton))) {
				ProcessActionPhase ();	
			
			
			}
		}
		if ((itIsNowTheActionPhase) && timer < timeActionPhase) {
			if ((GUI.Button (new Rect (75, 10, 100, 25), "Time:" + timer, timerButton))) {
			
			}
			if ((GUI.Button (new Rect (75, 40, 300, 25), "You may click cubes to destroy them!", clickButton))) {	
				
			}
		}
		if (itIsNowTheResolutionPhase) {	
			
		}
		if ((GUI.Button (new Rect (75, 70, 300, 25), "Score:" + score, scoreButton))) {
			
		}
	}

	void UpdatePusherCubes ()
	{


		pusherArrayX = new int [26];
		pusherArrayY = new int [26];
		SetPusherCubeArray ();
		pusherCube1X = pusherArrayX [pusherCube1Location];
		pusherCube1Y = pusherArrayY [pusherCube1Location];
		pusherCube2X = pusherArrayX [pusherCube2Location];
		pusherCube2Y = pusherArrayY [pusherCube2Location];
		//pusherCube1Location = new int [pusherCube1X,pusherCube1Y];
		//pusherCube2Location = new int [pusherCube2X,pusherCube2Y];
		pusherCube1.GetComponent<PusherCube1Script> ().x = pusherCube1X;
		pusherCube1.GetComponent<PusherCube1Script> ().y = pusherCube1Y;
		pusherCube2.GetComponent<PusherCube2Script> ().x = pusherCube2X;
		pusherCube2.GetComponent<PusherCube2Script> ().y = pusherCube2Y;
		pusherCube1.transform.position = new Vector3 (pusherCube1.GetComponent<PusherCube1Script> ().x, pusherCube1.GetComponent<PusherCube1Script> ().y, pusherCube1.transform.position.z);
		number1.transform.position = new Vector3 (pusherCube1.GetComponent<PusherCube1Script> ().x, pusherCube1.GetComponent<PusherCube1Script> ().y, 8);
		pusherCube2.transform.position = new Vector3 (pusherCube2.GetComponent<PusherCube2Script> ().x, pusherCube2.GetComponent<PusherCube2Script> ().y, pusherCube2.transform.position.z);
		number2.transform.position = new Vector3 (pusherCube2.GetComponent<PusherCube2Script> ().x, pusherCube2.GetComponent<PusherCube2Script> ().y, 8);
		
	}

	void ProcessPlanningPhase ()
	{
		itIsNowThePlanningPhase = true;
		itIsNowTheActionPhase = false;
		itIsNowTheResolutionPhase = false;
		print ("It's the planing phase");
		
//                if(Input.GetKeyDown (KeyCode.Space)){
//                        ProcessActionPhase();
//                }
//                else{
//                print ("this is the planning phase");
//                }
		
	}

	void ProcessActionPhase ()
	{
		itIsNowThePlanningPhase = false;
		itIsNowTheActionPhase = true;
		itIsNowTheResolutionPhase = false;
		canClickCubes = true;
		print ("It's the action phase");
		timer = 0;
			
		//action button disappears
		//a timer appears going from 4.0 seconds.
		
	
		//instructional text = string("you may click on cubes to destroy them")
		//}
//
//                if(Input.GetKeyDown (KeyCode.Space)){
//                        ProcessResolutionPhase();
//                }
//                else{
//                print ("this is the action phase");
//                }
	}

	void ProcessResolutionPhase ()
	{
		itIsNowThePlanningPhase = false;
		itIsNowTheActionPhase = false;
		itIsNowTheResolutionPhase = true;
		canClickCubes = false;
		print ("It's the resolution phase");
		//Score cubes
		//remove scored cubes
		
		//countdown timer disappears
		//instructional text disappears
		//pusher 1 does its thing
		//pusher 2 does its thing
		//ProcessLooting();
		//scoring
		//emptyspace gets filled up
		//go back to scoring if need be
//
//                if(Input.GetKeyDown (KeyCode.Space)){
//                        ProcessPlanningPhase();
//                }
//                else{
//                print ("this is the resolution phase");
//                }
	}

	void ProcessLooting ()
	{
		//do a bunch of looting things
		//max of two loots per turn
		print ("loot!");
	}

	void SetPusherCubeArray ()
	{
		//X Locations for Pusher Array
		pusherArrayX = new int[26];
		pusherArrayY = new int[26];
		pusherArrayX [0] = 0;
		pusherArrayX [1] = 2;
		pusherArrayX [2] = 2;
		pusherArrayX [3] = 2;
		pusherArrayX [4] = 2;
		pusherArrayX [5] = 2;
		pusherArrayX [6] = 0;
		pusherArrayX [7] = -2;
		pusherArrayX [8] = -4;
		pusherArrayX [9] = -6;
		pusherArrayX [10] = -8;
		pusherArrayX [11] = -10;
		pusherArrayX [12] = -12;
		pusherArrayX [13] = -14;
		pusherArrayX [14] = -16;
		pusherArrayX [15] = -16;
		pusherArrayX [16] = -16;
		pusherArrayX [17] = -16;
		pusherArrayX [18] = -16;
		pusherArrayX [19] = -14;
		pusherArrayX [20] = -12;
		pusherArrayX [21] = -10;
		pusherArrayX [22] = -8;
		pusherArrayX [23] = -6;
		pusherArrayX [24] = -4;
		pusherArrayX [25] = -2;
		//Y Locations for Pusher Array
		pusherArrayY [0] = 2;
		pusherArrayY [1] = 0;
		pusherArrayY [2] = -2;
		pusherArrayY [3] = -4;
		pusherArrayY [4] = -6;
		pusherArrayY [5] = -8;
		pusherArrayY [6] = -10;
		pusherArrayY [7] = -10;
		pusherArrayY [8] = -10;
		pusherArrayY [9] = -10;
		pusherArrayY [10] = -10;
		pusherArrayY [11] = -10;
		pusherArrayY [12] = -10;
		pusherArrayY [13] = -10;
		pusherArrayY [14] = -8;
		pusherArrayY [15] = -6;
		pusherArrayY [16] = -4;
		pusherArrayY [17] = -2;
		pusherArrayY [18] = 0;
		pusherArrayY [19] = 2;
		pusherArrayY [20] = 2;
		pusherArrayY [21] = 2;
		pusherArrayY [22] = 2;
		pusherArrayY [23] = 2;
		pusherArrayY [24] = 2;
		pusherArrayY [25] = 2;
		
	}

//	void ParentSomeCubes ()
//	{
//		GameObject[] children;
//		int targetX = pusher.GetComponent<CubeBehaviorScript> ().thisX, targetY = pusher.GetComponent<CubeBehaviorScript> ().thisY;
//		CastRayFromPusher1();
//		CastRayFromPusher2();
//	}

	void CastRayFromPusher1 ()
	{
		if (pusherCube1) {
			//if(pusherCube2Location =
			//Get location in Pusher Array	
			if ((pusherCube1Location ==19) ||
				(pusherCube1Location == 20) ||
					(pusherCube1Location == 21) ||
					(pusherCube1Location == 22) ||
					(pusherCube1Location == 23) ||
					(pusherCube1Location == 24) ||
					(pusherCube1Location == 25) ||
					(pusherCube1Location == 0)) {
				//Parent cubes down and move all cubes in this row down 2 spaces
				print ("PusherCube1 is in the top row");
				if(pusherCube1Location == 19){
					xToShootRayFrom1 = -14;
					yToShootRayFrom1 = 2;
					ShootRayDown();
				}
				if (pusherCube1Location == 20) {
					xToShootRayFrom1 = -12;
					yToShootRayFrom1 = 2;
					ShootRayDown();
				}
				if (pusherCube1Location == 21) {
					xToShootRayFrom1 = - 10;
					yToShootRayFrom1 = 2;
					ShootRayDown();
				}
				if (pusherCube1Location == 22) {
					xToShootRayFrom1 = -8;
					yToShootRayFrom1 = 2;
					ShootRayDown();
					//Shoot Ray Down
				}
				if (pusherCube1Location == 23) {
					xToShootRayFrom1 = -6;
					yToShootRayFrom1 = 2;
					ShootRayDown();
				}
				if (pusherCube1Location == 24) {
					xToShootRayFrom1 = -4;
					yToShootRayFrom1 = 2;
					ShootRayDown();
				}
				if (pusherCube1Location == 25) {
					xToShootRayFrom1 = -2;
					yToShootRayFrom1 = 2;
					ShootRayDown();
				}
				if (pusherCube1Location == 26) {
					xToShootRayFrom1 = -2;
					yToShootRayFrom1 = 2;
					ShootRayDown();
				}
				if (pusherCube1Location == 0) {
					xToShootRayFrom1 = 0;
					yToShootRayFrom1 = 2;
					ShootRayDown();
				}
			}
			if ((pusherCube1Location == 18) ||
					(pusherCube1Location == 17) ||
					(pusherCube1Location == 16) ||
					(pusherCube1Location == 15) ||
					(pusherCube1Location == 14)) { 	
				//Parent cubes right and move all cubes in this row right 2 spaces
				print ("PusherCube1 is in the left row");
				if (pusherCube1Location == 19) {
					xToShootRayFrom1 = -14;
					yToShootRayFrom1 = 2;
					ShootRayRight();
				}
				if (pusherCube1Location == 18) {
					xToShootRayFrom1 = -16;
					yToShootRayFrom1 = 0;
					ShootRayRight();
				}
				if (pusherCube1Location == 17) {
					xToShootRayFrom1 = -16;
					yToShootRayFrom1 = -2;
					ShootRayRight();
				}
				if (pusherCube1Location == 16) {
					xToShootRayFrom1 = -16;
					yToShootRayFrom1 = -4;
					ShootRayRight();
				}
				if (pusherCube1Location == 15) {
					xToShootRayFrom1 = -16;
					yToShootRayFrom1 = -6;
					ShootRayRight();
				}
				if (pusherCube1Location == 14) {
					xToShootRayFrom1 = -16;
					yToShootRayFrom1 = -8;
					ShootRayRight();
				}
			}
			if ((pusherCube1Location == 13) ||
					(pusherCube1Location == 12) ||
					(pusherCube1Location == 11) ||
					(pusherCube1Location == 10) ||
					(pusherCube1Location == 9) ||
					(pusherCube1Location == 8) ||
					(pusherCube1Location == 7) ||
					(pusherCube1Location == 6)) {
				//Parent cubes up and move all cubes in this row up 2 spaces
				print ("PusherCube1 is in the bottom row");
				if (pusherCube1Location == 13) {
					xToShootRayFrom1 = -14;
					yToShootRayFrom1 = -10;
					ShootRayUp ();
				}
				if (pusherCube1Location == 12) {
					xToShootRayFrom1 = -12;
					yToShootRayFrom1 = -10;
					ShootRayUp ();
				}
				if (pusherCube1Location == 11) {
					xToShootRayFrom1 = -10;
					yToShootRayFrom1 = -10;
					ShootRayUp ();
				}
				if (pusherCube1Location == 10) {
					xToShootRayFrom1 = -8;
					yToShootRayFrom1 = -10;
					ShootRayUp ();
				}
				if (pusherCube1Location == 9) {
					xToShootRayFrom1 = -6;
					yToShootRayFrom1 = -10;
					ShootRayUp ();
				}
				if (pusherCube1Location == 8) {
					xToShootRayFrom1 = -4;
					yToShootRayFrom1 = -10;
					ShootRayUp ();
				}
				if (pusherCube1Location == 7) {
					xToShootRayFrom1 = -2;
					yToShootRayFrom1 = -10;
					ShootRayUp ();
				}
				if (pusherCube1Location == 6) {
					xToShootRayFrom1 = 0;
					yToShootRayFrom1 = -10;
					ShootRayUp ();
				}
			}
			if ((pusherCube1Location == 5) ||
					(pusherCube1Location == 4) ||
					(pusherCube1Location == 3) ||
					(pusherCube1Location == 2) ||
					(pusherCube1Location == 1)) { 	
				//Parent cubes left and move all cubes in this row left 2 spaces
				print ("PusherCube1 is in the right row");
				if (pusherCube1Location == 5) {
					xToShootRayFrom1 = 2;
					yToShootRayFrom1 = -8;
					ShootRayLeft ();
				}
				if (pusherCube1Location == 4) {
					xToShootRayFrom1 = 2;
					yToShootRayFrom1 = -6;
					ShootRayLeft ();
				}
				if (pusherCube1Location == 3) {
					xToShootRayFrom1 = 2;
					yToShootRayFrom1 = -4;
					ShootRayLeft ();
				}
				if (pusherCube1Location == 2) {
					xToShootRayFrom1 = 2;
					yToShootRayFrom1 = -2;
					ShootRayLeft ();
				}
				if (pusherCube1Location == 1) {
					xToShootRayFrom1 = 2;
					yToShootRayFrom1 = 0;
					ShootRayLeft ();
				}
			}
		}
	}

	void CastRayFromPusher2 ()
	{
		if (pusherCube2) {	
			//if(pusherCube2Location =
			//Get location in Pusher Array	
			if ((pusherCube2Location == 20) ||
					(pusherCube2Location == 21) ||
					(pusherCube2Location == 22) ||
					(pusherCube2Location == 23) ||
					(pusherCube2Location == 24) ||
					(pusherCube2Location == 25) ||
					(pusherCube2Location == 26) ||
					(pusherCube2Location == 0)) {
				//Parent cubes down and move all cubes in this row down 2 spaces
				print ("PusherCube2 is in the top row");
				if (pusherCube2Location == 20) {
					xToShootRayFrom2 = -14;
					yToShootRayFrom2 = 2;
					ShootRayDown();
				}
				if (pusherCube2Location == 21) {
					xToShootRayFrom2 = - 12;
					yToShootRayFrom2 = 2;
					ShootRayDown();
				}
				if (pusherCube2Location == 22) {
					xToShootRayFrom2 = -10;
					yToShootRayFrom2 = 2;
					ShootRayDown();
				}
				if (pusherCube2Location == 23) {
					xToShootRayFrom2 = -8;
					yToShootRayFrom2 = 2;
					ShootRayDown();
				}
				if (pusherCube2Location == 24) {
					xToShootRayFrom2 = -6;
					yToShootRayFrom2 = 2;
					ShootRayDown();
				}
				if (pusherCube2Location == 25) {
					xToShootRayFrom2 = -4;
					yToShootRayFrom2 = 2;
					ShootRayDown();
				}
				if (pusherCube2Location == 26) {
					xToShootRayFrom2 = -2;
					yToShootRayFrom2 = 2;
					ShootRayDown();
				}
				if (pusherCube2Location == 0) {
					xToShootRayFrom2 = 0;
					yToShootRayFrom2 = 2;
					ShootRayDown();
				}
			}
			if ((pusherCube2Location == 19) ||
					(pusherCube2Location == 18) ||
					(pusherCube2Location == 17) ||
					(pusherCube2Location == 16) ||
					(pusherCube2Location == 15) ||
					(pusherCube2Location == 14)) { 	
				//Parent cubes right and move all cubes in this row right 2 spaces
				print ("PusherCube2 is in the left row");
				if (pusherCube2Location == 19) {
					xToShootRayFrom2 = -16;
					yToShootRayFrom2 = 0;
					ShootRayRight();
				}
				if (pusherCube2Location == 18) {
					xToShootRayFrom2 = -16;
					yToShootRayFrom2 = -2;
					ShootRayRight();
				}
				if (pusherCube2Location == 17) {
					xToShootRayFrom2 = -16;
					yToShootRayFrom2 = -4;
					ShootRayRight();
				}
				if (pusherCube2Location == 16) {
					xToShootRayFrom2 = -16;
					yToShootRayFrom2 = -6;
					ShootRayRight();
				}
				if (pusherCube2Location == 15) {
					xToShootRayFrom2 = -16;
					yToShootRayFrom2 = -8;
					ShootRayRight();
				}
				if (pusherCube2Location == 14) {
					xToShootRayFrom2 = -16;
					yToShootRayFrom2 = -8;
					ShootRayRight();
				}
			}
			if ((pusherCube2Location == 13) ||
					(pusherCube2Location == 12) ||
					(pusherCube2Location == 11) ||
					(pusherCube2Location == 10) ||
					(pusherCube2Location == 9) ||
					(pusherCube2Location == 8) ||
					(pusherCube2Location == 7) ||
					(pusherCube2Location == 6)) {
				//Parent cubes up and move all cubes in this row up 2 spaces
				print ("PusherCube2 is in the bottom row");
				if (pusherCube2Location == 13) {
					xToShootRayFrom2 = -14;
					yToShootRayFrom2 = -10;
					ShootRayUp();
				}
				if (pusherCube2Location == 12) {
					xToShootRayFrom2 = -12;
					yToShootRayFrom2 = -10;
					ShootRayUp();
				}
				if (pusherCube2Location == 11) {
					xToShootRayFrom2 = -10;
					yToShootRayFrom2 = -10;
					ShootRayUp();
				}
				if (pusherCube2Location == 10) {
					xToShootRayFrom2 = -8;
					yToShootRayFrom2 = -10;
					ShootRayUp();
				}
				if (pusherCube2Location == 9) {
					xToShootRayFrom2 = -6;
					yToShootRayFrom2 = -10;
					ShootRayUp();
				}
				if (pusherCube2Location == 8) {
					xToShootRayFrom2 = -4;
					yToShootRayFrom2 = -10;
					ShootRayUp();
				}
				if (pusherCube2Location == 7) {
					xToShootRayFrom2 = -2;
					yToShootRayFrom2 = -10;
					ShootRayUp();
				}
				if (pusherCube2Location == 6) {
					xToShootRayFrom2 = 0;
					yToShootRayFrom2 = -10;
					ShootRayUp();
				}
			}
			if ((pusherCube2Location == 5) ||
					(pusherCube2Location == 4) ||
					(pusherCube2Location == 3) ||
					(pusherCube2Location == 2) ||
					(pusherCube2Location == 1)) { 	
				//Parent cubes left and move all cubes in this row left 2 spaces
				print ("PusherCube2 is in the right row");
				if (pusherCube2Location == 5) {
					xToShootRayFrom2 = 2;
					yToShootRayFrom2 = -8;
					ShootRayLeft ();
				}
				if (pusherCube2Location == 4) {
					xToShootRayFrom2 = 2;
					yToShootRayFrom2 = -6;
					ShootRayLeft ();
				}
				if (pusherCube2Location == 3) {
					xToShootRayFrom2 = 2;
					yToShootRayFrom2 = -4;
					ShootRayLeft ();
				}
				if (pusherCube2Location == 2) {
					xToShootRayFrom2 = 2;
					yToShootRayFrom2 = -2;
					ShootRayLeft ();
				}
				if (pusherCube2Location == 1) {
					xToShootRayFrom2 = 2;
					yToShootRayFrom2 = 0;
					ShootRayLeft ();
				}
			}
		}
		
	}
	
	void ShootDemRays(){
	if(pusherCube1){	
		CastRayFromPusher1 ();
				
		}
	if(pusherCube2){	
		CastRayFromPusher2 ();	
		}
	}
	
	void ShootRayUp(){
		
		
}	
	
	void ShootRayDown(){
		
		var Location1 = new Vector3(xToShootRayFrom1,yToShootRayFrom1, 10);
		var Location2 = new Vector3(hitX,hitY, 10);
		var Location3 = new Vector3(hitX,hitY, 10);
		var Location4 = new Vector3(hitX,hitY, 10);
		
		directionToShoot=Vector3.down;
		var DirectionToShoot = directionToShoot;
		var HitCube = new Vector3(hitX,hitY);
		if(pusherCube1 && Location1== pusherCube1.transform.position){
			if(Physics.Raycast(Location1, directionToShoot, out hit)){
				hitX = hit.transform.gameObject.transform.position.x;
				hitY = hit.transform.gameObject.transform.position.y;
				if(HitCube!=Location1){
					if(Physics.Raycast(Location1, directionToShoot, out hit)){
						
					}
				}
			}
			Debug.DrawRay(Location1,directionToShoot);
			Debug.DrawRay(Location2,directionToShoot);	
		}
}
	
	void ShootRayRight(){
		
}
	
	void ShootRayLeft(){
		
}
	
}
