using Godot;
using System;

public partial class PlayerAttackState3 : IStateMachine, IAttackState {
    private Attack attack;
    private int activeFrame = 2;

    public PlayerAttackState3(Node hitboxes) {
        attack = new Attack(hitboxes);
    }
    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        player.AnimatedSprite.SpeedScale = 1;
        player.AnimatedSprite.Play("Attack3");
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
        //    player.ResetAttackCounter();
        //    attack.ClearHitlist();
        //    return player.playerIdleState;
        //}

        //if (Input.IsActionJustPressed("Attack")) {
        //    if (player.AnimatedSprite.IsPlaying()) {
        //        player.ResetComboCounter();
        //        return player.playerAttackState_1;
        //    }
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

                return player.playerAttackState1;

            }

            return player.playerIdleState;

        }

        return this;
    }
}
