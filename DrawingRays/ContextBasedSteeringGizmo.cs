using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class ContextBasedSteeringGizmo : Node2D {
    [Export]
    public PackedScene cbsn;

    private float steerForce = 0.1f;
    private float lookAhead = 250f;
    private int numRays = 32;
    private Vector2[] rayDirections;
    private float[] interest;
    private float[] danger;
    private Vector2 chosenDir;
    private float radius = 200f;
    private int points = 32;
    private bool mouseInterest = false;
    private bool playerInterest = true;
    private Vector2 startPos;
    private Vector2 velocity;
    private float moveSpeed = 200f;
    private List<ContextBasedSteeringNode> nodeList;
    private Dictionary<int, ContextBasedSteeringNode> dangerList;
    private float aggression = 1.25f; // The weight of the interest or danger
    private float passivity = 0.85f; // Used to multiply the aggression in the direction of danger (higher is more unlikely to travel that direction)

    private Player target;

    public override void _Ready() {
        interest = new float[numRays];
        danger = new float[numRays];
        rayDirections = new Vector2[numRays];
        for (int i = 0; i < numRays; i++) {
            var angle = i * 2 * Mathf.Pi / numRays;
            rayDirections[i] = Vector2.Right.Rotated(angle);
        }
        startPos = GetViewportRect().GetCenter();
        Position = startPos;
        velocity = new Vector2(1, 1) * moveSpeed;
        nodeList = new List<ContextBasedSteeringNode>();
        dangerList = new Dictionary<int, ContextBasedSteeringNode>();
        target = GetNode<Player>("../Player");
    }

    public override void _Process(float delta) {

        if (Input.IsActionJustPressed("DebugLogs")) {
            for (int i = 0; i < numRays; i++) {
                GD.Print(interest[i]);
            }
        }
        if (Input.IsActionJustPressed("SetInterestNode")) {
            var c = (ContextBasedSteeringNode)cbsn.Instance();
            c.Init(ContextBasedSteeringNode.NodeType.INTEREST, GetGlobalMousePosition(), Colors.Green);
            nodeList.Add(c);
            GetParent().AddChild(c);
        }
        if (Input.IsActionJustPressed("SetDangerNode")) {
            var c = (ContextBasedSteeringNode)cbsn.Instance();
            c.Init(ContextBasedSteeringNode.NodeType.DANGER, GetGlobalMousePosition(), Colors.Red);
            nodeList.Add(c);
            GetParent().AddChild(c);
        }

        SetInterest();
        SetDanger();
        ChooseDirection();
        Update();

    }

    public override void _Draw() {
        DrawArc(Vector2.Zero, radius, 0f, 2 * Mathf.Pi, points, Colors.White);
        for (int i = 0; i < numRays; i++) {
            DrawLine(Vector2.Zero, rayDirections[i] * interest[i] * radius, Colors.Green, 2.0f);
            DrawLine(Vector2.Zero, rayDirections[i] * danger[i] * radius, Colors.Red, 2.0f);
        }
        DrawLine(Vector2.Zero, chosenDir * moveSpeed, Colors.Yellow, 2f);
    }

    private void SetInterest() {
        //// Set the interest in each slot based on world direction
        if (mouseInterest) {
            for (int i = 0; i < numRays; i++) {
                var d = 0.0f;
                for (int j = 0; j < nodeList.Count; j++) {
                    if (nodeList[j].GetNodeType() == ContextBasedSteeringNode.NodeType.INTEREST) {
                        d += rayDirections[i].Rotated(Rotation).Dot(nodeList[j].Position.Normalized());
                    }
                }
                interest[i] = Mathf.Max(0, d);
            }
        }
        if (playerInterest) {
            for (int i = 0; i < numRays; i++) {
                var d = rayDirections[i].Rotated(Rotation).Dot(ToLocal(target.Position).Normalized());
                interest[i] = Mathf.Max(0, d);
            }
        }
        // If there is no interest set, use default interest
        else {
            SetDefaultInterest();
        }

    }

    // Default interest for moving around
    private void SetDefaultInterest() {
        for (int i = 0; i < numRays; i++) {
            var d = rayDirections[i].Rotated(Rotation).Dot(Transform.x);
            interest[i] = Mathf.Max(0, d);
        }
    }

    private void SetDanger() {
        // Cast rays to find danger directions
        var spaceState = GetWorld2d().DirectSpaceState; // This returns the current and potential collisions in World Space that this object is in
        for (int i = 0; i < numRays; i++) {
            // This will draw a ray in World Space starting from the Position of the object (Gizmo in this case) to the distance of lookAhead in the direction of rayDirection[i], excluding the target (because we want to be interested in the target)
            // We can change the array to exclude other objects (such as itself, if the Gizmo had a collision of its own)
            // The enemy that uses this context-based steering would also need to see the things that it's interested in so that it can ignore those objects.
            // i.e. Monster likes food, so it will not see it as danger.  But how to make it see food layer?  Maybe make a function that passes the collision layers it will need to see?
            var result = spaceState.IntersectRay(Position,
                Position + rayDirections[i].Rotated(Rotation) * lookAhead, new Godot.Collections.Array { target }); 
            if (result.Count != 0) {
                danger[i] = 1.0f;
            } else {
                danger[i] = 0.0f;
            }

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
            // This will place a weight on each of the ray directions.
            // This will also modify the chosen direction so it will try as much as possible to not go in the path of danger,
            // however this current iteration causes the enemy to run away if there's too much danger.
            // I want the enemy to strafe around danger (rocks and obstacles) and chase the player.
            // This will need a bit more tweaking before it's ready
            chosenDir += rayDirections[i] * interest[i];
            chosenDir += rayDirections[i] * -passivity * danger[i];
        }
        chosenDir = chosenDir.Normalized();
    }
}
