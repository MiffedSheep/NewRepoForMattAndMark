using UnityEngine;
using System.Collections;

public class GameControllerScript : MonoBehaviour
{

	//GameObjects
	public GameObject aCube;
	public GameObject pusherCube1;
	public GameObject pusherCube2;
	public Color[] startingColors;
	public int numberOfColors = 6;
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
	public bool freeToMoveRegularly = true;
	public int[] pusherArrayX, pusherArrayY;
	public int pusherCube1Location, pusherCube2Location;
	public int pusherCube1X, pusherCube1Y, pusherCube2X, pusherCube2Y;
	public Vector3[,] pusherArrayVector3;
	public GameObject number1, number2;
	
	//ColorCross related
	public Color CubeAbove, CubeToRight,CubeToLeft,CubeBelow;
	
	//GUI Related
	public GUIStyle actionButton;
	
	//RAYCASTING UGH
	RaycastHit hit;
	
	
	//Game Phase Related
	public float timer;
	public float timeActionPhase = 4f;
	public float timeLooting = 1f;
	public float timeScoredCubeFadeOut = 0.5f;
	public bool itIsNowThePlanningPhase;
	public bool itIsNowTheActionPhase;
	public bool itIsNowTheResolutionPhase;
	public bool thereAreThingsToLoot;


	// Use this for initialization
	void Start ()
	{
		itIsNowTheActionPhase=false;
		itIsNowTheResolutionPhase=false;
		itIsNowThePlanningPhase=true;
		pusherArrayVector3 = new Vector3 [26, 26];
		startingColors = new Color[numberOfColors];
		cubeArray = new GameObject [gridWidth, gridHeight];
		cubebehaviorscript = aCube.GetComponent<CubeBehaviorScript> ();
		for (int x =0; x<gridWidth; x++) {
			for (int y =0; y<gridHeight; y++) {
				cubeArray [x, y] = (GameObject)Instantiate (aCube, new Vector3 (x * gridSpacing - 14, y * gridSpacing - 8, 10), Quaternion.identity);
				cubeArray [x, y].GetComponent<CubeBehaviorScript> ().x = x;
				cubeArray [x, y].GetComponent<CubeBehaviorScript> ().y = y;
				startingColors [0] = Color.cyan;
				startingColors [1] = Color.black;
				startingColors [2] = Color.white;
				startingColors [3] = Color.red;
				startingColors [4] = Color.green;
				startingColors [5] = Color.yellow;
				cubeArray [x, y].renderer.material.color = startingColors [(Random.Range (0, 5))];
				//ForLoop to run ColorCrossCheck
				CheckForSameColorCrossV1(x,y);
				if(CheckForSameColorCrossV1(x,y)){
					cubeArray[x,y].renderer.material.color= startingColors[(Random.Range(0,5))];
				
					
					
				}
				
			
			}
		}



		//pusherCube1 begins in the horizontal orientation
		pusherCube1 = (GameObject)Instantiate (pusherCube1, new Vector3 (0, 2, 10), Quaternion.identity);
		pusherCube1Location = 0;
		pusherCube1.renderer.material.color = startingColors[(Random.Range (0,5))];
		number1 = (GameObject)Instantiate (number1, new Vector3(0, 2, 8),Quaternion.identity);
		//pusherCube2 begins in the vertical orientation
		pusherCube2 = (GameObject)Instantiate (pusherCube2, new Vector3 (2, 0, 10), Quaternion.identity);
		pusherCube2.renderer.material.color = startingColors[(Random.Range (0,5))];
		number2 = (GameObject)Instantiate (number2, new Vector3(2,0,8),Quaternion.identity);
		pusherCube2Location = 1;
	}

	
	void UpdatePusherCubes ()
	{


		pusherArrayX = new int [25];
		pusherArrayY = new int [25];
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

	void Update ()
	{
		//How do I make the planning phase method work simultaneously with keyboard input? Can Unity load up both methods at the same time somehow?
		UpdatePusherCubes ();
		ProcessKeyboardInput ();
		timer = Time.deltaTime;
		for (int x =0; x<gridWidth; x++) {
			for (int y =0; y<gridHeight; y++) {
				CheckForSameColorCrossV1 (x,y);
				if(CheckForSameColorCrossV1 (x,y)){
					print ("Found one!");
				}
				
			}
		}
	}

	void ProcessKeyboardInput ()
	{
		if (Input.GetKeyDown (KeyCode.DownArrow) && itIsNowThePlanningPhase) {
			if (pusherCube2Location == 26) {	
				pusherCube2Location = 0;
				if (pusherCube2Location == pusherCube1Location) {
					pusherCube2Location = pusherCube1Location + 1;
				}			
			} else if (pusherCube2Location < 26) {
				pusherCube2Location++;
				if (pusherCube2Location == pusherCube1Location) {
					pusherCube2Location = pusherCube1Location + 1;
				}	
				
			}
				
		}


		if (Input.GetKeyDown (KeyCode.UpArrow) && itIsNowThePlanningPhase) {
			if (pusherCube2Location == 0) {	
				pusherCube2Location = 26;
								
			} else if (pusherCube2Location > 0) {
				pusherCube2Location--;
				if (pusherCube2Location == pusherCube1Location) {
					pusherCube2Location = pusherCube1Location - 1;
				}	
			}
				
		}	
			
		if (Input.GetKeyDown (KeyCode.LeftArrow) && itIsNowThePlanningPhase) {
			if (pusherCube1Location == 0) {	
				pusherCube1Location = 26;
								
			} else if (pusherCube1Location > 0) {
				pusherCube1Location--;
				if (pusherCube1Location == pusherCube2Location) {
					pusherCube1Location = pusherCube2Location - 1;
				}	
			}
				
		
		}

		if (Input.GetKeyDown (KeyCode.RightArrow)&& itIsNowThePlanningPhase) {
			if (pusherCube1Location == 26) {	
				pusherCube1Location = 0;
				if (pusherCube1Location == pusherCube2Location) {
					pusherCube1Location = pusherCube2Location + 1;
				}				
			} else if (pusherCube1Location < 26) {
				pusherCube1Location++;
				if (pusherCube1Location == pusherCube2Location) {
					pusherCube1Location = pusherCube2Location + 1;
				}	
				
			}

                
					

		}
				

		if (Input.GetKeyDown (KeyCode.Space) && itIsNowThePlanningPhase == true) {
			ProcessActionPhase ();
		}

		else if (Input.GetKeyDown (KeyCode.Space) && itIsNowTheActionPhase == true) {
			ProcessResolutionPhase ();
		} 
		else if (Input.GetKeyDown (KeyCode.Space) && itIsNowTheResolutionPhase == true) {
			ProcessPlanningPhase ();
		} 
		
			
				

	}

	void ProcessMovePusherCube1 (GameObject pusherCube1, int xDir, int yDir)
	{
		pusherCube1.transform.position += new Vector3 (xDir * gridSpacing, yDir * gridSpacing, 0);
	}

	void ProcessMovePusherCube2 (GameObject pusherCube2, int xDir, int yDir)
	{
		pusherCube2.transform.position += new Vector3 (xDir * gridSpacing, yDir * gridSpacing, 0);
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
		print ("It's the action phase");
		//if (timeActionPhase >= Time.deltaTime){
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
		print ("It's the resolution phase");
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
		pusherArrayX = new int[27];
		pusherArrayY = new int[27];
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
		pusherArrayX [19] = -16;
		pusherArrayX [20] = -14;
		pusherArrayX [21] = -12;
		pusherArrayX [22] = -10;
		pusherArrayX [23] = -8;
		pusherArrayX [24] = -6;
		pusherArrayX [25] = -4;
		pusherArrayX [26] = -2;
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
		pusherArrayY [18] = -2;
		pusherArrayY [19] = 0;
		pusherArrayY [20] = 2;
		pusherArrayY [21] = 2;
		pusherArrayY [22] = 2;
		pusherArrayY [23] = 2;
		pusherArrayY [24] = 2;
		pusherArrayY [25] = 2;
		pusherArrayY [26] = 2;
	}
	
	void ScoringMethodV1(int x, int y){
	 if(CheckForSameColorCrossV1(x,y)){
			
			
		}
		
		
	}
	
	
	bool CheckForSameColorCrossV1 (int x, int y)
	{	
//	if(x!=-14){
//		CubeToLeft = cubeArray[x-1,y].renderer.material.color;
//	}
//	else if (x==-14){
//		CubeToLeft= 	cubeArray[x,y].renderer.material.color;	
//	}
//	if(x!=0){	
//		CubeToRight = cubeArray[x+1,y].renderer.material.color;
//	}
//	else if (x==0){
//	 CubeToRight= 	cubeArray[x,y].renderer.material.color;
//	}
//	if(y!=0){	
//		CubeAbove = cubeArray[x,y+1].renderer.material.color;
//	}
//	else if(y==0){
//		CubeAbove= 	cubeArray[x,y].renderer.material.color;	
//	}
//	if(y!=-8){		
//		CubeBelow = cubeArray[x,y-1].renderer.material.color;
//	}	
//	else if(y==-8){
//		CubeBelow= 	cubeArray[x,y].renderer.material.color;
//	}		
		if((cubeArray[x,y].renderer.material.color == 
				CubeToRight) && 
				(cubeArray[x,y].renderer.material.color ==
				CubeToLeft)){
		print("Horizontal Things");	
		}
		
		if((cubeArray[x,y].renderer.material.color ==
				CubeAbove) &&
				(cubeArray[x,y].renderer.material.color ==
				CubeBelow)){
		print ("Verticle Things");	
		}
		return true;
		
		
		
		
//		if(((cubeArray[x,y].renderer.material.color == 
//				CubeToRight) && 
//				(cubeArray[x,y].renderer.material.color ==
//				CubeToLeft)) &&
//				((cubeArray[x,y].renderer.material.color ==
//				CubeAbove) &&
//				(cubeArray[x,y].renderer.material.color ==
//				CubeBelow))){
//					if(((cubeArray[x,y].renderer.material.color == 
//						CubeToRight) && 
//						(cubeArray[x,y].renderer.material.color ==
//						CubeToLeft))){
//							//Fade Out Cubes at locations X,Y, X+,Y, and X-,Y
//							print("Horizontal!");
//							
//					}
//					else if(((cubeArray[x,y].renderer.material.color == 
//					CubeAbove) && 
//					(cubeArray[x,y].renderer.material.color ==
//					CubeBelow))){
//						//Fade Out Cubes at locations X,Y, X,Y+, and X,Y-
//						print ("Verticle!");
//					
//					}
//					return true;
//			}
//		else{
//			return false;
//			}
//		
		}
	void OnGUI (){
		
		
		if((GUI.Button (new Rect(75,10,100f,100f), "Action Phase",actionButton)) && itIsNowThePlanningPhase){
			ProcessActionPhase ();	
			
			
		}
		
	}
	void SendOutSomeRays(int x, int y){
	Vector3 fwd = transform.TransformDirection(Vector3.forward);
	Vector3 up = transform.TransformDirection(Vector3.up);
	Vector3 down = transform.TransformDirection(Vector3.down);
	Vector3 right = transform.TransformDirection(Vector3.right);
	Vector3 left = transform.TransformDirection(Vector3.left);
		
				if (Physics.Raycast(new Vector3 (x*2,y*2,10), up, out hit,10)){ 
					//cubeArray[x,y] = hit.transform.gameObject; 
					print ("I hit something Above!" + hit.transform.gameObject);
					
				}
				if (Physics.Raycast(new Vector3 (x*2,y*2,10), down, out hit,10)){ 
					//cubeArray[x,y] = hit.transform.gameObject; 
					print ("I hit something Below"+ hit.transform.gameObject);
				} 
				if (Physics.Raycast(new Vector3 (x*2,y*2,10), right, out hit,10)){ 
					//cubeArray[x,y] = hit.transform.gameObject;
					print ("I hit something to the Right!"+ hit.transform.gameObject);
				} 
				if (Physics.Raycast(new Vector3 (x*2,y*2,10), left, out hit,10)){ 
					//cubeArray[x,y] = hit.transform.gameObject; 
					print ("I hit something the the Left"+ hit.transform.gameObject);
				} 
				if (Physics.Raycast(new Vector3 (x*2,y*2,10), fwd, out hit,10)){ 
					//cubeArray[x,y] = hit.transform.gameObject; 
					print ("I hit something Forward?"+ hit.transform.gameObject);
				}
				Debug.DrawRay (new Vector3 (x*2 - 14,y*2 - 8,10), up*10, Color.cyan);
				Debug.DrawRay (new Vector3 (x*2 - 14,y*2 - 8,10), down*10, Color.red);
				Debug.DrawRay (new Vector3 (x*2 - 14,y*2 - 8,10), right*10, Color.blue);
				Debug.DrawRay (new Vector3 (x*2 - 14,y*2 - 8,10), left*10, Color.green);
	
		
	
	}

}