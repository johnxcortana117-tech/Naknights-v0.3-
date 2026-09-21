using UnityEngine;

/// <summary>
/// Controls navigation between the pause card and its inert audio settings panel.
/// </summary>
public class AudioSettingsController : MonoBehaviour
{
    [SerializeField] private GameObject pauseCard;
    [SerializeField] private GameObject audioSettingsPanel;

    /// <summary>
    /// Shows the audio settings panel and hides the main pause card.
    /// </summary>
    public void OpenAudioSettings()
    {
        if (pauseCard != null)
            pauseCard.SetActive(false);

        if (audioSettingsPanel != null)
            audioSettingsPanel.SetActive(true);
    }

    /// <summary>
    /// Hides the audio settings panel and returns to the main pause card.
    /// </summary>
    public void CloseAudioSettings()
    {
        if (audioSettingsPanel != null)
            audioSettingsPanel.SetActive(false);

        if (pauseCard != null)
            pauseCard.SetActive(true);
    }
}
