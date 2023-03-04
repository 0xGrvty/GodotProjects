using Godot;
using System;

public class EnemyRunState : IStateMachine
{
    public IStateMachine EnterState(Node2D e) {
        var enemy = (Enemy)e;
        enemy.CheckDistance();
        enemy.DoMovement();


        switch (enemy.GetFacing()) {
            case FaceDir.UP:
            case FaceDir.UP_LEFT:
            case FaceDir.UP_RIGHT:
                enemy.GetAnimatedSprite().Animation = "EnemyRunUp";
                break;
            case FaceDir.DOWN:
                enemy.GetAnimatedSprite().Animation = "EnemyRunDown";
                break;
            case FaceDir.RIGHT:
            case FaceDir.LEFT:
                enemy.GetAnimatedSprite().Animation = "EnemyRunRight";
                break;
            case FaceDir.DOWN_RIGHT:
            case FaceDir.DOWN_LEFT:
                enemy.GetAnimatedSprite().Animation = "EnemyRunDownRight";
                break;
        }
        enemy.GetAnimatedSprite().SpeedScale = 1;
        enemy.GetAnimatedSprite().Play();

        if (Mathf.Sign(enemy.GetVelocity().x) > 0 && enemy.GetAnimatedSprite().FlipH || Mathf.Sign(enemy.GetVelocity().x) < 0 && !enemy.GetAnimatedSprite().FlipH) {
            enemy.GetAnimatedSprite().FlipH = !enemy.GetAnimatedSprite().FlipH;
        }

        if (enemy.GetVelocity() == Vector2.Zero) {
            return enemy.enemyIdleState;
        }

        if (enemy.GetHealth() <= 0) {
            return enemy.enemyDeathState;
        }

        return enemy.enemyRunState;
    }

    public void EmitChangeStateSignal(Node2D e, IStateMachine state) {
        var enemy = (Enemy)e;
        enemy.EmitSignal("StateChanged", state.GetType().ToString());
    }
}
