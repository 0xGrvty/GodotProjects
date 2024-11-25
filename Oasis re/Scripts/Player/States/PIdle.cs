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

  public override void HandleInput(InputEvent e) {
    if (e.IsActionPressed("Jump")) p.IsJumping = true;
    else if (e.IsActionPressed("Attack")) p.IsAttacking = true;
  }

  public override void Update(double delta) {
    p.PollInputs();
  }

  public override void PhysicsUpdate(double delta) {
    p.Move();
    if (p.Dir != Direction.NO_DIR && p.IsOnFloor()) {
      EmitSignal(new StringName(nameof(StateFinished)), this, "Run");
    }

    if (p.IsJumping) {
      EmitSignal(new StringName(nameof(StateFinished)), this, "Jump");
    }

    if (p.IsAttacking) EmitSignal(new StringName(nameof(StateFinished)), this, "Attack");
  }
}
