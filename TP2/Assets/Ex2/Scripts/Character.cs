using UnityEngine;

public class Character : MonoBehaviour
{
    private Vector3 _velocity = Vector3.zero;

    private Vector3 _acceleration = Vector3.zero;

    private const float AccelerationMagnitude = 2;

    private const float MaxVelocityMagnitude = 5;

    private const float DamagePerSecond = 50;

    private const float DamageRange = 10;

    private Collider2D[] accelerationCollidersCache; // Pre-allocate array

    private void Start() {
        accelerationCollidersCache = new Collider2D[20];
    }

    private void Update()
    {
        Move();
        DamageNearbyShapes();
        UpdateAcceleration();
    }

    private void Move()
    {
        _velocity += _acceleration * Time.deltaTime;
        if (_velocity.magnitude > MaxVelocityMagnitude)
        {
            _velocity = _velocity.normalized * MaxVelocityMagnitude;
        }
        transform.position += _velocity * Time.deltaTime;
    }

    private void UpdateAcceleration()
    {
        var direction = Vector3.zero;
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, DamageRange, accelerationCollidersCache);
        
        for (int i = 0; i < count; i++)
        {
            if (accelerationCollidersCache[i].TryGetComponent<Circle>(out var circle))
            {
                direction += (circle.transform.position - transform.position) * circle.Health;
            }
        }
        _acceleration = direction.normalized * AccelerationMagnitude;
    }

    private void DamageNearbyShapes()
    {
        var nearbyColliders = Physics2D.OverlapCircleAll(transform.position, DamageRange);

        // Si aucun cercle proche, on retourne a (0,0,0)
        if (nearbyColliders.Length == 0)
        {
            transform.position = Vector3.zero;
        }

        foreach(var nearbyCollider in nearbyColliders)
        {
            if (nearbyCollider.TryGetComponent<Circle>(out var circle))
            {
                circle.ReceiveHp(-DamagePerSecond * Time.deltaTime);
            }
        }
    }
}
