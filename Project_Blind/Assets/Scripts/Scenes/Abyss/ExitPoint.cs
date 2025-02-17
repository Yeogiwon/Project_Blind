using System;
using UnityEngine;

namespace Blind.Abyss
{
    [RequireComponent(typeof(Collider2D))]
    public class ExitPoint : MonoBehaviour
    {
        public Stage stage;
        private Collider2D _collider;

        public void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        public void Disable()
        {
            _collider.enabled = false;
        }
        public void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                if(AbyssSceneManager.Instance.currentStageIndex <6)
                stage.NextStage();
            }
        }
    }
}