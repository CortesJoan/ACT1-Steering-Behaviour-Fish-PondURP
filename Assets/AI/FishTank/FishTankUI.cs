using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishTankUI : MonoBehaviour
{
    [SerializeField] FishCounterUI fishTotalCount;
    [SerializeField] FishTankController fishTankController;

    [SerializeField] GameObject fishCounterUIPrefab;
    Dictionary<FishType, FishCounterUI> fishCounterUIs;
    [SerializeField] Transform uiParent;

    // Start is called before the first frame update
    void Awake()
    {

        fishTotalCount.onValidInput.AddListener(UpdateDesiredCount);
        fishCounterUIs = new Dictionary<FishType, FishCounterUI>();
        fishTankController.onFishRegistered.AddListener(UpdateFishRegisteredUI);
        fishTankController.onFishUnregistered.AddListener(UpdateFishUnregisteredUI);
    }

    private void UpdateFishRegisteredUI(FishController newFish)
    {
        var newFishType = newFish.GetFishType();

        fishTotalCount.UpdateDesiredCount(fishTankController.GetCurrentFishCount);
        if (fishCounterUIs.ContainsKey(newFishType))
        {
            FishCounterUI fishCounterToUpdate = fishCounterUIs[newFish.GetFishType()];
            fishCounterToUpdate.UpdateDesiredCount(fishTankController.GetTypeFishCount(newFishType));
            return;
        }

        var newFishUI = Instantiate(fishCounterUIPrefab, uiParent).GetComponent<FishCounterUI>();
        newFishUI.UpdateName(newFish.name);
        newFishUI.UpdateDesiredCount(fishTankController.GetTypeFishCount(newFishType));
        fishCounterUIs.Add(newFishType, newFishUI); ;
        newFishUI.SubscribeToInputCount((d) => fishTankController.UpdateSpecificFishCount(newFishType, d));


    }
    private void UpdateFishUnregisteredUI(FishController fishToRemove)
    {
        return;

        //todo remove this (still not sure)
        if (fishCounterUIs.TryGetValue(fishToRemove.GetComponent<FishController>().GetFishType(), out FishCounterUI uiToRemove))
        {

            uiToRemove.UnSubscribeToInputCount(UpdateDesiredCount);
            Destroy(uiToRemove.gameObject);

        }
    }
    private void UpdateDesiredCount(int parsedCount)
    {
        fishTankController.UpdateDesiredTotalFishCount(parsedCount);

    }

}

