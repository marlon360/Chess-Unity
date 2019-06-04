using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class ChessAgent : Agent {

    public Team team;

    private List<int> notSelectable = new List<int> ();
    private List<int> notMovable = new List<int> ();

    private ChessGame chessGame;

    [Multiline (8)]
    public string DebugText;

    public override void InitializeAgent () {
        chessGame = GetComponentInParent<ChessGame> ();
        chessGame.OnKilled.AddObserver (KillChessman);
        chessGame.OnGameOver.AddObserver (GameOver);
    }

    public List<int> CreateMask () {
        List<int> mask = new List<int> ();
        for (int i = 0; i < 64 * 64; i++) {
            mask.Add (i);
            // Move move = IndexToMove (i);
            // if (chessGame.GetChess().IsMoveValid (move)) {
            //     mask.Remove (i);
            // }
        }
        foreach (Move move in chessGame.GetChess ().GetValidMoves ()) {
            int index = ChessDecision.MoveToIndex (move);
            mask.Remove (index);
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
        foreach (Piece piece in chessGame.GetChess ().state) {
            AddVectorObs (PieceToInt (piece));
        }
        DebugVectorObservation ();
        SetActionMask (0, CreateMask ());
    }

    private int PieceToInt (Piece piece) {
        if (piece == null) {
            return 0;
        } else {
            int value = 0;
            if (piece is Pawn) {
                value = 1;
            } else if (piece is Rook) {
                value = 2;
            } else if (piece is Knight) {
                value = 3;
            } else if (piece is Bishop) {
                value = 4;
            } else if (piece is Queen) {
                value = 5;
            } else if (piece is King) {
                value = 6;
            }
            if (piece.team != team) {
                value *= -1;
            }
            return value;
        }
    }

    private void GameOver(Team winner) {
        if (team == winner) {
            AddReward(10f);
        } else {
            AddReward(-10f);
        }
        Done();
    }

    // to be implemented by the developer
    public override void AgentAction (float[] vectorAction, string textAction) {

        Chess chess = new Chess ();

        Move move = IndexToMove (Mathf.FloorToInt (vectorAction[0]));

        //float currentCanBeAttackedReward = CanBeAttackedReward ();

        if (chessGame.GetChess ().IsMoveValid (move)) {
            chessGame.MakeMove (move);
            //AddReward (CanBeAttackedReward () - currentCanBeAttackedReward);
        } else {
            Debug.Log (move);
            RequestDecision ();
        }
    }

    [ContextMenu ("RequestDecision")]
    public void RequestNewDecision () {
        RequestDecision ();
    }

    float CanBeAttackedReward () {
        float reward = 0;
        // negative reward if chessman can be attacked
        foreach (Piece piece in chessGame.GetChess().GetPiecesByTeam (team)) {
            if (piece.CanBeAttacked ()) {
                reward += -(PieceToReward(piece) / 10);
            }
        }
        // positive reward if an enemy chessman can be attacked
        foreach (Piece piece in chessGame.GetChess().GetPiecesByTeam (team == Team.White ? Team.Black : Team.White)) {
            if (piece.CanBeAttacked ()) {
                reward += (PieceToReward(piece) / 4);
            }
        }
        return reward;
    }

    public void KillChessman (Piece piece) {
        if (piece.team != team) {
            AddReward (PieceToReward(piece));
        } else {
            AddReward (-(PieceToReward(piece) + 0.2f));
        }
    }

    public float PieceToReward (Piece piece) {

        if (piece is Pawn) {
            return 2f;
        } else if (piece is Rook) {
            return 5f;
        } else if (piece is Knight) {
            return 3f;
        } else if (piece is Bishop) {
            return 3f;
        } else if (piece is Queen) {
            return 10f;
        } else if (piece is King) {
            return 20f;
        } else {
            return 0;
        }

    }

    // to be implemented by the developer
    public override void AgentReset () {
        chessGame.Reset();
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

    public Move IndexToMove (int index) {

        int x1 = Mathf.FloorToInt ((((index % 512) % 64) % 8));
        int y1 = Mathf.FloorToInt (((index % 512) % 64) / 8);
        int x2 = Mathf.FloorToInt ((index % 512) / 64);
        int y2 = Mathf.FloorToInt (index / 512);

        Move move = new Move (
            new Vector2Int (x1, y1),
            new Vector2Int (x2, y2)
        );

        // if (team == Team.Black) {
        //     move = MirrorMove(move);
        // }

        return move;

    }

    public Move MirrorMove (Move move) {

        Move mirroredMove = new Move (
            new Vector2Int (7 - move.start.x, 7 - move.start.y),
            new Vector2Int (7 - move.end.x, 7 - move.end.y)
        );
        return mirroredMove;
    }

}