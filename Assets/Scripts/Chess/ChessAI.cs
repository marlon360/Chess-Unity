using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChessAI {

    static Move minimaxRoot (int depth, Chess game, bool isMaximisingPlayer, Team currentTeam) {

        Move[] newGameMoves = game.GetValidMoves ();
        float bestMove = -9999;
        Move bestMoveFound = null;

        for (var i = 0; i < newGameMoves.Length; i++) {
            Move newGameMove = newGameMoves[i];
            if (game.IsMoveValid (newGameMove)) {
                game.MakeMove (newGameMove);
                float value = minimax (depth - 1, game, -10000, 10000, !isMaximisingPlayer, currentTeam);
                game.Undo ();
                if (value >= bestMove) {
                    bestMove = value;
                    bestMoveFound = newGameMove;
                }
            }
        }
        return bestMoveFound;
    }

    static float minimax (int depth, Chess game, float alpha, float beta, bool isMaximisingPlayer, Team currentTeam) {
        if (depth == 0) {
            return -1 * evaluateBoard (game, currentTeam);
        }

        Move[] newGameMoves = game.GetValidMoves ();

        if (isMaximisingPlayer) {
            float bestMove = -9999;
            for (var i = 0; i < newGameMoves.Length; i++) {
                if (game.IsMoveValid (newGameMoves[i])) {
                    game.MakeMove (newGameMoves[i]);
                    bestMove = Mathf.Max (bestMove, minimax (depth - 1, game, alpha, beta, !isMaximisingPlayer, currentTeam));
                    game.Undo ();
                    alpha = Mathf.Max (alpha, bestMove);
                    if (beta <= alpha) {
                        return bestMove;
                    }
                }
            }
            return bestMove;
        } else {
            float bestMove = 9999;
            for (var i = 0; i < newGameMoves.Length; i++) {
                if (game.IsMoveValid (newGameMoves[i])) {
                    game.MakeMove (newGameMoves[i]);
                    bestMove = Mathf.Min (bestMove, minimax (depth - 1, game, alpha, beta, !isMaximisingPlayer, currentTeam));
                    game.Undo ();
                    beta = Mathf.Min (beta, bestMove);
                    if (beta <= alpha) {
                        return bestMove;
                    }
                }
            }
            return bestMove;
        }
    }

    static float evaluateBoard (Chess game, Team currentTeam) {
        float totalEvaluation = 0;
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                totalEvaluation = totalEvaluation + getPieceValue (game.state[x, y], currentTeam);
            }
        }
        return totalEvaluation;
    }

    static float getPieceValue (Piece piece, Team currentTeam) {
        float value = 0;
        if (piece == null) {
            value = 0;
        } else {
            if (piece is Pawn) {
                value = 30;
            } else if (piece is Knight) {
                value = 30;
            } else if (piece is Bishop) {
                value = 30;
            } else if (piece is Rook) {
                value = 50;
            } else if (piece is Queen) {
                value = 90;
            } else if (piece is King) {
                value = 900;
            }
            float multiplier = currentTeam != piece.team ? 1f : -1f;
            value *= multiplier;
        }
        return value;
    }

    public static Move GetBestMove (Chess game, int depth = 2) {
        game.ClearHistory();
        Move bestMove = minimaxRoot (depth, game, true, game.currentTeam);
        return bestMove;
    }

}