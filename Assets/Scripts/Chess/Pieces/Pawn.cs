using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece {

    public Pawn(Chess chess, Team team): base(chess, team) {
        
    }

    public override Vector2Int[] GetDestinations (Piece[,] state) {

        List<Vector2Int> destinations = new List<Vector2Int> ();

        bool firstMove = false;
        if ((team == Team.White && position.y == 1) || (team == Team.Black && position.y == 6)) {
            firstMove = true;
        }

        List<Vector2Int> moveDirections = new List<Vector2Int> ();
        moveDirections.Add (new Vector2Int (0, 1));
        for (int i = 0; i < moveDirections.Count; i++) {
            Vector2Int moveDirection = moveDirections[i];
            // rotate move direction by 180 if black
            if (team == Team.Black) {
                moveDirection *= -1;
            }
            // calculate destination
            Vector2Int destPos = position + moveDirection;

            // check if destination tile exists
            while (!PositionOutOfBound (destPos)) {
                Piece destinationChessman = chess.GetPiece (destPos);
                // check if destination is empty
                if (destinationChessman == null) {
                    destinations.Add (destPos);
                    destPos = destPos + moveDirection;
                    if (!firstMove) {
                        break;
                    }
                    firstMove = false;
                } else {
                    break;
                }
            }
        }
        moveDirections.Clear ();
        moveDirections.Add (new Vector2Int (1, 1));
        moveDirections.Add (new Vector2Int (-1, 1));
        for (int i = 0; i < moveDirections.Count; i++) {
            Vector2Int moveDirection = moveDirections[i];
            // rotate move direction by 180 if black
            if (team == Team.Black) {
                moveDirection *= -1;
            }
            // calculate destination
            Vector2Int destPos = position + moveDirection;

            // check if destination is empty
            if (!PositionOutOfBound (destPos)) {
                Piece destinationChessman = chess.GetPiece (destPos);
                if (destinationChessman != null && destinationChessman.team != team) {
                    destinations.Add (destPos);
                }
            }
        }

        return destinations.ToArray ();
    }
    public override Piece Clone() {
        Pawn clone =  new Pawn(chess, team);
        clone.position = position;
        return clone;
    }
}