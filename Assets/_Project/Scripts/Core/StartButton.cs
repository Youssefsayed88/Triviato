using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Image loadingImage;

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        loadingImage.fillAmount = 0f;
    }

    void OnStartButtonClicked()
    {
        startButton.interactable = false;
        StartCoroutine(FillLoadingImage());
    }

    IEnumerator FillLoadingImage()
    {
        while (loadingImage.fillAmount < 1f)
        {
            loadingImage.fillAmount += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        loadingImage.fillAmount = 1f;
        StartGame();
    }

    void StartGame() => GameManager.instance.StartGame();
}
