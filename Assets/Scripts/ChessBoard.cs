﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour {

    public Team currentTeam = Team.White;
    public Chessman selectedChessman;

    public bool UseWhiteAgent = true;
    public ChessAgent WhiteAgent;
    public bool UseBlackAgent = true;
    public ChessAgent BlackAgent;

    public bool gameOver = false;

    public GameObject UI;

    private Grid grid;

    // Start is called before the first frame update
    void Start () {
        grid = GetComponentInChildren<Grid> ();

        if (!UseWhiteAgent) {
            WhiteAgent = null;
        }
        if (!UseBlackAgent) {
            BlackAgent = null;
        }
        Reset ();
    }

    public Tile GetTile (int x, int y) {
        return grid.GetTile (x, y);
    }
    public Tile GetTile (Vector2 pos) {
        return grid.GetTile (pos);
    }

    public List<Chessman> GetChessmenByTeam(Team team) {
        return grid.GetChessmenByTeam(team);
    }

    public void SelectChessman (Chessman chessman) {
        selectedChessman = chessman;
        chessman.Select ();
        chessman.HighlightMoveToTiles ();
        chessman.HighlightAttackAtTiles ();
    }

    public void DeselectChessman () {
        selectedChessman.Deselect ();
        selectedChessman = null;
        grid.DeselectAllTiles ();
    }

    void Update () {
        if (Input.GetMouseButtonDown (0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit, 100.0f)) {

                GameObject hittedObject = hit.transform.gameObject;
                Chessman chessman = hittedObject.GetComponent<Chessman> ();
                Tile tile = hittedObject.GetComponent<Tile> ();
                if (chessman && chessman.GetChessBoard() == this) {
                    handleChessmanClick (chessman);
                }
                if (tile) {
                    handleTileClick (tile);
                }

            }
        }
    }

    private void handleChessmanClick (Chessman chessman) {
        if (selectedChessman == null) {
            if (chessman.team == currentTeam) {
                SelectChessman (chessman);
            }
        } else {
            if (selectedChessman == chessman) {
                DeselectChessman ();
            }
            if (chessman.team != currentTeam) {
                if (selectedChessman.CanAttackAt (chessman.currentTile)) {
                    selectedChessman.SetTile (chessman.currentTile);
                    DeselectChessman ();
                    KillChessman (chessman);
                    //ChangeTeam ();
                };
            }
        }

    }

    public void handleTileClick (Tile tile) {
        if (selectedChessman == null) {
            if (tile.chessman != null) {
                handleChessmanClick (tile.chessman);
            }
        } else {
            if (selectedChessman.CanAttackAt (tile)) {
                KillChessman (tile.chessman);
                selectedChessman.SetTile (tile);
                DeselectChessman ();
                //ChangeTeam ();
            } else if (selectedChessman.CanMoveTo (tile)) {
                selectedChessman.SetTile (tile);
                DeselectChessman ();
                //ChangeTeam ();
            }
        }
    }

    public void KillChessman (Chessman chessman) {
        if (chessman.GetComponent<King> () != null) {
            GameOver (currentTeam);
        } else {
            chessman.Kill ();
        }
    }

    public void ChangeTeam () {
        if (currentTeam == Team.Black) {
            currentTeam = Team.White;
            WhiteAgent?.RequestDecision ();
        } else {
            currentTeam = Team.Black;
            BlackAgent?.RequestDecision ();
        }
    }

    public void GameOver (Team winner) {
        gameOver = true;
        if (winner == Team.Black) {
            UI.GetComponentInChildren<Text>().text = "Black wins";
            BlackAgent?.AddReward (10);
            WhiteAgent?.AddReward (-10);
        } else {
            UI.GetComponentInChildren<Text>().text = "White wins";
            WhiteAgent?.AddReward (10);
            BlackAgent?.AddReward (-10);
        }
        BlackAgent?.Done ();
        WhiteAgent?.Done ();
        UI.SetActive(true);
        StartCoroutine(ResetAfterDelay(5f));
    }

    IEnumerator ResetAfterDelay(float sec) {
        yield return new WaitForSeconds(sec);
        Reset();
    }

    public void Reset () {
        UI.SetActive(false);
        foreach (Transform child in grid.gameObject.transform) {
            Destroy (child.gameObject);
        }
        grid.Setup ();
        currentTeam = Team.White;
        gameOver = false;
        WhiteAgent?.RequestDecision ();
    }

}