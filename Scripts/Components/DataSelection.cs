using Godot;
using GC = Godot.Collections;
using System;
using UF = UtilityFunctions;

public class DataSelection : Control
{
	[Signal]
	public delegate void InitFinished();
	
	[Signal]
	public delegate void LoadTriggered();
	
//	Limit of Save Files possible
	private const int SaveFiles = 24;
	
//	Component scene references
	private const string dataSlot = "res://Pages/GUI Components/DataSlot.tscn";
	
//	Component references
	private VBoxContainer slotCont;
	private Button save, load;
	
	private DataSlot selectedSlot;
	private DataManager dManager;
	
//	Thread reference
	public Thread t;
//	Reference to the current Game
	public Game game;
//	Flag to check if initialization is finished
	private bool initialized = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
//		Do not initialized if it's preloaded
		if(initialized) return;
			
		dManager = GetNode<DataManager>("/root/DataManager");
		
//		Initialize components
		slotCont = GetNode<VBoxContainer>("BG/Order/SaveLocations/LabelName");
		save = GetNode<Button>("BG/Order/Buttons/Save");
		load = GetNode<Button>("BG/Order/Buttons/Load");
		
//		Run initSlots as thread
		t = new Thread();
		t.Start(this, "initSlots");
	}
	
	
//	Perform save on selected slot
	private void savePressed()
	{
//		Print the file in the save location
		UF.writeFile(selectedSlot.saveFileLocation, dManager.getSaveData());
		
		selectedSlot.init();
	}

	
//	Load savedata from selected slot
	private void loadPressed()
	{
//		Get selectedSlot's data
		GC.Dictionary slotData = selectedSlot.saveData;
		dManager.loadSaveData(slotData);
//		Get currNode
		Location newLoc = MapData.gameMap.getNode(slotData["CurrNode"].ToString()) as Location;
		GD.Print("Switching to Game");
		
		SceneManager.Singleton.switchToGame(newLoc);
		
		Hide();
		EmitSignal(nameof(LoadTriggered));
	}
	
	
//	Initializes the amount of slots, this functions loads way too long,
//	so don't forget to thread it (added int nul because Godot threading
//	is weird and it can't find methods with no parameters')
	private async void initSlots(int nul)
	{		
//		Generate 24 Save Slots
		for(int i = 1; i <= SaveFiles; i++)
		{
//			Get reference to slot
			DataSlot newSlot;
			
//			Just connect signal to 7 preloaded values
			if ( i <= 7 )
				newSlot = slotCont.GetChild(i - 1) as DataSlot;
			else
			{
	//			create dataSlot
				newSlot = SceneManager.getSceneInstance(dataSlot) as DataSlot;
	//			set name to slot to determine slot
				newSlot.Name = i.ToString();			
				slotCont.AddChild(newSlot);				
			}

			
			GC.Array signalArgs = new GC.Array();
			signalArgs.Add(newSlot);
			
			newSlot.Connect("pressed", this, nameof(slotSelected), signalArgs );

		}
//		Wait for 0.5s to stabilize the SceneTree
		GlobalData.timer.WaitTime = 0.5f;
		GlobalData.timer.OneShot = true;
		GlobalData.timer.Start();
		await ToSignal(GlobalData.timer, "timeout");
		EmitSignal(nameof(InitFinished));
		initialized = true;
	}
	

//	Sets the slot selected for the save and load buttons
	private void slotSelected(DataSlot slot)
	{
		GD.Print("Slot selected: " + slot.Name);
		save.Disabled = false;
		load.Disabled = true;
		if(slot.saveData != null)
			load.Disabled = false;
		unselectSlots();
		slot.isSelected(true);
		selectedSlot = slot;
	}

	
//	Remove this scene from memory
	private void onClose()
	{
		Hide();
	}


//	Visual functions
	private void unselectSlots()
	{
		foreach(DataSlot s in slotCont.GetChildren())
		{
			s.isSelected(false);
		}
	}
}
