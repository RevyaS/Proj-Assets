using Godot;
using System;

public class HopOver : Puzzle
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cont = GetNode<HBoxContainer>("HBoxContainer");
//		initDifficulty(4);
//		switchPos(4, 6);
	}

	
//	Only Accepts Levels 1-8
	public override void initDifficulty(int level) {
//		Compute the width of each
//		Level defines the amount of hoppers each side sp total hoppers = level*2 + 1 (empty hopper)
//		Max Size = 550 
		hoppers = (level*2 + 1);
		int width = 550 / hoppers;
		
		solution = new int[hoppers];
		empty = level; //Center Position
		
//		Generate Red Hoppers
		for(int i = 0; i < hoppers; i++)
		{
			Hopper hop = SceneManager.getSceneInstance(HopperPath) as Hopper;
			int val = (i < level) ? 1 : (i == level) ? 0 : 2;
			
			solution[i] = (val == 1) ? 2 : (val ==2) ? 1: 0;
			hop.init(width, val, i);
			cont.AddChild(hop);
			
			hop.Connect("clicked", this, nameof(resolveAction));
		}
	}

	
//	Resolves Action once a hopper is pressed
	private void resolveAction(Hopper hopper) {
//		Red only moves to the right
		if(hopper.value == 1)
		{
			if(hopper.position + 1 == empty)
				switchPos(hopper.position, empty);
				
			if(hopper.position + 2 == empty)
				switchPos(hopper.position, empty);
		}
		
		if(hopper.value == 2)
		{
			if(hopper.position - 1 == empty)
				switchPos(hopper.position, empty);
				
			if(hopper.position - 2 == empty)
				switchPos(hopper.position, empty);
		}
	}


//	Switches position of 2 hoppers in cont
	private void switchPos(int pos1, int pos2)
	{
//		Get hoppers
		Hopper hop1 = cont.GetChild(pos1) as Hopper;
		Hopper hop2 = cont.GetChild(pos2) as Hopper;
		
		cont.MoveChild(hop1, pos2);
		cont.MoveChild(hop2, pos1);
		
		int newPos = hop1.position;
		
		hop1.position = hop2.position;
		hop2.position = newPos;
		
		empty = (hop1.position == empty) ? newPos : (hop2.position == empty) ? hop1.position : empty;
		checkSolution();
	}


//	Checks if puzzle is already solved
	private void checkSolution()
	{
//		GD.Print("----Checking-----");
		for(int i = 0; i < hoppers; i++)
		{
			int val = ( cont.GetChild(i) as Hopper).value;
//			GD.Print(solution[i] + ":" + val);
			if(solution[i] != val)
				return;
		}
		solved();
	}


//	Components
	HBoxContainer cont;

//	Solution Array to check if puzzle is solved
	int[] solution;
	
//	Position to keep track
	int empty;
//	Amount of hoppers available to avoid index out of bounds exception
	int hoppers;

//	ComponentPaths
	private string HopperPath = "res://Pages/Puzzles/Components/Hopper.tscn";
}
