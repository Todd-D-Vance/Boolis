using UnityEngine;
using System.Collections;

public class PlaySpace : MonoBehaviour
{


    public static PlaySpace instance;


    #region public fields

    public GameObject elementPrefab;
    public float targetAspect = 3f / 4f;
    public float topPad = 1f;
    public float bottomPad = 0f;
    public float targetPlayspaceWidth = 6f;
    public float targetPlayspaceHeight = 8f;

    public float tickRate = 1f;
    #endregion public fields

    private float leftX;
    private float rightX;
    private float bottomY;
    private float topY;
    private int maxX;
    private int maxY;

    private Element[,] gameboard;


    public float LeftX
    {
        get
        {
            return leftX;
        }

    }

    public float RightX
    {
        get
        {
            return rightX;
        }


    }

    public float BottomY
    {
        get
        {
            return bottomY;
        }


    }

    public float TopY
    {
        get
        {
            return topY;
        }


    }


    void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {
        SetupUnits();
        gameboard = new Element[maxX + 1, maxY + 1];
        SpawnNewElement();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject SpawnNewElement()
    {
        return Instantiate(elementPrefab) as GameObject;
    }

    public void UpdateLocation(Element element, int oldX, int oldY)
    {
        gameboard[oldX, oldY] = null;
        gameboard[element.X, element.Y] = element;
    }
    public bool locationFilled(int x, int y)
    {
        if (x < 0 || x > maxX || y < 0 || y > maxY)
        {
            return false;
        }
        return gameboard[x, y] != null;
    }

    private void SetupUnits()
    {
        //find aspect ratio, but make core game aspect close to targetAspect if possible
        Camera camera = FindObjectOfType<Camera>();
        float aspect = camera.aspect;
        if (aspect > PlaySpace.instance.targetAspect)
        {
            aspect = PlaySpace.instance.targetAspect;
        }

        //set main screen area based on camera and target aspect and padding
        topY = camera.transform.position.y + camera.orthographicSize - PlaySpace.instance.topPad;
        bottomY = camera.transform.position.y - camera.orthographicSize + PlaySpace.instance.bottomPad;
        leftX = camera.transform.position.x - camera.orthographicSize * aspect;
        rightX = camera.transform.position.x + camera.orthographicSize * aspect;

        //set width and height of sprite according to target playspace dimensions
        float elementWidth = computeElementWidth();
        float elementHeight = computeElementHeight();

        maxX = computeMaxX(elementWidth);
        maxY = computeMaxY(elementHeight);

        Debug.Log("Camera bounds: (" + LeftX + ", " + BottomY + ") to (" + RightX + ", " + TopY + ")");
        Debug.Log("Gamespace Bounds: (0, 0) to  (" + maxX + ", " + maxY + ")");

    }
    public int computeMaxX(float width)
    {
        return (int)((RightX - LeftX) / width - .01);
    }

    public int computeMaxY(float height)
    {
        return (int)((PlaySpace.instance.TopY - PlaySpace.instance.BottomY) / height - .01);
    }

    public float computeElementWidth()
    {
        return (RightX - LeftX) / targetPlayspaceWidth;
    }

    public float computeElementHeight()
    {
        float elementHeight = computeElementWidth();
        if (elementHeight * targetPlayspaceHeight > TopY - BottomY)
        {
            elementHeight = (TopY - BottomY) / targetPlayspaceHeight;
        }
        return elementHeight;
    }
}
