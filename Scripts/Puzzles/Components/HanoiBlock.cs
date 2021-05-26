using Godot;
using System;

public class HanoiBlock : Control
{
		
	public void init()
	{
		bar = GetNode<TextureRect>("HBoxContainer/Bar");
		barCont = GetNode<HBoxContainer>("HBoxContainer");
	}

//	Sets the Value (Cannot be less than 1)
	public void initValue(int val)
	{
		init();
		
		if(val < 1) throw new Exception("Invalid Argument For HanoiBlock Value");

//		Rename based on val
		Name = val.ToString();
		
//		Generate bars
		for(int i = 1; i < val; i++)
		{
			barCont.AddChild(bar.Duplicate());
		}		
		
	}
	
	private TextureRect bar;
	private HBoxContainer barCont;
}
