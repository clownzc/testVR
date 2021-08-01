// Shatter Toolkit
// Copyright 2012 Gustav Olsson
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
	public int index;
	public ShatterPoint point0, point1;
	public Vector3 line;
	
	public Edge(ShatterPoint point0, ShatterPoint point1)
	{
		this.point0 = point0;
		this.point1 = point1;
		this.line = point1.position - point0.position;
	}
}