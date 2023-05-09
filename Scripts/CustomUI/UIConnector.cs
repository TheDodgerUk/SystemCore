/*
	For using please download  Unity UI system from:
	https://bitbucket.org/Unity-Technologies/ui
*/

using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

[ExecuteInEditMode]
[RequireComponent(typeof(UILineRenderer))]
public class UIConnector : MonoBehaviour
{
    public enum ConnectorType
    {
        Center, Left, Right, Top, Bottom
    }

    [Serializable]
    public class ConnectorClass
    {
        public RectTransform Node;
        public ConnectorType Type;

        [Range(-1, 1)]
        public float Displacement;

        public float SeccondPoint = 50;

        public Vector2[] Position(Vector2 parentAnchor)
        {
            if (!Node) return new Vector2[2];

            Vector2 delta = Vector2.zero;
            Vector2 delta2 = Vector2.zero;
            switch (Type)
            {
                case ConnectorType.Right:
                    delta = new Vector2(Node.sizeDelta.x * 0.5f, Node.sizeDelta.y * Displacement * 0.5f);
                    delta2 = new Vector2(delta.x + SeccondPoint, delta.y);
                    break;
                case ConnectorType.Left:
                    delta = new Vector2(Node.sizeDelta.x * -0.5f, Node.sizeDelta.y * Displacement * 0.5f);
                    delta2 = new Vector2(delta.x - SeccondPoint, delta.y);
                    break;
                case ConnectorType.Top:
                    delta = new Vector2(Node.sizeDelta.x * Displacement * 0.5f, Node.sizeDelta.y * 0.5f);
                    delta2 = new Vector2(delta.x, delta.y + SeccondPoint);
                    break;
                case ConnectorType.Bottom:
                    delta = new Vector2(Node.sizeDelta.x * Displacement * 0.5f, Node.sizeDelta.y * -0.5f);
                    delta2 = new Vector2(delta.x, delta.y - SeccondPoint);
                    break;
            }

            Vector2[] result = new Vector2[2];

            result[0] = Node.anchoredPosition - parentAnchor + delta;
            result[1] = Node.anchoredPosition - parentAnchor + delta2;


            return result;
        }
    }

    public ConnectorClass startConnector;
    public ConnectorClass endConnector;

    private UILineRenderer line;

    // Use this for initialization
    private void Start()
    {
        line = GetComponent<UILineRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        RedrawLine();
    }

    private void RedrawLine()
    {
        if (!line)
        {
            line = GetComponent<UILineRenderer>();
            if (!line) return;
        }
        RectTransform tr = ((RectTransform)transform);

        transform.position = startConnector.Node.position - (startConnector.Node.position - endConnector.Node.position) * 0.5f;
        tr.sizeDelta = Vector2.zero;
        tr.anchorMax = Vector2.one * 0.5f;
        tr.anchorMin = Vector2.one * 0.5f;

        Vector2[] p = new Vector2[4];

        Vector2[] pp = startConnector.Position(tr.anchoredPosition);
        p[0] = pp[0];
        p[1] = pp[1];

        pp = endConnector.Position(tr.anchoredPosition);
        p[3] = pp[0];
        p[2] = pp[1];

        line.Points = p;

    }
}