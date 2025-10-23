using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // ����� ��ũ�ѵǴ� �ӵ� (Inspector���� ���� ����)
    public float scrollSpeed = 2f;

    // �ؽ�ó�� �����̱� ���� ��Ƽ������ ������ ����
    private Material mat;

    void Start()
    {
        // �� ��ũ��Ʈ�� �پ��ִ� ������Ʈ�� Renderer ������Ʈ�� �����ɴϴ�.
        // .material�� ȣ���ϸ� �� ������Ʈ ������ �� ��Ƽ���� �ν��Ͻ��� �����˴ϴ�.
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Time.time�� ������ ���۵� �� �� �ð��� �ǹ��մϴ�.
        // ���⿡ �ӵ��� ���� X�� ������(offset) ���� ����մϴ�.
        // (Y���� �������� �����Ƿ� 0���� �Ӵϴ�)
        float offsetX = Time.time * scrollSpeed;

        // Vector2 ���� ���� (x�� ���� ��, y�� 0)
        Vector2 newOffset = new Vector2(offsetX, 0);

        // ��Ƽ������ ���� �ؽ�ó ������ ���� �����մϴ�.
        // 1�ܰ迡�� Wrap Mode�� Repeat�� �����߱� ������
        // offsetX ���� 1.0�� �Ѿ�� �˾Ƽ� 0���� �ٽ� �ݺ��˴ϴ�.
        mat.mainTextureOffset = newOffset;
    }
}