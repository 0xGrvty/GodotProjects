using Godot;
using System;

public class Hammer : Area2D {
    private Vector2 spawnPosition;
    private float theta;
    private float frequency;
    private float radius;
    private float maxTimeAlive;
    private float rotationAngle;

    private const float BASE_RADIUS = 50;
    private const float RADIUS_GROWTH = 70;
    private const float MAX_SPEED = 12f;
    private const float BASE_TIME_ALIVE = 3f;
    public void Init(Vector2 SpawnPosition) {
        this.spawnPosition = SpawnPosition;
        Position = SpawnPosition + new Vector2(BASE_RADIUS, 0);
        frequency = MAX_SPEED;
        radius = BASE_RADIUS;
        maxTimeAlive = BASE_TIME_ALIVE;
        theta = 0; // 0 radians
        rotationAngle = 0; // 0 radians
    }

    public override void _PhysicsProcess(float delta) {
        // Mathematics of a circle: x^2 + y^2 = r, but this is in rectangular coordinates
        // Which makes it a little hard to figure out.  We can convert this to polar coordinates:
        // Look at notes for pictures.
        // x = rcos(theta)
        // y = rsin(theta)
        // Frequency is whatever is in the parenthesis: rcos(theta * frequency) (we can think of it as speed too)

        // Change Theta in respects to time
        theta += Mathf.Pi / 6 * delta;
        radius += RADIUS_GROWTH * delta;
        rotationAngle += Mathf.Pi / 2.75f * delta;
        Position = new Vector2(Mathf.Cos(theta * frequency) * radius, Mathf.Sin(theta * frequency) * radius) + spawnPosition;
        Rotation = rotationAngle * frequency;
        maxTimeAlive -= delta;
        if (maxTimeAlive <= 0) {
            QueueFree();
        }
    }

}
