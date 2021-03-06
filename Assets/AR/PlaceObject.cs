﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent (typeof (ARSessionOrigin))]
public class PlaceObject : MonoBehaviour {

    public GameObject Marker;

    public GameObject ObjectToPlace;

    private ARSessionOrigin SessionOrigin;
    private ARRaycastManager RaycastManager;
    Vector3 ScreenCenter;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit> ();

    private bool IsObjectPlaced = false;

    // Use this for initialization
    void Start () {
        SessionOrigin = GetComponent<ARSessionOrigin> ();
        RaycastManager = GetComponent<ARRaycastManager> ();
        ScreenCenter = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
    }

    // Update is called once per frame
    void Update () {
        if (!IsObjectPlaced) {
            if (RaycastManager.Raycast (ScreenCenter, s_Hits, TrackableType.PlaneWithinPolygon)) {
                Pose hitPose = s_Hits[0].pose;

                Marker.SetActive (true);

                Marker.transform.position = hitPose.position + new Vector3(0,0.1f,0);
                Quaternion rotation = Quaternion.Euler (hitPose.rotation.eulerAngles.x, 0, hitPose.rotation.eulerAngles.z);
                Marker.transform.rotation = rotation;
            } else {
                Marker.SetActive (false);
            }

            if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
                placeObjectAtMarker ();
            }
        }

    }

    [ContextMenu ("place")]
    private void placeObjectAtMarker () {
        ObjectToPlace.transform.position = Marker.transform.position+ new Vector3(-0.07f, 0, -0.07f);
        ObjectToPlace.SetActive (true);
        ObjectToPlace.transform.localScale = new Vector3(0.02f,0.02f,0.02f);
        Marker.SetActive (false);
        IsObjectPlaced = true;
    }

    private float distanceBetweenCameraAndMarker() {
        return SessionOrigin.camera.transform.position.y - Marker.transform.position.y;
    }

}