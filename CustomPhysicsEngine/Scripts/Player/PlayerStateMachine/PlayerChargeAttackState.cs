using Godot;
using System;

public partial class PlayerChargeAttackState : State, IAttackState {
    [Export]
    private Player p;
    [Export]
    private AnimationPlayer ap;

    [Signal]
    public delegate void CreateAfterimageEventHandler();

    private Timer t;

    

    public override void _Ready() {
        t = GetNode<Timer>("AfterimageTimer");
        t.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
    }

    public override void EnterState() {
        ap.Play("Charge_Hold");
        EmitSignal(nameof(OnAttack), this);
        
    }

    public override void ExitState() {
        p.AttackInputBuffer.ClearBuffer();
    }

    public override void PhysicsUpdate(double delta) {
        // We want to check if the charge button is not held because
        // There will be a polling issue if the player buffers the input and
        // lets go before it can poll the release in IsActionJustReleased()
        if (!Input.IsActionPressed("Charge")) {
            // If the timer has not started yet for the first time this animation plays
            if (!(t.TimeLeft > 0)) {
                t.Start();
            }
            // When charge is released, enable the shader and very slightly adjust the mix weight of the shader
            (p.Sprite.Material as ShaderMaterial).SetShaderParameter("mix_weight", 0.3);
            (p.Sprite.Material as ShaderMaterial).SetShaderParameter("whiten", true);
            ap.Play("Charge_Release");
        }
        if (ap.CurrentAnimation.Equals("Charge_Release")) {
            
            p.DoAttack();
        }
    }

    public void ChangeState() {
        t.Stop();
        // At the end of the animation, disable the shader
        (p.Sprite.Material as ShaderMaterial).SetShaderParameter("whiten", false);
        EmitSignal(nameof(StateFinished), this, "Idle");
    }

    private void OnTimerTimeout() {
        EmitSignal(nameof(CreateAfterimage), this);
    }
}
