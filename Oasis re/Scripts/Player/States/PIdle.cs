using Godot;

public partial class PIdle : State {
  [Export]
  private Player p;
  [Export]
  private AnimationPlayer ap;
  public override void EnterState() {
    p.BgAP.Play("Idle");
    if ((p.prevState is PRun || p.prevState is PAttack) && Mathf.Abs(p.Velocity.X) < 500) p.FgAP.Play("Idle");
    else p.FgAP.Play("Land");
  }

  public override void ExitState() {
    p.prevState = this;
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

  public void AfterLand() {
    p.FgAP.Play("Idle");
  }
}
