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


    private Tile undoSelection;
    private Tile undoDestination;
    private bool undoFirstMove;
    private GameObject undoKilled;

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

    public bool MakeMove (Tile selectionTile, Tile destinationTile) {

        if (selectionTile && selectionTile.chessman != null && selectionTile.chessman.team == currentTeam) {

            Chessman selectedChessman = selectionTile.chessman;

            if (selectedChessman.CanAttackAt (destinationTile)) {

                if (undoKilled != null) {
                    Destroy(undoKilled);
                    undoKilled = null;
                }
                undoKilled = Instantiate(destinationTile.chessman.gameObject);
                undoKilled.SetActive(false);
                undoSelection = destinationTile;
                undoDestination = selectionTile;
                if (selectedChessman.GetComponent<Pawn>() && selectedChessman.GetComponent<Pawn>().firstMove) {
                    undoFirstMove = true;
                } else {
                    undoFirstMove = false;
                }

                // kill enemy at tile
                KillChessman (destinationTile.chessman);
                // move to this tile
                selectedChessman.SetTile (destinationTile, (Chessman chessman) => {
                    ChangeTeam ();
                });
            } else if (selectedChessman.CanMoveTo (destinationTile)) {
                if (undoKilled != null) {
                    Destroy(undoKilled);
                    undoKilled = null;
                }
                undoSelection = destinationTile;
                undoDestination = selectionTile;
                if (selectedChessman.GetComponent<Pawn>() && selectedChessman.GetComponent<Pawn>().firstMove) {
                    undoFirstMove = true;
                } else {
                    undoFirstMove = false;
                }
                // move to tile
                selectedChessman.SetTile (destinationTile, (Chessman chessman) => {
                    ChangeTeam ();
                });
            } else {
                return false;
            }
        } else {
            return false;
        }
        return true;
    }

    [ContextMenu("undo")]
    public void Undo() {
        if (undoKilled) {
            undoKilled.SetActive(true);
        }
        undoSelection.chessman.SetTile(undoDestination, null, true);
        if (undoDestination.chessman.GetComponent<Pawn>() != null) {
            undoDestination.chessman.GetComponent<Pawn>().firstMove = undoFirstMove;
        } 
        ChangeTeam();
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
                if (MakeMove(selectedChessman.currentTile, chessman.currentTile)) {
                    DeselectChessman();
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
            if (MakeMove(selectedChessman.currentTile, tile)) {
                DeselectChessman();
            };
        }
    }

    public void KillChessman (Chessman chessman) {
        OnChessmanKilled.Notify (chessman);
        chessman.Kill ();
        if (chessman.GetComponent<King> () != null) {
            GameOver (currentTeam);
        }
    }

    public void ChangeTeam () {
        if (!gameOver) {
            if (currentTeam == Team.Black) {
                currentTeam = Team.White;
                WhiteAgent?.RequestDecision ();
            } else {
                currentTeam = Team.Black;
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