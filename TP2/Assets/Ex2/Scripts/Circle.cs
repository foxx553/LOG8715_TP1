using UnityEngine;
using UnityEngine.Serialization;

public class Circle : MonoBehaviour
{
    [FormerlySerializedAs("I")] [HideInInspector]
    public int i;

    [FormerlySerializedAs("J")] [HideInInspector]
    public int j;

    public float Health { get; private set; }

    private const float BaseHealth = 1000;

    private const float HealingPerSecond = 1;
    private const float HealingRange = 3;

    private Collider2D[] nearbyCollidersCache; // Pre-allocate array

    private GridShape grid;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        nearbyCollidersCache = new Collider2D[20];
        Health = BaseHealth;
        grid = GameObject.FindFirstObjectByType<GridShape>(); // Cache GridShape reference
        spriteRenderer = GetComponent<SpriteRenderer>(); // Cache SpriteRenderer reference
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateColor();
        HealNearbyShapes();
    }

    private void UpdateColor()
    {
        spriteRenderer.color = grid.Colors[i, j] * Health / BaseHealth;
    }

    private void HealNearbyShapes()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, HealingRange, nearbyCollidersCache);
        for (int i = 0; i < count; i++)
        {
            if (nearbyCollidersCache[i].TryGetComponent<Circle>(out var circle))
            {
                circle.ReceiveHp(HealingPerSecond * Time.deltaTime);
            }
        }
    }

    public void ReceiveHp(float hpReceived)
    {
        Health += hpReceived;
        Health = Mathf.Clamp(Health, 0, BaseHealth);
    }
}
