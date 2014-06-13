using UnityEngine;
using System.Collections;

public class FixFontScript : MonoBehaviour {

	public float fontScale;
	public float yOffset;

	// Use this for initialization
	void Start () {
		this.guiText.fontSize = Mathf.CeilToInt (Screen.height *  this.fontScale);
		this.guiText.pixelOffset = new Vector2(this.guiText.pixelOffset.x, Mathf.CeilToInt (Screen.height * this.yOffset));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
