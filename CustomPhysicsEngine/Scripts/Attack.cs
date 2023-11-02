using Godot;
using Godot.Collections;
using System;

public partial class Attack : Hitbox {

    // An attack shouldn't know the frame it is active on.  The attack should only worry about actually doing damage/adding an entity to the hitlist.
	private Godot.Collections.Array hitboxes;
    private Godot.Collections.Array hitlist;
    private int damage;
    public Attack(Node hitboxGroup) {
		var hitboxes = hitboxGroup.GetChildren();
        hitlist = new Godot.Collections.Array();
        this.hitboxes = new Godot.Collections.Array();
		foreach (Hitbox h in hitboxes) {
			if (!this.hitboxes.Contains(h)) {
				this.hitboxes.Add(h);
			}
		}
	}

	public bool CheckHitboxes(Node owner, Facing facing) {
        var hittable = owner.GetTree().GetNodesInGroup("Actors");

        foreach (Hitbox h in hitboxes) {
            // Since the attack hitboxes are attached to the player and we are not changing the scale, only flipping the animations
            // we need to flip the hitbox nodes and correct each hitbox's new Left and Right boundaries.
            h.FlipHitboxes(facing);
            h.Visible = true;
        }
        // O(N^2), can we make this faster somehow?  But then again, it breaks upon the very first hitbox that hits
        // And I can't imagine a metroidvania where there are millions of hitboxes on _one_ attack.
        foreach (Actor a in hittable) {
            if (a != owner && !hitlist.Contains(a)) {
                foreach (Hitbox h in hitboxes) {
                    if (h.Intersects(a.Hurtbox, Vector2.Zero)) {
                        hitlist.Add(a);
                        //owner.EmitSignal("Hitstop", 3);
                        //owner.EmitSignal("ShakeCamera", true);
                        //GD.Print("hit");
                        return true;
                    }
                }

            }
        }
        return false;
    }

    public Godot.Collections.Array GetHitboxes() {
        return hitboxes;
    }

    public void ClearHitlist() {
        hitlist.Clear();
    }

}
