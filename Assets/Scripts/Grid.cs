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

        for (int u = 0; u < 8; u++) {
            for (int v = 0; v < 8; v++) {
                Tile nextTile = (u + v) % 2 == 0 ? BlackTile : WhiteTile;
                GameObject tile = GameObject.Instantiate(nextTile.gameObject, new Vector3(v, 0, u), Quaternion.identity, gameObject.transform);
                tiles[u, v] = tile.GetComponent<Tile>();
                string zAxis = (u+1).ToString();
                string xAxis = alphabet[v].ToString();
                tile.name = xAxis + zAxis;
            }
        }

    }

    void SetupChessmen() {

        // white
        for (int i = 0; i < 8; i++) {
            GameObject chessman = GameObject.Instantiate(pawn.gameObject);
            Tile tile = tiles[1, i];
            chessman.GetComponent<Chessman>().SetTile(tile);
        }

        // black
        for (int i = 0; i < 8; i++) {
            GameObject chessman = GameObject.Instantiate(pawn.gameObject);
            Tile tile = tiles[6, i];
            chessman.GetComponent<Chessman>().SetTile(tile);
        }

    }

}
