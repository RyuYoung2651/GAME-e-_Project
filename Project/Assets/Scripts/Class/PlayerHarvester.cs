using UnityEngine;

// 참조 0개 (Reference 0)
public class PlayerHarvester : MonoBehaviour
{
    // 참조 1개 (Reference 1)
    public float rayDistance = 5f;          // 채집 가능 거리 (Harvesting distance)

    // 참조 1개 (Reference 1)
    public LayerMask hitMask = ~0;          // 가능한 레이어 전부 다 (일단) (All possible layers (for now))

    // 참조 1개 (Reference 1)
    public int toolDamage = 1;              // 타격 데미지 (Hit damage)

    // 참조 1개 (Reference 1)
    private float hitCooldwon = 0.15f;      // 연타 간격 (Rapid hit interval)

    // 참조 2개 (Reference 2)
    private float _nextHitTime;

    // 참조 2개 (Reference 2)
    private Camera _cam;

    // 참조 3개 (Reference 3)
    public Inventory inventory;             // 플레이어 인벤 (없으면 자동 부착) (Player inventory (auto-attached if missing))

    // 참조 0개 (Reference 0)
    void Awake()
    {
        _cam = Camera.main;
        if (inventory == null) inventory = gameObject.AddComponent<Inventory>();
    }

    // 참조 0개 (Reference 0)
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= _nextHitTime)
        {
            _nextHitTime = Time.time + hitCooldwon;

            Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // 화면 중앙에서 레이 발사 (Cast ray from the center of the screen)
            if (Physics.Raycast(ray, out var hit, rayDistance, hitMask))
            {
                var block = hit.collider.GetComponent<Block>();
                if (block != null)
                {
                    block.Hit(toolDamage, inventory);
                }
            }
        }
    }
}