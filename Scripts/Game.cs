//Description: Contains the entire flow of the game and it's subfunctions


using Godot;
using Godot.Collections;
using System;
using UF = UtilityFunctions;
using GData = GlobalData;

public class Game : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
//		Data Manager
		dManager = GetNode<DataManager>("/root/DataManager");

		tween = GetNode<Tween>("Tween");

		locationText = GetNode<RichTextLabel>("Margin/Bottom/Elements/ScrollContainer/Text");
		
//		Components
		btnComp = GetNode<ButtonComp>("Margin/Bottom/Elements/ActionButtons/ActionButtons");
		btnComp.init(dManager, this); //Initialize references
		
		top = GetNode<TopGUI>("Margin/Top");
		top.init(dManager, this, tween);
		
		timer = locationText.GetNode<Timer>("Timer");
//		setLocation(MapData.startNode);
	}
	
//*********************************************************************************************
	
//*	Event Functions
	
	//*	This Function must be connected to the action buttons' ChangedLocation signal
	public void setLocation(Location newLocation){
		GD.Print("Updating Location");

		eventMode = false;
//		Sets currNode for button generation
		dManager.CurrNode = newLocation;

//		Access DataManager location
		Dictionary location = dManager.getData("Location");
		
//		Change image based on newLocation
		top.changeImage(newLocation.image);
		
//		Get Node name
		String nodeName = newLocation.name;

//		Loads the first event key
		dManager.EventData = dManager.getRouteData(nodeName);
		
//		Get Flag value
		int flagValue = Convert.ToInt32(location[nodeName]);
		
//		Get the Event text
		dManager.CurrText = (string)dManager.EventData["Event" + flagValue.ToString()];
		eventText(dManager.CurrText);

//		Load Char images
		top.loadChars(flagValue);
		
//		Generate buttons for new locations
		btnComp.generateButtons(dManager.CurrNode.Neighbors);
		
//		Generate possible actions
		btnComp.generateActions(flagValue);
		
//		Update stats
		top.updateStats();
	}
	
	//*	Made the text's character invisible then the timer starts to trigger onTimerTimeout
	private void eventText(string text){
		locationText.Text = text;
		locationText.VisibleCharacters = 0;
		timer.Start();
	}
	
	//*	Make the locationText.Text's characters visible every 0.05 seconds
	private void onTimerTimeout() {
		if(locationText.Text.Length != locationText.VisibleCharacters)
			locationText.VisibleCharacters += 1;
		else timer.Stop();
	}


	//*	Loads an event
	private void loadEvent(Dictionary eventData) {
		eventMode = true;
		
//		Set eventData
		dManager.EventData = eventData;
		
//		Check if eventData contains "Text0"
		if(!eventData.Contains("Text0"))
		{
//			Just configure stat changes
			dManager.resolveDecision(0);
			top.resolveDecision(0);
			setLocation(dManager.CurrNode as Location);
			return;
		}
		
//		Change image if Image exists
		if(eventData.Contains("Image")){
			GD.Print("LoadEvent method");
//			Get the image path
			String imagePath = GData.getBGPath(eventData["Image"].ToString());
//			Change the image
			top.changeImage( UF.getTexture(imagePath) );
		}

//		Load "Text0" (initial event)
		nextDecision("Text0");
	}
	
	
	//*	Change the screen based on next decision
	private void nextDecision(String decisionValue) {
//		Access eventData
		Dictionary eventData = dManager.EventData;
//		Checks if the decision value contains "Text"
		if(decisionValue.Find("Text") == -1) {
//			Doesn't contain "Text" therefore move to location
			Location moveTo = MapData.gameMap.getNode(decisionValue) as Location;
			GD.Print("Passed here");
			setLocation(moveTo);
			return;
		}

//		Else change the text
		eventText(eventData[decisionValue].ToString());
		
//		Get the flag
		int flag = Convert.ToInt32(decisionValue.Replace("Text", ""));
		
//		Resolve data and GUI processes from these 2 components
		dManager.resolveDecision(flag);
		top.resolveDecision(flag);
		
//		Generate Buttons
		btnComp.generateDecisions(flag);
	}
	
	
//*	Global Variables
//	For data
	DataManager dManager;
	
//	Tween
	private Tween tween;
	
//	For the event	
	private RichTextLabel locationText;
	public bool eventMode;
	
//	Components
	private ButtonComp btnComp;
	private TopGUI top;
	
	//*	For the locationText's text visibility
	private Timer timer;
}
