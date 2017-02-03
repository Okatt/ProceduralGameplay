using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB : MonoBehaviour {

    public float width;
    public float height;

    public AABB() {
        this.width = 0;
        this.height = 0;
    }

    public AABB(float w, float h) {
        this.width = w;
        this.height = h;
    }

    public Vector2 position {
        get { return (Vector2) this.transform.position; }
        set { this.transform.position = new Vector3(value.x, value.y, this.transform.position.z); }
    }

    public float xw {
        get { return width / 2; }
    }

    public float yw {
        get { return height / 2; }
    }

    // Debug graphics
    void OnDrawGizmos() {
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(this.transform.position, new Vector3(width, height, 0));
    }

}
