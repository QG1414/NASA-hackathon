using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RekrutacjaBuilding : buildingFunctionality
{
    [SerializeField]
    private int amountToGive;

    [SerializeField]
    private float timeToGive;

    [SerializeField]
    private int buildingLevel = 1;

    [SerializeField]
    private TranslateKeys functionButtonText;

    [SerializeField]
    private ResearchData Level1;

    [SerializeField]
    private ResearchData Level2;

    [SerializeField]
    private ResearchData Level3;

    [SerializeField]
    private bool HasUpgrades = false;

    [SerializeField]
    private Sprite Level2Sprite;

    [SerializeField]
    private Sprite Level3Sprite;



    private float time = 0;

    public int AmountToGive { get => amountToGive; set => amountToGive = value; }

    private void OnEnable()
    {
        SchoolingBuilding schooling = FindObjectOfType<SchoolingBuilding>();

        if (schooling != null)
        {
            amountToGive += schooling.AddBuildingToList(this);
        }
    }

    private void Awake()
    {
        time = timeToGive;
        currentLevel = Level1;
    }

    public override void ChangeCurrentLevel(int level)
    {
        if (level == 1)
            currentLevel = Level1;
        else if (level == 2)
        {
            currentLevel = Level2;
            amountToGive += 5;
            SpriteRendererO.sprite = Level2Sprite;
        }
        else
        {
            currentLevel = Level3;
            amountToGive += 5;
            SpriteRendererO.sprite = Level3Sprite;
        }
    }

    public override void DisplayBuilding()
    {
        base.DisplayBuilding();
        currentUI.FunctionButtonText.text = Language.Instance.GetTranslation(functionButtonText);
        currentUI.FunctionButton.onClick.RemoveAllListeners();
        currentUI.FunctionButton.onClick.AddListener(() => UpgradeBuilding());

        if (HasUpgrades)
        {
            currentUI.DeactiveButton();
            if (currentLevel == Level1)
            {

                if (TechTreeController.Instance.isUnlocked(Level2))
                {

                    currentUI.ActivePrice(Level2.BuildingCost);
                    currentUI.ActivateButton();
                }
            }
            else if (currentLevel == Level2)
            {
                if (TechTreeController.Instance.isUnlocked(Level3))
                {
                    currentUI.ActivePrice(Level3.BuildingCost);
                    currentUI.ActivateButton();
                }
            }
        }
    }

    private void UpgradeBuilding()
    {
        if (currentLevel == Level1)
        {
            if (!MoneyController.Instance.CheckIfCanPurchase(Level2.BuildingCost))
                return;

            MoneyController.Instance.RemoveCash(Level2.BuildingCost);

            SpriteRendererO.sprite = Level2Sprite;
            currentLevel = Level2;
            amountToGive += 5;
            base.HideBuilding();
        }
        else if (currentLevel == Level2)
        {
            if (!MoneyController.Instance.CheckIfCanPurchase(Level3.BuildingCost))
                return;

            MoneyController.Instance.RemoveCash(Level3.BuildingCost);

            SpriteRendererO.sprite = Level3Sprite;

            currentLevel = Level3;
            amountToGive += 5;
            base.HideBuilding();
        }
    }

    private void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            time = timeToGive;
            MoneyController.Instance.AddCash(amountToGive);
        }
    }

    public void ReturnToMainValues()
    {
        if (currentLevel == Level1)
        {
            amountToGive = 5;
        }
        else if (currentLevel == Level2)
        {
            amountToGive = 10;
        }
        else
            amountToGive = 15;
    }
}
