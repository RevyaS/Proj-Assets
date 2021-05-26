//Description: Inherits the gNode class from DsGraph Data Struct, for the 
//	main purpose of embodying the Locations in the game

using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Godot.Collections;

public class Location : gNode
{
//	Base Constructor
	public Location() : base()
	{
		
	}
	
	public Location(String name, ImageTexture locationImage) : base(name)
	{
		image = locationImage;
	}

//	Additional Variables
//	List of Neighbors in String
	public ImageTexture image {get;}
}
