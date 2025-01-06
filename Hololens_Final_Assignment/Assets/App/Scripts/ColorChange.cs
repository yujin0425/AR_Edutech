using UnityEngine;

public class ColorChange : MonoBehaviour
{
    // 오브젝트와 색상을 지정하기 위한 구조체
    [System.Serializable]
    public class ObjectColorPair
    {
        public GameObject targetObject; // 색상이 변경될 오브젝트
        public Color color; // 변경될 색상
        [HideInInspector] public Renderer renderer; // 캐싱된 Renderer
    }

    // 오브젝트와 색상을 리스트로 관리
    public ObjectColorPair[] objectColorPairs;

    // 초기화 시 Renderer 캐싱
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

    // 버튼 클릭 시 호출되는 메서드
    public void ChangeObjectColor(int index)
    {
        // 유효한 인덱스인지 확인
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
