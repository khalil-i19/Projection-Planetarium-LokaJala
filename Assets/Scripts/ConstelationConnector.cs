using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ConstellationConnector : MonoBehaviour
{
    [TextArea(1, 6)] public string linesByHR = "3475, 3449, 3449, 3461, 3461, 3572, 3461, 3249";


    public GameObject linePrefab;
    public float lineWidth = .1f;
    public Transform starParent;
    public float lineAnimationDuration = .3f;

    [System.Serializable]
    public struct LinePair
    {
        public Vector3 line1;
        public Vector3 line2;

    }

    [Header("Data")]
    public List<GameObject> stars = new List<GameObject>();
    public List<LinePair> lineList = new();
    private List<LineRenderer> lines = new List<LineRenderer>();


    void Start()
    {
        GenerateLineData();
        GenerateLines();
    }

    [ContextMenu("Generate Data")]
    public void GenerateLineData()
    {
        lineList.Clear();
        stars.Clear();

        linesByHR = linesByHR.Replace(" ", "");
        string[] hrNumbers = linesByHR.Split(',');

        foreach (string hrNumber in hrNumbers)
        {
            Transform star = starParent.Find("HR " + hrNumber);
            if (star != null)
            {
                stars.Add(star.gameObject);
            }
            else
            {
                Debug.LogError("Star with HR number " + hrNumber + " not found.");
            }
        }

        if (stars.Count >= 2)
        {
            for (int i = 0; i < stars.Count; i += 2)
            {
                GameObject star1 = stars[i];
                GameObject star2 = stars[i + 1];

                Vector3 pos1 = star1.transform.position;
                Vector3 pos2 = star2.transform.position;
                Vector3 dir = (pos2 - pos1).normalized * 3;
                LinePair linePair;

                linePair.line1 = pos1 + dir;
                linePair.line2 = pos2 - dir;

                lineList.Add(linePair);
            }
        }

    }

    public void GenerateLines()
    {
        ClearLines();

        if (lineList.Count > 0)
        {
            for (int i = 0; i < lineList.Count; i++)
            {
                GameObject lineObject = Instantiate(linePrefab);
                lineObject.transform.SetParent(transform);

                LineRenderer line = lineObject.GetComponent<LineRenderer>();

                if (line != null)
                {
                    line.startWidth = lineWidth;
                    line.endWidth = lineWidth;

                    line.useWorldSpace = false;

                    line.positionCount = 2;
                    line.SetPosition(0, lineList[i].line1);
                    line.SetPosition(1, lineList[i].line2);

                    lines.Add(line);
                }
                else
                {
                    Debug.LogError("Line prefab '" + linePrefab.name + "' does not have a LineRenderer component.");
                }
            }
        }
        else
        {
            Debug.LogError("Please enter at least two HR numbers in the TextArea.");
        }
    }

    private void OnEnable()
    {
        StartCoroutine(AnimateLines());
    }

    private IEnumerator AnimateLines()
    {
        float animationDuration = lineAnimationDuration;
        float elapsedTime = 0;
        float delay = 0.1f; // Set delay time for each line

        // Set initial positions for all lines
        for (int i = 0; i < lines.Count; i++)
        {
            LineRenderer line = lines[i];
            line.SetPosition(1, lineList[i].line1);
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
                    Vector3 end = lineList[i].line2;
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
            Vector3 end = lineList[i].line2;
            line.SetPosition(1, end);
        }
    }

    void ClearLines()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        foreach (var line in lineList)
        {
            Gizmos.DrawLine(line.line1, line.line2);
        }
    }
}
