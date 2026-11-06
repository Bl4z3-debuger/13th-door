using UnityEngine;
using UnityEngine.UIElements;

public class Player_script : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Start()
    {
        Debug.Log("Start");
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(x, 0, z).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W Key was pressed!");
        }
    }
}
