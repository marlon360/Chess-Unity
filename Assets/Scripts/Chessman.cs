using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chessman : MonoBehaviour {

    public Team team;

    private Rigidbody rigid;
    private bool selected = false;

    public Tile currentTile;

    public float reward = 1;

    protected ChessBoard chessBoard;

    private bool moving = false;
    private Vector3 destination;

    void Start () {
        rigid = GetComponent<Rigidbody> ();
    }

    public void SetChessBoard(ChessBoard board) {
        chessBoard = board;
    }
    public ChessBoard GetChessBoard() {
        return chessBoard;
    }

    public virtual void SetTile (Tile tile) {
        //transform.position = tile.transform.position + new Vector3 (0, 1, 0);
        tile.chessman = this;
        if (currentTile != null) {
            currentTile.chessman = null;
        }
        currentTile = tile;
        moving = true;
        destination = tile.transform.position + new Vector3 (0, 1, 0);
    }

    void Update () {
        if (moving) {
            rigid.useGravity = false;
            transform.position = Vector3.Lerp (transform.position, destination, Time.deltaTime * 5f);
            if (Vector2.Distance (new Vector2(transform.position.x, transform.position.z), new Vector2(destination.x, destination.z)) < 0.1f) {
                moving = false;
                transform.position = destination;
                rigid.useGravity = true;
            }
        }
    }

    public void Select () {
        selected = true;
        rigid.useGravity = false;
        transform.position = transform.position + new Vector3 (0, 1, 0);
    }

    public bool CanMoveTo (Tile destinationTile) {
        bool canMove = false;
        GetMoveToTiles ().ForEach ((Tile tile) => {
            if (tile == destinationTile) {
                canMove = true;
            }
        });
        return canMove;
    }

    public bool CanAttackAt (Tile destinationTile) {
        bool canAttack = false;
        GetAttackAtTiles ().ForEach ((Tile tile) => {
            if (tile == destinationTile) {
                canAttack = true;
            }
        });
        return canAttack;
    }

    public bool CanBeAttacked() {
        List<Chessman> enemies = chessBoard.GetChessmenByTeam(team == Team.White ? Team.Black : Team.White);
        foreach (Chessman enemy in enemies) {
            if (enemy.CanAttackAt(currentTile)) {
                return true;
            }
        }
        return false;
    }

    public void Deselect () {
        selected = false;
        rigid.useGravity = true;
    }

    public void Kill () {
        Destroy (gameObject);
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