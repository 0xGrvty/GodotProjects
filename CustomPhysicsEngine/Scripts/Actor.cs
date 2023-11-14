using Godot;
using Godot.NativeInterop;
using System;


// Move only by integers.
// Why is this favored?
// Well now we know the exact (x, y) instead of having floating points
// This will streamline a lot of things such as AABB (Axis-Aligned Bounding Box Collision).
// This eliminates floating point comparisons since we know exactly where we are
// since everything is rounded to the nearest integer.
public partial class Actor : Node2D {
    private Vector2 remainder = Vector2.Zero;
    private Hitbox hurtbox;
    private Game gm;
    private Facing facing;

    // All actors will have either an Animated Sprite or an Animation Player.
    // Background objects, collectibles, and things of that nature will have Animated Sprites.
    // Controllable units and enemies will have an Animation Player.  This allows us to use more functionality
    // in the Animation Player such as call methods during specific frames of animation.
    private AnimatedSprite2D animatedSprite;
    
    private Sprite2D sprite;
    public Game GM { get => gm; set => gm = value; }
    public Hitbox Hurtbox { get => hurtbox; set => hurtbox = value; }
    public AnimatedSprite2D AnimatedSprite { get => animatedSprite; set => animatedSprite = value; }
    
    public Sprite2D Sprite { get => sprite; set => sprite = value; }
    public Facing Facing { get => facing; set => facing = value; }

    // Take the amount (velocity.x * delta time)
    // and add it to the remainder.  Round this and this will give us how much we should move.
    public void MoveX(float amount, Callable callback) {
        remainder.X += amount;
        var move = Mathf.Round(remainder.X);
        // If the amount we'll move by after rounding is not zero
        // i.e. we hold right and we move 11.245 units over
        // then subtract the move from the current remainder since we are moving by it already
        // then calculate and execute the exact movement in the X direction
        if (move != 0) {
            remainder.X -= move;
            MoveXExact(move, callback);
        }
    }

    // While the amount we're moving by is not 0, move our position in that direction until
    // we don't have to move anymore.
    // i.e if amount (how much we moved in MoveX) was 11
    // step is 1
    // After moving position in the X direction by step, subtract step from amount
    // until the amount we need to move is 0 (not holding a direction).
    // now amount is 0 and we break from changing our position.
    public void MoveXExact(float amount, Callable callback) {
        var step = Math.Sign(amount);
        while (amount != 0) {

            // If the game manager detects a collision, then perform the callback function, otherwise update the position
            if (gm.CheckWallsCollision(this, new Vector2(step, 0))) {
                callback.Call();
                return;
            }
            GlobalPosition += new Vector2(step, 0);
            amount -= step;
        }
    }

    public void MoveY(float amount, Callable callback) {
        remainder.Y += amount;
        var move = Mathf.Round(remainder.Y);
        if (move != 0) {
            remainder.Y -= move;
            MoveYExact(move, callback);
        }
    }

    public void MoveYExact(float amount, Callable callback) {
        var step = Math.Sign(amount);
        while (amount != 0) {
            if (gm.CheckWallsCollision(this, new Vector2(0, step))) {
                callback.Call();
                return;
            }
            GlobalPosition += new Vector2(0, step);
            amount -= step;
        }
    }

    public void ZeroRemainderX() {
        remainder.X = 0;
    }
    public void ZeroRemainderY() {
        remainder.Y = 0;
    }

    // Returns true if there is an intersect with the solid and if there is not an intersect with itself (squish)
    public bool IsRiding(Solid solid, Vector2 offset) {
        return !hurtbox.Intersects(solid.Hitbox, Vector2.Zero) && hurtbox.Intersects(solid.Hitbox, offset);
    }

    // Figure something out for getting squished.  Force player to crouch?
    public void Squish() {
        GD.Print("Squished");
    }
}