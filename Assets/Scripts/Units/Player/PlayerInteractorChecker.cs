using System.Collections.Generic;
using UnityEngine;
namespace CircleOfLife.Units
{
    public class PlayerInteractorChecker : MonoBehaviour
    {
        private List<GameObject> intetactableIconList;
        private GameObject icon;
        // Start is called before the first frame update
        void Awake()
        {
            intetactableIconList = new List<GameObject>();

        }

        //检测一定范围内物体的Tag,如果是friendlyNPC，则将其标记，使其头上浮现操作提示
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("FriendlyNPC"))
            {
                icon = (GameObject)Resources.Load(UnitIDManager.MapIconDict["Good"]);
                GameObject g = Instantiate(
                        icon,
                        new Vector3(other.transform.position.x, other.transform.position.y + 1.5f, other.transform.position.z),
                        Quaternion.identity,
                        other.transform
                    );
                intetactableIconList.Add(other.gameObject);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("FriendlyNPC"))
            {
                if (intetactableIconList.Contains(other.gameObject))
                {
                    other.gameObject.GetComponentInChildren<InteractableIcon>().DestroyIcon();
                    intetactableIconList.Remove(other.gameObject);
                }
            }
        }
    }
}