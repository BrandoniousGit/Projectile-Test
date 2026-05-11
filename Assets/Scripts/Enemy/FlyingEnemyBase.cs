using UnityEditor;
using UnityEngine;
using static Globals;
using static UnityEngine.EventSystems.EventTrigger;

public class FlyingEnemyBase : EnemyBase
{
    [Header("Flying Enemy Variables")]
    public float HomeRange;
    public float Speed;
    public float TimeBeforeMove;
    public Vector2 MinMaxDistanceCanMove;

    public Vector3? TargetPosition;

    public LayerMask LayerMask;

    private float IdleMoveTimer;

    protected override void Update()
    {
        if (!IsAlive)
        {
            rb.useGravity = true;
            rb.freezeRotation = false;
            return;
        }

        base.Update();

        if (TargetPosition.HasValue)
        {
            var approximateDistance = (TargetPosition.Value - rb.transform.position).magnitude;

            if (approximateDistance <= 0.5f)
            {
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 0.5f);
                TargetPosition = null;
            }
            else
            {
                rb.linearVelocity = (TargetPosition.Value - rb.transform.position).normalized * Speed;
            }
        }
        else if (CurrentState == EnemyState.Idle)
        {
            IdleMoveTimer += Time.deltaTime;
        }

        if (IdleMoveTimer > TimeBeforeMove)
        {
            IdleMoveTimer = 0;
            FindNewTargetToMoveTo();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0.75f, 0, 0.2f);
        Gizmos.DrawSphere(rb.transform.position, MinMaxDistanceCanMove.y);

        if (TargetPosition.HasValue)
        {
            Gizmos.color = new Color(0, 0, 0.75f, 0.2f);
            Gizmos.DrawSphere(TargetPosition.Value, 0.5f);
            Debug.DrawLine(rb.transform.position, TargetPosition.Value, Color.red);
        }
    }

    private void FindNewTargetToMoveTo()
    {
        var sphereMoveRadius = Random.Range(MinMaxDistanceCanMove.x, MinMaxDistanceCanMove.y);
        var targetPosition = rb.transform.position + Random.onUnitSphere * sphereMoveRadius;

        if (Physics.Raycast(rb.transform.position, (targetPosition - rb.transform.position).normalized, out var hit, MinMaxDistanceCanMove.y, LayerMask, QueryTriggerInteraction.Ignore))
        {
            targetPosition = hit.point - ((targetPosition - rb.transform.position).normalized * 0.5f);
        }

        TargetPosition = targetPosition;
        rb.transform.LookAt(TargetPosition.Value);
    }
}
