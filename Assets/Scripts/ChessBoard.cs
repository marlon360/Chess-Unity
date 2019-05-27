using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour {

    public Team currentTeam = Team.White;
    public Chessman selectedChessman;

    public bool UseWhiteAgent = true;
    public ChessAgent WhiteAgent;
    public bool UseBlackAgent = true;
    public ChessAgent BlackAgent;

    public bool gameOver = false;

    public GameObject UI;

    public Subject<Chessman> OnChessmanKilled = new Subject<Chessman> ();

    private Grid grid;

    private Stack<ChessBoardHistoryEntry> history = new Stack<ChessBoardHistoryEntry> ();

    // Start is called before the first frame update
    void Start () {
        grid = GetComponentInChildren<Grid> ();

        if (!UseWhiteAgent) {
            WhiteAgent = null;
        }
        if (!UseBlackAgent) {
            BlackAgent = null;
        }
        Reset ();
    }

    public Tile GetTile (int x, int y) {
        return grid.GetTile (x, y);
    }
    public Tile GetTile (Vector2 pos) {
        return grid.GetTile (pos);
    }

    public List<int> ValidMoves () {
        List<int> mask = new List<int> ();
        for (int i = 0; i < 64 * 64; i++) {
            Tile[] tiles = IndexToTiles (i);
            Tile selectionTile = tiles[0];
            Tile destinationTile = tiles[1];
            if (IsMoveValid (selectionTile, destinationTile)) {
                mask.Add (i);
            }
        }
        return mask;
    }

    public int TileObservation (int x, int y) {

        Tile tile = GetTile (x, y);

        if (tile.chessman == null) {
            return (0);
        } else if (tile.chessman.team == currentTeam) {
            if (tile.chessman.GetComponent<Pawn> () != null) {
                return (1);
            } else if (tile.chessman.GetComponent<Rook> () != null) {
                return (2);
            } else if (tile.chessman.GetComponent<Knight> () != null) {
                return (3);
            } else if (tile.chessman.GetComponent<Bishop> () != null) {
                return (4);
            } else if (tile.chessman.GetComponent<Queen> () != null) {
                return (5);
            } else {
                return (6);
            }
        } else {
            if (tile.chessman.GetComponent<Pawn> () != null) {
                return (7);
            } else if (tile.chessman.GetComponent<Rook> () != null) {
                return (8);
            } else if (tile.chessman.GetComponent<Knight> () != null) {
                return (9);
            } else if (tile.chessman.GetComponent<Bishop> () != null) {
                return (10);
            } else if (tile.chessman.GetComponent<Queen> () != null) {
                return (11);
            } else {
                return (12);
            }
        }
    }

    public List<int> GetObservation () {
        List<int> observation = new List<int> ();
        if (currentTeam == Team.White) {
            for (int y = 0; y < 8; y++) {
                for (int x = 0; x < 8; x++) {
                    observation.Add (TileObservation (x, y));
                }
            }
        } else {
            for (int y = 7; y >= 0; y--) {
                for (int x = 7; x >= 0; x--) {
                    observation.Add (TileObservation (x, y));
                }
            }
        }
        return observation;
    }

    public bool IsMoveValid (Tile selectionTile, Tile destinationTile) {
        if (selectionTile.chessman == null) {
            return false;
        }
        if (selectionTile.chessman.team != currentTeam) {
            return false;
        }
        if (!selectionTile.chessman.CanAttackAt (destinationTile) && !selectionTile.chessman.CanMoveTo (destinationTile)) {
            return false;
        }
        return true;
    }

    public bool MakeMove (Tile selectionTile, Tile destinationTile, bool requestDecision = true, bool noAnimation = false) {

        if (selectionTile && selectionTile.chessman != null && selectionTile.chessman.team == currentTeam) {
            Chessman selectedChessman = selectionTile.chessman;
            if (selectedChessman.CanAttackAt (destinationTile)) {
                GameObject undoKilled = Instantiate (destinationTile.chessman.gameObject);
                undoKilled.SetActive (false);
                Tile undoSelection = destinationTile;
                Tile undoDestination = selectionTile;
                bool undoFirstMove = false;
                if (selectedChessman.GetComponent<Pawn> () && selectedChessman.GetComponent<Pawn> ().firstMove) {
                    undoFirstMove = true;
                }

                ChessBoardHistoryEntry historyEntry = new ChessBoardHistoryEntry (undoSelection, undoDestination, undoFirstMove, undoKilled);
                history.Push (historyEntry);

                // kill enemy at tile
                KillChessman (destinationTile.chessman);
                // move to this tile
                selectedChessman.SetTile (destinationTile, (Chessman chessman) => {
                    ChangeTeam (requestDecision);
                }, noAnimation);
            } else if (selectedChessman.CanMoveTo (destinationTile)) {

                Tile undoSelection = destinationTile;
                Tile undoDestination = selectionTile;
                bool undoFirstMove = false;
                if (selectedChessman.GetComponent<Pawn> () && selectedChessman.GetComponent<Pawn> ().firstMove) {
                    undoFirstMove = true;
                }

                ChessBoardHistoryEntry historyEntry = new ChessBoardHistoryEntry (undoSelection, undoDestination, undoFirstMove, null);
                history.Push (historyEntry);

                // move to tile
                selectedChessman.SetTile (destinationTile, (Chessman chessman) => {
                    ChangeTeam (requestDecision);
                }, noAnimation);
            } else {
                return false;
            }
        } else {
            return false;
        }
        return true;
    }

    public bool MakeMove (int move, bool requestDecision, bool noAnimation = false) {
        Tile[] tiles = IndexToTiles (move);
        return MakeMove (tiles[0], tiles[1], requestDecision, noAnimation);
    }

    [ContextMenu ("undo")]
    public void Undo () {
        if (history.Count > 0) {
            gameOver = false;
            ChessBoardHistoryEntry undoStep = history.Pop ();
            undoStep.undoSelection.chessman.SetTile (undoStep.undoDestination, null, true);
            if (undoStep.undoDestination.chessman.GetComponent<Pawn> () != null) {
                undoStep.undoDestination.chessman.GetComponent<Pawn> ().firstMove = undoStep.undoFirstMove;
            }
            if (undoStep.undoKilled != null) {
                undoStep.undoKilled.transform.parent = grid.transform;
                undoStep.undoKilled.SetActive (true);
                undoStep.undoSelection.chessman = undoStep.undoKilled.GetComponent<Chessman>(); 
                undoStep.undoKilled.GetComponent<Chessman>().SetChessBoard(this);
            }
            ChangeTeam (false);
        }
    }

    public void ClearHistory() {
        while (history.Count > 0) {
            ChessBoardHistoryEntry undoStep = history.Pop ();
            if (undoStep.undoKilled != null) {
                Destroy(undoStep.undoKilled);
            }
        }
        history = new Stack<ChessBoardHistoryEntry>();
    }

    public List<Chessman> GetChessmenByTeam (Team team) {
        return grid.GetChessmenByTeam (team);
    }

    public void SelectChessman (Chessman chessman) {
        selectedChessman = chessman;
        chessman.Select ();
        chessman.HighlightMoveToTiles ();
        chessman.HighlightAttackAtTiles ();
    }

    public void DeselectChessman () {
        selectedChessman.Deselect ();
        selectedChessman = null;
        grid.DeselectAllTiles ();
    }

    void Update () {
        if (Input.GetMouseButtonDown (0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit, 100.0f)) {

                GameObject hittedObject = hit.transform.gameObject;
                Chessman chessman = hittedObject.GetComponent<Chessman> ();
                Tile tile = hittedObject.GetComponent<Tile> ();
                if (chessman && chessman.GetChessBoard () == this) {
                    handleChessmanClick (chessman);
                }
                if (tile) {
                    handleTileClick (tile);
                }

            }
        }
    }

    private void handleChessmanClick (Chessman chessman) {
        if (selectedChessman == null) {
            if (chessman.team == currentTeam) {
                SelectChessman (chessman);
            }
        } else {
            if (selectedChessman == chessman) {
                DeselectChessman ();
            }
            if (chessman.team != currentTeam) {
                if (MakeMove (selectedChessman.currentTile, chessman.currentTile)) {
                    DeselectChessman ();
                }
            }
        }
    }

    public void handleTileClick (Tile tile) {
        if (selectedChessman == null) {
            if (tile.chessman != null) {
                handleChessmanClick (tile.chessman);
            }
        } else {
            if (MakeMove (selectedChessman.currentTile, tile)) {
                DeselectChessman ();
            };
        }
    }

    public void KillChessman (Chessman chessman) {
        Debug.Log("Kill");
        OnChessmanKilled.Notify (chessman);
        chessman.Kill ();
        if (chessman.GetComponent<King> () != null) {
            GameOver (currentTeam);
        }
    }

    public void ChangeTeam (bool requestDecision = true) {
        if (!gameOver) {
            if (currentTeam == Team.Black) {
                currentTeam = Team.White;
                if (requestDecision) {
                    WhiteAgent?.RequestDecision ();
                }
            } else {
                currentTeam = Team.Black;
                if (requestDecision) {
                    BlackAgent?.RequestDecision ();
                }
            }
        }
    }

    public void SetTeam (Team team, bool requestDecision = true) {
        if (team != Team.Black) {
            currentTeam = Team.White;
            if (requestDecision) {
                WhiteAgent?.RequestDecision ();
            }
        } else {
            currentTeam = Team.Black;
            if (requestDecision) {
                BlackAgent?.RequestDecision ();
            }
        }
    }

    public void GameOver (Team winner) {
        gameOver = true;
        if (winner == Team.Black) {
            UI.GetComponentInChildren<Text> ().text = "Black wins";
            BlackAgent?.AddReward (10);
            WhiteAgent?.AddReward (-10);
        } else {
            UI.GetComponentInChildren<Text> ().text = "White wins";
            WhiteAgent?.AddReward (10);
            BlackAgent?.AddReward (-10);
        }
        BlackAgent?.Done ();
        WhiteAgent?.Done ();
        UI.SetActive (true);
        StartCoroutine (ResetAfterDelay (5f));
    }

    IEnumerator ResetAfterDelay (float sec) {
        yield return new WaitForSeconds (sec);
        Reset ();
    }

    public Tile[] IndexToTiles (int index) {

        int x1 = Mathf.FloorToInt ((((index % 512) % 64) % 8));
        int y1 = Mathf.FloorToInt (((index % 512) % 64) / 8);
        int x2 = Mathf.FloorToInt ((index % 512) / 64);
        int y2 = Mathf.FloorToInt (index / 512);

        Tile tile1 = GetTile (x1, y1);
        Tile tile2 = GetTile (x2, y2);

        if (currentTeam == Team.Black) {
            tile1 = MirroredTile (tile1);
            tile2 = MirroredTile (tile2);
        }

        return new Tile[] { tile1, tile2 };

    }

    public Tile MirroredTile (Tile tile) {

        int x = 7 - Mathf.FloorToInt (tile.position.x);
        int y = 7 - Mathf.FloorToInt (tile.position.y);

        return GetTile (x, y);

    }

    public void Reset () {
        UI.SetActive (false);
        foreach (Transform child in grid.gameObject.transform) {
            Destroy (child.gameObject);
        }
        grid.Setup ();
        currentTeam = Team.White;
        gameOver = false;
        WhiteAgent?.RequestDecision ();
    }

}