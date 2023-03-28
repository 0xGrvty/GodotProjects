using Godot;
using System;

[Tool]
public partial class Hitbox : Node2D
{
    [Export]
    private int x = 0;
    [Export]
    private int y = 0;
    [Export]
    private int width = 16;
    [Export]
    private int height = 16;
    [Export]
    private Color color = new Color (0, 0, 1, 0.5f);
    [Export]
    private Color disableColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);

    public Color originalColor;


    private int left;
    private int right;
    private int top;
    private int bottom;
    private bool mCollidable = true;
    public bool Collidable { get => mCollidable; set => SetCollidable(value); }
    public int Left { get => GetLeft(); set => left = value; }
    public int Right { get => GetRight(); set => right = value; }
    public int Top { get => GetTop(); set => top = value; }
    public int Bottom { get => GetBottom(); set => bottom = value; }

    public override void _Ready() {
        originalColor = color;
    }

    public override void _Draw() {
        DrawRect(new Rect2(x, y, width, height), color);
    }

    public override void _PhysicsProcess(double delta) {
        QueueRedraw();
    }

    public void SetCollidable(bool isCollidable) {
        if (isCollidable) {
            color = originalColor;
        } else {
            color = disableColor;
        }
        mCollidable = isCollidable;
    }

    public int GetLeft() {
        return (int)GlobalPosition.X + x;
    }
    public int GetRight() {
        return (int)GlobalPosition.X + x + width;
    }
    public int GetTop() {
        return (int)GlobalPosition.Y + y;
    }
    public int GetBottom() {
        return (int)GlobalPosition.Y + y + height;
    }

    public bool Intersects(Hitbox other, Vector2 offset) {
        if (!this.mCollidable || !other.Collidable) {
            return false;
        }

        return ((this.Right + offset.X) > other.Left && (this.Left + offset.X ) < other.Right
            && (this.Bottom + offset.Y) > other.Top && (this.Top + offset.Y) < other.Bottom);
    }
}
