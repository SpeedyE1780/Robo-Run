using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetAttractor : MonoBehaviour
{
    public Transform Player;
    float range;

    public void InitializeMagnet(float r)
    {
        range = r;
        StartCoroutine("Attract");
    }

    IEnumerator Attract()
    {
        while (true)
        {
            foreach (Collider collider in Physics.OverlapSphere(this.transform.position, range))
            {
                if (collider.tag == "Coin")
                {
                    collider.gameObject.GetComponent<PickUpController>().StartSeek(Player);
                }
            }

            yield return null;
        }
    }

    private void LateUpdate()
    {
        transform.position = Player.position + Vector3.up * 0.2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, range);
    }
}