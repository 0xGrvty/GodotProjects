using Godot;
using System;

public class Player : Area2D {

#pragma warning disable 649
    [Export]
    public PackedScene BulletScene;
#pragma warning restore 649

    [Signal]
    public delegate void Hit();
    // This Attribute allows us to see it in the inspector window, much like Unity
    [Export]
    public int Speed = 400; // Maximum move speed of our character

    public Vector2 ScreenSize; // Size of the game window

    private Vector2 Velocity = Vector2.Zero;
    private Vector2 Facing = Vector2.Right;
    private float FireDelay = 0;
    private bool DisableInput = false;
    private float BaseFireSpeed = STARTING_FIRE_SPEED;
    private bool MaxFireDelayReached = false;
    private const float MAX_FIRE_SPEED = 0.15f;
    private const float STARTING_FIRE_SPEED = 2f;

    public void Start(Vector2 pos) {
        Position = pos;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
        DisableInput = false;
    }

    public override void _Ready() {
        Hide();
        ScreenSize = GetViewportRect().Size;
    }

    public override void _Process(float delta) {
        if (!DisableInput) {
            //Facing = Vector2.Right;
            var tempFacingX = Facing.x;
            var tempFacingY = Facing.y;
            var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            Velocity = Vector2.Zero;

            if (Input.IsActionPressed("move_right")) {
                Velocity.x += 1;

            }
            if (Input.IsActionPressed("move_left")) {
                Velocity.x -= 1;
            }
            if (Input.IsActionPressed("move_down")) {
                Velocity.y += 1;
            }

            if (Input.IsActionPressed("move_up")) {
                Velocity.y -= 1;
            }

            if (Velocity.x != 0 || Velocity.y != 0) {
                tempFacingX = Velocity.x;
                tempFacingY = Velocity.y;
            }

            Facing.x = tempFacingX;
            Facing.y = tempFacingY;

            if (Velocity.Length() > 0) {
                Velocity = Velocity.Normalized() * Speed;
                animatedSprite.Play();
            } else {
                animatedSprite.Stop();
            }


            if (Velocity.x != 0) {
                animatedSprite.Animation = "walk";
                animatedSprite.FlipV = false;
                animatedSprite.FlipH = Velocity.x < 0; // Will only flip the sprite horizontally if our velocity.x is less than 0
            } else if (Velocity.y != 0) {
                animatedSprite.Animation = "up";
                animatedSprite.FlipV = Velocity.y > 0;
            }

            if (FireDelay <= 0) {
                if (Input.IsActionPressed("fire")) {
                        
                    Bullet bullet = (Bullet)BulletScene.Instance();
                    bullet.Init(Facing);
                    // Set the bullet's transform to be the same as the Player's transform, that way
                    // If the player rotates, the transform will be relative to the player.
                    // Since there's no player rotation in this game, can also do:
                    // bullet.Position = Position;
                    bullet.Transform = Transform;

                    // Vector2.Angle() is equivalent to this:
                    // bullet.Rotation = Mathf.Atan(Facing.y / Facing.x);
                    // bullet.GetNode<Sprite>("Sprite").FlipH = animatedSprite.FlipH;
                    // or
                    // bullet.Rotation = Mathf.Atan2(Facing.y / Facing.x);
                    bullet.Rotation = Facing.Angle();

                    // Add the bullet to the parent, which is the Main scene.
                    GetParent().AddChild(bullet);
                    FireDelay = BaseFireSpeed;
                }
            } else {
                FireDelay -= delta;

            }
        }

        Position += Velocity * delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.x, 0, ScreenSize.x),
            y: Mathf.Clamp(Position.y, 0, ScreenSize.y)
            );

    }

    public void OnPlayerBodyEntered(PhysicsBody2D body) {
        Hide(); // Player disappears after being hit
        EmitSignal(nameof(Hit));

        // Must be deferred as we can't change physics properties on a physics callback
        // Disabling the area's collision shape can cause an error if it happens in the
        // middle of the engine's collision processing. Using set_deferred() tells Godot
        // to wait to disable the shape until it's safe to do so.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        DisableInput = true;
    }

    public bool GetMaxFireDelayReached() {
        return MaxFireDelayReached;
    }

    public void SetDisableInput(bool disable) {
        DisableInput = disable;
    }

    public void ResetFireSpeed() {
        BaseFireSpeed = STARTING_FIRE_SPEED;
    }

    public void ReduceFiringSpeed(float reduction) {
        var temp = BaseFireSpeed * reduction;
        BaseFireSpeed = temp >= MAX_FIRE_SPEED ? temp : MAX_FIRE_SPEED;
        if (BaseFireSpeed <= MAX_FIRE_SPEED) {
            MaxFireDelayReached = true;
        }
        FireDelay = BaseFireSpeed; ;
        Console.WriteLine("Firing Speed: " + BaseFireSpeed);
    }

}
