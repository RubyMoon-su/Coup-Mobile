using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject cursor;
    public Camera _camera;

    private Vector3 touchStart;
    public float groundZ = 0;
    public float HeightCam = 200;

   public CharacterController cc;
    public void Awake()
    {
        _camera = Camera.main;
    }

    public void LateUpdate()
    {
        Input_Function();
    }

    #if UNITY_ANDROID

    private void Input_Function()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

             switch (touch.phase)
            {
                case TouchPhase.Began:
                    // ทำงานเมื่อการสัมผัสเริ่มต้นขึ้น
                    //Debug.Log("Touch Began at position: " + touch.position);

                    touchStart = GetWorldPosition(groundZ);
                    break;

                case TouchPhase.Moved:
                    // ทำงานเมื่อการสัมผัสเคลื่อนที่
                    //Debug.Log("Touch Moved to position: " + touch.position);

                    Vector3 Direction = touchStart - GetWorldPosition(groundZ);
                    Vector3 GlobalDirection = transform.TransformDirection(Direction);
                    float movespeed = 0.1f;
                    Vector3 NewMove = new Vector3(GlobalDirection.x * movespeed , 0 , GlobalDirection.y * movespeed);
                    Debug.Log(NewMove);

                    cc.Move(NewMove);

                    break;

                case TouchPhase.Stationary:
                    // ทำงานเมื่อการสัมผัสคงที่
                    //Debug.Log("Touch Stationary at position: " + touch.position);
                    break;

                case TouchPhase.Ended:
                    // ทำงานเมื่อการสัมผัสสิ้นสุดลง
                    //Debug.Log("Touch Ended at position: " + touch.position);
                    break;

                case TouchPhase.Canceled:
                    // ทำงานเมื่อการสัมผัสถูกยกเลิก
                    //Debug.Log("Touch Canceled at position: " + touch.position);
                    break;
            }
        }
    }

    #endif

    #if Unity_WEBGL

    private void Input_Function()
    {
        if (Input.GetMouseButton(0))
        {

        }
    }


    #endif

    private Vector3 GetWorldPosition(float z)
    {
        Ray movePos = _camera.ScreenPointToRay(Input.GetTouch(0).position);
        Plane ground = new Plane(Vector3.forward , new Vector3(0 , 0, z));
        
        float distance;
        ground.Raycast(movePos , out distance);
        return movePos.GetPoint(distance);
    }
}
