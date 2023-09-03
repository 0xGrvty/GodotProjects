using Godot;
using System;

public partial class Level : Node {
    private Marker2D startingPosition;
    private Player player;

    public override void _Ready() {
        startingPosition = GetNode<Marker2D>("StartingPosition");
        player = (Player)GetParent().GetNode<Node2D>("Game/Player");
    }
}
