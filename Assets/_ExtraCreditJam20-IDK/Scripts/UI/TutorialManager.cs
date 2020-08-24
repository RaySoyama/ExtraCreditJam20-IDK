using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> AllSlides = new List<GameObject>();

    [SerializeField, ReadOnlyField]
    private int currentSlide = 0;

    [SerializeField]
    private SceneFunctions sceneFunc = null;

    [SerializeField, ReadOnlyField]
    private string targetScene = "exit";

    void Start()
    {
        currentSlide = 0;

        DisabelAllSlides();
    }

    [ContextMenu("Start Slides")]
    public void StartSlides()
    {
        AllSlides[currentSlide].SetActive(true);
    }

    public void StartSlides(string sceneName)
    {
        AllSlides[currentSlide].SetActive(true);
        targetScene = sceneName;
    }

    [ContextMenu("Next Slide")]
    public void NextSlide()
    {
        if (currentSlide == AllSlides.Count - 1)
        {
            DisabelAllSlides();
            PostTutorialAction();
            return;
        }
        AllSlides[currentSlide].SetActive(false);
        currentSlide++;
        AllSlides[currentSlide].SetActive(true);
    }

    public void DisabelAllSlides()
    {
        foreach (GameObject slide in AllSlides)
        {
            slide.SetActive(false);
        }
    }

    public void PostTutorialAction()
    {
        if (targetScene == "exit")
        {
            sceneFunc.CloseGame();
            return;
        }

        sceneFunc.ChangeScene(targetScene);
    }

}
