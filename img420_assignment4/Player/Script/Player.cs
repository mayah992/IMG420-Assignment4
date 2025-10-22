using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float speed = 50.0f;
	[Export] 
	public float sprintSpeed = 60.0f;
	// Optional multiplier you can tweak for a more noticeable sprint
	[Export]
	public float sprintMultiplier = 1.2f;

	private Vector2 currentVelocity;

	private AnimationPlayer _anim;
	private Vector2 lastFacing = new Vector2(0, 1); // default facing down

	private Area2D collision;

	[Export]
	public PackedScene ParticlesScene;

	// Instance of the currently spawned sprint particles (if any)
	private Node2D sprintParticlesInstance = null;

	[Signal] public delegate void GameOverEventHandler();

	public override void _Ready()
	{
		// Cache a reference to the AnimatedSprite2D for switching animations and flipping
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");

		collision = GetNode<Area2D>("Area2D");
		collision.BodyEntered += OnBodyEntered;

	}

	private void OnBodyEntered(Node2D body)
    {
        // Only react to the Enemy (customise this check as needed)
        if (body is Enemy)
		{
			// Kill Player (Game Over)
			EmitSignal(SignalName.GameOver);
        }
    }

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		// handle input
		handleInput();

		Velocity = currentVelocity;
		MoveAndSlide();


		// handle animation
		handleAnimation();
	}

	private void handleAnimation()
	{
		// if the player is moving
		if (currentVelocity.Length() > 0.1f)
		{
			// find direction based on velocity; if horizontal
			if (Math.Abs(currentVelocity.X) > Math.Abs(currentVelocity.Y))
			{
				// if x value is greater than 0
				if (currentVelocity.X > 0)
				{
					// moving right
					lastFacing = new Vector2(1, 0);
					_playAnimationIfNotPlaying("walk_right");
				}
				else
				{
					// else, moving left
					lastFacing = new Vector2(-1, 0);
					_playAnimationIfNotPlaying("walk_left");
				}
			}
			// else movement is verticle
			else
			{
				// if y valye is less than 0
				if (currentVelocity.Y < 0)
				{
					// walking backward
					lastFacing = new Vector2(0, -1);
					_playAnimationIfNotPlaying("walk_backward");
				}
				else
				{
					// else, walking forward
					lastFacing = new Vector2(0, 1);
					_playAnimationIfNotPlaying("walk_forward");
				}
			}
		}
		// else the player is not moving
		else
		{
			// use last facing direction to pick idle animation
			if (Math.Abs(lastFacing.X) > Math.Abs(lastFacing.Y))
			{
				if (lastFacing.X > 0)
					_playAnimationIfNotPlaying("idle_right");
				else
					_playAnimationIfNotPlaying("idle_left");
			}
			else
			{
				if (lastFacing.Y < 0)
					_playAnimationIfNotPlaying("idle_forward");
				else
					_playAnimationIfNotPlaying("idle_backward");
			}
		}
	}

	private void _playAnimationIfNotPlaying(string animName)
	{
		if (_anim == null)
			return;
		if (_anim.IsPlaying() && _anim.AssignedAnimation == animName)
			return;
		_anim.Play(animName);
	}

	private void handleInput()
	{
		currentVelocity = Input.GetVector("left", "right", "up", "down");

		// Debug: detect sprint state changes so you can confirm the input is registered
		bool sprintPressed = Input.IsActionPressed("sprint");

		// If sprintSpeed feels too small, consider increasing sprintMultiplier or sprintSpeed
		float effectiveSpeed = sprintPressed ? sprintSpeed * sprintMultiplier : speed;

		if (sprintPressed)
		{
			// Spawn sprint particles once when sprint starts
			if (sprintParticlesInstance == null && ParticlesScene != null)
			{
				var instance = ParticlesScene.Instantiate() as Node2D;
				if (instance != null)
				{
					// Attach to the player so it moves with them; reset local position
					instance.Position = Vector2.Zero;
					AddChild(instance);
					sprintParticlesInstance = instance;
				}
			}
		}
		else
		{
			// Sprint released: remove particle instance if it exists
			if (sprintParticlesInstance != null)
			{
				sprintParticlesInstance.QueueFree();
				sprintParticlesInstance = null;
			}
		}

			currentVelocity *= effectiveSpeed;
    }
}
