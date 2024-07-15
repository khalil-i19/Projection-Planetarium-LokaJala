using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ConstellationLinesManager : MonoBehaviour
{

    public TextAsset constellationLineText;

    [Serializable]
    public struct ConstellationLineData
    {
        public string name;
        public List<string> hrNumberArray;
        public List<Vector3> starPosition;
    }

    public Transform starParent;
    public GameObject linePrefab;
    public float lineWidth = .1f;
    public float lineAnimationDuration = .3f;


    [Header("Data")]
    public List<ConstellationLineData> constellationLineDataList = new();
    public List<GameObject> constellationList = new();

    void Start()
    {
        ProcessData();
        GenerateLine();
    }

    [ContextMenu("Process Data")]
    void ProcessData()
    {
        constellationLineDataList.Clear();
        string[] dataLine = constellationLineText.text.Split('\n');

        for (int i = 0; i < dataLine.Length; i++)
        {
            string[] dataByComa = dataLine[i].Split(',');

            ConstellationLineData constellationLineData;

            constellationLineData.name = dataByComa[0];
            constellationLineData.hrNumberArray = new List<string>(dataByComa);
            constellationLineData.hrNumberArray.RemoveAt(0);
            constellationLineData.hrNumberArray.RemoveAt(0);
            constellationLineData.starPosition = new List<Vector3>();

            for (int k = 0; k < constellationLineData.hrNumberArray.Count; k++)
            {
                int val = int.Parse(constellationLineData.hrNumberArray[k]);

                constellationLineData.starPosition.Add(GetStarPosition(val.ToString()));
            }

            constellationLineDataList.Add(constellationLineData);
        }

    }

    Vector3 GetStarPosition(string hrNumber)
    {

        Transform star = starParent.Find("HR " + hrNumber);
        if (star != null)
        {
            return star.position;
        }
        else
        {
            Debug.LogError("Star with HR number (" + hrNumber + ") not found.");
            return Vector3.zero;
        }

    }

    void GenerateLine()
    {
        for (int i = 0; i < constellationLineDataList.Count; i++)
        {
            GameObject constellation = new(constellationLineDataList[i].name);
            constellation.transform.SetParent(gameObject.transform);

            for (int j = 1; j < constellationLineDataList[i].starPosition.Count; j++)
            {
                GameObject lineObject = Instantiate(linePrefab);
                lineObject.transform.SetParent(constellation.transform);

                LineRenderer line = lineObject.GetComponent<LineRenderer>();

                if (line != null)
                {
                    line.startWidth = lineWidth;
                    line.endWidth = lineWidth;

                    line.useWorldSpace = false;

                    line.positionCount = 2;
                    Vector3 pos1 = constellationLineDataList[i].starPosition[j - 1];
                    Vector3 pos2 = constellationLineDataList[i].starPosition[j];
                    Vector3 dir = (pos2 - pos1).normalized * 3;

                    line.SetPosition(0, pos1 + dir);
                    line.SetPosition(1, pos2 - dir);

                }
                else
                {
                    Debug.LogError("Line prefab '" + linePrefab.name + "' does not have a LineRenderer component.");
                }
            }
            constellation.SetActive(false);
            constellationList.Add(constellation);

        }
    }

    void Update()
    {
        // Check for numeric presses and toggle the constellation highlighting.
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                ToggleConstellation(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ShowAll();
        }
    }

    public void ShowAll()
    {
        for (int i = 0; i < constellationList.Count; i++)
        {
            ToggleConstellation(i);
        }
    }

    public void ToggleConstellation(int index)
    {
        bool activate = !constellationList[index].activeInHierarchy;
        constellationList[index].SetActive(!constellationList[index].activeInHierarchy);
        Debug.Log("toggle " + constellationList[index].name);

        StartCoroutine(AnimateLines(constellationList[index], index));
    }

    private IEnumerator AnimateLines(GameObject contellationObject, int index)
    {
        float animationDuration = lineAnimationDuration;
        float elapsedTime = 0;
        float delay = 0.1f; // Set delay time for each line

        List<LineRenderer> lines = contellationObject.GetComponentsInChildren<LineRenderer>().ToList<LineRenderer>();

        // Set initial positions for all lines
        for (int i = 0; i < lines.Count; i++)
        {
            LineRenderer line = lines[i];
            line.SetPosition(1, constellationLineDataList[index].starPosition[i]);
        }

        // Animate all lines with a slight delay for each line
        while (elapsedTime < animationDuration + (lines.Count - 1) * delay)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (elapsedTime >= i * delay)
                {
                    LineRenderer line = lines[i];
                    Vector3 start = line.GetPosition(0);
                    Vector3 end = constellationLineDataList[index].starPosition[i + 1];
                    float t = (elapsedTime - i * delay) / animationDuration;
                    t = Mathf.Clamp01(t); // Ensure t is between 0 and 1
                    t = t * t * (3f - 2f * t); // Cubic easing in-out
                    line.SetPosition(1, Vector3.Lerp(start, end, t));
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure all lines reach their final position
        for (int i = 0; i < lines.Count; i++)
        {
            LineRenderer line = lines[i];
            Vector3 end = constellationLineDataList[index].starPosition[i + 1];
            line.SetPosition(1, end);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < constellationLineDataList.Count; i++)
        {
            for (int j = 1; j < constellationLineDataList[i].starPosition.Count; j++)
            {
                Vector3 pos1 = constellationLineDataList[i].starPosition[j - 1];
                Vector3 pos2 = constellationLineDataList[i].starPosition[j];
                Vector3 dir = (pos2 - pos1).normalized * 3;
                Gizmos.DrawLine(pos1 + dir, pos2 - dir);
            }
        }
    }
}
