using UnityEngine;
using System.Collections;

public class ChangeColor : MonoBehaviour {

    private Renderer render;
    private Color currentColor = Color.black;
    private System.Random rnd;
    // Set new color after this duration
    public float duration = 0.05f;
    // Increment values for each color
    private float dr, dg, db;
    // Change color after one duration
    private float colorTick = 1f / 10f;
    // Current time counter
    private float t = 0;

    // Use this for initialization
    void Start () {
        render = GetComponent<Renderer>();
        render.material.color = currentColor;

        rnd = new System.Random(System.DateTime.Now.Millisecond);
        // Initialy all increments for all colors positive values
        dr = dg = db = colorTick;
	}
	
	// Update is called once per frame
	void Update () {
        // Duration is not reached
        if (t < duration)
        {
            // Add current duration to time counter
            t += Time.deltaTime;
        }
        else // time duration reached
        {
            // Zero timer counter
            t = 0;
            // First lets choose color for change
            int color = rnd.Next(0, 3);
            switch (color)
            {
                case 0: // Change red
                        // If we reach top value for red
                        // we cant increase more so we backward
                    if (currentColor.r + dr > 1f)
                    {
                        dr = -colorTick;
                    }
                    else if (currentColor.r + dr < 0)
                    {
                        dr = +colorTick;
                    }
                    currentColor.r += dr;
                    break;
                case 1: // Change green
                    if (currentColor.g + dg > 1f)
                    {
                        dg = -colorTick;
                    }
                    else if (currentColor.g + dg < 0)
                    {
                        dg = +colorTick;
                    }
                    currentColor.g += dg;
                    break;
                default: // Default change blue
                    if (currentColor.b + db > 1)
                    {
                        db = -colorTick;
                    }
                    else if (currentColor.b + db < 0)
                    {
                        db = +colorTick;
                    }
                    currentColor.b += db;
                    break;
            }
            // Set up new color to the gameObject
            render.material.color = currentColor;
        }
    }
}
