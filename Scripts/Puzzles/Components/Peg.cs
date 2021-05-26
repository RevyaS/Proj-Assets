using Godot;
using System;
using UF = UtilityFunctions;

public class Peg : TextureButton
{
	[Signal]
	public delegate void giveValue(Peg p);
	
//	[Top, Right, Bottom, Left]
	public Peg[] neighbors;
	public bool highlight;
	public int highlightValue;
	int val;
//	The neighbor's value that allow us to determine 
//	which neighbor it is in respect to the selected peg
//	The state will contain a disabled and non disabled version
//	[Normal, Disabled]
	public ImageTexture[] state;
//	The 2 states, [NoHighlight, Highlighted]
	public ImageTexture[] empty;
	public ImageTexture[] gem;
		
//	The peg's value
	public int Val {
		get {
			return val;
		}
		set {
			this.val = value;
			switch(this.val)
			{
				case 0:
					state = empty;
					break;
				case 1:
					state = gem;
					break;
				default:
					state = null;
					break;
			}
			loadState(false);
		}
	}
	
	
	// Initializes the value itself from (-1 -> 1)
	public void init(int val)
	{
//		Load states
		empty = new ImageTexture[2]{
			UF.getTexture("res://Assets/Puzzles/Peg Solitaire/GemNone.png"),
			UF.getTexture("res://Assets/Puzzles/Peg Solitaire/GemNoneOutline.png")
		};
		gem = new ImageTexture[2]{
			UF.getTexture("res://Assets/Puzzles/Peg Solitaire/Gem.png"),
			UF.getTexture("res://Assets/Puzzles/Peg Solitaire/GemOutline.png")
		};
		
		highlight = false;
		Val = val;
	}


 	// Initializes neighbors and the value itself from (-1 -> 1)
	public void initNeighbors(Peg top, Peg bottom, Peg right, Peg left)
	{
		neighbors = new Peg[4];
		neighbors[0] = top;
		neighbors[1] = right;
		neighbors[2] = bottom;
		neighbors[3] = left;
	}

	
//	Sets the size of the Peg
	public void setSize(Vector2 size)
	{
		RectMinSize = size;
	}
	
	
//	Highlight neighbors's jump
	public void highlightNeighbors()
	{
//		Emit Signal to clear highlights
		EmitSignal("giveValue", this);
		
//		Highlight neighbors if the value is 1
		if(val != 1) return;		

//		Check Neighbors
		for(int i = 0; i < neighbors.Length; i++)
		{
			if(neighbors[i] != null)
				highlightJump(neighbors[i], i);
		}
	}
	
	
//	Highlights the jump (Assumes the peg to be jumped on is not null)
	public void highlightJump(Peg peg, int direction)
	{
//		The neighbor's value must be 1
		if(peg.Val != 1) return;

//		The neighbor at direction musn't be null
		if(peg.neighbors[direction] == null) return;

//		The neighbor's val must be zero
		if(peg.neighbors[direction].Val != 0) return;
	
//		Highlight the jump
		peg.neighbors[direction].toggleHighlight(true);
		
//		Give the highlight value as direction to determine where it's located
		peg.neighbors[direction].highlightValue = direction;
	}
	
	
	//Loads the image based on the state
	public void loadState(bool highlight)
	{
		if(state == null)
		{
			TextureNormal = null;
			return;
		}
		
//		Get outline size
		TextureNormal = state[Convert.ToInt32(highlight)];
	}
	
	
//	Toggle Highlight
	public void toggleHighlight(bool on)
	{
//		Do not toggle if disabled (-1)
		if(val == -1) return;
		
//		Set highlight
		highlight = on;
		
		loadState(on);
	}
	
}
