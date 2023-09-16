using Godot;
using System;

[Tool]
public partial class Hitbox: Node2D {
    [Export]
    private int x = 0;
    [Export]
    private int y = 0;
    [Export]
    private int width = 16;
    [Export]
    private int height = 16;
    [Export]
    private Color color = new Color(0, 0, 1, 0.5f);
    [Export]
    private Color disableColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
    [Export]
    private bool flipped = false;

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

    public Hitbox() {

    }

    public Hitbox(int x, int y, int width, int height, Color color, Color disableColor, Vector2 startPos) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.color = color;
        this.disableColor = disableColor;
        GlobalPosition = startPos;
        mCollidable = true;
    }
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
        return flipped ? (int)GlobalPosition.X + x + width : (int)GlobalPosition.X + x;
    }
    public int GetRight() {
        return flipped ? (int)GlobalPosition.X + x : (int)GlobalPosition.X + x + width;
    }
    public int GetTop() {
        return (int)GlobalPosition.Y + y;
    }
    public int GetBottom() {
        return (int)GlobalPosition.Y + y + height;
    }

    //public int GetLocalLeft() {
    //    return flipped ? (int)Position.X + x + width : (int)Position.X + x;
    //}
    //public int GetLocalRight() {
    //    return flipped ? (int)Position.X + x : (int)Position.X + x + width;
    //}
    //public int GetLocalTop() {
    //    return (int)Position.Y + y;
    //}
    //public int GetLocalBottom() {
    //    return (int)Position.Y + y + height;
    //}

    //public float GetRealLeft() {
    //    return flipped ? GlobalPosition.X + x + width : GlobalPosition.X + x;
    //}
    //public float GetRealRight() {
    //    return flipped ? GlobalPosition.X + x : GlobalPosition.X + x + width;
    //}
    //public float GetRealTop() {
    //    return GlobalPosition.Y + y;
    //}
    //public float GetRealBottom() {
    //    return GlobalPosition.Y + y + height;
    //}

    //public float GetRealLocalLeft() {
    //    return flipped ? Position.X + x + width : Position.X + x;
    //}
    //public float GetRealLocalRight() {
    //    return flipped ? Position.X + x : Position.X + x + width;
    //}
    //public float GetRealLocalTop() {
    //    return Position.Y + y;
    //}
    //public float GetRealLocalBottom() {
    //    return Position.Y + y + height;
    //}

    public int GetWidth()
    {
        return width;
    }

    //AABB -> Axis-aligned Boundary Box
    public bool Intersects(Hitbox other, Vector2 offset) {
        // If either this or the other hitbox are intangible, return false (they are dodging, invincible, etc)
        if (!this.mCollidable || !other.Collidable) {
            return false;
        }

        return ((this.Right + offset.X) > other.Left && (this.Left + offset.X) < other.Right
            && (this.Bottom + offset.Y) > other.Top && (this.Top + offset.Y) < other.Bottom);
    }

    // This is a helper function that reverses the x and width variables when the hitbox is flipped.
    // Example would be if our player is asymmetrical for some reason
    // or if a hitbox needs to be reflected to the other side when the player turns around

    // Can probably find a better way to do this.
    public void FlipHitboxes(Facing facing) {
        if (facing == Facing.LEFT && flipped == false) {
            //GD.Print(scale.X);
            x *= -1;
            width *= -1;
            flipped = true;
        } else if (facing == Facing.RIGHT && flipped == true) {
            x *= -1;
            width *= -1;
            flipped = false;
        }

    }

}
