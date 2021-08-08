using Godot;
using System;
using System.Text.RegularExpressions;
using Godot.Collections;
using YamlDotNet.RepresentationModel;

public class DataManager : Node
{
//	Current Data being used by Game.cs
	private Dictionary currData;

	//Stroy flag
	public int StoryFlag {
		get {return Convert.ToInt32(currData["Story"]);}
	}

	public Dictionary Stats {
		get {return currData["Stats"] as Dictionary;}
	}
	
//	currNode's property
	public gNode CurrNode { 
		get{ return currData["CurrNode"] as gNode;}
		set { currData["CurrNode"] = value; }
	}
	
//	eventData property
	public Dictionary EventData {
		get {return currData["Event"] as Dictionary;}
		set {currData["Event"] = value;}
	}
	
//	MapData property
	public int MapFlag {
		get {return (int)currData["Map"];}
		set {currData["Map"] = value;}
	}
	
//	CurrText property (The current text displayed)
	public String CurrText {
		get {return currData["CurrText"] as String;}
		set {currData["CurrText"] = value;}
	}

	public int RouteFlag {
		get {return (int)currData["Route"];}
		set {currData["Route"] = (int)value;}
	}
	
	// Init Functions to give references
	public override void _Ready() {
		GlobalData.dManager = this;	
	}


//	Process functions
//	Performs the necessary data process in Game's nextDecision function
	public void resolveDecision(int flag)
	{
		// Get currData
		Dictionary currEvent = EventData["Event" + flag.ToString()] as Dictionary;

//		Updating Locations
		if(currEvent.Contains("Locations")) {
//			Get update dictionary
			Dictionary update = currEvent["Locations"] as Dictionary;
			updateData("Location", update);	
		}
		
//		Updating character flags
		if(currEvent.Contains("Chars")) {
//			Get Image path
			Dictionary update = currEvent["Chars"] as Dictionary;
			updateData("Chars", update);
		}
		
//		Updating map;
		if(currEvent.Contains("Map")) {
//			Get map flag
			MapFlag = Convert.ToInt32(currEvent["Map"]);
//			Update map
			MapData.loadMap(MapFlag, StoryFlag);
		}
		
//		Updating route
		if(currEvent.Contains("Route")) {
//			Get map flag
			currData["Route"] = Convert.ToInt32(currEvent["Route"]);
		}
	}
	
	
//	GETTER FUNCTIONS
	public Dictionary getData(String key)
	{
		return currData[key] as Dictionary;	
	}
	
	
//	Generates Save Data info from currData
	public Dictionary getSaveData()
	{
		Dictionary save = new Dictionary();
		save.Add("Location", currData["Location"]);
		save.Add("Stats", currData["Stats"]);
		save.Add("Chars", currData["Chars"]);
		save.Add("Map", MapFlag);
		save.Add("Route", currData["Route"]);
		save.Add("CurrNode", CurrNode.name);
		save.Add("CurrText", CurrText);
		save.Add("Story", currData["Story"]);
		return save;
	}
	
	
//	Gets value from current route based on key
	public Dictionary getRouteData(String key)
		=> YamlToDict(GlobalData.getRouteData(Convert.ToInt32(currData["Route"]), key));	


//	SETTER (UPDATE) FUNCTIONS
	public void updateData(String dictionaryKey, Dictionary updateFlags)
	{
		GD.Print("Updating " + dictionaryKey + " Data");
		foreach(string key in updateFlags.Keys)
			(currData[dictionaryKey] as Dictionary)[key] = updateFlags[key];
	}
	
	
//	Updates CurrData based on saveData
	public void loadSaveData(Dictionary saveData)
	{
		if(currData == null) initFlags( Convert.ToInt32(saveData["Story"]) );
		
		updateData("Location", saveData["Location"] as Dictionary);
		updateData("Stats", saveData["Stats"] as Dictionary);
		updateData("Chars", saveData["Chars"] as Dictionary);
		currData["Map"] = Convert.ToInt32(saveData["Map"]);
		currData["Route"] = Convert.ToInt32(saveData["Route"]);
		currData["Event"] = null;
		currData["CurrText"] = saveData["CurrText"].ToString();
		int storyVal = Convert.ToInt32(saveData["Story"]);
		currData["Story"] = storyVal;
//		Updates map
		MapData.initData(storyVal);
		MapData.loadMap(MapFlag, storyVal);
		
		GD.Print("Loaded Data");

	}
	
	
//	INIT FUNCTIONS
	public void initFlags(int flag)
	{
		currData = new Dictionary();
//		Load flags
		currData.Add("Story", flag);
		currData.Add("Location", initLocationFlags());
		currData.Add("Stats", initStats());
		currData.Add("Chars", initChars());
		currData.Add("Event", null);
		currData.Add("Map", 0);
		currData.Add("Route", 0);
		currData.Add("CurrNode", null);
		currData.Add("Text", "");
	}


	//Gets the baseStats from GlobalData + flag file but currData must have Story flag first
	private Dictionary initStats()
		=> YamlToDict(GlobalData.getStoryData("BaseStats"));
		
	//Gets the baseChars from GlobalData + flag file and required Story flag from currData
	private Dictionary initChars()
		=> YamlToDict(GlobalData.getStoryData("BaseChars"));


	//*	Resets the flags based on flags existing in GlobalData + flag, reqyures Story key in currData to exist
	private Dictionary initLocationFlags(){
		YamlMappingNode keyList = GlobalData.getStoryLocations();
		
		Dictionary newFlags = new Dictionary();
		foreach(String key in keyList.Children.Keys)
		{
			newFlags.Add(key, 0);
		}
		return newFlags;
	}


	//Converts YmamlMappingNode to Dictionary
	private Dictionary YamlToDict(YamlMappingNode yamlData)
	{
		Dictionary data = new Dictionary();
		//GD.Print(yamlData.ToString());
		foreach(YamlScalarNode key in yamlData.Children.Keys)
		{
			object output = null;
			if(yamlData.Children[key] is YamlMappingNode)
				output = YamlToDict(yamlData.Children[key] as YamlMappingNode);
			else 
				output = (yamlData.Children[key] as YamlScalarNode).Value;
			data.Add(key.Value, output);
		}
		//GD.Print(data);
		return data;
	}
}
