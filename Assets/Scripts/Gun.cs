using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Shoot Setting")]
    [SerializeField] private bool BulletSpread = false;
    [SerializeField] private Vector3 BulletSpreadVariance = new Vector3(.1f, .1f, .1f);
    [SerializeField] private float ShootingDelay = .5f;
    [SerializeField] private float ShootingRange = 1000;

    [Header("Bullet Setting")]
    [SerializeField] private Transform BulletSpawnPoint = null;
    [SerializeField] private TrailRenderer BulletTrail = null;
    [SerializeField] private ParticleSystem ShootingSystem = null;
    [SerializeField] private ParticleSystem ImpactSystem = null;
    [SerializeField] private LayerMask BulletMask = default;
    [SerializeField] private int GenerateCount = 5;
    [SerializeField] private float ImpactLeftTimer = 5f;

    private float lastShootingTime = 0f;

    private List<Vector3> startPointList = null;
    private List<Vector3> destPointList = null;
    private List<float> shootTimerList = null;
    private List<TrailRenderer> trailRendererList = null;
    private List<ParticleSystem> impactSystemList = null;

    private List<float> endShootTimerList = null;
    private List<TrailRenderer> endTrailRendererList = null;
    private List<ParticleSystem> endImpactSystemList = null;

    private Queue<TrailRenderer> trailRendererQueue = null;
    private Queue<ParticleSystem> impactSystemQueue = null;

    private void Start()
    {
        startPointList = new List<Vector3>();
        destPointList = new List<Vector3>();
        shootTimerList = new List<float>();
        trailRendererList = new List<TrailRenderer>();
        impactSystemList = new List<ParticleSystem>();

        endShootTimerList = new List<float>();
        endTrailRendererList = new List<TrailRenderer>();
        endImpactSystemList = new List<ParticleSystem>();

        trailRendererQueue = new Queue<TrailRenderer>();
        impactSystemQueue = new Queue<ParticleSystem>();
        GenerateBulletPrefab();
    }

    private void Update()
    {
        for (int i = 0; i < shootTimerList.Count; ++i)
        {
            trailRendererList[i].transform.position = Vector3.Lerp(startPointList[i], destPointList[i], shootTimerList[i]);
            shootTimerList[i] += Time.deltaTime / trailRendererList[i].time;

            if (shootTimerList[i] > 1f)
            {
                trailRendererList[i].gameObject.SetActive(false);
                impactSystemList[i].gameObject.SetActive(true);
                endTrailRendererList.Add(trailRendererList[i]);
                endImpactSystemList.Add(impactSystemList[i]);
                endShootTimerList.Add(0f);

                startPointList.RemoveAt(i);
                destPointList.RemoveAt(i);
                shootTimerList.RemoveAt(i);
                trailRendererList.RemoveAt(i);
                impactSystemList.RemoveAt(i--);
            }
        }

        for (int i = 0; i < endShootTimerList.Count; ++i)
        {
            endShootTimerList[i] += Time.deltaTime;
            if (endShootTimerList[i] > ImpactLeftTimer)
            {
                //endTrailRendererList[i].transform.position = Vector3.zero;
                //endImpactSystemList[i].transform.position = Vector3.zero;
                //endImpactSystemList[i].transform.rotation = Quaternion.identity;

                endImpactSystemList[i].gameObject.SetActive(false);
                trailRendererQueue.Enqueue(endTrailRendererList[i]);
                impactSystemQueue.Enqueue(endImpactSystemList[i]);

                endTrailRendererList.RemoveAt(i);
                endImpactSystemList.RemoveAt(i);
                endShootTimerList.RemoveAt(i--);
            }
        }
    }

    public void Shoot()
    {
        if (lastShootingTime + ShootingDelay < Time.time)
        {
            Vector3 direction = GetDirection();

            if (Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, ShootingRange, BulletMask))
            {
                ShootingSystem.Play();
                if (trailRendererQueue.Count.Equals(0))
                    GenerateBulletPrefab();

                TrailRenderer trailRenderer = trailRendererQueue.Dequeue();
                ParticleSystem impactSystem = impactSystemQueue.Dequeue();
                trailRenderer.gameObject.SetActive(true);

                trailRenderer.transform.position = BulletSpawnPoint.position;
                impactSystem.transform.rotation = Quaternion.LookRotation(hit.normal);
                impactSystem.transform.position = hit.point;

                startPointList.Add(BulletSpawnPoint.position);
                destPointList.Add(hit.point);
                shootTimerList.Add(0f);

                trailRendererList.Add(trailRenderer);
                impactSystemList.Add(impactSystem);

                lastShootingTime = Time.time;
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;
        if (BulletSpread)
        {
            direction += new Vector3(
                    Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                    Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                    Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)                
                );
            direction.Normalize();
        }
        return direction;
    }

    private void GenerateBulletPrefab()
    {
        for (int i = 0; i < GenerateCount; ++i)
        {
            TrailRenderer trailRenderer = Instantiate(BulletTrail, Vector3.zero, Quaternion.identity);
            ParticleSystem impactSystem = Instantiate(ImpactSystem, Vector3.zero, Quaternion.identity);
            trailRendererQueue.Enqueue(trailRenderer);
            impactSystemQueue.Enqueue(impactSystem);
            trailRenderer.gameObject.SetActive(false);
            impactSystem.gameObject.SetActive(false);
        }
    }
}
