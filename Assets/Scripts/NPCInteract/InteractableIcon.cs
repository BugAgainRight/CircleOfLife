using UnityEngine;
namespace CircleOfLife.NPCInteract
{
    public class InteractableIcon : MonoBehaviour
    {
        public Vector3 IconOffset = new Vector3(0, 2f, 0);

        private GameObject parentGameObject;
        private Transform parentTransform;
        private Vector3 direction;

        public void Initialize()
        {
            direction = new Vector3(1, 1, 1);
            parentGameObject = transform.parent.gameObject;
            parentTransform = transform.parent;
            float x = transform.localScale.x / parentTransform.localScale.x;
            float y = transform.localScale.y / parentTransform.localScale.y;
            float z = transform.localScale.z / parentTransform.localScale.z;
            transform.localScale = new Vector3(x, y, z);
        }
        void FixedUpdate()
        {
            transform.position = parentGameObject.transform.position + IconOffset;
            if (parentTransform.localScale != direction)
            {
                direction = parentTransform.localScale;
                var scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
                transform.localScale = scale;
            }
        }
    }
}


