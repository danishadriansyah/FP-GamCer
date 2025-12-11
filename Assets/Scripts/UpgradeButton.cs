using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    private UpgradeData currentUpgrade;
    private Image buttonImage;
    private Button myButton;

    void Awake()
    {
        // Automatically find the Image component on this GameObject
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();

        // AUTOMATIC FIX: Tell the button to listen to this script!
        if (myButton != null)
        {
            myButton.onClick.RemoveAllListeners(); // Clear old connections
            myButton.onClick.AddListener(OnClick); // Connect our function
        }
    }

    public void SetUpgrade(UpgradeData data)
    {
        currentUpgrade = data;

        // Swap the sprite to your pre-generated card
        buttonImage.sprite = data.fullCardImage;

        // Ensure the sprite doesn't get squashed/stretched weirdly
        buttonImage.preserveAspect = true;
    }

    public void OnClick()
    {
        // Find player and apply stats (Same logic as before)
        if (GameManager.instance != null)
        {
            Debug.Log("Upgrade Button OnClick");
            GameManager.instance.ApplyUpgrade(currentUpgrade);
            Debug.Log("Upgrade Applied");
        }

    }
}