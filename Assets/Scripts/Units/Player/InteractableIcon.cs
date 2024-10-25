using UnityEngine;
public class InteractableIcon : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 2f, 0);
    private GameObject parentGameObject;
    void Awake()
    {
        parentGameObject = transform.parent.gameObject;
    }

    void Update()
    {
        transform.position = parentGameObject.transform.position + offset;
    }

    public void DestroyIcon()
    {
        Destroy(this.gameObject);
    }
}

