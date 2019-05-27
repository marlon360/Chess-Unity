using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class ChessAgent : Agent {

    public Team team;

    private List<int> notSelectable = new List<int> ();
    private List<int> notMovable = new List<int> ();

    private ChessBoard chessBoard;

    [Multiline (8)]
    public string DebugText;

    public override void InitializeAgent () {
        chessBoard = GetComponentInParent<ChessBoard> ();
    }

    public List<int> CreateMask () {
        List<int> mask = new List<int> ();
        for (int i = 0; i < 64 * 64; i++) {
            mask.Add (i);
            Tile[] tiles = IndexToTiles (i);
            Tile selectionTile = tiles[0];
            Tile destinationTile = tiles[1];
            if (IsMoveValid (selectionTile, destinationTile)) {
                mask.Remove (i);
            }
        }
        return mask;
    }

    private bool IsMoveValid (Tile selectionTile, Tile destinationTile) {
        if (selectionTile.chessman == null) {
            return false;
        }
        if (selectionTile.chessman.team != team) {
            return false;
        }
        if (!selectionTile.chessman.CanAttackAt (destinationTile) && !selectionTile.chessman.CanMoveTo (destinationTile)) {
            return false;
        }
        return true;
    }

    public void TileObservation (int x, int y) {

        Tile tile = chessBoard.GetTile (x, y);
        if (tile.chessman == null) {
            AddVectorObs (0);
        } else if (tile.chessman.team == team) {
            notMovable.Add (PositionToIndex (x, y));
            if (tile.chessman.GetComponent<Pawn> () != null) {
                AddVectorObs (1);
            } else if (tile.chessman.GetComponent<Rook> () != null) {
                AddVectorObs (2);
            } else if (tile.chessman.GetComponent<Knight> () != null) {
                AddVectorObs (3);
            } else if (tile.chessman.GetComponent<Bishop> () != null) {
                AddVectorObs (4);
            } else if (tile.chessman.GetComponent<Queen> () != null) {
                AddVectorObs (5);
            } else if (tile.chessman.GetComponent<King> () != null) {
                AddVectorObs (6);
            }
        } else {
            if (tile.chessman.GetComponent<Pawn> () != null) {
                AddVectorObs (7);
            } else if (tile.chessman.GetComponent<Rook> () != null) {
                AddVectorObs (8);
            } else if (tile.chessman.GetComponent<Knight> () != null) {
                AddVectorObs (9);
            } else if (tile.chessman.GetComponent<Bishop> () != null) {
                AddVectorObs (10);
            } else if (tile.chessman.GetComponent<Queen> () != null) {
                AddVectorObs (11);
            } else if (tile.chessman.GetComponent<King> () != null) {
                AddVectorObs (12);
            }
        }
        DebugVectorObservation ();
    }

    public void DebugVectorObservation () {
        DebugText = "";
        if (info.vectorObservation.Count == 64) {
            for (int y = 7; y >= 0; y--) {
                for (int x = 0; x < 8; x++) {
                    int index = y * 8 + x;
                    DebugText += info.vectorObservation[index] + " ";
                }
                DebugText += "\n";
            }
        }
    }

    public override void CollectObservations () {
        if (team == Team.White) {
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    TileObservation (y, x);
                }
            }
        } else {
            for (int x = 7; x >= 0; x--) {
                for (int y = 7; y >= 0; y--) {
                    TileObservation (y, x);
                }
            }
        }
        SetActionMask (0, CreateMask ());
    }

    // to be implemented by the developer
    public override void AgentAction (float[] vectorAction, string textAction) {

        Tile[] tiles = IndexToTiles (Mathf.FloorToInt (vectorAction[0]));

        Tile chessmanTile = tiles[0];

        if (chessmanTile.chessman != null && chessmanTile.chessman.team == team) {

            float currentCanBeAttackedReward = CanBeAttackedReward();

            Chessman selectedChessman = chessmanTile.chessman;
            Tile destinationTile = tiles[1];

            if (selectedChessman.CanAttackAt (destinationTile)) {
                // kill enemy at tile
                KillChessman (destinationTile.chessman);
                // move to this tile
                selectedChessman.SetTile (destinationTile);
                AddReward(CanBeAttackedReward () - currentCanBeAttackedReward);
            } else if (selectedChessman.CanMoveTo (destinationTile)) {
                // move to tile
                selectedChessman.SetTile (destinationTile);
                AddReward(CanBeAttackedReward () - currentCanBeAttackedReward);
            } else {
                //Debug.Log ("Cannot move");
                AddReward (-0.005f);
                RequestDecision ();
            }

        } else {
            //Debug.Log ("Team "+team+": Cannot select " + chessmanTile.position.x + ", " + chessmanTile.position.y);
            AddReward (-0.005f);
            RequestDecision ();
        }

    }

    float CanBeAttackedReward () {
        float reward = 0;
        // negative reward if chessman can be attacked
        foreach (Chessman chessman in chessBoard.GetChessmenByTeam (team)) {
            if (chessman.CanBeAttacked ()) {
                reward += -(chessman.reward / 4);
            }
        }
        // positive reward if an enemy chessman can be attacked
        foreach (Chessman chessman in chessBoard.GetChessmenByTeam (team == Team.White ? Team.Black : Team.White)) {
            if (chessman.CanBeAttacked ()) {
                reward += (chessman.reward / 4);
            }
        }
        return reward;
    }

    public void KillChessman (Chessman chessman) {
        if (chessman.team == Team.White) {
            chessBoard.BlackAgent?.AddReward (chessman.reward);
            chessBoard.WhiteAgent?.AddReward (-(chessman.reward + 0.5f));
        } else {
            chessBoard.WhiteAgent?.AddReward (chessman.reward);
            chessBoard.BlackAgent?.AddReward (-(chessman.reward + 0.5f));
        }
        if (chessman.GetComponent<King> () != null) {
            chessBoard.GameOver (team);
        } else {
            chessman.Kill ();
        }
    }

    // to be implemented by the developer
    public override void AgentReset () {

    }

    private Vector2 IndexToPosition (int index) {
        int x = index % 8;
        int y = Mathf.FloorToInt (index / 8);
        if (team == Team.Black) {
            x = 7 - x;
            y = 7 - y;
        }
        return new Vector2 (x, y);
    }

    private int PositionToIndex (int xPos, int yPos) {
        if (team == Team.Black) {
            xPos = 7 - xPos;
            yPos = 7 - yPos;
        }
        int x = xPos;
        int y = yPos * 8;
        return x + y;
    }

    public int TilesToIndex (Tile tile1, Tile tile2) {

        int x1 = Mathf.FloorToInt (tile1.position.x);
        int x2 = Mathf.FloorToInt (tile1.position.y);
        int y1 = Mathf.FloorToInt (tile2.position.x);
        int y2 = Mathf.FloorToInt (tile2.position.y);

        int index = x1 + y1 * 8 + x2 * 8 + 8 + y2 * 8 * 8 * 8;
        return index;

    }

    public Tile[] IndexToTiles (int index) {

        int x1 = Mathf.FloorToInt ((((index % 512) % 64) % 8));
        int y1 = Mathf.FloorToInt (((index % 512) % 64) / 8);
        int x2 = Mathf.FloorToInt ((index % 512) / 64);
        int y2 = Mathf.FloorToInt (index / 512);

        Tile tile1 = chessBoard.GetTile (x1, y1);
        Tile tile2 = chessBoard.GetTile (x2, y2);

        if (team == Team.Black) {
            tile1 = MirroredTile (tile1);
            tile2 = MirroredTile (tile2);
        }

        return new Tile[] { tile1, tile2 };

    }

    public Tile MirroredTile (Tile tile) {

        int x = 7 - Mathf.FloorToInt (tile.position.x);
        int y = 7 - Mathf.FloorToInt (tile.position.y);

        return chessBoard.GetTile (x, y);

    }

}