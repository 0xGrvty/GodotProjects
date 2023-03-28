using Godot;
using System;

public partial class Wall : Node2D
{
    private Hitbox hitbox;
    public Hitbox Hitbox { get => hitbox; set => hitbox = value; }

    // Set this in the editor on a platform-by-platform basis
    [Export]
    private bool jumpThru = false;

    public bool JumpThru { get => jumpThru; }

    public override void _Ready() {
        hitbox = (Hitbox)GetNode<Node2D>("Hitbox");
        AddToGroup("Walls");
    }
}
