using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour {
    public Vector2[] MoveDirections;
    public bool continuous;
    public bool allDirections;
    public bool jumping;

    private Rigidbody rigid;
    private bool selected = false;

    void Start () {
        rigid = GetComponent<Rigidbody> ();
    }

    public void SetTile (Tile tile) {
        transform.position = tile.transform.position + new Vector3 (0, 2, 0);
        tile.chessman = this;
    }

    public void Select () {
        selected = true;
        rigid.useGravity = false;
        transform.position = transform.position + new Vector3 (0, 1, 0);
    }

    public void Deselect () {
        selected = false;
        rigid.useGravity = true;
    }

    // public Tile[] GetMovableTiles() {
    //
    // }

    void OnMouseUp() {
        if (selected) {
            Deselect();
        } else {
            Select();
        }
    }

}