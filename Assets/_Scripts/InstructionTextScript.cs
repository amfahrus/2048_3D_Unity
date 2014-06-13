using UnityEngine;
using System.Collections;

public class InstructionTextScript : MonoBehaviour {

	public GUISkin currentGUISkin;
	public Vector2 scrollPosition;
	private string longString = @"The object of 2048-3D is to"
+ " move the blocks in such a way"
+ " that same numbered blocks collide"
+ " forming a new block that is twice"
+ " as much as the originals until"
+ " the number 2048 is reached.\n\n"
+ "For every collision you receive"
+ " the number of points of the newly"
+ " created block. After each move is"
+ " made a new number (either 0, 2 or 4)"
+ " will be randomly assigned to an"
+ " empty block.  If after making a move"
+ " all blocks are filled and no new"
+ " moves are possible, the game ends. \n\n"

+ "To move the blocks simply swipe up,"
+ " down, left or right. To move the"
+ " blocks forward and backward use the" 
+ " red arrow keys on screen.  Blocks"
+ " with 0 will combine but never"
+ " increase higher than 0. You can turn"
+ " off 0s in the game options tab.\n\n"
+ "2048-3D is based upon the original"
+ " 2048 game designed by Gabriele Cirulli.";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		GUI.skin = currentGUISkin;

		GUIStyle labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.UpperCenter;
		labelStyle.fontSize = Mathf.CeilToInt(Screen.height * 0.1F);
		GUILayout.Label ("Instructions", currentGUISkin.label);

		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Mathf.Ceil(Screen.height * .75f)));
			currentGUISkin.label.fontSize = Mathf.CeilToInt(Screen.height * 0.04F);
			GUILayout.Label(this.longString);
			
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
