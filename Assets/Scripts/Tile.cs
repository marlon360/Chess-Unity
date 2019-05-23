using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Material HighlightMaterial;

    private Renderer renderer;
    private Material defaultMaterial;

    void Stat() {
        renderer = GetComponent<Renderer>();
        defaultMaterial = renderer.material;
    }

    public void SelectTile() {
        renderer.material = HighlightMaterial;
    }

    public void DeselectTile() {
        renderer.material = defaultMaterial;
    }

}
