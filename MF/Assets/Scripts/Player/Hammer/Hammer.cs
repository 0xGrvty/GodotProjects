using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

public class Hammer : Area2D {

    private const float BASE_RADIUS = 50;
    private const float RADIUS_GROWTH_SPEED = 8f;
    private const float PI_6 = Mathf.Pi / 6;
    private const float MAX_SPEED = 12f;
    private const float BASE_TIME_ALIVE = 3f;
    private const float BASE_DAMAGE = 50.0f;

    private float damage;
    private Node2D source;
    private float positionAngle;
    private float frequency;
    private float radius;
    private float maxTimeAlive;
    private float rotationAngle;
    private float xOffset;
    private int xDirection;
    private float yOffset;
    private int yDirection;
    private float amplitudeX;
    private float amplitudeY;
    private List<Node2D> hitList;

    public void Init(Node2D source) {
        this.source = source;
        Position = source.Position;
        positionAngle = 0f;
        frequency = 15f;
        radius = 2f;
        maxTimeAlive = 3f;
        rotationAngle = 0f;
        xOffset = 0f;
        yOffset = 0f;
        //xDirection = source.GetAnimatedSprite().FlipH ? -1 : 1;
        xDirection = 1;
        yDirection = 1;
        amplitudeX = 1f;
        amplitudeY = 1f;
        damage = BASE_DAMAGE;
        hitList = new List<Node2D>();
    }

    public override void _PhysicsProcess(float delta) {
        // Mathematics of a circle:
        // x = r * B * cos(frequency)
        // y = r * B * sin(frequency)
        // where B is some constant
        // where frequency is how fast the object is rotating
        // where r is the radius
        // for a growing radius, we can do: (r + growth) * B * cos(frequency)
        // and expanding further, we can do: (r + (growth * delta)) * B * cos(frequency)

        // Change Theta in respects to time
        positionAngle += PI_6 * delta;
        radius += RADIUS_GROWTH_SPEED * delta;
        rotationAngle += Mathf.Pi / 2.75f * delta;
        //Position = new Vector2(Mathf.Cos(theta * frequency) * radius, Mathf.Sin(theta * frequency) * radius) + spawnPosition;

        // positionAngle * frequency = speed at which the hammer is spinning in a circle
        // rotationAngle = the rotation angle of the object itself
        // These are related by frequency so it looks like the hammer is consistently spinning around the head of the hammer no matter where it is on the circle
        // however the rotation angle should spin faster than the position angle so that it looks like it's spinning and not riding a line
        Position = new Vector2((xDirection * xOffset) + Mathf.Cos(positionAngle * frequency) * radius * amplitudeX,
            (yDirection * yOffset) + Mathf.Sin(positionAngle * frequency) * radius * amplitudeY) + Position;
        Rotation = rotationAngle * frequency;
        maxTimeAlive -= delta;
        if (maxTimeAlive <= 0) {
            GD.Print("Queue Free Position: " + Position);
            QueueFree();
        }
    }

    public void OnHammerBodyEntered(Node2D body) {
        //body.CallDeferred("TakeDamage", damage);
        if (hitList.Contains(body)) {
            return;
        }
        hitList.Add(body);
        body.EmitSignal("Hit", damage);
    }

}
