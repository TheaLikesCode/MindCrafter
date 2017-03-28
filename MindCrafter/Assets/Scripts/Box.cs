using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    internal class Box : MonoBehaviour
    {
        [SerializeField]
        private Material fortifiedMaterial;
        [SerializeField]
        private bool hasLanded;
        [SerializeField]
        private bool shouldBeDestroyed;

        private Grid grid;
        private Material material;
        private Color originalColor;
        private Color currentColor;
        private Color transparent;
        private Color selectedColor;
        private Vector3 spawnPosition;
        private Transform thisTransform;
        private RaycastHit hit;
        private Ray ray;
        private Slider dropSpeedSlider;
        List<Vector3> directions = new List<Vector3>();
        private float velocityY;
        private bool selected;
        private float yVelocity;

        [SerializeField]
        private bool fortified;
        private GameObject guidanceCube;

        protected internal Color Color { get { return currentColor; } set { currentColor = value; } }
        protected internal bool HasLanded { get { return hasLanded; } }
        protected internal bool DoDestroy { get { return shouldBeDestroyed; } }
        protected internal bool Selected { get { return selected; } set { selected = value; } }
        protected internal Vector3 SpawnPosition { set { spawnPosition = value; } }
        protected internal Grid Grid { set { grid = value; } }
        protected internal Slider DropSpeedSlider
        {
            get { return dropSpeedSlider; }
            set
            {

                ConfigureSlider(value);
            }
        }

        public int MaxHeight { get; internal set; }

        private void ConfigureSlider(Slider slider)
        {
            dropSpeedSlider = slider;
            slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(slider.value); });
        }
        private void Start()
        {
            material = GetComponent<MeshRenderer>().material;
            thisTransform = transform;

            Vector3 tempPos = transform.position;
            tempPos.x = spawnPosition.x * thisTransform.lossyScale.x;
            tempPos.z = spawnPosition.z * thisTransform.lossyScale.y;
            tempPos.y = spawnPosition.y;

            transform.position = tempPos;

            transparent = Color.grey;
            transparent.a = 0.6f;

            selectedColor = material.color;
           // yVelocity = 1f;
            //velocityY = 1f;
            velocityY = dropSpeedSlider.value;
            Debug.Log("Start");
            originalColor = Color;
            material.color = Color;
            CreateGuidanceCube();
            directions.Add(transform.right);
            directions.Add(-transform.right);
            directions.Add(transform.forward);
            directions.Add(-transform.forward);
        }
  
        private void FixedUpdate()
        {

           // if (selected) return;
            if (!hasLanded)
            {
                Fall();
                CheckForCollisionBelow();
                if(!hasLanded)
                     CheckGridCollision();
         
            }
            else
            {
                if(!fortified)
                    CheckForSideCollisions();
            }

        }
        private Vector3 screenPoint;
        private Vector3 offset;
        private float lockedY;
     
        private void OnMouseDown()
        {
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));


        }

        private void CreateGuidanceCube()
        {
            float distanceToGrid = transform.position.y - grid.transform.position.y;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = thisTransform;
            float yScale = distanceToGrid / (thisTransform.lossyScale.y);
            Vector3 scale = cube.transform.lossyScale*0.98f;
            scale.y = yScale;
            cube.transform.localScale = scale;
            cube.transform.localPosition = new Vector3(0f, - (cube.transform.lossyScale.y/2f ) - this.transform.lossyScale.y/2f, 0f);

            cube.GetComponent<MeshRenderer>().material = this.GetComponent<MeshRenderer>().material;
            Color orgColor = this.GetComponent<MeshRenderer>().material.color;
            cube.GetComponent<MeshRenderer>().material.color = new Color(orgColor.r, orgColor.g, orgColor.b, 0.4f);
            guidanceCube = cube;
        }

        private void OnMouseDrag()
        {
           
            
            if (!hasLanded && !fortified)
            {
                Debug.Log("Selected");
                selected = true;
                lockedY = transform.position.y;

                Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
                cursorPosition.y = lockedY;
                RepositionWithinGrid(cursorPosition);
                if (guidanceCube != null) return;
                //CreateGuidanceCube();
            }
           
           
        }

        private void RepositionWithinGrid(Vector3 newPosition)
        {
           
            float unitsX = grid.Columns / transform.lossyScale.x;
            float unitsZ = grid.Rows / transform.lossyScale.z;

            Vector3 gridPosition = grid.transform.position;

            if ((int)newPosition.x  >= (int)gridPosition.x && (int)newPosition.x <= (int)gridPosition.x + unitsX - 1)
            {
                newPosition.x = (int)(newPosition.x);
                float distX = newPosition.x - gridPosition.x;
                if (distX >= 0 && distX <= unitsX) transform.position = new Vector3(newPosition.x, transform.position.y, transform.position.z);
            }

            if ((int)newPosition.z >= (int)gridPosition.z && (int)newPosition.z <= (int)gridPosition.z + unitsZ - 1)
            {
                newPosition.z = (int)(newPosition.z);
                float distZ = newPosition.z - gridPosition.z;
                if (distZ >= 0 && distZ <= unitsZ) transform.position = new Vector3(transform.position.x, transform.position.y, newPosition.z); ;
            }

        


        }
        private void OnMouseUp()
        {
            selected = false;
            //Destroy(guidanceCube);
          //  guidanceCube = null;
            Debug.Log("No longer selected");
        }


        private void Fall()
        { 
            Vector3 temp = thisTransform.position;
            temp.y = temp.y - 0.01f * velocityY;
 
            thisTransform.position = temp;
        }

        List<Box> boxesInVicinity = new List<Box>();
        private void CheckForSideCollisions()
        {
          
           
           if (!hasLanded) return;
           if (fortified) return;
            boxesInVicinity.Clear();
            foreach (Vector3 dir in directions)
            {
                ray.direction = dir;
                ray.origin = thisTransform.position;
                Physics.Raycast(ray.origin, ray.direction, out hit, 0.5f, 1 << LayerMask.NameToLayer("Box"));
                if (hit.collider != null)
                {
                  
                    if(hit.distance > 0 && !hit.collider.gameObject.GetComponent<Box>().fortified && hit.collider.gameObject != this)
                        boxesInVicinity.Add(hit.collider.gameObject.GetComponent<Box>());
  
                }

            }


            if (boxesInVicinity.Count == 0) return;
            List<Box> matches = new List<Box>();
            for(int i=0; i < boxesInVicinity.Count; i++)
            {
                Box box = boxesInVicinity[i];

                if (box.originalColor == this.originalColor)
                {
                    matches.Add(box);

                }
                else
                {
                    box.shouldBeDestroyed = true;
                }

            }

            if(matches.Count > 0)
            {
                foreach(Box box in matches)
                {
                    box.fortified = true;
                    box.CheckCombo(gameObject);
                    box.GetComponent<MeshRenderer>().material = fortifiedMaterial;
                    box.GetComponent<MeshRenderer>().material.color = Color.white;
                }


                Fortify();

            }
            else
            {
                grid.PlayNegativeFeedback();
                this.shouldBeDestroyed = true;
            }

        }
       
        private void CheckCombo(GameObject exclude)
        {
            Ray ray = new Ray(); ;
            RaycastHit hit;
            foreach (Vector3 dir in directions)
            {
                ray.direction = dir;
                ray.origin = thisTransform.position;
                Physics.Raycast(ray.origin, ray.direction, out hit, 0.5f, 1 << LayerMask.NameToLayer("Box"));
                if (hit.collider != null)
                {

                    if (hit.distance > 0 && !hit.collider.gameObject.GetComponent<Box>().fortified && hit.collider.gameObject != exclude)
                    {
                        Box box = hit.collider.gameObject.GetComponent<Box>();
                        if (box.originalColor != exclude.GetComponent<Box>().originalColor) return;
                        box.fortified = true;
                        box.GetComponent<MeshRenderer>().material = fortifiedMaterial;
                        box.GetComponent<MeshRenderer>().material.color = Color.white;
                    }
                }

            }

        }
        private void Fortify()
        {
            this.GetComponent<MeshRenderer>().material = fortifiedMaterial;
            GetComponent<MeshRenderer>().material.color = Color.white;
            this.fortified = true;
            grid.PlayPositiveFeedback();
        }

        private float rayCastLength = 0.5f;
        internal float RayCastLength { set { rayCastLength = value; } }
        private void CheckGridCollision()
        {
            ray.direction = new Vector3(0, -1, 0);
            ray.origin = thisTransform.position;
            Physics.Raycast(ray.origin, ray.direction, out hit, rayCastLength, 1 << LayerMask.NameToLayer("GridCell"));

            if (hit.collider == null) return;
            if (hit.distance <= 0) return;

 
            transform.position = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);
            hasLanded = true;

            if (!this.fortified)
            {
                this.GetComponent<MeshRenderer>().material.color = transparent;
            }
            
        }
        // Should be abstracted
        private void CheckForCollisionBelow()
        {
            
            ray.direction = new Vector3(0, -1, 0);
            ray.origin = thisTransform.position;
            Physics.Raycast(ray.origin, ray.direction, out hit, rayCastLength, 1 << LayerMask.NameToLayer("Box"));


            if (hit.collider == null) return;
            if (hit.distance <= 0) return;

            Box placedOn;

            if (!hit.collider.gameObject.GetComponent<Box>()) return;

            placedOn = hit.collider.gameObject.GetComponent<Box>();
      
            //Land the cube
            Vector3 newPos = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);
            transform.position = newPos;
            hasLanded = true;

            if (fortified) return;
            Material thisMaterial = this.GetComponent<MeshRenderer>().material;

            if (placedOn.fortified)
            {
                this.GetComponent<MeshRenderer>().material.color = transparent;
                return;
            }

            //Destroy both boxes if the color of THIS cube is wrong
            if (placedOn.originalColor != this.originalColor)
            {
                shouldBeDestroyed = true;
                placedOn.shouldBeDestroyed = true;
                grid.PlayNegativeFeedback();
                Debug.Log("DESTROYED");
               
            }
            else
            {
               
                this.fortified = true;
                placedOn.fortified = true;
                placedOn.GetComponent<MeshRenderer>().material = fortifiedMaterial;
                GetComponent<MeshRenderer>().material = fortifiedMaterial;
                Debug.Log("HERDET");
            }


            grid.PlayPositiveFeedback();
        }

        private void OnSliderValueChanged(float value)
        {
          
            velocityY = value;
        }
    }
}

