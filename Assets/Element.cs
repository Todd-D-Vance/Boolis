using UnityEngine;
using System.Collections;

public class Element : MonoBehaviour
{

    int min;
    int max;
    PlaySpace playspace;
    float speed = 1.0f;
    float rotateSpeed = 2.0f;
    float orientation = 0f;
    float nextOrientation = 0f;
    float rotatePerSecond = 0f;

    bool falling = true;
    bool rotating = false;

    // Use this for initialization
    void Start()
    {
        playspace = FindObjectOfType<PlaySpace>() as PlaySpace;
        min = playspace.leftX;
        max = playspace.rightX;

        int x = Random.Range(min, max + 1);
        int y = playspace.topY;
        float rotation = 90f * Random.Range(0, 4);

        transform.position = new Vector3(x, y);
        transform.GetChild(0).transform.Rotate(Vector3.back, rotation);
        rotateRight();
    }

    void rotateRight()
    {
        nextOrientation = (orientation + 90f) % 360f;
        rotatePerSecond = rotateSpeed * 90f;
        rotating = true;
    }


    void rotateLeft()
    {
        nextOrientation = (orientation + 270f) % 360f;
        rotatePerSecond = -rotateSpeed * 90f;
        rotating = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (falling)
        {
            transform.position -= new Vector3(0, speed * Time.deltaTime);
        }

        if (rotating)
        {
            float amount = Mathf.Min(rotatePerSecond * Time.deltaTime, Mathf.Abs(nextOrientation - orientation)*Mathf.Sign(rotatePerSecond));
            transform.GetChild(0).transform.Rotate(Vector3.back, amount);
            orientation += amount;

            if (Mathf.Abs(orientation - nextOrientation) < amount)
            {
                //amount = Mathf.Min(nextOrientation - orientation);
                //transform.GetChild(0).transform.Rotate(Vector3.back, amount);
                //orientation = nextOrientation;
                rotating = false;
            }
        }
        else
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                rotateRight();
            }else
            {
                rotateLeft();
            }
        }

    }

}
