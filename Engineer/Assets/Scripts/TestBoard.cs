using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoard : MonoBehaviour {
    Group group;
    Maze maze;


	// Use this for initialization
	void Start () {
        maze.seed = Random.Range(0, 1000000000);
        maze.MazeGen(maze.seed);


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
