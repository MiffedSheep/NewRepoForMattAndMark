using UnityEngine;
using System.Collections;

public class StartScreenControllerScript : MonoBehaviour {

public GUIStyle mainBannerStyle;
public GUIStyle startButtonStyle;
private bool startButtonWasClicked = false;
	// Use this for initialization
	void Start () {
	 
	}
	
	// Update is called once per frame
	void Update () {
		if (startButtonWasClicked == true){
			print ("start game!");
			//initiate gamescene.unity
		}
	}
	void OnGUI(){
		GUI.Box (new Rect(-10,0,2000,75), "Block Shift", mainBannerStyle);
		
		if (GUI.Button (new Rect(150,150,700,300), "START", startButtonStyle)) {
					startButtonWasClicked = true;
		}
	}
}