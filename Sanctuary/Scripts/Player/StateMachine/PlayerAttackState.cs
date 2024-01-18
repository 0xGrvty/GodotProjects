using Godot;
using System;

public partial class PlayerAttackState : State {
    [Export]
    private Player p;
    public override void EnterState() {
        p.SetIsAttacking(true);
    }

    public override void ExitState() {
        p.SetIsAttacking(false);
    }

    public override void Update(double delta) {
        base.Update(delta);
    }

    public override void PhysicsUpdate(double delta) {
        var projInstance = (Projectile)p.GetScene(Scenes.PROJ_SCENE).Instantiate();
        projInstance.Init(p.GlobalPosition, p.GetAttackDir());
        p.GetParent().AddChild(projInstance);
        EmitSignal(State.SignalName.StateFinished, this, Globals.PLAYER_IDLE);
    }

    
}
