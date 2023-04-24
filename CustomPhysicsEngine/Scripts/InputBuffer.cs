using Godot;
using Godot.Collections;
using System;
using System.Collections;

public partial class InputBuffer : Node {
    private int size;
    private Array<int> buffer;
    public InputBuffer(int size) {
        this.size = size;
        buffer = new Array<int>();
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

    private DIRECTION dir;
    public Array<int> GetBuffer() { return buffer; }
    public void AddInput(Vector2 input) {
        if (!(buffer.Count >= size)) {
            buffer.Add(TranslateInput(input));
        } else {
            buffer.RemoveAt(0);
            buffer.Add(TranslateInput(input));
        }
    }

    private int TranslateInput(Vector2 input) {
        if (input == new Vector2(-1, 1)) {
            return (int)DIRECTION.DOWN_LEFT;
        }
        if (input == new Vector2(0, 1)) {
            return (int)DIRECTION.DOWN;
        }
        if (input == new Vector2(1, 1)) {
            return (int)DIRECTION.DOWN_RIGHT;
        }
        if (input == new Vector2(-1, 0)) {
            return (int)DIRECTION.LEFT;
        }
        if (input == new Vector2(0, 0)) {
            return (int)DIRECTION.NEUTRAL;
        }
        if (input == new Vector2(1, 0)) {
            return (int)DIRECTION.RIGHT;
        }
        if (input == new Vector2(-1, -1)) {
            return (int)DIRECTION.UP_LEFT;
        }
        if (input == new Vector2(0, -1)) {
            return (int)DIRECTION.UP;
        }
        if (input == new Vector2(1, -1)) {
            return (int)DIRECTION.UP_RIGHT;
        }
        return (int)DIRECTION.NIL;
    }

}
