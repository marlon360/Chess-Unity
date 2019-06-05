using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    private ChessGame chessGame;

    private PieceObject selectedPiece;

    void Start () {
        chessGame = GetComponent<ChessGame> ();
    }

    void Update () {
        if (Input.GetMouseButtonDown (0) || Input.touchCount > 0) {

            RaycastHit hit;
            Ray ray;
            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch (0);
                if (touch.phase == TouchPhase.Began) {
                    ray = Camera.main.ScreenPointToRay (touch.position);
                } else {
                    return;
                }
            } else {
                ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            }
            if (Physics.Raycast (ray, out hit, 100.0f)) {
                GameObject hittedObject = hit.transform.gameObject;
                PieceObject piece = hittedObject.GetComponentInParent<PieceObject> ();
                Tile tile = hittedObject.GetComponent<Tile> ();
                if (piece != null) {
                    handlePieceClick (piece);
                }
                if (tile != null) {
                    handleTileClick (tile);
                }
            }
        }
    }

    private void handlePieceClick (PieceObject clickedPiece) {
        Piece piece = chessGame.GetPiece (clickedPiece.position);
        if (selectedPiece == null) {
            if (piece.team == chessGame.GetChess ().currentTeam) {
                SelectPiece (clickedPiece);
            }
        } else {
            if (selectedPiece == clickedPiece) {
                DeselectPiece ();
            }
            if (piece.team != chessGame.GetChess ().currentTeam) {
                if (chessGame.GetChess ().IsMoveValid (selectedPiece.position, clickedPiece.position)) {
                    chessGame.MakeMove (selectedPiece.position, clickedPiece.position);
                };
            }
        }
    }

    public void handleTileClick (Tile tile) {
        if (selectedPiece != null) {
            if (chessGame.GetChess ().IsMoveValid (selectedPiece.position, tile.position)) {
                chessGame.MakeMove (selectedPiece.position, tile.position);
            }
        }
    }

    public void SelectPiece (PieceObject pieceObject) {
        selectedPiece = pieceObject;
        chessGame.SelectPiece (pieceObject);
    }
    public void DeselectPiece () {
        chessGame.DeselectPiece (selectedPiece);
        selectedPiece = null;
    }

}