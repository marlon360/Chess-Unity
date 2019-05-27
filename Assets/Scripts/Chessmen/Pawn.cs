using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chessman {

    public bool firstMove;

    public override void SetTile (Tile tile, Action<Chessman> callback = null, bool init = false) {
        base.SetTile(tile, callback, init);
        if (!init) {
            firstMove = false;
        }
    }

    public override List<Tile> GetMoveToTiles () {

        int counter = 1;
        if (firstMove) {
            counter = 2;
        }
        List<Tile> destinations = new List<Tile> ();

        Vector2 currentPosition = currentTile.position;
        List<Vector2> moveDirections = new List<Vector2>();
        moveDirections.Add(new Vector2 (0, 1));
        for (int i = 0; i < moveDirections.Count; i++) {
            Vector2 moveDirection = moveDirections[i];
            // rotate move direction by 180 if black
            if (team == Team.Black) {
                moveDirection = Rotate (moveDirection, 180);
            }

            // calculate destination
            Vector2 destPos = currentPosition + moveDirection;
            // get tile at destination
            Tile destinationTile = chessBoard.GetTile (destPos);
            // check if destination tile exists
            while (destinationTile != null && counter > 0) {
                // check if destination is empty
                if (destinationTile.chessman == null) {
                    destinations.Add (destinationTile);
                    destPos = destPos + moveDirection;
                    destinationTile = chessBoard.GetTile (destPos);
                    counter--;
                } else {
                    break;
                }
            }
        }

        return destinations;
    }

    public override List<Tile> GetAttackAtTiles () {
        List<Tile> destinations = new List<Tile> ();

        Vector2 currentPosition = currentTile.position;
        Vector2[] moveDirections = {
            new Vector2 (1, 1),
            new Vector2 (-1, 1),
        };
        for (int i = 0; i < moveDirections.Length; i++) {
            Vector2 moveDirection = moveDirections[i];
            // rotate move direction by 180 if black
            if (team == Team.Black) {
                moveDirection = Rotate (moveDirection, 180);
            }

            // calculate destination
            Vector2 destPos = currentPosition + moveDirection;
            // get tile at destination
            Tile destinationTile = chessBoard.GetTile (destPos);
            // check if destination tile exists
            if (destinationTile != null) {
                // check if destination is empty
                if (destinationTile.chessman != null) {
                    // check if blocked by enemy
                    if (destinationTile.chessman.team != team) {
                        destinations.Add (destinationTile); // attack
                    }
                }
            }
        }

        return destinations;
    }

}