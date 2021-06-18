//Description: Contains what an action button could do
//Soon to add: can act as a move button, or an action/choice button

using Godot;
using System;
using System.Text.RegularExpressions;
using Godot.Collections;
using UF = UtilityFunctions;

public class ActionButton : TextureButton
{
	public override void _Ready()
	{
		init();
	}
	
	public void init()
	{
//		Components
		lText = (Label)GetNode("Label");
		setText(Name);
	}
	
	
//	Due to lack of constructors, this must be called after initiating this scene
	public void init(gNode nxtLocation)
	{
		init();
		
//		Variables
		this.nextLocation = nxtLocation;
	}

	
//	Due to lack of constructors, this must be called after initiating this scene
	public void init(Dictionary eventInfo, String text)
	{
		init();
		
		setText(text);
				
//		Get the event key
		String eventKey = eventInfo["Event"].ToString();
		
		GD.Print("DEBUG: " + eventKey);
//		Get the dictionary of the event
		this.eventData = GlobalData.getEventData(eventKey, (int)eventInfo["Route"]);
	}
	
	
	//	Due to lack of constructors, this must be called after initiating this scene
	public void init(String text, String decision)
	{
		init();
		
		setText(text);
		
//		Get the dictionary of the event
		decisionValue = decision;
	}
	
//*********************************************************************************************
	
//	Observer Functions
//	Fires when this button is pressed
	private void onPress(){
//		Emits a signal with the button's value
		if(nextLocation != null)
			EmitSignal(nameof(ChangedLocation), nextLocation);
		else if(eventData != null)
			EmitSignal(nameof(TriggeredEvent), eventData);
		else if(decisionValue != "")
			EmitSignal(nameof(NextDecision), decisionValue);
	}
	
	
//	Resizes button when label resizes
	private void resizeButton()
	{
//		Don't resize if label is null to avoid null exception
		if(lText == null) return;
//		Only resize when label is larger by a significant amount (5),
//		to avoid recursive calls
		if(lText.RectSize.y < RectSize.y + 7) return;
		
//		Resize the Height of the button
		float y = lText.RectSize.y + 20;
		Vector2 sett = new Vector2(RectSize.x, y);
		RectSize = sett;
		RectMinSize = sett;
	}


//*********************************************************************************************

//	Changes the button's text
	public void setText(string newText)
	{
		lText.Text = Regex.Replace(newText.StripEdges(true, true), "%.*?%", "");
	}


//	Changes the button's text
	public string getText()
	{
		return (string)lText.Text;
	}
	
//*********************************************************************************************
	
//	Signals

//	If it acts like a location button
	[Signal]
	public delegate void ChangedLocation(Location newLocation);
	
//	If it acts like an event button
	[Signal]
	public delegate void TriggeredEvent(Dictionary eventData);
	
//	If it acts as a decision button
	[Signal]
	public delegate void NextDecision(String decisionValue);
	
//	Component Variables
	private gNode nextLocation {get; set;}
	private Dictionary eventData {get; set;}
	private String decisionValue {get; set;}
	
	Label lText;
}
