//SINGLETON 
//Handles everything that is related to scenes

using Godot;
using GC = Godot.Collections;
using System;

public class SceneManager : Node
{

//	ChangeScene is non static so a non static referene would be useful
	public static SceneManager Singleton;
	
//	Preloaded Scenes
	private DataSelection dataSelect;
	private static Game game; //Game scene
	
//	Scene properties
	public static Game Game{
		get{return game;}
		set{game = value;}
	}
	
//	Scene properties
	public DataSelection DataSelect{
		get{return dataSelect;}
	}
	
//	Preloading references
	public bool dataSelectInitDone = false;
	
	public override void _Ready()
	{
		Singleton= this;
		
//		Preload DataSelection
		dataSelect = getSceneInstance(GlobalData.dataSelection) as DataSelection;
		dataSelect.Hide();
		dataSelect.Connect("InitFinished", this, nameof(dataSelectDone));
//		Temporarily add this as child
		AddChild(dataSelect);
	}
	
//*********************************************************************************************
//	For preloaded Scenes that needs reference to a Node that will be Freed once the load button is pressed
	public async void showDataSelect()
	{		
//		if data init is not yet done, then wait
		if(!dataSelectInitDone)
		{
			await ToSignal(dataSelect, "InitFinished");
		}
		dataSelect.Show();
		dataSelect.Raise();
	}
	
	
//	Generates a new game and sets location
	public async void switchToGame(Location newLocation)
	{		
		GD.Print("Start");
//		Create new game instance
		Game newGame = SceneManager.getSceneInstance(GlobalData.gamePath) as Game;
//		Only perform actions if dataSelect is loaded
		setScene(newGame);
//		Update Game object in SceneManager
		game = newGame;
//		Init game
		newGame.setLocation(newLocation);
	}
	
	
//*********************************************************************************************	
//	For preloading
	private void dataSelectDone()
	{
		dataSelectInitDone = true;
//		Remove child from tree
		RemoveChild(dataSelect);
//		Add data Select page to main tree
		GetTree().Root.AddChild(dataSelect);
	}
	

//*********************************************************************************************
	
	//*	Returns an instance of a scene (must be converted before use)	
	public static object getSceneInstance(String ScenePath)
		=> getPackedScene(ScenePath).Instance();
	
	public static PackedScene getPackedScene(String ScenePath) => (PackedScene)ResourceLoader.Load(ScenePath);
	
	
//	Stacks a page scene above view
	public void stackPage(Node page)
	{
		GetTree().Root.AddChild(page);
	}
	
	
//	Changes main scene
	public void setScene(String newScene)
	{
		GetTree().ChangeScene(newScene);
	}
	
	
	public void setScene(Node newScene)
	{
//		Sets the scene
		stackPage(newScene);
//		Delete current scene
		Node prevCurr = GetTree().CurrentScene;
		GetTree().CurrentScene = newScene;
		prevCurr.QueueFree();
		dataSelect.Raise();
	}
	
	//*	Removes all children and subchildren of a node or inheritors of node
	public static void clearChildren(Node node)
	{
		int childCount = node.GetChildCount();
		for(int i = 0; i < childCount; i++)
		{
			Node child = (Node)node.GetChild(0);
			node.RemoveChild(child);
			child.QueueFree();
		}
	}
	
	
	public static Node gotoScene(Node parentNode, Node currentNode, Node newScene){
		parentNode.RemoveChild(currentNode);
		currentNode.CallDeferred("free");
		currentNode = duplicateScene(newScene);
		parentNode.AddChild(currentNode);
		return currentNode;
	}
	
	public static Node duplicateScene(Node certainNode) => certainNode.Duplicate();
}
