using System;									// System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;							// System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
	Scene currentScene;
	UI ui;
	public string currentMapName = "Facility.tmx";
	bool levelLoad = false;

	public MyGame() : base(200, 150, false, false, 800, 600, true)		// Create a window that's 800x600 and NOT fullscreen
	{
		createLevel(currentMapName);
		Console.WriteLine("MyGame initialized");
	}

	// For every game object, Update is called every frame, by the engine:
	void Update()
	{
		ui.clearText(); //since this is always the first update called, clear the ui text here

		//DEBUG reload level
		if(Input.GetKeyDown(Key.R))
        {
			currentScene.LateDestroy();
			ui.LateDestroy();
        }
		if(Input.GetKeyUp(Key.R))
        {
			createLevel(currentMapName);
        }

		if(levelLoad)
        {
			createLevel(currentMapName);
			levelLoad = false;
            Console.WriteLine("new Level loaded");
        }
	}

	void createLevel(String mapName)
    {
		createUI();
		currentScene = new Scene(ui);
		AddChild(currentScene);
		currentScene.loadMapFile(mapName);
		currentScene.createLevel();

		//set the index of the ui to be in front of the level
		this.SetChildIndex(ui, 1);
	}

	void createUI()
    {
		ui = new UI(width, height);
		AddChild(ui);
    }

	public void loadNewLevel(string newMapName)
    {
		currentScene.LateDestroy();
		ui.LateDestroy();
		currentMapName = newMapName;
		levelLoad = true;
    }

	static void Main()							// Main() is the first method that's called when the program is run
	{
		new MyGame().Start();					// Create a "MyGame" and start it
	}
}