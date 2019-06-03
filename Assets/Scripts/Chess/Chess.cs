using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess {

    public Team currentTeam = Team.White;
    public Piece[, ] state = new Piece[8, 8];

    public Subject<Team> OnGameOver = new Subject<Team>();
    public Subject<Piece> OnKilled = new Subject<Piece>();
    public Subject<Team> OnTeamChanged = new Subject<Team>();

    private Stack<Piece[, ]> history = new Stack<Piece[, ]> ();
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
            return startPiece.CanMoveTo (end);
        } else {
            Debug.Log ("No Piece on " + start.x + ", " + start.y + " for team " + currentTeam);
            return false;
        }
    }

    public bool IsMoveValid (Move move) {
        return IsMoveValid (move.start, move.end);
    }

    public void MakeMove (Vector2Int start, Vector2Int end) {
        Piece startPiece = GetPiece (start);
        if (IsMoveValid (start, end)) {
            Piece[, ] oldState = CloneState (state);
            history.Push (oldState);
            Piece destinationPiece = GetPiece (end);
            if (destinationPiece != null) {
                Kill (destinationPiece);
            }
            SetPiece (startPiece, end);
            SetPiece (null, start);
            // pawn promotion
            if (startPiece is Pawn) {
                if (end.y == 0 || end.y == 7) {
                    SetPiece(new Queen(startPiece.chess, startPiece.team), end);
                }
            }
            ChangeTeam ();
        }
    }

    public void MakeMove (Move move) {
        MakeMove (move.start, move.end);
    }

    public Move[] GetValidMoves () {
        List<Move> moves = new List<Move> ();
        foreach (Piece piece in state) {
            if (piece != null && piece.team == currentTeam) {
                foreach (Vector2Int destination in piece.GetValidDestinations ()) {
                    moves.Add (new Move (piece.position, destination));
                }
            }
        }
        return moves.ToArray ();
    }

    public void Kill (Piece piece) {
        OnKilled.Notify(piece);
        if (piece is King) {
            GameOver (OtherTeam(piece.team));
        }
    }

    public void GameOver (Team winner) {
        gameOver = true;
        OnGameOver.Notify(winner);
    }

    public void ChangeTeam () {
        currentTeam = OtherTeam(currentTeam);
        OnTeamChanged.Notify(currentTeam);
    }

    public void Undo () {
        if (history.Count > 0) {
            Piece[, ] newState = history.Pop ();
            state = newState;
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

    private Team OtherTeam(Team team) {
        if (team == Team.Black) {
            return Team.White;
        } else {
            return Team.Black;
        }
    }

    public Piece[] GetPiecesByTeam(Team team) {
        List<Piece> pieces = new List<Piece>();
        foreach (Piece piece in state) {
            if (piece != null && piece.team == team) {
                pieces.Add(piece);
            }
        }
        return pieces.ToArray();
    }

}