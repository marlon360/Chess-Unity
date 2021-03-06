﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess {

    public Team currentTeam = Team.White;
    public Piece[, ] state = new Piece[8, 8];

    private Stack<ChessHistory> history = new Stack<ChessHistory> ();
    private int movesWithoutKill = 0;
    private bool gameOver = false;

    void Start () {
        currentTeam = Team.White;
        gameOver = false;
        StartFormation ();
    }

    public void StartFormation () {
        for (int i = 0; i < 8; i++) {
            SetPiece (new Pawn (this, Team.White), i, 1);
            SetPiece (new Pawn (this, Team.Black), i, 6);
        }

        SetPiece (new Rook (this, Team.White), 0, 0);
        SetPiece (new Rook (this, Team.Black), 0, 7);

        SetPiece (new Knight (this, Team.White), 1, 0);
        SetPiece (new Knight (this, Team.Black), 1, 7);

        SetPiece (new Bishop (this, Team.White), 2, 0);
        SetPiece (new Bishop (this, Team.Black), 2, 7);

        SetPiece (new Queen (this, Team.White), 3, 0);
        SetPiece (new Queen (this, Team.Black), 4, 7);

        SetPiece (new King (this, Team.White), 4, 0);
        SetPiece (new King (this, Team.Black), 3, 7);

        SetPiece (new Bishop (this, Team.White), 5, 0);
        SetPiece (new Bishop (this, Team.Black), 5, 7);

        SetPiece (new Knight (this, Team.White), 6, 0);
        SetPiece (new Knight (this, Team.Black), 6, 7);

        SetPiece (new Rook (this, Team.White), 7, 0);
        SetPiece (new Rook (this, Team.Black), 7, 7);
    }

    public Piece GetPiece (int x, int y) {
        return state[7 - y, x];
    }
    public Piece GetPiece (Vector2Int pos) {
        return GetPiece (pos.x, pos.y);
    }
    public void SetPiece (Piece piece, int x, int y) {
        state[7 - y, x] = piece;
        if (piece != null) {
            piece.position = new Vector2Int (x, y);
        }
    }
    public void SetPiece (Piece piece, Vector2Int pos) {
        SetPiece (piece, pos.x, pos.y);
    }

    public bool IsMoveValid (Vector2Int start, Vector2Int end) {
        if (gameOver) {
            return false;
        }
        Piece startPiece = GetPiece (start);
        if (startPiece != null && startPiece.team == currentTeam) {
            Team team = currentTeam;
            if (startPiece.CanMoveTo (end)) {
                MakeMove (start, end);
                if (GetKing (team).CanBeAttacked ()) {
                    Undo ();
                    return false;
                } else {
                    Undo ();
                    return true;
                }
            }
            return false;
        } else {
            //Debug.Log ("No Piece on " + start.x + ", " + start.y + " for team " + currentTeam);
            return false;
        }
    }

    public bool IsMoveValid (Move move) {
        return IsMoveValid (move.start, move.end);
    }

    public void MakeMove (Vector2Int start, Vector2Int end) {
        Piece startPiece = GetPiece (start);
        Piece[, ] oldState = CloneState (state);
        ChessHistory newHistory = new ChessHistory ();
        newHistory.state = oldState;
        newHistory.movesWithoutKill = 0;
        history.Push (newHistory);
        Piece destinationPiece = GetPiece (end);
        if (destinationPiece != null) {
            Kill (destinationPiece);
        } else {
            movesWithoutKill++;
        }
        SetPiece (startPiece, end);
        SetPiece (null, start);
        // pawn promotion
        if (startPiece is Pawn) {
            if (end.y == 0 || end.y == 7) {
                SetPiece (new Queen (startPiece.chess, startPiece.team), end);
            }
        }
        if (movesWithoutKill >= 50) {
            GameOver ();
            return;
        }
        ChangeTeam ();
    }

    public void MakeMove (Move move) {
        MakeMove (move.start, move.end);
    }

    public Move[] GetValidMoves () {
        Team team = currentTeam;
        List<Move> moves = new List<Move> ();
        foreach (Piece piece in state) {
            if (piece != null && piece.team == team) {
                foreach (Vector2Int destination in piece.GetValidDestinations ()) {
                    Move move = new Move (piece.position, destination);
                    if (IsMoveValid (move)) {
                        moves.Add (move);
                    }
                }
            }
        }

        return moves.ToArray ();
    }

    public bool NoMovesPossible () {
        return GetValidMoves ().Length == 0;
    }

    private Piece GetKing (Team team) {
        foreach (Piece piece in state) {
            if (piece is King && piece.team == team) {
                return piece;
            }
        }
        return null;
    }

    public void Kill (Piece piece) {
        movesWithoutKill = 0;
        if (piece is King) {
            GameOver ();
        }
    }

    public void GameOver () {
        gameOver = true;
    }

    public void ChangeTeam () {
        currentTeam = OtherTeam (currentTeam);
    }

    public void Undo () {
        if (history.Count > 0) {
            ChessHistory newState = history.Pop ();
            state = newState.state;
            movesWithoutKill = newState.movesWithoutKill;
            gameOver = false;
            ChangeTeam ();
        }
    }
    public void ClearHistory () {
        history.Clear ();
    }

    public Piece[, ] CloneState (Piece[, ] toClone) {
        Piece[, ] clonedState = new Piece[8, 8];
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                Piece piece = toClone[x, y];
                if (piece != null) {
                    Piece cloned = piece.Clone ();
                    clonedState[x, y] = cloned;
                }
            }
        }
        return clonedState;
    }

    private Team OtherTeam (Team team) {
        if (team == Team.Black) {
            return Team.White;
        } else {
            return Team.Black;
        }
    }

    public Piece[] GetPiecesByTeam (Team team) {
        List<Piece> pieces = new List<Piece> ();
        foreach (Piece piece in state) {
            if (piece != null && piece.team == team) {
                pieces.Add (piece);
            }
        }
        return pieces.ToArray ();
    }

}