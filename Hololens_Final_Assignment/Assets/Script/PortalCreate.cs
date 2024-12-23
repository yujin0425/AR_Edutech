using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.OpenXR;


public class PortalCreate : MonoBehaviour
{
    public void Portal(SpatialGraphNode qrID, Vector3 qrPosition, Quaternion qrRotation)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = qrPosition;
        gameObject.transform.rotation = qrRotation;
    }
}
