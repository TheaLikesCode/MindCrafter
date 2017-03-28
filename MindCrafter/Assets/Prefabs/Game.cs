using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	private int sizeX,sizeY;
	private List<Tile> tiles;
	private List<Box> boxes;
	private Vector3 origo = Vector3.zero;
	private bool doSpawnBox;
	private float time;
	private int spawnInterval;
	private int velocity;
	private int spawnHeight;
	// Use this for initialization
	void Start () {
		sizeX = 4;
		sizeY = 4;

		velocity = 1;   //1 er raskest
		spawnHeight = 5;
		spawnInterval = spawnHeight+1;
		time=0;
		createGrid();
		boxes = new List<Box>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{

		foreach(Tile t in tiles){ t.update();}
		foreach(Box b in boxes)
		{
			b.update();
			if(b.getObj().transform.position.y < 0.0f)
			{
				b.shouldBeDestroyed = true;
			}
			if(b.shouldBeDestroyed)
			{
				Destroy(b.getObj());
				boxes.Remove(b);
				break;
			}
		}

		AddBox();
	}
		
	private bool doAddBox = false;
	void AddBox()
	{
		//if(Input.GetKeyUp(KeyCode.Space)) 
		if(doAddBox)
		{
			GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
			int randX = Random.Range(0,sizeX);
			int randZ = Random.Range(0,sizeY);
			box.transform.position = new Vector3(randX,spawnHeight+0.5f,randZ);
			int colorID = Random.Range(1,4);
			Color col = Color.black;
			col = GetColorForID(colorID);
			boxes.Add (new Box(box,velocity,col));
			doAddBox=false;
		}
		if(2.0f*Time.time%spawnInterval==1.0) doAddBox=true;
		else doAddBox=false;

	}

	void createGrid()
	{
		tiles = new List<Tile>();

		for(int i=0;i<sizeX;i++)
		{
			for(int j=0;j<sizeY;j++)
			{
				GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
				Vector3 tempPos = tile.transform.position;
			//	origo=new Vector3(sizeX/2.0f ,0,sizeY/2.0f);
	
				tempPos.x = origo.x + i*tile.transform.lossyScale.x;
				tempPos.z = origo.z + j*tile.transform.lossyScale.z;
				tile.transform.Rotate(new Vector3(90,0,0));
				tile.transform.position = tempPos;
		
				MeshRenderer rdr = tile.GetComponent<MeshRenderer>();
				int colorID = Random.Range(1,4);
				Color col = GetColorForID(colorID);

				rdr.material.color = col;//new Color(i*0.1f, j*0.1f, i*0.1f+j*0.1f, 1);

				Tile newTile = new Tile(tile);
				tiles.Add (newTile);
			}  
		}

		
	}
	Color GetColorForID(int id)
	{
		Color col = Color.black;
		if(id==1)col = Color.green;
		if(id==2)col = Color.red;
		if(id==3)col = Color.blue;
		return col;
	}


	class Tile
	{
		private GameObject obj;
		public Tile(GameObject obj)
		{
			this.obj = obj;
		}
		public void update()
		{
			//this.obj.transform.Rotate(0,45,0);

		}
	}

	class Box
	{
		private GameObject obj;
		private float timer;
		private RaycastHit hit;
		private Ray ray;
		private bool hasLanded;
		private int velocity;
		private Color col;
		private MeshRenderer rdr;
		public bool shouldBeDestroyed {
			get;
			set;
		} 

		public Box(GameObject obj,int velocity, Color col)
		{
			this.obj = obj;
			this.timer = 0;
			this.velocity = velocity;
			this.col = col;
			this.rdr = obj.GetComponent<MeshRenderer>();
			rdr.material.color=col;
		}
		public void update()
		{

			Vector3 temp = obj.transform.position;
			if( (2.0f*Time.time%velocity ==.0) && !hasLanded)temp.y -= 1.0f;
			obj.transform.position = temp;

			if(!hasLanded)
			{
				ray.direction = new Vector3(0,-1,0);
				ray.origin = temp;
				Physics.Raycast(ray.origin,ray.direction,out hit,1.0f);
				if(hit.distance>0)
				{
					Color testColor = Color.black;
					try{
						testColor = hit.collider.gameObject.GetComponent<MeshRenderer>().material.color;
					}catch{}
					if(testColor == col && testColor != Color.black)
					{hasLanded=true;return;}
					else shouldBeDestroyed = true;
				}
			}

		}
		public GameObject getObj()
		{
			return obj;
		}
	}
}
