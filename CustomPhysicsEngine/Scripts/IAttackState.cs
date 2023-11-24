using Godot;
using System;

public partial interface IAttackState {
    // Each attack should have a unique way to change states.  For example, maybe we want to allow the player to
    // cancel Attack 3 into a ChargeAttack, but not let them cancel Attack 1 or Attack 2 into a ChargeAttack.
    // Maybe we want special attacks like ChargeAttack to be able to cancel into Uppercut, but we don't want
    // Uppercut to cancel into ChargeAttack.

    void ChangeState();
}
