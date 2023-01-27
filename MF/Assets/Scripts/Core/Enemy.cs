using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public abstract class  Enemy : KinematicBody2D
{
    [Signal]
    public delegate void Hit(int damage);

    private int health;
    
    private Vector2 velocity;
    private Node2D target;
    private EnemyBehavior behavior;
    private const float PATROL_DISTANCE = 500.0f;
    private const float AGGRO_DISTANCE = 750.0f;
    private MeshInstance2D mesh;
    private Vector2 lastPos;
    private Vector2 spawnLoc;

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
    }

    public override void _PhysicsProcess(float delta) {
        // Before implementing context-based steering behavior
        //lastPos = Position;
        //velocity = Vector2.Zero;
        CheckBehavior();
        //velocity = CheckDistance(delta);
        //MoveAndSlide(velocity);

        // Still not quite working... 1/08/2023
        // Context-based steering behavior: https://kidscancode.org/godot_recipes/3.x/ai/context_map/
        SetInterest();
        SetDanger();
        ChooseDirection();
        //EmitSignal("draw");
        var desiredVelocity = chosenDir.Rotated(Rotation) * moveSpeed;
        velocity = velocity.LinearInterpolate(desiredVelocity, steerForce);
        //velocity = desiredVelocity;
        Rotation = velocity.Angle();
        MoveAndSlide(velocity);
        Update();
    }


    public override void _Process(float delta) {
        // This fixes the jitter.  It gets a fraction  through the current physics tick
        // and uses that fraction as the weight to linearly interpolate the transform of the object (in this case the enemy)
        // from the previous position
        //var fraction = Engine.GetPhysicsInterpolationFraction();
        //var transform = mesh.Transform;
        //transform.origin = lastPos.LinearInterpolate(GlobalTransform.origin, fraction);
    }
    public override void _Draw() {
        for (int i = 0; i < numRays; i++) {
            DrawLine(Vector2.Zero, (rayDirections[i] * 10f * velocity * interest[i]).Rotated(-Rotation), Color.ColorN("blue"), 2f);
        }
        //DrawLine(Position, chosenDir.Rotated(Rotation) * moveSpeed, Color.ColorN("red"), 2.0f);
        //DrawLine(new Vector2(), (new Vector2((Position * velocity.Normalized() * moveSpeed) - Position)).Rotated(-Rotation), Color.ColorN("blue"), 5.0f);
    }

    public void Spawn(Node2D target) {
        this.target = target;
        health = 100;
        moveSpeed = 20;
        velocity = Vector2.Zero;
        behavior = EnemyBehavior.IDLE;
        mesh = GetNode<MeshInstance2D>("MeshInstance2D");
        steerForce = 0.1f;
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
        var spaceState = GetWorld2d().DirectSpaceState; // This returns the current and potential collisions in the World that this object is in
        for (int i = 0; i < numRays; i++) {
            var result = spaceState.IntersectRay(Position,
                Position + rayDirections[i].Rotated(Rotation) * lookAhead, new Godot.Collections.Array(this));
            if (result.Count != 0) {
                Vector2 d = (Vector2) result["position"];
                interest[i] = Mathf.Max(0, d.Normalized().Length());
            }
        }
    }

    // Default interest for moving around
    private void SetDefaultInterest() {
        for (int i = 0; i < numRays; i++) {
            var d = rayDirections[i].Rotated(Rotation).Dot(Transform.x);
            //var d = CheckDistance().Dot(Transform.x);
            interest[i] = Mathf.Max(0, d);
        }
    }

    private void SetDanger() {
        // Cast rays to find danger directions
        var spaceState = GetWorld2d().DirectSpaceState; // This returns the current and potential collisions in the World that this object is in
        for (int i = 0; i < numRays; i++) {
            var result = spaceState.IntersectRay(Position,
                Position + rayDirections[i].Rotated(Rotation) * lookAhead, new Godot.Collections.Array(this, target));
            danger[i] = result.Count != 0 ? 1.0f : 0.0f;
        }
    }

    private void ChooseDirection() {
        // Don't choose to go into the way of danger
        for (int i = 0; i < numRays; i++) {
            if (danger[i] > 0.0f) {
                interest[i] = 0.0f;
            }
        }
        // Choose direction based on remainin interest
        chosenDir = Vector2.Zero;
        for (int i = 0; i < numRays; i++) {
            // This will place a weight on each of the ray directions
            chosenDir += rayDirections[i] * interest[i];
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
        }
        else if (distanceToPlayer > PATROL_DISTANCE && distanceToPlayer <= AGGRO_DISTANCE) {
            behavior = EnemyBehavior.MOVE_TOWARDS;
        }
        else {
            behavior = EnemyBehavior.IDLE;
        }
    }

    // This is good for now, but look into this: https://www.youtube.com/watch?v=6BrZryMz-ac&t=346s
    // This pathfinding could make for some really cool immersive movement from the enemy.
    private Vector2 CheckDistance() {
        switch (behavior) {
            case EnemyBehavior.IDLE:
                return Vector2.Zero;
            case EnemyBehavior.PATROL:
                //return Position.DirectionTo(target.Position) * moveSpeed;
                return new Vector2((Position - target.Position).Normalized().Rotated(Mathf.Pi / 2) * moveSpeed);
            case EnemyBehavior.MOVE_TOWARDS:
                return Position.DirectionTo(target.Position) * moveSpeed;
        }
        return Vector2.Zero;
    }

    private void TakeDamage(int damage) {
        health -= damage;
        //GD.Print("Oh shit I'm dying");
        if (health <= 0) {
            QueueFree();
        }
    }
}
