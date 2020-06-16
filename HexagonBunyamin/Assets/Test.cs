using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// Profiller için yada test amaçlı kullandığım bir scritp

public class Test : MonoBehaviour/*, IEqualityComparer<Test>*/
{


    public Collider2D[] allColiders;
    public Collider2D[] idcolider;
    public Hexagon[] hexagons;
    Dictionary<int, Hexagon> allHex = new Dictionary<int, Hexagon>();

    private void Start()
    {
        // hexagons = new Hexagon[idcolider.Length];
        // for (int i = 0; i < allColiders.Length; i++)
        // {
        //     allHex.Add(allColiders[i].GetInstanceID(), allColiders[i].GetComponent<Hexagon>());
        // }

        StartCoroutine(FallAnim());
    }
    public float dotFloat;

    public Vector2 ihs;
    public Vector2 rhs;
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dotFloat = Vector2.Dot(ihs , mousePos - ihs);
        Debug.DrawLine(Vector2.zero, ihs);
        Debug.DrawRay(ihs, mousePos - ihs);
        if (Input.GetMouseButtonDown(0))
        {
            ihs = new Vector2(ihs.x, mousePos.y);
            if (ihs.y < 0) {
            }
        }

        if (Input.GetMouseButton(0))
        {
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            // for (int i = 0; i < hexagons.Length; i++)
            // {
            //     hexagons[i] = allHex[idcolider[i].GetInstanceID()];
            // }
        }


    }
    public float duration = 1;
    public float fallTime = 3;

    
    public float zamanFarki = 2;
    
    public IEnumerator FallAnim()
    {
        float startTime = Time.time;
        float speed = 1f / fallTime;
        for (int i = 0; i < 3; i++)
        {
            float percent = 0;
            while (percent < 1)
            {
                percent += speed * Time.deltaTime;
                percent = Mathf.Clamp01(percent);
                yield return null;
            }
            yield return new WaitForSeconds(duration);
        }
        zamanFarki = Time.time - startTime;
    }
 
    private void OnDrawGizmos() {
        
    }
}
