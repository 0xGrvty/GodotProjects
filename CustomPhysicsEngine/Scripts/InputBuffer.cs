using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class InputBuffer : Node {
    private const int NIL = 0; // No input, not necessarily Neutral input
    private int size;
    private Array<Variant> buffer; // May change this later.  Not sure if I want all inputs to be translated into ints
    public InputBuffer(int size) {
        this.size = size;
        buffer = new Array<Variant>();
        for ( int i = 0; i <= size - 1; i++ ) {
            buffer.Add(NIL);
        }
    }

    private enum DIRECTION {
        NIL = 0,
        DOWN_LEFT = 1,
        DOWN = 2,
        DOWN_RIGHT = 3,
        LEFT = 4,
        NEUTRAL = 5,
        RIGHT = 6,
        UP_LEFT = 7,
        UP = 8,
        UP_RIGHT = 9
    }

    public enum BUTTON {
        NIL = 0,
        ATTACK = 1, // X
        JUMP = 2, // Z
        CHARGE = 3 // F
    }

    private DIRECTION dir;

    // Returns the buffer itself containing Variant objects
    public Array<Variant> GetBuffer() { return buffer; }

    // Takes a generic input, translates it, and adds it to the buffer
    public void AddInput<T>(T input) {
        if (!(buffer.Count >= size)) {
            buffer.Add(TranslateInput(input));
        } else {
            buffer.RemoveAt(0);
            buffer.Add(TranslateInput(input));
        }
    }

    public void ClearBuffer() {
        for (int i = 0; i <= buffer.Count - 1; i++) {
            buffer[i] = NIL;
        }
        //GD.Print("Buffer cleared: " + buffer);
    }

    // Translates a generic input into an int.
    private int TranslateInput<T>(T input) {
        // Vector2 input
        if (input is Vector2) {
            object o = input;
            var i = (Vector2)o;

            // Temporary, just seeing if it is working as intended
            if (i == new Vector2(-1, 1)) {
                return (int)DIRECTION.DOWN_LEFT;
            }
            if (i == new Vector2(0, 1)) {
                return (int)DIRECTION.DOWN;
            }
            if (i == new Vector2(1, 1)) {
                return (int)DIRECTION.DOWN_RIGHT;
            }
            if (i == new Vector2(-1, 0)) {
                return (int)DIRECTION.LEFT;
            }
            if (i == new Vector2(0, 0)) {
                return (int)DIRECTION.NEUTRAL;
            }
            if (i == new Vector2(1, 0)) {
                return (int)DIRECTION.RIGHT;
            }
            if (i == new Vector2(-1, -1)) {
                return (int)DIRECTION.UP_LEFT;
            }
            if (i == new Vector2(0, -1)) {
                return (int)DIRECTION.UP;
            }
            if (i == new Vector2(1, -1)) {
                return (int)DIRECTION.UP_RIGHT;
            }

        }

        // StringName input
        // From Godot docs: StringNames are immutable strings designed for general-purpose representation of unique names.
        // This means that two StringNames with the same value are the same object (i.e. they are pointers to the same address),
        // which makes comparing them much faster than directly comparing primitive strings and String objects.
        if (input is StringName) {
            object o = input;
            var i = (StringName)o;

            if (i == new StringName("Attack")) {
                return (int)BUTTON.ATTACK;
            }

            if (i == new StringName("Jump")) {
                return (int)BUTTON.JUMP;
            }

            if (i == new StringName("Charge")) {
                return (int)BUTTON.CHARGE;
            }

        }
        return NIL;
    }

}
