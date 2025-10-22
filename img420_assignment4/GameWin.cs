using Godot;
using System;

public partial class GameWin : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        Button menu_button = GetNode<Button>("menu");
		
		if (menu_button != null)
		{
			GD.Print("Returning to main menu");
			menu_button.Pressed += ReturnToMenu;
		}
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void ReturnToMenu()
	{
		// change to main scene
		GetTree().ChangeSceneToFile("res://menu.tscn");
	}
}
