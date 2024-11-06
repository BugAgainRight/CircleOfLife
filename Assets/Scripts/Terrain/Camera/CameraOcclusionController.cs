using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CircleOfLife
{
    public enum CameraFollowerTag
    {
        Player
    }
    public class CameraOcclusionController : MonoBehaviour
    {
        public GameObject playerGo;
        public Transform playerTransform
        {
            get
            {
                if (playerGo == null)
                {
                    FindPlayerGo();
                }
                return playerGo.transform;
            }
        }
        private MeshRenderer playerMeshRenderer;
        private ConcurrentDictionary<GameObject, bool> occlusionsDict;
        private List<GameObject> lastOcclusions;
        private float colorAlpha = 0.3f;

        void Start()
        {
            FindPlayerGo();
        }

        void FixedUpdate()
        {

            CheckRayCastCollider(playerTransform.position);
            RegisterOcclusion();
            UpdateOcclusions();
        }
        //寻找玩家
        private void FindPlayerGo()
        {
            playerGo = GameObject.FindGameObjectWithTag(CameraFollowerTag.Player.ToString());
            playerMeshRenderer = playerGo.GetComponent<MeshRenderer>();

            occlusionsDict = new ConcurrentDictionary<GameObject, bool>();
        }

        //登记射线检测到的碰撞体
        private void CheckRayCastCollider(Vector3 originPosOnWorld)
        {
            lastOcclusions = new List<GameObject>();
            Vector3 playerPos = Camera.main.WorldToScreenPoint(originPosOnWorld);
            Vector2 cameraPos = Camera.main.ScreenToWorldPoint(new Vector3(playerPos.x, playerPos.y, 0));
            RaycastHit2D[] hits = Physics2D.RaycastAll(cameraPos, -Vector2.zero);
            foreach (RaycastHit2D hit in hits)
            {
                //Debug.Log(hit.collider.name);
                lastOcclusions.Add(hit.collider.gameObject);
            }
        }
        //注册射线获取的除了玩家以外的所有碰撞体
        private void RegisterOcclusion()
        {
            foreach (GameObject occlusion in lastOcclusions)
            {
                if (occlusion == null || occlusion.tag == CameraFollowerTag.Player.ToString())
                {
                    continue;
                }
                if (occlusionsDict.ContainsKey(occlusion))
                {
                    occlusionsDict[occlusion] = true;
                }
                else
                {
                    occlusionsDict.TryAdd(occlusion, true);
                }
            }
        }
        //更新currentOcclusions，将不在lastOcculusions的碰撞体Alpha设至为1后,bool设为false
        private void UpdateOcclusions()
        {
            foreach (KeyValuePair<GameObject, bool> occlusion in occlusionsDict)
            {
                if (lastOcclusions.Contains(occlusion.Key) == false)
                {
                    occlusionsDict[occlusion.Key] = false;
                    SetAlphaToOcclusion(occlusion.Key, 1f);
                }
                else if (occlusion.Value == false)
                {
                    SetAlphaToOcclusion(occlusion.Key, 1f);
                }
                else
                {
                    SetAlphaToOcclusion(occlusion.Key, colorAlpha);
                }
            }
        }
        //尝试获取碰撞体的sortingOrder
        private int GetSortingOrder(GameObject gameObject)
        {
            SpriteRenderer spriteRenderer;
            MeshRenderer meshRenderer;
            Tilemap tilemap;
            if (gameObject.TryGetComponent(out spriteRenderer))
            {
                return spriteRenderer.sortingOrder;
            }
            else if (gameObject.TryGetComponent(out tilemap))
            {
                TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
                return tilemapRenderer.sortingOrder;
            }
            else if (gameObject.TryGetComponent(out meshRenderer))
            {
                return meshRenderer.sortingOrder;
            }
            return 0;
        }
        //改变碰撞体的Alpha值
        private void ChangeAlpha(GameObject gameObject, float a)
        {
            SpriteRenderer spriteRenderer;
            MeshRenderer meshRenderer;
            Tilemap tilemap;
            if (gameObject.TryGetComponent(out spriteRenderer))
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.b, spriteRenderer.color.g, a);
            }
            else if (gameObject.TryGetComponent(out tilemap))
            {
                TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
                tilemap.color = new Color(tilemap.color.r, tilemap.color.b, tilemap.color.g, a);
            }
            else if (gameObject.TryGetComponent(out meshRenderer))
            {
                meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.b, meshRenderer.material.color.g, a);
            }
        }
        //改变碰撞体的Alpha值
        private void SetAlphaToOcclusion(GameObject gameObject, float a)
        {
            if (a < 0.9f && GetSortingOrder(gameObject) < GetSortingOrder(playerGo))
            {
                return;
            }
            ChangeAlpha(gameObject, a);
        }
    }
}
