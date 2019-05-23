using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Material HighlightMaterial;
    public Chessman chessman;
    public Vector2 position;

    private Renderer rend;
    private Material defaultMaterial;

    void Start() {
        rend = GetComponent<Renderer>();
        defaultMaterial = rend.material;
    }

    public void SelectTile() {
        rend.material = HighlightMaterial;
    }

    public void DeselectTile() {
        rend.material = defaultMaterial;
    }

}
