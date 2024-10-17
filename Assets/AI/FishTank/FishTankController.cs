using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FishTankController : MonoBehaviour
{
    [Header("Fish to Spawn")]
    [SerializeField] List<FishController> availableTypesOfFish;
    [SerializeField] int desiredFishCount;

    [Header("Random Scale")]
    [SerializeField] bool toggleRandomScale= true;
    [SerializeField] bool proportionalScale= false;
    [SerializeField] Vector2 minRandomScale;
    [SerializeField] Vector2 maxRandomScale;
    [Header("SpawnDistance")]
    [SerializeField] float spawnRangeX = 20;
    [SerializeField] float spawnRangeY = 20;
    private int currentTotalFishCount;
    private Dictionary<FishType, int> fishTypeCount;
    List<FishController> instantiatedFishes = new List<FishController>();

    public UnityEvent<FishController> onFishRegistered;
    public UnityEvent<FishController> onFishUnregistered;


    void Start()
    {
        availableTypesOfFish.Sort((fish1, fish2) => fish1.GetFishType().CompareTo(fish2.GetFishType()));
        instantiatedFishes = new List<FishController>();
        if (desiredFishCount < availableTypesOfFish.Count)
        {
            desiredFishCount = availableTypesOfFish.Count;
        }
        fishTypeCount = new Dictionary<FishType, int>();

        foreach (var type in availableTypesOfFish)
        {
            if (!type) continue;
            fishTypeCount.Add(type.GetFishType(), 0);
        }
        foreach (var type in availableTypesOfFish)
        {
            if (!type) continue;
            var newFish = InstantiateASpecificFish(type.GetFishType());
            RegisterFish(newFish);
        }

        SpawnDesiredNumberOfFish();

    }

    public void UpdateDesiredTotalFishCount(int newNumber)
    {
        if (newNumber < 0)
        {
            return;
        }
        bool willAddFish = newNumber > desiredFishCount;
        desiredFishCount = newNumber;
        if (willAddFish)
        {
            SpawnDesiredNumberOfFish();
        }
        else
        {
            PurgeFishUntilDesiredNumber();
        }
    }
    void PurgeFishUntilDesiredNumber()
    {
        for (int i = currentTotalFishCount; i > desiredFishCount; i--)
        {

            DestroyFish(instantiatedFishes[currentTotalFishCount - 1].gameObject);
        }
    }

    void SpawnDesiredNumberOfFish()
    {
        for (int i = currentTotalFishCount; i < desiredFishCount; i++)
        {
            var newFish = InstantiateARandomFish();
            RegisterFish(newFish);
        }
    }
    FishController InstantiateARandomFish()
    {
        var currentFish = Instantiate(availableTypesOfFish[Random.Range(0, availableTypesOfFish.Count)], this.transform);
        currentFish.transform.position = this.transform.position + new Vector3(Random.Range(-spawnRangeX, spawnRangeX), Random.Range(-spawnRangeY, spawnRangeY), 0);

        if (toggleRandomScale) {
            float xScale =  Random.Range(minInclusive: minRandomScale.x, maxInclusive: maxRandomScale.x);
            currentFish.transform.localScale = proportionalScale ? new Vector2(xScale,xScale): new Vector2(xScale,
            Random.Range(minInclusive: minRandomScale.y, maxInclusive: maxRandomScale.y));
            currentFish.SetMass(xScale);
        }
        return currentFish;

    }
    FishController InstantiateASpecificFish(FishType fishType)
    {
        var currentFish = Instantiate(availableTypesOfFish[(int)fishType], this.transform);
        currentFish.transform.position = this.transform.position + new Vector3(Random.Range(-spawnRangeX, spawnRangeX), Random.Range(-spawnRangeY, spawnRangeY), 0);
        return currentFish;

    }
    void RegisterFish(FishController fishToRegister)
    {
        instantiatedFishes.Add(fishToRegister);
        fishToRegister.onFishKilled.AddListener(DestroyFish);
        if (fishTypeCount.TryGetValue(fishToRegister.GetFishType(), out int previousCount))
        {
            fishTypeCount[fishToRegister.GetFishType()] = previousCount + 1;
            UpdateFishCount();
            onFishRegistered?.Invoke(fishToRegister);

        }
    }
    void DestroyFish(GameObject fish)
    {
        DestroyFish(fish.GetComponent<FishController>());
    }
    void DestroyFish(FishController fishToDestroy)
    {
        instantiatedFishes.Remove(fishToDestroy);

        fishToDestroy.onFishKilled.RemoveListener(DestroyFish);
        if (fishTypeCount.TryGetValue(fishToDestroy.GetFishType(), out int previousCount))
        {
            fishTypeCount[fishToDestroy.GetFishType()] = previousCount - 1;
            UpdateFishCount();
            onFishRegistered?.Invoke(fishToDestroy);

        }
        Destroy(fishToDestroy.gameObject);
        UpdateFishCount();

        onFishUnregistered?.Invoke(fishToDestroy);

    }
    void DestroyFishOfType(FishType fishType,int amount)
    {

        var fishesToDestroy = instantiatedFishes.FindAll((fish) => fish.GetFishType() == fishType);
        if (amount >= fishesToDestroy.Count)
        {
            amount = fishesToDestroy.Count;
        }
        for (int i = 0; i < amount; i++)
        {
            FishController fish = fishesToDestroy[i];
            DestroyFish(fish);
        }
    }
    void UpdateFishCount()
    {
        currentTotalFishCount = instantiatedFishes.Count;
    }
    public void SpawnSpecificTypeOfFish(FishType fishType, int amount)
    {

        for (int i = 0; i < amount; i++)
        {
            var newFish = InstantiateASpecificFish(fishType);
            RegisterFish(newFish);
            desiredFishCount = currentTotalFishCount;
        }
    }
    public int GetTypeFishCount(FishType fishType)
    {
        return fishTypeCount[fishType];
    }

    internal void UpdateSpecificFishCount(FishType newFishType, int newNumber)
    {

        if (newNumber < 0)
        {
            return;
        }
        bool willAddFish = newNumber > fishTypeCount[newFishType];
        if (willAddFish)
        {
                 SpawnSpecificTypeOfFish(newFishType, newNumber - fishTypeCount[newFishType]);
        }
        else
        {
            DestroyFishOfType(newFishType,  fishTypeCount[newFishType]-newNumber);
            

        }
    }

    public int GetCurrentFishCount => currentTotalFishCount;
}

