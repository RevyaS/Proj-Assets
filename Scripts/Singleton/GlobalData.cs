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
	
	//*	Gets the Dictionary data from route0
	public static Dictionary getRouteData(int routeNumber, string key)
	{
		String routeLocation = "res://GameData/Route" +  routeNumber + ".id";
		return getData(key, routeLocation);
	}
	
	//*	Gets the global data
	public static Dictionary getGlobalData(string key) 
		=> getData(key, globalData);
	
	
	//*	Gets the Dictionary data from Event
	//*	The key is based on the Event's name
	public static Dictionary getEventData(String key) 
		=> getData(key, eventData);
	
	//*	Gets the char's image from dictionary
	public static String getCharImage(String key) 
		=> getGlobalData("Characters")[key].ToString();
	
	
	//*	Gets the puzzle's path
	public static String getPuzzlePath(String key) 
		=> getGlobalData("Puzzles")[key].ToString();
	
	//*	Gets the bg image path
	public static String getBGPath(String key) 
		=> getGlobalData("Backgrounds")[key].ToString();
	
	
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

	//*	Data
	private static string globalData = "res://GameData/GlobalData.id";
	
	private static string eventData = "res://GameData/EventData.id";
	
	//*	Music
	public static string mGymnopedie = "res://Assets/Music/Erik Satie - Gymnopédie No.1.ogg";
	//*	SFX
	public static string sfxButton = "res://Assets/Music/clickSfx.wav";
}
