using Godot;
using System;

public enum StoryLine {AuthorsLabrat, AchingDreams};

public class MainMenu : TextureRect
{
	DataManager dManager; 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dManager = GetNode<DataManager>("/root/DataManager") as DataManager;
	}
	
	
//	Create new game
	private void newGame(StoryLine story)
	{
//		Switch to game
		GD.Print("New Game");
//		Reset data
		dManager.initFlags((int)story);
		MapData.initData((int)story);
		MapData.loadMap((int)story);
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
