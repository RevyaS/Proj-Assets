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
		prevMove = new int[] { 1, 1 };
		
		GD.Print(path += rnd.Next(1, 5).ToString() + "/");
		
		whole.Texture = UF.getTexture(ProjectSettings.GlobalizePath(path + "whole.jpg"));
		
		init("", 8);
	}
	
	
	//* Initialize difficulty
	public override void initDifficulty(int level){
		base.initDifficulty(level);
		
		row = 3;
		col = 3;
		tiles = new TextureButton[col, row];
		
		for(int i = 0; i < row; ++i){
			for(int j = 0; j < col; ++j)
				container.AddChild(tiles[j, i] = getTile(i, j));
		}
		
		timer.Start();
	}
	
//***********************************************************************
//* Signal Methods
	//* Select, Unselect or move a tile
	private void onPressed(int i, int j){
		TextureButton tile1 = tiles[j, i];
		TextureButton tile2 = (pPos != null) ? tiles[pPos[1], pPos[0]] : null;
		string str1 = tile1.EditorDescription;
		string str2 = (tile2 != null) ? tile2.EditorDescription : null;
		
		//* If the tile is being pressed once.
		// if(tile1 != null && tile2 == null || tile1 == tile2){
		if(tile2 == null || tile1 == tile2){
			//* The dark tile has only "Way Labot" in the EditorDescription
			if(!str1.Equals("Way Labot"))
				swapTextureAfterPressed(tile1);
				
		//* If the tile is being pressed twice
		// } else if(tile1 != null && tile2 != null){
		} else if(tile2 != null){
			if(!str1.Equals("Way Labot") && !str2.Equals("Way Labot")){
				swapTextureAfterPressed(tile1);
				swapTextureAfterPressed(tile2);
				pPos = null;
			} else if(str1.Equals("Way Labot")){
				swapTextureAfterPressed(tile2);
				
				//* If the dark tile is pressed in the second click, move the tile
				if(canMove(i, j)) moveATile(tile1, tile2);
			} 
		} pPos = (pPos == null && tile1 != tile2) ? new int[] { i, j } : null;
		
		GD.Print("Solved? ", checkWin());
		
		//* Solved if the puzzle is being solved
		if(checkWin()){
		//     solved();
			
		}
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
						if(temp == 1){  //* Move to left
							if(j + 1 < col)
								l += 1;
						} else if(temp == 2){   //* Move to top
							if(i + 1 < row)
								k += 1;
						} else if(temp == 3){   //* Move to right
							if(j - 1 > -1)
								l -= 1;
						} else if(temp == 4){   //* Move to bottom
							if(i - 1 > -1)
								k -= 1;
						}   //* Loop back is the k and l index are the same as the previous or the current one
					} while(isLastPos(prevMove, k, l) || (i == k && j == l));
					
					//* Remove the word "Tile " in the tile's name, in order to know if the tile's position is already visited by the dark tile
					if(tiles[l, k].EditorDescription.Contains("Tile "))
						tiles[l, k].EditorDescription = tiles[l, k].EditorDescription.Replace("Tile ", "");
					
					//* Pressed twice
					onPressed(k, l);
					onPressed(i, j);
					
					//* Assigned the current pos as the previous move
					prevMove = new int[] { i, j };
					
					//* Break out the double loop
					done = true;
				}
			}
		} if(!doRepeat())   //* If done repeat thn the timer stops
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
		string name = tileA.Name, eD = tileA.EditorDescription;
		Texture img = tileA.TextureNormal;
		
		swapInfo(tileA, tileB.EditorDescription, tileB.TextureNormal, tileB.TexturePressed);
		swapInfo(tileB, eD, img, null);
	}
	
	//* Swap some information between 2 tiles
	private void swapInfo(TextureButton tile, string eD, Texture normal, Texture pressed){
		tile.EditorDescription = eD;
		tile.TextureNormal = normal;
		tile.TexturePressed = tile.TextureHover = pressed;
	}
	
	//* Swap texture after being pressed in order to know if the tile is being selected
	private void swapTextureAfterPressed(TextureButton tileA){
		Texture img = tileA.TextureNormal;
		tileA.TextureNormal = tileA.TexturePressed;
		tileA.TexturePressed = img;
	}
	
	//* Checks if the current indices is the same as the previous indices
	private bool isLastPos(int[] last, int curX, int curY) => (last[0] == curX && last[1] == curY);
	
	//* Checks if the tile plan to move is valid or not
	private bool canMove(int i, int j) => canMoveLeft(i, j) || canMoveUp(i, j) || canMoveRight(i, j) || canMoveDown(i, j);
	
	//* Checks if it is valid to move the tile to left
	private bool canMoveLeft(int i, int j) => ((pPos[1] - 1 == j) && pPos[0] == i) ? true : false;
	
	//* Checks if it is valid to move the tile to up
	private bool canMoveUp(int i, int j) => (pPos[1] == j && (pPos[0] - 1 == i)) ? true : false;
	
	//* Checks if it is valid to move the tile to right
	private bool canMoveRight(int i, int j) => ((pPos[1] + 1 == j) && pPos[0] == i) ? true : false;
	
	//* Checks if it is valid to move the tile to down
	private bool canMoveDown(int i, int j) => (pPos[1] == j && (pPos[0] + 1 == i)) ? true : false;
	
//***********************************************************************8
//* Fields
	private GridContainer container;
	private TextureButton[,] tiles;
	private Timer timer;
	private TextureRect whole;
	private int row, col;
	private int[] pPos, prevMove;
	private Random rnd = new Random();
	private string path = "res://Assets/Puzzles/SlidingPuzzlePic/0";
}
