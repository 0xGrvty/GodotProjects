using Godot;
using System;

public partial class Missile : Enemy
{
    // Private variables
    private Vector2 velocity = Vector2.Zero;
    private bool collided = false;
    private Node2D target = null;
    private Vector2 targetDirection = Vector2.Zero;
    private bool attackPlayer = false;
    private Vector2 start = Vector2.Zero;
    private Vector2 targetPos = Vector2.Zero;
    private int counter = 0;
    private Vector2 remainder = Vector2.Zero;

    // Tween variables
    private Tween tween;
    [Export]
    private float time = 2;
    [Export]
    private float delay = 1;
    [Export]
    private Vector2 offset = Vector2.Zero;
    [Export]
    private Vector2 speedMultiplier = Vector2.One;

    public override void _Ready() {
        Hurtbox = (Hitbox)GetNode<Node2D>("Hitbox");
        Hurtbox.SetCollidable(true);
        GM = GetNode<Game>("/root/Game");

        start = GlobalPosition;

        AddToGroup("Actors");
        AddToGroup("Enemy Projectiles");

        foreach (Node2D a in GM.GetAllActors()) {
            if (a is Player) {
                target = a;
                targetPos = target.GlobalPosition;
                targetDirection = GlobalPosition.DirectionTo(targetPos);
            }
        }
        // This is in _Ready() to ensure we only create one tween per instance of a Missile
        InitTween();
    }

    public override void _Process(double delta) {
        Move();
    }

    public void Move() {
        var delta = GetPhysicsProcessDeltaTime();
        MoveX(velocity.X * (float)delta, new Callable(this, nameof(OnCollisionX)));
        MoveY(velocity.Y * (float)delta, new Callable(this, nameof(OnCollisionY)));
    }

    new public void OnCollisionX() {
        velocity.X = 0;
        QueueFree();
    }

    new public void OnCollisionY() {
        velocity.Y = 0;
        QueueFree();
    }

    public void InitTween() {
        // Default Missile behavior
        // The missile will travel back first, a little "wind-up"
        //tween = CreateTween().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
        // We want the velocity to change since we are on our own physics engine.
        //tween.TweenProperty(this, "velocity", targetDirection * GetMaxSpeed(), time).SetDelay(delay);

        // Medusa head behavior
        //tween = CreateTween().SetLoops().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        //tween.TweenProperty(this, "velocity", speedMultiplier * new Vector2(targetDirection.X * GetMaxSpeed(), GetMaxSpeed()), time).SetDelay(delay);
        //tween.TweenProperty(this, "velocity", speedMultiplier * new Vector2(targetDirection.X * GetMaxSpeed(), -GetMaxSpeed()), time).SetDelay(delay);

        tween = CreateTween().SetLoops().SetTrans(Tween.TransitionType.Spring).SetEase(Tween.EaseType.In);
        tween.SetParallel(true);
        tween.TweenProperty(this, "rotation", Mathf.Pi / 2, time).SetDelay(delay);
        tween.TweenProperty(this, "velocity", targetDirection * GetMaxSpeed(), time).SetDelay(delay);

        // This works, but we wouldn't be moving by integers as defined by our physics engine.
        //tween.TweenProperty(this, "global_position", targetPos, time).SetDelay(delay);
    }
}
