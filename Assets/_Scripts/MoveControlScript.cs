using UnityEngine;
using System.Collections;

public class MoveControlScript : MonoBehaviour {

	public int direction;
	public string axis;
	public GameObject gameController;



	// Use this for initialization
	void Start () {
		transform.renderer.material.color = Color.red;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUpAsButton() {
		GameControllerScript gameControllerScript = this.gameController.GetComponent<GameControllerScript> ();
		gameControllerScript.doMove (this.axis, this.direction);
	}
}
