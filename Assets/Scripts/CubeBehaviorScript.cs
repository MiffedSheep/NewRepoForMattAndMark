using UnityEngine;
using System.Collections;

public class CubeBehaviorScript : MonoBehaviour {
	public GameObject aCube;
	public GameObject GameController;
	public Vector3 location;
	public int x,y,thisX,thisY;
	private GameControllerScript2 gameControllerScript2;
	public bool scored=false;
	// Use this for initialization
	void Start () {
	//location = gameControllerScript2.cubeArray[x,y].transform.position;
	
	}
	void OnMouseDown(){
		gameControllerScript2=GameController.GetComponent<GameControllerScript2>();	
		//Destroy (gameObject);	
		if((gameControllerScript2.canClickCubes)&& (gameControllerScript2.itIsNowTheActionPhase)){
		print ("Works!");
		thisX=GetComponent<CubeBehaviorScript>().x; 
		thisY=GetComponent<CubeBehaviorScript>().y;  
		Destroy (aCube);			
		}
		else if (gameControllerScript2.canClickCubes==false){
		print("nah");	
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
