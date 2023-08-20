using System.Collections;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("Arrow Setting"), Space(10)]
    [SerializeField] private Transform ArrowPrefab = null;
    [SerializeField] private Transform ArrowSpawnSpot = null;
    [SerializeField] private LayerMask Mask = default;
    [SerializeField] private float ArrowSpeed = 10f;
    [SerializeField] private float DisappearTimer = 3f;
    [SerializeField] private Vector3 ArrowVariance = new Vector3(.1f, .1f, .1f);

    public void Shoot()
    {
        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        Transform arrowPrefab = Instantiate(ArrowPrefab, ArrowSpawnSpot.position, ArrowSpawnSpot.rotation);

        Vector3 start = arrowPrefab.position;
        Vector3 dest = Vector3.zero;
        if (Physics.Raycast(ArrowSpawnSpot.position, GetDirection(), out RaycastHit hit, 10000, Mask))
        {
            dest = hit.point;
        }

        float timer = 0f;
        while (timer < 1f)
        {
            arrowPrefab.position = Vector3.Lerp(start, dest, timer);
            timer += ArrowSpeed * Time.deltaTime;

            yield return null;
        }
        arrowPrefab.position = Vector3.Lerp(start, dest, 1);

        timer = 0f;
        while (timer < DisappearTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(arrowPrefab.gameObject);
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = ArrowSpawnSpot.forward;

        direction += new Vector3(
                Random.Range(-ArrowVariance.x, ArrowVariance.x),
                Random.Range(-ArrowVariance.y, ArrowVariance.y),
                Random.Range(-ArrowVariance.z, ArrowVariance.z)
            );

        return direction;
    }
}
