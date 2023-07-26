using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DrawLineScript : MonoBehaviour
{
    
    public GameObject linePrefab;
    public GameObject currentLine;
    public GameObject lineList;

    public LineRenderer lineRenderer;
    public List<Vector2> linePosition;
    public RectTransform drawArea;
    public Camera cam;
    private List<GameObject> _lineList;

    public delegate void LineDrawnEventHandler();
    public event Action LineDrawn;

    private void Awake()
    {
        _lineList = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(drawArea, Input.mousePosition, cam))
            {
                CreateLine();
                InvokeOnLineDrawn();
            }
            else
            {
                linePosition.Clear();
            }
            
        }
        if (Input.GetMouseButton(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(drawArea, Input.mousePosition, cam))
            {
                if (linePosition.Count == 0)
                {
                    CreateLine();
                    return;
                }

                Vector2 tempLinePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(tempLinePos, linePosition[linePosition.Count - 1]) > .1f)
                {
                    UpdateLine(tempLinePos);
                }
                InvokeOnLineDrawn();
            }
            else
            {
                linePosition.Clear();
            }
            
        }
    }

    void CreateLine()
    {
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, lineList.transform);
        _lineList.Add(currentLine);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        linePosition.Clear();
        linePosition.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        linePosition.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        lineRenderer.SetPosition(0, linePosition[0]);
        lineRenderer.SetPosition(1, linePosition[1]);
    }

    void UpdateLine(Vector2 newLinePos)
    {
        linePosition.Add(newLinePos);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newLinePos);
    }

    public void Restart()
    {
        _lineList.Clear();
        linePosition.Clear();
        foreach (Transform child in lineList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Undo()
    {
        linePosition.Clear();

        if (_lineList.Count > 0)
        {
            var line = _lineList[^1];
            Destroy(line);
            _lineList.Remove(line);
        }

    }

    protected virtual void InvokeOnLineDrawn()
    {
        LineDrawn?.Invoke();
    }
}
