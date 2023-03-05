using Godot;
using System;

public class EnemyDeathState : IStateMachine
{
    private bool isDead = false;
    public IStateMachine EnterState(Node2D e) {
        var enemy = (Enemy)e;
        enemy.GetAnimatedSprite().Animation = "EnemyDeathDown";
        enemy.GetAnimatedSprite().SpeedScale = 1;
        enemy.GetAnimatedSprite().Play();

        if (enemy.GetAnimatedSprite().Frame >= enemy.GetAnimatedSprite().Frames.GetFrameCount(enemy.GetAnimatedSprite().Animation) - 1 && !isDead) {
            isDead = true;
            enemy.EmitSignal("Died");
            enemy.QueueFree();
        }

        return enemy.enemyDeathState;
    }

    public void EmitChangeStateSignal(Node2D e, IStateMachine state) {
        var enemy = (Enemy)e;
        enemy.EmitSignal("StateChanged", state.GetType().ToString());
    }
}
