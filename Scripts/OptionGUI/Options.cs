//* Description: Use for removing the node itself

using Godot;
using System;
using UF = UtilityFunctions;

public class Options : Control{
	
	//*	where the signal ValueChanged method from MasterVol HSlider emitted
	public void onMasterVolValueChanged(float value){
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), value);
		GD.Print("Master Volume: ", AudioServer.GetBusVolumeDb(0));
	}
	
	//*	where the signal ValueChanged method from MusicVol HSlider emitted
	public void onMusicSliderVolValueChanged(float value){
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), value);
		GD.Print("Music volume: ", AudioServer.GetBusVolumeDb(1));
	}
	
	//*	where the signal ValueChanged method from SfxVol HSlider emitted
	public void onSfxSliderVolValueChanged(float value){
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), value);
		GD.Print("Sound db: ", AudioServer.GetBusVolumeDb(2));
	}
	
	
//	Use to Open the Data Selection GUI
	public void dataButtonPressed(){
		SceneManager.Singleton.DataSelect.Connect("LoadTriggered", this, nameof(closeMenu));
		SceneManager.Singleton.showDataSelect();
	}
	
	
//	Activates the Save/Load button
	public void enableDataButton()
	{
		(GetNode<Button>("BG/OptionList/DataButton") as Button).Show();
	}
	
	
//	Activates the Main Menu button
	public void enableMainMenuButton()
	{
		(GetNode<Button>("BG/OptionList/MainMenu") as Button).Show();
	}
	
//	Returns to MainMenu
	private void returnToMainMenu()
	{
		SceneManager.Singleton.setScene(GlobalData.mainMenuPath);
		QueueFree();
	}
	
	
	//*	Undo the changes of the option
	private void closeMenu(){
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), masterDb);
		GD.Print("Master: ", masterDb);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), musicDb);
		GD.Print("Music: ", musicDb);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), sfxDb);
		GD.Print("Sfx: ", sfxDb);
		QueueFree();
	}
	
	//*	HSlider getter
	private HSlider GetSlider(string path){
		return GetNode<HSlider>(path);
	}
	
	//*	Initializing values of HSliders
	private void initValues(){
		GetSlider(gridContainerPath + "/MasterVol").Value = masterDb = AudioServer.GetBusVolumeDb(0);
		GetSlider(gridContainerPath + "/MusicVol").Value = musicDb = AudioServer.GetBusVolumeDb(1);
		GetSlider(gridContainerPath + "/SfxVol").Value = sfxDb = AudioServer.GetBusVolumeDb(2);
	}
	
	public override void _Ready(){
		initValues();
	}
	
	
	private float masterDb, musicDb, sfxDb;
	private const string gridContainerPath = "BG/OptionList/GridContainer";
}
