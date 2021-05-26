using Godot;
using System;
using UF = UtilityFunctions;

public class WaterPuzzle : Puzzle
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		highlight = UF.getTexture(ProjectSettings.GlobalizePath("res://Assets/Puzzles/WhiteHighlight.png"));
		
		maxLabels = new Label[]{
			GetNode<Label>("Max/Left"),
			GetNode<Label>("Max/Center"),
			GetNode<Label>("Max/Right"),
		};
		
		btns = new TextureButton[] {
			GetNode<TextureButton>("HBoxContainer/Left"),
			GetNode<TextureButton>("HBoxContainer/Center"),
			GetNode<TextureButton>("HBoxContainer/Right"),
		};
		
		conts = new VBoxContainer[] {
			btns[0].GetNode<VBoxContainer>("Container"),
			btns[1].GetNode<VBoxContainer>("Container"),
			btns[2].GetNode<VBoxContainer>("Container"),
		};	
		
		diffLevels = new int[][]{diff1, diff2, diff3, diff4, diff5, diff6, diff7};
	}
	
	
//	Required override
	public override void initDifficulty(int level)
	{
		init(diffLevels[level -1]);
	}
	
	
	private void init(int[] levelData)
	{
//		Compute height for each bar
//		Height = (maxHeight - totalPadding)/left bar max bars;
		int height = (240 - (4 * levelData[0]))/(levelData[0]);
		
		values[0] = levelData[0];
		winCon = levelData[3];
		
//		Generate left side with -1 because each column already have 1 bar
//		Loop through each containers
		for(int i = 0; i < 3; i++)
		{
//			Resize existing bars
			(conts[i].GetChild(0) as TextureRect).RectMinSize = new Vector2(10, height);
//			Set label
			maxLabels[i].Text = levelData[i].ToString();

//			Fill in the rest
			for(int j = 0; j < levelData[i] -1; j++)
			{
				TextureRect newTexture = generateBar(height);
				conts[i].AddChild(newTexture);
			}			
		}
		setValues();
	}
	
	
//	Dealing with inputs
//	If left bar clicked
	private void leftClicked()
	{
		resolveAction(0);
	}


	private void centerClicked()
	{
		resolveAction(1);
	}
	
	
	private void rightClicked()
	{
		resolveAction(2);
	}
	

//	Configure clicks
	private void resolveAction(int index)
	{
		if(selected != -1) //selection
		{
			if(selected != index)
			{
//				Get amount of free space in the new container
				int free = (conts[index].GetChildCount()) - values[index];
//				exit if there is no free values
				if(free == 0) return;
				
//				Get remaining water from prev selected container
				int remain = values[selected];
	
//				Get amount to be transferred and must not be greater than remain
				int transfer = (remain < free) ? remain : free;
				
//				Perform transfer
				values[selected] -= transfer;
				values[index] += transfer;
				
//				Update columns
				setValues();
//				Check if solved (Left and Center containers will never be 0 so...)
				if(values[0] - values[1] == 0) {
					disableInputs();
					solved();
				}
				
			}
//			Set selected to no selection
			selected = -1;
			unhighlightAll();
		} else if(values[index] != 0) // no Selection but will only select if there is a potential transfer
		{
//			highlight selected
			btns[index].TextureNormal = highlight;
			selected = index;
		}
	}
	
	
//	Remove highlights
	private void unhighlightAll()
	{
		foreach(TextureButton btn in btns)
			btn.TextureNormal = null;
	}
	
	
//	Disable inputs
	private void disableInputs()
	{
		foreach(TextureButton btn in btns)
			btn.MouseFilter = MouseFilterEnum.Ignore;
	}
	
	
//	Creates a bar for containers
	TextureRect generateBar(int height)
	{
		TextureRect nTR = new TextureRect();
		nTR.Texture = UF.getTexture(ProjectSettings.GlobalizePath("res://Assets/Puzzles/DarkYellow.png"));
		nTR.RectMinSize = new Vector2(50, height);
		nTR.Expand = true;
//		Make sure that the RectSize will stretch out horizontally
		nTR.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
		return nTR;
	}
	
	
//	Sets the bar value for each container
	void setValues()
	{
		for(int i = 0; i < 3; i++)
		{
			for(int j = 0; j < conts[i].GetChildCount(); j++)
			{
				if(j < values[i])
					(conts[i].GetChild(j) as TextureRect).Show();
				else
					(conts[i].GetChild(j) as TextureRect).Hide();
			}			
		}
	}
	
	
//	Format [left max, center max, right max, win condition]
	int[] diff1 = {4,3,1, 2};
	int[] diff2 = {8,5,3, 4};
	int[] diff3 = {12,7,5, 6};
	int[] diff4 = {12,7,3, 6};
	int[] diff5 = {16,9,5, 8};
	int[] diff6 = {16,9,7, 8};
	int[] diff7 = {20,13,7, 10};
	
//	Contains the values of all containers [Left, Center, Right]
	int[] values = {0, 0, 0};
	
	int[][] diffLevels;
	
//	Components
//	Left Container, Center Container, Right Container
	VBoxContainer[] conts;
	
//	The previously selected container
	int selected = -1;
	int winCon;
	ImageTexture highlight;
	
	Label[] maxLabels;
	
	TextureButton[] btns;
}
