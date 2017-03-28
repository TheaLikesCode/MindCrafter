
namespace Assets.Scripts
{
    using System;
    using UnityEngine;


    internal class BoxEventManager : MonoBehaviour
    {

        internal delegate void BoxEvent();
        internal delegate void ClickEvent(Box b);

        internal static event BoxEvent OnSpawnBox;
        internal static event ClickEvent OnBoxClicked;

        private BoxSpawner spawner;
        private float spawnRate = 0f;
        private float spawnTimer = 0f;

        public static Action<Box> OnBoxReleased { get; internal set; }

        void Start()
        {
            spawner = GetComponent<BoxSpawner>();
        }

        void Update()
        {

            if (spawnTimer < spawner.SpawnRate)
            {
                spawnTimer += Time.deltaTime;
                return;
            }

            if (OnSpawnBox == null) return;
            OnSpawnBox();
            spawnTimer = 0;

      

        }
      
    }


}
