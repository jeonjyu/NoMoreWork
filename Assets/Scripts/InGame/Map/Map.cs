using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : ObjectBase
{
    public GameObject puzzleObjects;
    public GameObject mapObjects;
    public GameObject gate;
    public OutTrigger outTrigger;
    public Tilemap tilemap;
    public CinemachineVirtualCamera virtualCamera;
}
