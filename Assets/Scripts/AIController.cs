using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public bool useBlackAI = false;
    public bool useWhiteAI = false;

    public int depth = 3;

    private ChessGame chessGame;

    // Start is called before the first frame update
    void Start () {
        chessGame = GetComponent<ChessGame> ();
        chessGame.OnTeamChanged.AddObserver(OnTeamChanged);
    }

    private void OnTeamChanged(Team currentTeam) {
        if ((useBlackAI && currentTeam == Team.Black) || (useWhiteAI && currentTeam == Team.White)) {
            AIMove(currentTeam);
        }
    }

    private void AIMove (Team team) {
        if (chessGame.GetChess().currentTeam == team) {
            Move move = ChessAI.GetBestMove (chessGame.GetChess(), depth);
            chessGame.GetChess().currentTeam = team;
            chessGame.MakeMove (move);
        }
    }
}