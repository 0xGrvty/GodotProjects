using Godot;
using Godot.Collections;
using System;

public partial class Attack : Hitbox {
	private Godot.Collections.Array hitboxes;
    private Godot.Collections.Array hitlist;
    private int damage;
    public Attack(Node hitboxGroup) {
		var hitboxes = hitboxGroup.GetChildren();
        this.hitlist = new Godot.Collections.Array();
        this.hitboxes = new Godot.Collections.Array();
		foreach (Hitbox h in hitboxes) {
			if (!this.hitboxes.Contains(h)) {
				this.hitboxes.Add(h);
			}
		}
	}

	public void CheckHitboxes(Node owner, Facing facing) {
        var hittable = owner.GetTree().GetNodesInGroup("Actors");

        foreach (Hitbox h in hitboxes) {
            h.SetFlipped(facing);
            h.Visible = true;
        }
        // O(N^2), can we make this faster somehow?  But then again, it breaks upon the very first hitbox that hits
        // And I can't imagine a metroidvania where there are millions of hitboxes on _one_ attack.
        foreach (Actor a in hittable) {
            if (a is Enemy && !hitlist.Contains(a)) {
                foreach (Hitbox h in hitboxes) {
                    if (h.Intersects(a.Hitbox, Vector2.Zero)) {
                        hitlist.Add(a);
                        owner.EmitSignal("Hitstop", 3);
                        owner.EmitSignal("ShakeCamera", true);
                        GD.Print("Ow fuck");
                        break;
                    }
                }

            }
        }
    }

    public Godot.Collections.Array GetHitboxes() {
        return hitboxes;
    }

    public void ClearHitlist() {
        hitlist.Clear();
    }

}
