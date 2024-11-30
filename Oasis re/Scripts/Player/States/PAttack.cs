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
    ap.Play("Attack");
    p.IsAttacking = false;
    p.Velocity = Vector2.Zero;
    
  }

  public override void ExitState() {
    attack.Monitoring = false;
    hitlist.Clear();
  }

  public override void Update(double delta) {
    p.PollInputs();
  }

  public override void PhysicsUpdate(double delta) {
    
  }

  private void Startup() {

  }
  private void Active() {
    //attack.Monitoring = true;

    var pos = p.GlobalPosition + new Vector2(0.0f, -20.0f);
    var to = pos + new Vector2((float)p.Facing * 300.0f, 0);
    var spaceState = p.GetWorld2D().DirectSpaceState;
    var query = PhysicsRayQueryParameters2D.Create(pos, to);
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
    EmitSignal(new StringName(nameof(StateFinished)), this, "Idle");
  }

  private void OnAttackBodyEntered(Node2D body) {

    if (body is Boss && !hitlist.Contains(body)) {
      hitlist.Add(body);
      GD.Print("Attacking: " + body.Name);
    }

  }

}