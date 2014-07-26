using UnityEngine;
using System.Collections;

public class InstructionsScript : MonoBehaviour {
	private GameControllerScript gameScript;
	private GUISkin currentGUISkin;
	private Vector2 scrollPosition;

	//instruction text

			
		
			

	// Use this for initialization
	void Start () {
		this.gameScript = this.gameObject.GetComponent ("GameControllerScript") as GameControllerScript;
		this.currentGUISkin = gameScript.currentGUISkin;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (this.gameScript.gameView == "instructions") {
			GUI.skin = currentGUISkin;

			this.gameScript.mainCamera.transform.eulerAngles = new Vector3 (120, 23, 0);
			
			GUIStyle labelStyle = new GUIStyle(currentGUISkin.label);
			labelStyle.alignment = TextAnchor.UpperLeft;
			GUILayout.Label ("Instructions", "BigLabel");


			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Mathf.Ceil(Screen.height * .80f)));

			GUILayout.Label ("Object", "Subheader");

			GUILayout.Label (@"The object of 2048-3D is to"
                 + " slide numbered blocks in such a way"
                 + " so that blocks with the same numbers collide"
                 + " and combine into a new block that is twice"
                 + " as much as the originals until"
                 + " the number 2048 is reached.", labelStyle);

			GUILayout.Label ("Moving the Blocks", "Subheader");
			
			GUILayout.Label (@"You cannot move blocks individually, but must"
                + " move all blocks simultaneously in the same direction." 
                + " Blocks can move forward, backward, up, down, left"
				+ " and right along the green connectors."
		        + " Simply swipe any part of the screen to move up,"
				+ " down, left or right (keyboard: arrow keys). To move the"
				+ " blocks forward and backward use the" 
				+ " big red arrow keys at the bottom of the screen (keyboard: a and z keys)." 
				+ " When moving, all blocks that can slide in the chosen direction will move."
				+ " Any block moving toward another block with the same number will collide "
			    + " and form a single block with twice the number as the originals", labelStyle);


			GUILayout.Label ("New Blocks", "Subheader");

			GUILayout.Label (@"After each move is"
                + " made a new block will appear randomly in an empty position."
                + " This block will have a number of either 2 or 4."
			    + " For an extra challenge, there is a game option you can"
			    + " set so that zeros can also be assinged to a new block."
			    + " Zeros act like any other number in that they can"
			    + " collide with other zeros to make a block twice as much "
			    + " (which is still zero).", labelStyle);



			GUILayout.Label ("Scoring and Finishing", "Subheader");

			GUILayout.Label(@"For every block collision that occurs you receive"
                + " the number of points of the newly"
                + " created block.  If after making a move"
                + " all positions are filled and no new"
                + " moves are possible, the game ends."
			    + " A separate high score / highest block is kept for each"
			    + " distinct combination of game options", labelStyle);


			GUILayout.Label ("Game Layout Options", "Subheader");

			GUILayout.Label (@"When I first made this game there"
		         + " was only one game layout, a 3x3x3 cube."
		         + " After testing it a bit, it was way to easy"
		         + " so the zero option was added."
		         + " It was still way to easy "
		         + " (e.g. you could swipe without even looking and get pretty far)."
		         + " Therefore there are now several diffent game layouts that"
		         + " make the game more challenging and fun.", labelStyle);

			GUILayout.Label ("Game Timer Option", "Subheader");

			GUILayout.Label (@"To give yourself even more of a challenge"
			                 + " you can set game options to include a timer."
			                 + " If a timer is chosen you have a specific"
			                 + " amount of time to combined blocks to make the 64 block."
			                 + " If you run out of time the game is over."
			                 + " If you reach your target before the timer runs down you will"
			                 + " receive additional time to reach the next target."
			                 + " The time you received is as follows: \n"
			                 + " 64: option time + 5 seconds (because the first one is the hardest!)\n"
			                 + " 128: option time\n"
			                 + " 256: 2X option time\n"
			                 + " 512: 4X option time \n"
			                 + " 1024: 8X option time \n"
			                 + " you get the idea.", labelStyle);

			
			GUILayout.Label ("Acknowledgements \nand Confessions", "Subheader");

			GUILayout.Label (@"2048-3D is based upon the original" +
			                 " 2048 game designed by Gabriele Cirulli (over which" +
			                 " I became completely obsessed).\n\n" +
			                 " Sound effects by freeSFX http://www.freesfx.co.uk.\n\n" +
			                 " This game was designed using the Unity3D game engine.\n\n" +
			                 " Making this game was a blast." +
			                 " I am a complete newb to both Unity3D, C#" +
			                 " and any kind of mobile app development" +
			                 " so if you like this game that is awesome!" +
			                 " If you don't, well, I'm just a newb anyway." +
			                 " You can also play this on your desktop by going to " +
			                 " http://www.jrweth.com.  " +
			                 " In any case I would love to get any feed back from you " +
			                 " to make this game even better.\n\n" +
			                 " The code for this game is also available on Github at" +
			                 " https://github.com/jrweth/2048_3D_Unity", labelStyle);


			foreach (Touch touch in Input.touches) {
				if (touch.phase == TouchPhase.Moved)
				{
					// dragging
					this.scrollPosition.y += touch.deltaPosition.y;
				}
			}
			GUILayout.EndScrollView();
			
			if (GUILayout.Button ("Return to Menu")) {
				this.gameScript.gameView = "menu";
			}
		}
	}
}
