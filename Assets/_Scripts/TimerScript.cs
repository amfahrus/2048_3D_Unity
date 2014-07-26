using UnityEngine;
using System.Collections;

public class TimerScript : MonoBehaviour {

	private GameControllerScript gameScript;
	private OptionsScript options;
	private GUISkin currentGUISkin;
	private bool timerEnabled;
	private int timeRemaining;
	public AudioClip countdownSound;
	private AudioSource countdownAudioSource; //reserved for countdown

	// Use this for initialization
	void Start () {
		this.gameScript = this.gameObject.GetComponent ("GameControllerScript") as GameControllerScript;
		this.options = this.gameObject.GetComponent ("OptionsScript") as OptionsScript;
		this.currentGUISkin = gameScript.currentGUISkin;
		this.InitTimer ();

		AudioSource[] audioSources = GetComponents<AudioSource>();
		this.countdownAudioSource = audioSources[2];
		countdownAudioSource.clip = this.countdownSound;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		
		GUI.skin = this.gameScript.currentGUISkin;
		if(this.timerEnabled
		   && gameScript.gameView == "game"
		) {
			
			GUI.Label (new Rect (0, Screen.height * 0.70f , Screen.width * 0.35f, Screen.height * 0.15f), 
			           this.timeRemaining.ToString(), "BigLabel");

			GUI.Label (new Rect (Screen.width * 0.0f, Screen.height * 0.77f , Screen.width * 0.35f, Screen.height * 0.08f), 
			           "Target");

			
			int nextHighest = PlayerPrefs.GetInt ("game_highest_block") * 2;
			if (nextHighest < 64) nextHighest = 64;
			GUI.Label (new Rect (Screen.width * 0.0f, Screen.height * 0.82f , Screen.width * 0.35f, Screen.height * 0.08f), 
			           nextHighest.ToString ());
		}
	}

	//initialize the timer
	public void InitTimer() {
		//set time remaining if it doesn't exist yet
		if (!PlayerPrefs.HasKey ("timer_time_remaining")){
			this.timeRemaining = PlayerPrefs.GetInt("options_timer_duration");
			PlayerPrefs.SetInt ("timer_time_remaining", this.timeRemaining);
		}
		else {
			this.timeRemaining = PlayerPrefs.GetInt ("timer_time_remaining");
		}
		PlayerPrefs.Save ();

		
		CancelInvoke ("TimeChange");
		if (PlayerPrefs.GetInt ("options_timer_duration") > 0) {
			this.timerEnabled = true;
			InvokeRepeating ("TimeChange", 1, 1);
		}
		else {
			this.timerEnabled = false;
		}
	}

	public void ResetTimer() {
		this.InitTimer ();
		this.timeRemaining = PlayerPrefs.GetInt ("options_timer_duration") + 5;
		PlayerPrefs.SetInt ("timer_time_remaining", this.timeRemaining);
		PlayerPrefs.Save();
	}

	private void TimeChange() {
		if (this.gameScript.gameView == "game"
		    && PlayerPrefs.GetString ("game_status") == "playing"
		) {
			if(
				this.options.play_sounds
				&& this.timeRemaining <= 6
				&& this.timeRemaining > 1
			) {
				this.countdownAudioSource.Play();
			}
			if(this.timeRemaining == 1) {
				PlayerPrefs.SetString ("game_status", "game_over");
			}
			this.timeRemaining = this.timeRemaining - 1;
		}

		PlayerPrefs.SetInt ("timer_time_remaining", this.timeRemaining);
		PlayerPrefs.Save ();

	}

	/**
	 * Set the next block target
	 * 
	 */
	public void SetNextBlockTarget(int TargetBlock) {
		//add the amount of time necessary for the next number to be reached
		if(TargetBlock >= 64) {
			int addTime = PlayerPrefs.GetInt ("options_timer_duration") * TargetBlock / 64;
			this.timeRemaining = this.timeRemaining + addTime;
		}
	}
}
