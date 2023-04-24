using Godot;
using System;

public partial class PlayerAttackState_2 : IStateMachine {
    private Attack attack;
    private int activeFrame = 3;

    public PlayerAttackState_2(Node hitboxes) {
        attack = new Attack(hitboxes);
    }

    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        player.AnimatedSprite.SpeedScale = 1;
        player.AnimatedSprite.Play("Attack2");
        player.DoAttack();

        if (player.AnimatedSprite.Frame == activeFrame) {

            attack.CheckHitboxes(player, player.Scale);

        } else {

            foreach (Hitbox h in attack.GetHitboxes()) {
                h.Visible = false;
            }

        }

        if (player.AnimatedSprite.Frame >= player.AnimatedSprite.SpriteFrames.GetFrameCount(player.AnimatedSprite.Animation) - 1) {
            attack.ClearHitlist();
            if (player.AttackInputBuffer > 0.0f) {
                return player.playerAttackState_3;
            }
            return player.playerIdleState;
        }

        return player.playerAttackState_2;
    }
    public void EmitStateChanged(Node actor, IStateMachine state) {

    }
}
