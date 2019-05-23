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
                    chessman.Kill();
                    selectedChessman.SetTile(chessman.currentTile);
                    DeselectChessman();
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
            if (selectedChessman.CanMoveTo(tile)) {
                selectedChessman.SetTile(tile);
                DeselectChessman();
                ChangeTeam();
            }
            if (selectedChessman.CanAttackAt(tile)) {
                tile.chessman.Kill();
                selectedChessman.SetTile(tile);
                DeselectChessman();
                ChangeTeam();
            }
        }
    }

    private void ChangeTeam() {
        if (currentTeam == Team.Black) {
            currentTeam = Team.White;
        } else {
            currentTeam = Team.Black;
        }
    }

}