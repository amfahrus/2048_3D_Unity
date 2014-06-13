using UnityEngine;
using System.Collections;

public class SwipeListenerScript : MonoBehaviour {
	
	private Vector2 startPosition;  // first finger position
	private Vector2 lastPosition;  // last finger position

	
	void Update () {
		//transform.guiText.text = "my text";
		int fingerCount = 0;
		foreach (Touch touch in Input.touches) {
			if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				fingerCount++;

			if (touch.phase == TouchPhase.Began)
			{
				startPosition = touch.position;
				lastPosition = touch.position;
			}

			if (touch.phase == TouchPhase.Moved )
			{
				lastPosition = touch.position;
			}
			if(touch.phase == TouchPhase.Ended)
			{ 
				
				if((startPosition.x - lastPosition.x) > 80) // left swipe
				{
					this.Swipe ("x", -1);
				}
				else if((startPosition.x - lastPosition.x) < -80) // right swipe
				{
					this.Swipe ("x", 1);
				}
				else if((startPosition.y - lastPosition.y) < -80 ) // up swipe
				{
					this.Swipe ("y", 1);
				}
				else if((startPosition.y - lastPosition.y) > 80 ) // down swipe
				{
					this.Swipe ("y", -1);
				}
			
			}
		}
	}

	void Swipe(string axis, int direction) {
		GameControllerScript gameScript = this.GetComponent ("GameControllerScript") as GameControllerScript;
		if (gameScript.gameView == "game") gameScript.doMove (axis, direction);
	}
}