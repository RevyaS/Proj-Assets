using Godot;
using System;

public class PegSolitaire : Puzzle
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		grid = GetNode<GridContainer>("GridContainer");
		diffArray = new int[][,] { diff1, diff2, diff3, diff4, diff5};
	}


//	OVERRIDE FUNCTIONS
	public override void initDifficulty(int level)
	{
		//Limit 1 - 4
		initLevel(diffArray[level]);
	}

	
//	Generate Pegs based on the data input 
	public void initLevel(int[,] data)
	{
//		Get base size nxn
		int len = data.GetLength(0);
		
//		Set the Columns amount
		grid.Columns = len;
		
//		Calculate Peg size
		int baseSize = containerSize / len;
		GD.Print("Base: " + baseSize.ToString());
		Vector2 pegSize = new Vector2(baseSize, baseSize);
		
//		Generate Pegs
		for(int y = 0; y < len; y++)
		{
			for(int x = 0; x < len; x++)
			{
				Peg newPeg = SceneManager.getSceneInstance(PegPath) as Peg;
				newPeg.setSize(pegSize);
				newPeg.init(data[y,x]);
				grid.AddChild(newPeg);
				
//				Connect each peg's click to unhighlight everything
				newPeg.Connect("giveValue", this, nameof(selectPeg));
			}
		}
		
//		Connect Pegs
		for(int y = 0; y < len; y++)
		{
			for(int x = 0; x < len; x++)
			{
//				Get top
				Peg top = (y-1 < 0) ? null : (Peg) grid.GetChild((y-1) * len + x);
//				Get Bottom
				Peg bottom = (y+1 == len) ? null : (Peg) grid.GetChild((y+1) * len + x);
//				Get right
				Peg right = (x+1 == len) ? null : (Peg) grid.GetChild((y * len) + x+1);
//				Get left
				Peg left = (x-1 < 0) ? null : (Peg) grid.GetChild((y * len) + x-1);

//				Get current iterated peg
				int index = (y*len) + x;
				Peg currPeg = grid.GetChild(index) as Peg;

//				Set curr's neighbors
				currPeg.initNeighbors(top, bottom, right, left);
			}
		}
	}


	private void selectPeg(Peg peg)
	{		
//		set the peg as Selected
		if(peg.Val == 1) prevPeg = peg;
		
//		Unhighlight if the peg selected wasn't highlighted and set this as prev
		if(!peg.highlight) unhighlightAll();
		
//		But if the peg is highlighted resolve movement (Only 0 will have highlights)
		else 
		{
//			Get the direction
			int dir = peg.highlightValue;
//			Get the center peg
			Peg center = prevPeg.neighbors[dir];
			
//			Resolve
			prevPeg.Val = 0;
			peg.Val = 1;
			center.Val = 0;
			prevPeg = peg;
			
//			Unhighlight
			unhighlightAll();
		
//			Check if solved
			check();
		}
	}
	

//	Iterates and sets highlight to off
	private void unhighlightAll()
	{

		foreach(Node n in grid.GetChildren())
		{
			(n as Peg).toggleHighlight(false);
		}
	}
	
	
//	Check if solved
	private bool check()
	{
		int gemCount = 0;
		for(int i = 0; i < grid.Columns * grid.Columns; i++)
		{
			if( (grid.GetChild(i) as Peg).Val == 1 )
				gemCount++;
			
			if(gemCount == 2)
				return false;
		}
//		Call checked
		solved();
		return true;
	}
	
	
//	Data list

	int[,] diff1 = new int[5,5]{
			{-1, -1,  1, -1, -1},
			{-1, -1,  1, -1, -1},
			{ 0,  1,  0, -1, -1},
			{ 1, -1,  1, -1, -1},
			{ 0,  1,  0, -1, -1},
		};
		
	int[,] diff2 = new int[5,5]{
			{ 0,  0,  0,  0,  0},
			{ 0,  0,  1,  0,  0},
			{ 0,  0,  1,  0,  0},
			{ 0,  1,  1,  1,  0},
			{ 0,  0,  0,  0,  0},
	};
	
	int[,] diff3 = new int[5,5]{
			{-1,  0,  1,  0, -1},
			{ 0,  1,  1,  1,  0},
			{ 1,  1,  1,  1,  1},
			{ 0,  1,  0,  1,  0},
			{-1,  0,  0,  0, -1},
	};
	
	int[,] diff4 = new int[7,7]{
			{-1, -1,  1,  1,  1, -1, -1},
			{-1,  1,  1,  1,  1, -1, -1},
			{ 1,  1,  1,  1,  1,  1,  1},
			{ 1,  1,  1,  0,  1,  1,  1},
			{ 1,  1,  1,  1,  1,  1,  1},
			{-1, -1,  1,  1,  1, -1, -1},
			{-1, -1,  1,  1,  1, -1, -1},
	};
	
	int[,] diff5 = new int[7,7]{
			{-1, -1,  0,  0,  0, -1, -1},
			{-1,  1,  0,  0,  1,  0, -1},
			{ 0,  0,  0,  1,  1,  1,  0},
			{ 0,  0,  1,  0,  1,  0,  0},
			{ 0,  1,  1,  1,  0,  0,  0},
			{-1,  0,  1,  0,  0,  1, -1},
			{-1, -1,  0,  0,  0, -1, -1},
	};
		
//	Previously Selected peg
	Peg prevPeg;

//	Components
	GridContainer grid;
	
//	Container will always be a square
	const int containerSize = 300;
	int[][,] diffArray;
	
	const string PegPath =  "res://Pages/Puzzles/Components/Peg.tscn";
}
