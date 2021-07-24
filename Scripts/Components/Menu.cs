//* Description: Manages menu buttons

using Godot;
using System;

public class Menu : VBoxContainer{
	
	//*	If setting is pressed then the signal is being emitted to the game.cs
	public void settingPressed(){
		playSfx();
//		Generate options page
		Options opt = SceneManager.getSceneInstance(GlobalData.optionPath) as Options;
//		Activate Main menu screen since we're in game
		opt.enableMainMenuButton();
//		Activate Data Button when not in eventMode
		if(!SceneManager.Game.eventMode)
			opt.enableDataButton();
		SceneManager.Singleton.stackPage(opt);
	}
	
	//* When sound button is being pressed, the sfx will be off or on otherwise
	private void soundPressed(){
		isSfxOn = !isSfxOn;
		onOffTextures(1);
	}
	
	public void playSfx(){
		sfx.Stream = loadAudio(GlobalData.sfxButton);
		if(isSfxOn) sfx.Play();
	}


	// Plays specific Sfx
	public void playSfx(String sfxPath){
		sfx.Stream = loadAudio(sfxPath);
		if(isSfxOn) sfx.Play();
	}
	
	
	//*	Initialize some values of music and sounds
	public void playBGM(String bgmPath){
		if(currentBGMPath == bgmPath) return;

		currentBGMPath = bgmPath;
		bgMusic.Stream = loadAudio(bgmPath);
		bgMusic.Play();
	}
	
	
	//* When music button is being pressed, the music will be off or on otherwise
	private void musicPressed(){
		isMusicOn = !(bgMusic.StreamPaused = !bgMusic.StreamPaused);
		onOffTextures(2);
	}
	
	
	//* Decide on what the Textures of the off or on button will be
	private void onOffTextures(int code){
		switch(code){
			case 1:
				soundButton.TextureNormal = (isSfxOn) ? loadSomeIconStuffs("/Sound/Sfx.png") : loadSomeIconStuffs("/Sound/Sfx3.png");
				break;
			case 2:
				musicButton.TextureNormal = (isMusicOn) ? loadSomeIconStuffs("/Music/Music1.png") : loadSomeIconStuffs("/Music/Music3.png");
				break;
		}
	}
	
	//* Returns some loading icon png image
	private Texture loadSomeIconStuffs(string endPath) => GD.Load<Texture>(loadIconPath + endPath);
	
	//* Load some audio and return
	private AudioStream loadAudio(string path) => GD.Load<AudioStream>(path);
	
	//*	Initialize some texture in buttons
	private void initTextures(){
		soundButton = GetNode<TextureButton>("Sound");
		musicButton = GetNode<TextureButton>("Music");
		
		//* Set the Button Textures by loading some icons
		onOffTextures(1);
		soundButton.TexturePressed = loadSomeIconStuffs("/Sound/Sfx3.png");
		soundButton.TextureHover = loadSomeIconStuffs("/Sound/Sfx2.png");
		
		onOffTextures(2);
		musicButton.TexturePressed = loadSomeIconStuffs("/Music/Music3.png");
		musicButton.TextureHover = loadSomeIconStuffs("/Music/Music2.png");
	}
	
	
	public override void _Ready(){
		sfx = GetNode<AudioStreamPlayer>("SFX");
		bgMusic = GetNode<AudioStreamPlayer>("BGM");
		
		isSfxOn = isMusicOn = true;	//*	default value
		initTextures();
		
		sfx.Stream = loadAudio(GlobalData.sfxButton);
		menuNode = GetParent().GetNode<VBoxContainer>("Menu");
	}

	[Signal]
	private delegate void stackSettingScene(Control setting);
	
	public static AudioStreamPlayer bgMusic { get; set; }
	// SFX
	public static AudioStreamPlayer sfx { get; set; }
	public static VBoxContainer menuNode { get; set; }
	public static bool isMusicOn { get; set; }
	public static bool isSfxOn { get; set; }
	
	private string currentBGMPath = "";
	private string loadIconPath = "res://Assets/Icons";
	public static string menuNodePath = "Margin/Bottom/Elements/MenuContainer/CenterContainer/Menu";
	private TextureButton soundButton, musicButton;
	

}
