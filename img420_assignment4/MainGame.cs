using Godot;
using System;

public partial class MainGame : Node2D
{
	// generator variable	
	private Player _player;
	private Pickup _generator1;
	private Pickup _generator2;
	private Pickup _generator3;

	private Area2D _escapeZone;

	private Label _generatorsLeft;

	private int genLeft = 3;

	bool doorOpened = false;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_generatorsLeft = GetNode<Label>("Player/Camera2D/GeneratorsLeft");
		_player = GetNode<Player>("Player");
		_player.GameOver += OnGameOver;

		_generator1 = GetNode<Pickup>("Generator");
		_generator2 = GetNode<Pickup>("Generator2");
		_generator3 = GetNode<Pickup>("Generator3");
		_generator1.GeneratorFound += OnGeneratorFound;
		_generator2.GeneratorFound += OnGeneratorFound;
		_generator3.GeneratorFound += OnGeneratorFound;

		_escapeZone = GetNode<Area2D>("EscapeZone");
		_escapeZone.BodyEntered += OnGameWin;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
		if (genLeft == 0 && !doorOpened)
		{
			doorOpened = true;
			StaticBody2D Door = GetNode<StaticBody2D>("Door");
			Door.QueueFree();
		}

		_generatorsLeft.Text = "" + genLeft;
    }

	private void OnGameOver()
	{
		GetTree().ChangeSceneToFile("res://game_over.tscn");
	}

	private void OnGameWin(Node2D player)
	{
		if (player is Player)
		{
			GetTree().ChangeSceneToFile("res://game_win.tscn");
        }
    }
	
	private void OnGeneratorFound()
    {
		genLeft--;
    }
}
