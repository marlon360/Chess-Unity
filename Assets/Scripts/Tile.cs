using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Material MoveToMaterial;
    public Material AttackAtMaterial;
    public Chessman chessman;
    public Vector2 position;

    private Renderer rend;
    private Material defaultMaterial;

    void Start() {
        rend = GetComponent<Renderer>();
        defaultMaterial = rend.material;
    }

    public void SelectMoveToTile() {
        rend.material = MoveToMaterial;
    }
    public void SelectAttackAtTile() {
        rend.material = AttackAtMaterial;
    }

    public void DeselectTile() {
        rend.material = defaultMaterial;
    }

}
