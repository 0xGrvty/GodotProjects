using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public abstract class Enemy : KinematicBody2D {
#pragma warning disable 649
    [Export]
    public PackedScene enemyExplosionScene;
    [Export]
    private AnimatedSprite animations;
#pragma warning restore 649
    [Signal]
    public delegate void Hit(int damage, Node2D source, Dictionary upgrades = null);
    [Signal]
    public delegate void Died();

    private int health;

    private Vector2 velocity;
    private Node2D target;
    private EnemyBehavior behavior;
    private const float PATROL_DISTANCE = 150.0f;
    private const float AGGRO_DISTANCE = 250.0f;
    private Vector2 lastPos;
    private Vector2 spawnLoc;
    private FaceDir facing;

    // Context-based Steering
    private float moveSpeed;
    private float steerForce;
    private float lookAhead;
    private int numRays;

    private Vector2[] rayDirections;
    private float[] interest;
    private float[] danger;

    private Vector2 chosenDir;
    private Vector2 acceleration;

    private IStateMachine currentState;
    public EnemyRunState enemyRunState = new EnemyRunState();
    public EnemyDeathState enemyDeathState = new EnemyDeathState();
    public EnemyIdleState enemyIdleState = new EnemyIdleState();

    private enum EnemyBehavior {
        IDLE = 0,
        MOVE_TOWARDS = 1,
        PATROL = 2
    }

    public override void _Ready() {
        interest = new float[numRays];
        danger = new float[numRays];
        rayDirections = new Vector2[numRays];
        for (int i = 0; i < numRays; i++) {
            var angle = i * 2 * Mathf.Pi / numRays;
            rayDirections[i] = Vector2.Right.Rotated(angle);
        }
        Connect(nameof(Hit), this, nameof(TakeDamage));
        animations = GetNode<AnimatedSprite>("AnimatedSprite");
        facing = FaceDir.RIGHT;
        currentState = enemyIdleState;
        //steerForce = 200f;
    }

    public override void _PhysicsProcess(float delta) {
        //CheckDistance();
        //DoMovement();
        currentState = currentState.EnterState(this);
        Update();
    }


    public void DoMovement() {
        ChooseDirection();
        //EmitSignal("draw");
        var desiredVelocity = chosenDir.Rotated(Rotation) * moveSpeed;
        velocity = velocity.LinearInterpolate(desiredVelocity, steerForce);
        velocity = desiredVelocity;
        //Rotation = velocity.Angle();

        if (Mathf.Sign(velocity.y) < 0) {
            facing = FaceDir.UP;
        }
        if (Mathf.Sign(velocity.y) > 0) {
            facing = FaceDir.DOWN;
        }
        if (Mathf.Sign(velocity.x) > 0) {
            facing = FaceDir.RIGHT;
            if (Mathf.Sign(velocity.y) > 0) { facing = FaceDir.DOWN_RIGHT; } else if (Mathf.Sign(velocity.y) < 0) { facing = FaceDir.UP_RIGHT; }
        }
        if (Mathf.Sign(velocity.x) < 0) {
            facing = FaceDir.LEFT;
            if (Mathf.Sign(velocity.y) > 0) { facing = FaceDir.DOWN_LEFT; } else if (Mathf.Sign(velocity.y) < 0) { facing = FaceDir.UP_LEFT; }
        }

        MoveAndSlide(velocity);
    }

    public override void _Draw() {
        for (int i = 0; i < numRays; i++) {
            DrawLine(Vector2.Zero, (rayDirections[i] * velocity * interest[i]).Rotated(-Rotation), Color.ColorN("blue"), 2f);
        }
        DrawLine(Vector2.Zero, chosenDir.Rotated(Rotation) * moveSpeed, Color.ColorN("red"), 2.0f);
        //DrawLine(new Vector2(), (new Vector2((Position * velocity.Normalized() * moveSpeed) - Position)).Rotated(-Rotation), Color.ColorN("blue"), 5.0f);
    }

    public void Spawn(Node2D target) {
        this.target = target;
        health = 100;
        moveSpeed = 50;
        velocity = Vector2.Zero;
        behavior = EnemyBehavior.IDLE;
        steerForce = 0.5f;
        lookAhead = PATROL_DISTANCE;
        numRays = 8;
        spawnLoc = Position;
        lastPos = spawnLoc;
    }

    private void SetInterest() {
        //// Set the interest in each slot based on world direction
        //if (Owner != null && Owner.HasMethod("GetPathDirection")) {
        //    var pathDirection = Vector2.Zero;
        //    Owner.CallDeferred("GetPathDirection", new Godot.Collections.Array(pathDirection));
        //    for (int i = 0; i < numRays; i++) {
        //        var d = rayDirections[i].Rotated(Rotation).Dot(pathDirection);
        //        interest[i] = Mathf.Max(0, d);
        //    }
        //}
        //// If there is no world path or owner, use default interest
        //else {
        //    SetDefaultInterest();
        //}
        for (int i = 0; i < numRays; i++) {
            var d = rayDirections[i].Rotated(Rotation).Dot(ToLocal(target.Position).Normalized());
            interest[i] = Mathf.Max(0, d);
        }
    }

    // Default interest for moving around
    private void SetDefaultInterest() {
        for (int i = 0; i < numRays; i++) {
            interest[i] = 0;
        }
    }

    private void SetDanger() {
        // Cast rays to find danger directions
        var spaceState = GetWorld2d().DirectSpaceState; // This returns the current and potential collisions in the World that this object is in
        for (int i = 0; i < numRays; i++) {
            var result = spaceState.IntersectRay(Vector2.Zero,
                Vector2.Zero + rayDirections[i].Rotated(Rotation) * lookAhead, new Godot.Collections.Array( this, target), this.CollisionLayer);
            //danger[i] = result.Count != 0 ? 1.0f : 0.0f;
            if (result.Count > 0) {
                var d = 1.25f * rayDirections[i].Rotated(Rotation).Dot(ToLocal((Vector2)result["position"]).Normalized());
               // var d = 2.5f;
                //GD.Print(result["collider"]);
                danger[i] = d;
            }
        }
    }

    private void ChooseDirection() {
        // Don't choose to go into the way of danger
        //for (int i = 0; i < numRays; i++) {
        //    if (danger[i] > 0.0f) {
        //        interest[i] = 0.0f;
        //    }
        //}
        // Choose direction based on remainin interest
        chosenDir = Vector2.Zero;
        for (int i = 0; i < numRays; i++) {
            // This will place a weight on each of the ray directions
            chosenDir += rayDirections[i] * (interest[i] - danger[i]);
        }
        chosenDir = chosenDir.Normalized();
    }

    private void CheckBehavior() {
        // Eventually change this and add a circle detection in the inspector
        // All it does right now is check if the player is in range.  Just needed to test
        // simple pathfinding for now.
        var distanceToPlayer = Position.DistanceTo(target.Position);
        if (distanceToPlayer <= PATROL_DISTANCE) {
            behavior = EnemyBehavior.PATROL;
        } else if (distanceToPlayer > PATROL_DISTANCE && distanceToPlayer <= AGGRO_DISTANCE) {
            behavior = EnemyBehavior.MOVE_TOWARDS;
        } else {
            behavior = EnemyBehavior.IDLE;
        }
    }

    //// This is good for now, but look into this: https://www.youtube.com/watch?v=6BrZryMz-ac&t=346s
    //// This pathfinding could make for some really cool immersive movement from the enemy.
    //private Vector2 CheckDistance() {
    //    switch (behavior) {
    //        case EnemyBehavior.IDLE:
    //            return Vector2.Zero;
    //        case EnemyBehavior.PATROL:
    //            //return Position.DirectionTo(target.Position) * moveSpeed;
    //            return new Vector2((Position - target.Position).Normalized().Rotated(Mathf.Pi / 2) * moveSpeed);
    //        case EnemyBehavior.MOVE_TOWARDS:
    //            return Position.DirectionTo(target.Position) * moveSpeed;
    //    }
    //    return Vector2.Zero;
    //}

    public void CheckDistance() {
        // Before implementing context-based steering behavior
        //lastPos = Position;
        //velocity = Vector2.Zero;
        //CheckBehavior();
        //velocity = CheckDistance(delta);
        //MoveAndSlide(velocity);

        // Still not quite working... 1/08/2023
        // Context-based steering behavior: https://kidscancode.org/godot_recipes/3.x/ai/context_map/
        if (IsInstanceValid(target)) {
            if (Position.DistanceTo(target.Position) <= AGGRO_DISTANCE) {
                SetInterest();
            } else {
                SetDefaultInterest();
            }
            SetDanger();
        }

    }

    private void TakeDamage(int damage, Node2D source, Dictionary hammerUpgrades) {
        health -= damage;
        var effects = hammerUpgrades.Duplicate(true);
        if (health <= 0) {
            //currentState = enemyDeathState;
            //if ((int)effects?["explodeOnKill"] >= 1) {
            //    var e = (EnemyExplode)enemyExplosionScene.Instance();
            //    e.Init(this, effects);
            //    GetParent().CallDeferred("add_child", e);
            //}
            //if (source?.GetType() == typeof(PlayerBody)) {
            //    // add to kill counter if the source was the player
            //    EmitSignal(nameof(Died));
            //}
            //QueueFree();
        }
    }

    public int GetHealth() {
        return health;
    }

    public Vector2 GetVelocity() {
        return velocity;
    }

    public AnimatedSprite GetAnimatedSprite() {
        return animations;
    }

    public FaceDir GetFacing() {
        return facing;
    }
}
