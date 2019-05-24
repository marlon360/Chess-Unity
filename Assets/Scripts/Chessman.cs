using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chessman : MonoBehaviour {

    public Team team;

    private Rigidbody rigid;
    private bool selected = false;

    public Tile currentTile;

    void Start () {
        rigid = GetComponent<Rigidbody> ();
    }

    public virtual void SetTile (Tile tile) {
        transform.position = tile.transform.position + new Vector3 (0, 1, 0);
        // if (tile.chessman != null) {
        //     tile.chessman.Kill();
        // }
        tile.chessman = this;
        if (currentTile != null) {
            currentTile.chessman = null;
        }
        currentTile = tile;
    }

    public void Select () {
        selected = true;
        rigid.useGravity = false;
        transform.position = transform.position + new Vector3 (0, 1, 0);
    }

    public bool CanMoveTo(Tile destinationTile) {
        bool canMove = false;
        GetMoveToTiles().ForEach((Tile tile) => {
            if (tile == destinationTile) {
                canMove = true;
            }
        });
        return canMove;
    }

    public bool CanAttackAt(Tile destinationTile) {
        bool canAttack = false;
        GetAttackAtTiles().ForEach((Tile tile) => {
            if (tile == destinationTile) {
                canAttack = true;
            }
        });
        return canAttack;
    }

    public void Deselect () {
        selected = false;
        rigid.useGravity = true;
    }

    public void Kill() {
        Destroy(gameObject);
    }

    public abstract List<Tile> GetMoveToTiles ();
    public abstract List<Tile> GetAttackAtTiles ();

    public void HighlightMoveToTiles () {
        GetMoveToTiles ().ForEach ((Tile tile) => {
            tile.SelectMoveToTile ();
        });
    }
    public void HighlightAttackAtTiles () {
        GetAttackAtTiles ().ForEach ((Tile tile) => {
            tile.SelectAttackAtTile ();
        });
    }

    protected Vector2 Rotate (Vector2 vec, int deg) {
        return Quaternion.Euler (0, 0, deg) * vec;
    }

}