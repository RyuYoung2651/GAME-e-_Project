using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // 배경이 스크롤되는 속도 (Inspector에서 조절 가능)
    public float scrollSpeed = 2f;

    // 텍스처를 움직이기 위해 머티리얼을 저장할 변수
    private Material mat;

    void Start()
    {
        // 이 스크립트가 붙어있는 오브젝트의 Renderer 컴포넌트를 가져옵니다.
        // .material을 호출하면 이 오브젝트 전용의 새 머티리얼 인스턴스가 생성됩니다.
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Time.time은 게임이 시작된 후 총 시간을 의미합니다.
        // 여기에 속도를 곱해 X축 오프셋(offset) 값을 계산합니다.
        // (Y축은 움직이지 않으므로 0으로 둡니다)
        float offsetX = Time.time * scrollSpeed;

        // Vector2 값을 생성 (x는 계산된 값, y는 0)
        Vector2 newOffset = new Vector2(offsetX, 0);

        // 머티리얼의 메인 텍스처 오프셋 값을 변경합니다.
        // 1단계에서 Wrap Mode를 Repeat로 설정했기 때문에
        // offsetX 값이 1.0을 넘어가도 알아서 0부터 다시 반복됩니다.
        mat.mainTextureOffset = newOffset;
    }
}