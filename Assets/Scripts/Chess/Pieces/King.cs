using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece {
    public King(Chess chess, Team team): base(chess, team) {
        
    }
    public override Vector2Int[] GetDestinations (Piece[,] state) {

        List<Vector2Int> destinations = new List<Vector2Int> ();

        List<Vector2Int> moveDirections = new List<Vector2Int> ();
        moveDirections.Add (new Vector2Int (0, 1)); // n
        moveDirections.Add (new Vector2Int (1, 1)); // ne
        moveDirections.Add (new Vector2Int (1, 0)); // e
        moveDirections.Add (new Vector2Int (1, -1)); // se
        moveDirections.Add (new Vector2Int (0, -1)); // s
        moveDirections.Add (new Vector2Int (-1, -1)); // sw
        moveDirections.Add (new Vector2Int (-1, 0)); // w
        moveDirections.Add (new Vector2Int (-1, 1)); // nw
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
                } else if (destinationChessman.team != team) {
                    destinations.Add (destPos);
                }
                break;
            }
        }

        return destinations.ToArray ();
    }
    public override Piece Clone() {
        King clone =  new King(chess, team);
        clone.position = position;
        return clone;
    }
}