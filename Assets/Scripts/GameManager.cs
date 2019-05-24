using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public Grid grid;
    public Team currentTeam = Team.White;
    public Chessman selectedChessman;

    // Start is called before the first frame update
    void Start () {
        if (instance == null) {
            instance = this;
        }
        Reset();
    }

    public void SelectChessman (Chessman chessman) {
        selectedChessman = chessman;
        chessman.Select();
        chessman.HighlightMoveToTiles();
        chessman.HighlightAttackAtTiles();
    }

    public void DeselectChessman () {
        selectedChessman.Deselect();
        selectedChessman = null;
        grid.DeselectAllTiles();
    }

    void Update () {
        if (Input.GetMouseButtonDown (0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit, 100.0f)) {

                GameObject hittedObject = hit.transform.gameObject;
                Chessman chessman = hittedObject.GetComponent<Chessman>();
                Tile tile = hittedObject.GetComponent<Tile>();
                if (chessman) {
                    handleChessmanClick(chessman);
                }
                if (tile) {
                    handleTileClick(tile);
                }

            }
        }
    }

    private void handleChessmanClick(Chessman chessman) {
        if (selectedChessman == null) {
            if (chessman.team == currentTeam) {
                SelectChessman(chessman);
            }
        } else {
            if (selectedChessman == chessman) {
                DeselectChessman();
            }
            if (chessman.team != currentTeam) {
                if (selectedChessman.CanAttackAt(chessman.currentTile)) {
                    selectedChessman.SetTile(chessman.currentTile);
                    DeselectChessman();
                    KillChessman(chessman);
                    ChangeTeam();
                };
            }
        }

    }

    private void handleTileClick(Tile tile) {
        if (selectedChessman == null) {
            if (tile.chessman != null) {
                handleChessmanClick(tile.chessman);
            }
        } else {
            if (selectedChessman.CanAttackAt(tile)) {
                selectedChessman.SetTile(tile);
                DeselectChessman();
                KillChessman(tile.chessman);
                ChangeTeam();
            } else if (selectedChessman.CanMoveTo(tile)) {
                selectedChessman.SetTile(tile);
                DeselectChessman();
                ChangeTeam();
            }
        }
    }

    private void KillChessman(Chessman chessman) {
        if (chessman.GetComponent<King>() != null) {
            GameOver(currentTeam);
        } else {
            chessman.Kill();
        }
    }

    private void ChangeTeam() {
        if (currentTeam == Team.Black) {
            currentTeam = Team.White;
        } else {
            currentTeam = Team.Black;
        }
    }

    private void GameOver(Team winner) {
        Debug.Log(winner + " won!");
        Reset();
    }

    public void Reset() {
        foreach (Transform child in grid.gameObject.transform) {
            Destroy(child.gameObject);
        }
        grid.Setup();
    }


}