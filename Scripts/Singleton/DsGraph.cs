//*	SINGLETON 
//*	Description: The graph data structure

using Godot;
using System;
//*	For the ToList Function of converting ICollection into List<>
using System.Linq;
using System.Collections.Generic;
using GC = Godot.Collections;

public class DsGraph : Node
{
//*	Constructors
	
//*	Base Constructor
	public DsGraph() => nodes = new Dictionary<String, gNode>();
	
	
	public DsGraph(params gNode[] nodes):this()
	{
		foreach(gNode node in nodes)
			addNode(node);
	}
	
//*********************************************************************************************
	
	//	Returns the Count of Nodes
	public int nodeCount() => nodes.Count;
	
	
	//	Checks if 2 nodes are connected
	public bool isConnected(gNode node1, gNode node2) => (nodes[node1.name] as gNode).isNeighbor(node2);
	
	
	//	Prints Graph
	public void print(){
		GD.Print("Printing Graph");
		
		foreach(gNode node in nodes.Values)
		{
			GD.Print(node.strConnections());
		}
	}
	
//*********************************************************************************************

//	Node Functions
	
	//*	Removes a node and that includes their edges
	public bool removeNode(gNode node){
		if(!nodes.ContainsKey(node.name))
			return false;
		foreach(string key in nodes.Keys)
			removeEdge(node, nodes[key] as gNode);
		nodes.Remove(node.name);
		return true;
	}
	
	
	//*	Add a group of nodes
	public void addNodes(params gNode[] nodes){
		foreach(gNode node in nodes)
			addNode(node);
	}
	
	
	//*	Adds a node
	public bool addNode(gNode node){
		if(nodes.ContainsKey(node.name))	//!	to avoid duplication of place
			return false;
		nodes.Add(node.name, node);
		return true;
	}
	
	//*	Adds a node with a name
	public bool addNode(gNode node, string name){
		if(nodes.ContainsKey(name))	//!	to avoid duplication of place
			return false;
		nodes.Add(name, node);
		return true;
	}
	
//*********************************************************************************************
	
//	Edge Functions
	
	//*	Removed an edge for both nodes/place
//	Removes Edge between 2 nodes based on the node name
	public bool removeEdge(gNode node1, gNode node2){
//		Check if there is no connection between them or if edge ever existed
//		Only Check one since it would confirm the other
		if(!isConnected(node1, node2)) 
			return false;
			
		
//		Check if both nodes exist within this graph
		if(!nodes.ContainsKey(node1.name) ||  !nodes.ContainsKey(node2.name))
			return false;
		
//		Finally Remove their connections
		node1.removeNeighbor(node2);
		node2.removeNeighbor(node1);
		
		return true;
		
	}

	
//	Add edge between 2 nodes	
	public bool addEdge(gNode node1, gNode node2)
	{
//		Check if both objects are Connected
		if(isConnected(node1, node2)) 
			return false;
		
		if(!nodes.ContainsKey(node1.name) || !nodes.ContainsKey(node2.name))
			return false;
		
		node1.addNeighbor(node2);
//		Commented this out to make the graph directed
//		node2.addNeighbor(node1);
		
		return true;

	}
	
	
//	Clears all edges
	public void removeConnections()
	{
		foreach(gNode node in nodes.Values)
		{
			node.clearNeighbors();
		}
	}
	
//*********************************************************************************************
	
//Getters
	public gNode getNode(String nodename) => nodes[nodename] as gNode;
	
	
	public List<String> getNodeNames() => nodes.Keys.ToList();
	
	public gNode[] getNodes() => nodes.Values.ToArray<gNode>();
	
//*********************************************************************************************
	
//* Properties
	public Dictionary<String, gNode> nodes { get; set;}
}


//Sub class node for the Graph Data Structure
public class gNode : Node
{

//	Constructors
	public gNode(string name = null){
		neighbors = new List<gNode>();
		this.name = name;
	}
	
//*********************************************************************************************
	
//*	Checks if this node is neighbor of a node with given name
	public bool isNeighbor(gNode node) => neighbors.Contains(node);
	
//*	Removes neighbor
	public void removeNeighbor(gNode node) => neighbors.Remove(node);
	
//*	Adds neighbor
	public void addNeighbor(gNode node) => neighbors.Add(node);

//*********************************************************************************************

//	String Functions for Printing similar to magic functions in python

	public String strConnections()
	{
		String names = name + " : ";
		foreach(gNode neighbor in neighbors)
		{
			names += neighbor.name + ", ";
		}
		return names;
	}
	
	
//	Prints important info
	public void printInfo()
	{
		GD.Print(name);
		GD.Print(strConnections());
	}
		
	
	public void printNeighbors()
	{
		foreach(gNode neighbor in neighbors)
		{
			GD.Print(neighbor.name);
		}
	}
	
	public void clearNeighbors()
	{
		neighbors = new List<gNode>();
	}
	

//*********************************************************************************************
	
//	Fields
	private List<gNode> neighbors;
//*	Properties
	public string name { get; set; }
	public List<gNode> Neighbors{
		get {return neighbors;}
	}

}
