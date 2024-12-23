using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;


public class DualHandJointVisualizer : MonoBehaviour
{
    public GameObject jointPrefab;                // �Ϲ� ����Ʈ�� ��Ÿ�� ������Ʈ
    public GameObject thumbTipPrefab;             // ���� �� ����Ʈ�� ��Ÿ�� ������Ʈ
    public GameObject indexTipPrefab;             // ���� �� ����Ʈ�� ��Ÿ�� ������Ʈ
    public float jointScale = 0.02f;              // ����Ʈ ������Ʈ ũ��

    public GameObject thumbsUpIndicatorPrefab;    // ���� �ø� ����ó ������Ʈ
    public GameObject fistIndicatorPrefab;        // �ָ� ����ó ������Ʈ
    public GameObject vGestureIndicatorPrefab;    // ���� ����ó ������Ʈ

    private Dictionary<TrackedHandJoint, GameObject> rightHandJoints = new Dictionary<TrackedHandJoint, GameObject>();
    private Dictionary<TrackedHandJoint, GameObject> leftHandJoints = new Dictionary<TrackedHandJoint, GameObject>();

    private GameObject currentIndicator;          // ���� Ȱ��ȭ�� ����ó ������Ʈ
    public UdpSender udpSender;  // UdpSender ����

    void Start()
    {
        // �� �� ����Ʈ �ʱ�ȭ
        foreach (TrackedHandJoint joint in System.Enum.GetValues(typeof(TrackedHandJoint)))
        {
            // ������ ����Ʈ
            GameObject rightJointObject = Instantiate(
                joint == TrackedHandJoint.ThumbTip ? thumbTipPrefab :
                joint == TrackedHandJoint.IndexTip ? indexTipPrefab : jointPrefab);

            rightJointObject.name = $"Right_{joint}";
            rightJointObject.transform.localScale = Vector3.one * jointScale;
            rightJointObject.SetActive(false);
            rightHandJoints[joint] = rightJointObject;

            // �޼� ����Ʈ
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
        // �����հ� �޼� ����Ʈ ������Ʈ
        UpdateHandJoints(Handedness.Right, rightHandJoints);
        UpdateHandJoints(Handedness.Left, leftHandJoints);

        // ����ó ���� �� ó��
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

        // �ո� ��ġ ��������
        Transform wristTransform = jointObjects[TrackedHandJoint.Wrist].transform;

        // ���� �ø�(Thumbs Up) ����
        if (IsThumbsUp(jointObjects))
        {
            ActivateGestureIndicator(thumbsUpIndicatorPrefab, wristTransform);
            if (udpSender != null) udpSender.gestureNUM = 1;  // �ָ� �ø� ���� �� gestureNUM�� 1�� ����
        }
        // �ָ�(Fist) ����
        else if (IsFist(jointObjects))
        {
            ActivateGestureIndicator(fistIndicatorPrefab, wristTransform);
            if (udpSender != null) udpSender.gestureNUM = 2;  // �ָ� ���� �� gestureNUM�� 2�� ����

        }
        // ����(V Gesture) ����
        else if (IsVGesture(jointObjects))
        {
            ActivateGestureIndicator(vGestureIndicatorPrefab, wristTransform);
            if (udpSender != null) udpSender.gestureNUM = 3;  // ���� ����ó ���� �� gestureNUM�� 3���� ����

        }
        else
        {
            DeactivateCurrentIndicator();
        }
    }

    private void ActivateGestureIndicator(GameObject prefab, Transform wristTransform)
    {
        // ���� Ȱ��ȭ�� ������Ʈ�� ��Ȱ��ȭ
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
        }

        // ���ο� ����ó ������Ʈ ����
        currentIndicator = Instantiate(prefab);
        currentIndicator.transform.position = wristTransform.position + Vector3.up * 0.05f; // �ո� ���� ǥ��
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
        // ���� ���� ���� �� ������ �Ÿ�
        float indexMiddleDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.MiddleTip].transform.position);

        // ���� ���� �չٴ� ������ �Ÿ�
        float thumbDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.ThumbTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ���� ���� �չٴ� ������ �Ÿ�
        float ringDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.RingTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ���� ���� �չٴ� ������ �Ÿ�
        float pinkyDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.PinkyTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ���� ���� �չٴ� ������ �Ÿ�
        float indexDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ���� ���� �չٴ� ������ �Ÿ�
        float middleDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.MiddleTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ���� ����ó ����:
        // 1. ������ ������ ����� �ָ� ������ �ִ�.
        // 2. ������ ������ �չٴڿ��� ����� �ָ� ������ �ִ�.
        // 3. ����, ����, ���� �հ����� �չٴ� �����̿� �ִ�.
        return indexMiddleDistance > 0.04f &&        // ������ ������ �ָ� ������ �ִ��� Ȯ��
               indexDistance > 0.05f &&             // ������ �չٴڿ��� �ָ� ������ �ִ��� Ȯ��
               middleDistance > 0.05f &&            // ������ �չٴڿ��� �ָ� ������ �ִ��� Ȯ��
               thumbDistance < 0.06f &&            // ������ �����ִ��� Ȯ��
               ringDistance < 0.06f &&             // ������ �����ִ��� Ȯ��
               pinkyDistance < 0.06f;              // �����հ����� �����ִ��� Ȯ��
    }


}
