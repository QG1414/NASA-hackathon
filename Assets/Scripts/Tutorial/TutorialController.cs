using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : Singleton<TutorialController>
{
    [SerializeField]
    private CanvasGroup startingTutorial;

    [SerializeField]
    private CanvasGroup endTutorialPanel;

    [SerializeField]
    private CanvasGroup minigameTutorialPanel;

    [SerializeField]
    private List<TutorialStepsData> steps = new List<TutorialStepsData>();

    [SerializeField]
    private RectTransform indicator;

    [SerializeField]
    private RectTransform infoBox;

    [SerializeField]
    private RectTransform indicatorParent;

    private bool tutorialGoing = true;

    public bool TutorialGoing { get => tutorialGoing; set => tutorialGoing = value; }

    private bool minigameTutorialGoing = false;

    public bool MinigameTutorialGoing { get => minigameTutorialGoing; set => minigameTutorialGoing = value; }

    int currentQuest = -1;

    private RectTransform lastObject = null;

    private RectTransform lastInfoPanel = null;

    // Start is called before the first frame update
    void Start()
    {
        BuildingPanelController.Instance.BlockBuilding = true;
        ScreenTransition.Instance.startFadingOut();
        hideFinalTutorialPanel();
        startTutorial();
        HideMinigameTutorialPanel();
    }

    private void startTutorial()
    {
        startingTutorial.alpha = 1.0f;
        startingTutorial.interactable = true;
        startingTutorial.blocksRaycasts = true;
    }

    public void DisplayFinalTutorialPanel()
    {
        tutorialGoing = false;
        endTutorialPanel.alpha = 1;
        endTutorialPanel.interactable = true;
        endTutorialPanel.blocksRaycasts = true;
    }

    public void DisplayMinigameTutorialPanel()
    {
        minigameTutorialGoing = true;
        minigameTutorialPanel.alpha = 1;
        minigameTutorialPanel.interactable = true;
        minigameTutorialPanel.blocksRaycasts = true;
    }

    public void hideTutorialPanel()
    {
        startingTutorial.alpha = 0f;
        startingTutorial.interactable = false;
        startingTutorial.blocksRaycasts = false;
        NextQuest();
    }

    public void hideFinalTutorialPanel()
    {
        endTutorialPanel.alpha = 0;
        endTutorialPanel.interactable = false;
        endTutorialPanel.blocksRaycasts = false;
    }

    public void HideMinigameTutorialPanel()
    {
        minigameTutorialGoing = false;
        minigameTutorialPanel.alpha = 0;
        minigameTutorialPanel.interactable = false;
        minigameTutorialPanel.blocksRaycasts = false;
    }

    private void NextQuest()
    {
        currentQuest++;

        if (lastObject != null)
            DeleteLastObject();
        if (lastInfoPanel != null)
            DeleteLastPanel();

        if (currentQuest == steps.Count)
        {
            DisplayFinalTutorialPanel();
            return;
        }

        if (!steps[currentQuest].hiddenIndicator && BuildingPanelController.Instance.BlockBuilding)
        {
            BuildingPanelController.Instance.BlockBuilding = false;
        }

        if (steps[currentQuest].showInfoBox)
        {
            DisplayInfoBox();
        }

        if (steps[currentQuest].hiddenIndicator)
            return;

        lastObject = Instantiate(indicator, steps[currentQuest].questIndicatorPosition, Quaternion.identity,indicatorParent);
        lastObject.anchoredPosition3D = steps[currentQuest].questIndicatorPosition;
        lastObject.localScale = steps[currentQuest].questIndicatorScale;
    }



    private void DisplayInfoBox()
    {
        lastInfoPanel = Instantiate(infoBox, steps[currentQuest].questInfoPanelPosition, Quaternion.identity, indicatorParent);
        InfoPanelManager manager = lastInfoPanel.GetComponent<InfoPanelManager>();
        manager.SetInfo(Language.Instance.GetTranslation(steps[currentQuest].questName), Language.Instance.GetTranslation(steps[currentQuest].questDescription));

        if (steps[currentQuest].disableButton)
        {
            manager.OkButton.gameObject.SetActive(false);
        }
        else
        {
            manager.OkButton.onClick.AddListener(() => FinishQuest(steps[currentQuest]));
        }
        lastInfoPanel.anchoredPosition3D = steps[currentQuest].questInfoPanelPosition;
    }

    public void DeleteLastObject()
    {
        Destroy(lastObject.gameObject);
        lastObject = null;
    }
    private void DeleteLastPanel()
    {
        Destroy(lastInfoPanel.gameObject);
        lastInfoPanel = null;
    }

    public void FinishQuest(TutorialStepsData currentQuest)
    {
        if (currentQuest != steps[this.currentQuest])
            return;

        NextQuest();
    }
}
