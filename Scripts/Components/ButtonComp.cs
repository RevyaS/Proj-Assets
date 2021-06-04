//* Description: For adding button purposes as well as where it will added whether if it is and event or just and ordinary button.

using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using UF = UtilityFunctions;

public class ButtonComp : HBoxContainer
{
//	Initializes required references
	public void init(DataManager dManager, Game game)
	{
		this.dManager = dManager;
		this.game = game;
	}
	
	//*	Generates a list of button of traversal based on MapData's currentNode
	public void generateButtons(List<gNode> neighbors){
		SceneManager.clearChildren(rightActionButtonContainer);
		
		GD.Print("Generating Buttons");
		
		foreach(gNode neighbor in neighbors){
			
			ActionButton newButton = addMoveButton(neighbor);
			
			newButton.Connect("ChangedLocation", game, "setLocation");
		}
	}
	
	
//	Generate Button variations for different scenarios
//*	Similar to generateButtons but this one is for actions, and it requires a flag
	public void generateActions(int flag){
//		Get the requirements if it exists
		clearContainer(true, false);
		
//		Get the dictionary for the currNode
		String dataKey = dManager.CurrNode.name;
		GD.Print("Generate action method");
		Dictionary data = dManager.getRouteData(dataKey);
		
		String actionKey = "Action" + flag.ToString();
		
//		Check if actionkey exists
		if(!data.Contains(actionKey))
			return;
		
//		Get current stats
		Dictionary stats = dManager.getData("Stats");
		Dictionary charFlags = dManager.getData("Chars");
		
//		Get the action Dictionary
		Dictionary actionData = (Dictionary)data[actionKey];
		
//		GD.Print("Printing Keys");
//		Generate buttons
		foreach(String key in actionData.Keys){
//			GD.Print(key);
//			Get event data of the action button
			Dictionary eventData = actionData[key] as Dictionary;
//			Check if eventData contains "Req"
			if(eventData.Contains("Req"))
			{
//				Compare current stats from the req
				Dictionary req = eventData["Req"] as Dictionary;
				if(!reqMet(req, stats)) continue;
			}
			
			if(eventData.Contains("Chars"))
			{
//				Compare current stats from the req
				Dictionary req = eventData["Chars"] as Dictionary;
				if(!reqMet(req, charFlags)) continue;
			}
				
			ActionButton newButton = addEventButton(eventData, key);
//			Connect the button's signal
			newButton.Connect("TriggeredEvent", game, "loadEvent");
		}
	}
	
	
//*	For event action buttons
	public void generateDecisions(int flag){
//		Clear everything
		clearContainer(true, true);
		
//		Get key
		String actionKey = "Actions" + flag.ToString();
		
//		Get dictionary of actions
		Dictionary actionData = dManager.EventData[actionKey] as Dictionary;
		
		foreach(String key in actionData.Keys){	
//			Generate an actionButton
			ActionButton newButton = addDecisionButton(key, actionData[key].ToString());
//			Connect the button's ChangedLocation signal to the changeLocation function
			newButton.Connect("NextDecision", game, "nextDecision");
		}
	}

	
	//*	Generates a button that corresponds to a node from the mapData's graph
	public ActionButton addMoveButton(gNode node){
		ActionButton moveButton = addButton();
		rightActionButtonContainer.AddChild(moveButton);
		moveButton.init(node);
		moveButton.setText(UF.extractPascal(node.name));
		return moveButton;
	}
	
	
	//*	Generates a button that contains an event
	public ActionButton addEventButton(Dictionary data, string text){
		ActionButton eventButton = addButton();
		leftActionButtonContainer.AddChild(eventButton);
		eventButton.init(data, text);
		return eventButton;
	}
	
	
	//*	Generates a button that contains an event
	public ActionButton addDecisionButton(string name, string eventValue){
		ActionButton decisionButton = addButton();
		leftActionButtonContainer.AddChild(decisionButton);
		decisionButton.init(name, eventValue);
		return decisionButton;
	}
	
	//*	Returns a default button instance with connected signalled sound effects.
	public ActionButton addButton(){
		ActionButton newButton = SceneManager.getSceneInstance(GlobalData.actionButton) as ActionButton;
		newButton.Connect("pressed", Menu.menuNode, "playSfx");
		return newButton;
	}

//*********************************************************************************************

//	Utility Methods
//	Returns true if values meet the requirements (req)
	private bool reqMet(Dictionary req, Dictionary values)
	{
		foreach(String key in req.Keys)
		{
			int reqValue = Convert.ToInt32(req[key]);
			int compValue = Convert.ToInt32(values[key]);
			if(compValue < reqValue) return false;
		}
		return true;
	}
	
//*********************************************************************************************
	
//	Control Methods
	
	//*	Clear ActionButton containers left or right
	public void clearContainer(bool left, bool right)
	{
		if(left) SceneManager.clearChildren(leftActionButtonContainer);
		if(right) SceneManager.clearChildren(rightActionButtonContainer);
	}

//*********************************************************************************************
	
	public override void _Ready(){
		rightActionButtonContainer = GetNode<VBoxContainer>("Right/Right");
		leftActionButtonContainer = GetNode<VBoxContainer>("Left/Left");
	}	
	
//	Important Singletons
	private DataManager dManager;
//	The Game singleton that owns this component
	private Game game;
	
	private VBoxContainer rightActionButtonContainer { get; set; }
	private VBoxContainer leftActionButtonContainer { get; set; }
}