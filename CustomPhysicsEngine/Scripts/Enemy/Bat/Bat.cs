using Godot;
using System;

public partial class Bat : Enemy {
    private Vector2 velocity = Vector2.Zero;
    //private bool onGround = true;
    private Node2D target = null;
    private Vector2 targetDirection = Vector2.Zero;
    private bool attackPlayer = false;
    private Vector2 startingPos = Vector2.Zero;
    private Vector2 targetPos = Vector2.Zero;
    private Vector2 swoopDirection = Vector2.Zero;
    private int counter = 0;

    // Tween for straight state
    private Tween tween;
    private Vector2 start;
    private Vector2 follow;
    [Export]
    Vector2 offset = Vector2.Zero;
    [Export]
    float time = 2;
    [Export]
    float delay = 1;
    public enum Behavior {
        SLEEP = 0,
        STRAIGHT = 1,
        SWOOP = 2,
        CVANIA = 3,
        ROCKET = 4
    }

    [Export]
    private Behavior behavior = Behavior.SLEEP;
    [Export]
    private float jumpHeight = 10f;
    [Export]
    private float jumpTimeToPeak = 0.4f;
    [Export]
    private float jumpTimeToDescent = 0.2f;
    private float jumpVelocity;
    private float jumpGravity;
    private float fallGravity;


    // State Machine
    public IStateMachine currentState;
    public BatSleepState sleepState;
    public BatSwoopState swoopState;
    public BatStraightState straightState;
    public BatCVaniaState cvaniaState;
    public BatRocketState rocketState;


    public override void _Ready() {
        Hitbox = (Hitbox)GetNode<Node2D>("Hitbox");
        Hitbox.SetCollidable(false);

        GM = GetNode<Game>("/root/Game");

        AddToGroup("Actors");

        startingPos = GlobalPosition;

        sleepState = new BatSleepState();
        swoopState = new BatSwoopState();
        straightState = new BatStraightState();
        cvaniaState = new BatCVaniaState();
        rocketState = new BatRocketState();

        //switch (behavior) {
        //    case Behavior.SLEEP:
        //        currentState = sleepState;
        //        break;

        //    case Behavior.STRAIGHT:
        //        currentState = straightState;
        //        break;

        //    case Behavior.SWOOP:
        //        currentState = swoopState;
        //        break;

        //    case Behavior.CVANIA:
        //        currentState = cvaniaState;
        //        break;

        //    default:
        //        currentState = sleepState;
        //        break;
        //}
        currentState = sleepState;
    }


    public override void _Process(double delta) {
        //velocity.X = Mathf.MoveToward(velocity.X, maxSpeed, maxAccel * (float)delta);
        //velocity.Y = Mathf.MoveToward(velocity.Y, GetGravity(), GetGravity() * (float)delta);

        //MoveX(velocity.X * (float)delta, new Callable(this, nameof(OnCollisionX)));
        //MoveY(velocity.Y * (float)delta, new Callable(this, nameof(OnCollisionY)));

        currentState = currentState.EnterState(this);
    }

    public void Move() {
        var delta = GetPhysicsProcessDeltaTime();
        switch (behavior) {
            case Bat.Behavior.SLEEP:
                velocity = Vector2.Zero;
                break;

            case Bat.Behavior.STRAIGHT:
                velocity.X = Mathf.MoveToward(velocity.X, GetMaxSpeed() * GlobalPosition.DirectionTo(target.GlobalPosition).X, GetMaxAccel() * (float)delta);
                velocity.Y = Mathf.MoveToward(velocity.Y, GetMaxSpeed() * GlobalPosition.DirectionTo(target.GlobalPosition).Y, GetMaxAccel() * (float)delta);
                break;
            // This can probably be done with a tween.  Let's explore this option later.
            case Bat.Behavior.SWOOP:
                velocity.X = Mathf.MoveToward(velocity.X, targetDirection.X * GetMaxSpeed(), GetMaxAccel() * (float)delta);
                // y = mx + b <- velocity
                // y = mx^2 + 2mx + b <- position
                //velocity.Y = Mathf.MoveToward(velocity.Y, swoopDirection.X * GetMaxSpeed() * swoopPos.DirectionTo(GlobalPosition).X, GetMaxAccel() * (float)delta);
                velocity.Y = Mathf.MoveToward(velocity.Y, swoopDirection.X * GetMaxSpeed() * (targetPos.DirectionTo(GlobalPosition).X - targetPos.Normalized().Y), GetMaxAccel() * (float)delta);
                //velocity.Y = Mathf.MoveToward(velocity.Y, swoopDirection.X * GetMaxSpeed() * (Mathf.Pow(swoopPos.DirectionTo(GlobalPosition).X, 2) - swoopPos.Normalized().Y), GetMaxAccel() * (float)delta);
                //velocity.Y = Mathf.MoveToward(velocity.Y, swoopDirection.X * 0.25f * GetMaxSpeed() * (Mathf.Sin(4f* counter * (float)delta)), GetMaxAccel() * (float)delta);
                counter++;
                break;
            case Bat.Behavior.CVANIA:
                velocity.X = Mathf.MoveToward(velocity.X, targetDirection.X * GetMaxSpeed(), GetMaxAccel() * (float)delta);
                velocity.Y = Mathf.MoveToward(velocity.Y, 2.4f * swoopDirection.X * GetMaxSpeed() * (targetPos.DirectionTo(GlobalPosition).X - targetPos.Normalized().Y), GetMaxAccel() * (float)delta);
                if (GlobalPosition.Y >= targetPos.Y) {
                    velocity.X = Mathf.MoveToward(velocity.X, 0.25f * targetDirection.X * GetMaxSpeed(), GetMaxAccel() * (float)delta);
                    velocity.Y = 0;
                }

                break;

            case Bat.Behavior.ROCKET:
                InitTween();
                GD.Print(GlobalPosition);
                break;
            default:
                break;
        }

        MoveX(velocity.X * (float)delta, new Callable(this, nameof(OnCollisionX)));
        MoveY(velocity.Y * (float)delta, new Callable(this, nameof(OnCollisionY)));
    }

    public bool CheckProximity(Node2D actor) {
        if (GlobalPosition.DistanceTo(actor.GlobalPosition) < 200 && target == null) {
            // do stuff when the actor gets in range
            if (actor is Player) {
                target = actor;
                targetPos = target.GlobalPosition;
                targetDirection = GlobalPosition.DirectionTo(targetPos);
                swoopDirection = targetPos.DirectionTo(GlobalPosition);
                return true;
            }
        }

        return false;
    }

    public Behavior GetBehavior() {
        return behavior;
    }

    public Node2D GetTarget() {
        return target;
    }

    public void InitTween() {
        // Ensure we create only one tween and not reset it constantly
        if (tween != null) { return; }

        // The missile will travel back first, a little "wind-up"
        // Wait, what if we got rid of the bat all together and made it a projectile instead?
        // That way we can mess with the tween types and have a crap ton of different projectiles.
        tween = CreateTween().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
        // We want the velocity to change since we are on our own physics engine.
        tween.TweenProperty(this, "velocity", targetDirection * GetMaxSpeed(), time).SetDelay(delay);

        // This works, but we wouldn't be moving by integers as defined by our physics engine.
        //tween.TweenProperty(this, "global_position", targetPos, time).SetDelay(delay);
    }

    public Tween GetTween() {
        return tween;
    }

    public Vector2 GetTargetPosSnapshot() {
        return targetDirection;
    }

    public void SetBehavior(Behavior behavior) {
        this.behavior = behavior;
    }

    public Vector2 GetStartingPos() {
        return startingPos;
    }
}
