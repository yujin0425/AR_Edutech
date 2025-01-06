using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class DualHandJointVisualizer2 : MonoBehaviour
{
    public GameObject jointPrefab;                // 일반 조인트를 나타낼 오브젝트
    public GameObject thumbTipPrefab;             // 엄지 끝 조인트를 나타낼 오브젝트
    public GameObject indexTipPrefab;             // 검지 끝 조인트를 나타낼 오브젝트
    public float jointScale = 0.02f;              // 조인트 오브젝트 크기 설정

    public GameObject thumbsUpIndicatorPrefab;    // 엄지 올림 제스처 시 나타날 오브젝트
    public GameObject fistIndicatorPrefab;        // 주먹 제스처 시 나타날 오브젝트
    public GameObject vGestureIndicatorPrefab;    // 브이 제스처 시 나타날 오브젝트

    private Dictionary<TrackedHandJoint, GameObject> rightHandJoints = new Dictionary<TrackedHandJoint, GameObject>();
    // 오른손 조인트 오브젝트를 저장할 딕셔너리
    private Dictionary<TrackedHandJoint, GameObject> leftHandJoints = new Dictionary<TrackedHandJoint, GameObject>();
    // 왼손 조인트 오브젝트를 저장할 딕셔너리

    private GameObject currentIndicator;          // 현재 활성화된 제스처 오브젝트

    void Start()
    {
        // 각 손 조인트를 초기화
        foreach (TrackedHandJoint joint in System.Enum.GetValues(typeof(TrackedHandJoint)))
        {
            // 오른손 조인트 생성
            GameObject rightJointObject = Instantiate(
                joint == TrackedHandJoint.ThumbTip ? thumbTipPrefab :
                joint == TrackedHandJoint.IndexTip ? indexTipPrefab : jointPrefab);

            rightJointObject.name = $"Right_{joint}"; // 오브젝트 이름 설정
            rightJointObject.transform.localScale = Vector3.one * jointScale; // 크기 설정
            rightJointObject.SetActive(false); // 초기에는 비활성화
            rightHandJoints[joint] = rightJointObject;

            // 왼손 조인트 생성
            GameObject leftJointObject = Instantiate(
                joint == TrackedHandJoint.ThumbTip ? thumbTipPrefab :
                joint == TrackedHandJoint.IndexTip ? indexTipPrefab : jointPrefab);

            leftJointObject.name = $"Left_{joint}"; // 오브젝트 이름 설정
            leftJointObject.transform.localScale = Vector3.one * jointScale; // 크기 설정
            leftJointObject.SetActive(false); // 초기에는 비활성화
            leftHandJoints[joint] = leftJointObject;
        }
    }

    void Update()
    {
        // 오른손과 왼손 조인트 정보를 업데이트
        UpdateHandJoints(Handedness.Right, rightHandJoints);
        UpdateHandJoints(Handedness.Left, leftHandJoints);

        // 제스처를 감지하고 처리
        CheckHandGestures(Handedness.Right, rightHandJoints);
        CheckHandGestures(Handedness.Left, leftHandJoints);
    }

    private void UpdateHandJoints(Handedness handedness, Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // 손바닥 조인트 위치를 가져올 수 있는지 확인
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, handedness, out MixedRealityPose pose))
        {
            foreach (TrackedHandJoint joint in jointObjects.Keys)
            {
                // 각 조인트의 위치와 회전 정보를 업데이트
                if (HandJointUtils.TryGetJointPose(joint, handedness, out pose))
                {
                    jointObjects[joint].transform.position = pose.Position;
                    jointObjects[joint].transform.rotation = pose.Rotation;
                    jointObjects[joint].SetActive(true); // 활성화
                }
                else
                {
                    jointObjects[joint].SetActive(false); // 비활성화
                }
            }
        }
        else
        {
            // 손바닥 데이터를 가져오지 못하면 모든 조인트 비활성화
            foreach (var jointObject in jointObjects.Values)
            {
                jointObject.SetActive(false);
            }
        }
    }

    private void CheckHandGestures(Handedness handedness, Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // 손목 조인트가 활성화되지 않았다면 제스처 감지 중단
        if (!jointObjects[TrackedHandJoint.Wrist].activeSelf) return;

        // 손목 위치 가져오기
        Transform wristTransform = jointObjects[TrackedHandJoint.Wrist].transform;

        // 엄지 올림(Thumbs Up) 제스처 감지
        if (IsThumbsUp(jointObjects))
        {
            ActivateGestureIndicator(thumbsUpIndicatorPrefab, wristTransform);
        }
        // 주먹(Fist) 제스처 감지
        else if (IsFist(jointObjects))
        {
            ActivateGestureIndicator(fistIndicatorPrefab, wristTransform);
        }
        // 브이(V Gesture) 제스처 감지
        else if (IsVGesture(jointObjects))
        {
            ActivateGestureIndicator(vGestureIndicatorPrefab, wristTransform);
        }
        else
        {
            DeactivateCurrentIndicator(); // 활성화된 제스처가 없으면 오브젝트 비활성화
        }
    }

    private void ActivateGestureIndicator(GameObject prefab, Transform wristTransform)
    {
        // 기존에 활성화된 오브젝트를 제거
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
        }

        // 새로운 제스처 오브젝트 생성 및 손목 위에 표시
        currentIndicator = Instantiate(prefab);
        currentIndicator.transform.position = wristTransform.position + Vector3.up * 0.05f; // 손목 위에 위치 설정
        currentIndicator.transform.rotation = wristTransform.rotation; // 손목의 회전 값 적용
    }

    private void DeactivateCurrentIndicator()
    {
        // 활성화된 오브젝트 제거
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }
    }

    private bool IsThumbsUp(Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // 엄지 끝과 손바닥 사이의 거리 계산
        float thumbDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.ThumbTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 검지 끝과 손바닥 사이의 거리 계산
        float indexDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 엄지는 손바닥에서 멀고, 검지는 가까운 경우 엄지 올림 제스처로 간주
        return thumbDistance > 0.06f && indexDistance < 0.04f;
    }

    private bool IsFist(Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // 각 손가락 끝과 손바닥 사이의 거리 계산
        float thumbDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.ThumbTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        float indexDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        float middleDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.MiddleTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 엄지, 검지, 중지가 손바닥에 가까운 경우 주먹 제스처로 간주
        return thumbDistance < 0.06f && indexDistance < 0.06f && middleDistance < 0.06f;
    }

    private bool IsVGesture(Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // 검지 끝과 중지 끝 사이의 거리 계산
        float indexMiddleDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.MiddleTip].transform.position);

        // 엄지 끝과 손바닥 사이의 거리 계산
        float thumbDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.ThumbTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 약지 끝과 손바닥 사이의 거리 계산
        float ringDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.RingTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 새끼 끝과 손바닥 사이의 거리 계산
        float pinkyDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.PinkyTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // 검지와 중지 끝이 서로 멀고, 다른 손가락은 손바닥에 가까운 경우 브이 제스처로 간주
        return indexMiddleDistance > 0.04f &&        // 검지와 중지가 충분히 멀리 떨어져 있는지 확인
               Vector3.Distance(jointObjects[TrackedHandJoint.IndexTip].transform.position,
                                jointObjects[TrackedHandJoint.Palm].transform.position) > 0.05f &&
               Vector3.Distance(jointObjects[TrackedHandJoint.MiddleTip].transform.position,
                                jointObjects[TrackedHandJoint.Palm].transform.position) > 0.05f &&
               thumbDistance < 0.06f &&
               ringDistance < 0.06f &&
               pinkyDistance < 0.06f;
    }
}
