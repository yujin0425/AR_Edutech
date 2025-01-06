using UnityEngine;

public class ColorChange : MonoBehaviour
{
    // ������Ʈ�� ������ �����ϱ� ���� ����ü
    [System.Serializable]
    public class ObjectColorPair
    {
        public GameObject targetObject; // ������ ����� ������Ʈ
        public Color color; // ����� ����
        [HideInInspector] public Renderer renderer; // ĳ�̵� Renderer
    }

    // ������Ʈ�� ������ ����Ʈ�� ����
    public ObjectColorPair[] objectColorPairs;

    // �ʱ�ȭ �� Renderer ĳ��
    private void Awake()
    {
        foreach (var pair in objectColorPairs)
        {
            if (pair.targetObject != null)
            {
                pair.renderer = pair.targetObject.GetComponent<Renderer>();
            }
        }
    }

    // ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void ChangeObjectColor(int index)
    {
        // ��ȿ�� �ε������� Ȯ��
        if (index >= 0 && index < objectColorPairs.Length)
        {
            var pair = objectColorPairs[index];
            if (pair.renderer != null)
            {
                pair.renderer.material.color = pair.color;
            }
        }
    }
}
