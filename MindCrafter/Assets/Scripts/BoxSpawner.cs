using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts
{
    [RequireComponent(typeof(BoxEventManager))]
    internal class BoxSpawner : MonoBehaviour
    {
        [SerializeField]
        private BoxSpawner_Requirements requirements = new BoxSpawner_Requirements();
        [Space(10)]
        [SerializeField]
        private BoxSpawner_Options options = new BoxSpawner_Options();

        internal float SpawnRate { get { return spawnRate; } }

        private float spawnRate = 0f;
        private float initialBoxVelocity;
        private List<Color> colors;
        private Grid grid;
        private Box currentBox;
     
        private void Awake()
        {
            spawnRate = options.BoxSpawnRate;
            BoxEventManager.OnSpawnBox += SpawnBox;
            BoxEventManager.OnBoxClicked += BoxClicked;
            grid = requirements.Grid;


        }
        void Start()
        {
            requirements.DropSpeedSlider.value = options.BoxDropVelocity;
            initialBoxVelocity = requirements.DropSpeedSlider.value;
            //requirements.SpawnRateSlider.value = options.BoxSpawnRate;
           // requirements.SpawnRateSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(requirements.SpawnRateSlider.value); });
        }

        /*
        void FixedUpdate() {
            OnClick();
        }*/

        private void OnSliderValueChanged(float value)
        {
            spawnRate = value;
        }


        private void BoxClicked(Box box)
        {
            box.Selected = true;
            Debug.Log("Box Selected");

        }

        private void SpawnBox()
        {
            if (grid.GridBox != null) Destroy(grid.GridBox);
            if (currentBox != null && !currentBox.HasLanded) return;

            //First get a random one
            int column = UnityEngine.Random.Range(0, grid.Columns);
            int row = UnityEngine.Random.Range(0, grid.Rows);

            GridCell dropOn = grid.XZCells[column, row];
            GridCell shortest = SortByHeight(grid.XZCells, grid.Columns, grid.Rows).First();

            //There might be stacks of boxes that are much shorter than the
            //stack of boxes in the random gridcell
            if (dropOn.BoxesInCell - shortest.BoxesInCell >= options.MaxHeight / 2)
            {
                dropOn = shortest;
            }


            //Set droprate to 0 to spam boxes faster
            //NEEDS rework
            if (dropOn.BoxesInCell - shortest.BoxesInCell >= 1)
            {
                dropOn = shortest;
            }

            if (dropOn.BoxesInCell >= options.MaxHeight) return;
            Color BoxColor = grid.Colors[UnityEngine.Random.Range(0, grid.Colors.Count)];


            var cellPosition = dropOn.GameObject.transform.position;
            var spawnPosition = cellPosition;

            GameObject cube = Instantiate(requirements.BoxPrefab);
            Box box = cube.GetComponent<Box>();
            box.Color = BoxColor;
            box.RayCastLength = 0.5f;
            box.transform.parent = this.transform;          
            spawnPosition.y += dropOn.Boxes.Count + options.MaxHeight;
            box.MaxHeight = options.MaxHeight;
            box.SpawnPosition = spawnPosition;
            box.Grid = grid;

            box.DropSpeedSlider = requirements.DropSpeedSlider;
            box.DropSpeedSlider.value = initialBoxVelocity;
           
            dropOn.AddBox(box);
            currentBox = box;
        }

        public void OnClick()
        {

            currentBox.RayCastLength = 100f;
            currentBox.DropSpeedSlider.value = 60f;
            //  yVelocity = 10f;
          //  Debug.Log("Clicked turbo");

//selected = false;

        }
        private List<GridCell> SortByHeight(GridCell[,] xzCells, int columns, int rows)
        {
            GridCell[,] cells = xzCells;
            List<GridCell> temp = new List<GridCell>();
            int shortest = 0;
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (cells[i, j].BoxesInCell >= shortest)
                    {
                        temp.Add(cells[i, j]);
                    }
                }

            }

            return temp;
        }
    }

    [System.Serializable]
    internal sealed class BoxSpawner_Requirements
    {
        [SerializeField]
        private GameObject boxPrefab = null;
        [SerializeField]
        private SliderWithText dropSpeedSlider = null;
        [SerializeField]
        private SliderWithText spawnRateSlider = null;
        [SerializeField]
        private Grid grid = null;

        internal GameObject BoxPrefab { get { return boxPrefab; } }
        internal Slider DropSpeedSlider { get { return dropSpeedSlider.Component; } }
        internal Slider SpawnRateSlider { get { return spawnRateSlider.Component; } }
        internal Grid Grid { get { return grid; } }
    }

    [System.Serializable]
    internal sealed class BoxSpawner_Options
    {
        
        [SerializeField]
        private int maxHeight = 5;
        [SerializeField]
        private float boxSpawnRate = 20f;
        [SerializeField]
        private float boxDropVelocity = 1f;

        internal float BoxSpawnRate { get { return boxSpawnRate; } }
        internal float BoxDropVelocity { get { return boxDropVelocity; } }
        internal int MaxHeight { get { return maxHeight; } }


    }

}

