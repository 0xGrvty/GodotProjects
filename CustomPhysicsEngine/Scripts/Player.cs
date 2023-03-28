using Godot;
using System;

public partial class Player : Actor {
	private Vector2 velocity = Vector2.Zero;
	private float maxSpeed = 100;
	private float maxAccel = 800;
	private AnimatedSprite2D animatedSprite;
	private float gravity = 1000;
	private float maxFall = 160;
	private float jumpForce = -160;
	private float jumpHoldTime = 0.2f;
	private float localHoldTime = 0;
	


    public override void _Ready() {
		// No onready, so get the node in this part of the pipeline.
		animatedSprite = GetNode<AnimatedSprite2D>("Animations");
        Hitbox = (Hitbox)GetNode<Node2D>("Hitbox");

        // Since C# does not have onready, we still need to fetch the globals.
        // The documentation said we should be able to just call Game, but it did not work.
        // It is because GetTree is not a static method, it seems like.  I wonder if I am doing something incorrectly.
        GM = GetNode<Game>("/root/Game");
        AddToGroup("Actors");
    }
    public override void _Process(double delta) {
		var direction = Math.Sign(Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"));
		var onGround = GM.CheckWallsCollision(this, Vector2.Down);
		//var onGround = GlobalPosition.Y >= 160;

		var jumping = Input.IsActionJustPressed("Jump");

		if (jumping && onGround) {
			velocity = new Vector2(velocity.X, jumpForce);
			localHoldTime = jumpHoldTime;
		} else if (localHoldTime > 0) {
			if (Input.IsActionPressed("Jump")) {
				velocity = new Vector2(velocity.X, jumpForce);
			} else {
				localHoldTime = 0;
			}
		}

		localHoldTime -= (float)delta;

		if (direction > 0) { animatedSprite.FlipH = false; }
		else if (direction < 0) { animatedSprite.FlipH = true; }
		
		if (direction != 0) { animatedSprite.Play("Run"); }
		else { animatedSprite.Play("Idle"); }

		velocity.X = Mathf.MoveToward(velocity.X, maxSpeed * direction, maxAccel * (float)delta);
		velocity.Y = Mathf.MoveToward(velocity.Y, maxFall, gravity * (float)delta);

		// In Actor, if a collision is detected, then perform the callback function
		MoveX(velocity.X * (float)delta, new Callable(this, nameof(OnCollisionX)));
		MoveY(velocity.Y * (float)delta, new Callable(this, nameof(OnCollisionY)));
	}


    public void OnCollisionX() {
		velocity.X = 0;
		ZeroRemainderX();
    }

	public void OnCollisionY() {
		velocity.Y = 0;
		ZeroRemainderY();
	}
}
