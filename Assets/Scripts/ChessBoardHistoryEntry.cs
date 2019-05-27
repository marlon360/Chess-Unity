using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardHistoryEntry {

    public Tile undoSelection;
    public Tile undoDestination;
    public bool undoFirstMove;
    public GameObject undoKilled;

    public ChessBoardHistoryEntry (Tile undoSelection, Tile undoDestination, bool undoFirstMove, GameObject undoKilled) {
        this.undoDestination = undoDestination;
        this.undoSelection = undoSelection;
        this.undoFirstMove = undoFirstMove;
        this.undoKilled = undoKilled;
    }

}