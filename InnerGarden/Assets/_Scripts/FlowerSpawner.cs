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


    public bool affectedByScore = true;
    private bool prevCheck = true;
    public float scoreModifier = 2.0f;
    private float prevScoreMod = 0.0f;
    public uint[] flowerScores = { 0, 0, 4, 0 };
    private uint[] prevScores = new uint[4];
    
    


    // Start is called before the first frame update
    void Start()
    {
        circleTrans = this.transform.GetChild(0);
        prevPos = circleTrans;
        spawned = new List<GameObject>();

        GameManager.PrintScores();
        flowerScores = GameManager.GetScores();

        prevScoreMod = scoreModifier;
        prevScores = flowerScores;

        Redraw();
    }

    // Update is called once per frame
    void Update()
    {
        circleTrans = this.transform.GetChild(0);
        flowerScores = GameManager.GetScores();

        if (prevCount != count || prevRadius != radius || prevPos != circleTrans 
            || prevScores != flowerScores || scoreModifier != prevScoreMod || prevCheck != affectedByScore)
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
        if (!gameObject.activeSelf)
            return;

        prevPos = circleTrans;
        prevCount = count;
        prevScores = flowerScores;
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
    }

    uint GenerateModifier(int index)
    {
        // returns true/false if we want to spawn object
        if (flowerMap.ContainsKey(flowerObjects[index]))
        {
            // match to score
            var flowerIndex = System.Enum.Parse(typeof(Narrative.Archetype), flowerMap[flowerObjects[index]].ToString());
            int cArch = (int)flowerIndex;

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
