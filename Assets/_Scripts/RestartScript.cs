using UnityEngine;
using System.Collections;

public class RestartScript : MonoBehaviour {

	public GameObject gameController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUpAsButton() {
		GameControllerScript gameControllerScript = this.gameController.GetComponent<GameControllerScript> ();
		gameControllerScript.restart();
	}
}
