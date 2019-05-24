using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class ChessAgent : Agent {

    public Team team;

    public override void InitializeAgent () {

    }

    public override void CollectObservations () {
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                Tile tile = GameManager.instance.grid.GetTile(x, y);
                if (tile.chessman == null) {
                    AddVectorObs(0);
                } else if (tile.chessman.team == team) {
                    AddVectorObs(1);
                } else {
                    AddVectorObs(2);
                }
            }
        }
    }

    // to be implemented by the developer
    public override void AgentAction (float[] vectorAction, string textAction) {

        int chessmanPositionIndex = Mathf.FloorToInt (vectorAction[0]);
        Vector2 chessmanPosition = IndexToPosition(chessmanPositionIndex);
        Tile chessmanTile = GameManager.instance.grid.GetTile(chessmanPosition);

        if (chessmanTile.chessman != null && chessmanTile.chessman.team == team) {

            Chessman selectedChessman = chessmanTile.chessman;

            int destinationPositionIndex = Mathf.FloorToInt (vectorAction[1]);
            Vector2 destinationPosition = IndexToPosition(destinationPositionIndex);
            Tile destinationTile = GameManager.instance.grid.GetTile(destinationPosition);

            if (selectedChessman.CanAttackAt(destinationTile)) {
                // kill enemy at tile
                KillChessman(destinationTile.chessman);
                // move to this tile
                selectedChessman.SetTile(destinationTile);
                // add reward
                AddReward(1);
                // change team
                GameManager.instance.ChangeTeam();
            } else if (selectedChessman.CanMoveTo(destinationTile)) {
                // move to tile
                selectedChessman.SetTile(destinationTile);
                // change team
                GameManager.instance.ChangeTeam();
            } else {
                AddReward(-0.01f);
                RequestDecision();
            }

        } else {
            AddReward(-0.01f);
            RequestDecision();
        }
        

        
    }

    public void KillChessman(Chessman chessman) {
        if (chessman.GetComponent<King>() != null) {
            GameManager.instance.GameOver(team);
        } else {
            chessman.Kill();
        }
    }

    // to be implemented by the developer
    public override void AgentReset () {
        
    }

    private Vector2 IndexToPosition(int index) {
        int x = Mathf.FloorToInt(index / 8);
        int y = index % 8;
        // if (team == Team.Black) {
        //     x = 7 - x;
        //     y = 7 - y;
        // }
        return new Vector2(x, y);
    }

}