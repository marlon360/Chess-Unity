using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece {
    public Knight(Chess chess, Team team): base(chess, team) {
        
    }
    public override Vector2Int[] GetDestinations (Piece[,] state) {

        List<Vector2Int> destinations = new List<Vector2Int> ();

        List<Vector2Int> moveDirections = new List<Vector2Int> ();
        moveDirections.Add (new Vector2Int (1, 2)); //ne
        moveDirections.Add (new Vector2Int (-1, 2)); //nw
        moveDirections.Add (new Vector2Int (2, 1)); //en
        moveDirections.Add (new Vector2Int (2, -1)); //es
        moveDirections.Add (new Vector2Int (-2, 1)); //wn
        moveDirections.Add (new Vector2Int (-2, -1)); //ws
        moveDirections.Add (new Vector2Int (1, -2)); //se
        moveDirections.Add (new Vector2Int (-1, -2)); //sw
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
        Knight clone =  new Knight(chess, team);
        clone.position = position;
        return clone;
    }
}