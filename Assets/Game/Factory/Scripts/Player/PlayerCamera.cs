using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Transform _player;
    [SerializeField] float _followSpeed;
    [SerializeField] FactoryBaseLayer _baseLayer;

    [Header("Bounds")]
    bool _rectangular;
    int _boundsOffsetX;
    int _boundsOffsetY;

    Camera _cam;
    float _cameraZ;

    // Start is called before the first frame update
    void Start()
    {
        _cameraZ = transform.position.z;
        _cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        LerpToTarget();
        //FixBounds();
    }

    void LerpToTarget()
    {
        Vector3 res = Vector3.Lerp(transform.position, _player.position, _followSpeed * Time.deltaTime);
        res.z = _cameraZ;
        transform.position = res;
    }

    void FixBounds()
    {
        Vector2 isoCords = TileMaster.ToIsometricBasis(transform.position);

        //we want c.x >= halfWidth.x, c.y >= halfWidth.y
        //and c.x <= max.x - halfWidth.x, c.y <= max.y - halfWidth.y

        //get the scale factor of the width and height
        Vector2 v1 = _baseLayer.gridBasisRight;
        Vector2 v2 = _baseLayer.gridBasisUp;
        float det = 1 / Mathf.Abs(v1.x * v2.y - v2.x * v1.y);

        float orthographicWidth = _cam.orthographicSize * _cam.aspect;
        orthographicWidth /= 2;

        //clamp it to...
        isoCords.x = Mathf.Clamp(isoCords.x, det * orthographicWidth, _baseLayer.baseLayer.cellBounds.xMax);
        isoCords.y = Mathf.Clamp(isoCords.y, det * orthographicWidth, _baseLayer.baseLayer.cellBounds.yMax);

        //convert back
        isoCords = _baseLayer.FromIsometricBasis(isoCords);
        transform.position = isoCords;
        transform.position += Vector3.back * 10;

    }
}
