using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FishCounterUI : MonoBehaviour
{
    [SerializeField] TMP_Text fishCountName;
    [SerializeField] TMP_InputField fishCount;
    public UnityEvent<int> onValidInput;

    public void Start()
    {
        fishCount.onEndEdit.AddListener(ResetToZero);
    }
    string previousValidCountText="0";
    private void ResetToZero(string value)
    {
        if (int.TryParse(value, out int parsedCount))
        {

            if (parsedCount < 0)
            {
                fishCount.text = previousValidCountText;
            }
            else
            {
                previousValidCountText = value;
                onValidInput?.Invoke(parsedCount);
            }

        }
        else
        {
            fishCount.text = previousValidCountText;
        }
    }

    public FishCounterUI(TMP_Text fishCountName, TMP_InputField fishCount)
    {
        this.fishCountName = fishCountName;
        this.fishCount = fishCount;
    }

    public void UpdateName(string newName)
    {
        newName = newName.Replace("(Clone)", "");
        fishCountName.text = newName;
    }
    public void UpdateDesiredCount(int newCount)
    {
        fishCount.text = newCount.ToString();
        previousValidCountText = newCount.ToString();
    }

    public void SubscribeToInputCount(UnityAction<int> unityAction)
    {
        onValidInput.AddListener(unityAction);
    }


    public void UnSubscribeToInputCount(UnityAction<int> unityAction)
    {
        onValidInput.RemoveListener(unityAction);
    }

}