using Godot;
using System;

// Custom camera class
// Not finished, just seeing how it works.
public partial class Camera : Node {

    private Vector2 screenSize = new Vector2((float) ProjectSettings.GetSetting("display/window/size/viewport_width"), (float) ProjectSettings.GetSetting("display/window/size/viewport_height"));
    private Player player;

    public override void _Ready() {
        //var canvasTransform = GetViewport().CanvasTransform;
        //canvasTransform[2] = GetParent().GetNode<Node2D>("Player").GlobalPosition;
        //GD.Print(GetParent().GetNode<Node2D>("Player").GlobalPosition);
        //GD.Print(canvasTransform);
        //GetViewport().CanvasTransform = canvasTransform;
        player = (Player)GetParent().GetNode<Node2D>("Player");

    }

    public void UpdateCamera() {
        return;
    }
}
