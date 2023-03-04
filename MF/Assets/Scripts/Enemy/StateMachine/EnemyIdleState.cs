using Godot;
using System;

public class EnemyIdleState : IStateMachine
{
    public IStateMachine EnterState(Node2D e) {
        var enemy = (Enemy)e;
        enemy.CheckDistance();
        enemy.DoMovement();

        switch (enemy.GetFacing()) {
            case FaceDir.UP:
            case FaceDir.UP_LEFT:
            case FaceDir.UP_RIGHT:
                enemy.GetAnimatedSprite().Animation = "EnemyIdleUp";
                break;
            case FaceDir.DOWN:
                enemy.GetAnimatedSprite().Animation = "EnemyIdleDown";
                break;
            case FaceDir.RIGHT:
            case FaceDir.LEFT:
                enemy.GetAnimatedSprite().Animation = "EnemyIdleRight";
                break;
            case FaceDir.DOWN_RIGHT:
            case FaceDir.DOWN_LEFT:
                enemy.GetAnimatedSprite().Animation = "EnemyIdleDownRight";
                break;
        }

        enemy.GetAnimatedSprite().SpeedScale = 1;
        enemy.GetAnimatedSprite().Play();

        if (enemy.GetHealth() <= 0) {
            return enemy.enemyDeathState;
        }

        if (enemy.GetVelocity() != Vector2.Zero) {
            return enemy.enemyRunState;
        }

        return enemy.enemyIdleState;
    }

    public void EmitChangeStateSignal(Node2D e, IStateMachine state) {
        var enemy = (Enemy)e;
        enemy.EmitSignal("StateChanged", state.GetType().ToString());
    }
}
