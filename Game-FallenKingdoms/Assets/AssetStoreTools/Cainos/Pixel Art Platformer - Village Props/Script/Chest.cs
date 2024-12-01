using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.LucidEditor;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    public class Chest : MonoBehaviour
    {
        [FoldoutGroup("Reference")]
        public Animator animator;
        public float interactionDistance = 2f; // Distancia de interacción
        private Transform player; // Referencia al jugador

        [FoldoutGroup("Runtime"), ShowInInspector, DisableInEditMode]
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;
                animator.SetBool("IsOpened", isOpened);
            }
        }
        private bool isOpened;

        private void Start()
        {
            // Encuentra el objeto jugador por su etiqueta
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            // Verifica la distancia al jugador y si se presiona la tecla "F"
            if (Vector2.Distance(transform.position, player.position) < interactionDistance && Input.GetKeyDown(KeyCode.F))
            {
                Open();
            }
        }

        [FoldoutGroup("Runtime"), Button("Open"), HorizontalGroup("Runtime/Button")]
        public void Open()
        {
            IsOpened = true;
        }

        [FoldoutGroup("Runtime"), Button("Close"), HorizontalGroup("Runtime/Button")]
        public void Close()
        {
            IsOpened = false;
        }
    }
}