using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;


public class DualHandJointVisualizer : MonoBehaviour
{
    public GameObject jointPrefab;                // 일반 조인트를 나타낼 오브젝트
    public GameObject thumbTipPrefab;             // 엄지 끝 조인트를 나타낼 오브젝트
    public GameObject indexTipPrefab;             // 검지 끝 조인트를 나타낼 오브젝트
    public float jointScale = 0.02f;              // 조인트 오브젝트 크기

    public GameObject thumbsUpIndicatorPrefab;    // 엄지 올림 제스처 오브젝트
    public GameObject fistIndicatorPrefab;        // 주먹 제스처 오브젝트
    public GameObject vGestureIndicatorPrefab;    // 브이 제스처 오브젝트

    private Dictionary<TrackedHandJoint, GameObject> rightHandJoints = new Dictionary<TrackedHandJoint, GameObject>();
    private Dictionary<TrackedHandJoint, GameObject> leftHandJoints = new Dictionary<TrackedHandJoint, GameObject>();

    private GameObject currentIndicator;          // 현재 활성화된 제스처 오브젝트
    public UdpSender udpSender;  // UdpSender 참조

    void Start()
    {
        // 각 손 조인트 초기화
        foreach (TrackedHandJoint joint in System.Enum.GetValues(typeof(TrackedHandJoint)))
        {
            // 오른손 조인트
            GameObject rightJointObject = Instantiate(
                joint == TrackedHandJoint.ThumbTip ? thumbTipPrefab :
                joint == TrackedHandJoint.IndexTip ? indexTipPrefab : jointPrefab);

            rightJointObject.name = $"Right_{joint}";
            rightJointObject.transform.localScale = Vector3.one * jointScale;
            rightJointObject.SetActive(false);
            rightHandJoints[joint] = rightJointObject;

            // 왼손 조인트
            GameObject leftJointObject = Instantiate(
                joint == TrackedHandJoint.ThumbTip ? thumbTipPrefab :
                joint == TrackedHandJoint.IndexTip ? indexTipPrefab : jointPrefab);

            leftJointObject.name = $"Left_{joint}";
            leftJointObject.transform.localScale = Vector3.one * jointScale;
            leftJointObject.SetActive(false);
            leftHandJoints[joint] = leftJointObject;
        }
    }

    void Update()
    {
        // 오른손과 왼손 조인트 업데이트
        UpdateHandJoints(Handedness.Right, rightHandJoints);
        UpdateHandJoints(Handedness.Left, leftHandJoints);

        // 제스처 감지 및 처리
        CheckHandGestures(Handedness.Right, rightHandJoints);
        CheckHandGestures(Handedness.Left, leftHandJoints);
    }

    private void UpdateHandJoints(Handedness handedness, Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, handedness, out MixedRealityPose pose))
        {
            foreach (TrackedHandJoint joint in jointObjects.Keys)
            {
                if (HandJointUtils.TryGetJointPose(joint, handedness, out pose))
                {
                    jointObjects[joint].transform.position = pose.Position;
                    jointObjects[joint].transform.rotation = pose.Rotation;
                    jointObjects[joint].SetActive(true);
                }
                else
                {
                    jointObjects[joint].SetActive(false);
                }
            }
        }
        else
        {
            foreach (var jointObject in jointObjects.Values)
            {
                jointObject.SetActive(false);
            }
        }
    }

    private void CheckHandGestures(Handedness handedness, Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        if (!jointObjects[TrackedHandJoint.Wrist].activeSelf) return;

        // 손목 위치 가져오기
        Transform wristTransform = jointObjects[TrackedHandJoint.Wrist].transform;

        // 엄지 올림(Thumbs Up) 감지
        if (IsThumbsUp(jointObjects))
        {
            ActivateGestureIndicator(thumbsUpIndicatorPrefab, wristTransform);
            if (udpSender != null) udpSender.gestureNUM = 1;  // 주먹 올림 감지 시 gestureNUM을 1로 설정
        }
        // 주먹(Fist) 감지
        else if (IsFist(jointObjects))
        {
            ActivateGestureIndicator(fistIndicatorPrefab, wristTransform);
            if (udpSender != null) udpSender.gestureNUM = 2;  // 주먹 감지 시 gestureNUM을 2로 설정

        }
        // 브이(V Gesture) 감지
        else if (IsVGesture(jointObjects))
        {
            ActivateGestureIndicator(vGestureIndicatorPrefab, wristTransform);
            if (udpSender != null) udpSender.gestureNUM = 3;  // 브이 제스처 감지 시 gestureNUM을 3으로 설정

        }
        else
        {
            DeactivateCurrentIndicator();
        }
    }

    private void ActivateGestureIndicator(GameObject prefab, Transform wristTransform)
    {
        // 기존 활성화된 오브젝트를 비활성화
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
        }

        // 새로운 제스처 오브젝트 생성
        currentIndicator = Instantiate(prefab);
        currentIndicator.transform.position = wristTransform.position + Vector3.up * 0.05f; // 손목 위에 표시
        currentIndicator.transform.rotation = wristTransform.rotation;
    }

    private void DeactivateCurrentIndicator()
    {
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }
    }

    private bool IsThumbsUp(Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        float thumbDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.ThumbTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        float indexDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        return thumbDistance > 0.06f && indexDistance < 0.04f;
    }

    private bool IsFist(Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        float thumbDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.ThumbTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        float indexDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        float middleDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.MiddleTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        return thumbDistance < 0.06f && indexDistance < 0.06f && middleDistance < 0.06f;
    }

    private bool IsVGesture(Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // 검지 끝과 중지 끝 사이의 거리
        float indexMiddleDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.MiddleTip].transform.position);

        // 엄지 끝과 손바닥 사이의 거리
        float thumbDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.ThumbTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 약지 끝과 손바닥 사이의 거리
        float ringDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.RingTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 새끼 끝과 손바닥 사이의 거리
        float pinkyDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.PinkyTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 검지 끝과 손바닥 사이의 거리
        float indexDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 중지 끝과 손바닥 사이의 거리
        float middleDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.MiddleTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 브이 제스처 조건:
        // 1. 검지와 중지가 충분히 멀리 떨어져 있다.
        // 2. 검지와 중지는 손바닥에서 충분히 멀리 떨어져 있다.
        // 3. 엄지, 약지, 새끼 손가락은 손바닥 가까이에 있다.
        return indexMiddleDistance > 0.04f &&        // 검지와 중지가 멀리 떨어져 있는지 확인
               indexDistance > 0.05f &&             // 검지가 손바닥에서 멀리 떨어져 있는지 확인
               middleDistance > 0.05f &&            // 중지가 손바닥에서 멀리 떨어져 있는지 확인
               thumbDistance < 0.06f &&            // 엄지가 접혀있는지 확인
               ringDistance < 0.06f &&             // 약지가 접혀있는지 확인
               pinkyDistance < 0.06f;              // 새끼손가락이 접혀있는지 확인
    }


}
