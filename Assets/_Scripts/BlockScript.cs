using UnityEngine;
using System.Collections;

public class BlockScript : MonoBehaviour {

	public int blockNumber;  //number displayed on the block (-1 for no number)
	public Vector3 originalPosition;  //orginal position of the block
	private int moveNewBlockNumber; //number to change to once the move has been completed
	private float moveStartTime = -1F;  //amount of seconds since move started
	private Vector3 moveNewPosition;      //final destination of move
	private float moveDuration = 0.1F;
	private float scale = 0;

	// Use this for initialization
	void Start () {
		//this.originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		if (moveStartTime >= 0F) {
			float percentComplete = (Time.time - this.moveStartTime) / this.moveDuration;
			if(this.blockNumber > -1) {
				transform.position = Vector3.Lerp(this.originalPosition, this.moveNewPosition, percentComplete);
			}
			if (percentComplete >= 1F) {
				this.moveStartTime = -1F;
				this.setBlockNumber(this.moveNewBlockNumber);
				if(this.blockNumber > -1) {
					transform.position = this.originalPosition;
				}
			}
		}


	}

	public void move(string axis, int units, float scale, int newBlockNumber) {

		//this.moveDuration = duration;
		this.scale = scale;
		this.moveNewBlockNumber = newBlockNumber;
		this.moveStartTime = Time.time;
		this.moveNewPosition = this.originalPosition;
		//this.transform = this.originalPosition;
		if (axis == "x")
						this.moveNewPosition.x = this.originalPosition.x + (this.scale * units);
		if (axis == "y")
						this.moveNewPosition.y = this.originalPosition.y + (this.scale * units);
		if (axis == "z")
						this.moveNewPosition.z = this.originalPosition.z + (this.scale * units);
	}

	public void setBlockNumber(int blockNumber) {
		this.blockNumber = blockNumber;
		TextMesh textMesh = this.GetComponentInChildren<TextMesh>();
		if (blockNumber == -1 ) {
			textMesh.color = new Color(1, 1, 1);
			textMesh.text = "";
			transform.position = new Vector3(0,0,100);
		}
		else {
			textMesh.color = new Color(0, 0, 1);
			if(blockNumber == 0) textMesh.color = new Color(1,1,1);
			textMesh.text = blockNumber.ToString();
			transform.position = this.originalPosition;
		}
		MeshRenderer cube = gameObject.GetComponentInChildren<MeshRenderer>();
		cube.material.color = this.getColor (blockNumber);
	}

	private Color getColor(int num) {
		
		if (num == 0) return HexToColor ("000055");
		if (num == 2) return HexToColor ("ccccff");
		if (num == 4) return HexToColor ("99ffff");
		if (num == 8) return HexToColor ("66ffb2");
		if (num == 16) return HexToColor ("33ff33");
		if (num == 32) return HexToColor ("99ff33");
		if (num == 64) return HexToColor ("ffff00");
		if (num == 128) return HexToColor ("ffcc99");
		if (num == 256) return HexToColor ("ff9933");
		if (num == 512) return HexToColor ("ff6666");
		if (num == 1024) return HexToColor ("ff0000");
		if (num == 2048) return HexToColor ("ff66b2");
		if (num == 4096) return HexToColor ("b266ff");
		if (num == 8192) return HexToColor ("9999ff");
		if (num == 16284) return HexToColor ("ffffff");
		return new Color (0.5f, 0.5f, 0.5f);

	}

	Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r,g,b, 255);
	}
}
