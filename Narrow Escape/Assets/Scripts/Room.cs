using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {Small = 0, Large = 1, Hallway = 2, HallwayRoom = 3, Hide = 4};

public class Room : MonoBehaviour {

    public int width;
    public int height;
    public int tileSize;
    public RoomType type = RoomType.Small;

    void Start() {
        //type = RoomType.Small;
    }

    // Debug graphics
    void OnDrawGizmos() {
        if (type == RoomType.Small) { Gizmos.color = Color.white; }
        if (type == RoomType.Large) { Gizmos.color = Color.cyan; }
        if (type == RoomType.Hallway) { Gizmos.color = Color.red; }
        if (type == RoomType.HallwayRoom) { Gizmos.color = new Color(0.5f, 0.0f, 0.0f); }
        if (type == RoomType.Hide) { Gizmos.color = Color.clear; }
        Gizmos.DrawCube(this.transform.position, new Vector3(width * tileSize, height * tileSize, 0));
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(this.transform.position, new Vector3(width * tileSize, height * tileSize, 0));
    }
}
