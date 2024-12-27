using Godot;

public partial class PAttack : State {

  // Export variables
  [Export]
  private Player p;
  [Export]
  private AnimationPlayer ap;
  [Export]
  private Area2D attack;

  // Private variables
  private Godot.Collections.Array<Node2D> hitlist;

  public override void _Ready() {
    hitlist = new Godot.Collections.Array<Node2D>();
  }

  public override void EnterState() {
    // if(p.Dir != Direction.NO_DIR) p.BotAP.Play("Idle");
    // p.TopAP.Play("Attack");
    // ap.Play("Attack");

    // p.TestAnimsArms.Play("Attack");
    // p.TestAnimsTorso.Play("Attack");
    p.ArmsAP.Play("Attack");
    
  }

  public override void ExitState() {
    attack.Monitoring = false;
    p.IsAttacking = false;
    hitlist.Clear();
  }

  // We need to poll the inputs just in case the player is still moving while attacking or is holding jump.
  public override void Update(double delta) {
    p.PollInputs();
    if (p.IsOnFloor() && p.Dir == Direction.NO_DIR) {
      //p.TestAnimsTorso.Play("Attack");
      //p.TestAnimsLegs.Play("Attack");
      p.TorsoAP.Play("Attack");
      p.LegsAP.Play("Attack");
    }
    else if (p.IsOnFloor() && p.Dir != Direction.NO_DIR){
      //p.TestAnimsTorso.Play("Run");
      //p.TestAnimsLegs.Play("Run");
      p.TorsoAP.Play("Run");
      p.LegsAP.Play("Run");
    } 
    else if (p.IsHoldingJump) {
      //p.TestAnimsTorso.Play("Jump");
      //p.TestAnimsLegs.Play("Jump");
      p.TorsoAP.Play("Jump");
      p.LegsAP.Play("Jump");
    }
    else if (!p.IsOnFloor()) {
      //p.TestAnimsTorso.Play("Fall");
      //p.TestAnimsLegs.Play("Fall");
      p.TorsoAP.Play("Fall");
      p.LegsAP.Play("Fall");
    }
  }

  public override void PhysicsUpdate(double delta) {
    if (p.IsHoldingJump) p.Jump(delta);
    p.Move(delta);

    if (p.TestAnimsArms.Frame == p.TestAnimsArms.SpriteFrames.GetFrameCount("Attack") - 1) {
      if (p.Dir == Direction.NO_DIR && p.IsOnFloor()) EmitSignal(SignalName.StateFinished, this, p.pIdle.Name);
      else if (p.Dir != Direction.NO_DIR && p.IsOnFloor()) EmitSignal(SignalName.StateFinished, this, p.pRun.Name);
      else if (p.IsHoldingJump && Mathf.Sign(p.Velocity.Y) < 0.0f) EmitSignal(SignalName.StateFinished, this, p.pJump.Name);
      else if (!p.IsOnFloor()) EmitSignal(SignalName.StateFinished, this, p.pFall.Name);
    }
    
  }

  private void Startup() {

  }
  private void Active() {
    //attack.Monitoring = true;

    var pos = p.GlobalPosition + new Vector2(0.0f, -20.0f);
    var to = pos + new Vector2((float)p.Facing * 300.0f, 0);
    var spaceState = p.GetWorld2D().DirectSpaceState;
    // For now, excluding the attack area2D.  We aren't going to use this as the attack is changing from a melee hitbox to a ranged raycast
    var query = PhysicsRayQueryParameters2D.Create(pos, to, p.CollisionMask);
    query.Exclude = new Godot.Collections.Array<Rid>{ attack.GetRid() };
    query.CollideWithAreas = true;
    var result = spaceState.IntersectRay(query);

    GD.Print(result);
    GD.Print(p.GlobalPosition);

    var collider = (Node2D)result?["collider"];

    var tween = GetTree().CreateTween();
    tween.TweenProperty(collider, "modulate", Colors.Red, 0.25f);
    tween.TweenProperty(collider, "modulate", Colors.White, 0.25f);
    
  }
  private void Recovery() {
    
  }

  private void End() {
    if (p.Dir == Direction.NO_DIR && p.IsOnFloor()) EmitSignal(SignalName.StateFinished, this, p.pIdle.Name);
    else if (p.Dir != Direction.NO_DIR && p.IsOnFloor()) EmitSignal(SignalName.StateFinished, this, p.pRun.Name);
    else if (p.IsHoldingJump && Mathf.Sign(p.Velocity.Y) < 0.0f) EmitSignal(SignalName.StateFinished, this, p.pJump.Name);
    else if (!p.IsOnFloor()) EmitSignal(SignalName.StateFinished, this, p.pFall.Name);
  }

  private void OnAttackBodyEntered(Node2D body) {

    if (body is Boss && !hitlist.Contains(body)) {
      hitlist.Add(body);
      GD.Print("Attacking: " + body.Name);
    }

  }

}