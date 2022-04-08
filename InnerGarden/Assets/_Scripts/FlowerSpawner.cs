using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Narrative;

public class FlowerSpawner : MonoBehaviour
{
    public int count = 1;
    public float radius = 5;
    public GameObject[] flowerObjects = new GameObject[3];
    public Dictionary<GameObject, string> flowerMap = new Dictionary<GameObject, string>();

    private Transform circleTrans;

    private int prevCount;
    private float prevRadius;
    private Transform prevPos;
    public List<GameObject> spawned;
    public Vector3 epsilon;

    public bool affectedByScore = true;
    public bool dominantMod = false;
    private bool prevCheck = true;
    public float scoreModifier = 2.0f;
    private float prevScoreMod = 0.0f;
    public uint[] flowerScores = { 0, 0, 4, 0 };
    private uint[] prevScores = new uint[4];

    public bool needUpdate = false;


    // Start is called before the first frame update
    void Start()
    {
        circleTrans = this.transform.GetChild(0);
        prevPos = circleTrans;
        spawned = new List<GameObject>();

        GameManager.PrintScores();
        flowerScores = GameManager.GetScores();

        prevScoreMod = scoreModifier;

        for (uint i = 0; i < flowerScores.Length; ++i)
        {
            prevScores[i] = (uint)flowerScores.GetValue(i);
        }

        Redraw();
    }

    // Update is called once per frame
    void Update()
    {
        circleTrans = this.transform.GetChild(0);
        flowerScores = GameManager.GetScores();

        // check score
        for (uint i = 0; i < flowerScores.Length; ++i)
        {
            if (flowerScores[i] != prevScores[i])
            {
                needUpdate = true;
                break;
            }
        }


        if (prevCount != count || prevRadius != radius || prevPos != circleTrans
            || scoreModifier != prevScoreMod
            || prevCheck != affectedByScore
            || needUpdate)
        {
            DeleteAll();
            Redraw();
        }
    }

    void DeleteAll()
    {
        foreach (GameObject go in spawned)
        {
            Destroy(go);
        }
        spawned.Clear();
    }

    void Redraw()
    {
        print("Redraw");
        if (!gameObject.activeSelf)
            return;

        needUpdate = false;
        prevPos = circleTrans;
        prevCount = count;

        // have to do this to pass by value
        for (uint i = 0; i < flowerScores.Length; ++i)
        {
            prevScores[i] = (uint)flowerScores.GetValue(i);
        }

        prevScoreMod = scoreModifier;
        prevCheck = affectedByScore;

        uint drawScore = 0;
        long totalDS = 1;
        for (int j = 1; j <= count * totalDS; ++j)
        {

            for (int i = 0; i < flowerObjects.Length; ++i)
            {
                // this at the moment is for each flower attached to the zones it'll use 
                prevRadius = radius;

                if (affectedByScore)
                {
                // Do we plant? 
                    drawScore = GenerateModifier(i);
                    if (drawScore == 0)
                    {
                        print(" nope");
                        continue;
                    }
                    print(" yip");
                    // lower end of drawscore check
                    if (i < drawScore)
                    {
                        // totalDrawScore is upper bounds of modifier
                        totalDS = drawScore / flowerObjects.Length;
                        print("drawing : " + totalDS + " " + drawScore + " " + i + flowerObjects[i]);
                        Transform t = Instantiate(flowerObjects[i].transform);
                        Vector3 instanceLoc = Random.insideUnitCircle * radius;
                        t.localPosition = circleTrans.position + new Vector3(instanceLoc.x, 0.0f, instanceLoc.y);
                        t.SetParent(transform);
                        Quaternion initialRot = t.localRotation;
                        float randomAngle = Random.Range(0, 360);
                        t.localRotation = initialRot * Quaternion.Euler(0, randomAngle, 0);
                        spawned.Add(t.gameObject);
                    }
                }
                else
                {
                    Transform t = Instantiate(flowerObjects[i].transform);
                    Vector3 instanceLoc = Random.insideUnitCircle * radius;
                    t.localPosition = circleTrans.position + new Vector3(instanceLoc.x, 0.0f, instanceLoc.y);
                    t.SetParent(transform);
                    Quaternion initialRot = t.localRotation;
                    float randomAngle = Random.Range(0, 360);
                    t.localRotation = initialRot * Quaternion.Euler(0, randomAngle, 0);
                    spawned.Add(t.gameObject);
                }



            }
        }

        ShuffleOverlaps();
    }

    bool Intersects(Vector3 vecA, Vector3 vecB)
    {
        if (vecA.x <= (vecB.x + epsilon.x) || vecA.x >= (vecB.x - epsilon.x)
            || vecA.z <= (vecB.z + epsilon.z) || vecA.z >= (vecB.z - epsilon.z))
        {
            return true;
        }
        return false;
    }

    void ShuffleOverlaps()
    {
        // for each of the current spawned flowers, if it's ontop of another, shift it. 
        int shiftNum = 0; 
        for(int i = 0; i < spawned.Count - 1; ++i)
        {
            if (Intersects(spawned[i].transform.position, spawned[i+1].transform.position))
            {
                shiftNum++;

                if (shiftNum > 3)
                    shiftNum = 0;

                switch (shiftNum)
                {
                    case 0:
                        spawned[i + 1].transform.position += epsilon;
                        break;
                    case 1:
                        spawned[i + 1].transform.position -= epsilon;
                        break;
                    case 2:
                        spawned[i + 1].transform.position += new Vector3(epsilon.x, epsilon.y, -epsilon.z);
                        break;
                    case 3:
                        spawned[i + 1].transform.position += new Vector3(-epsilon.x, epsilon.y, epsilon.z);
                        break;                    
                }
            }
        }
       
    }

    public void ChangeFlowerType(Narrative.Archetype domArch, List<GameObject> inFlowers)
    {
        if (inFlowers.Count == 0)
        {
            Debug.Log("No Flower selected to replace " + domArch);
        }
        print(domArch);
        print(inFlowers);
        if (!dominantMod)
            return;

        int usedIdx = 0;

        // for the flowers that don't match the archetype, change them to a random one. 
        // alter flowerObjects, delete then respawn. 
        for(int i =0; i < flowerObjects.Length; ++i)
        {
            Narrative.Archetype result;
            if (System.Enum.TryParse<Narrative.Archetype>(flowerMap[flowerObjects[i]], out result))
            {
                if (result != domArch)
                {
                    print("ohno "+ usedIdx + " " + inFlowers.Count);
                    ++usedIdx;
                    if (usedIdx >= inFlowers.Count)
                    {
                        usedIdx = 0; 
                    }
                    flowerObjects[i] = inFlowers[usedIdx];
                    
                }   
            }
        }

        needUpdate = true;
    }

    uint GenerateModifier(int index)
    {
        // returns true/false if we want to spawn object
        if (flowerMap.ContainsKey(flowerObjects[index]))
        {
            // match to score
            Narrative.Archetype flowerIndex;
            int cArch = 4; // default is neutral
            if (System.Enum.TryParse<Narrative.Archetype>(flowerMap[flowerObjects[index]].ToString(), out flowerIndex))
            {
                // set to specific
                cArch = (int)flowerIndex;
            }

            
            if (cArch < 4)
            {
                print(flowerScores[cArch]);

                // waht's that score for 
                print(flowerMap[flowerObjects[index]].ToString());

                print("score is: " + flowerScores[(int)flowerIndex]);
                if (flowerScores[(int)flowerIndex] > 0)
                {
                    return (uint)flowerScores[(int)flowerIndex] * (uint)scoreModifier;
                }
                else
                {
                    return 0;
                }
            }
            else if (cArch == 4)
            {
                return 10 * (uint)scoreModifier;
            }
        }
        else
        {
            return 0;
        }
        return 0;
    }

    public void AddFlowerMap(GameObject inGO, string inArch)
    {
        flowerMap.Add(inGO, inArch);
    }
}
