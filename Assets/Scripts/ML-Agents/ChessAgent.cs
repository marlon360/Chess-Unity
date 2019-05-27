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
        chessBoard.OnChessmanKilled.AddObserver(KillChessman);
    }

    public List<int> CreateMask () {
        List<int> mask = new List<int> ();
        for (int i = 0; i < 64 * 64; i++) {
            mask.Add (i);
            Tile[] tiles = IndexToTiles (i);
            Tile selectionTile = tiles[0];
            Tile destinationTile = tiles[1];
            if (chessBoard.IsMoveValid (selectionTile, destinationTile)) {
                mask.Remove (i);
            }
        }
        return mask;
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
        chessBoard.GetObservation ().ForEach ((int i) => {
            AddVectorObs (i);
        });
        DebugVectorObservation ();
        SetActionMask (0, CreateMask ());
    }

    // to be implemented by the developer
    public override void AgentAction (float[] vectorAction, string textAction) {

        Tile[] tiles = IndexToTiles (Mathf.FloorToInt (vectorAction[0]));

        Tile chessmanTile = tiles[0];
        Tile destinationTile = tiles[1];
        float currentCanBeAttackedReward = CanBeAttackedReward ();

        if (chessBoard.MakeMove (chessmanTile, destinationTile)) {
            AddReward (CanBeAttackedReward () - currentCanBeAttackedReward);
        } else {
            Debug.Log("CANNOT MOVE "+chessmanTile.name+ " Team: "+chessBoard.currentTeam);
            //RequestDecision ();
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