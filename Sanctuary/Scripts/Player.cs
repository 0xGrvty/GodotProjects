using Godot;
using System;
using static Godot.TextServer;

public partial class Player : Actor {
    [Signal]
    public delegate void MoveEventHandler();
    // 16 units move threshold.  i.e. don't move if the mouse clicks within 16 units of the global position
    private const int MOVE_THRESHOLD = 256;

    private float maxSpeed = 200;
    private float maxAccel = 800;
    private Vector2 velocity = Vector2.Zero;
    private Vector2 moveTo = Vector2.Zero;
    private bool isMoving = false;

    public override void _Ready() {
        InitNodes(); 
    }
    public override void _Process(double delta) {
        if (Input.IsActionPressed("Move")) {
            moveTo = GetGlobalMousePosition();
            isMoving = true;
        }
    }
    public override void _PhysicsProcess(double delta) {
        if (GlobalPosition.DistanceSquaredTo(moveTo) <= MOVE_THRESHOLD) {
            isMoving = false;
        }
        if (isMoving) {
            var direction = GlobalPosition.DirectionTo(moveTo);
            velocity = new Vector2(maxSpeed * direction.X, maxSpeed * direction.Y);
            MoveX(velocity.X * (float)delta, new Callable(this, nameof(OnCollisionX)));
            MoveY(velocity.Y * (float)delta, new Callable(this, nameof(OnCollisionY)));
        }
        
    }

    public void OnCollisionX() {
        velocity.X = 0;
        ZeroRemainderX();
    }

    public void OnCollisionY() {

        // From 2D project, might be able to use it in this project too.  Let's keep it for now.
        //// First do a fuzzy check to see if the player hits a corner
        //if (velocity.Y < 0) {
        //    if (velocity.X >= 0) {
        //        for (int i = 1; i <= CORNER_CORRECTION; i++) {
        //            // If the fuzzy check returns false, then move them and let them jump!
        //            if (!GM.CheckWallsCollision(this, new Vector2(i, -1))) {
        //                GlobalPosition += new Vector2(i, -1);
        //                // return since we don't want to zero the remainder when we correct the jump
        //                return;
        //            }
        //            if (!GM.CheckWallsCollision(this, new Vector2(-i, -1))) {
        //                GlobalPosition += new Vector2(-i, -1);
        //                // return since we don't want to zero the remainder when we correct the jump
        //                return;
        //            }
        //        }
        //    }
        //    if (velocity.X <= 0) {
        //        for (int i = 1; i <= CORNER_CORRECTION; i++) {
        //            if (!GM.CheckWallsCollision(this, new Vector2(i, -1))) {
        //                GlobalPosition += new Vector2(i, -1);
        //                return;
        //            }
        //            if (!GM.CheckWallsCollision(this, new Vector2(-i, -1))) {
        //                GlobalPosition += new Vector2(-i, -1);
        //                return;
        //            }
        //        }
        //    }

        //}
        velocity.Y = 0;
        ZeroRemainderY();
    }


}
