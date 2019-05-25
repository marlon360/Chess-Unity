using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman {

    public override List<Tile> GetMoveToTiles () {
        List<Tile> destinations = new List<Tile> ();

        Vector2 currentPosition = currentTile.position;
        Vector2[] moveDirections = {
            new Vector2 (1, 1),
            new Vector2 (0, 1),
        };
        for (int i = 0; i < moveDirections.Length; i++) {
            Vector2 moveDirection = moveDirections[i];
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
                if (destinationTile != null) {
                    // check if destination is empty
                    if (destinationTile.chessman == null) {
                        destinations.Add (destinationTile);
                    }
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
            new Vector2 (0, 1),
        };
        for (int i = 0; i < moveDirections.Length; i++) {
            Vector2 moveDirection = moveDirections[i];
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
                if (destinationTile != null) {
                    // check if destination is empty
                    if (destinationTile.chessman == null) {
                        //destinations.Add (destinationTile);
                    } else {
                        // check if blocked by enemy
                        if (destinationTile.chessman.team != team) {
                            destinations.Add (destinationTile); // attack
                        }
                    }
                }
            }
        }

        return destinations;
    }
}