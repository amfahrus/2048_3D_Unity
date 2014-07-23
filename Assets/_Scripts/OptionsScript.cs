using UnityEngine;
using System.Collections;

/**
 * class for handling retrieval, setting and saving of game options
 */
public class OptionsScript : MonoBehaviour {

	private GameControllerScript gameScript;

	public bool use_0;    //whether or not to use 0 blocks in the game
	public bool play_sounds; //whether or not to play sounds
	public string board_type; //which type of board using: Solid Cube, Hollow Cube, Shaft, Box Outline

	//previous values for detecting change
	private bool previous_use_0;
	private bool previous_play_sounds;
	private string previous_board_type;



	private GUISkin currentGUISkin;
	private Vector2 scrollPosition;

	// Use this for initialization
	void Start () {
		this.gameScript = this.gameObject.GetComponent ("GameControllerScript") as GameControllerScript;
		this.currentGUISkin = gameScript.currentGUISkin;

		this.InitOptions();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string GetOptionsKey() {

		string optionsKey = "Board Type: " + this.board_type;

		optionsKey += " / Use Zeros: ";
		if (this.use_0) {
			optionsKey = optionsKey + " Yes";
		}
		else {
			optionsKey = optionsKey + " No";
		}


		return optionsKey;
	}

	/**
	 * Set up the options in PlayerPrefs if they do not already exist
	 */

	private void InitOptions() {
		this.initOption ("use_0", true);
		this.initOption ("play_sounds", true);
		this.initOption ("board_type", "Solid Cube");
	}

	private void initOption(string optionKey, bool defaultValue) {
		if (!PlayerPrefs.HasKey ("options_" + optionKey)) {
			this.setOption (optionKey, defaultValue);
		}
		else {
			if (PlayerPrefs.GetInt ("options_" + optionKey) == 1) {
				this.setOption(optionKey, true);
			}
			else {
				this.setOption(optionKey, false);
			}
		}
	}

	private void initOption(string optionKey, string defaultValue) {
		if (!PlayerPrefs.HasKey ("options_" + optionKey)) {
			this.setOption (optionKey, defaultValue);
		}
		else {
			this.setOption (optionKey, PlayerPrefs.GetString ("options_" + optionKey));
		}
	}

	private void setOption(string optionKey, bool optionBoolValue) {	
		//convert bool to int
		int optionIntValue;
		if (optionBoolValue == true) {
			optionIntValue = 1;
		}
		else {
			optionIntValue = 0;
		}
		
		//set the Player Prefis Value
		PlayerPrefs.SetInt ("options_" + optionKey,optionIntValue);	
		PlayerPrefs.Save();

		//I want to do this but getting a null reference exception for "this"  argh!!
		//GetType().GetProperty(optionKey).SetValue(this, optionBoolValue, null);
		//GetType().GetProperty("previous_" + optionKey).SetValue(this, optionBoolValue, null);

		if(optionKey == "use_0") {
			this.use_0 = optionBoolValue;
			this.previous_use_0 = optionBoolValue;
		}
		if(optionKey == "play_sounds") {
			this.play_sounds = optionBoolValue;
			this.previous_play_sounds = optionBoolValue;
		}
	}

	private void setOption(string optionKey, string optionStringValue) {	
		//set the Player Prefis Value
		PlayerPrefs.SetString ("options_" + optionKey, optionStringValue);	
		PlayerPrefs.Save();
		
		//I want to do this but getting a null reference exception for "this"  argh!!
		//GetType().GetProperty(optionKey).SetValue(this, optionBoolValue, null);
		//GetType().GetProperty("previous_" + optionKey).SetValue(this, optionBoolValue, null);
		
		if(optionKey == "board_type") {
			this.board_type = optionStringValue;
			this.previous_board_type = optionStringValue;
		}
	}

	void OnGUI() {
		if (this.gameScript.gameView == "options") {
			this.gameScript.mainCamera.transform.eulerAngles = new Vector3 (120, 23, 0);

			GUI.skin = this.gameScript.currentGUISkin;
			
			//check to see if any options changed
			if (previous_use_0 != use_0) {
				this.setOption ("use_0", this.use_0);
				GameControllerScript.performRestart = true;
			}

			if (this.previous_board_type != this.board_type) {
				this.setOption ("board_type", this.board_type);
				GameControllerScript.performRestart = true;
			}

			if (previous_play_sounds != play_sounds) {
				this.setOption ("play_sounds", this.play_sounds);
			}
			//set the label 
			GUILayout.Label ("Options", "BigLabel");
			
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Mathf.Ceil(Screen.height * .80f)));
		
			
			
			//Sounds
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (this.play_sounds, "Play Sounds", currentGUISkin.toggle)) {
				this.play_sounds = true;		
			}
			else {
				this.play_sounds = false;
			}
			GUILayout.Label ( "Play Sounds", "ToggleLabel");
			GUILayout.EndHorizontal();

			
			GUILayout.Label ("WARNING! Changing any of the following options will cause the game to reset!", "ToggleLabelWarning");

			GUILayout.Label ("Game Board Type", "OptionSubheader");
			//Game Board Type
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (this.board_type == "Solid Cube", "Solid Cube", currentGUISkin.toggle)) {
				this.board_type = "Solid Cube";
			}
			GUILayout.Label ( "Solid Cube (27 Blocks)", "ToggleLabelWarning");
			GUILayout.EndHorizontal();

			
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (this.board_type == "Hollow Cube", "Hollow Cube", currentGUISkin.toggle)) {
				this.board_type = "Hollow Cube";
			}
			GUILayout.Label ( "Hollow Cube (26 Blocks)", "ToggleLabelWarning");
			GUILayout.EndHorizontal();

			
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (this.board_type == "Shaft", "Shaft", currentGUISkin.toggle)) {
				this.board_type = "Shaft";
			}
			GUILayout.Label ( "Shaft (24 Blocks)", "ToggleLabelWarning");
			GUILayout.EndHorizontal();

			
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (this.board_type == "Box Outline", "Box Outline", currentGUISkin.toggle)) {
				this.board_type = "Box Outline";
			}
			GUILayout.Label ( "Cube Outline (20 Blocks)", "ToggleLabelWarning");
			GUILayout.EndHorizontal();
			
			GUILayout.Label ("Other Options", "OptionSubheader");

			//Use Zeros
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (use_0, "Use 0s (Note: Changing this will reset the current game!)", currentGUISkin.toggle)) {
				use_0 = true;		
			}
			else {
				use_0 = false;
			}
			GUILayout.Label ( "Use Zeros", "ToggleLabelWarning");
			GUILayout.EndHorizontal();



			foreach (Touch touch in Input.touches) {
				if (touch.phase == TouchPhase.Moved)
				{
					// dragging
					scrollPosition.y += touch.deltaPosition.y;
				}
			}

			GUILayout.EndScrollView();
			
			if (GUILayout.Button ("Return to Menu", "Button")) {
				gameScript.gameView = "menu";
			}
		}
	}
}
