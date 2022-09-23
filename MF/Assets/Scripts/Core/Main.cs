using Godot;
using System;

public class Main : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private Camera2D mainCamera;
    private Node2D follow;
    private Vector2 cameraBounds;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        mainCamera = GetNode<Camera2D>("Camera2D");
        follow = GetNode<Player>("Player");
        cameraBounds = new Vector2(50, 50);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public override void _PhysicsProcess(float delta) {
        mainCamera.Align();
        mainCamera.Position = follow.Position;
    }
}
