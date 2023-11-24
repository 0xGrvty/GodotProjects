using Godot;



public partial class PlayerAttackState1 : State, IAttackState {

    // Newly refactored to use the animation player.  We are calling ChangeState() from the animation timeline now.
    // Using AnimatedSprite2D caused an issue where the next animation would start before allowing the last frame to finish
    // due to the ChangeState function being called on the last frame exactly when it comes out and not allowing the last frame to fully play through.
    // AnimatedSprite2D should be used for non-actors such as collectibles, trees, NPCs in the background possibly.

    

    [Export]
    private Player p;
    [Export]
    private AnimationPlayer ap;


    public override void EnterState() {
        ap.Play("Attack1");
    }

    public override void PhysicsUpdate(double delta) {
        p.DoAttack();
    }

    public void ChangeState() {
        if (p.GetInputBufferContents().Contains(1)) {
            EmitSignal(nameof(StateFinished), this, "Attack2");
        } else {
            EmitSignal(nameof(StateFinished), this, "Idle");
        }
    }
}
