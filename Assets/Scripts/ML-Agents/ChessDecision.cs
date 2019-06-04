using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class ChessDecision : Decision {

    public ChessGame chessGame;
    public Team currentTeam;
    public int depth = 3;
    private int positionCount = 0;
    private ChessAcademy academy;

    /// <summary>
    /// Defines the decision-making logic of the agent. Given the information 
    /// about the agent, returns a vector of actions.
    /// </summary>
    /// <returns>Vector action vector.</returns>
    /// <param name="vectorObs">The vector observations of the agent.</param>
    /// <param name="visualObs">The cameras the agent uses for visual observations.</param>
    /// <param name="reward">The reward the agent received at the previous step.</param>
    /// <param name="done">Whether or not the agent is done.</param>
    /// <param name="memory">
    /// The memories stored from the previous step with 
    /// <see cref="MakeMemory(List{float}, List{Texture2D}, float, bool, List{float})"/>
    /// </param>
    public override float[] Decide (List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory) {
        if (chessGame == null) {
            chessGame = FindObjectOfType<ChessGame> ();
        }
        if (academy == null) {
            academy = FindObjectOfType<ChessAcademy> ();
        } else {
            if (academy.resetParameters.ContainsKey("ai-depth")) {
                depth =  Mathf.FloorToInt(academy.resetParameters["ai-depth"]);
            }
        }
        currentTeam = chessGame.GetChess ().currentTeam;

        Move bestmove = ChessAI.GetBestMove (ObservationToChess(vectorObs), depth);

        float[] act = new float[brainParameters.vectorActionSize.Length];
        act[0] = MoveToIndex (bestmove);
        chessGame.GetChess ().currentTeam = currentTeam;

        return act;
    }

    /// <summary>
    /// Defines the logic for creating the memory vector for the Agent.
    /// </summary>
    /// <returns>The vector of memories the agent will use at the next step.</returns>
    /// <param name="vectorObs">The vector observations of the agent.</param>
    /// <param name="visualObs">The cameras the agent uses for visual observations.</param>
    /// <param name="reward">The reward the agent received at the previous step.</param>
    /// <param name="done">Whether or not the agent is done.</param>
    /// <param name="memory">
    /// The memories stored from the previous call to this method.
    /// </param>
    public override List<float> MakeMemory (List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory) {
        return new List<float> ();
    }

    public static int MoveToIndex (Move move) {

        int x1 = move.start.x;
        int y1 = move.start.y;
        int x2 = move.end.x;
        int y2 = move.end.y;

        int index = x1 + y1 * 8 + x2 * 8 * 8 + y2 * 8 * 8 * 8;
        return index;

    }

    private Piece IntToPiece (int i, Chess chess, Team currentTeam) {
        if (i == 0) {
            return null;
        } else {
            Team pieceTeam = currentTeam;
            if (i < 0) {
                pieceTeam = currentTeam == Team.Black ? Team.White : Team.Black;
                i *= -1;
            }
            if (i == 1) {
                return new Pawn (chess, pieceTeam);
            } else if (i == 2) {
                return new Rook (chess, pieceTeam);
            } else if (i == 3) {
                return new Knight (chess, pieceTeam);
            } else if (i == 4) {
                return new Bishop (chess, pieceTeam);
            } else if (i == 5) {
                return new Queen (chess, pieceTeam);
            } else {
                return new King (chess, pieceTeam);
            }
        }
    }

    public Chess ObservationToChess (List<float> observation) {
        Chess chess = new Chess();
        chess.currentTeam = currentTeam;
        chess.state = new Piece[8,8];
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                int pieceInt = Mathf.FloorToInt(observation[y * 8 + x]);
                Piece piece = IntToPiece(pieceInt, chess, currentTeam);
                if (piece != null) {
                    piece.position = new Vector2Int(x, 7 - y);
                }
                chess.state[y, x] = piece;
            }
        }
        return chess;
    }

    public void DebugVectorObservation (Piece[,] state) {
        string DebugText = "";
            for (int y = 7; y >= 0; y--) {
                for (int x = 0; x < 8; x++) {
                    if (state[x,y] == null) {
                        DebugText += " ---- ";
                    } else {
                        DebugText += state[x,y] + " ("+ state[x,y].position+") ";
                    } 
                }
                DebugText += "\n";
            }
        
        Debug.Log(DebugText);
    }


}