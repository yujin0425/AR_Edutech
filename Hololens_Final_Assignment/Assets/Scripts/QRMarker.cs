using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRMarker : MonoBehaviour
{
    Transform Origin
    {
        get
        {
            if (transform.childCount == 0)
            {
                var go = new GameObject("Coordinate");
                go.transform.parent = transform;
                go.transform.localPosition = new Vector3(0, 0, 0);
                go.transform.localRotation = Quaternion.identity;
                return go.transform;
            }
            else return transform.GetChild(0);
        }
    }
   
    
   
    /// <summary>QR 코드 인식 결과</summary>
    public string Data;
    [HideInInspector]
    public float Size = 0;
    public Guid? ID = null;
    SpatialGraphNode trackedNode = null;
    public Vector3 Position => Origin.transform.position;
    public Quaternion Rotation => Origin.transform.rotation;
    public TMPro.TextMeshPro Info;
    private Camera mainCamera = null;
    
    public GameObject gate;
    private bool gate_On = false;
    
    
    public void Start() => mainCamera = FindObjectOfType<Camera>();

   



    public void Update()
    {
        if (ID == null || Size == 0) return;
        try
        {
            if (mainCamera != null)
            {
                Info.transform.LookAt(mainCamera.transform, mainCamera.transform.up);
                Info.transform.rotation *= Quaternion.Euler(0, 180, 0);
            }
            trackedNode = SpatialGraphNode.FromStaticNodeId(ID.Value);
            if (trackedNode.TryLocate(FrameTime.OnUpdate, out Pose pose))
            {
                if (Microsoft.MixedReality.Toolkit.Utilities.CameraCache.Main.transform.parent != null)
                    pose = pose.GetTransformedBy(Microsoft.MixedReality.Toolkit.Utilities.CameraCache.Main.transform.parent);
                if (Vector3.Distance(pose.position, transform.position) > 0.002)
                {
                    GetComponent<AudioSource>().Play();
                    transform.SetPositionAndRotation(pose.position, pose.rotation);
                    Origin.localPosition = new Vector3(Size / 2, Size / 2, 0);
                    Origin.localEulerAngles = new Vector3(90, 0, 0);
                    Info.text = string.Format("" +
                        "===Position===\n" +
                        "x: {0}\n" +
                        "y: {1}\n" +
                        "z: {2}\n" +
                        "===Rotation===\n" +
                        "x: {3}\n" +
                        "y: {4}\n" +
                        "z: {5}\n", Position.x, Position.y, Position.z, Rotation.eulerAngles.x, Rotation.eulerAngles.y, Rotation.eulerAngles.z);
                 
                    if(gate_On != true)
                    {
                        var gate_obj = Instantiate(gate,Position,Rotation);
                        gate_On = true;
                    }
                  
                }

            
              Origin.gameObject.SetActive(true);
            }
        }
        catch (Exception exception)
        {
            UnityEngine.Debug.Log(exception.GetType().ToString());
        }
    }
}
