using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public partial class PlayerAttackState1 : IStateMachine, IAttackState {
    // An attack shouldn't know the frame it is active on.  The attack should only worry about actually doing damage/adding an entity to the hitlist.
    // The state will handle any frame checks.
    private Attack attack;
    private int activeFrame = 2;

    public PlayerAttackState1(Node hitboxes) {
        attack = new Attack(hitboxes);
    }
    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        player.AnimatedSprite.SpeedScale = 1;
        player.AnimatedSprite.Play("Attack1");
        player.DoAttack(attack, activeFrame);


        // Refactor this into player.DoAttack()
        //if (player.AnimatedSprite.Frame == activeFrame) {

        //    if (attack.CheckHitboxes(player, player.Facing)) {
        //        player.EmitSignal("Hitstop", 3);
        //        player.EmitSignal("ShakeCamera", true);
        //    }

        //} else {

        //    foreach (Hitbox h in attack.GetHitboxes()) {
        //        h.Visible = false;
        //    }

        //}

        //if (player.AnimatedSprite.Frame >= player.AnimatedSprite.SpriteFrames.GetFrameCount(player.AnimatedSprite.Animation) - 1) {
        //    attack.ClearHitlist();
        //    if (player.AttackInputBuffer > 0.0f) {
        //        return player.playerAttackState2;
        //    }
        //    return player.playerIdleState;
        //}

        //return player.ChangeAttackState(this, player.playerAttackState2);
        return ChangeState(player);
    }

    public IStateMachine ChangeState(Node actor) {
        var player = actor as Player;
        var playerAttackBuffer = player.GetInputBufferContents();
        if (player.IsOnLastFrame()) {
            attack.ClearHitlist();
            if (playerAttackBuffer.Contains((int)InputBuffer.BUTTON.ATTACK)) {
                return player.playerAttackState2;
            }
            return player.playerIdleState;
        }
        return this;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {

    }


}
