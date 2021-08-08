using Godot;
using Godot.Collections;
using System;
using UF= UtilityFunctions;
using YamlDotNet.RepresentationModel;

public class DataSlot : TextureButton
{
	public override void _Ready()
	{
//		Preload Normal Textures
		normal = UF.getTexture("res://Assets/BG/paper background.png");
		selected = UF.getTexture("res://Assets/BG/paper background (3) (Darker).png");
		
		title = GetNode<Label>("HC/DataDescription/SaveNumber");
		text = GetNode<Label>("HC/DataDescription/DataContains/Text");
		loc = GetNode<TextureRect>("HC/Location");
	
		title.Text = "Empty Slot";
		text.Text = "";
		loc.Texture = null;
		
		init();
	}
	
	
//	Initialize data based on Dictionary read
	public void init()
	{
//		Slot number is based on node name
		slot = Convert.ToInt32(Name);
			
//		Generate save file path
		saveFileLocation = GlobalData.dataPath.PlusFile("Save" + slot.ToString() + ".sav");
	
//		check if save file exists
		if(!UF.fileExists(saveFileLocation))
		{
			GD.Print(saveFileLocation + " null");
			return;
		}
		
//		Get data from slotNumber
		saveData = UF.readFile(saveFileLocation);
		text.Text = saveData["CurrText"] as String;
		GD.Print(saveData["CurrNode"].ToString());
		//Get data from story file
		int storyVal = Convert.ToInt32(saveData["Story"]);
		//Show story
		title.Text = slot.ToString() + ". " + GlobalData.stories[storyVal];
		//Show Image
		YamlMappingNode storyData = GlobalData.getStoryData(storyVal, "Locations");
		String nodeTexLoc = storyData[ saveData["CurrNode"].ToString() ].ToString();
		loc.Texture = UF.getTexture(nodeTexLoc);
	}
	
	
//	Change the texture to selected
	public void isSelected(bool isSelected)
	{
		TextureNormal = (isSelected) ? selected : normal;
	}
	
//	Components
	Label title, text;
	TextureRect loc;
	ImageTexture normal, selected;
	
	public String saveFileLocation;
	public int slot;
	public Dictionary saveData;
}
