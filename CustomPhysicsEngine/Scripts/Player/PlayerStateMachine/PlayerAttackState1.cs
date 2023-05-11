using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class PlayerAttackState1 : IStateMachine {
    private Attack attack;
    private int activeFrame = 2;
    public PlayerAttackState1(Node hitboxes) {
        attack = new Attack(hitboxes);
    }
    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        player.AnimatedSprite.SpeedScale = 1;
        player.AnimatedSprite.Play("Attack1");
        player.DoAttack();

        if (player.AnimatedSprite.Frame == activeFrame) {

            attack.CheckHitboxes(player, player.Facing);
        } else {

            foreach (Hitbox h in attack.GetHitboxes()) {
                h.Visible = false;
            }

        }

        if (player.AnimatedSprite.Frame >= player.AnimatedSprite.SpriteFrames.GetFrameCount(player.AnimatedSprite.Animation) - 1) {
            attack.ClearHitlist();
            if (player.AttackInputBuffer > 0.0f) {
                return player.playerAttackState2;
            }
            return player.playerIdleState;
        }

        
        //if (Input.IsActionJustPressed("Attack") && player.AnimatedSprite.Frame >= player.AnimatedSprite.SpriteFrames.GetFrameCount("Attack1") - 3) {
        //    player.AttackCounter--;
        //    player.GetAttackTimer().Start();
        //    return player.playerAttackState_2;
        //}

        return player.playerAttackState1;
    }
    public void EmitStateChanged(Node actor, IStateMachine state) {

    }
}
