using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberGenerator{

    // Returns a Gaussian distributed random number.
    public static float standardNormal() {
        float v1, v2, w;
        do {
            v1 = Random.Range(-1.0f, 1.0f); // 2.0f * Random.Range(0.0f, 1.0f) - 1.0f;
            v2 = Random.Range(-1.0f, 1.0f); //  2.0f * Random.Range(0.0f, 1.0f) - 1.0f;
            w = v1 * v1 + v2 * v2;
        } while (w >= 1.0f || w == 0.0f);

        w = Mathf.Sqrt((-2.0f * Mathf.Log(w)) / w);

        return v1 * w;
    }

    // Returns a Gaussian distributed random number.
    public static float gaussian(float mean, float standardDeviation) {
        return mean + standardNormal() * standardDeviation;
    }

    // Returns a Gaussian distributed random number.
    public static float gaussian(float mean, float standardDeviation, float min, float max) {
        float x;
        do {
            x = gaussian(mean, standardDeviation);
        } while (x < min || x > max);
        return x;
    }

}
