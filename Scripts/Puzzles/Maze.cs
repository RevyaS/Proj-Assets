using Godot;
using SM = SceneManager;
using System;

public class Maze : Puzzle {
	
//********************************************************************************
//* Initialize methods
	public override void _Ready(){
		timer = GetNode<Timer>("Container/Timer");
		godotBox = GetNode<GodotBox>("GodotBox");
		goldPot = GetNode<Area2D>("GoldPot");
		container = GetNode<GridContainer>("Container");
		
		godotBox.Visible = goldPot.Visible = false;
		ctr = 64;   //* 65 above starts with alphabet letter 'A'
		
		tileScene[0] = getTileInstance("res://Pages/Puzzles/Maze Components/MBlock0.tscn");
		tileScene[1] = getTileInstance("res://Pages/Puzzles/Maze Components/MBlock1Side.tscn");
		tileScene[2] = getTileInstance("res://Pages/Puzzles/Maze Components/MBlock2SidesParallel.tscn");
		tileScene[3] = getTileInstance("res://Pages/Puzzles/Maze Components/MBlock90DegreeAngle2Side.tscn");
		tileScene[4] = getTileInstance("res://Pages/Puzzles/Maze Components/MBlock3Sides.tscn");
		tileScene[5] = getTileInstance("res://Pages/Puzzles/Maze Components/MBlock01234.tscn");
	}
	
	
	//* Initialize a tile and return it
	private StaticBody2D initTile(int x, int y, int level){
		string tileType = "01234";  //! default tile have left(1), top(1), right(3), & bottom(4) sides
		StaticBody2D tile = getSceneTile(tileType);
		
		tile.GetNode<TextureButton>("IconButton").Connect("pressed", this, nameof(onPressed));
		
		tile.Name = $"{y}, {x} -> Path: ";
		tile.EditorDescription = tileType;
		tile.Position = new Vector2(y * 15, x * 15);    //* StaticBody2D type needs position unlike UI interface types that auto positions in parent UI nodes
		
		return tile;
	}
	
	//* Initialize difficulty
	public override void initDifficulty(int level){
		base.initDifficulty(level);
		
		levelVal = getLevelScaleDimensions(level);

		container.RectScale = godotBox.Scale = goldPot.Scale = new Vector2(levelVal[0], levelVal[1]);
		button = new StaticBody2D[container.Columns = col = (int)levelVal[3], row = (int)levelVal[2]];
		
		setContainerPos(levelVal[8], levelVal[9]);
		
		for(int i = 0; i < row; ++i){
			for(int j = 0; j < col; ++j)
				container.AddChild(button[j, i] = initTile(i, j, level));
		}

		//*	Autostart generating the maze
		for(int i = 0; i < 3; ++i)
			button[i, 0].GetNode<TextureButton>("IconButton").EmitSignal("pressed");
	}
	
//*******************************************************************************************
//* Setter methods
	//* Sets a container position
	private void setContainerPos(float x, float y) => container.RectPosition = new Vector2(x, y);
	
	//* Sets the maze dimensions
	private void setDimensions(int row, int col){
		this.row = row;
		container.Columns = this.col = col;
	}
	
	//* Sets a tile correct rotation degrees after changing into a new type as well as the position
	private void setProperTileRotation(string number, StaticBody2D theTile, int row, int col){
		int x = row * 15;
		int y = col * 15;
		
		if(number.Equals("02") || number.Equals("023") || number.Equals("024") || number.Equals("0234"))
			setPosRot(theTile, 90, x, y + 15);
		else if(number.Equals("03") || number.Equals("034") || number.Equals("0134"))
			setPosRot(theTile, 180, x + 15, y + 15);
		else if(number.Equals("04") || number.Equals("014") || number.Equals("0124"))
			setPosRot(theTile, 270, x + 15, y);
		else setPosRot(theTile, 0, x, y);
	}
	
	//* Assigns a position and rotation degrees of a certainTile
	private void setPosRot(StaticBody2D certainTile, int rotDegrees, int row, int col){
		certainTile.RotationDegrees = rotDegrees;
		certainTile.Position = new Vector2(col, row);
	}
	
	//* Set a pathname and a tiles' description/side.
	private void setPathTile(StaticBody2D tileA, StaticBody2D tileB, string str1, string str2){
		setTile(str1, cX, cY);
		setTile(str2, nX, nY);
		pathName(button[cY, cX], button[nY, nX]);
	}
	
	//* Sets a certain tile into a new type of tile
	private void setTile(string numStr, int x, int y){
		StaticBody2D tile = button[y, x];
		Vector2 pos = tile.Position;
		Vector2 scale = tile.Scale;
		string name = tile.Name;
		tile = button[y, x] = SM.gotoScene(container, tile, getSceneTile(numStr)) as StaticBody2D;
		tile.GetNode<TextureButton>("IconButton").Connect("pressed", this, nameof(onPressed));
		
		tile.Name = name;
		tile.Scale = scale;
		tile.EditorDescription = numStr;
		doChange = false;   //* set as false meaning there are some changes in the maze generator container, triggers to stop the loop
		
		setProperTileRotation(numStr, button[y, x], x, y);  //* Sets a 
	}
	
//****************************************************************************************
//* Getter methods
	//* Returns all the scale, dimension, starting, finish, and container's position values based on what level was given.
	private float[] getLevelScaleDimensions(int level) {
		int blockSize = 15; //Size of blocks
		// Determine grid dimensions
		Vector2 dim = (level <= 3) ? new Vector2(5,9) : 
					  (level == 4) ? new Vector2(6, 11) :
					  (level == 5) ? new Vector2(7,13) : 
					  (level == 6) ? new Vector2(9,17) :
					  (level == 7) ? new Vector2(12, 24) :
					  new Vector2(20, 38);

		Vector2 dimTrans = new Vector2(dim.y, dim.x); //Transposed Vector
		float scale = container.RectSize.y / (blockSize * dim.x);
		Vector2 halfBlock = (scale/2) * new Vector2(blockSize, blockSize); //Half a block's size
		Vector2 scaledSize = (dimTrans * blockSize  * scale); //Grid Size after scale
		Vector2 pos = new Vector2((container.RectSize.x - (scale * blockSize * dim.y))/2, 0); //Container positions
		Vector2 sPos = halfBlock + pos; //StartPos
		Vector2 ePos = scaledSize - halfBlock + pos; //EndPos
		return new float[] {scale, scale, dim.x, dim.y, sPos.x, sPos.y, ePos.x, ePos.y, pos.x, pos.y};
	}

	
	//* Returns a scene tile depends of the given number in the parameter
	private StaticBody2D getSceneTile(string number) => 
		(number.Equals("0")) ? duplicateTile(0) : 
		(number.Length.Equals(2)) ? duplicateTile(1) :
		(number.Length.Equals(3)) ? (number.Equals("013") || number.Equals("024")) ? duplicateTile(2) : duplicateTile(3) :
		(number.Length.Equals(4)) ? duplicateTile(4) : duplicateTile(5);
	
	//* Gets a duplicateTile from SceneManager class
	private StaticBody2D duplicateTile(int index) => SM.duplicateScene(tileScene[index]) as StaticBody2D;
	//* Gets an instance tile from SceneManager class
	private StaticBody2D getTileInstance(string path) => SM.getSceneInstance(path) as StaticBody2D;
	
	
//***************************************************************************
//* Signal Methods
	//* Start generating the maze after the tile is being pressed, can be pressed 3x to make the generation faster
	private void onPressed(){
		if(noOfClicks > 2) return;
		
		timer.WaitTime = (noOfClicks > 0) ? timer.WaitTime /= 10 : 0.1F;
		
		++noOfClicks;
		
		if(timer.IsStopped() && doRepeat()) timer.Start();
	}
	
	
	//* Signal method from the timer, wait time = 0.1 sec. Activated after onPressed called
	private void onTimerTimeout(){
		generateMazeKruskal();
		if(!doRepeat()){
			timer.Stop();   //* Timer stops after the maze is completely generated
			GD.Print("Maze Generation done.");
			
			godotBox.Position = new Vector2(levelVal[4], levelVal[5]);
			goldPot.Position = new Vector2(levelVal[6], levelVal[7]);
			godotBox.Visible = goldPot.Visible = true;
		}
	}
	
	//* After reaching the gold pot this methods triggers and solved the maze
	private void onGoldPotBodyEntered(Node body){
		goldPot.QueueFree();
		//* Disables godotBox Control
		godotBox.SetPhysicsProcess(false);
		solved();
	}
	
//**********************************************************************************************
//* Maze methods
	//* Generate a maze using kruskal algorithm
	private void generateMazeKruskal(){
		doChange = true;    //* If there is no changes due to random x and y, it will loop again until something is changing.
		while(doChange) //* Loop if there's nothing change
			makeWay();  //* Create a way of tile or something
	}
	
	//* Create a way or something
	private void makeWay(){
		int x = cX = rnd.Next(0, row), y = cY = rnd.Next(0, col);
		int ltrb = rnd.Next(1, 5);  //* ltrb means left = 1, top = 2, right = 3, bottom = 4
		
		StaticBody2D curTile = button[y, x];
		StaticBody2D leftTile = checkLeft(y) ? button[y - 1, x] : null;
		StaticBody2D topTile = checkTop(x) ? button[y, x - 1] : null;
		StaticBody2D rightTile = checkRight(y) ? button[y + 1, x] : null;
		StaticBody2D bottomTile = checkBottom(x) ? button[y, x + 1] : null;
		string valC = curTile.EditorDescription;
		
		switch(ltrb){
			case 1: //* Make a way from currentTile/centerTile to a LEFT neighbor tile
				if(checkLeft(y) && doAWay(leftTile, curTile)){
					madeWay(
						cX, cY - 1,  leftMatch, 
						new string[] { valC, leftTile.EditorDescription, "1", "3"},
						new StaticBody2D[] { topTile, rightTile, bottomTile, curTile, leftTile }
					);
					
				} break;
			case 2: //* Make a way from currentTile/centerTile to a TOP neighbor tile
				if(checkTop(x) && doAWay(topTile, curTile)){
					madeWay(
						cX - 1, cY,  topMatch, 
						new string[] { valC, topTile.EditorDescription, "2", "4"},
						new StaticBody2D[] { leftTile, rightTile, bottomTile, curTile, topTile }
					);
				} break;
			case 3: //* Make a way from currentTile/centerTile to a RIGHT neighbor tile
				if(checkRight(y) && doAWay(rightTile, curTile)){
					madeWay(
						cX, cY + 1,  rightMatch, 
						new string[] { valC, rightTile.EditorDescription, "3", "1"},
						new StaticBody2D[] { leftTile, topTile, bottomTile, curTile, rightTile}
					);
				} break;
			case 4: //* Make a way from currentTile/centerTile to a BOTTOM neighbor tile
				if(checkBottom(x) && doAWay(bottomTile, curTile)){
					madeWay(
						cX + 1, cY,  bottomMatch, 
						new string[] { valC, bottomTile.EditorDescription, "4", "2"},
						new StaticBody2D[] { leftTile, topTile, rightTile, curTile, bottomTile }
					);
				} break;
		}
	}
	
	//* Decides a tile after the way was made.
	private void madeWay(int nX, int nY, string[][][] matches, string[] sideR, StaticBody2D[] tileList){
		this.nX = nX;
		this.nY = nY;
		decideTile(tileList, matches, sideR);
	}
	
	//* Possible decisions for the currentTile and a certain neighbor tile
	private void decideTile(StaticBody2D[] tile, string[][][] match, string[] matchR){
		if(tile[0] == null && tile[1] == null && tile[2] != null)
			iteratePossibleSides( match[0], matchR, new StaticBody2D[] { tile[3], tile[4] } );
		else if(tile[0] != null && tile[1] == null && tile[2] == null)
			iteratePossibleSides( match[1], matchR, new StaticBody2D[] { tile[3], tile[4] } );
		else if(tile[0] != null && tile[1] == null && tile[3] != null)
			iteratePossibleSides( match[2], matchR, new StaticBody2D[] { tile[3], tile[4] } );
		else if(tile[0] == null && tile[1] != null && tile[2] != null)
			iteratePossibleSides( match[3], matchR, new StaticBody2D[] { tile[3], tile[4] } );
		else if(tile[0] != null && tile[1] != null && tile[2] == null)
			iteratePossibleSides( match[4], matchR, new StaticBody2D[] { tile[3], tile[4] } );
		else if(tile[0] != null && tile[1] != null && tile[2] != null)
			iteratePossibleSides( match[5], matchR, new StaticBody2D[] { tile[3], tile[4] } );
	}
	
	//* Iterates a string array that contains possible sides after a way was made
	private void iteratePossibleSides(string[][] str, string[] val, StaticBody2D[] tile){
		foreach(string str1 in str[0]){
			foreach(string str2 in str[1]){
				if(val[0].Equals(str1) && val[1].Equals(str2)){ //* If both tiles' description == possible side from array, removed 1 side. 
					setPathTile(tile[0], tile[1], str1.Replace(val[2], ""), str2.Replace(val[3], ""));
				}
			}
		}
	}
	
	//* Returns true if there are still grid left then loop otherwise false then stop.
	private bool checkGrid(){
		foreach(StaticBody2D tile in button){
			if(tile.EditorDescription.Length >= 5)
				return true;
		} return false;
	}
	
	//* Check grid is not enough so this methods checks if there is only one tree/edge left and expects that it is a 'A'.
	private bool checkOneWayPath(){
		foreach(StaticBody2D tile in button){
			if(tile.Name[tile.Name.Length - 1] != 'A')
				return false;
		} return true;
	}
	
	//* Name a pathName or tree's name or edge's name after you create a way
	private void pathName(StaticBody2D tile1, StaticBody2D tile2){
		int t1LN = tile1.Name.Length - 1, t2LN = tile2.Name.Length - 1;
		
		if(g64(tile1.Name[t1LN]) && g64(tile2.Name[t2LN])){
			if(tile1.Name[t1LN] > tile2.Name[t2LN])
				replaceAll(tile1.Name[t1LN], tile2.Name[t2LN]);
			else replaceAll(tile2.Name[t2LN], tile1.Name[t1LN]);
		} else if(g64(tile1.Name[t1LN]))
			tile2.Name += tile1.Name[t1LN];
		else if(g64(tile2.Name[t2LN]))
			tile1.Name += tile2.Name[t2LN];
		else{   //* Name an unamed tree
			string temp = ((char)++ctr).ToString();
			tile1.Name += temp;
			tile2.Name += temp;
		}
	}
	
	//* Replaces all certain edge's name to a new one after two trees mix
	private void replaceAll(char toRemove, char replacement){
		foreach(StaticBody2D tile in button){
			if(tile.Name[tile.Name.Length - 1].Equals(toRemove))
				tile.Name = tile.Name.Replace(toRemove, replacement);
		}
	}
	
	
//******************************************************************************
//* Checker methods
	//* Checks a left neighbor if it exist to avoid out of bound index error
	private bool checkLeft(int column) => (column - 1 > -1);
	
	//* Checks a top neighbor if it exist to avoid out of bound index error
	private bool checkTop(int row) => (row - 1 > -1);
	
	//* Checks a right neighbor if it exist to avoid out of bound index error
	private bool checkRight(int column) => (column + 1 < col);
	
	//* Checks a bottom neighbor if it exist to avoid out of bound index error
	private bool checkBottom(int row) => (row + 1 < this.row);
	
//******************************************************************************
	
	//* Decides to make a way or not
	private bool doAWay(StaticBody2D tileA, StaticBody2D tileB) => 
		(tileA.EditorDescription.Length == 5 || !tileA.Name[tileA.Name.Length - 1].Equals(tileB.Name[tileB.Name.Length - 1]));
	//* Decides whether to repeat if no repeat meaning the generation of maze is done
	private bool doRepeat() => !checkOneWayPath() || checkGrid();
	//* I just want to start a name of a tree to 'A' to 'Z' and ends all the mix tree to 'A' also
	private bool g64(int val) => val > 64;
	
//*****************************************************************************************************
//* Global variables
	private GridContainer container;
	private StaticBody2D[,] button;
	private Area2D goldPot;
	private Timer timer;
	private GodotBox godotBox;
	private StaticBody2D[] tileScene =  new StaticBody2D[6];
	private Random rnd = new Random();
	private bool doChange;
	private int row, col, cX, cY,nX, nY, ctr, noOfClicks = 0;
	private float[] levelVal;
	
//* 1 = LEFT, 2 = TOP, 3 = RIGHT, 4 = BOTTOM
	private string[][][] leftMatch = {
		new string[][] { new string[]{ "01234", "0123",},  new string[] { "01234", "0123", "0234" , "023" } },  //* Top right corner
		new string[][] { new string[]{ "01234", "0134",},  new string[] { "01234", "0134", "0234" , "034" } },  //* Bottom right corner
		new string[][] { new string[]{ "01234", "0123", "0134", "013" },  new string[] { "01234", "0123", "0134" , "0234" , "013", "023", "034", "03" } },  //* Right area
		new string[][] { new string[]{ "01234", "0123", "0124", "012" },  new string[] { "01234", "0123", "0234" , "023" } },   //* Top area
		new string[][] { new string[]{ "01234", "0124", "0134", "014" },  new string[] { "01234", "0134", "0234", "034" } },    //* Bottom area
		new string[][] { new string[]{ "01234", "0123", "0124", "134", "012", "013", "014", "01" },  new string[] { "01234", "0123", "0134", "0234", "013", "023", "034", "03" } }, //* Center area
	};
	private string[][][] topMatch = {
		new string[][] { new string[]{ "01234", "0124" },  new string[] { "01234", "0124", "0134" , "014" } },  //* Bottom left corner
		new string[][] { new string[]{ "01234", "0234" },  new string[] { "01234", "0134", "0234" , "034" } },  //* Bottom right corner
		new string[][] { new string[]{ "01234", "0124", "0234", "024" },  new string[] { "01234", "0124", "0134" , "0234", "014", "024", "034", "04" } },   //* Bottom area
		new string[][] { new string[]{ "01234", "0123", "0124", "012" },  new string[] { "01234", "0124", "0134" , "014" } },  //* Left area 
		new string[][] { new string[]{ "01234", "0123", "0234", "023" },  new string[] { "01234", "0134", "0234" , "034" } },   //* Right area
		new string[][] { new string[]{ "01234", "0123", "0124", "0234", "012", "023", "024", "02" },  new string[] { "01234", "0124", "0134", "0234" , "014", "024", "034", "04" } },   //* Center area
	};
	private string[][][] rightMatch = {
		new string[][] { new string[]{ "01234", "0123" },  new string[] { "01234", "0123", "0124", "012" } },   //* Top left corner
		new string[][] { new string[]{ "01234", "0134" },  new string[] { "01234", "0124", "0134", "014" } },   //* Bottom left corner
		new string[][] { new string[]{ "01234", "0123", "0134", "013" },  new string[] { "01234", "0123", "0124", "0134", "012", "013", "014", "01" } },    //* Left area
		new string[][] { new string[]{ "01234", "0123", "0234", "023" },  new string[] { "01234", "0123", "0124", "012" } },    //* Top area
		new string[][] { new string[]{ "01234", "0134", "0234", "034" },  new string[] { "01234", "0124", "0134", "014" } },    //* Bottom area
		new string[][] { new string[]{ "01234", "0123", "0134", "0234", "013", "023", "034", "03" },  new string[] { "01234", "0123","0124", "0134", "012", "013","014", "01" } },  // //* Center area
	};
	private string[][][] bottomMatch = {
		new string[][] { new string[]{ "01234", "0124" },  new string[] { "01234", "0123", "0124", "012" } },    //* Top left corner
		new string[][] { new string[]{ "01234", "0234" },  new string[] { "01234", "0123", "0234", "023" } },   //* Top right corner
		new string[][] { new string[]{ "01234", "0124", "0234", "024" },  new string[] { "01234", "0123", "0124", "0234", "012", "023", "024", "02" } },    //* Top area
		new string[][] { new string[]{ "01234", "0124", "0134", "014" },  new string[] { "01234", "0123", "0124", "012" } },    //* Left area
		new string[][] { new string[]{ "01234", "0134", "0234", "034" },  new string[] { "01234", "0123", "0234", "024" } },    //* Right area
		new string[][] { new string[]{ "01234", "0124", "0134", "0234", "014", "024", "034", "04" },  new string[] { "01234", "0123", "0124", "0234", "012", "023", "024", "02" } },    //* Center area
	};
}
