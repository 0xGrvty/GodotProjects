using Godot;
using System;
using static Bat;

public partial class BatSleepState : IStateMachine {
    public IStateMachine EnterState(Node actor) {
        var bat = actor as Bat;
        foreach (Node2D a in bat.GM.GetAllActors()) {

            if (bat.CheckProximity(a)) {

                switch (bat.GetBehavior()) {

                    case Bat.Behavior.STRAIGHT:
                        return bat.straightState;

                    case Bat.Behavior.SWOOP:
                        return bat.swoopState;

                    case Bat.Behavior.CVANIA:
                        return bat.cvaniaState;

                    case Bat.Behavior.ROCKET:
                        return bat.rocketState;

                    default:
                        return bat.sleepState;
                }

            }

        }

        return bat.sleepState;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        throw new NotImplementedException();
    }
}
