using Godot;

public partial class PIdle : State {
  [Export]
  private Player p;
  [Export]
  private AnimationPlayer ap;
  public override void EnterState() {
    ap.Play("Idle");
  }

  public override void ExitState() {
  }

  public override void Update(double delta) {
    p.PollInputs();
  }

  public override void PhysicsUpdate(double delta) {
    p.Move(delta);

    if (p.Dir != Direction.NO_DIR && p.IsOnFloor()) EmitSignal(SignalName.StateFinished, this, p.pRun.Name);
    else if (p.IsJumping) EmitSignal(SignalName.StateFinished, this, p.pJump.Name);
    else if (p.IsAttacking) EmitSignal(SignalName.StateFinished, this, p.pAttack.Name);
    else if (!p.IsOnFloor()) EmitSignal(SignalName.StateFinished, this, p.pFall.Name);

  }
}
