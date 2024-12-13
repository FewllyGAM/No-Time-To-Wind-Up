using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Transform player;
    [SerializeField] float smoothness = 1f;

    [SerializeField] Vector2Int cameraXRange = new Vector2Int(-70, 70);
    [SerializeField] Vector2Int cameraZRange = new Vector2Int(-70, 70);

    private void Start()
    {
        player = GameManager.gm.player.transform;
    }

    private void Update()
    {
        Vector3 newPos = Vector3.Lerp(transform.position, player.position, Time.deltaTime * smoothness);
        float cameraX = Mathf.Clamp(newPos.x, cameraXRange.x, cameraXRange.y);
        float cameraZ = Mathf.Clamp(newPos.z, cameraZRange.x, cameraZRange.y);
        transform.position = new Vector3(cameraX, transform.position.y, cameraZ);
    }
}
