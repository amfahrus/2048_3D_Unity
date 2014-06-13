//look for FIXME_VAR_TYPE
// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public Transform block;
	public Transform connector;
	private Transform[,,] blocks = new Transform[3,3,3];
	public float scale = 3.5f;

	void  Start (){
		/*
		int x; 
		int y; 
		int z;
		Transform blockInstance;
		Transform connectorInstance;
		TextMesh textMesh;

		//instantiate the blocks and position them
		for (x = 0; x <= 2; x++) {
			for (y = 0; y <= 2; y++) {
				for (z = 0; z <= 2; z++) {
					//blockInstance = Instantiate (block, new Vector3(x * this.scale, y * this.scale, z * this.scale), Quaternion.identity);
					//this.setBlockNumber(blockInstance, -1);
					//this.blocks[x,y,z] = blockInstance;
				}
			}
		}
		/*
		//instantiate the connectors and position them 
		for(FIXME_VAR_TYPE i=0; i <=2; i++) {
			for(FIXME_VAR_TYPE j=0; j<= 2; j++) {
				Instantiate (connector, Vector3(i * this.scale, this.scale, j * this.scale), Quaternion.identity);
				connectorInstance = Instantiate (connector, Vector3(i * this.scale, j * this.scale, this.scale), Quaternion.identity);	
				connectorInstance.Rotate(Vector3(90,0,0));	
				connectorInstance = Instantiate (connector, Vector3(this.scale, i * this.scale, j * this.scale), Quaternion.identity);	
				connectorInstance.Rotate(Vector3(0,0,90));	
			}
		}
		
		//add the first 2 blocks
		this.fillRandomBlock();
		this.fillRandomBlock();
		doMove('x', -1);
		*/
	}
	/*
	void  setBlockNumber ( Transform blockInstance ,   int blockNumber  ){
		BlockScript blockScript; 
		blockScript = blockInstance.gameObject.GetComponent('BlockScript');
		blockScript.setBlockNumber(blockNumber);	
	}
	
	void  getBlockNumber ( Transform blockInstance  ){
		BlockScript blockScript; 
		blockScript = blockInstance.gameObject.GetComponent('BlockScript');
		return blockScript.blockNumber;	
	}
	
	void  getEmptyBlocks (){
		int x; 
		int y; 
		int z;
		int count = 0;
		Hashtable emptyBlocks = new Hashtable();
		for (x = 0;x <= 2; x++) {
			for (y = 0;y <= 2; y++) {
				for (z = 0;z <= 2; z++) {
					if (this.getBlockNumber(this.blocks[x][y][z]) == -1) {
						emptyBlocks[count] = blocks[x][y][z];
						count = count + 1;
					}
				}
			}
		}
		return emptyBlocks;
	}
	
	void  fillRandomBlock (){
		FIXME_VAR_TYPE emptyBlocks= this.getEmptyBlocks();
		int emptyIndex;
		if (emptyBlocks.Count > 0) {
			emptyIndex = Random.Range(0, emptyBlocks.Count);
			this.setBlockNumber(emptyBlocks[emptyIndex], Random.Range(0,3) * 2);
			return true;
		}
		
		//no empty blocks to return
		return false;
		
	}
	
	void  doMove ( string axis ,   int direction  ){
		int x;
		int y;
		int z;
		FIXME_VAR_TYPE rows= new Hashtable();
		Hashtable rowChanges;
		if(axis == 'x') {
			for(y = 0; y <=2; y++) {
				rows[y] = Hashtable();
				for(z = 0; z <=2; z ++) {
					rows[y][z] = Hashtable();
					FIXME_VAR_TYPE blockA= this.blocks[0][y][z];
					FIXME_VAR_TYPE blockB= this.blocks[1][y][z];
					FIXME_VAR_TYPE blockC= this.blocks[2][y][z];
					FIXME_VAR_TYPE numA= getBlockNumber(blockA);
					FIXME_VAR_TYPE numB= getBlockNumber(blockB);
					FIXME_VAR_TYPE numC= getBlockNumber(blockC);
					rowChanges = calculateRowChanges(numA, numB, numC);
					//blockA.move((x, direction * this.scale * rowchanges['a']['shift']);
					//move back then set block number
					this.setBlockNumber(blockA, rowChanges['a']['number']);
				}
			}
		}
	}
	
	//Determine the possible successful combines between 3 blockNumbers (a,b,c) being pushed toward c
	void  calculateRowChanges ( int a ,   int b ,   int c  ){
		FIXME_VAR_TYPE changes= new Hashtable();
		changes['a'] = new Hashtable();
		changes['b'] = new Hashtable();
		changes['c'] = new Hashtable();
		
		//figure out if any of the three merges occurred
		
		if (c > -1 && c == b) {  //b and c merged
			changes['c']['number'] = c*2;
			changes['b']['number'] = a;
			changes['a']['number'] = -1;
			changes['b']['shift'] = 1;
			if(a > -1) {
				changes['a']['shift'] = 1;
			}
			else if (a == -1) {
				changes['a']['shift'] = 0;
			}
		}
		else if (b > -1 && a == b) { //a and b merged
			if(c == -1) {
				changes['c']['number'] = b*2;
				changes['b']['number'] = -1;
				changes['a']['number'] = -1;
				changes['b']['shift'] = 1;
				changes['a']['shift'] = 2;
			}
			else {
				changes['c']['number'] = c;
				changes['b']['number'] = b*2;
				changes['a']['number'] = -1;
				changes['b']['shift'] = 0;
				changes['a']['shift'] = 1;
			}
		}
		else if (c > -1 && a == c && b == -1) {  //a and c merged
			changes['c']['number'] = c*2;
			changes['b']['number'] = -1;
			changes['a']['number'] = -1;
			changes['b']['shift'] = 0;
			changes['a']['shift'] = 2;
		} //end of merges block
		else { //no merges occurred
			if(c > -1) { //last column has number
				changes['c']['number'] = c;
				if(b > -1) { //second column has number
					changes['b']['number'] = b;
					changes['a']['number'] = a;
					changes['b']['shift'] = 0;
					changes['a']['shift'] = 0;
				}
				else if(b == -1) {//second column empty
					changes['b']['number'] = a;
					changes['a']['number'] = -1;
					changes['b']['shift'] = 0;
					if (a == -1) {
						changes['a']['shift'] = 0;
					}
					else {
						changes['a']['shift'] = 1;
					}
				}
				
			} //end of block for value in c column
			else if (c == -1) { //first column empty
				if(b > -1) { //second column has number
					changes['c']['number'] = b;
					changes['b']['number'] = a;
					changes['a']['number'] = -1;
					changes['b']['shift'] = 1;
					if(a > -1) {
						changes['a']['shift'] = 1;
					}
					else {
						changes['a']['shift'] = 0;
					}
					
				}
				else if(b == -1) { //second column empty and first column empty
					changes['c']['number'] = a;
					changes['b']['number'] = -1;
					changes['a']['number'] = -1;
					changes['b']['shift'] = 0;
					if (a == -1) {
						changes['a']['shift'] = 0;
					}
					else {
						changes['a']['shift'] = 2;
					}
				}
			}
		} //end of no merges block	
		
		return changes;
		
	}
	
	void  Update (){
		
	}*/
}
