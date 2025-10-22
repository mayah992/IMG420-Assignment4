using Godot;
using System;

public partial class Menu : Node2D
{
	// generator variable	

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		Button start_button = GetNode<Button>("start");
		Button exit_button = GetNode<Button>("exit");

		if (start_button != null)
		{
			GD.Print("Starting Game");
			start_button.Pressed += StartGame;
		}
		
		if (exit_button != null)
		{
			GD.Print("Exiting Game");
            exit_button.Pressed += ExitGame;
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void StartGame()
	{
		// change to main scene
		GetTree().ChangeSceneToFile("res://main_game.tscn");
	}
	
	private void ExitGame()
    {
		// Exit Game
		GetTree().Quit();
    }
}

