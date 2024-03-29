using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSocket : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private BuildingTypes buildingType;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Color inActiveColor;

    [SerializeField]
    private Color activeColor;

    [SerializeField]
    private Color selectedColor;

    [SerializeField]
    private Vector3 positionOffset;

    [SerializeField]
    private TranslateKeys lockedButtonText;

    [SerializeField]
    private ResearchData firstLevelData;

    public TranslateKeys LockedButtonText { get => lockedButtonText; set => lockedButtonText = value; }
    public ResearchData FirstLevelData { get => firstLevelData; set => firstLevelData = value; }

    public buildingFunctionality BuildingOnSocket = null;

    private bool selected = false;

    public bool Selected { get => selected; set => selected = value; }

    public BuildingTypes BuildingType { get => buildingType; set => buildingType = value; }

    private bool blockSocket = false;

    public bool BlockSocket { get => blockSocket; set => blockSocket = value; } 

    /*    private void OnEnable()
        {
            PlayerEvents.Instance.OnPlayerMouseDown += Pointer;
        }

        private void OnDisable()
        {
            if(PlayerEvents.Instance != null)
                PlayerEvents.Instance.OnPlayerMouseDown -= Pointer;
        }*/

    public void Pointer()
    {
        if (!selected)
            return;
        BuildingPanelController.Instance.DeInit();
        selected = false;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (blockSocket)
            return;

        BuildingScriptableObject buildingData = BuildingController.Instance.GetBuildingData(buildingType);
        if(buildingData == null)
        {
            return;
        }

        selected = BuildingPanelController.Instance.Init(buildingData, this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (blockSocket)
            return;

        if (selected)
            return;

        spriteRenderer.color = activeColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (blockSocket)
            return;

        if (selected)
            return;

        spriteRenderer.color = inActiveColor;
    }

    public void SelectedColor()
    {
        selected = true;
        spriteRenderer.color = selectedColor;
    }

    public void BuildBuilding()
    {
        BuildingScriptableObject buildingData = BuildingController.Instance.GetBuildingData(buildingType);
        GameObject createdObject = Instantiate(buildingData.BuildingPrefab, transform.position + positionOffset, Quaternion.identity);
        createdObject.GetComponent<buildingFunctionality>().Offset = positionOffset;
        createdObject.GetComponent<buildingFunctionality>().Socket = this;
        BuildingOnSocket = createdObject.GetComponent<buildingFunctionality>();
        this.gameObject.SetActive(false);
        BuildingController.Instance.CallOnBuild(buildingData);
    }

    public void UnSelect()
    {
        selected = false;
        spriteRenderer.color = inActiveColor;
    }
}

