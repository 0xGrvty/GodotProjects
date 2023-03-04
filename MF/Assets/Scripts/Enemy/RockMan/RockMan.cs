using Godot;
using System;

public class RockMan : Enemy
{
    public void OnHitboxBodyEntered(Node2D body) {
        if (body is PlayerBody) {
            body.EmitSignal("PlayerDamaged");
        }
    }
}
