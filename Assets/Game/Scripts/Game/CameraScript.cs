using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float panSpeed = 5f;

    /* 08/19, (4)Transform variables allotted for all four(4) systems */
    public Transform floor1;
    public Transform floor2;
    public Transform floor3;
    public Transform floor4;

    void Update()
    {
        float vertical = Input.GetAxis("Horizontal");
        float horizontal = Input.GetAxis("Vertical");

        Vector3 isoHorizontal = new Vector3(-1, 0, 1);
        Vector3 isoVertical = new Vector3(1, 0, 1);

        Vector3 movement = (isoHorizontal * horizontal) + (isoVertical * vertical);

        float speed = 100f;
        transform.position += movement * speed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F))
        {
            transform.position = floor1.position;
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            transform.position = floor2.position;
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            transform.position = floor3.position;
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            transform.position = floor4.position;
        }
    }
}