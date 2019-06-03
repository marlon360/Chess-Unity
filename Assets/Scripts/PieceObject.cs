using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceObject : MonoBehaviour {
    public Vector2Int position;

    private float startYOffset;
    private float selectionHeight = 1f;
    private float duration = 4f;

    private bool select = false;
    private bool deselect = false;
    private bool moveTo = false;
    private Vector2Int moveToDestination;
    private bool kill = false;

    private Action OnMoved;
    private Action OnDeselected;
    private Action OnKilled;

    void Start() {
        startYOffset = transform.position.y;
    }

    private void Update() {
        if (select) {
            transform.position = Vector3.Lerp (transform.position, new Vector3(transform.position.x, selectionHeight, transform.position.z), Time.deltaTime * duration);
            if ((selectionHeight - transform.position.y) < 0.05f) {
                select = false;
                transform.position = new Vector3(transform.position.x, selectionHeight, transform.position.z);
            }
        } else if (deselect) {
            transform.position = Vector3.Lerp (transform.position, new Vector3(transform.position.x, startYOffset, transform.position.z), Time.deltaTime * duration);
            if ((transform.position.y - startYOffset) < 0.05f) {
                deselect = false;
                transform.position = new Vector3(transform.position.x, startYOffset, transform.position.z);
                if (OnDeselected != null) {
                    OnDeselected.Invoke();
                }
            }
        } else if (moveTo) {
            Vector3 destination = new Vector3(moveToDestination.x, selectionHeight, moveToDestination.y);
            transform.position = Vector3.Lerp (transform.position, destination, Time.deltaTime * duration);
            if ((Vector3.Distance(transform.position, destination)) < 0.05f) {
                moveTo = false;
                transform.position = destination;
                if (OnMoved != null) {
                    OnMoved.Invoke();
                }
            }
        } else if (kill) {
            Vector3 endScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
            transform.localScale = Vector3.Lerp (transform.localScale, endScale, Time.deltaTime * duration * 1.1f);
            if ((Vector3.Distance(transform.localScale, endScale)) < 0.1f) {
                kill = false;
                transform.localScale = endScale;
                if (OnKilled != null) {
                    OnKilled.Invoke();
                }
            }
        }
    }

    public void SelectPiece () {
        select = true;
    }

    public void DeselectPiece (Action callback = null) {
        OnDeselected = callback;
        deselect = true;
    }

    public void MoveTo(Vector2Int dest, Action callback = null) {
        OnMoved = callback;
        moveTo = true;
        moveToDestination = dest;
    }

    public void Kill(Action callback = null) {
        OnKilled = callback;
        kill = true;
    }


}