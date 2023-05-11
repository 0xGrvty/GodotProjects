using Godot;
using System;

public partial class PlayerAttackState3 : IStateMachine {
    private Attack attack;
    private int activeFrame = 2;

    public PlayerAttackState3(Node hitboxes) {
        attack = new Attack(hitboxes);
    }
    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        player.AnimatedSprite.SpeedScale = 1;
        player.AnimatedSprite.Play("Attack3");
        player.DoAttack();

        /// TODO: Make hitbox active on X frame
        if (player.AnimatedSprite.Frame == activeFrame) {

            attack.CheckHitboxes(player, player.Facing);

        } else {

            foreach (Hitbox h in attack.GetHitboxes()) {
                h.Visible = false;
            }

        }

        if (player.AnimatedSprite.Frame >= player.AnimatedSprite.SpriteFrames.GetFrameCount(player.AnimatedSprite.Animation) - 1) {
            player.ResetAttackCounter();
            attack.ClearHitlist();
            return player.playerIdleState;
        }

        //if (Input.IsActionJustPressed("Attack")) {
        //    if (player.AnimatedSprite.IsPlaying()) {
        //        player.ResetComboCounter();
        //        return player.playerAttackState_1;
        //    }
        //}

        return player.playerAttackState3;
    }
    public void EmitStateChanged(Node actor, IStateMachine state) {

    }
}
