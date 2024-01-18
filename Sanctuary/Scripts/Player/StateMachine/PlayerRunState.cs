using Godot;
using System;

public partial class PlayerRunState : State {
    // 16 units move threshold.  i.e. don't move if the mouse clicks within 16 units of the global position
    private const int MOVE_THRESHOLD = 256;

    private float maxSpeed = 200;
    private float maxAccel = 800;

    [Export]
    private Player p;
    public override void EnterState() {
        p.SetIsMoving(true);
    }

    public override void ExitState() {
        p.SetIsMoving(false);
    }

    public override void Update(double delta) {
        if (Input.IsActionPressed("Move")) {
            p.SetMoveTo(p.GetGlobalMousePosition());
        }

        if (Input.IsActionJustPressed("Spell1")) {
            p.SetAttackDir(p.GetLocalMousePosition().Normalized());
            EmitSignal(State.SignalName.StateFinished, this, Globals.PLAYER_ATTACK);
        }
    }

    public override void PhysicsUpdate(double delta) {
        if (p.GlobalPosition.DistanceSquaredTo(p.GetMoveTo()) <= MOVE_THRESHOLD || p.GetIsAttacking()) {
            EmitSignal(State.SignalName.StateFinished, this, Globals.PLAYER_IDLE);
        }
        var direction = p.GlobalPosition.DirectionTo(p.GetMoveTo());
        p.SetVelocity(new Vector2(maxSpeed * direction.X, maxSpeed * direction.Y) * (float)delta);
        p.MoveAndCollide(p.GetVelocity());
    }

    
}
