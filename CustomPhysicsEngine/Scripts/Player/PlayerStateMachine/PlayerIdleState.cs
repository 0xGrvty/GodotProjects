using Godot;
using System;

public partial class PlayerIdleState : IStateMachine {
    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        var direction = player.GetDirectionInput();
        player.DoMovement(player.GetProcessDeltaTime(), direction);
        //player.DoAttack();
        player.AnimatedSprite.SpeedScale = 1;
        player.AnimatedSprite.Play("Idle");

        if (player.IsJumping && player.IsGrounded()) {
            //EmitStateChanged(player, player.playerJumpState);
            //player.WasGrounded = true;
            return player.playerJumpState;
        }

        if (player.IsGrounded() && player.Velocity.X != 0 && direction != 0) {
            //EmitStateChanged(player, player.playerRunState);
            return player.playerRunState;
        }

        

        if (player.Velocity.Y > 0.0 && !player.IsGrounded()) {
            player.WasGrounded = true;
            return player.playerFallState;
        }

        if (Input.IsActionJustPressed("Attack")) {
            var attackButton = new StringName("Attack");
            // Technically don't *need* this, but just for consistency's sake, we should
            // add the input into the buffer when we press it for the first time
            player.AttackInputBuffer.AddInput(attackButton);
            return player.playerAttackState1;
        }

        //if (Input.IsActionJustPressed("ChargeAttack")) {
        //    return player.playerChargeAttackState;
        //}

        return player.playerIdleState;
    }
    public void EmitStateChanged(Node actor, IStateMachine state) {
        GD.Print(actor.Name, " Is now in ", state, " state");
    }
}
