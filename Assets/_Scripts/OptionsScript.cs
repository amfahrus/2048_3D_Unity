using UnityEngine;
using System.Collections;

/**
 * class for handling retrieval, setting and saving of game options
 */
public class OptionsScript : MonoBehaviour {

	private GameControllerScript gameScript;

	public bool use_0;    //whether or not to use 0 blocks in the game
	public bool play_sounds; //whether or not to play sounds
	public string board_type; //which type of board using: Solid Cube, Hollow Cube, Four Walls, Box Outline
	public int timer_duration;

	//previous values for detecting change
	private bool previous_use_0;
	private bool previous_play_sounds;
	private string previous_board_type;
	private int previous_timer_duration;


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
			optionsKey += "Yes";
		}
		else {
			optionsKey += "No";
		}

		optionsKey += " / Timer: ";
		if (this.timer_duration == 0) {
			optionsKey += "none";
		}
		else {
			optionsKey += this.timer_duration.ToString () + " seconds";
		}

		return optionsKey;
	}

	/**
	 * Set up the options in PlayerPrefs if they do not already exist
	 */

	private void InitOptions() {
		this.initOption ("use_0", false);
		this.initOption ("play_sounds", true);
		this.initOption ("board_type", "No Corners");
		this.initOption ("timer_duration", 0);
	}
	//Boolean init option
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

	//string init option
	private void initOption(string optionKey, string defaultValue) {
		if (!PlayerPrefs.HasKey ("options_" + optionKey)) {
			this.setOption (optionKey, defaultValue);
		}
		else {
			this.setOption (optionKey, PlayerPrefs.GetString ("options_" + optionKey));
		}
	}

	
	//int init option
	private void initOption(string optionKey, int defaultValue) {
		if (!PlayerPrefs.HasKey ("options_" + optionKey)) {
			this.setOption (optionKey, defaultValue);
		}
		else {
			this.setOption (optionKey, PlayerPrefs.GetInt("options_" + optionKey));
		}
	}

	//boolean set option
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

	//string set option
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

	//int set option
	private void setOption(string optionKey, int optionValue) {	
		//set the Player Prefis Value
		PlayerPrefs.SetInt ("options_" + optionKey, optionValue);	
		PlayerPrefs.Save();

		//I want to do this but getting a null reference exception for "this"  argh!!
		//GetType().GetProperty(optionKey).SetValue(this, optionBoolValue, null);
		//GetType().GetProperty("previous_" + optionKey).SetValue(this, optionBoolValue, null);
		
		if(optionKey == "timer_duration") {
			this.timer_duration = optionValue;
			this.previous_timer_duration = optionValue;
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

			if (this.previous_timer_duration != this.timer_duration) {
				this.setOption ("timer_duration", this.timer_duration);
				GameControllerScript.performRestart = true;
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

			GUILayout.Label ("Game Board Type", "Subheader");
			//Game Board Type
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (this.board_type == "Solid Cube", "Solid Cube", currentGUISkin.toggle)) {
				this.board_type = "Solid Cube";
			}
			GUILayout.Label ( "Solid Cube (27 Blocks)", "ToggleLabel");
			GUILayout.EndHorizontal();

			
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (this.board_type == "Hollow Cube", "Hollow Cube", currentGUISkin.toggle)) {
				this.board_type = "Hollow Cube";
			}
			GUILayout.Label ( "Hollow Cube (26 Blocks)", "ToggleLabel");
			GUILayout.EndHorizontal();

			
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (this.board_type == "Four Walls", "Four Walls", currentGUISkin.toggle)) {
				this.board_type = "Four Walls";
			}
			GUILayout.Label ( "Four Walls (24 Blocks)", "ToggleLabel");
			GUILayout.EndHorizontal();

			
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (this.board_type == "Box Outline", "Box Outline", currentGUISkin.toggle)) {
				this.board_type = "Box Outline";
			}
			GUILayout.Label ( "Cube Outline (20 Blocks)", "ToggleLabel");
			GUILayout.EndHorizontal();

			
			
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (this.board_type == "No Corners", "No Corners", currentGUISkin.toggle)) {
				this.board_type = "No Corners";
			}
			GUILayout.Label ( "No Corners (19 Blocks)", "ToggleLabel");
			GUILayout.EndHorizontal();


			//TIMER OPTIONS ####################################################
			GUILayout.Label ("Timer Options", "Subheader");

			foreach (int i in this.GetTimerDurationTimes())
			{
				GUILayout.BeginHorizontal ();
				if (GUILayout.Toggle (this.timer_duration == i, "", currentGUISkin.toggle)) {
					this.timer_duration = i;
				}
				GUILayout.Label (TimerDurationToString (i), "ToggleLabel");
				GUILayout.EndHorizontal();
			}



			
			//Other OPTIONS ####################################################
			GUILayout.Label ("Block Generation Options", "Subheader");

			//Use Zeros
			GUILayout.BeginHorizontal ();
			if (GUILayout.Toggle (use_0, "Use 0s (Note: Changing this will reset the current game!)", currentGUISkin.toggle)) {
				use_0 = true;		
			}
			else {
				use_0 = false;
			}
			GUILayout.Label ( "Use Zeros", "ToggleLabel");
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

	private int[] GetTimerDurationTimes() {
		int[] times = new int[4]{0,25,20,15};
		return times;
	}

	private string TimerDurationToString(int timerDuration) {
		if (timerDuration == 0) {
			return "No Timer";
		}
		if (timerDuration == 25) {
			return "Light Speed (25 Sec)";
		}
		if (timerDuration == 20) {
			return "Ridiculous Speed (20 Sec)";
		}
		if (timerDuration == 15) {
			return "Ludicrous Speed (15 Sec)";
		}
		return "Unknown Speed";
	}
}
