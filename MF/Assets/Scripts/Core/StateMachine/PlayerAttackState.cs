using Godot;
using System;

public class PlayerAttackState : IStateMachine
{
    public IStateMachine EnterState(Player player) {
        player.GetAnimatedSprite().Animation = "PlayerAttack";
        player.GetAnimatedSprite().SpeedScale = 2;
        player.GetAnimatedSprite().Play();
        // Check to see if we are attacking, then make a hammer if we aren't
        if (!player.IsAttacking && player.GetAnimatedSprite().Frame == 2) {

            // To control if we are attacking, don't want to spawn infinity hammers
            player.IsAttacking = true;
            Hammer h = (Hammer)player.hammerScene.Instance();
            h.Init(player.Position);
            player.GetParent().AddChild(h);
        }

        // Have the player update the attack cooldown, then check if it's less than or equal to 0
        // if (player.UpdateAttackCooldown <= 0) {
        if (player.GetAnimatedSprite().Frame >= 13) {
            // We can attack again if the player attack cooldown is up, so reset the attack cooldown
            player.ResetAttackCooldown();
            if (Input.IsActionPressed("BlessedHammer")) {
                EmitChangeStateSignal(player, player.playerAttackState);
                return player.playerAttackState;
            }
            // Check to see if the player is holding a direction after attacking
            if (player.GetVelocity() != Vector2.Zero) {
                EmitChangeStateSignal(player, player.playerRunState);
                return player.playerRunState;

            // Otherwise, change their state to Idle
            } else {
                EmitChangeStateSignal(player, player.playerIdleState);
                return player.playerIdleState;
            }
        }
        // Continue returning this state, we don't want them to be able to move while attacking (Diablo 2-esque)
        
        return player.playerAttackState;
        
    }
    public void EmitChangeStateSignal(Player player, IStateMachine state) {
        player.EmitSignal("StateChanged", state.GetType().ToString());
    }
}
