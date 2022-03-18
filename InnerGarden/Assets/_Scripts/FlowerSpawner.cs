using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{
    public int count = 1;
    public int radius = 5; 
    public GameObject[] flowerObjects = new GameObject[3];

    private Transform circleTrans;

    private int prevCount;
    private int prevRadius;
    private Transform prevPos;
    public List<GameObject> spawned;

    // Start is called before the first frame update
    void Start()
    {
        circleTrans = this.transform.GetChild(0);
        prevPos = circleTrans;
        spawned = new List<GameObject>();

        Redraw();

    }

    // Update is called once per frame
    void Update()
    {
        circleTrans = this.transform.GetChild(0);

        if (prevCount != count || prevRadius != radius || prevPos != circleTrans)
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
    }

    void Redraw()
    {
        prevPos = circleTrans;
        prevCount = count;
        for (int j = 1; j <= count; ++j)
        {
            for (int i = 0; i < flowerObjects.Length; ++i)
            {
                prevRadius = radius;
                Transform t = Instantiate(flowerObjects[i].transform);
                Vector3 instanceLoc = Random.insideUnitCircle * radius;
                t.localPosition = circleTrans.position + new Vector3(instanceLoc.x, 0.0f, instanceLoc.y);
                t.SetParent(transform);
                Quaternion initialRot = t.localRotation;
                float randomAngle = Random.Range(0, 360);
                t.localRotation = initialRot *Quaternion.Euler(0, randomAngle, 0);
                spawned.Add(t.gameObject);
            }
        }
    }
}
