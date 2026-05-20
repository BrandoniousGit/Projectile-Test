using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindXItemsSmall : ObjectiveBase
{
    public GameObject smallItemToSpawn;

    public override void SetData(ObjectiveData data)
    {
        Data = data;
        smallItemToSpawn = (GameObject)Resources.Load($"Objectives/{data.ObjectiveItemResourceName}");
        
        UpdateObjectiveText();
    }

    public override void SetupObjectiveItems()
    {
        if (smallItemToSpawn == null)
        {
            Debug.Log("Small item prefab has not been set");
            return;
        }

        var possibleSpawns = GameObject.FindGameObjectsWithTag("CollectSmallSpawn").ToList();

        if (possibleSpawns.Count < Data.ProgressNeeded)
        {
            Debug.Log("Not enough spawns for objective, you need to re-generate level");
            return;
        }

        var selectedSpawns = new List<GameObject>();

        for (int i = 1; i <= Data.ProgressNeeded; i++)
        {
            var randomIndex = Random.Range(0, possibleSpawns.Count);

            selectedSpawns.Add(possibleSpawns[randomIndex]);
            possibleSpawns.RemoveAt(randomIndex);
        }

        foreach (var spawn in selectedSpawns)
        {
            Instantiate(smallItemToSpawn, spawn.transform.position, spawn.transform.rotation);
        }
    }
}