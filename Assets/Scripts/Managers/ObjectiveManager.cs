using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    public Canvas ObjectiveCanvas;
    public GameObject ScanProgressBar;

    [SerializeField]
    private TextMeshProUGUI ScanText;

    [SerializeField]
    private Image ScanProgressBarImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(this);
        Instance = this;
    }

    public void SetScanProgressBarVisible(bool visible)
    {
        ScanProgressBar.gameObject.SetActive(visible);
    }

    public void UpdateScanProgressBar(float progress)
    {
        ScanText.SetText($"{Mathf.Round(progress * 100)}%");
        ScanProgressBarImage.fillAmount = progress;
    }
}