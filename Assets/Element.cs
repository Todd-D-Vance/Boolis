using UnityEngine;
using System.Collections;
using Assets;

public class Element : MonoBehaviour
{
 
    private float gameTime = 0f;
    private float screenX, screenY, screenRot;

    private float width;//TODO scale the sprite itself
    private float height;


    private ElementType elementType;

    private bool falling = false;
    private bool rotatingLeft = false;
    private bool rotatingRight = false;
    private bool movingLeft = false;
    private bool movingRight = false;
    private bool receivingInput = true;
    private bool disable = false;

    public bool canMove = true;
    public bool canRotate = true;
    public bool canFall = true;

    public float moveSpeed = 2.5f;
    public float fallSpeed = 2f;
    public float rotateSpeed = 2.5f;




    private void syncGameToScreen()
    {
        ScreenX = X * width + PlaySpace.instance.LeftX + width / 2;
        ScreenY = Y * height + PlaySpace.instance.BottomY + height / 2;
        ScreenRot = Rot * 90f;
    }

    #region properties

    int _maxX;
    int _maxY;
    int _x;
    int _y;
    int _rot;

    public int MaxX
    {
        get
        {
            return _maxX;
        }

        private set
        {
            _maxX = value;
        }
    }

    public int MaxY
    {
        get
        {
            return _maxY;
        }

        private set
        {
            _maxY = value;
        }
    }

    public int X
    {
        get
        {
            return _x;
        }

        set
        {
            _x = value < 0 ? 0 : value > MaxX ? MaxX : value;
        }
    }

    public int Y
    {
        get
        {
            return _y;
        }

        set
        {
            _y = value < 0 ? 0 : value > MaxY ? MaxY : value;

        }
    }

    public int Rot
    {
        get
        {
            return _rot;
        }

        set
        {
            _rot = value % 4;
            if (_rot < 0)
            {
                Rot += 4;
            }
        }
    }

    public float ScreenX
    {
        get
        {
            return screenX;
        }

        set
        {
            screenX = value;
            //X = (int)Mathf.Ceil((screenX - leftX) / width);
        }
    }

    public float ScreenY
    {
        get
        {
            return screenY;
        }

        set
        {
            screenY = value;
            //Y = (int)Mathf.Ceil((screenY - bottomY) / height);

        }
    }

    public float ScreenRot
    {
        get
        {
            return screenRot;
        }

        set
        {
            screenRot = value % 360f;
            if (screenRot < 0)
            {
                screenRot += 360f;
            }
            //Rot = (int)Mathf.Ceil(screenRot / 90f);
        }
    }

    public float DeltaScreenX
    {
        get
        {
            return X * width + PlaySpace.instance.LeftX + width/2 - ScreenX;
        }
    }

    public float DeltaScreenY
    {
        get
        {
            return Y * height + PlaySpace.instance.BottomY + height/2 - ScreenY;
        }
    }

    public float DeltaScreenRot
    {
        get
        {
            float d = Rot * 90f - ScreenRot;
            if (d > 180f)
            {
                d -= 360f;
            }
            else if (d < -180f)
            {
                d += 360f;
            }
            return d;
        }
    }

    public ElementType ElementType
    {
        get
        {
            return elementType;
        }

        set
        {
            if (elementType != null)
            {
                elementType.Deregister(this);
            }
            elementType = value;
            SetSprite(elementType);
            elementType.Register(this);
        }
    }


    #endregion properties

    public void SetSprite(ElementType elementType)
    {
        //TODO set sprite according to name and terminal values of elementType
    }


    void Start()
    {
        SetupUnits();
        Init();
        Fall();
    }

    private float deltaTime;

    void Update()
    {

        deltaTime = Time.deltaTime * PlaySpace.instance.tickRate;
        bool tick = false;
        int ticks = (int)gameTime;
        gameTime += deltaTime;
        if ((int)gameTime > ticks)
        {
            tick = true;
            if (falling)
            {
                Debug.Log("X=" + X);
                Debug.Log("Y=" + Y);
                Debug.Log("Rot=" + Rot);
            }
        }

        if(tick && !falling)
        {
            if (disable)
            {
                canMove = false;
                canRotate = false;
                canFall = false;
            }  
            else
            {
                disableNextTick();
            }
        }

        if (canMove || canFall)
        {
            UpdateLocation(tick);
        }
        if (canRotate)
        {
            UpdateRotation(tick);
        }

        UpdateSprite();
        CheckKeyboard();
    }

    private void disableNextTick()
    {
        disable = true;
        rotatingLeft = false;
        rotatingRight = false;
        movingLeft = false;
        movingRight = false;
        receivingInput = false;
        PlaySpace.instance.SpawnNewElement();
        UpdateValues();
    }

    private void UpdateValues()
    {
        //TODO update values of all elements
    }

    void CheckKeyboard()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }else 
        {
            StopMoveLeft();
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }
        else 
        {
            StopMoveRight();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            RotateRight();
        }
        else
        {
            StopRotateRight();
        }
    }

    private void UpdateSprite()
    {
        transform.GetChild(0).position = new Vector3(ScreenX, ScreenY);
        transform.GetChild(0).rotation = Quaternion.AngleAxis(ScreenRot, Vector3.forward);
    }

    private void UpdateRotation(bool tick)
    {
        float dr = DeltaScreenRot;
        if (Mathf.Abs(dr) > 1E-5)
        {
            float amount = Mathf.Sign(dr) * Mathf.Min(deltaTime * rotateSpeed * 90f, Mathf.Abs(dr));
            ScreenRot += amount;
        }
        else if (rotatingLeft && tick)
        {
            Rot++;
        }
        else if (rotatingRight && tick)
        {
            Rot--;
        }
    }

    private void UpdateLocation(bool tick)
    {
        float dx = DeltaScreenX;
        float dy = DeltaScreenY;

        int oldX = X;
        int oldY = Y;
        bool locChanged = false;

        if (canMove)
        {
            if (Mathf.Abs(dx) > 1E-5)
            {
                float amount = Mathf.Sign(dx) * Mathf.Min(Time.deltaTime * moveSpeed, Mathf.Abs(dx));
                ScreenX += amount;
            }
            else if (movingLeft && tick)
            {

                X--;
                locChanged = true;
            }
            else if (movingRight && tick)
            {
                X++;
                locChanged = true;

            }
        }

        if (canFall)
        {
            if (Mathf.Abs(dy) > 1E-5)
            {
                float amount = Mathf.Sign(dy) * Mathf.Min(deltaTime * fallSpeed, Mathf.Abs(dy));
                ScreenY += amount;
            }
            else if (falling && tick)
            {
                if (PlaySpace.instance.locationFilled(X, Y - 1))
                {
                    FinalDestination();
                }
                else
                {
                    Y--;
                    locChanged = true;

                    if (Y == 0)
                    {
                        FinalDestination();
                    }
                }
            }
        }
        if (locChanged)
        {
            PlaySpace.instance.UpdateLocation(this, oldX, oldY);
        }
    }

    private void SetupUnits()
    {
        //set width and height of sprite according to target playspace dimensions
        width = PlaySpace.instance.computeElementWidth();
        height = PlaySpace.instance.computeElementHeight();

        MaxX = (int)((PlaySpace.instance.RightX - PlaySpace.instance.LeftX) / width - .01);
        MaxY = (int)((PlaySpace.instance.TopY - PlaySpace.instance.BottomY) /height - .01);
        Debug.Log("Sprite dims: " + width + " x " + height);
    }

    public void resumeFall()
    {
        canFall = true;
        falling = true;
    }

    private void Init()
    {
        Y = MaxY;
        X = Random.Range(0, MaxX);
        Rot = 0;
        syncGameToScreen();
    }

    public void Fall()
    {
        falling = true;
        receivingInput = true;
    }

    public void FinalDestination()
    {
        falling = false;
    }

    public void StopMoveLeft()
    {
        movingLeft = false;
    }

    public void StopMoveRight()
    {
        movingRight = false;
    }

    public void StopRotateLeft()
    {
        rotatingLeft = false;
    }

    public void StopRotateRight()
    {
        rotatingRight = false;
    }

    public void MoveLeft()
    {
        if (receivingInput)
        {
            movingLeft = true;
        }
    }

    public void MoveRight()
    {
        if (receivingInput)
        {
            movingRight = true;
        }
    }

    public void RotateLeft()
    {
        if (receivingInput)
        {
            rotatingLeft = true;
        }
    }

    public void RotateRight()
    {
        if (receivingInput)
        {
            rotatingRight = true;
        }
    }

    public void die()
    {

    }

    public void setZero()
    {

    }

    public void setOne()
    {

    }

    public void setPoison()
    {

    }

    public void setUnconnected()
    {

    }

}
