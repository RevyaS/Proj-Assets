using Godot;
using GA = Godot.Collections.Array;
using UF = UtilityFunctions;
using System;

public class LightsOut : Puzzle{
	//* Ready method
	public override void _Ready(){

		
		string bulbPath = "res://Assets/Puzzles/Bulb/";
		
		//* Initializing on bulb textures
		normalOn = getBulbTexture(bulbPath + "BulbBright.jpg");
		hoverOn = getBulbTexture(bulbPath + "BulbBright1.jpg");
		pressedOn = getBulbTexture(bulbPath + "BulbDark.jpg");
		
		//* Initializing off bulb textures
		normalOff = getBulbTexture(bulbPath + "BulbDark.jpg");
		hoverOff = getBulbTexture(bulbPath + "BulbDark1.jpg");
		pressedOff = getBulbTexture(bulbPath + "BulbBright.jpg");
		
		// init("", 8);
	}
	
	
	//* Initialize difficulty
	public override void initDifficulty(int level){
		container = GetNode<GridContainer>("GridContainer");
		
		base.initDifficulty(container.Columns = level);
		
		//* If the level is lesser than 6 then it multiplied itself else multiplied by 5(5 row limit)
		button = new TextureButton[(level < 6) ? (int)Math.Pow(level, 2.0) : level * 5];
		
		container.RectPosition = getContainerPos();
		
		//* Add buttons
		for(int i = 0; i < button.Length; ++i){
			initBulb(i);
			container.AddChild(button[i]);
		}  
		startGame();
	}
	
	//* Initalized a certain bulb with a given index
	private void initBulb(int index){
		button[index] = SceneManager.getSceneInstance(GlobalData.iconButton) as TextureButton;
		button[index].Connect("pressed", this, nameof(onPressed), new GA() {index});
		setOff(index);
	}
	
	//* Checks the lights if it has light bulb, returned false otherwise true.
	private bool checkWin(){
		bool win = true;
		foreach(TextureButton btn in button){
			if(btn.EditorDescription.Equals("On"))
				return !win;
		} return win;
	}
	
	//* Pressed the tile or button bulb
	private void onPressed(int i){
		int col = container.Columns;
		int temp = (i + 1) % col;
		int[] arr = (temp != 0 && (temp - 1 != 0)) ? new int[] {i, i - 1, i + 1, i - col, i + col} :
					(temp != 0) ? new int[] {i, i + 1, i - col, i + col} : new int[] {i, i - 1, i - col, i + col};
		foreach(int val in arr)
			tryOnOff(val);
		bool win = checkWin();
		GD.Print("Win: ", win);
		if(win) {
			disableMouse();
			solved();
		}
	}
	
	//* try to on or off the button and ignore the index out of bounds exception. Because I used 1D array and I'm to laze to change it
	private void tryOnOff(int i){
		try{
			setOffOnBtn(button[i].EditorDescription, i);
		} catch(Exception){}
	}
	
	//* Start or restart the game. Just doing randomize pressed button.
	private void startGame(){
		RandomNumberGenerator rnd = new RandomNumberGenerator();
		//Generate steps as amount of clicks to generate
		int steps = rnd.RandiRange(5, button.Length*2 -1);
		
		//Every step will choose a random position to click, therefore it will always have an on bulb
		for(int i = 0; i < steps; i ++)
		{
			int pos = rnd.RandiRange(1, button.Length-1);
			onPressed(pos);	
		}
	}
	
	
	//Disable all mouse input on tiles
	private void disableMouse() {
		foreach(TextureButton btn in button)
			btn.MouseFilter = MouseFilterEnum.Ignore;
	}
	
	
	//* Return the bulb texture
	private ImageTexture getBulbTexture(string path) => UF.getTexture(ProjectSettings.GlobalizePath(path));
	
	
	//* Returns a container position
	private Vector2 getContainerPos() =>
		(container.Columns <= 3) ? new Vector2(215, 66) :
		(container.Columns == 4) ? new Vector2(188, 45) :
		(container.Columns == 5) ? new Vector2(160, 19) :
		(container.Columns == 6) ? new Vector2(130, 18) :
		(container.Columns == 7) ? new Vector2(101, 18) : 
			new Vector2(72, 18);
	
	
	//* Decides either the bulb button turned off or on
	private void setOffOnBtn(string str, int i){
		if(str.Equals("Off"))
			setOn(i);
		else setOff(i);
	}
	
	//* Turn on the off bulb button
	private void setOn(int i){
		button[i].EditorDescription = "On";
		button[i].TextureNormal = normalOn;
		button[i].TextureHover = hoverOn;
		button[i].TexturePressed = pressedOn;
	}
	
	//* Turn off the on bulb button
	private void setOff(int i){
		button[i].EditorDescription = "Off";
		button[i].TextureNormal = normalOff;
		button[i].TextureHover = hoverOff;
		button[i].TexturePressed = pressedOff;
	}
	
	private GridContainer container { get; set; }
	private TextureButton[] button;
	private ImageTexture hoverOn, normalOn, pressedOn;
	private ImageTexture hoverOff, normalOff, pressedOff;
}
