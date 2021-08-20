//SINGLETON 
//Description: Contains the data every class must have access to

using Godot;
using System;
using UF = UtilityFunctions;
using Godot.Collections;
using YamlDotNet.RepresentationModel;
public class GlobalData : Node{
	
	//*	returns data from globalData.id in Dictionary type via key
	//*	Keylist can be found on KEYLIST.txt file
	public static Dictionary getData(String key, string dataPath) 
		=> UF.readFile(dataPath)[key] as Dictionary;

	public static YamlMappingNode getYamlData(String key, string dataPath)
		=> UF.readYAML(dataPath).Children[key] as YamlMappingNode;

	public static Godot.Collections.Array getList(String key, string dataPath) 
		=> UF.readFile(dataPath)[key] as Godot.Collections.Array;
	
	//*	Gets the Dictionary data from route0
	public static YamlMappingNode getRouteData(int routeNumber, string key)
	{
		String routeLocation = "res://GameData/" + dManager.StoryFlag + "/Route" +  routeNumber + ".id";
		return getYamlData(key, routeLocation);
	}
	
	//*	Gets the global data
	public static YamlMappingNode getGlobalData(string key)
	{
		String globalPath =  "res://GameData/GlobalData.id";
        return getYamlData(key, globalPath);
	}


	private static void getStories()
	{
		if(storyData != null) return;
        YamlMappingNode storyList = getGlobalData("Stories");
        storyData = dManager.YamlToDict(storyList);
    }


	//*	Gets the story data based from story flag
	public static YamlMappingNode getStoryData(string key)
	{
		String globalPath =  "res://GameData/" + dManager.StoryFlag + "/StoryData.id";
		return getYamlData(key, globalPath);
	}

	public static YamlMappingNode getStoryData(int storyFlag, string key)
	{
		String globalPath =  "res://GameData/" + storyFlag + "/StoryData.id";
		return getYamlData(key, globalPath);
	}

	public static YamlMappingNode getStoryLocations() 
		=> getStoryData("Locations");
	
	public static YamlMappingNode getStoryCharacters(int flag)
		=> getStoryData("Characters");
	
	
	//*	Gets the Dictionary data from Event
	//*	The key is based on the Event's name
	public static Dictionary getEventData(String key, int route) => dManager.YamlToDict(getYamlData(key, "res://GameData/" + dManager.StoryFlag + "/EventData" + route + ".id"));
	
	//*	Gets the char's image from dictionary
	public static String getCharImage(String key, int flag) 
		=> getStoryCharacters(flag)[key].ToString();
	
	//*	Gets the puzzle's path
	public static String getPuzzlePath(String key) 
		=> getGlobalData("Puzzles")[key].ToString();
	
	//*	Gets the bg image path
	public static String getBGPath(String key) 
		=> getStoryLocations()[key].ToString();

	public static String getSFXPath(String key) 
		=> getGlobalData("SFX")[key].ToString();

	
	public static String getBGMPath(String key) 
		=> getGlobalData("BGM")[key].ToString();
	
	
	//* Called when the node enters the scene tree for the first time.
	public override void _Ready() 
	{
		timer = new Timer();
		AddChild(timer);
		generateDirs();
		getStories();
	}
	
	
	private void generateDirs()
	{
		Directory dir = new Directory();
		
//		Check if Borrowers folder exists
		if(!dir.DirExists(dataPath))
		{
//			Generate the Borrowers folder
			GD.Print("Generating Necessary folders");
			dir.MakeDir(dataPath);
		}
	}
	
	
//	Shareable components
	public static Timer timer;

	// References
	public static DataManager dManager;

    // StoryList
    public static Dictionary storyData;

    //	Pages
    public static string optionPath = "res://Pages/GUI Components/Option.tscn";
	public static string gamePath = "res://Pages/Game.tscn";
	
//	Scenes
	public static string mainMenuPath = "res://Pages/MainMenu.tscn";
	
//	Data locations
	public static string dataPath = "user://Saves";
	
//	Components
	public static string dataSelection = "res://Pages/Components/DataSelection.tscn";
	
	//*	Buttons
	public static string actionButton = "res://Pages/GUI Components/ActionButton.tscn";
	
	public static string iconButton = "res://Pages/GUI Components/IconButton.tscn";
	
	//*	SFX
	public static string sfxButton = "res://Assets/SFX/click.wav";

    //Themes
    public static string themeActionButton = "res://Resources/ActionButtonTheme.tres";
    //Fonts
    public static string fontAcme20 = "res://Resources/Acme20.tres";
}
