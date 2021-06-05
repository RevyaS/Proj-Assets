using Godot;
using SM = SceneManager;
using UF = UtilityFunctions;
using GA = Godot.Collections.Array;
using System;

public class SlidingPuzzle : Puzzle{
	
//*****************************************************************
//* Initializer methods
	public override void _Ready(){
		container = GetNode<GridContainer>("GridContainer");
		timer = GetNode<Timer>("Timer");
		whole = GetNode<TextureRect>("Whole");
		//* Sets as 1, 1 to avoid conflict
		pPos = new int[] { 1, 1 };
		
	}
	
	
	//* Initialize difficulty
	public override void initDifficulty(int level){
		base.initDifficulty(level);
		
		if(level > 2 && level < 5){
			path += "Normal/0";
			
			row = 3;
			col = 3;
		} else if(level > 4 && level < 7){
			path += "Hard/0";
			row = 3;
			container.Columns = col = 4;
			container.RectPosition =  new Vector2(80, 35);
		} else if(level > 6 && level < 10){
			path += "Expert/0";
			
			row = 3;
			container.Columns = col = 5;
			container.RectPosition = new Vector2(35, 35);
			whole.RectPosition = new Vector2(440, 104);
		} 
		
		path += rnd.Next(1, 6) + "/";
		whole.Texture = UF.getTexture(ProjectSettings.GlobalizePath(path + "/whole.jpg"));
		
		tiles = new TextureButton[col, row];
		
		for(int i = 0; i < row; ++i){
			for(int j = 0; j < col; ++j)
				container.AddChild(tiles[j, i] = getTile(i, j));
		}
		
	}
	
	private void onDelayTimeout()
		=> timer.Start();
	
	private TextureButton checkNoTile(params int[] x){
		//*	LEFT
		if(canMove(x[1] - 1, x[0]) && tiles[x[1] - 1, x[0]].EditorDescription.Equals("Way Labot"))
			return tiles[x[1] - 1, x[0]];
		//*	TOP
		if(canMove(x[1], x[0] - 1) && tiles[x[1], x[0] - 1].EditorDescription.Equals("Way Labot"))
			return tiles[x[1], x[0] - 1];
		//*	RIGHT
		if(canMove(x[1] + 1, x[0]) && tiles[x[1] + 1, x[0]].EditorDescription.Equals("Way Labot"))
			return tiles[x[1] + 1, x[0]];
		//*	BOTTOM
		if(canMove(x[1], x[0] + 1) && tiles[x[1], x[0] + 1].EditorDescription.Equals("Way Labot"))
			return tiles[x[1], x[0] + 1];
		return null;
	}
	
//***********************************************************************
//* Signal Methods
	//* Select, Unselect or move a tile
	private void onPressed(int i, int j){
		TextureButton tile1 = tiles[j, i];
		TextureButton tile2 = null;
		
		if((tile2 = checkNoTile(i, j)) == null)
			return;
		
		moveATile(tile1, tile2);;
		
		// //* Solved if the puzzle is being solved
		if(checkWin()){
		//     solved();
			GD.Print("WIN");
		}
	}
	
	private bool isAllVisited(){
		foreach(TextureButton b in tiles){
			if(b.EditorDescription.Contains("Tile "))
				return false;
		} return true;
	}
	
	//* Signal method from the timer's method onTimeOut(), auto disarrange the tile before the game starts
	private void disArrangeTiles(){
		bool done = false;
		for(int i = 0; !done && i < row; ++i){
			for(int j = 0; !done && j < col; ++j){
				if(tiles[j, i].EditorDescription.Equals("Way Labot")){
					int temp, k, l;
					do{
						temp = rnd.Next(1, 5);
							
						k = i; l = j;
						if(temp == 1){  //* Move to right
							if(isAllVisited())
								continue;
							if(j + 1 < col)
								l += 1;
						} else if(temp == 2){   //* Move to bottom
							if(isAllVisited())
								continue;
							if(i + 1 < row)
								k += 1;
						} else if(temp == 3){   //* Move to left
							if(j - 1 > -1)
								l -= 1;
						} else if(temp == 4){   //* Move to top
							if(i - 1 > -1)
								k -= 1;
						}   //* Loop back is the k and l index are the same as the previous or the current one
					} while(isLastPos(pPos, k, l) || (i == k && j == l));
					
					//* Remove the word "Tile " in the tile's name, in order to know if the tile's position is already visited by the dark tile
					if(tiles[l, k].EditorDescription.Contains("Tile "))
						tiles[l, k].EditorDescription = tiles[l, k].EditorDescription.Replace("Tile ", "");
					
					//* Pressed twice
					onPressed(k, l);
					
					//* Assigned the current pos as the previous move
					pPos = new int[] { i, j };
					
					//* Break out the double loop
					done = true;
				}
			}
		}  if(!doRepeat() && !checkWin())   //* If done repeat thn the timer stops
			timer.Stop();
	}
	
	
	//* Returns a tile
	private TextureButton getTile(int i, int j){
		TextureButton tile = SM.getSceneInstance(GlobalData.iconButton) as TextureButton;
		tile.TextureNormal = UF.getTexture(ProjectSettings.GlobalizePath(path + $"{i}{j}.png"));
		
		tile.TexturePressed = tile.TextureHover = (i != 0 || j != 0) ? UF.getTexture(ProjectSettings.GlobalizePath(path + $"{i}{j} (2).png")) : null;
		tile.Expand = true;
		tile.RectSize = tile.RectMinSize = new Vector2(75, 75);
		tile.EditorDescription = (i != 0 || j !=0 ) ?  $"Tile [{j}, {i}]" : "Way Labot";
		tile.Connect("pressed", this, nameof(onPressed), new GA { i, j });
		
		return tile;
	}
	
	//* Returns true if all tiles' editorDescription container "Tile " or the dark tile is not yet at index 0, 0
	private bool doRepeat(){
		foreach(TextureButton tile in tiles){
			if(tile.EditorDescription.Contains("Tile "))
				return true;
		} 
		if(tiles[0, 0].EditorDescription.Equals("Way Labot"))
			return false;
		return true;
	}
	
	//* Checks the game if it is already solve or not
	private bool checkWin(){
		for(int i = 0; i < row; ++i){
			for(int j = 0; j < col; ++j){
				TextureButton tile = tiles[j, i];
				if(tile.EditorDescription.Equals("Way Labot"))
					continue;
				if(!tile.EditorDescription.Equals($"[{j}, {i}]"))
					return false;
			}
		} return true;
	}
	
	//* Move a tile, 2 tiles just exchanged some information
	private void moveATile(TextureButton tileA, TextureButton tileB){
		string name = tileB.Name, eD = tileB.EditorDescription;
		Texture img = tileB.TextureNormal;
		
		
		swapInfo(tileB, tileA.EditorDescription, tileA.TextureNormal, tileA.TexturePressed);
		swapInfo(tileA, eD, img, null);
		
		// swapTextureAfterPressed(tileB);
	}
	
	//* Swap some information between 2 tiles
	private void swapInfo(TextureButton tile, string eD, Texture normal, Texture pressed){
		tile.EditorDescription = eD;
		tile.TextureNormal = normal;
		tile.TexturePressed = tile.TextureHover = pressed;
	}
	
	//* Checks if the current indices is the same as the previous indices
	private bool isLastPos(int[] last, int curX, int curY) => (!isAllVisited() && last[0] == curX && last[1] == curY);
	
	private bool canMove(int x, int y){
		try{
			if(tiles[x, y].EditorDescription.Equals("Way Labot"))
				return true;
		}catch(IndexOutOfRangeException){} 
		return false;
	}
	
//***********************************************************************8
//* Fields
	private GridContainer container;
	private TextureButton[,] tiles;
	private Timer timer;
	private TextureRect whole;
	private int row, col;
	private int[] pPos;
	private Random rnd = new Random();
	private string path = "res://Assets/Puzzles/SlidingPuzzlePic/";
}
