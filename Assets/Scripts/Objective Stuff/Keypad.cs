using TMPro;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    [SerializeField] private string passcode;
    private string enteredCode = string.Empty;
    public TextMeshProUGUI screenText;

    private void OnEnable()
    {
        ResetScreen();
    }

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        passcode = GeneratePasscode();
    }

    private string GeneratePasscode()
    {
        var a = Random.Range(0, 10).ToString();
        var b = Random.Range(0, 10).ToString();
        var c = Random.Range(0, 10).ToString();
        var d = Random.Range(0, 10).ToString();
        var e = Random.Range(0, 10).ToString();
        var f = Random.Range(0, 10).ToString();

        return a + b + c + d + e + f;
    }

    public void ReceiveKeyPress(string buttonName)
    {
        switch (buttonName)
        {
            case "Confirm":
                CheckCode();
                return;
            case "Reset":
                ResetScreen();
                return;
        }

        if (enteredCode.Length < 6)
        {
            enteredCode += buttonName;
            screenText.SetText($"> {enteredCode}");
        }
    }

    private void CheckCode()
    {
        if (enteredCode == passcode)
        {
            OnKeypadSuccess();
            return;
        }

        OnKeypadFailed();
    }

    private void OnKeypadSuccess()
    {
        Debug.Log("Correct!");
    }    
    
    private void OnKeypadFailed()
    {
        Debug.Log("Wrong!");
        ResetScreen();
    }

    private void ResetScreen()
    {
        screenText.SetText("> ");
        enteredCode = string.Empty;
    }
}
