using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour {
    public Vector2[] MoveDirections;
    public bool continuous;
    public bool allDirections;
    public bool jumping;

    public Team team;

    private Rigidbody rigid;
    private bool selected = false;

    private Tile currentTile;

    void Start () {
        rigid = GetComponent<Rigidbody> ();
    }

    public void SetTile (Tile tile) {
        transform.position = tile.transform.position + new Vector3 (0, 2, 0);
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

    public void Deselect () {
        selected = false;
        rigid.useGravity = true;
    }

    public List<Tile> GetDestinationTiles () {

        List<Tile> destinations = new List<Tile> ();

        Vector2 currentPosition = currentTile.position;
        // all move directions
        for (int i = 0; i < MoveDirections.Length; i++) {
            Vector2 moveDirection = MoveDirections[i];
            // rotate move direction by 180 if black
            if (team == Team.Black) {
                moveDirection = Rotate (moveDirection, 180);
            }
            // if all directions enabled rotate to 270 deg in 90deg steps 
            int maxRotation = allDirections ? 270 : 0;
            // iterate over rotations
            for (int j = 0; j <= maxRotation; j += 90) {
                // rotate move direction
                Vector2 rotateMoveDirection = Rotate (moveDirection, j);
                // calculate destination
                Vector2 destPos = currentPosition + rotateMoveDirection;
                // get tile at destination
                Tile destinationTile = Grid.instance.GetTile (destPos);
                // check if destination tile exists
                while (destinationTile != null) {
                    // check if destination is empty
                    if (destinationTile.chessman == null) {
                        destinations.Add (destinationTile);
                    } else {
                        // check if blocked by enemy
                        if (destinationTile.chessman.team != team) {
                            destinations.Add (destinationTile); // attack
                        }
                        // if jumping is disabled break loop
                        if (!jumping) {
                            break;
                        }
                    }
                    // if continuos enabled add move direction to current destination
                    if (continuous) {
                        destPos = destPos + rotateMoveDirection;
                        destinationTile = Grid.instance.GetTile (destPos);
                    } else {
                        break;
                    }

                }
            }

        }
        return destinations;
    }

    public void HighlightDestinations () {
        GetDestinationTiles ().ForEach ((Tile tile) => {
            tile.SelectTile ();
        });
    }

    void OnMouseUp () {
        if (selected) {
            Deselect ();
            Grid.instance.DeselectAllTiles ();
        } else {
            Select ();
            HighlightDestinations ();
        }
    }

    private Vector2 Rotate (Vector2 vec, int deg) {
        return Quaternion.Euler (0, 0, deg) * vec;
    }

}