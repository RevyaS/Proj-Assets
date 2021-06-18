//SINGLETON 
//Description: Contains the data every class must have access to

using Godot;
using System;
using UF = UtilityFunctions;
using Godot.Collections;
public class GlobalData : Node{
	
	//*	returns data from globalData.id in Dictionary type via key
	//*	Keylist can be found on KEYLIST.txt file
	public static Dictionary getData(String key, string dataPath) 
		=> UF.readFile(dataPath)[key] as Dictionary;

	public static Godot.Collections.Array getList(String key, string dataPath) 
		=> UF.readFile(dataPath)[key] as Godot.Collections.Array;
	
	//*	Gets the Dictionary data from route0
	public static Dictionary getRouteData(int routeNumber, string key)
	{
		String routeLocation = "res://GameData/" + dManager.StoryFlag + "/Route" +  routeNumber + ".id";
		return getData(key, routeLocation);
	}
	
	//*	Gets the global data
	public static Dictionary getGlobalData(string key)
	{
		String globalPath =  "res://GameData/GlobalData.id";
		return getData(key, globalPath);
	}

	//*	Gets the story data based from story flag
	public static Dictionary getStoryData(string key)
	{
		String globalPath =  "res://GameData/" + dManager.StoryFlag + "/StoryData.id";
		return getData(key, globalPath);
	}

	public static Godot.Collections.Array getStoryLocations()
	{
		String globalPath =  "res://GameData/" + dManager.StoryFlag + "/StoryData.id";
		return getList("Locations", globalPath);
	}
	
	public static Dictionary getStoryCharacters(int flag)
	{
		return getStoryData("Characters");
	}
	
	
	//*	Gets the Dictionary data from Event
	//*	The key is based on the Event's name
	public static Dictionary getEventData(String key, int route) => getData(key, "res://GameData/" + dManager.StoryFlag + "/EventData" + route + ".id");	
	
	//*	Gets the char's image from dictionary
	public static String getCharImage(String key, int flag) 
		=> getStoryCharacters(flag)[key].ToString();
	
	//*	Gets the puzzle's path
	public static String getPuzzlePath(String key) 
		=> getGlobalData("Puzzles")[key].ToString();
	
	//*	Gets the bg image path
	public static String getBGPath(String key) 
		=> getGlobalData("Backgrounds")[key].ToString();

	public static String getSFXPath(String key) 
		=> getGlobalData("SFX")[key].ToString();
	
	
	//* Called when the node enters the scene tree for the first time.
	public override void _Ready() 
	{
		timer = new Timer();
		AddChild(timer);
		generateDirs();
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
	
	//*	Music
	public static string mGymnopedie = "res://Assets/Music/Erik Satie - Gymnopédie No.1.ogg";
	//*	SFX
	public static string sfxButton = "res://Assets/SFX/click.wav";
}
