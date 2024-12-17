using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackRadius = 1f;

    [SerializeField] private PlayerControl controls;
    [SerializeField] private SpriteRenderer attackIndicator;

    private bool hasTargetInRange = false;

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRadius);
        List<PlayerControl> detectedPlayers = new List<PlayerControl>();
        foreach (Collider collider in colliders)
        {
            if (collider.transform.gameObject == this.gameObject) continue;
            if (!collider.gameObject.TryGetComponent(out PlayerControl player)) continue;
            if (player.isImmune) continue;
            detectedPlayers.Add(player);
        }

        hasTargetInRange = detectedPlayers.Count > 0;
        if (!hasTargetInRange)
        {
            attackIndicator.enabled = false;
            return;
        }
        else
        {
            attackIndicator.enabled = true;
        }

        detectedPlayers.OrderBy((a) => Vector3.Distance(transform.position, a.transform.position));
        if (controls.playerVariables.actionThing)
        {
            detectedPlayers[0].StunPlayer();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, attackRadius);
    }
}
