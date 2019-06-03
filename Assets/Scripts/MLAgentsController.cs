using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLAgentsController : MonoBehaviour
{

    public bool useBlackAgent = false;
    public bool useWhiteAgent = false;

    public ChessAgent BlackAgent;
    public ChessAgent WhiteAgent;

    private ChessGame chessGame;

    // Start is called before the first frame update
    void Start () {
        chessGame = GetComponent<ChessGame> ();
        chessGame.OnTeamChanged.AddObserver(OnTeamChanged);
    }

    private void OnTeamChanged(Team currentTeam) {
        if (useBlackAgent && currentTeam == Team.Black){
            BlackAgent.RequestDecision();
        } else if (useWhiteAgent && currentTeam == Team.White) {
            WhiteAgent.RequestDecision();
        }
    }


}
