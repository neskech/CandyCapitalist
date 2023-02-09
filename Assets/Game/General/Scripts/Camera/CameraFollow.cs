using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float _followSpeed;
    [Tooltip("How far from the y axis you're allowed to be from the edge of the map")]
    [SerializeField] int _cameraBoundsY;
    [Tooltip("How far from the x axis you're allowed to be from the edge of the map")]
    [SerializeField] int _cameraBoundsX;
    [SerializeField] bool _renderGizmos;
    [SerializeField] GameObject tileMapBaseLayer;

    float camWidth, camHeight;
    BaseLayout _layout;
    Tilemap _baseLayer;


    private void OnValidate() 
    {
        _layout = tileMapBaseLayer.GetComponent<BaseLayout>();
        _baseLayer = tileMapBaseLayer.GetComponent<Tilemap>();

        Camera cam = GetComponent<Camera>();
        camHeight = 2.0f * cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
    }

    void Start()
    {
        transform.position = _target.transform.position;
    }


    void Update()
    {

        Vector2 camPos = transform.position;
        camPos = Vector2.Lerp(camPos, _target.transform.position, 
                              _followSpeed * Time.deltaTime);

        transform.position = new Vector3(camPos.x, camPos.y, -10);
    }
    
    private void OnDrawGizmos() 
    {
        if (!_renderGizmos) return;

        Vector3 br = _baseLayer.CellToWorld(new Vector3Int(-_cameraBoundsX, -_cameraBoundsY, 1));
        Vector3 bl = _baseLayer.CellToWorld(new Vector3Int(_layout.GridWidth  + _cameraBoundsX, -_cameraBoundsY, 1));
        Vector3 tr = _baseLayer.CellToWorld(new Vector3Int(-_cameraBoundsX, _layout.GridHeight  + _cameraBoundsY, 1));
        Vector3 tl = _baseLayer.CellToWorld(new Vector3Int(_layout.GridWidth + _cameraBoundsX, _layout.GridHeight + _cameraBoundsY, 1));
        Mesh quad = Mesher.CreateQuadMesh(tl, tr, bl, br);

        Gizmos.color = Color.red;
        Gizmos.DrawMesh(quad);
    }
}
