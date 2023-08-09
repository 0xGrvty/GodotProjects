using Godot;
using System;

public partial class Bat : Enemy {
    private Vector2 velocity = Vector2.Zero;
    //private bool onGround = true;
    private Node2D target = null;
    private Vector2 targetDirection = Vector2.Zero;
    private bool attackPlayer = false;
    private Vector2 startingPos = Vector2.Zero;
    private Vector2 swoopPos = Vector2.Zero;
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
        CVANIA = 3
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

            case Bat.Behavior.STRAIGHT:
                velocity.X = Mathf.MoveToward(velocity.X, GetMaxSpeed() * GlobalPosition.DirectionTo(targetDirection).X, GetMaxAccel() * (float)delta);
                velocity.Y = Mathf.MoveToward(velocity.Y, GetMaxSpeed() * GlobalPosition.DirectionTo(targetDirection).Y, GetMaxAccel() * (float)delta);
                break;
            case Bat.Behavior.SWOOP:
                velocity.X = Mathf.MoveToward(velocity.X, targetDirection.X * GetMaxSpeed(), GetMaxAccel() * (float)delta);
                // y = mx + b
                //velocity.Y = Mathf.MoveToward(velocity.Y, swoopDirection.X * GetMaxSpeed() * swoopPos.DirectionTo(GlobalPosition).X, GetMaxAccel() * (float)delta); // <-- this is really close
                velocity.Y = Mathf.MoveToward(velocity.Y, swoopDirection.X * GetMaxSpeed() * (swoopPos.DirectionTo(GlobalPosition).X - swoopPos.Normalized().Y), GetMaxAccel() * (float)delta);
                
                
                break;
            case Bat.Behavior.CVANIA:
                velocity.X = Mathf.MoveToward(velocity.X, 0.75f * targetDirection.X * GetMaxSpeed(), GetMaxAccel() * (float)delta);
                velocity.Y = Mathf.MoveToward(velocity.Y, swoopDirection.X * GetMaxSpeed() * swoopPos.DirectionTo(GlobalPosition).X, GetMaxAccel() * (float)delta);
                if (GlobalPosition.Y == swoopPos.Y)
                {
                    velocity.X = Mathf.MoveToward(velocity.X, 0.25f * targetDirection.X * GetMaxSpeed(), GetMaxAccel() * (float)delta);
                    velocity.Y = 0;
                }

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
                swoopPos = target.GlobalPosition;
                targetDirection = GlobalPosition.DirectionTo(swoopPos);
                swoopDirection = swoopPos.DirectionTo(GlobalPosition);
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
        tween = CreateTween().SetLoops().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.InOut);
        //tween.Connect(Tween.SignalName.StepFinished, new Callable(this, nameof(OnTweenStep)));
        tween.TweenProperty(this, "follow", start + offset, time).SetDelay(delay);
        tween.TweenProperty(this, "follow", start, time).SetDelay(delay);
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
