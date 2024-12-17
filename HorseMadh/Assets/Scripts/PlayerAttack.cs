using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackRadius = 1f;

    [SerializeField] private PlayerControl controls;

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRadius);
        List<PlayerControl> detectedPlayers = new List<PlayerControl>();
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == this.gameObject) continue;
            collider.gameObject.TryGetComponent(out PlayerControl player);
            detectedPlayers.Add(player);
        }

        if (controls.playerVariables.actionThing)
        {
            Debug.Log("Attack");
            Debug.Log(string.Join(", ", detectedPlayers));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, attackRadius);
    }
}
