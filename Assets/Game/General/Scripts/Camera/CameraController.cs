using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    //exposed
    [Header("General")]
    [SerializeField] Transform _target;
    [Header("Focusing")]
    [SerializeField] float _followSpeed;
    [SerializeField] float _jumpToFocusSpeed;
    [SerializeField] LayerMask _focusableMask;
    [Header("Free Look and Zoom")]
    [SerializeField] bool _allowFreeLook;
    [SerializeField] float _scrollSpeed;
    [SerializeField] bool _allowZoom;
    [SerializeField] float _zoomSpeed;
    [Header("Camera Shake")]
    [SerializeField] float _minShakeRadius;
    [SerializeField] float _maxShakeRadius;
    [SerializeField] float _shakeSpeed;
    [SerializeField] float _shakeDuration;
    [Header("Bounds")]
    bool _rectangular;
    int _boundsOffsetX;
    int _boundsOffsetY;


    //vars
    Vector2 _baseWorldPosition;
    Vector3 _baseMousePosition;
    
    bool _onFocus;
    bool _isJumpingToFocus;
    bool _isShaking;
    bool _hasLeftClicked;

    float _cameraZ;
    Camera _cam;

    //properties
    public Transform Target {get => _target;}
    public Camera Camera {get => _cam;}
    public static CameraController Instance {get => _instance;}

    //singleton
    static CameraController _instance;

    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    void Init()
    {
        _isShaking = false;
        _onFocus = true;
        _hasLeftClicked = false;

        _cam = GetComponent<Camera>();
        _cameraZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {

        if (_allowFreeLook)
        {
            TryToFocus();

            if (_isJumpingToFocus || _isShaking) return;

            HandleScroll();
        }

        if (_allowZoom)
            HandleZoom();
        
        if (_onFocus)
            LerpToTarget();

        FixBounds();
    }       

    void TryToFocus()
    {
        if (!Input.GetKeyDown(KeyCode.R)) return;

        Vector2 origin = _cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, _focusableMask);

        if (hit)
        {
            _target = hit.collider.transform;
            _onFocus = true;

            StartCoroutine(JumpToFocus());
        }
    }

    void HandleScroll()
    {
        //not holding left click
        if (!Input.GetMouseButton(0)) 
            return;

        //removes focus on any entity
        _onFocus = false;

        //if clicked left
        if (Input.GetMouseButtonDown(0)) 
        {
             _baseMousePosition = Input.mousePosition;
             _baseWorldPosition = transform.position;
             _hasLeftClicked = true;
        }  

        /*
            Take the case of a jump to focus. If the user left clicks
            durign then, then the above if statement will never pass through.
            This is due to the early exit from the update loop.
            As such, has left clicked will be false. Return if this is the 
            case

            Not returning causes a 'jump' in camera position after a 
            jump to focus is finished
        */
        if (!_hasLeftClicked) return;

        Vector3 baseMouseWorld = _cam.ScreenToWorldPoint(_baseMousePosition) - transform.position;
        Vector3 currMouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Vector2 mouseDelta = (currMouseWorld - baseMouseWorld) * _scrollSpeed;

        Vector3 res = _baseWorldPosition - mouseDelta;
        res.z = _cameraZ;
        transform.position = res;

    }

    void HandleZoom()
    {
        float delta = Input.mouseScrollDelta.y * _zoomSpeed * Time.deltaTime;
        _cam.orthographicSize += delta;
    }
 
    void LerpToTarget()
    {
        Vector3 res = Vector3.Lerp(transform.position, _target.position, _followSpeed * Time.deltaTime);
        res.z = _cameraZ;
        transform.position = res;
    }

    void FixBounds()
    {
        Vector2 isoCords = TileMaster.ToIsometricBasis(transform.position);

        //we want c.x >= halfWidth.x, c.y >= halfWidth.y
        //and c.x <= max.x - halfWidth.x, c.y <= max.y - halfWidth.y

        //get the scale factor of the width and height
        Vector2 v1 = TileMaster.Instance.GridBasisRight;
        Vector2 v2 = TileMaster.Instance.GridBasisUp;
        float det = 1 / Mathf.Abs(v1.x * v2.y - v2.x * v1.y);

        float orthographicWidth = _cam.orthographicSize * _cam.aspect;
        orthographicWidth /= 2;

        //clamp it to...
        isoCords.x = Mathf.Clamp(isoCords.x, det * orthographicWidth, TileMaster.Instance.CellBounds.xMax);
        isoCords.y = Mathf.Clamp(isoCords.y, det * orthographicWidth, TileMaster.Instance.CellBounds.yMax);

        //convert back
        isoCords = TileMaster.FromIsometricBasis(isoCords);
        transform.position = isoCords;
        transform.position += Vector3.back * 10;

    }

    IEnumerator JumpToFocus()
    {
        _hasLeftClicked = false;
        _isJumpingToFocus = true;

         /*
            We don't want to loop forever if our camera tries to move
            out of bounds
        */
        Vector2 prevPos = transform.position; 

        const float DELTA = 0.01f;
        while (Vector2.Distance(transform.position, _target.position) > DELTA)
        {
            Vector3 res = Vector2.Lerp(transform.position, _target.position, _jumpToFocusSpeed * Time.deltaTime);
            res.z = _cameraZ;
            transform.position = res;

            if (Vector2.Distance(transform.position, prevPos) < DELTA) break;
            prevPos = transform.position;

            yield return null;
        }

        _isJumpingToFocus = false;
    }

    IEnumerator CameraShake()
    {
        _isShaking = true;

        const float DELTA = 0.01f;

        float elapsedTime = 0.0f;
        Vector2 origin = transform.position;

        /*
            We don't want to loop forever if our camera tries to move
            out of bounds
        */
        Vector2 prevPos = origin; 

        while (elapsedTime < _shakeDuration)
        {
            float dist = Vector2.Distance(origin, transform.position);

            if (dist < 0.1f)
            {
                float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
                Vector2 direc = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 translation = Random.Range(_minShakeRadius, _maxShakeRadius) *
                                      direc;

                transform.Translate(translation);
            }

            Vector3 res = Vector2.Lerp(transform.position, origin, _shakeSpeed * Time.deltaTime);;
            res.z = _cameraZ;

            elapsedTime += Time.deltaTime;

            if (Vector2.Distance(transform.position, prevPos) < DELTA) break;
            prevPos = transform.position;

            yield return null;
        }

        prevPos = origin; 

        //return back
        while (Vector2.Distance(origin, transform.position) > DELTA)
        {
             Vector3 res = Vector2.Lerp(transform.position, origin, _shakeSpeed * Time.deltaTime);
             res.z = _cameraZ;
             transform.position = res;

             if (Vector2.Distance(transform.position, prevPos) < DELTA) break;
             prevPos = transform.position;

             yield return null;
        }

        _isShaking = false;
    }

    IEnumerator CameraShake2()
    {
        _isShaking = true;

        const float DELTA = 0.01f;

        float elapsedTime = 0.0f;
        Vector2 origin = transform.position;

        /*
            We don't want to loop forever if our camera tries to move
            out of bounds
        */
        Vector2 prevPos = origin; 

        while (elapsedTime < _shakeDuration)
        {
            float dist = Vector2.Distance(origin, transform.position);
            float x = Random.Range(-1, 1);
            float y = Random.Range(-1, 1);
            Vector2 offset = new Vector2(x, y).normalized * _minShakeRadius;

            Vector3 res = offset;
            res.z = _cameraZ;
            transform.position = res;

            elapsedTime += Time.deltaTime;

            if (Vector2.Distance(transform.position, prevPos) < DELTA) break;
            prevPos = transform.position;

            yield return null;
        }

        prevPos = origin; 

        //return back
        while (Vector2.Distance(origin, transform.position) > DELTA)
        {
             Vector3 res = Vector2.Lerp(transform.position, origin, _shakeSpeed * Time.deltaTime);
             res.z = _cameraZ;
             transform.position = res;

             if (Vector2.Distance(transform.position, prevPos) < DELTA) break;
             prevPos = transform.position;

             yield return null;
        }

        _isShaking = false;
    }

    void OnDrawGizmos()
    {

    }
}
