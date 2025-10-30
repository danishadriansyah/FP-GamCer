using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 

    private float cameraZ;

    void Start()
    {
        
        cameraZ = transform.position.z;
    }

    void LateUpdate()
    {
        // Jika target (Player) sudah ada
        if (target != null)
        {
            Vector3 newPosition = new Vector3(target.position.x, target.position.y, cameraZ);

            // Atur posisi kamera ke posisi baru tersebut
            transform.position = newPosition;
        }
    }
}