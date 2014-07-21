using UnityEngine;
using System.Collections;

public class ConnectorScript : MonoBehaviour {

	public bool show;
	// Use this for initialization
	void Start () {
		this.show = true;
	}
	
	// Update is called once per frame
	void Update () {
		this.renderer.enabled = this.show;
	}
}
