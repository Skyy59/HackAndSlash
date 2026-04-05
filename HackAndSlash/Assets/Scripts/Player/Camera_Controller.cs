using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Player_Controller player;
    [SerializeField] private Transform crosshair;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float crosshairMaxDistance;
    [SerializeField] private float crosshairReductionMult;

    private void OnEnable() 
    {
        player.SendLookInput += MoveCrosshair;    
    }

    private void OnDisable() 
    {
        player.SendLookInput -= MoveCrosshair;
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera.transform.position = (player.transform.position + crosshair.position) / 2f + cameraOffset;
    }

    public void MoveCrosshair(Vector2 _lookInput)
    {
        Vector2 _position = crosshair.position;
        _position += _lookInput * crosshairReductionMult;

        if (Vector3.Distance(_position, player.transform.position) < crosshairMaxDistance) crosshair.position = _position;
    }
}
