//SINGLETON 
//Description: Contains data for the game's map
//Soon to add: reads a file that contains a map and generates a graph

using Godot;
using System;
using GC = Godot.Collections;
using YamlDotNet.RepresentationModel;
using UF = UtilityFunctions;

public class MapData : Node
{
	static DataManager dManager;

// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		dManager = GetNode<DataManager>("/root/DataManager");
		
	}
		
	
//	Generates the necessary foundation for the gameMap
	public static void initData(int story)
	{
//		Reset GameMap
		gameMap = new DsGraph();
		
//		Load Location Data
		YamlMappingNode data = GlobalData.getStoryData("Locations");
		
//		Generate the nodes
//		Loop through each keys
		foreach(String locationName in data.Children.Keys)
		{
//			Load location image
			ImageTexture imgTxt = UF.getTexture(data[locationName].ToString());
//			Create a gNode based on the location
			Location newLoc = new Location(locationName, imgTxt);

			gameMap.addNode(newLoc);
		}
	}
	
	
//	Loads map and returns a Graph from the path
//	Also sets the mapData into the data from file
	public static DsGraph loadMap(int flag, int story)
	{		
//		Clear edges of gameMap
		gameMap.removeConnections();
//		gameMap.print();
//		Generate the Map Key
		String mapKey = "Map" + flag;
		startNode = null;
//		Read the current map from Story file
		YamlMappingNode data = GlobalData.getStoryData(story, mapKey);
//		Loop through all existing names for the map
		foreach(String area in data.Children.Keys)
		{
//			Access the array as List<String>
			YamlSequenceNode neighbors = data[area] as YamlSequenceNode;
		
//			Get the node from the area (LocationName)
			Location currNode = gameMap.getNode( area ) as Location;
			GD.Print(area + " : " + currNode == null);
			if(startNode == null) startNode = currNode;

//			Loop through each location's neighbor
			foreach(String neighborName in neighbors)
			{
//				Get the neighbor node
				gNode neighbor = gameMap.getNode(neighborName);

//				Connect
				gameMap.addEdge(currNode, neighbor);
			}
		}
		
		gameMap.print();
		
		return gameMap;
	}
	
//	Variables
//	The Graph of the map
	public static DsGraph gameMap;
//	The currentNode to be based by the map
	public static Location startNode;
}
