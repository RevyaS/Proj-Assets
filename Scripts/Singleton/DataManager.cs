using Godot;
using System;
using Godot.Collections;

public class DataManager : Node
{
//	Current Data being used by Game.cs
	private Dictionary currData;
	
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

	
// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		initFlags();
	}
	
	
//	Process functions
//	Performs the necessary data process in Game's nextDecision function
	public void resolveDecision(int flag)
	{
//		Updating Locations
		String updateKey = "Locations" + flag.ToString();
		if(EventData.Contains(updateKey)) {
//			Get update dictionary
			Dictionary update = EventData[updateKey] as Dictionary;
			updateData("Location", update);	
		}
		
//		Updating character flags
		String charsKey = "Chars" + flag.ToString();
		if(EventData.Contains(charsKey)) {
//			Get Image path
			Dictionary update = EventData[charsKey] as Dictionary;
			updateData("Chars", update);
		}
		
//		Updating map
		String mapKey = "Map" + flag.ToString();
		if(EventData.Contains(mapKey)) {
//			Get map flag
			MapFlag = Convert.ToInt32(EventData[mapKey]);
//			Update map
			MapData.loadMap(MapFlag);
		}
		
//		Updating route
		String routeKey = "Route" + flag.ToString();
		if(EventData.Contains(routeKey)) {
//			Get map flag
			currData["Route"] = Convert.ToInt32(EventData[routeKey]);
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
		return save;
	}
	
	
//	Gets value from current route based on key
	public Dictionary getRouteData(String key)
	{
		return GlobalData.getRouteData(Convert.ToInt32(currData["Route"]), key);
	}


//	SETTER (UPDATE) FUNCTIONS
	public void updateData(String dictionaryKey, Dictionary updateFlags)
	{
		GD.Print("Updating " + dictionaryKey + " Data");
		foreach(string key in updateFlags.Keys)
			(currData[dictionaryKey] as Dictionary)[key] = updateFlags[key];
		GD.Print(currData);
	}
	
	
//	Updates CurrData based on saveData
	public void loadSaveData(Dictionary saveData)
	{
		updateData("Location", saveData["Location"] as Dictionary);
		currData["Stats"] = saveData["Stats"] as Dictionary;
		currData["Chars"] = saveData["Chars"] as Dictionary;
		currData["Map"] = Convert.ToInt32(saveData["Map"]);
		currData["Route"] = Convert.ToInt32(saveData["Route"]);
		currData["Event"] = null;
		currData["CurrText"] = saveData["CurrText"].ToString();
//		Updates map
		MapData.loadMap(MapFlag);
		
		GD.Print("Loaded Data");
		GD.Print(currData);
	}
	
	
//	INIT FUNCTIONS
	public void initFlags()
	{
		currData = new Dictionary();
//		Load flags
		currData.Add("Location", initLocationFlags());
		currData.Add("Stats", initStats());
		currData.Add("Chars", initChars());
		currData.Add("Event", null);
		currData.Add("Map", 0);
		currData.Add("Route", 0);
		currData.Add("CurrNode", null);
		currData.Add("Text", "");
	}


	//*	Resets the flags
	private Dictionary initLocationFlags(){
		Dictionary newFlags = new Dictionary();
		newFlags.Add("House", 0);
		newFlags.Add("Downtown", 0);
		newFlags.Add("Mall", 0);
		newFlags.Add("Park", 0);
		newFlags.Add("ParkOuter", 0);
		newFlags.Add("MedicalCampus", 0);
		newFlags.Add("ShoppingStreet", 0);
		return newFlags;
	}
	
	
	//*	Resets the flags, or just mainly returns starting stats
	private Dictionary initStats(){
		Dictionary newStats = new Dictionary();
		newStats.Add("Day", 1);
		newStats.Add("Strength", 10);
		newStats.Add("Charm", 10);
		newStats.Add("Intelligence", 10);
		newStats.Add("Speed", 10);
		newStats.Add("Money", 100);
		newStats.Add("Amount", 0);
		
		return newStats;
	}
	
	
	//Init Character Flags for relationship meter
	//This will be very manual
	private Dictionary initChars()
	{
		Dictionary chars = new Dictionary();
		chars.Add("Himeno", 0);
		chars.Add("Asuka", 0);
		return chars;
	}
}
