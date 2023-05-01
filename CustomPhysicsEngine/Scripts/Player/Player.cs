using Godot;
using System;

public partial class Player : Actor {
    // Constants
    private const int NUM_JUMPS = 1;
    private const int NUM_HEAD_RAYS = 4;

    // Static variables
    private static float COYOTE_TIME = 3f * Game.ONE_FRAME;
    private static float JUMP_BUFFER_TIME = 6f * Game.ONE_FRAME;
    private static float JUMP_HOLD_TIME = 12f * Game.ONE_FRAME;
    public static float ATTACK_INPUT_BUFFER = 12f * Game.ONE_FRAME;

    // private variables
    private Vector2 velocity = Vector2.Zero;
    private float maxSpeed = 100;
    private float maxAccel = 800;
    //private AnimatedSprite2D animatedSprite;
    //private float gravity = 1000;
    //private float maxFall = 160;
    //private float jumpForce = -160;

    private float localHoldTime = 0;
    private bool onGround = true;
    [Export]
    private float jumpHeight = 10f;
    [Export]
    private float jumpTimeToPeak = 0.4f;
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
    private Vector2[] headRays;

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
    public PlayerAttackState_1 playerAttackState_1;
    public PlayerAttackState_2 playerAttackState_2;
    public PlayerAttackState_3 playerAttackState_3;

    public override void _Ready() {
        // No onready, so get the node in this part of the pipeline.
        AnimatedSprite = GetNode<AnimatedSprite2D>("Animations");
        Hitbox = (Hitbox)GetNode<Node2D>("Hitbox");
        attackTimer = GetNode<Timer>("AttackTimeout");

        // Since C# does not have onready, we still need to fetch the globals.
        // The documentation said we should be able to just call Game, but it did not work.
        // It is because GetTree is not a static method, it seems like.  I wonder if I am doing something incorrectly.
        GM = GetNode<Game>("/root/Game");
        AddToGroup("Actors");

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
        playerAttackState_1 = new PlayerAttackState_1(GetNode<Node2D>("Attacks/Attack1"));
        playerAttackState_2 = new PlayerAttackState_2(GetNode<Node2D>("Attacks/Attack2"));
        playerAttackState_3 = new PlayerAttackState_3(GetNode<Node2D>("Attacks/Attack3"));
        currentState = playerIdleState;
        facing = Facing.RIGHT;
        wasGrounded = true;
        inputBuffer = new InputBuffer(8);
        headRays = new Vector2[NUM_HEAD_RAYS];
        //InitHeadRays();
    }
    public override void _Draw() {
        for (int i = 0; i < NUM_HEAD_RAYS; i++) {
            DrawLine(headRays[i], headRays[i] * new Vector2(1, 5f * 1), Colors.Red);
        }
    }
    public override void _Process(double delta) {
        //var direction = Math.Sign(Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"));
        //if (direction > 0) {
        //    facing = Facing.RIGHT;
        //} else if (direction < 0) {
        //    facing = Facing.LEFT;
        //}
        //var direction = GetDirectionInput();
        onGround = GM.CheckWallsCollision(this, Vector2.Down);
        //DoMovement(delta, direction);

        //var onGround = GlobalPosition.Y >= 160;

        //var jumping = Input.IsActionJustPressed("Jump");

        //if (jumping && onGround) {
        //	velocity = new Vector2(velocity.X, jumpForce);
        //	localHoldTime = jumpHoldTime;
        //} else if (localHoldTime > 0) {
        //	if (Input.IsActionPressed("Jump")) {
        //		velocity = new Vector2(velocity.X, jumpForce);
        //	} else {
        //		localHoldTime = 0;
        //	}
        //}
        //localHoldTime -= (float)delta;

        //Jump(delta);


        ////if (direction > 0) { animatedSprite.FlipH = false; } else if (direction < 0) { animatedSprite.FlipH = true; }
        ////if (direction != 0) { animatedSprite.Play("Run"); } else { animatedSprite.Play("Idle"); }



        //velocity.X = Mathf.MoveToward(velocity.X, maxSpeed * direction, maxAccel * (float)delta);
        ////velocity.Y = Mathf.MoveToward(velocity.Y, maxFall, gravity * (float)delta); // Before implementing Building a Better Jump GDC talk
        //velocity.Y = Mathf.MoveToward(velocity.Y, GetGravity(), GetGravity() * (float)delta); // == velocity.Y += GetGravity() * (float)delta;
        ////velocity.X = maxSpeed * direction;

        //// In Actor, if a collision is detected, then perform the callback function
        //MoveX(velocity.X * (float)delta, new Callable(this, nameof(OnCollisionX)));
        //MoveY(velocity.Y * (float)delta, new Callable(this, nameof(OnCollisionY)));
        currentState = currentState.EnterState(this);
        //GD.Print(inputBuffer.GetBuffer());
        //GD.Print(Hitbox.Left);
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta) {
        var spaceState = GetWorld2D().DirectSpaceState;

        for (int i = 0; i < NUM_HEAD_RAYS; i++) { 
        }
    }

    public void OnCollisionX() {
        velocity.X = 0;
        ZeroRemainderX();
    }

    public void OnCollisionY() {
        velocity.Y = 0;
        ZeroRemainderY();
    }

    public int GetDirectionInput() {
        var directionVector = new Vector2(Math.Sign(Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left")), Math.Sign(Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")));
        var direction = Math.Sign(Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"));

        if (direction > 0 && facing == Facing.LEFT) {
            facing = Facing.RIGHT;
        }
        else if (direction < 0 && facing == Facing.RIGHT) {
            facing = Facing.LEFT;
        }

        switch (facing) {
            case Facing.RIGHT:
                Scale = new Vector2(1, 1);
                Hitbox.SetFlipped(Scale);
                break;
            case Facing.LEFT:
                Scale = new Vector2(-1, 1);
                Hitbox.SetFlipped(Scale);
                break;
        }

        inputBuffer.AddInput(directionVector);

        return direction;
    }

    public void DoMovement(double delta, int direction) {

        Jump(delta);

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
            //localHoldTime = JUMP_HOLD_TIME;
            //jumpBufferTime = JUMP_BUFFER_TIME;
            jumpBufferTime = 0;
            localHoldTime = JUMP_HOLD_TIME;
            jumping = true;
        }
        else if (localHoldTime > 0) {
            if (Input.IsActionPressed("Jump")) {
                velocity = new Vector2(velocity.X, jumpVelocity);
            }
            else {

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
            attackCounter--;
            switch (attackCounter) {
                case 2:
                    return playerAttackState_1;
                case 1:
                    return playerAttackState_2;
                case 0:
                    return playerAttackState_3;
            }
        }
        
        return currentState;
    }

    private void InitHeadRays() {
        var rayDist = Hitbox.GetWidth() / NUM_HEAD_RAYS;
        for (int i = 0; i < NUM_HEAD_RAYS; i++) {
            var rayStart = new Vector2(1 + (i * rayDist) + Hitbox.GetLocalLeft(), Hitbox.GetLocalTop());
            headRays[i] = rayStart;
            GD.Print(headRays[i]);
        }
    }
}
