using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Chessman {

    public override List<Tile> GetMoveToTiles () {
        List<Tile> destinations = new List<Tile> ();

        Vector2 currentPosition = currentTile.position;
        Vector2 moveDirection = new Vector2 (1, 0);
        // rotate move direction by 180 if black
        if (team == Team.Black) {
            moveDirection = Rotate (moveDirection, 180);
        }
        // iterate over rotations
        for (int j = 0; j <= 270; j += 90) {
            // rotate move direction
            Vector2 rotateMoveDirection = Rotate (moveDirection, j);
            // calculate destination
            Vector2 destPos = currentPosition + rotateMoveDirection;
            // get tile at destination
            Tile destinationTile = chessBoard.GetTile (destPos);
            // check if destination tile exists
            while (destinationTile != null) {
                // check if destination is empty
                if (destinationTile.chessman == null) {
                    destinations.Add (destinationTile);
                    destPos = destPos + rotateMoveDirection;
                    destinationTile = chessBoard.GetTile (destPos);
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
        Vector2 moveDirection = new Vector2 (1, 0);
        // rotate move direction by 180 if black
        if (team == Team.Black) {
            moveDirection = Rotate (moveDirection, 180);
        }
        // iterate over rotations
        for (int j = 0; j <= 270; j += 90) {
            // rotate move direction
            Vector2 rotateMoveDirection = Rotate (moveDirection, j);
            // calculate destination
            Vector2 destPos = currentPosition + rotateMoveDirection;
            // get tile at destination
            Tile destinationTile = chessBoard.GetTile (destPos);
            // check if destination tile exists
            while (destinationTile != null) {
                // check if destination is empty
                if (destinationTile.chessman == null) {
                    //destinations.Add (destinationTile);
                    destPos = destPos + rotateMoveDirection;
                    destinationTile = chessBoard.GetTile (destPos);
                } else {
                    // check if blocked by enemy
                    if (destinationTile.chessman.team != team) {
                        destinations.Add (destinationTile); // attack
                    }
                    break;
                }
            }
        }

        return destinations;
    }
}