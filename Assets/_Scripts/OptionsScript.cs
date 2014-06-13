using UnityEngine;
using System.Collections;


public class OptionsScript : MonoBehaviour {
	
	public GUISkin currentGUISkin;
	private Vector2 scrollPosition;
	private bool optionsUse0;
	private bool previousOptionsUse0;

	// Use this for initialization
	void Start () {
		if (!PlayerPrefs.HasKey ("options_use_0")) {
			PlayerPrefs.SetInt ("options_use_0",1);
			PlayerPrefs.Save();
			optionsUse0 = true;
			previousOptionsUse0 = true;
		}
		else {
			if(PlayerPrefs.GetInt ("options_use_0") == 1) {
				optionsUse0 = true;
				previousOptionsUse0 = true;
			}
			else {
				optionsUse0 = false;
				previousOptionsUse0 = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		GUI.skin = currentGUISkin;

		//check to see if any options changed
		if (previousOptionsUse0 != optionsUse0) {
			PlayerPrefs.SetInt ("options_use_0", (optionsUse0) ? 1 : 0);
			previousOptionsUse0 = optionsUse0;
			GameControllerScript.performRestart = true;
		}

		//set the label 
		GUIStyle labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.UpperCenter;
		labelStyle.fontSize = Mathf.CeilToInt(Screen.height * 0.1F);
		GUILayout.Label ("Options", labelStyle);


		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Mathf.Ceil(Screen.height * .75f)));


			if (GUILayout.Toggle (optionsUse0, "Use 0s \n(Note: Changing this will reset \nthe current game!)", currentGUISkin.toggle)) {
				optionsUse0 = true;		
			}
			else {
				optionsUse0 = false;
			}


			foreach (Touch touch in Input.touches) {
				if (touch.phase == TouchPhase.Moved)
				{
					// dragging
					scrollPosition.y += touch.deltaPosition.y;
				}
			}
		GUILayout.EndScrollView();
		
		currentGUISkin.button.fontSize = Mathf.CeilToInt(Screen.height * 0.06F);
		if (GUILayout.Button ("Return to Game")) {
			Application.LoadLevel("2048_3d");
		}
		
	}
}
