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

    private string[] catNames = { "champ", "sover", "lover", "mage", "neutral", "unass" };

    public Transform grassPrefab;
    public int instances = 500;
    public float radius = 5f;

    public GameObject[] gardenSpawners = new GameObject[4];

    private FlowerSpawner[] spawners; 

    // Start is called before the first frame update
    void Start()
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
        PrintUnassigned();
    }

    // Update is called once per frame
    void Update()
    {
        
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

                    // which category does the plant belong to? 
                    for (int i = 0; i < categories.Length - 1; ++i)
                    {
                        if (categories[i].Contains(go))
                        {
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
