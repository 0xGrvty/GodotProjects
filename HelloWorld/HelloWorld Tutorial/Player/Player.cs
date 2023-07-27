using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public int moveSpeed = 250;

	public override void _PhysicsProcess(float delta) {
		var motion = new Vector2();

		// Motion in x directionis the strength of the joystick moving on the right minus the left and similarly for y direction
		motion.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		motion.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

		MoveAndCollide(motion.Normalized() * moveSpeed * delta);
	}
}
