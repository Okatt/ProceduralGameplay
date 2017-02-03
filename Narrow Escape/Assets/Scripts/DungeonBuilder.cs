using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour {

    public int mean = 0;
    public int stddev = 4;
    public int rooms = 100;
    public int tileSize = 100;
    public int radius = 1000;

    public int extraPaths = 0;
    
    private List<GameObject> list;          // All rooms
    private List<GameObject> listL;         // All large rooms
    private List<GameObject> listH;         // All hallways
    private List<Pair<string, string>> listCL;
    private bool separationCompleted = false;
    private bool selectionCompleted = false;
    private bool connectionComplete = false;
    private bool hallwaysCompleted = false;

	// Use this for initialization
	void Start () {
        list = new List<GameObject>();
        listL = new List<GameObject>();
        listH = new List<GameObject>();
        listCL = new List<Pair<string, string>>();
        generateRooms();
	}
	
	// Update is called once per frame
	void Update () {
        if (!separationCompleted) separationCompleted = separateRooms();
        if (separationCompleted && !selectionCompleted) selectionCompleted = selectRooms();
        if (selectionCompleted && !connectionComplete) connectionComplete = connectRooms();
        if (connectionComplete && !hallwaysCompleted) hallwaysCompleted = createHallways();
	}

    // Generate a bunch of rooms.
    void generateRooms() {
        Debug.Log("Generating rooms...");
        for (int i = 0; i < rooms; i++) {
            Vector2 pos = getRandomPointInCircle(radius);
            pos = snapToGrid(pos);

            // Create a room.
            GameObject room = new GameObject("Room_" + list.Count.ToString("D3"));
            room.AddComponent<Room>();
            room.GetComponent<Room>().width = (int) Mathf.Ceil(Mathf.Abs(NumberGenerator.gaussian(mean, stddev) / 2)) * 2;
            room.GetComponent<Room>().height = (int) Mathf.Ceil(Mathf.Abs(NumberGenerator.gaussian(mean, stddev, room.GetComponent<Room>().width - stddev, room.GetComponent<Room>().width) / 2)) * 2;
            room.GetComponent<Room>().tileSize = tileSize;
            room.AddComponent<AABB>();
            room.GetComponent<AABB>().width = room.GetComponent<Room>().width * tileSize;
            room.GetComponent<AABB>().height = room.GetComponent<Room>().height * tileSize;
            room.transform.position = new Vector3(pos.x, pos.y, 0);
            
            list.Add(room);
        }
        Debug.Log("Room generation completed");
    }

    // TODO when overlap and px < py. when d.x = 0 nothing happends FIX;
    bool separateRooms() {
        Debug.Log("Seperating rooms...");
        bool isCompleted = true;

        // Check all rooms with each other.
        for (int i = 0; i < list.Count; i++) {
            for (int j = i + 1; j < list.Count; j++) {
                AABB one = list[i].GetComponent<AABB>();
                AABB two = list[j].GetComponent<AABB>();

                Vector2 d = two.position - one.position;        // Direction vector from AABB1 to AABB2.
                float px = one.xw + two.xw - Mathf.Abs(d.x);    // Penetration on the X axis.
                float py = one.yw + two.yw - Mathf.Abs(d.y);    // Penetraion on the Y axis.

                // Check collision.
                if (px > 0 && py > 0) {
                    // one and two are overlapping. We still have overlapping rooms so the separation has not been completed.
                    isCompleted = false;
                    if (px <= py) {
                        // Separate on the X axis.
                        int dx = System.Math.Sign(d.x) != 0 ? System.Math.Sign(d.x) : 1;

                        one.position = new Vector2(one.position.x + px * -dx, one.position.y);
                        two.position = new Vector2(two.position.x + px * dx, two.position.y);
                    }
                    else {
                        // Sperate on the Y axis.
                        int dy = System.Math.Sign(d.y) != 0 ? System.Math.Sign(d.y) : 1;

                        one.position = new Vector2(one.position.x, one.position.y + py * -dy);
                        two.position = new Vector2(two.position.x, two.position.y + py * dy);
                    }
                }
            }
        }

        if(isCompleted) Debug.Log("Separation rooms completed");
        return isCompleted;
    }

    // Select the larger rooms.
    bool selectRooms() {
        Debug.Log("Selecting large rooms...");

        foreach (GameObject r in list) {
            Room roomData = r.GetComponent<Room>();
            if (roomData.width >= mean + stddev * 1.25f && roomData.height >= mean + stddev * 1.25f) {
                roomData.type = RoomType.Large;
                listL.Add(r);
            }
        }
        
        Debug.Log("Selecting large rooms completed");
        return true;
    }

    // Connect the selected rooms with a tree structure.
    bool connectRooms() {
        Debug.Log("Connecting rooms...");

        if (listL.Count == 0) {
            Debug.Log("There are no rooms to connect");
            return false;
        }

        // Create a PrimGraph.
        PrimGraph graph = new PrimGraph();

        // Create a vertex for each large room.
        foreach (GameObject r in listL) {
            graph.vertices.Add(r.name);
        }

        // Connect all large rooms.
        for (int i = 0; i < listL.Count; i++) {
            for (int j = i + 1; j < listL.Count; j++) {
                GameObject room1 = listL[i];
                GameObject room2 = listL[j];
                int weight = (int) Vector2.Distance(room2.GetComponent<AABB>().position, room1.GetComponent<AABB>().position);

                graph.connect(room1.name, room2.name, weight);
            }
        }

        // Create a Pathfinder and find the MST
        listCL = Pathfinder.prim(graph, listL[0].name);

        Debug.Log("listCL count:" + listCL.Count);

        // Create a few extra branching paths TODO: clean up
        int r1, r2;

        int c = extraPaths;
        while(c > 0){
            r1 = (int) Mathf.Round(Random.Range(0, listL.Count - 1));
            r2 = (int) Mathf.Round(Random.Range(1, listL.Count - 1));
            r2 = (r2 + r1) % (listL.Count);

            Pair<string, string> randomPath = new Pair<string, string>(listL[r1].name, listL[r2].name);

            bool dup = false;
            foreach(Pair<string, string> path in listCL){
                if ((randomPath.first.Equals(path.first) && randomPath.second.Equals(path.second)) ||
                    (randomPath.first.Equals(path.second) && randomPath.second.Equals(path.first))) {
                        dup = true;
                        break;
                }
            }
            if (!dup) { listCL.Add(randomPath); c -= 1; Debug.Log(randomPath.first + " -- " + randomPath.second); }
        }

        Debug.Log("Connecting rooms completed");
        return true;
    }

    // Create hallways TODO: clean this shit up
    bool createHallways() {
        Debug.Log("Creating hallways...");

        foreach (Pair<string, string> p in listCL) {
            AABB one = GameObject.Find(p.first).GetComponent<AABB>();
            AABB two = GameObject.Find(p.second).GetComponent<AABB>();

            Vector2 d = two.position - one.position;
            float px = one.xw + two.xw - Mathf.Abs(d.x);
            float py = one.yw + two.yw - Mathf.Abs(d.y);

            GameObject hallway = new GameObject(GameObject.Find(p.first).name + "_HallwayTo_" + GameObject.Find(p.second).name);
            hallway.AddComponent<Room>();
            hallway.GetComponent<Room>().width = 2 + (int) Mathf.Abs(d.x) / tileSize;
            hallway.GetComponent<Room>().height = 2;
            hallway.GetComponent<Room>().tileSize = tileSize;
            hallway.GetComponent<Room>().type = RoomType.Hallway;
            hallway.AddComponent<AABB>();
            hallway.GetComponent<AABB>().width = hallway.GetComponent<Room>().width * tileSize;
            hallway.GetComponent<AABB>().height = hallway.GetComponent<Room>().height * tileSize;
            hallway.transform.position = new Vector3(one.position.x + Mathf.Abs(d.x / 2) * System.Math.Sign(d.x), one.position.y, 10);
            listH.Add(hallway);

            GameObject hallway2 = new GameObject(GameObject.Find(p.second).name + "_HallwayTo_" + GameObject.Find(p.first).name);
            hallway2.AddComponent<Room>();
            hallway2.GetComponent<Room>().width = 2;
            hallway2.GetComponent<Room>().height = 2 + (int) Mathf.Abs(d.y) / tileSize;
            hallway2.GetComponent<Room>().tileSize = tileSize;
            hallway2.GetComponent<Room>().type = RoomType.Hallway;
            hallway2.AddComponent<AABB>();
            hallway2.GetComponent<AABB>().width = hallway2.GetComponent<Room>().width * tileSize;
            hallway2.GetComponent<AABB>().height = hallway2.GetComponent<Room>().height * tileSize;
            hallway2.transform.position = new Vector3(two.position.x, two.position.y + Mathf.Abs(d.y / 2) * -System.Math.Sign(d.y), 10);
            listH.Add(hallway2);
        }

        // TODO: remove test shit
        foreach (GameObject go in list) {
            for (int j = 0; j < listH.Count; j++) {
                AABB one = go.GetComponent<AABB>();
                AABB two = listH[j].GetComponent<AABB>();

                Vector2 d = two.position - one.position;        // Direction vector from AABB1 to AABB2.
                float px = one.xw + two.xw - Mathf.Abs(d.x);    // Penetration on the X axis.
                float py = one.yw + two.yw - Mathf.Abs(d.y);    // Penetraion on the Y axis.

                // Check collision.
                if (go.GetComponent<Room>().type == RoomType.Small) {
                    go.GetComponent<Room>().type = px >= 0 && py >= 0 ? RoomType.HallwayRoom : go.GetComponent<Room>().type;
                }
            }
        }

        foreach (GameObject go in list) {
            if (go.GetComponent<Room>().type == RoomType.Small) {
                go.GetComponent<Room>().type = RoomType.Hide;
            }
        }

        Debug.Log("Creating hallways completed");
        return true;
    }

    // Returns a uniform random point in a circle. 
    Vector2 getRandomPointInCircle(int radius) {
        float t = 2 * Mathf.PI * Random.value;
        float u = Random.value + Random.value;
        float r = u > 1 ? 2 - u : u;

        float x = radius * r * Mathf.Cos(t);
        float y = radius * r * Mathf.Sin(t);

        return new Vector2(x, y);
    }

    Vector2 snapToGrid(Vector2 point) {
        int x = ((int) Mathf.Round(point.x / tileSize)) * tileSize;
        int y = ((int) Mathf.Round(point.y / tileSize)) * tileSize;

        return new Vector2(x, y);
    }

    // Debug graphics
    void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.color = Color.green;
            foreach (Pair<string, string> p in listCL) {
                if (listCL.IndexOf(p) > (listCL.Count - 1) - extraPaths) { Gizmos.color = new Color(0.0f, 0.5f, 0.0f); }
                else { Gizmos.color = Color.green; }

                GameObject room1 = GameObject.Find(p.first);
                GameObject room2 = GameObject.Find(p.second);
                Gizmos.DrawLine(room1.transform.position, room2.transform.position);
            }
        }
    }

}
