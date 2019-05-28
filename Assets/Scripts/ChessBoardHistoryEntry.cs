using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardHistoryEntry {

    public Tile undoSelection;
    public Tile undoDestination;
    public GameObject undoKilled;

    public ChessBoardHistoryEntry (Tile undoSelection, Tile undoDestination, GameObject undoKilled) {
        this.undoDestination = undoDestination;
        this.undoSelection = undoSelection;
        this.undoKilled = undoKilled;
    }

}