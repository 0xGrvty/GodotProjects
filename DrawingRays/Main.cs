using Godot;
using System;
using System.Collections.Generic;

public class Main : Node2D {
    [Export]
    public PackedScene cbsn;
    Player player;
    [Export]
    private int numRays = 8;
    private int points = 32;
    private float radius = 200f;
    private Vector2[] rayDirections;
    private float[] interest;
    private float[] danger;
    private Vector2 chosenDir;
    private float lookAhead = 100f;
    private bool drawInterestNode = false;
    private List<ContextBasedSteeringNode> nodeList;
    private bool drawDangerNode = false;


    public override void _Ready() {
        nodeList = new List<ContextBasedSteeringNode>();
    }

    public override void _Process(float delta) {
    }
}
