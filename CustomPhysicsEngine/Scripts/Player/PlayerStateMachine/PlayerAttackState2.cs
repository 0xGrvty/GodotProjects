using Godot;
using System;

public partial class PlayerAttackState2 : IStateMachine, IAttackState {
    private Attack attack;
    private int activeFrame = 3;

    public PlayerAttackState2(Node hitboxes) {
        attack = new Attack(hitboxes);
    }

    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        player.AnimatedSprite.SpeedScale = 1;
        player.AnimatedSprite.Play("Attack2");
        player.DoAttack(attack, activeFrame);

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
        //        return player.playerAttackState3;
        //    }
        //    return player.playerIdleState;
        //}

        return ChangeState(player);
    }
    public void EmitStateChanged(Node actor, IStateMachine state) {

    }

    public IStateMachine ChangeState(Node actor) {
        var player = actor as Player;
        var playerAttackBuffer = player.GetInputBufferContents();

        if (player.IsOnLastFrame()) {

            attack.ClearHitlist();

            if (playerAttackBuffer.Contains((int)InputBuffer.BUTTON.ATTACK)) {

                return player.playerAttackState3;

            }

            return player.playerIdleState;

        }

        return this;
    }
}
