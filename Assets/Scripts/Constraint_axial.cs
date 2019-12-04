﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Windows.Forms;

public class Constraint_axial : MonoBehaviour {

	public Transform pointPrefab;
	public GameObject torusPrefab;

	public double wx, wy, wz, dx, dy, dz, radius;

	private Transform torusTransform;
	
	// Use this for initialization
	void Start () {
		
		double[] p = new double[3];

		LineRenderer line = gameObject.GetComponent<LineRenderer>();

		//Get each row from csv data file
		GameObject UIController = GameObject.Find("UI Controller");
		
		string data;

		/*StreamReader constraint_data;
		try{
			constraint_data = new StreamReader(UIController.GetComponent<UI_Controller>().constraintDataFilePath);
			data = constraint_data.ReadLine();
			string[] consData = data.Split(new char[] {','} );
			wx = double.Parse(consData[0]);
			wy = double.Parse(consData[1]);
			wz = double.Parse(consData[2]);
			dx = double.Parse(consData[3]);
			dy = double.Parse(consData[4]);
			dz = double.Parse(consData[5]);
		}catch{
			Debug.LogWarning("Constraint_axial.cs: Constraint data file does not exist or cannot be read!");
			MessageBox.Show("Constraint data file does not exist or cannot be read!", "Warning!");
			return;
		}*/

		StreamReader xyz_data;
		try{
			xyz_data = new StreamReader(UIController.GetComponent<UI_Controller>().pointDataFilePath);
		}catch{
			Debug.LogWarning("Constraint_axial.cs: Point data file does not exist or cannot be read!");
			MessageBox.Show("Point data file does not exist or cannot be read!", "Error!");
			return;
		}
		//TextAsset xyz_data = Resources.Load<TextAsset>(dataFileName);
		//string[] data = xyz_data.text.Split(new char[] {'\n'} );

		//Set number of vertices for line
		//line.positionCount = data.Length-1;
		data = xyz_data.ReadLine();
		line.positionCount = 0;
		//Plot each point
		do{
			string[] pointData = data.Split(new char[] {' '} );
			p[0] = double.Parse(pointData[0]);
			p[1] = double.Parse(pointData[2]);
			p[2] = double.Parse(pointData[1]);
			transform.position = new Vector3((float)p[0], (float)p[1], (float)p[2]);
			/*var point = Instantiate(pointPrefab, transform.position, Quaternion.identity);
			point.name = "Point " + (line.positionCount).ToString();*/

			//Set a vertex for the line at the point
			line.SetPosition(line.positionCount++, transform.position);

			//Check if point is on constraint
			//int resConstraint = Func.check_constraint_axial(wx, wy, wz, dx, dy, dz, radius, p);
			
			data = xyz_data.ReadLine();
		}while(data != null);

		//Move constraint center to centroid
		
		double[,] w = {{-wx},{wz},{-wy}};
		double[,] ew = new double[3, 3];
		float qx = 0, qy = 0, qz = 0, qw = 0;

		ew = Func.exp_map(w);
		Func.matToQ(ew, ref qx, ref qy, ref qz, ref qw);

		Quaternion rot = new Quaternion(qx, qy, qz, qw);
		GameObject obj_axialTorus;
		Vector3 center = new Vector3((float)dx, (float)dz, (float)dy);
		obj_axialTorus = (GameObject)Instantiate(torusPrefab, center, rot);
		ParticleSystem.ShapeModule ring = obj_axialTorus.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().shape;
		ring.radius = Mathf.Abs((float)radius);
	}
}
