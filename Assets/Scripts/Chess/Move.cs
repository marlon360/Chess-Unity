using UnityEngine;

public class Move {

    public Vector2Int start;
    public Vector2Int end;

    public Move(Vector2Int start, Vector2Int end) {
        this.start = start;
        this.end = end;
    }

    public override string ToString() {
        return "Move ("+start+", "+end+")";
    }

}