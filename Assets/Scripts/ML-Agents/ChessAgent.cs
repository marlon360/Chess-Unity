using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class ChessAgent : Agent {

    public Team team;

    private List<int> notSelectable = new List<int> ();
    private List<int> notMovable = new List<int> ();

    [Multiline (8)]
    public string DebugText;

    public void TileObservation (int x, int y) {
        Tile tile = GameManager.instance.grid.GetTile (x, y);
        if (tile.chessman == null) {
            notSelectable.Add (PositionToIndex (x, y));
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
            notSelectable.Add (PositionToIndex (x, y));
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
        notSelectable.Clear ();
        notMovable.Clear ();
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
        SetActionMask (0, notSelectable);
        SetActionMask (1, notMovable);
    }

    // to be implemented by the developer
    public override void AgentAction (float[] vectorAction, string textAction) {

        int chessmanPositionIndex = Mathf.FloorToInt (vectorAction[0]);
        Vector2 chessmanPosition = IndexToPosition (chessmanPositionIndex);
        Tile chessmanTile = GameManager.instance.grid.GetTile (chessmanPosition);

        if (chessmanTile.chessman != null && chessmanTile.chessman.team == team) {

            Chessman selectedChessman = chessmanTile.chessman;

            int destinationPositionIndex = Mathf.FloorToInt (vectorAction[1]);
            Vector2 destinationPosition = IndexToPosition (destinationPositionIndex);
            Tile destinationTile = GameManager.instance.grid.GetTile (destinationPosition);

            if (selectedChessman.CanAttackAt (destinationTile)) {
                // kill enemy at tile
                KillChessman (destinationTile.chessman);
                // move to this tile
                selectedChessman.SetTile (destinationTile);
                // change team
                GameManager.instance.ChangeTeam ();
            } else if (selectedChessman.CanMoveTo (destinationTile)) {
                // move to tile
                selectedChessman.SetTile (destinationTile);
                // negative reward
                AddReward (-0.002f);
                // change team
                GameManager.instance.ChangeTeam ();
            } else {
                AddReward (-0.005f);
                RequestDecision ();
            }

        } else {
            AddReward (-0.005f);
            RequestDecision ();
        }

    }

    public void KillChessman (Chessman chessman) {
        if (chessman.team == Team.White) {
            GameManager.instance.BlackAgent?.AddReward (chessman.reward);
            GameManager.instance.WhiteAgent?.AddReward (-chessman.reward + 0.5f);
        } else {
            GameManager.instance.WhiteAgent?.AddReward (chessman.reward);
            GameManager.instance.BlackAgent?.AddReward (-chessman.reward + 0.5f);
        }
        if (chessman.GetComponent<King> () != null) {
            GameManager.instance.GameOver (team);
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

}