namespace Assets.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    internal class GridCell : MonoBehaviour
    {
        //int posX, posZ;
       // Vector2 gridPosition;
        private List<Box> boxes;
      
        protected internal Transform Parent { set { gameObject.transform.parent = value; } }
        protected internal GameObject GameObject { get { return gameObject; } }
        protected internal List<Box> Boxes { get { return boxes; } set { boxes = value; } }
        protected internal int BoxesInCell { get { return boxes.Count; } }

        private Color correctColor;
 
        protected internal Color CorrectColor { get { return correctColor; } set { gameObject.GetComponent<MeshRenderer>().material.color = value; correctColor = value; } }
        
        internal GridCell(GameObject prefab, int positionX, int positionZ, Vector3 origin)
        {
            // posX = (int)gridPosition.x;
            //  posZ = (int)gridPosition.y;
            // this.gridPosition = gridPosition;
           
         /*  var gameObject = Instantiate(prefab);
            gameObject.transform.Rotate(90, 0, 0);
           // gameObject.AddComponent<BoxCollider>();

            Vector3 tempPos = gameObject.transform.localPosition;

            tempPos.x = origin.x + positionX * gameObject.transform.lossyScale.x;
            tempPos.z = origin.z + positionZ * gameObject.transform.lossyScale.y;

            gameObject.transform.localPosition = tempPos;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
*/

        }

        private void Start()
        {
            boxes = new List<Box>();
        }

        private void Update()
        {
          
            foreach(Box b in boxes)
            {
                if (b.DoDestroy)
                {
                    Destroy(b.gameObject);

                    boxes.Remove(b);
                    break;

                }
               

            }
        }


    


 
    internal void AddBox(Box box)
        {
            boxes.Add(box);
            
        }
    
    }
}
