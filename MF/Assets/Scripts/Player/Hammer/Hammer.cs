using Godot;
using Godot.Collections;
using System;
using System.Collections;

public class Hammer : Area2D {
    private const float BASE_RADIUS = 50;
    private const float RADIUS_GROWTH_SPEED = 70f;
    private const float PI_6 = Mathf.Pi / 6;
    private const float MAX_SPEED = 12f;
    private const float BASE_TIME_ALIVE = 3f;

    private Vector2 spawnPosition;
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

    public void Init(Vector2 SpawnPosition) {
        this.spawnPosition = SpawnPosition;
        Position = SpawnPosition;
        GD.Print("Starting Position: " + Position);
        positionAngle = 0f;
        frequency = 15f;
        radius = 50f;
        maxTimeAlive = 3f;
        rotationAngle = 0f;
        xOffset = 0f;
        yOffset = 0f;
        xDirection = 1;
        yDirection = 1;
        amplitudeX = 1f;
        amplitudeY = 1f;
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
        Position = new Vector2(xOffset + xDirection * Mathf.Cos(positionAngle * frequency) * radius * amplitudeX,
            yOffset + yDirection * Mathf.Sin(positionAngle * frequency) * radius * amplitudeY) + spawnPosition;
        Rotation = rotationAngle * frequency;
        maxTimeAlive -= delta;
        if (maxTimeAlive <= 0) {
            GD.Print("Queue Free Position: " + Position);
            QueueFree();
        }
    }

}
