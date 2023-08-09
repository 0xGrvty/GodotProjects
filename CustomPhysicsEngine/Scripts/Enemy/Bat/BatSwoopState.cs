using Godot;
using System;

public partial class BatSwoopState : IStateMachine {
    public IStateMachine EnterState(Node actor) {
        var bat = actor as Bat;
        //velocity.X = Mathf.MoveToward(velocity.X, maxSpeed, maxAccel * (float)delta);
        //velocity.Y = Mathf.MoveToward(velocity.Y, GetGravity(), GetGravity() * (float)delta);
        //Vector2 vel = Vector2.Zero;
        //var targetX = bat.GlobalPosition.DirectionTo(bat.GetTarget().GlobalPosition).X;
        //var targetY = bat.GlobalPosition.DirectionTo(bat.GetTarget().GlobalPosition).Y;
        //var targetPos = bat.GlobalPosition.DirectionTo(bat.GetTargetPosSnapshot());
        //var targetPos = bat.GetTargetPosSnapshot();
        //vel.X = Mathf.MoveToward(bat.Velocity.X, targetPos.X * bat.GetMaxSpeed(), bat.GetMaxAccel() * (float)bat.GetPhysicsProcessDeltaTime());
        //vel.Y = Mathf.MoveToward(bat.Velocity.Y, bat.GetMaxSpeed() * Mathf.Pow((float)bat.GetPhysicsProcessDeltaTime() - targetPos.X, 2) + targetPos.Y, bat.GetMaxAccel() * (float)bat.GetPhysicsProcessDeltaTime());
        //vel.Y = Mathf.MoveToward(bat.Velocity.Y, targetPos.Y * 2f * bat.GetMaxSpeed() * (float) bat.GetPhysicsProcessDeltaTime() - 2f * bat.GetMaxSpeed() * targetPos.X, bat.GetMaxAccel() * (float)bat.GetPhysicsProcessDeltaTime());
        //bat.GlobalPosition += new Vector2(targetPos.X * bat.GetMaxSpeed() * (float)bat.GetPhysicsProcessDeltaTime(), Mathf.Pow((float)bat.GetPhysicsProcessDeltaTime() - targetPos.X, 2) - targetPos.Y);
        //bat.Velocity.X += bat.GetMaxSpeed() * targetPos.X * (float)bat.GetPhysicsProcessDeltaTime(); 
        //GD.Print(vel);
        //bat.Velocity = vel;

        //bat.MoveX(bat.Velocity.X * (float)bat.GetPhysicsProcessDeltaTime(), new Callable(bat, nameof(bat.OnCollisionX)));
        //bat.MoveY(bat.Velocity.Y * (float)bat.GetPhysicsProcessDeltaTime(), new Callable(bat, nameof(bat.OnCollisionY)));
        bat.Move();
        if (bat.Velocity.Y < 0 && bat.GlobalPosition.Y <= bat.GetStartingPos().Y) {

            bat.Velocity = Vector2.Zero;
            return bat.sleepState;
        }
        return bat.swoopState;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        throw new NotImplementedException();
    }
}
