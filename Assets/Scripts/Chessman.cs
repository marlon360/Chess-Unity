using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public Vector2[] MoveDirections;
    public bool continuous;
    public bool allDirections;
    public bool jumping;

    public void SetTile(Tile tile) {
        transform.position = tile.transform.position + new Vector3(0, 2, 0);
        tile.chessman = this;
    }
}
