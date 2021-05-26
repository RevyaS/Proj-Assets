using Godot;
using System;

public class HanoiPuzzle : Puzzle
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
//		Initialize buttons
		TextureButton btnLeft = GetNode<TextureButton>("VBoxContainer/Buttons/Left"),
					  btnCenter = GetNode<TextureButton>("VBoxContainer/Buttons/Center"),
					  btnRight = GetNode<TextureButton>("VBoxContainer/Buttons/Right");
	
//		Initilize Containers
		VBoxContainer left = GetNode<VBoxContainer>("VBoxContainer/Puzzle/Left/VBoxContainer"),
					  center = GetNode<VBoxContainer>("VBoxContainer/Puzzle/Center/VBoxContainer"),
					  right = GetNode<VBoxContainer>("VBoxContainer/Puzzle/Right/VBoxContainer");
	
		selectShow = GetNode<TextureRect>("VBoxContainer/Selected");
	
		containers = new VBoxContainer[3] { left, center, right };
		buttons = new TextureButton[3] { btnLeft, btnCenter, btnRight };
		
	}
	
//	OVERRIDE METHODS
	
	public override void initDifficulty(int level)
	{
//		Clear the Container's Amount
		Node container = GetNode("VBoxContainer/Puzzle/Left/VBoxContainer");
		SceneManager.clearChildren(container);
		
//		Generate blocks
		for(int i = 1; i <= level; i++)
		{
//			Get block's instance
			HanoiBlock block = SceneManager.getSceneInstance(hanoiBlock) as HanoiBlock;
//			Set value
			block.initValue(i);
//			Add the block to container
			container.AddChild(block);			
		}
	}

//*********************************************************************************************

//	Button Signal Methods
	private void leftPressed()
	{
		selectContainer(containers[0]);		
	}
	
	
	private void centerPressed()
	{
		selectContainer(containers[1]);
	}	
	
	
	private void rightPressed()
	{
		selectContainer(containers[2]);		
	}
	
	
	private void selectContainer(VBoxContainer container)
	{
//		Check if we selected an item to transfer
		if(!selecting)
		{
//			Selected item to transfer
			selecting = true;
//			Get the top of the container
			selected = container.GetChild(0);
//			Remove the top of the container
			container.RemoveChild(selected);
//			Add the top to the selected
			selectShow.AddChild(selected);
			(selected as Control).SetAnchorsAndMarginsPreset(LayoutPreset.VcenterWide, LayoutPresetMode.KeepSize);
		} else
		{
//			Selected where to transfer
			selecting = false;
//			Remove selected item from the selected
			selectShow.RemoveChild(selected);
//			Move the item to the Container
			container.AddChild(selected);
			(selected as Control).SetAnchorsAndMarginsPreset(LayoutPreset.VcenterWide, LayoutPresetMode.KeepSize);
			container.MoveChild(selected, 0);
//			Clear the selected
			selected = null;

		}
//		Disable some buttons based on the selected
		toggleButtons();
	
//		Check for victory
		int victory = 3;
		if(containers[0].GetChildCount() == 0) victory--;
		if(containers[1].GetChildCount() == 0) victory--;
		if(selectShow.GetChildCount() == 0) victory--;
		
		if(victory == 0)
			solved();
//			GD.Print("Solved, Send Signal");
	}


//	Disables button based on seleted container to avoid
//	invalid input
	private void toggleButtons()
	{
		for(int i = 0; i < 3; i++)
		{
			buttons[i].Disabled = false;
//			If we are selecting where to move
			if(selecting)
			{
//				Checks if it contains an item	
				if(containers[i].GetChildCount() != 0)
				{
					Node top = containers[i].GetChild(0);
//					Check if the top has larger value than the selected					
					if(Convert.ToInt32(top.Name) < Convert.ToInt32(selected.Name))
						buttons[i].Disabled = true;
				}
				
			} else
//				Disable input to empty container
				if(containers[i].GetChildCount() == 0)
					buttons[i].Disabled = true;
		
		}
	}

//	Component Variables
//	Item containers [left, center, right]
	VBoxContainer[] containers;

//	Buttons [left, center, right]
	TextureButton[] buttons;
	
//	Selected Container
	TextureRect selectShow;
//	Components
	private static string hanoiBlock = "res://Pages/Puzzles/Components/HanoiBlock.tscn";
	
//	Flags
//	True if we are selecting where to move
	bool selecting = false;
//	The selected item
	Node selected = null;
}
