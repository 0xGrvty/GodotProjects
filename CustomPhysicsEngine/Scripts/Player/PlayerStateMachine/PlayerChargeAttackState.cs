using Godot;
using System;

public partial class PlayerChargeAttackState : IStateMachine {
    private Attack attack;
    private int activeFrame = 0;

    public Attack Attack { get { return attack; } }
    public PlayerChargeAttackState(Node hitboxes) {
        attack = new Attack(hitboxes);
    }
    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        

        player.AnimatedSprite.SpeedScale = 1;
        if (Input.IsActionPressed("ChargeAttack")) {
            player.AnimatedSprite.Play("ChargeAttack_Charge");
            return this;
        }

        if (Input.IsActionJustReleased("ChargeAttack")) {
            player.AnimatedSprite.Play("ChargeAttack_Release");
        }

        /// TODO: Use the new ChangeAttackState function.  Since this one does a bit of math
        /// with the player's velocity, maybe it's worth making a new interface for
        /// AttackStates.  At this point, I don't know if every attack will have a general
        /// state tree or if only special attacks will have special state trees. See note in Player.ChangeAttackState()

        if (player.AnimatedSprite.Animation == "ChargeAttack_Release") {
            player.Velocity = new Vector2((int)player.Facing * 1.25f * player.GetMaxSpeed(), 0f);
            player.DoMovement(player.GetProcessDeltaTime(), (int)player.Facing);
            if (player.AnimatedSprite.Frame == activeFrame) {

                attack.CheckHitboxes(player, player.Facing);
            } else {

                foreach (Hitbox h in attack.GetHitboxes()) {
                    h.Visible = false;
                }

            }

            // There has to be an absolute better way to do this.  AnimatedSprite.AnimationFinished signal?
            //if (player.AnimatedSprite.Frame >= player.AnimatedSprite.SpriteFrames.GetFrameCount(player.AnimatedSprite.Animation) - 1) {
            //    attack.ClearHitlist();
            //    return player.playerIdleState;
            //}
        }

        return this;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        throw new NotImplementedException();
    }
}
