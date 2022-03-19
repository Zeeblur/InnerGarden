using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSystem : MonoBehaviour
{
    public List<GameObject>[] categories = new List<GameObject>[6];
    public List<GameObject> champFlowers = new List<GameObject>();
    public List<GameObject> soveFlowers = new List<GameObject>();
    public List<GameObject> loverFlowers = new List<GameObject>();
    public List<GameObject> mageFlowers = new List<GameObject>();
    public List<GameObject> neutralFlowers = new List<GameObject>();
    public List<GameObject> unassignedFlowers = new List<GameObject>();

    private string[] catNames = { "CHAMPION", "SOVEREIGN", "LOVER", "MAGICIAN", "NEUTRAL", "unass" };

    public Transform grassPrefab;
    public int instances = 500;
    public float radius = 5f;

    public GameObject[] gardenSpawners = new GameObject[4];

    private FlowerSpawner[] spawners; 

    void Awake()
    {
        //intialise the category lists
        categories[0] = champFlowers;
        categories[1] = soveFlowers;
        categories[2] = loverFlowers;
        categories[3] = mageFlowers;
        categories[4] = neutralFlowers;
        categories[5] = unassignedFlowers;

        // draw the grass
        for (int i = 0; i < instances; ++i)
        {
            Transform t = Instantiate(grassPrefab);
            Vector3 instanceLoc = Random.insideUnitCircle * radius;
            t.localPosition = new Vector3(instanceLoc.x, 0.0f, instanceLoc.y);
            t.SetParent(transform);
        }

        GetStats();
        // PrintUnassigned();

        ActivateGarden();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ActivateGarden()
    {
        int stage = GameManager.gardenCounter;
        print("Stage: " + stage);

        if (stage==0)
        {
            // We want TestGarden4
            gardenSpawners[3].SetActive(true);
        }
        else if (stage == 1)
        {
            // just for testing TODO score
            gardenSpawners[1].SetActive(true);
        }
        else if (stage == 3)
        {
            gardenSpawners[3].SetActive(false);
            gardenSpawners[1].SetActive(false);

            gardenSpawners[2].SetActive(true);
        }
    }

    void GetStats()
    {
        // find all the flower spawners
        // take the garden, print list of flowers
        
        for (int j = 0; j < gardenSpawners.Length; ++j)
        {
            spawners = gardenSpawners[j].GetComponentsInChildren<FlowerSpawner>();
            int[] totalInArchetype = { 0, 0, 0, 0, 0, 0 };

            foreach (FlowerSpawner fs in spawners)
            {
                foreach (GameObject go in fs.flowerObjects)
                {
                    bool found = false;

                    // which of 5 category does the plant belong to? 
                    for (int i = 0; i < categories.Length - 1; ++i)
                    {
                        if (categories[i].Contains(go))
                        {
                            // set the flower archetype in the map on the spawner
                            fs.AddFlowerMap(go, catNames[i]);
                            ++totalInArchetype[i];
                            
                            found = true;

                            break;
                        }
                    }

                    if (!found)
                    {
                        ++totalInArchetype[5];
                        unassignedFlowers.Add(go);
                    }
                }
            }

            string flValues = "Garden" + (j+1) + ": ";
            for (int i = 0; i < totalInArchetype.Length; ++i)
            {
                flValues += catNames[i] + " " + totalInArchetype[i] + "    ";
            }

            print(flValues);
        }
    }

    // Debug for 
    void PrintUnassigned()
    {
        foreach(GameObject go in unassignedFlowers)
        {
            print(go);
        }
    }
}
