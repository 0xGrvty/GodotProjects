using Godot;
using System;

public partial class BatSwoopState : IStateMachine {
    public IStateMachine EnterState(Node actor) {
        var bat = actor as Bat;
        //velocity.X = Mathf.MoveToward(velocity.X, maxSpeed, maxAccel * (float)delta);
        //velocity.Y = Mathf.MoveToward(velocity.Y, GetGravity(), GetGravity() * (float)delta);
        Vector2 vel = Vector2.Zero;
        var targetX = bat.GlobalPosition.DirectionTo(bat.GetTarget().GlobalPosition).X;
        var targetY = bat.GlobalPosition.DirectionTo(bat.GetTarget().GlobalPosition).Y;
        vel.X = Mathf.MoveToward(bat.Velocity.X, 3f * bat.GetMaxSpeed() * targetX, bat.GetMaxAccel() * (float)bat.GetPhysicsProcessDeltaTime());
        vel.Y = Mathf.MoveToward(bat.Velocity.Y, -bat.GetMaxSpeed() * (Mathf.Pow(targetX, 2) * targetX - targetY), bat.GetMaxAccel() * (float)bat.GetPhysicsProcessDeltaTime());
        GD.Print(vel);
        bat.Velocity = vel;

        bat.MoveX(bat.Velocity.X * (float)bat.GetPhysicsProcessDeltaTime(), new Callable(bat, nameof(bat.OnCollisionX)));
        bat.MoveY(bat.Velocity.Y * (float)bat.GetPhysicsProcessDeltaTime(), new Callable(bat, nameof(bat.OnCollisionY)));
        return bat.swoopState;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        throw new NotImplementedException();
    }
}
