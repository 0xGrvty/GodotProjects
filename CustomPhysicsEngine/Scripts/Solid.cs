using Godot;
using System;

public partial class Solid : Wall {

    public Vector2 remainder = Vector2.Zero;
    private Game gm;
    public Game GM { get => gm; set => gm = value; }

    // Taken straight from Actor.cs.
    // Some added functionality because we need to
    // transport the riders.
    // Priority is move the platform, THEN move the riders.
    public void MoveY(float amount) {

        remainder.Y += amount;
        var move = Mathf.Round(remainder.Y);

        if (move != 0) {

            var riders = gm.GetAllRidingActors(this);
            remainder.Y -= move;
            GlobalPosition += new Vector2(0, move);
            Hitbox.Collidable = !JumpThru;

            foreach (Actor actor in gm.GetAllActors()) {
                if (actor is Missile) {
                    continue;
                }

                if (Hitbox.Intersects(actor.Hitbox, Vector2.Zero)) {

                    // If we should be moving, check all interacting actors whether or not we should push them down or push them up
                    // If they are being squished (i.e. ceiling/floor/another solid platform) then call the actor's Squish function
                    if (move > 0) {
                        actor.MoveYExact(Hitbox.Bottom - actor.Hitbox.Top, new Callable(actor, "Squish"));
                    } else {
                        actor.MoveYExact(Hitbox.Top - actor.Hitbox.Bottom, new Callable(actor, "Squish"));
                    }

                } else if (riders.Contains(actor)) {
                    // Can't make a null Callable or else C# cries at runtime.
                    actor.MoveYExact(move, new Callable(this, nameof(Bumper)));
                }
            }
        }
        Hitbox.Collidable = true;
    }

    public void MoveX(float amount) {
        remainder.X += amount;
        var move = Mathf.Round(remainder.X);

        if (move != 0) {

            var riders = gm.GetAllRidingActors(this);
            remainder.X -= move;
            GlobalPosition += new Vector2(move, 0);

            foreach (Actor actor in gm.GetAllActors()) {

                if (Hitbox.Intersects(actor.Hitbox, Vector2.Zero)) {

                    if (move > 0) {
                        actor.MoveXExact(Hitbox.Right - actor.Hitbox.Left, new Callable(actor, "Squish"));
                    } else {
                        actor.MoveXExact(Hitbox.Left - actor.Hitbox.Right, new Callable(actor, "Squish"));
                    }

                } else if (riders.Contains(actor)) {
                    actor.MoveXExact(move, new Callable(this, nameof(Bumper)));
                }
            }
        }
    }

    private void Bumper() {

    }
}
