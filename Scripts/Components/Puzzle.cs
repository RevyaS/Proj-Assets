using Godot;
using System;

public class Puzzle : TextureRect
{
	[Signal]
	public delegate void Solved(String val);

//	OVERRIDE THIS IF POSSIBLE Just to initialize puzzle
	public virtual void init(String val, int level)
	{
		this.val = val;
		initDifficulty(level);
	}
	
	
//	OVERRIDE THIS FOR Added difficulty
	public virtual void initDifficulty(int level)
	{
		GD.Print("You used the BASE method");
	}
	
	
//	Call this method if puzzle is solved
	protected void solved()
	{
//		Make this not clickable
		MouseFilter = MouseFilterEnum.Ignore;
//		Send signal
		EmitSignal(nameof(Solved), val);
	}
	

//	The value to send when puzzle is solved
	private String val;
}
