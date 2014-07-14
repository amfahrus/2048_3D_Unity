//look for FIXME_VAR_TYPE
// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class GameControllerScript : MonoBehaviour {

	public Transform block;
	public Transform connector;
	private Transform[,,] blocks = new Transform[3,3,3];
	private float moveStartTime = -1F;
	public float scale = 3.5F;
	public bool moving = false;
	private float moveDuration = 0.2F;
	private float yOffset;
	private int score;
	public GUISkin currentGUISkin;
	public static bool performRestart = false;
	private bool initialized = false;
	public string gameView = "game";
	public Camera mainCamera;
	private Vector2 scrollPosition;
	public Light gameLight;
	public AudioClip swipeSoundX;
	public AudioClip swipeSoundY;
	public AudioClip swipeSoundZ;
	public AudioClip collideSound;

	
	private bool optionsUse0;
	private bool previousOptionsUse0;
	private bool optionsPlaySounds;
	private bool previousOptionsPlaySounds;

	//instruction text
	private string instructionText = @"The object of 2048-3D is to"
		+ " slide all the blocks in such a way"
			+ " that blocks with the same numbers collide"
			+ " forming a new block that is twice"
			+ " as much as the originals until"
			+ " the number 2048 is reached.\n\n"
			
			+ "To slide the blocks simply swipe up,"
			+ " down, left or right (keyboard: arrow keys). To move the"
			+ " blocks forward and backward use the" 
			+ " big red arrow keys at the bottom of the screen (keyboard: a and z keys).  " 
			+ " When moving, all blocks that can slide in the chosen direction will move"
			+ " simultaneously, and any blocks moving toward a block with the same number will collide. "
			+ " Two blocks with the number 0 will collide but never"
			+ " increase higher than 0. You can turn"
			+ " off 0s in the game options tab.\n\n"

			+ "For every collision you receive"
			+ " the number of points of the newly"
			+ " created block. After each move is"
			+ " made a new number (either 0, 2 or 4)"
			+ " will be randomly assigned to an"
			+ " empty slot.  If after making a move"
			+ " all blocks are filled and no new"
			+ " moves are possible, the game ends. \n\n"

			+ "2048-3D is based upon the original"
			+ " 2048 game designed by Gabriele Cirulli.\n\n" 
			+ "Soud effects by freeSFX http://www.freesfx.co.uk";
	void  Start (){

		int x; 
		int y; 
		int z;
		yOffset = 1F;
		Transform blockInstance;
		BlockScript blockScript; 
		Transform connectorInstance;
		TextMesh textMesh;

		this.sizeGUI();
		this.InitOptions ();
		this.setScore (0);

		//instantiate the blocks and position them
		for (x = 0; x <= 2; x++) {
			for (y = 0; y <= 2; y++) {
				for (z = 0; z <= 2; z++) {
					blockInstance = Instantiate (block, new Vector3(x * this.scale, y * this.scale + this.yOffset, z * this.scale), Quaternion.identity) as Transform;
					this.blocks[x,y,z] = blockInstance;
					blockScript = blockInstance.gameObject.GetComponent("BlockScript") as BlockScript;
					blockScript.originalPosition = new Vector3(x * this.scale, y * this.scale + this.yOffset, z * this.scale);
					blockScript.x = x;
					blockScript.y = y;
					blockScript.z = z;
					this.setBlockNumber(blockInstance, -1);
					DontDestroyOnLoad(blockInstance);
				}
			}
		}
	
		//instantiate the connectors and position them 
		for(int i=0; i <=2; i++) {
			for(int j=0; j<= 2; j++) {
				connectorInstance = Instantiate (connector, new Vector3(i * this.scale, this.scale + this.yOffset, j * this.scale), Quaternion.identity) as Transform;
				connectorInstance.renderer.material.color = new Color(0,0.4f,0);
				connectorInstance = Instantiate (connector, new Vector3(i * this.scale, j * this.scale + this.yOffset, this.scale), Quaternion.identity) as Transform;	
				connectorInstance.Rotate(new Vector3(90,0,0));
				connectorInstance.renderer.material.color = new Color(0,0.4f,0);
				connectorInstance = Instantiate (connector, new Vector3(this.scale, i * this.scale + this.yOffset, j * this.scale), Quaternion.identity) as Transform;	
				connectorInstance.Rotate(new Vector3(0,0,90));
				connectorInstance.renderer.material.color = new Color(0,0.4f,0);
			}
		}

		//instantiate the move blocks
		this.restart ();

	}

	void setBlockNumber ( Transform blockInstance ,   int blockNumber  ){
		BlockScript blockScript; 
		blockScript = blockInstance.gameObject.GetComponent("BlockScript") as BlockScript;
		blockScript.setBlockNumber(blockNumber);	
	}

	private int getBlockNumber ( Transform blockInstance  ){
		BlockScript blockScript; 
		blockScript = blockInstance.gameObject.GetComponent("BlockScript") as BlockScript;
		return blockScript.blockNumber;	
	}

	private List<Transform>  getEmptyBlocks (){
		int x; 
		int y; 
		int z;
		int count = 0;
		List<Transform> emptyBlocks = new List<Transform>();
		for (x = 0; x <= 2; x++) {
			for (y = 0; y <= 2; y++) {
				for (z = 0; z <= 2; z++) {
					if (this.getBlockNumber(this.blocks[x,y,z]) == -1) {
						emptyBlocks.Add(this.blocks[x,y,z]);
						count = count + 1;
					}
				}
			}
		}
		return emptyBlocks;
	}

	private bool fillRandomBlock (ref int x, ref int y, ref int z, ref int newNumber) {
		List<Transform> emptyBlocks = this.getEmptyBlocks();
		int emptyIndex;
		BlockScript blockScript; 

		if (emptyBlocks.Count > 0) {
			emptyIndex = Random.Range(0, emptyBlocks.Count);
			if(this.optionsUse0) {
				newNumber = Random.Range(0,3) * 2;
			}
			else {
				newNumber = Random.Range(1,3) * 2;
				this.setBlockNumber(emptyBlocks[emptyIndex], Random.Range(1,3) * 2);
			}
			this.setBlockNumber(emptyBlocks[emptyIndex], Random.Range(0,3) * 2);
			blockScript = emptyBlocks[emptyIndex].GetComponent("BlockScript") as BlockScript;
			x = blockScript.x;
			y = blockScript.y;
			z = blockScript.z;
			return true;
		}

		//no empty blocks to return
		return false;	
	}

	public void doMove (string axis, int direction) {
		int loop1, loop2;  //looping variables
		int x, y, z;  //block location variables
		int numMoves = 0; //number of moves to make
		int scoreChange = 0;
		bool blockCollision = false;
		bool blockCollisionSound = false;
		Transform blockA, blockB, blockC;  //variables for holding the three blocks in a row
		IDictionary<string, int> newNumbers = new Dictionary<string, int>(); //new numbers for blocks a,b & c to have
		IDictionary<string, int> shiftBy = new Dictionary<string, int>(); //distance for blocks a,b & c to shift
		int[,,] numbersAfterMove = new int[3,3,3];

		//loop through each of the 9 rows to be calculated 
		for(loop1 = 0; loop1 <=2; loop1++) {
			for(loop2 = 0; loop2 <=2; loop2++) {
				if (axis == "x") {
					y = loop1;
					z = loop2;
					if (direction == 1) {
						blockA= this.blocks[0,y,z];
						blockB= this.blocks[1,y,z];
						blockC= this.blocks[2,y,z];
					}
					else {
						blockA= this.blocks[2,y,z];
						blockB= this.blocks[1,y,z];
						blockC= this.blocks[0,y,z];
					}
				}
				else if (axis == "y") {
					x = loop1;
					z = loop2;
					if (direction == 1) {
						blockA= this.blocks[x,0,z];
						blockB= this.blocks[x,1,z];
						blockC= this.blocks[x,2,z];
					}
					else {
						blockA= this.blocks[x,2,z];
						blockB= this.blocks[x,1,z];
						blockC= this.blocks[x,0,z];
					}
				}
				else  {
					x = loop1;
					y = loop2;
					if (direction == 1) {
						blockA= this.blocks[x,y,0];
						blockB= this.blocks[x,y,1];
						blockC= this.blocks[x,y,2];
					}
					else {
						blockA= this.blocks[x,y,2];
						blockB= this.blocks[x,y,1];
						blockC= this.blocks[x,y,0];
					}
				}
				calculateRowChanges(this.getBlockNumber(blockA), this.getBlockNumber(blockB), this.getBlockNumber(blockC), ref scoreChange, ref newNumbers, ref shiftBy, ref blockCollision);

				blockA.GetComponent<BlockScript>().move (axis, shiftBy["a"] * direction, this.scale, newNumbers["a"]);
				blockB.GetComponent<BlockScript>().move (axis, shiftBy["b"] * direction, this.scale, newNumbers["b"]);
				blockC.GetComponent<BlockScript>().move (axis, shiftBy["c"] * direction, this.scale, newNumbers["c"]);

				numMoves = numMoves + shiftBy["a"] + shiftBy["b"] + shiftBy["c"];
				if (blockCollision) blockCollisionSound = true;
				if(newNumbers["a"] > this.getHighestBlock()) this.SetHighestBlock(newNumbers["a"]);
				if(newNumbers["b"] > this.getHighestBlock()) this.SetHighestBlock(newNumbers["b"]);
				if(newNumbers["c"] > this.getHighestBlock()) this.SetHighestBlock(newNumbers["c"]);

				//save the numbers after the move for redo
				numbersAfterMove[blockA.GetComponent<BlockScript>().x,
				                 blockA.GetComponent<BlockScript>().y,
				                 blockA.GetComponent<BlockScript>().z] = newNumbers["a"];
				
				numbersAfterMove[blockB.GetComponent<BlockScript>().x,
				                 blockB.GetComponent<BlockScript>().y,
				                 blockB.GetComponent<BlockScript>().z] = newNumbers["b"];
				
				numbersAfterMove[blockC.GetComponent<BlockScript>().x,
				                 blockC.GetComponent<BlockScript>().y,
				                 blockC.GetComponent<BlockScript>().z] = newNumbers["c"];
			}
		}
		if (numMoves > 0) {
			this.moveStartTime = Time.time;
			this.setScore (this.score + scoreChange);
			if(this.optionsPlaySounds) {
				if (axis == "x") audio.PlayOneShot(swipeSoundX);
				if (axis == "y") audio.PlayOneShot(swipeSoundY);
				if (axis == "z") audio.PlayOneShot(swipeSoundZ);
			}
		}
		if(blockCollisionSound && this.optionsPlaySounds) {
			audio.clip = collideSound;
			audio.PlayDelayed(this.moveDuration);
		}
	}

	//Determine the possible successful combines between 3 blockNumbers (a,b,c) being pushed toward c
	void calculateRowChanges (int a, int b, int c, ref int scoreChange, ref IDictionary<string, int> newNumbers, ref IDictionary<string, int> shiftBy, ref bool blockCollision ) {
		blockCollision = false;
		//figure out if any of the three merges occurred
		shiftBy ["c"] = 0;
		if (c > -1 && c == b) {  //b and c merged
			scoreChange = scoreChange + c*2;
			blockCollision = true;
			newNumbers["c"] = c*2;
			newNumbers["b"] = a;
			newNumbers["a"] = -1;
			shiftBy["b"] = 1;
			if(a > -1) {
				shiftBy["a"] = 1;
			}
			else if (a == -1) {
				shiftBy["a"] = 0;
			}
		}
		else if (b > -1 && a == b) { //a and b merged
			scoreChange =  scoreChange + a*2;
			blockCollision = true;
			if(c == -1) {
				newNumbers["c"] = b*2;
				newNumbers["b"] = -1;
				newNumbers["a"] = -1;
				shiftBy["b"] = 1;
				shiftBy["a"] = 2;
			}
			else {
				newNumbers["c"] = c;
				newNumbers["b"] = b*2;
				newNumbers["a"] = -1;
				shiftBy["b"] = 0;
				shiftBy["a"] = 1;
			}
		}
		else if (c > -1 && a == c && b == -1) {  //a and c merged
			scoreChange =  scoreChange  + c*2;
			blockCollision = true;
			newNumbers["c"] = c*2;
			newNumbers["b"] = -1;
			newNumbers["a"] = -1;
			shiftBy["b"] = 0;
			shiftBy["a"] = 2;
		} //end of merges block
		else { //no merges occurred
			if(c > -1) { //last column has number
				newNumbers["c"] = c;
				if(b > -1) { //second column has number
					newNumbers["b"] = b;
					newNumbers["a"] = a;
					shiftBy["b"] = 0;
					shiftBy["a"] = 0;
				}
				else if(b == -1) {//second column empty
					newNumbers["b"] = a;
					newNumbers["a"] = -1;
					shiftBy["b"] = 0;
					if (a == -1) {
						shiftBy["a"] = 0;
					}
					else {
						shiftBy["a"] = 1;
					}
				}
				
			} //end of block for value in c column
			else if (c == -1) { //first column empty
				if(b > -1) { //second column has number
					newNumbers["c"] = b;
					newNumbers["b"] = a;
					newNumbers["a"] = -1;
					shiftBy["b"] = 1;
					if(a > -1) {
						shiftBy["a"] = 1;
					}
					else {
						shiftBy["a"] = 0;
					}
					
				}
				else if(b == -1) { //second column empty and first column empty
					newNumbers["c"] = a;
					newNumbers["b"] = -1;
					newNumbers["a"] = -1;
					shiftBy["b"] = 0;
					if (a == -1) {
						shiftBy["a"] = 0;
					}
					else {
						shiftBy["a"] = 2;
					}
				}
			}
		} //end of no merges block	

	}


	void  Update (){
		int x =0, y=0, z=0, newNumber=0;
		if(GameControllerScript.performRestart) {
			this.restart ();
			GameControllerScript.performRestart = false;
		}
		if(moveStartTime >= 0) {
			if(Time.time - this.moveStartTime > this.moveDuration + 0.05F) {
				this.fillRandomBlock(ref x, ref y, ref z, ref newNumber);
				this.saveHistory ();
				this.moveStartTime = -1F;
				if(this.getEmptyBlocks().Count == 0) {
					if(this.CheckGameOver()) {
						this.gameView = "game_over";
					}
				}
			}
		}
		
		if (Input.GetKeyUp("right")) this.doMove ("x", 1);
		if (Input.GetKeyUp("left")) this.doMove ("x", -1);
		if (Input.GetKeyUp("up")) this.doMove ("y", 1);
		if (Input.GetKeyUp("down")) this.doMove ("y", -1);
		if (Input.GetKeyUp("a")) this.doMove ("z", 1);
		if (Input.GetKeyUp("z")) this.doMove ("z", -1);

		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	void FixedUpdate () {
	}

	void setScore(int score) {
		this.score = score;
		if(this.score > this.GetHighScore()) {
			this.SetHighScore(this.score);
		}
	}

	public void restart() {
		int[,,] positions = new int[3, 3, 3];
		int x=0, y=0, z=0, newNumber=0;
		this.gameLight.intensity = 4;
		if(this.gameView == "game_over") { this.gameView = "game"; }

		for (x = 0; x <= 2; x++) {
			for (y = 0; y <= 2; y++) {
				for (z = 0; z <= 2; z++) {
					this.setBlockNumber(this.blocks[x,y,z], -1);
					positions[x,y,z] = -1;
				}
			}
		}
		PlayerPrefs.SetString ("redo_moves2","");
		PlayerPrefs.SetString ("redo_moves1","");
		PlayerPrefs.SetString ("redo_moves0","");
		PlayerPrefs.SetInt ("redos", 2);
		this.fillRandomBlock(ref x, ref y, ref z, ref newNumber);
		positions [x, y, z] = newNumber;
		this.fillRandomBlock(ref x, ref y, ref z, ref newNumber);
		positions [x, y, z] = newNumber;
		this.saveHistory();

		this.setScore (0);
	}

	public int GetHighScore()
	{
		string key = "highscore-" + PlayerPrefs.GetInt ("options_use_0").ToString();
		if (PlayerPrefs.HasKey (key)) {
			return PlayerPrefs.GetInt (key);
		}
		else {
			this.SetHighScore(0);
			return 0;
		}
	}

	void SetHighScore(int myHighScore)
	{
		string key = "highscore-" + PlayerPrefs.GetInt ("options_use_0").ToString();
		PlayerPrefs.SetInt( key, myHighScore );
		PlayerPrefs.Save();
	}

	public int getHighestBlock()
	{
		string key = "highestblock-" + PlayerPrefs.GetInt ("options_use_0").ToString();
		if (PlayerPrefs.HasKey (key)) {
			return PlayerPrefs.GetInt (key);
		}
		else {
			this.SetHighestBlock(0);
			return 0;
		}
	}
	
	void SetHighestBlock(int highestBlock)
	{
		string key = "highestblock-" + PlayerPrefs.GetInt ("options_use_0").ToString();
		PlayerPrefs.SetInt( key, highestBlock );
		PlayerPrefs.Save();
	}

	private bool CheckGameOver() {

		string[] axisList = new string[3] {"x", "y", "z"};
		int direction = 1;
		string axis = "x";
		int loop1, loop2, loopAxis;  //looping variables
		int x, y, z;  //block location variables
		int numMoves = 0; //number of moves to make
		int scoreChange = 0;
		bool blockCollision = false;
		Transform blockA, blockB, blockC;  //variables for holding the three blocks in a row
		IDictionary<string, int> newNumbers = new Dictionary<string, int>(); //new numbers for blocks a,b & c to have
		IDictionary<string, int> shiftBy = new Dictionary<string, int>(); //distance for blocks a,b & c to shift
		
		//loop through each of the 9 rows to be calculated
		for(loopAxis = 0; loopAxis < 3; loopAxis ++) {
			axis = axisList[loopAxis];
		for(direction = -1; direction < 2; direction += 2) {
		for(loop1 = 0; loop1 <=2; loop1++) {
		for(loop2 = 0; loop2 <=2; loop2++) {
			if (axis == "x") {
				y = loop1;
				z = loop2;
				if (direction == 1) {
					blockA= this.blocks[0,y,z];
					blockB= this.blocks[1,y,z];
					blockC= this.blocks[2,y,z];
				}
				else {
					blockA= this.blocks[2,y,z];
					blockB= this.blocks[1,y,z];
					blockC= this.blocks[0,y,z];
				}
			}
			else if (axis == "y") {
				x = loop1;
				z = loop2;
				if (direction == 1) {
					blockA= this.blocks[x,0,z];
					blockB= this.blocks[x,1,z];
					blockC= this.blocks[x,2,z];
				}
				else {
					blockA= this.blocks[x,2,z];
					blockB= this.blocks[x,1,z];
					blockC= this.blocks[x,0,z];
				}
			}
			else  {
				x = loop1;
				y = loop2;
				if (direction == 1) {
					blockA= this.blocks[x,y,0];
					blockB= this.blocks[x,y,1];
					blockC= this.blocks[x,y,2];
				}
				else {
					blockA= this.blocks[x,y,2];
					blockB= this.blocks[x,y,1];
					blockC= this.blocks[x,y,0];
				}
			}
			calculateRowChanges(this.getBlockNumber(blockA), this.getBlockNumber(blockB), this.getBlockNumber(blockC), ref scoreChange, ref newNumbers, ref shiftBy, ref blockCollision);
			
			numMoves = numMoves + shiftBy["a"] + shiftBy["b"] + shiftBy["c"];
		}
		}
		}
		}
		if (numMoves > 0) {
			return false;
		}
		else {
			return true;
		}
	}

	void OnGUI() {
		//GAME GUI
		if (this.gameView == "game") {
			this.ShowGame();
		}
		//OPTIONS GUI
		if (this.gameView == "options") {
			this.ShowOptions();
		}
		//INSTRUCTIONS GUI
		if (this.gameView == "instructions") {
			this.ShowInstructions();
		}
		if(this.gameView == "game_over") {
			this.ShowGameOver();
		}
	}

	void ShowInstructions() {
		GUI.skin = currentGUISkin;
		
		this.mainCamera.transform.eulerAngles = new Vector3 (180, 23, 0);

		GUIStyle labelStyle = new GUIStyle();
		GUILayout.Label ("Instructions", "BigLabel");
		
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Mathf.Ceil(Screen.height * .80f)));
		GUILayout.Label(this.instructionText);
		
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Moved)
			{
				// dragging
				scrollPosition.y += touch.deltaPosition.y;
			}
		}
		GUILayout.EndScrollView();

		if (GUILayout.Button ("Return to Game")) {
			this.gameView = "game";
		}
	}

	void ShowGame() {
		GUI.skin = currentGUISkin;
		this.gameLight.intensity = 4;
		this.mainCamera.transform.eulerAngles = new Vector3 (19F, 29.5F, 0);

		GUI.Label (new Rect (0, 0, Screen.width, Screen.height * 0.06F), "Score: " + this.score.ToString (), "BigLabel");
		string highScoreText = "High Score/Block: " + this.GetHighScore ().ToString () + " / " + this.getHighestBlock ().ToString ();
		GUI.Label (new Rect (0, Screen.height * 0.06F, Screen.width, Screen.height / 10), highScoreText, "SmallLabel");

		
		GUIStyle style = currentGUISkin.GetStyle ("button");
		style.fontSize = 14;

		if (GUI.Button(new Rect(1, Screen.height * 0.12F, Screen.width * 0.30F, Screen.height * 0.06F),"Options")) {
			this.gameView = "options";
		}
		if (GUI.Button(new Rect(Screen.width * .33f, Screen.height * 0.12F, Screen.width * 0.33F, Screen.height * 0.06F),"Instructions")) {
			this.gameView = "instructions";
		}
		if (GUI.Button(new Rect(Screen.width * .7f, Screen.height * 0.12F, Screen.width * .30f, Screen.height * 0.06F), "Restart")) {
			this.restart();
		}
		if (GUI.Button(new Rect(Screen.width * .05f, Screen.height * 0.90F, Screen.width * .3f, Screen.height * 0.06F), "Undo (" + PlayerPrefs.GetInt ("redos").ToString () + ")")) {
			this.undo();
		}
	}

	void ShowOptions() {
		GUI.skin = currentGUISkin;
		this.mainCamera.transform.eulerAngles = new Vector3 (180, 23, 0);
		
		//check to see if any options changed
		if (previousOptionsUse0 != optionsUse0) {
			PlayerPrefs.SetInt ("options_use_0", (optionsUse0) ? 1 : 0);
			previousOptionsUse0 = optionsUse0;
			GameControllerScript.performRestart = true;
		}

		
		if (previousOptionsPlaySounds != optionsPlaySounds) {
			PlayerPrefs.SetInt ("options_play_sounds", (optionsPlaySounds) ? 1 : 0);
			previousOptionsPlaySounds = optionsPlaySounds;
		}
		
		//set the label 
		GUILayout.Label ("Options", "BigLabel");

		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Mathf.Ceil(Screen.height * .80f)));
		

			GUILayout.BeginHorizontal ();
				if (GUILayout.Toggle (optionsUse0, "Use 0s (Note: Changing this will reset the current game!)", currentGUISkin.toggle)) {
					optionsUse0 = true;		
				}
				else {
					optionsUse0 = false;
				}
				GUILayout.Label ( "Use 0s (Note: Changing this will reset the current game!)", "ToggleLabel");
			GUILayout.EndHorizontal();
	

			GUILayout.BeginHorizontal ();
				if (GUILayout.Toggle (this.optionsPlaySounds, "Play Sounds", currentGUISkin.toggle)) {
					this.optionsPlaySounds = true;		
				}
				else {
					this.optionsPlaySounds = false;
				}
				GUILayout.Label ( "Play Sounds", "ToggleLabel");
			GUILayout.EndHorizontal();

		foreach (Touch touch in Input.touches) {
				if (touch.phase == TouchPhase.Moved)
				{
					// dragging
					scrollPosition.y += touch.deltaPosition.y;
				}
			}
		GUILayout.EndScrollView();

		if (GUILayout.Button ("Return to Game")) {
			this.gameView = "game";
		}

	}
	void ShowGameOver() {
		this.ShowGame ();
		GUI.skin = currentGUISkin;
		GUI.Label (new Rect (0, Screen.height /2 , Screen.width, Screen.height * 0.10F), "Game Over", "BigLabel");
		this.gameLight.intensity = 0;
	}


	private void sizeGUI() {
		this.currentGUISkin.GetStyle ("Label").fontSize = Mathf.CeilToInt(Screen.height * 0.04F);
		this.currentGUISkin.GetStyle ("Button").fontSize = Mathf.CeilToInt(Screen.height * 0.04F);
		this.currentGUISkin.GetStyle ("Toggle").fontSize = Mathf.CeilToInt(Screen.height * 0.04F);
		this.currentGUISkin.GetStyle ("ToggleLabel").fontSize = Mathf.CeilToInt(Screen.height * 0.04F);

		this.currentGUISkin.GetStyle ("SmallLabel").fontSize = Mathf.CeilToInt(Screen.height * 0.03F);
		
		this.currentGUISkin.GetStyle ("BigLabel").fontSize = Mathf.CeilToInt(Screen.height * 0.06F);

		this.currentGUISkin.GetStyle ("Toggle").padding.top = Mathf.CeilToInt(Screen.height * 0.04F);
		this.currentGUISkin.GetStyle ("Toggle").padding.left = Mathf.CeilToInt(Screen.height * 0.04F);
	}

	private void InitOptions() {
		if (!PlayerPrefs.HasKey ("options_use_0")) {
			PlayerPrefs.SetInt ("options_use_0",1);
			PlayerPrefs.Save();
			this.optionsUse0 = true;
			this.previousOptionsUse0 = true;
		}
		else {
			if(PlayerPrefs.GetInt ("options_use_0") == 1) {
				this.optionsUse0 = true;
				this.previousOptionsUse0 = true;
			}
			else {
				this.optionsUse0 = false;
				this.previousOptionsUse0 = false;
			}
		}
		if (!PlayerPrefs.HasKey ("options_play_sounds")) {
			PlayerPrefs.SetInt ("options_play_sounds",1);
			PlayerPrefs.Save();
			this.optionsPlaySounds = true;
			this.previousOptionsPlaySounds = true;
		}
		else {
			if(PlayerPrefs.GetInt ("options_play_sounds") == 1) {
				this.optionsPlaySounds = true;
				this.previousOptionsUse0 = true;
			}
			else {
				this.optionsPlaySounds = false;
				this.previousOptionsPlaySounds = false;
			}
		}
	}

	private void saveHistory() {
		int x,y,z;
		string stringVal = "";

		//create comma separated list of values
		for (x = 0; x <= 2; x++) {
			for (y = 0; y <= 2; y++) {
				for (z = 0; z <= 2; z++) {
					stringVal = stringVal + getBlockNumber(this.blocks[x,y,z]).ToString() + ",";
				}
			}
		}
		//strip off the last comma
		stringVal = stringVal.Substring (0, stringVal.Length - 1);

		PlayerPrefs.SetString ("redo_moves2",PlayerPrefs.GetString ("redo_moves1"));
		PlayerPrefs.SetString ("redo_moves1",PlayerPrefs.GetString ("redo_moves0"));
		PlayerPrefs.SetString ("redo_moves0",stringVal);

		PlayerPrefs.Save();
	}

	private void undo() {
		string[] positions = new string[27];
		int positionNum = 0;
		char[] separator = {','};

		if( PlayerPrefs.GetString ("redo_moves1") != "" && PlayerPrefs.GetInt ("redos") > 0) {
			positions = PlayerPrefs.GetString ("redo_moves1").Split (separator);
			//loop through and set the block numbers for each block
			for (int x = 0; x <= 2; x++) {
				for (int y = 0; y <= 2; y++) {
					for (int z = 0; z <= 2; z++) {
						int.TryParse(positions[9*x+3*y+z], out positionNum);
						this.setBlockNumber(this.blocks[x,y,z],positionNum);
					}
				}
			}

			//move all the redo moves back
			PlayerPrefs.SetString ("redo_moves0",PlayerPrefs.GetString ("redo_moves1"));
			PlayerPrefs.SetString ("redo_moves1",PlayerPrefs.GetString ("redo_moves2"));
			PlayerPrefs.SetString ("redo_moves2","");
			PlayerPrefs.SetInt ("redos", PlayerPrefs.GetInt ("redos") - 1);
		}
	}
}

