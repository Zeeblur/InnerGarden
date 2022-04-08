using System;
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

    private string[] catNames = { "SOVEREIGN", "CHAMPION", "LOVER", "MAGICIAN", "NEUTRAL", "unass" };

    public Transform grassPrefab;
    public int instances = 250;
    public float radius = 5f;

    public int gardenIndex = 1;
    private int stage = 0;

    public GameObject[] gardenSpawners = new GameObject[4];

    private FlowerSpawner[] spawners;

    void Awake()
    {
        //intialise the category lists
        categories[0] = soveFlowers;
        categories[1] = champFlowers;
        categories[2] = loverFlowers;
        categories[3] = mageFlowers;
        categories[4] = neutralFlowers;
        categories[5] = unassignedFlowers;

        stage = GameManager.gardenCounter;
        stage /= 5;
        print("Counter: " + GameManager.gardenCounter);
        print("Stage: " + stage);
        print("G: " + GameManager.gardenVisits);

        if (stage == 0)
            stage = 2;

        // draw the grass
        for (int i = 0; i < instances * stage; ++i)
        {
            Transform t = Instantiate(grassPrefab);
            Vector3 instanceLoc = UnityEngine.Random.insideUnitCircle * radius;
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

    public struct Rank : IComparable<Rank>
    {
        public int location;
        public uint score;
        public Rank(int inLoc, uint inScore)
        {
            location = inLoc;
            score = inScore;
        }

        public int CompareTo(Rank other)
        {
            if (this.score < other.score)
            {
                return 1;
            }
            else if (this.score > other.score)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

    }

    List<Rank> GetRank(uint[] input)
    {
        // rank them 
        List<Rank> prevHScoreIdx = new List<Rank>();
        uint highestScore = 0;
        int hsIndex = 0;
        for (int i = 0; i < input.Length; ++i)
        {

            Rank rk = new Rank(i, input[i]);
            prevHScoreIdx.Add(rk);

            if (input[i] > highestScore)
            {
                highestScore = input[i];
                hsIndex = i;


            }

        }

        prevHScoreIdx.Sort();
        
        

        foreach (Rank r in prevHScoreIdx)
        {
            print((Narrative.Archetype)r.location);
        }
        return prevHScoreIdx;
    }

    void ActivateGarden()
    {
        // whats your highest score?
        uint[] scores = GameManager.GetScores();

        uint highestScore = 0;
        int hsIndex = 0;
        for (int i = 0; i < scores.Length; ++i)
        {
            if (scores[i] > highestScore)
            {
                highestScore = scores[i];
                hsIndex = i;
            }

        }

        print("High score: " + (Narrative.Archetype)hsIndex + highestScore);

        if (stage==1 || stage==0)
        {
            // We want TestGarden4
            gardenSpawners[3].SetActive(true);
            gardenIndex = 3;            

            if (highestScore > 0)
            {
                // for the spawner in garden 4 we want to switch the flowers to the highest scoring one. 
                FlowerSpawner[] currentSpawners = gardenSpawners[3].GetComponentsInChildren<FlowerSpawner>();
                foreach (FlowerSpawner fs in currentSpawners)
                {
                    if ((Narrative.Archetype)hsIndex == Narrative.Archetype.MAGICIAN)
                    {
                        fs.count = 5; // magician looks sparse so bump out a bit
                    }
                    fs.ChangeFlowerType((Narrative.Archetype)hsIndex, categories[hsIndex]);
                }
            }
        }
        else if (stage == 2)
        {
            // just for testing TODO score

            // test input 3 1 3 4
            uint[] scoresIn = { 3, 1, 3, 4 };

            scoresIn = scores;

            gardenSpawners[1].SetActive(true);
            gardenIndex = 1;

            // sub the pink layer for secondary pink scorer (orignial Sovereign)
            // who's higehst sovern/magician?
            FlowerSpawner[] garden2Spawners = gardenSpawners[1].GetComponentsInChildren<FlowerSpawner>();

            List<FlowerSpawner> pinkSpawns = new List<FlowerSpawner>();
            List<FlowerSpawner> blueSpawns = new List<FlowerSpawner>();
            List<FlowerSpawner> whiteSpawns = new List<FlowerSpawner>();
            
            foreach(FlowerSpawner fs in garden2Spawners)
            {
                if (fs.transform.parent.name == "subPink")
                {
                    pinkSpawns.Add(fs);
                }
                else if (fs.transform.parent.name == "subBlue")
                {
                    blueSpawns.Add(fs);
                }
                else if (fs.transform.parent.name == "subWhite")
                {
                    whiteSpawns.Add(fs);
                }
            }

            List<Rank> ranking = GetRank(scoresIn);
            int rnkIdx = 0;

            foreach(FlowerSpawner fSpawn in pinkSpawns)
            {
                // change the flowers into ranking[0](archetype) pink's flower.
                Rank rank = ranking[rnkIdx];

                print(categories[rank.location]);


                List<GameObject> inFlowers = new List<GameObject>();
                foreach (GameObject go in categories[rank.location])
                {
                    print(go.name);
                    if (go.name.Contains("P") && !go.name.Contains("Pu"))
                    {
                        inFlowers.Add(go);
                        print("P found");
                    }
                }

                fSpawn.ChangeFlowerType((Narrative.Archetype)rank.location, inFlowers);
            }

            ++rnkIdx;

            // blue
            foreach (FlowerSpawner fSpawn in blueSpawns)
            {
                // change the flowers into ranking[1](archetype) blues flower.
                Rank rank = ranking[rnkIdx];

                print(categories[rank.location]);


                List<GameObject> inFlowers = new List<GameObject>();
                foreach (GameObject go in categories[rank.location])
                {
                    print("what " + go.name + " " + rank.location);
                    if (go.name.Contains("Blu") || go.name.Contains("-b"))
                    {
                        inFlowers.Add(go);
                        print("Blue found");
                    }
                }

                if (inFlowers.Count == 0)
                {
                    Debug.Log("No Flower selected to replace bluee, try again with next flower:" );
                    ++rnkIdx;
                    rank = ranking[rnkIdx];
                    foreach (GameObject go in categories[rank.location])
                    {
                        print("what " + go.name + " " + rank.location);
                        if (go.name.Contains("Blu") || go.name.Contains("-b"))
                        {
                            inFlowers.Add(go);
                            print("Blue found");
                        }
                    }
                }


                fSpawn.ChangeFlowerType((Narrative.Archetype)rank.location, inFlowers);
            }

            ++rnkIdx;

            // white
            foreach (FlowerSpawner fSpawn in whiteSpawns)
            {
                // change the flowers into ranking[2](archetype) white flower.
                Rank rank = ranking[rnkIdx];

                List<GameObject> inFlowers = new List<GameObject>();
                foreach (GameObject go in categories[rank.location])
                {
                    if (go.name.Contains("W"))
                    {
                        inFlowers.Add(go);
                        print("White found");
                    }
                }

                fSpawn.ChangeFlowerType((Narrative.Archetype)rank.location, inFlowers);
            }

        }
        else if (stage == 3)
        {
            gardenSpawners[3].SetActive(false);
            gardenSpawners[1].SetActive(false);

            gardenSpawners[2].SetActive(true);
            gardenIndex = 2;
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
