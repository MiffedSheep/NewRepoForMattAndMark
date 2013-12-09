using UnityEngine;
using System.Collections;

public class GameSummaryControllerScript : MonoBehaviour {
	public GUIStyle victoryTextStyle;
	public GUIStyle restartButtonStyle;
	public GUIStyle finalScoreStyle;
	private bool restartButtonWasClicked = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (restartButtonWasClicked == true){
			//initiate gamescene.unity
			print("restart game!");
		}
	}
	void OnGUI(){
		GUI.Box (new Rect(-10,0,2000,75), "Block Shift", victoryTextStyle);
		
		GUI.Box (new Rect(0,0,0,0), "Your Score = " /*gamecontrollerscript.finalScore*/, victoryTextStyle);
		
		if (GUI.Button (new Rect(150,150,700,300), "RESTART", restartButtonStyle)) {
					restartButtonWasClicked = true;
		}
	}
}
