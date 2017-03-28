

namespace Assets.Scripts
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using UnityEngine.UI;

    internal class Grid : MonoBehaviour
    {
        [SerializeField]
        int maxHeight = 1;
        [SerializeField]
        private GameObject quadPrefab;

        [Tooltip("Random colours will be generated if no colours are specified")]
        [SerializeField]
        private List<Color> colors;

        [SerializeField]
        private int numberOfColors = 3;
        [SerializeField]
        private GameObject OutlinePrefab;
        [SerializeField]
        private AudioClip positiveFeedback;

        [SerializeField]
        private AudioClip negativeFeedback;

        private GridCell[,] xzCells;

        private Vector3 origin;

        //   private float spawnRate = 3f;

        private float timer = 0f;
        //    private string spawnRateText = "Spawn Rate: ";
        private int columns;
        private int rows;
        //  private List<Color> colors;
        private int defaultWidth = 1, defaultLength = 1;
        private AudioSource audioSource;

        protected internal int Rows { get { return rows; } }
        protected internal int Columns { get { return columns; } }
        protected internal GameObject GridBox { get; private set; }

        protected internal AudioSource AudioSource { get { return audioSource; } }
        protected internal GridCell[,] XZCells { get { return xzCells; } }
        internal List<Color> Colors
        {
            get
            {
                if (colors.Count == 0 || colors == null)
                {
                    GenerateGridColors();
                }
                return colors;
            }
        }
        private void Start()
        {
            origin = this.transform.position;
            xzCells = new GridCell[0, 0];
            columns = 2;
            rows = 2;
            audioSource = GetComponent<AudioSource>();
            colors = new List<Color>();
            GenerateGridColors();
            GenerateGrid(columns, rows);
            

        }


        private void GenerateGridColors()
        {
            if (colors.Count != 0) return;
            colors = new List<Color>();
            for (int i = 0; i < numberOfColors; i++)
            {
                colors.Add(new Color(UnityEngine.Random.Range(0f, 1f),
                                    UnityEngine.Random.Range(0f, 1f),
                                    UnityEngine.Random.Range(0f, 1f),
                                    1));
            }
        }

        private void GenerateGrid(int newColumnCount, int newRowCount)
        {

            rows = newRowCount;
            columns = newColumnCount;
            Debug.Log("Generating grid with columns: " + newColumnCount + " and rows: " + newRowCount);
            Vector3 newGridPosition = origin;

            newGridPosition.x = (int)(origin.x - (newColumnCount - 1)/2f);
            newGridPosition.z = (int)(origin.z - (newRowCount - 1)/2f);


            this.transform.position = newGridPosition;
            if (GridBox != null) Destroy(GridBox);
            ShowGridOutline();
            xzCells = new GridCell[newColumnCount, newRowCount];
            for (int width = 0; width < newColumnCount; width++)
            {
                for (int length = 0; length < newRowCount; length++)
                {

                    GameObject quad = Instantiate(quadPrefab);
                    quad.transform.Rotate(90, 0, 0);

                    Vector3 tempPos = quad.transform.localPosition;
                    tempPos.x = newGridPosition.x + width;
                    tempPos.z = newGridPosition.z + length;
                    quad.transform.localPosition = tempPos;
                    Vector4 gridColor = new Vector4(Color.grey.r + 0.1f, Color.grey.g + 0.1f, Color.grey.b + 0.1f, 1f);
                    quad.GetComponent<MeshRenderer>().material.color = gridColor;
                    var cell = quad.GetComponent<GridCell>();
                    cell.Parent = transform;
                    // cell.CorrectColor = Colors[UnityEngine.Random.Range(0, Colors.Count)];
                    // var cell = new GridCell(quadPrefab, width, length, newGridPosition) { Parent = transform, CorrectColor = Colors[UnityEngine.Random.Range(0, Colors.Count)] };
                    xzCells[width, length] = cell;

                }
            }
        }

        private void ShowGridOutline()
        {
            GameObject gridBox = Instantiate(OutlinePrefab);
      //      gridBox.GetComponent<MeshRenderer>().material.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.3f);
            gridBox.transform.parent = this.transform;
            gridBox.transform.localScale = new Vector3(columns, maxHeight, rows);
            //gridBox.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
            gridBox.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            int unitsX = (int)gridBox.transform.localScale.x / columns;
            int unitsZ = (int)gridBox.transform.localScale.z / rows;
            float positionx = gridBox.transform.position.x - gridBox.transform.lossyScale.x/2f + 0.5f;
            float positionz = gridBox.transform.position.z - gridBox.transform.lossyScale.z / 2f + 0.5f;
            float positiony = transform.position.y + maxHeight / 2f;


            //   Vector3 scale = gridBox.transform.lossyScale;
            //  scale.y = yScale;
           
            gridBox.transform.localPosition = new Vector3(positionx, positiony, positionz);
            GridBox = gridBox;
        }
        private void DestroyRows()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {

                    Destroy(xzCells[j, i].GameObject);
                    foreach (Box b in xzCells[j, i].Boxes)
                    {
                        Debug.Log("Destroying box");
                        Destroy(b.gameObject);
                    }
                    xzCells[j, i].Boxes.Clear();
                }
            }
        }

        private void DestroyColumns()
        {
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Destroy(xzCells[i, j].GameObject);
                    foreach (Box b in xzCells[i, j].Boxes)
                    {
                        Debug.Log("Destroying box");
                        Destroy(b.gameObject);
                    }
                    xzCells[i, j].Boxes.Clear();
                }
            }
        }


        internal void PlayNegativeFeedback()
        {
            audioSource.clip = negativeFeedback;
            audioSource.Play();
        }

        internal void PlayPositiveFeedback()
        {
            audioSource.clip = positiveFeedback;
            audioSource.Play();
        }
        public void ChangeRows(string size)
        {
            int newRowCount;
            if (!Int32.TryParse(size, out newRowCount))
            {
                return;
            }
            DestroyRows();
            GenerateGrid(columns, newRowCount);
        }
        public void ChangeColumns(string size)
        {
            int newColumnCount;
            if (!Int32.TryParse(size, out newColumnCount))
            {
                return;
            }
            DestroyColumns();
            GenerateGrid(newColumnCount, rows);
        }
        public void ChangeNumberOfColors(string size)
        {
            int newColorCount;
            if (!Int32.TryParse(size, out newColorCount))
            {
                return;
            }
            DestroyColumns();
            DestroyRows();
            numberOfColors = newColorCount;
            colors.Clear();
            GenerateGridColors();
            GenerateGrid(columns, rows);
        }




    }
}

