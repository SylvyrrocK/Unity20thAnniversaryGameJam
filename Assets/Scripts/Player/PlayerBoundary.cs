using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
       [SerializeField] float xMin, xMax, zMin, zMax;

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        pos.y = Mathf.Clamp(pos.y, zMin, zMax);
        transform.position = pos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 topLeft = new Vector3(xMin, zMax, 0);
        Vector3 topRight = new Vector3(xMax, zMax, 0);
        Vector3 bottomLeft = new Vector3(xMin, zMin, 0);
        Vector3 bottomRight = new Vector3(xMax, zMin, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
