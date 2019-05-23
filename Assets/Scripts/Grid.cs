using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Tile BlackTile;
    public Tile WhiteTile;

    public Chessman pawn;

    private Tile[,] tiles = new Tile[8,8];

    // Start is called before the first frame update
    void Start()
    {
        SetupGrid();
        SetupChessmen();
    }

    void SetupGrid() {

        BoxCollider blackCollider = BlackTile.GetComponent<BoxCollider>();
        BoxCollider whiteCollider = WhiteTile.GetComponent<BoxCollider>();

        Vector3 blackSize = blackCollider.bounds.extents;
        Vector3 whiteSize = whiteCollider.bounds.extents;

        char[] alphabet = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'};

        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                Tile nextTile = (y + x) % 2 == 0 ? BlackTile : WhiteTile;
                GameObject tile = GameObject.Instantiate(nextTile.gameObject, new Vector3(x, 0, y), Quaternion.identity, gameObject.transform);
                tiles[x, y] = tile.GetComponent<Tile>();
                string xAxis = alphabet[x].ToString();
                string yAxis = (y+1).ToString();
                tile.name = xAxis + yAxis;
            }
        }

    }

    void SetupChessmen() {

        // white
        for (int i = 0; i < 8; i++) {
            GameObject chessman = GameObject.Instantiate(pawn.gameObject);
            Tile tile = tiles[i, 1];
            chessman.GetComponent<Chessman>().SetTile(tile);
        }

        // black
        for (int i = 0; i < 8; i++) {
            GameObject chessman = GameObject.Instantiate(pawn.gameObject);
            Tile tile = tiles[i, 6];
            chessman.GetComponent<Chessman>().SetTile(tile);
        }

    }

}
