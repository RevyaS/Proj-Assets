using Godot;
using Godot.Collections;
using System;
using UF = UtilityFunctions;

public class TopGUI : Control
{
	public override void _Ready()
	{
//		Init Components
		bgBox = GetNode<TextureRect>("HBoxContainer/BGbox");

		charRects = new TextureRect[3]{
			GetNode<TextureRect>("HBoxContainer/BGbox/Left"),
			GetNode<TextureRect>("HBoxContainer/BGbox/Center"),
			GetNode<TextureRect>("HBoxContainer/BGbox/Right"),
		};
		
		statNames = GetNode<VBoxContainer>("HBoxContainer/StatBox/HBoxContainer/Name");
		statValues = GetNode<VBoxContainer>("HBoxContainer/StatBox/HBoxContainer/Value");
		statChanges = GetNode<VBoxContainer>("HBoxContainer/StatBox/HBoxContainer/Change");
	}
	
	
//	Initializes required references
	public void init(DataManager dManager, Game game, Tween tween)
	{
		this.dManager = dManager;
		this.game = game;
		this.tween = tween;
		
		//Clear the ff containers
		SceneManager.clearChildren(statNames);
		SceneManager.clearChildren(statValues);
		SceneManager.clearChildren(statChanges);

		// Font for each label
		DynamicFont f20 = (DynamicFont)ResourceLoader.Load("res://Resources/Acme20.tres");
		Color black = new Color("#000000");
		//Generates the labels needed
		foreach(String key in dManager.Stats.Keys)
		{
			//Label for name
			Label name = new Label();
			name.Text = key;
			name.Name = key;
			statNames.AddChild(name);
			name.Set("custom_fonts/font", f20);
			name.Set("custom_colors/font_color", black);

			//Label for values
			Label val = new Label();
			val.Name = key;
			val.Align = Label.AlignEnum.Right;
			statValues.AddChild(val);
			val.Set("custom_fonts/font", f20);
			val.Set("custom_colors/font_color", black);
			
			//Label for changes
			Label change = new Label();
			change.Name = key;
			statChanges.AddChild(change);
			change.Set("custom_fonts/font", f20);
			change.Set("custom_colors/font_color", black);
		}
	}
	
	
//	Resolve decision
	public void resolveDecision(int flag)
	{
//		Access EventData
		Dictionary eventData = dManager.EventData;
		Dictionary currData = eventData["Event" + flag.ToString()] as Dictionary;

		if(currData.Contains("Stats")) {
//			Get update dictionary
			Dictionary updateStats = currData["Stats"] as Dictionary;
			changeStats(updateStats);
		}
		
		if(currData.Contains("Image")) {
//			Get Image path
			String key = currData["Image"].ToString();
			String path = GlobalData.getBGPath(key);
//			Load Image from path
			ImageTexture img = UF.getTexture(path);
			changeImage(img);
		}
		
//		Load puzzle if it exist
		loadPuzzle(flag);

//		Load Char images
		loadChars(flag);
	}

	
//	Attaches the puzzle scene
	public void loadPuzzle(int flag)
	{
//		Access eventData
		Dictionary eventData = dManager.EventData;
		Dictionary currData = eventData["Event" + flag.ToString()] as Dictionary;

//		Exit if no puzzle key
		if(!currData.Contains("Puzzle")) return;
		
//		Get Dictionary
		Dictionary puzzleData = currData["Puzzle"] as Dictionary;
		
//		Get path
		String key = puzzleData["Name"].ToString();
		String path = GlobalData.getPuzzlePath(key);
		
//		Get instance from path
		currPuzzle = SceneManager.getSceneInstance(path) as Puzzle;
		
//		Append the instance to the bgBox
		bgBox.AddChild(currPuzzle);
		bgBox.MoveChild(currPuzzle, 0);
//		Get puzzle difficulty
		int difficulty = Convert.ToInt32(puzzleData["Difficulty"]);
		
//		Get puzzle value
		String val = puzzleData["Solved"].ToString();
		GD.Print(difficulty);
//		init the Values
		currPuzzle.init(val, difficulty);
		
//		Connect the signal
		currPuzzle.Connect("Solved", game, "nextDecision");
	}
	
	
//	Updates the background image
	public void changeImage(ImageTexture img)
	{
//		Set the Texture of the scene to the new image texture
		bgBox.Texture = img;
		
//		Check if currPuzzle exists (since it would block the view)
		if(currPuzzle != null) 
		{
			currPuzzle.QueueFree();
			currPuzzle = null;
		}
	}
	
	
//	Loads the char images given a flag
	public void loadChars(int flag, int alt = -1)
	{
//		Clear images
		clearChars();
		
//		Access eventData
		Dictionary eventData = dManager.EventData;
		String currDataKey = "Event" + flag.ToString();
		if(alt != -1) currDataKey = "Event" + flag.ToString() + "_" + alt.ToString();
		Dictionary currData = eventData[currDataKey] as Dictionary;
//		Loop the containers {Left, Center, Right}
		for(int i = 0; i < 3; i++)
		{
//			Generate key
			String key = charNames[i];
//			Check if key exist
			if(!currData.Contains(key)) continue;
			
//			Get path value
			String path = currData[key].ToString();
			
//			Get the imagePath
			String imagePath = GlobalData.getCharImage(path, dManager.StoryFlag);
			
			ImageTexture imgText = UF.getTexture(imagePath);
			charRects[i].Texture = imgText;
		}		
	}
	
	
//	Updates stats on the GUI
	public void updateStats()
	{
//		Access stats
		Dictionary stats = dManager.getData("Stats");
		
//		Iterate through each stat
		foreach(String stat in stats.Keys)
		{
//			Get the Value
			Label lblValue = statValues.GetNode(stat) as Label;
//			Update the Value
			lblValue.Text = stats[stat].ToString();
		}
	}
	
	
//	Change stats on the stats dictionary
	public void changeStats(Dictionary statChange)
	{
//		Create a copy for current stats
		Dictionary stats = dManager.getData("Stats");
		
//		Iterate through each stat
		foreach(String key in stats.Keys)
		{
//			Get the label ChangedValue
			Label lblValue = statChanges.GetNode(key) as Label;

//			If key doesn't exist just clear the value
			if(!statChange.Contains(key)) {
				lblValue.Text = "";
				continue;
			}
			
//			Update stat for each key
			int valueChange = Convert.ToInt32(statChange[key]);
			int newValue = Convert.ToInt32(stats[key]) + valueChange;
			stats[key] = newValue;
			
//			Update the label
			lblValue.Text = (valueChange > 0) ? "+" : "";
			lblValue.Text += statChange[key].ToString();
		}
		
//		Create a modulate that has 0 alpha for the fade effect ARGB format
		Color newModulate = new Color("#00ffffff");
		
//		Set modulate's alpha to max
		statChanges.Modulate = new Color("ffffff");
	
//		Interpolate modulate using tween
		tween.InterpolateProperty(statChanges, "modulate", statChanges.Modulate,
			newModulate, 5, Tween.TransitionType.Linear, Tween.EaseType.In);
		
//		Start tween
		tween.Start();
		
		dManager.updateData("Stats",stats);
		updateStats();
	}
	
	
//	Clears images from char location
	private void clearChars()
	{
		foreach(TextureRect cont in charRects)
			cont.Texture = null;
	}
	
//	Components
	private TextureRect bgBox;
	
//	Class References
	private Puzzle currPuzzle;
	private DataManager dManager;
	private Game game;
	private Tween tween;
//	References
	private String[] charNames = {"Left", "Center", "Right"};
	private VBoxContainer statValues,
						  statChanges,
						  statNames;
//	Char data to be loopable
	private TextureRect[] charRects;
}
