using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CircleOfLife.MapTile
{
    [RequireComponent(typeof(Tilemap))]
    [RequireComponent(typeof(TilemapRenderer))]
    public class NonCollisionWall : MonoBehaviour
    {
        Tilemap tilemap;

        TilemapRenderer tilemapRenderer;
        private int playerCurrentOrderInLayer;
        void Awake()
        {
            tilemap = GetComponent<Tilemap>();
            tilemapRenderer = GetComponent<TilemapRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerCurrentOrderInLayer = other.GetComponent<SpriteRenderer>().sortingOrder;
                other.GetComponent<SpriteRenderer>().sortingOrder = tilemapRenderer.sortingOrder - 1;
                tilemap.color = new Color(1, 1, 1, 0.5f);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<SpriteRenderer>().sortingOrder = playerCurrentOrderInLayer + 1;
                tilemap.color = new Color(1, 1, 1, 1f);
            }
        }
    }
}
