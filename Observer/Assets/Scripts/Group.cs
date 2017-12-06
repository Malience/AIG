using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Group {
    public Enemy[] squad;
    [SerializeField]
    public float speed = 0.25f;

    public int casualties;

    void Update()
    {
    }

    public void clear()
    {
        squad = null;
    }

    public void move()
    {
        if (squad == null) return;
        float movement = Time.deltaTime * speed;
        for (int i = 0; i < squad.Length; i++) if (squad[i] != null) squad[i].move(movement);
    }

    public void ResetPath(Maze maze)
    {
        Node path = maze.path(squad[0].transform.localPosition, 0);
        for (int i = 0; i < squad.Length; i++) if (squad[i] != null) squad[i].setPath(path, null);
    }

    public void Path(Maze maze)
    {
        Node path = maze.path(squad[0].transform.localPosition, 0);
        for (int i = 1; i < squad.Length; i++) if (squad[i] != null) squad[i].setPath(path, squad[0].path);
        squad[0].path = path;
    }

    public void PathOpposite(Maze maze)
    {
        Node path = maze.path(squad[0].transform.localPosition, squad[0].path.id);
        for (int i = 0; i < squad.Length; i++) if (squad[i] != null) squad[i].setPath(path, null);
    }
}
