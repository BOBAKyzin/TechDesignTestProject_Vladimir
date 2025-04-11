using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject exitConfirmDialog;
    public GameObject loadingPanel;

    public CanvasGroup loadingPanelCanvasGroup;

    public void OnPlayPressed()
    {
        loadingPanel.SetActive(true);
        loadingPanelCanvasGroup.alpha = 0;
        loadingPanelCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.InOutQuad);

        DOVirtual.DelayedCall(1f, () =>
        {
            SceneManager.LoadSceneAsync("GameScene");
        });
    }


    public void OnExitPressed()
    {
        exitConfirmDialog.SetActive(true);
    }

    public void OnExitYes()
    {
        Application.Quit();
    }

    public void OnExitNo()
    {
        exitConfirmDialog.SetActive(false);
    }

    public void ToggleLanguage()
    {
        var current = LocalizationSettings.SelectedLocale.Identifier.Code;
        string nextCode;

        if (current == "en")
            nextCode = "ru-RU";
        else
            nextCode = "en"; 

        var locale = LocalizationSettings.AvailableLocales.Locales
            .Find(l => l.Identifier.Code == nextCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            RefreshAllLocalizedText();
        }
    }


    private void RefreshAllLocalizedText()
    {

        var allStringEvents = Resources.FindObjectsOfTypeAll<UnityEngine.Localization.Components.LocalizeStringEvent>();

        foreach (var loc in allStringEvents)
        {
            if (loc.gameObject.scene.IsValid())
            {
                loc.RefreshString();
            }
        }
    }


}
