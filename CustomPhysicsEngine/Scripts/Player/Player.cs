using Godot;
using System;

public partial class Player : Actor {
    // Signals
    [Signal]
    public delegate void LoadZoneTriggeredEventHandler(int facing);
    [Signal]
    public delegate void HitstopEventHandler();
    [Signal]
    public delegate void ShakeCameraEventHandler();
    [Signal]
    public delegate void MoveEventHandler();

    // Constants
    private const int NUM_JUMPS = 1;
    private const int NUM_HEAD_RAYS = 4;
    private const int CORNER_CORRECTION = 4;

    // Static variables
    private static float COYOTE_TIME = 3f * Game.ONE_FRAME;
    private static float JUMP_BUFFER_TIME = 6f * Game.ONE_FRAME;
    private static float JUMP_HOLD_TIME = 12f * Game.ONE_FRAME;
    public static float ATTACK_INPUT_BUFFER = 12f * Game.ONE_FRAME;

    // private variables
    private Vector2 velocity = Vector2.Zero;
    [Export]
    private float maxSpeed = 100;
    [Export]
    private float maxAccel = 800;

    private float localHoldTime = 0;
    private bool onGround = true;
    [Export]
    private float jumpHeight = 21f;
    [Export]
    private float jumpTimeToPeak = 0.3f;
    [Export]
    private float jumpTimeToDescent = 0.2f;
    private float jumpVelocity;
    private float jumpGravity;
    private float fallGravity;
    private Facing facing;
    private bool isJumping;
    private int attackCounter = 3;
    private Timer attackTimer;
    private int numJumps = NUM_JUMPS;
    private float coyoteTime = 0;
    private bool wasGrounded = true;
    private float jumpBufferTime = 0;
    private InputBuffer inputBuffer;
    private bool showComboList = false;
    private float attackInputBuffer = 0;
    private Vector2 snapshotVelocity;
    private int snapshotDirection;

    // public variables
    public int AttackCounter { get => attackCounter; set => attackCounter = value; }
    public float JumpBufferTime { get => jumpBufferTime; set => jumpBufferTime = value; }
    public float CoyoteTime { get => coyoteTime; set => coyoteTime = value; }
    public Facing Facing { get => facing; set => facing = value; }
    public Vector2 Velocity { get => velocity; set => velocity = value; }
    public bool IsJumping { get => isJumping; }
    public int NumJumps { get => numJumps; set => numJumps = value; }
    public bool WasGrounded { get => wasGrounded; set => wasGrounded = value; }
    public float AttackInputBuffer { get => attackInputBuffer; set => attackInputBuffer = value; }
    // State machine
    public IStateMachine currentState;
    public PlayerRunState playerRunState;
    public PlayerIdleState playerIdleState;
    public PlayerJumpState playerJumpState;
    public PlayerJumpSquatState playerJumpSquatState;
    public PlayerFallState playerFallState;
    public PlayerAttackState1 playerAttackState1;
    public PlayerAttackState2 playerAttackState2;
    public PlayerAttackState3 playerAttackState3;
    public PlayerSceneTransitionState playerSceneTransitionState;
    public PlayerChargeAttackState playerChargeAttackState;

    public override void _Ready() {
        // No onready, so get the node in this part of the pipeline.
        AnimatedSprite = GetNode<AnimatedSprite2D>("Animations");
        Hurtbox = (Hitbox)GetNode<Node2D>("Hurtbox");
        attackTimer = GetNode<Timer>("AttackTimeout");

        // Since C# does not have onready, we still need to fetch the globals.
        // The documentation said we should be able to just call Game, but it did not work.
        // It is because GetTree is not a static method, it seems like.  I wonder if I am doing something incorrectly.
        GM = GetNode<Game>("/root/Game");
        AddToGroup("Actors");
        AddToGroup("CameraShakers");

        // Building a Better Jump GDC talk
        jumpVelocity = ((2.0f * jumpHeight) / jumpTimeToPeak) * -1.0f; // In Godot 2D, down is positive, so flip the signs
        jumpGravity = ((-2.0f * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak)) * -1.0f;
        fallGravity = ((-2.0f * jumpHeight) / (jumpTimeToDescent * jumpTimeToDescent)) * -1.0f;

        // player state machine
        playerRunState = new PlayerRunState();
        playerIdleState = new PlayerIdleState();
        playerJumpState = new PlayerJumpState();
        playerJumpSquatState = new PlayerJumpSquatState();
        playerFallState = new PlayerFallState();
        playerAttackState1 = new PlayerAttackState1(GetNode<Node2D>("Attacks/Attack1"));
        playerAttackState2 = new PlayerAttackState2(GetNode<Node2D>("Attacks/Attack2"));
        playerAttackState3 = new PlayerAttackState3(GetNode<Node2D>("Attacks/Attack3"));
        playerSceneTransitionState = new PlayerSceneTransitionState();
        playerChargeAttackState = new PlayerChargeAttackState(GetNode<Node2D>("Attacks/ChargeAttack"));
        currentState = playerIdleState;
        facing = Facing.RIGHT;
        wasGrounded = true;
        inputBuffer = new InputBuffer(8);
        AddUserSignal(nameof(OnLoadZoneTriggered));
        LoadZoneTriggered += OnLoadZoneTriggered;
    }
    public override void _Draw() {
        DrawLine(Vector2.Zero, 15f * Vector2.Down, Colors.Yellow);
        DrawLine(Vector2.Zero, 15f * Vector2.Up, Colors.Yellow);
        DrawLine(Vector2.Zero, 15f * Vector2.Right, Colors.Green);
        DrawLine(Vector2.Zero, 15f * Vector2.Left, Colors.Green);
    }
    public override void _Process(double delta) {
        onGround = GM.CheckWallsCollision(this, Vector2.Down);
        currentState = currentState.EnterState(this);
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta) {
        // The ray-casting method actually doesn't work because in Godot, Raycast2D only interacts with _CollisionObject2D_
        // and since we are using our own physics engine, we are only using Nodes, which are not CollisionObject2Ds.  Now we must figure out how to make our physics engine
        // do what we want, which is to detect if we bonk our head, and if we can move them to the side so they can clear a corner
        // Here's a way to check, but that's O(N^2) time: https://www.youtube.com/watch?v=tW-Nxbxg5qs
        // But maybe we can simply make Hitboxes and check if there's a collision instead.  I was originally thinking this, but was not sure if 
        // this was the best approach.  We are really decoupled from the Godot Physics API, so instead of forcing using it, we should try letting go of it as much as possible.

        // Something I've realized:  Even though the solution above is O(N^2), gotta realize that we set the bounds of what we loop through in that particular instance.
        // It is technically O(N^2), however we only check a hard-coded set of 3 pixels.  Now what if we had an upgrade that bumped corner correction (for some reason)
        // an infinite amount of times?  Then yes, the above solution isn't optimal.  But we know that we want to check roughly 3-4 pixels only at a time whenever we jump.
        // This limit makes the time negligible. I shouldn't make things harder on myself, just do it.  Remember the rule of thumb: Do not optimize unless you have to.

        //if (velocity.Y < 0)
        //{
        //    var spaceState = GetWorld2D().DirectSpaceState;
        //    var rayDist = Math.Abs(Hitbox.GetWidth()) / NUM_HEAD_RAYS;
        //    for (int i = 0; i < NUM_HEAD_RAYS; i++)
        //    {
        //        var rayStart = new Vector2((i * rayDist) + Hitbox.GetLeft(), Hitbox.GetTop());
        //        var query = PhysicsRayQueryParameters2D.Create(rayStart, new Vector2(rayStart.X, 15f * -1));
        //        //query.Exclude = new Godot.Collections.Array<Rid> { CollisionObject2D.GetRid() };
        //        var result = spaceState.IntersectRay(query);
        //        if (result.Count > 0)
        //        {
        //            GD.Print("We hit this: " + result["collider_id"]);
        //        }
        //    }
        //}
    }

    public void OnCollisionX() {
        velocity.X = 0;
        ZeroRemainderX();
    }

    public void OnCollisionY() {
        // See note in _PhysicsProcess.  You know what?  Maybe the best solution is the most straight forward.  We only need to check a few pixels to the left and right of our hitbox
        // Why go through all that work making rays, checking if the corner is in between the rays, moving the character, and etc?  Just check 2-3 pixels to the left, then check 2-3 pixels to the right.
        // This is why the Celeste devs are smart and I am not.

        // First do a fuzzy check to see if the player hits a corner
        if (velocity.Y < 0) {
            if (velocity.X >= 0) {
                for (int i = 1; i <= CORNER_CORRECTION; i++) {
                    // If the fuzzy check returns false, then move them and let them jump!
                    if (!GM.CheckWallsCollision(this, new Vector2(i, -1))) {
                        GlobalPosition += new Vector2(i, -1);
                        // return since we don't want to zero the remainder when we correct the jump
                        return;
                    }
                    if (!GM.CheckWallsCollision(this, new Vector2(-i, -1))) {
                        GlobalPosition += new Vector2(-i, -1);
                        // return since we don't want to zero the remainder when we correct the jump
                        return;
                    }
                }
            }
            if (velocity.X <= 0) {
                for (int i = 1; i <= CORNER_CORRECTION; i++) {
                    if (!GM.CheckWallsCollision(this, new Vector2(i, -1))) {
                        GlobalPosition += new Vector2(i, -1);
                        return;
                    }
                    if (!GM.CheckWallsCollision(this, new Vector2(-i, -1))) {
                        GlobalPosition += new Vector2(-i, -1);
                        return;
                    }
                }
            }

        }
        velocity.Y = 0;
        ZeroRemainderY();
    }

    public int GetDirectionInput() {
        var directionVector = new Vector2(Math.Sign(Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left")), Math.Sign(Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")));
        var direction = Math.Sign(Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"));

        if (direction > 0 && facing == Facing.LEFT) {
            facing = Facing.RIGHT;
            AnimatedSprite.FlipH = false;
        } else if (direction < 0 && facing == Facing.RIGHT) {
            facing = Facing.LEFT;
            AnimatedSprite.FlipH = true;
        }

        inputBuffer.AddInput(directionVector);

        return direction;
    }

    public void DoMovement(double delta, int direction, bool isSceneTransition = false) {

        Jump(delta);

        velocity.X = Mathf.MoveToward(velocity.X, maxSpeed * direction, maxAccel * (float)delta);
        velocity.Y = Mathf.MoveToward(velocity.Y, GetGravity(), GetGravity() * (float)delta);

        if (isSceneTransition && onGround) {
            velocity.X = maxSpeed * snapshotDirection;
        } else if (isSceneTransition && !onGround) {
            velocity.X = 0;
        }

        MoveX(velocity.X * (float)delta, new Callable(this, nameof(OnCollisionX)));
        MoveY(velocity.Y * (float)delta, new Callable(this, nameof(OnCollisionY)));
        if (velocity != Vector2.Zero) EmitSignal(SignalName.Move);
    }

    public void ForceMove(double delta, int direction) {
        velocity.X = Mathf.MoveToward(velocity.X, maxSpeed * direction, maxAccel * (float)delta);
        velocity.Y = Mathf.MoveToward(velocity.Y, GetGravity(), GetGravity() * (float)delta);
        MoveX(velocity.X * (float)delta, new Callable(this, nameof(OnCollisionX)));
        MoveY(velocity.Y * (float)delta, new Callable(this, nameof(OnCollisionY)));
    }

    public void Jump(double delta) {
        var jumpPressed = Input.IsActionJustPressed("Jump");
        var jumping = false;

        // If the player presses the jump button JUMP_BUFFER_TIME frames before hitting the ground,
        // allow them to jump
        jumpBufferTime -= (float)delta;
        if (jumpPressed) {
            jumpBufferTime = JUMP_BUFFER_TIME;
        }

        // Check if the player was allowed to jump, then check if the player is on the ground or was on the ground (coyote time)
        // or check if the player was allowed to jump and if they have enough jumps (double jump, mid-air jump)
        if ((jumpBufferTime > 0.0f && (onGround || wasGrounded)) || (jumpBufferTime > 0.0f && numJumps > 0)) {
            numJumps--;
            velocity = new Vector2(velocity.X, jumpVelocity);
            jumpBufferTime = 0;
            localHoldTime = JUMP_HOLD_TIME;
            jumping = true;
        } else if (localHoldTime > 0) {
            if (Input.IsActionPressed("Jump")) {
                velocity = new Vector2(velocity.X, jumpVelocity);
            } else {

                // Give the player a little nudge so that if they let go too early they can still make their jump
                //velocity = new Vector2(velocity.X, 0.5f * velocity.Y);
                localHoldTime = 0;
            }
            localHoldTime -= (float)delta;
        }

        isJumping = jumping;
    }

    // return jumpGravity if player is jumping, otherwise return fallGravity
    public float GetGravity() {
        return velocity.Y < 0.0 ? jumpGravity : fallGravity;
    }

    public bool IsGrounded() {
        return onGround;
    }

    public void ResetAttackCounter() {
        //attackTimer.Stop();
        attackInputBuffer = 0;
        attackCounter = 3;
    }

    public Timer GetAttackTimer() {
        return attackTimer;
    }

    public void OnAttackTimeout() {
        attackCounter = 3;
    }

    public void ResetGroundedStats() {
        coyoteTime = COYOTE_TIME;
        numJumps = NUM_JUMPS;
        jumpBufferTime = 0;
    }

    // Redo this function.  It is messy and doesn't make sense.
    public IStateMachine DoAttack() {
        var attackPressed = Input.IsActionJustPressed("Attack");
        
        if (attackInputBuffer < 0.0) {
            ResetAttackCounter();
        }

        attackInputBuffer -= (float)GetProcessDeltaTime();

        if (attackPressed) {
            attackInputBuffer = ATTACK_INPUT_BUFFER;
        }

        if (attackInputBuffer > 0) {
            velocity = Vector2.Zero;
            attackCounter--;
            switch (attackCounter) {
                case 2:
                    return playerAttackState1;
                case 1:
                    return playerAttackState2;
                case 0:
                    return playerAttackState3;
            }
        }

        return currentState;
    }

    public void OnLoadZoneTriggered(int doorDirection) {
        SnapshotMovement(doorDirection);
        currentState = playerSceneTransitionState;
    }

    public void SnapshotMovement(int doorDirection) {
        snapshotDirection = doorDirection;
        snapshotVelocity = velocity;
    }

    public int GetSnapshotDirection() {
        return snapshotDirection;
    }

    public Vector2 GetSnapshotVelocity() {
        return snapshotVelocity;
    }

    public float GetMaxSpeed() {
        return maxSpeed;
    }

    public float GetMaxAccel() {
        return maxAccel;
    }
}
