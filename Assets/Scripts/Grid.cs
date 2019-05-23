﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public Tile BlackTile;
    public Tile WhiteTile;

    public Chessman pawn;
    public Chessman knight;
    public Chessman king;
    public Chessman queen;
    public Chessman bishop;
    public Chessman rook;

    private Tile[, ] tiles = new Tile[8, 8];

    // Start is called before the first frame update
    void Start () {
        SetupGrid ();
        SetupChessmen ();
    }

    void SetupGrid () {

        BoxCollider blackCollider = BlackTile.GetComponent<BoxCollider> ();
        BoxCollider whiteCollider = WhiteTile.GetComponent<BoxCollider> ();

        Vector3 blackSize = blackCollider.bounds.extents;
        Vector3 whiteSize = whiteCollider.bounds.extents;

        char[] alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                Tile nextTile = (y + x) % 2 == 0 ? BlackTile : WhiteTile;
                GameObject tileObject = GameObject.Instantiate (nextTile.gameObject, new Vector3 (x, 0, y), Quaternion.identity, gameObject.transform);
                Tile tile = tileObject.GetComponent<Tile> ();
                tiles[x, y] = tile;
                tile.position = new Vector2 (x, y);
                // change name to chess coordinates
                string xAxis = alphabet[x].ToString ();
                string yAxis = (y + 1).ToString ();
                tileObject.name = xAxis + yAxis;
            }
        }

    }

    public Chessman PlaceChessman (Chessman chessman, int x, int y, Team team) {
        GameObject pawnObject = GameObject.Instantiate (chessman.gameObject);
        pawnObject.transform.parent = transform;
        Tile tile = tiles[x, y];
        pawnObject.GetComponent<Chessman> ().SetTile (tile);
        pawnObject.GetComponent<Chessman> ().team = team;
        return pawnObject.GetComponent<Chessman> ();
    }

    void SetupChessmen () {

        // white
        for (int i = 0; i < 8; i++) {
            Pawn pawnComp = PlaceChessman(pawn, i, 1, Team.White) as Pawn;
            pawnComp.firstMove = true;
        }
        PlaceChessman(rook, 0, 0, Team.White);
        PlaceChessman(knight, 1, 0, Team.White);
        PlaceChessman(bishop, 2, 0, Team.White);
        PlaceChessman(queen, 3, 0, Team.White);
        PlaceChessman(king, 4, 0, Team.White);
        PlaceChessman(bishop, 5, 0, Team.White);
        PlaceChessman(knight, 6, 0, Team.White);
        PlaceChessman(rook, 7, 0, Team.White);

        // black
        for (int i = 0; i < 8; i++) {
            Pawn pawnComp = PlaceChessman(pawn, i, 6, Team.Black) as Pawn;
            pawnComp.firstMove = true;
        }
        PlaceChessman(rook, 0, 7, Team.Black);
        PlaceChessman(knight, 1, 7, Team.Black);
        PlaceChessman(bishop, 2, 7, Team.Black);
        PlaceChessman(king, 3, 7, Team.Black);
        PlaceChessman(queen, 4, 7, Team.Black);
        PlaceChessman(bishop, 5, 7, Team.Black);
        PlaceChessman(knight, 6, 7, Team.Black);
        PlaceChessman(rook, 7, 7, Team.Black);


    }

    public Tile GetTile (int x, int y) {
        try {
            return tiles[x, y];
        } catch {
            return null;
        }
    }
    public Tile GetTile (Vector2 pos) {
        return GetTile (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.y));
    }

    public void DeselectAllTiles () {
        foreach (Tile tile in tiles) {
            tile.DeselectTile ();
        }
    }

}