using Godot;
using System;
using UF = UtilityFunctions;

public class Hopper : TextureButton
{
	
//	size (N) of the hopper NxN 
//	val - value of the hopper (0 - Empty, 1 - RED, 2 - GREEN) 
	public void init(int size, int val, int pos)
	{
//		Set size
		RectMinSize = new Vector2(size, size);
//		Set position and value
		position = pos;
		value = val;
//		Initialize appearance
		switch(val)
		{
			case 1:
				TextureNormal = UF.getTexture(ProjectSettings.GlobalizePath("res://Assets/Puzzles/HopOver/RedGem.png"));
				break;
			case 2:
				TextureNormal = UF.getTexture(ProjectSettings.GlobalizePath("res://Assets/Puzzles/Peg Solitaire/Gem.png"));
				break;
			default:
				TextureNormal = UF.getTexture(ProjectSettings.GlobalizePath("res://Assets/Puzzles/Peg Solitaire/GemNone.png"));
				break;			
		}
	}
	
	
//	Emits clicked signal when pressed
	private void emit()
	{
		EmitSignal("clicked", this);
	}

	[Signal]
	public delegate void clicked(Hopper hopper);
	
//	Hopper's position
	public int position {get; set;}
	public int value {get; set;}
}
