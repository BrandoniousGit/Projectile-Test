using UnityEngine;

public class ShooterEnemy : EnemyBase
{
    protected override void Update()
    {
        base.Update();

        var playerPos = PlayerManager.Instance.GetPlayerPosition();
        LayerMask mask = LayerMask.GetMask("Player");

        Debug.DrawRay(transform.position, playerPos - transform.position, Color.red, Time.deltaTime);

        if (Physics.Raycast(transform.position, playerPos - transform.position, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
        {
            var targetPos = new Vector3(playerPos.x, transform.position.y, playerPos.z);
            transform.LookAt(targetPos);
        }
    }
}
