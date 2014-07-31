using UnityEngine;
using System.Collections;

public class ConnectorScript : MonoBehaviour {

	public int x;
	public int y;
	public int z;
	public string axis; //x,y,z
	private GameControllerScript gameScript;
	private float scale;
	private float yOffset;
	private float rotationTotal = -1f;
	private Vector3 rotateDirection;
	private float moveDuration;
	// Use this for initialization
	void Start () {
		//setup local versions of Game Variables



	}

	public void Initialize(int x, int y, int z, string axis, GameControllerScript gameScript) {
		this.scale = gameScript.scale;
		this.yOffset = gameScript.yOffset;
		this.gameScript = gameScript;
		this.moveDuration = gameScript.moveDuration;
		this.x = x;
		this.y = y;
		this.z = z;
		this.axis = axis;
		this.ResetPosition();
	}

	private void ResetPosition() {
		
		if (this.axis == "x") {
			transform.position = new Vector3(
				x * this.scale + (this.scale/2f),
				y * this.scale + this.yOffset,
				z * this.scale);
			transform.eulerAngles = new Vector3(0,0,90);
		}
		if (this.axis == "y") {
			transform.position = new Vector3(
				x * this.scale,
				y * this.scale + this.yOffset + this.scale/2f,
				z * this.scale);
			transform.eulerAngles = new Vector3(0,0,0);
		}
		if (this.axis == "z") {
			transform.position = new Vector3(
				x * this.scale,
				y * this.scale + this.yOffset,
				z * this.scale + this.scale/2f);
			transform.eulerAngles = new Vector3(90,0,0);
		}
	}

	/*
	 * Method to determine if the connector should be shown
	 */
	private bool show(){

		//if this block is one that should not be shown
		if (this.GetBlockNumber(this.x, this.y, this.z) == -2) return false;

		if (this.axis == "x") {
			//if this is on the edge then don't display
			if(this.x == 2) return false;
			//check if the neighboring block should not be shown
			if(this.GetBlockNumber (this.x + 1, this.y, this.z) == -2) return false;
		}
		if (this.axis == "y") {
			//if this is on the edge then don't display
			if(this.y == 2) return false;
			//check if the neighboring block should not be shown
			if(this.GetBlockNumber (this.x, this.y+1, this.z) == -2) return false;
		}
		if (this.axis == "z") {
			//if this is on the edge then don't display
			if(this.z == 2) return false;
			//check if the neighboring block should not be shown
			if(this.GetBlockNumber (this.x, this.y, this.z+1) == -2) return false;
		}

		return true;
	}

	private Color getConnectorColor(){
		Color standardColor = new Color(0,0.5f, 0);
		Color highlightedColor = new Color(0f, 0f, 1f);
		int block0;
		int block1;
		int block2;
		int blockPosition;

		if (this.axis == "x") {
			blockPosition = this.x;
			block0 = this.GetBlockNumber(0,this.y, this.z);
			block1 = this.GetBlockNumber(1,this.y, this.z);
			block2 = this.GetBlockNumber(2,this.y, this.z);
		}
		else if (this.axis == "y") {
			blockPosition = this.y;
			block0 = this.GetBlockNumber(this.x,0, this.z);
			block1 = this.GetBlockNumber(this.x,1, this.z);
			block2 = this.GetBlockNumber(this.x,2, this.z);
		}
		else if (this.axis == "z") {
			blockPosition = this.z;
			block0 = this.GetBlockNumber(this.x,this.y, 0);
			block1 = this.GetBlockNumber(this.x,this.y, 1);
			block2 = this.GetBlockNumber(this.x,this.y, 2);
		}
		else { //not quite sure why axis isn't set but throwing error
			return standardColor;
		}

		//no highlighting needed since connector isn't visible
		if (blockPosition == 2) return standardColor;

		//middle block is blank and first and second blocks match
		if (block1 == BlockScript.emptyBlock 
		    && block0 != BlockScript.emptyBlock
		    && block0 == block2) return highlightedColor;

		//if this block is empty then return standard color
		if (blockPosition == BlockScript.emptyBlock) return standardColor;

		//if the next matches 
		if(blockPosition == 0 && block0 == block1 && block0 != BlockScript.emptyBlock) return highlightedColor;
		if(blockPosition == 1 && block1 == block2 && block1 != BlockScript.emptyBlock) return highlightedColor;

		//set the default color
		return standardColor;
	}
	/*
	 * Get the number on the block corresponding to the position parameters
	 */
	private int GetBlockNumber(int x, int y, int z) {
		return this.gameScript.getBlockNumber (x,y,z);
	}

	public void rotateConnector(Vector3 direction) {
		this.rotateDirection = direction;
		this.rotationTotal = 0f;
	}
	// Update is called once per frame
	void Update () {
		//this.renderer.enabled = this.show;
		//Vector3 rotationPoint = new Vector3 (3f, 4f, 3f);

		this.renderer.enabled = this.show();
		this.renderer.material.color = this.getConnectorColor();

		
		if (this.rotationTotal >= 0F) {
			float rotateBy = 90 * Time.deltaTime / this.moveDuration;
			Vector3 rotationPoint = new Vector3 (3f, 4f, 3f);
			transform.RotateAround(rotationPoint, this.rotateDirection, rotateBy);
			this.rotationTotal += rotateBy;
			
			if(rotationTotal >= 90) {
				this.rotationTotal = -1F;
				this.ResetPosition ();
			}
		}
	}
		//transform.RotateAround(rotationPoint, Vector3.down, 20 * Time.deltaTime);
		//transform.RotateAround(rotationPoint, Vector3.left, 20 * Time.deltaTime);
}
