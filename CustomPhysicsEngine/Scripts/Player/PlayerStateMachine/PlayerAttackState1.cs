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

        return player.ChangeAttackState(this, player.playerAttackState2);
    }
    public void EmitStateChanged(Node actor, IStateMachine state) {

    }
}
