using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class ChessDecision : Decision {

    public ChessBoard chessBoard;
    public Team currentTeam;
    private int positionCount = 0;

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
        if (chessBoard == null) {
            chessBoard = FindObjectOfType<ChessBoard> ();
        }
        currentTeam = chessBoard.currentTeam;

        int bestmove = GetBestMove (chessBoard);

        float[] act = new float[brainParameters.vectorActionSize.Length];
        act[0] = bestmove;
        chessBoard.SetTeam(currentTeam, false);
        chessBoard.ClearHistory();

        //chessBoard.SetTeam(currentTeam == Team.White ? Team.Black : Team.White, false);

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

    public float CalculateValue (ChessBoard chessBoard) {
        float result = 0;
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                Tile tile = chessBoard.GetTile (x, y);
                if (tile.chessman == null) {
                    result += 0;
                } else {
                    float multiplier = tile.chessman.team != currentTeam ? 1.0f : -1.0f;
                    if (tile.chessman.GetComponent<Pawn> () != null) {
                        result += 10 * multiplier;
                    } else if (tile.chessman.GetComponent<Rook> () != null) {
                        result += 50 * multiplier;
                    } else if (tile.chessman.GetComponent<Knight> () != null) {
                        result += 30 * multiplier;
                    } else if (tile.chessman.GetComponent<Bishop> () != null) {
                        result += 30 * multiplier;
                    } else if (tile.chessman.GetComponent<Queen> () != null) {
                        result += 90 * multiplier;
                    } else if (tile.chessman.GetComponent<King> () != null) {
                        result += 900 * multiplier;
                    }
                }
            }
        }
        return result;
    }

    int MinimaxRoot (int depth, ChessBoard game, bool isMaximisingPlayer) {

        List<int> newGameMoves = game.ValidMoves ();
        float bestMove = -9999;
        int bestMoveFound = 0;

        for (int i = 0; i < newGameMoves.Count; i++) {
            int newGameMove = newGameMoves[i];
            if (game.MakeMove (newGameMove, false, true)) {
                float value = Minimax (depth - 1, game, !isMaximisingPlayer);
                game.Undo ();
                if (value >= bestMove) {
                    bestMove = value;
                    bestMoveFound = newGameMove;
                }
            }
        }
        return bestMoveFound;
    }

    float Minimax (int depth, ChessBoard game, bool isMaximisingPlayer) {
        positionCount++;
        if (depth == 0) {
            return -1.0f * CalculateValue (game);
        }

        List<int> newGameMoves = game.ValidMoves ();

        if (isMaximisingPlayer) {
            float bestMove = -9999;
            for (int i = 0; i < newGameMoves.Count; i++) {
                if (game.MakeMove (newGameMoves[i], false, true)) {
                    bestMove = Mathf.Max (bestMove, Minimax (depth - 1, game, !isMaximisingPlayer));
                    game.Undo ();
                }
            }
            return bestMove;
        } else {
            float bestMove = 9999;
            for (var i = 0; i < newGameMoves.Count; i++) {
                if (game.MakeMove (newGameMoves[i], false, true)) {
                    bestMove = Mathf.Min (bestMove, Minimax (depth - 1, game, !isMaximisingPlayer));
                    game.Undo ();
                }
            }
            return bestMove;
        }
    }

    int GetBestMove (ChessBoard game, int depth = 2) {
        positionCount = 0;
        int bestMove = MinimaxRoot (depth, game, true);
        return bestMove;
    }

}