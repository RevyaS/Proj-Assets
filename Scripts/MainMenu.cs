using Godot;
using System;

public class MainMenu : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	
//	Create new game
	private void newGame()
	{
//		Switch to game
		GD.Print("New Game");
//		Reset data
		DataManager dManager = GetNode<DataManager>("/root/DataManager") as DataManager;
		dManager.initFlags();
		SceneManager.Singleton.switchToGame(MapData.startNode);
//		QueueFree();
	}


//	Load game via Data Select
	private void loadGame()
	{
		SceneManager.Singleton.showDataSelect();
	}


//	Open options
	private void openOptions()
	{
//		Generate options page
		Options opt = SceneManager.getSceneInstance(GlobalData.optionPath) as Options;
		SceneManager.Singleton.stackPage(opt);
	}
}
