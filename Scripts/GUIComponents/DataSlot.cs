using Godot;
using Godot.Collections;
using System;
using UF= UtilityFunctions;

public class DataSlot : TextureButton
{
	public override void _Ready()
	{
//		Preload Normal Textures
		normal = UF.getTexture("res://Assets/BG/paper background.png");
		selected = UF.getTexture("res://Assets/BG/paper background (3) (Darker).png");
		
		title = GetNode<Label>("HC/DataDescription/SaveNumber");
		day = GetNode<Label>("HC/DataDescription/DataContains/Day");
		text = GetNode<Label>("HC/DataDescription/DataContains/Text");
		money = GetNode<Label>("HC/DataDescription/DataContains/Money");
		loc = GetNode<TextureRect>("HC/Location");
	
		title.Text = "Empty Slot";
		day.Text = "";
		text.Text = "";
		money.Text = "";
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
		
		title.Text = "Save Slot " + slot.ToString();
		day.Text = "Day : " + (saveData["Stats"] as Dictionary)["Day"];
		text.Text = saveData["CurrText"] as String;
		money.Text = "Money : " + (saveData["Stats"] as Dictionary)["Money"];
		loc.Texture = (MapData.gameMap.getNode(saveData["CurrNode"].ToString()) as Location).image;
	
	}
	
	
//	Change the texture to selected
	public void isSelected(bool isSelected)
	{
		TextureNormal = (isSelected) ? selected : normal;
	}
	
//	Components
	Label title, day, text, money;
	TextureRect loc;
	ImageTexture normal, selected;
	
	public String saveFileLocation;
	public int slot;
	public Dictionary saveData;
}
