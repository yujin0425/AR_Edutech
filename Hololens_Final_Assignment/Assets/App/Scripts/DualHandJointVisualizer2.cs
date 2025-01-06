using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class DualHandJointVisualizer2 : MonoBehaviour
{
    public GameObject jointPrefab;                // �Ϲ� ����Ʈ�� ��Ÿ�� ������Ʈ
    public GameObject thumbTipPrefab;             // ���� �� ����Ʈ�� ��Ÿ�� ������Ʈ
    public GameObject indexTipPrefab;             // ���� �� ����Ʈ�� ��Ÿ�� ������Ʈ
    public float jointScale = 0.02f;              // ����Ʈ ������Ʈ ũ�� ����

    public GameObject thumbsUpIndicatorPrefab;    // ���� �ø� ����ó �� ��Ÿ�� ������Ʈ
    public GameObject fistIndicatorPrefab;        // �ָ� ����ó �� ��Ÿ�� ������Ʈ
    public GameObject vGestureIndicatorPrefab;    // ���� ����ó �� ��Ÿ�� ������Ʈ

    private Dictionary<TrackedHandJoint, GameObject> rightHandJoints = new Dictionary<TrackedHandJoint, GameObject>();
    // ������ ����Ʈ ������Ʈ�� ������ ��ųʸ�
    private Dictionary<TrackedHandJoint, GameObject> leftHandJoints = new Dictionary<TrackedHandJoint, GameObject>();
    // �޼� ����Ʈ ������Ʈ�� ������ ��ųʸ�

    private GameObject currentIndicator;          // ���� Ȱ��ȭ�� ����ó ������Ʈ

    void Start()
    {
        // �� �� ����Ʈ�� �ʱ�ȭ
        foreach (TrackedHandJoint joint in System.Enum.GetValues(typeof(TrackedHandJoint)))
        {
            // ������ ����Ʈ ����
            GameObject rightJointObject = Instantiate(
                joint == TrackedHandJoint.ThumbTip ? thumbTipPrefab :
                joint == TrackedHandJoint.IndexTip ? indexTipPrefab : jointPrefab);

            rightJointObject.name = $"Right_{joint}"; // ������Ʈ �̸� ����
            rightJointObject.transform.localScale = Vector3.one * jointScale; // ũ�� ����
            rightJointObject.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
            rightHandJoints[joint] = rightJointObject;

            // �޼� ����Ʈ ����
            GameObject leftJointObject = Instantiate(
                joint == TrackedHandJoint.ThumbTip ? thumbTipPrefab :
                joint == TrackedHandJoint.IndexTip ? indexTipPrefab : jointPrefab);

            leftJointObject.name = $"Left_{joint}"; // ������Ʈ �̸� ����
            leftJointObject.transform.localScale = Vector3.one * jointScale; // ũ�� ����
            leftJointObject.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
            leftHandJoints[joint] = leftJointObject;
        }
    }

    void Update()
    {
        // �����հ� �޼� ����Ʈ ������ ������Ʈ
        UpdateHandJoints(Handedness.Right, rightHandJoints);
        UpdateHandJoints(Handedness.Left, leftHandJoints);

        // ����ó�� �����ϰ� ó��
        CheckHandGestures(Handedness.Right, rightHandJoints);
        CheckHandGestures(Handedness.Left, leftHandJoints);
    }

    private void UpdateHandJoints(Handedness handedness, Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // �չٴ� ����Ʈ ��ġ�� ������ �� �ִ��� Ȯ��
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, handedness, out MixedRealityPose pose))
        {
            foreach (TrackedHandJoint joint in jointObjects.Keys)
            {
                // �� ����Ʈ�� ��ġ�� ȸ�� ������ ������Ʈ
                if (HandJointUtils.TryGetJointPose(joint, handedness, out pose))
                {
                    jointObjects[joint].transform.position = pose.Position;
                    jointObjects[joint].transform.rotation = pose.Rotation;
                    jointObjects[joint].SetActive(true); // Ȱ��ȭ
                }
                else
                {
                    jointObjects[joint].SetActive(false); // ��Ȱ��ȭ
                }
            }
        }
        else
        {
            // �չٴ� �����͸� �������� ���ϸ� ��� ����Ʈ ��Ȱ��ȭ
            foreach (var jointObject in jointObjects.Values)
            {
                jointObject.SetActive(false);
            }
        }
    }

    private void CheckHandGestures(Handedness handedness, Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // �ո� ����Ʈ�� Ȱ��ȭ���� �ʾҴٸ� ����ó ���� �ߴ�
        if (!jointObjects[TrackedHandJoint.Wrist].activeSelf) return;

        // �ո� ��ġ ��������
        Transform wristTransform = jointObjects[TrackedHandJoint.Wrist].transform;

        // ���� �ø�(Thumbs Up) ����ó ����
        if (IsThumbsUp(jointObjects))
        {
            ActivateGestureIndicator(thumbsUpIndicatorPrefab, wristTransform);
        }
        // �ָ�(Fist) ����ó ����
        else if (IsFist(jointObjects))
        {
            ActivateGestureIndicator(fistIndicatorPrefab, wristTransform);
        }
        // ����(V Gesture) ����ó ����
        else if (IsVGesture(jointObjects))
        {
            ActivateGestureIndicator(vGestureIndicatorPrefab, wristTransform);
        }
        else
        {
            DeactivateCurrentIndicator(); // Ȱ��ȭ�� ����ó�� ������ ������Ʈ ��Ȱ��ȭ
        }
    }

    private void ActivateGestureIndicator(GameObject prefab, Transform wristTransform)
    {
        // ������ Ȱ��ȭ�� ������Ʈ�� ����
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
        }

        // ���ο� ����ó ������Ʈ ���� �� �ո� ���� ǥ��
        currentIndicator = Instantiate(prefab);
        currentIndicator.transform.position = wristTransform.position + Vector3.up * 0.05f; // �ո� ���� ��ġ ����
        currentIndicator.transform.rotation = wristTransform.rotation; // �ո��� ȸ�� �� ����
    }

    private void DeactivateCurrentIndicator()
    {
        // Ȱ��ȭ�� ������Ʈ ����
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }
    }

    private bool IsThumbsUp(Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // ���� ���� �չٴ� ������ �Ÿ� ���
        float thumbDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.ThumbTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ���� ���� �չٴ� ������ �Ÿ� ���
        float indexDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ������ �չٴڿ��� �ְ�, ������ ����� ��� ���� �ø� ����ó�� ����
        return thumbDistance > 0.06f && indexDistance < 0.04f;
    }

    private bool IsFist(Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // �� �հ��� ���� �չٴ� ������ �Ÿ� ���
        float thumbDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.ThumbTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        float indexDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        float middleDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.MiddleTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ����, ����, ������ �չٴڿ� ����� ��� �ָ� ����ó�� ����
        return thumbDistance < 0.06f && indexDistance < 0.06f && middleDistance < 0.06f;
    }

    private bool IsVGesture(Dictionary<TrackedHandJoint, GameObject> jointObjects)
    {
        // ���� ���� ���� �� ������ �Ÿ� ���
        float indexMiddleDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.IndexTip].transform.position,
            jointObjects[TrackedHandJoint.MiddleTip].transform.position);

        // ���� ���� �չٴ� ������ �Ÿ� ���
        float thumbDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.ThumbTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ���� ���� �չٴ� ������ �Ÿ� ���
        float ringDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.RingTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ���� ���� �չٴ� ������ �Ÿ� ���
        float pinkyDistance = Vector3.Distance(
            jointObjects[TrackedHandJoint.PinkyTip].transform.position,
            jointObjects[TrackedHandJoint.Palm].transform.position);

        // ������ ���� ���� ���� �ְ�, �ٸ� �հ����� �չٴڿ� ����� ��� ���� ����ó�� ����
        return indexMiddleDistance > 0.04f &&        // ������ ������ ����� �ָ� ������ �ִ��� Ȯ��
               Vector3.Distance(jointObjects[TrackedHandJoint.IndexTip].transform.position,
                                jointObjects[TrackedHandJoint.Palm].transform.position) > 0.05f &&
               Vector3.Distance(jointObjects[TrackedHandJoint.MiddleTip].transform.position,
                                jointObjects[TrackedHandJoint.Palm].transform.position) > 0.05f &&
               thumbDistance < 0.06f &&
               ringDistance < 0.06f &&
               pinkyDistance < 0.06f;
    }
}
