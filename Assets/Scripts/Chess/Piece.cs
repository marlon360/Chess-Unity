using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece {

    public Chess chess;
    public Team team;
    public Vector2Int position;

    public Piece(Chess chess, Team team) {
        this.chess = chess;
        this.team = team;
    }

    public Vector2Int[] GetValidDestinations() {
        if (!IsGameOver(chess.state)) {
            return GetDestinations(chess.state);
        } else {
            return new Vector2Int[0];
        }
    }

    public Vector2Int[] GetValidDestinations(Piece[,] state) {
        if (!IsGameOver(state)) {
            List<Vector2Int> destinations = new List<Vector2Int>();
            Vector2Int pos = position;
            foreach (Vector2Int dest in GetDestinations()) {
                if (chess.IsMoveValid(pos, dest)) {
                    destinations.Add(dest);
                }
            }
            return destinations.ToArray();
        } else {
            return new Vector2Int[0];
        }
    }

    public Vector2Int[] GetDestinations() {
        return GetDestinations(chess.state);
    }
    public abstract Vector2Int[] GetDestinations(Piece[,] state);

    public bool CanMoveTo(Vector2Int destination) {
        return GetDestinations(chess.state).Contains(destination);
    }

    protected bool PositionOutOfBound(Vector2Int pos) {
        return pos.x < 0 || pos.x > 7 || pos.y > 7 || pos.y < 0;
    }

    protected bool IsGameOver() {
        return IsGameOver(chess.state);
    }

    protected bool IsGameOver(Piece[,] state) {
        int kingCounter = 0;
        foreach (Piece piece in state) {
            if (piece is King) {
                kingCounter++;
            }
        }
        return kingCounter < 2;
    }

    public bool CanBeAttacked() {
        foreach (Piece piece in chess.GetPiecesByTeam(team == Team.Black ? Team.White : Team.Black)) {   
            if (piece.CanMoveTo(position)) {
                return true;
            }
        }
        return false;
    }

    public abstract Piece Clone();

}
