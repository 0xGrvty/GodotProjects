using Godot;
using Godot.Collections;
using System;

public partial class Attack : Node {

    // An attack shouldn't know the frame it is active on.  The attack should only worry about actually doing damage/adding an entity to the hitlist.
	private Godot.Collections.Array hitboxes;
    private Godot.Collections.Array hitlist;
    private int damage;

    // The attack should know who the owner is, so that it doesn't attack its owner
    [Export]
    private Node owner;

    public override void _Ready() {
        var hitboxes = GetChildren();
        hitlist = new Godot.Collections.Array();
        this.hitboxes = new Godot.Collections.Array();
        foreach (Hitbox h in hitboxes) {
            if (!this.hitboxes.Contains(h)) {
                this.hitboxes.Add(h);
            }
        }
    }

    public bool CheckHitboxes() {
        var hittable = GetTree().GetNodesInGroup("Actors");
        var o = owner as Actor;
        foreach (Hitbox h in hitboxes) {
            // Since the attack hitboxes are attached to the player and we are not changing the scale, only flipping the animations
            // we need to flip the hitbox nodes and correct each hitbox's new Left and Right boundaries.

            h.FlipHitboxes(o.Facing); /// TODO: this is a no-no, try to find a way to decouple this.
        }
        // O(N^2), can we make this faster somehow?  But then again, it breaks upon the very first hitbox that hits
        // And I can't imagine a metroidvania where there are millions of hitboxes on _one_ attack.
        foreach (Actor a in hittable) {
            if (a != owner && !hitlist.Contains(a)) {
                foreach (Hitbox h in hitboxes) {
                    if (h.Intersects(a.Hurtbox, Vector2.Zero)) {
                        hitlist.Add(a);
                        GD.Print("hit");
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
