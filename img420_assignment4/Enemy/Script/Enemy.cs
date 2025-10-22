using Godot;
using System;

/// <summary>
/// A simple enemy that uses a NavigationAgent2D to follow the player.  To use this
/// script, create a CharacterBody2D with a NavigationAgent2D child.  In the
/// inspector, set the "TargetPath" export to point to the Player node.  Ensure
/// your TileSet has a NavigationLayer painted on walkable tiles and that your
/// TileMap has a NavigationRegion2D so the agent can compute paths.  The enemy
/// will continuously update its target and move along the computed path.
/// </summary>
public partial class Enemy : CharacterBody2D
{
    [Export]
    public float Speed = 120f;

    /// <summary>
    /// Exposed NodePath to assign the target (e.g. Player) in the editor.
    /// </summary>
    [Export]
    public NodePath TargetPath;

    private NavigationAgent2D _navAgent;
    private Node2D _target;

    private Vector2 lastFacing = new Vector2(0, 1);

    private AnimationPlayer _anim;

    public override void _Ready()
    {
        _navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

        if (TargetPath != null)
        {
            _target = GetNode<Node2D>(TargetPath);
        }

        // Configure navigation agent properties if needed (e.g., max speed)
        // _navAgent.MaxSpeed = Speed;

        // Cache a reference to the AnimatedSprite2D for switching animations and flipping
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_target == null || _navAgent == null)
            return; 

        // Update the navigation target each frame to follow the player's current position
        _navAgent.TargetPosition = _target.GlobalPosition;

        // Retrieve the next point along the computed path
        Vector2 nextPoint = _navAgent.GetNextPathPosition();

        // Compute direction towards the next point
        Vector2 direction = (nextPoint - GlobalPosition).Normalized();

        // Move towards the target
        Velocity = direction * Speed;
        MoveAndSlide();

        // Handle animation
        handleAnimation();
    }

    private void handleAnimation()
    {
        // if the player is moving
        if (Velocity.Length() > 0.1f)
        {
            // find direction based on velocity; if horizontal
            if (Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
            {
                // if x value is greater than 0
                if (Velocity.X > 0)
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
                if (Velocity.Y < 0)
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
}
