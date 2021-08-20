using Godot;
using System;
using GC = Godot.Collections;

public enum StoryLine {AuthorsLabrat, AchingDreams};

public class MainMenu : TextureRect
{
	DataManager dManager;
	//Components
	VBoxContainer storyButtonCont;
	TabContainer infoTab;
	RichTextLabel synopsisText;
	TextureRect coverImage;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dManager = GetNode<DataManager>("/root/DataManager") as DataManager;
		storyButtonCont = GetNode<VBoxContainer>("VBoxContainer/Sides/Stories/ScrollContainer/BtnContainers");
		infoTab = GetNode<TabContainer>("VBoxContainer/Sides/Info");
		synopsisText = GetNode<RichTextLabel>("VBoxContainer/Sides/Info/Synopsis/RichTextLabel");
		coverImage = GetNode<TextureRect>("VBoxContainer/Sides/StoryCover");
		generateStories();
	}
	
	/// <summary>
	///Generates a new story
	/// </summary>
	/// <param name="story">Refers to the number on the folder</param>
	private void newGame(int storyIndex)
	{
//		Switch to game
		GD.Print("New Game");
//		Reset data
		dManager.initFlags(storyIndex);
		MapData.initData(storyIndex);
		MapData.loadMap(0, storyIndex);
		SceneManager.Singleton.switchToGame(MapData.startNode);
//		QueueFree();
	}

	private void newGame(int storyIndex, GC.Dictionary overrideStats)
	{
		GD.Print("New Game");
//		Reset data
		dManager.initFlags(storyIndex);
		MapData.initData(storyIndex);
        if(overrideStats.Contains("Chars"))
	        dManager.updateData("Chars", overrideStats["Chars"] as GC.Dictionary);
		if(overrideStats.Contains("Stats"))
	        dManager.updateData("Stats", overrideStats["Stats"] as GC.Dictionary);
		if(overrideStats.Contains("Locations"))
	        dManager.updateData("Chars", overrideStats["Locations"] as GC.Dictionary);
		if(overrideStats.Contains("Map"))
            dManager.MapFlag = Convert.ToInt32(overrideStats["Map"]);
		if(overrideStats.Contains("Route"))
            dManager.RouteFlag = Convert.ToInt32(overrideStats["Route"]);
        MapData.loadMap(dManager.MapFlag, storyIndex);
        SceneManager.Singleton.switchToGame(MapData.startNode);
	}


	//Generates the buttons for every story registered in GlobalData
	private void generateStories()
	{
		GC.Dictionary storyData = GlobalData.storyData;
		foreach(String index in storyData.Keys)
		{
            GC.Dictionary storyInfo = storyData[index] as GC.Dictionary;
            String title = storyInfo["Title"].ToString();
            Button b = generateButton(title);
			storyButtonCont.AddChild(b);

			if(storyInfo.Contains("Chapters"))
                b.Connect("pressed", this, nameof(showChapters), new GC.Array(Convert.ToInt32(index)));
			else
				b.Connect("pressed", this, nameof(newGame), new GC.Array(Convert.ToInt32(index)));

            b.Connect("mouse_entered", this, nameof(showInfo), new GC.Array(Convert.ToInt32(index)));
		}
	}


	//Shows the Synopsis of the Story flag
	private void showInfo(int story)
	{
        infoTab.CurrentTab = (infoTab.CurrentTab != 0) ? 0 : infoTab.CurrentTab;
		GC.Dictionary storyData = GlobalData.storyData[story.ToString()] as GC.Dictionary;
		synopsisText.Text = storyData["Synopsis"].ToString();
        coverImage.Texture = null;
        if(storyData.Contains("Cover"))
			coverImage.Texture = UtilityFunctions.getTexture(storyData["Cover"].ToString());
	}


	//Presents the list of chapters
	private void showChapters(int story)
	{
		infoTab.CurrentTab = (infoTab.CurrentTab != 1) ? 1 : infoTab.CurrentTab;
		GC.Dictionary storyData = GlobalData.storyData[story.ToString()] as GC.Dictionary;
		if(!storyData.Contains("Chapters")) return;
        //Get the Chapter button container
        VBoxContainer chaptersCont = infoTab.GetNode<VBoxContainer>("Chapters/Container");
        SceneManager.clearChildren(chaptersCont);
        GC.Dictionary chapters = storyData["Chapters"] as GC.Dictionary;
        foreach(String title in chapters.Keys)
		{
            Button b = generateButton(title);
            chaptersCont.AddChild(b);
            GC.Dictionary overrideStats = chapters[title] as GC.Dictionary;
			b.Connect("pressed", this, nameof(newGame), new GC.Array(Convert.ToInt32(story), overrideStats));
        }
    }


	//Generates button
	private Button generateButton(String title)
	{
		Button b = new Button();
        b.Text = title;
        b.RectMinSize = new Vector2(0, 50);
		b.Theme = ResourceLoader.Load(GlobalData.themeActionButton) as Theme;
		b.Set("custom_fonts/font", ResourceLoader.Load(GlobalData.fontAcme20) as Font);
        return b;
    }


//	Load game via Data Select
	private void loadGame()
	{
		SceneManager.Singleton.showDataSelect();
	}


//	Open options
	private void openOptions()
	{
//		Generate options page
		Options opt = SceneManager.getSceneInstance(GlobalData.optionPath) as Options;
		SceneManager.Singleton.stackPage(opt);
	}
}
