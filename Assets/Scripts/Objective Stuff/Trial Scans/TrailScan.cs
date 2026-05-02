using System;
using UnityEngine;

public class TrailScan : MonoBehaviour
{
    private Guid ScanId;

    private float progress;
    private bool isComplete, mostRecentScan;

    public float maxProgress, regressRate;
    public int playersInScan = 0;

    public static event Action<Guid> OnPlayerEnterScan;

    private void Update()
    {
        if (mostRecentScan)
        {
            UpdateScanProgress();
        }
        
        if (isComplete)
        {
            Destroy(gameObject);
        }

        progress = Mathf.Clamp(progress, 0, maxProgress);
    }

    private void Awake()
    {
        ScanId = Guid.NewGuid();
    }

    private void OnEnable()
    {
        OnPlayerEnterScan += (scanId) =>
        {
            if (ScanId == scanId)
            {
                mostRecentScan = true;
                return;
            }

            mostRecentScan = false;
        };
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playersInScan -= 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnPlayerEnterScan?.Invoke(ScanId);

        if (other.tag == "Player")
        {
            playersInScan += 1;
            //ObjectiveManager.Instance.SetScanProgressBarVisible(true);
        }
    }

    private void UpdateScanProgress()
    {
        if (playersInScan > 0)
        {
            progress += Time.deltaTime;

            if (progress > maxProgress)
            {
                isComplete = true;
            }
        }
        else
        {
            if (progress > 0)
            {
                progress -= regressRate * Time.deltaTime;
            }
        }
    }
}
