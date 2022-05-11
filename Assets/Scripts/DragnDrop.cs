using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class DragnDrop : MonoBehaviour
{
    private Plane dragPlane;


    private Vector3 offset;

    private Camera myMainCamera;


    static Material lineMaterial;

    CapsuleCollider zCollider;
    CapsuleCollider xCollider;
    CapsuleCollider yCollider;

    private bool selected;
    private bool dragX;
    private bool dragY;
    private bool dragZ;

    void Start()
    {
        myMainCamera = Camera.main; // Camera.main is expensive ; cache it here
    }

    private void Update()
    {
        Select();
        if (selected)
        {
            if (!zCollider)
            {
                zCollider = gameObject.AddComponent<CapsuleCollider>();
                zCollider.isTrigger = true;
                zCollider.direction = 2;
                zCollider.height = 2;
                zCollider.radius = 0.1f;
                //zCollider.height = 
            }

            if (!xCollider)
            {
                xCollider = gameObject.AddComponent<CapsuleCollider>();
                xCollider.isTrigger = true;
                xCollider.direction = 0;
                xCollider.height = 2;
                xCollider.radius = 0.1f;
            }

            if (!yCollider)
            {
                yCollider = gameObject.AddComponent<CapsuleCollider>();
                yCollider.isTrigger = true;
                yCollider.height = 2;
                yCollider.radius = 0.1f;
            }
        }
    }

    private void Select()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragPlane = new Plane(Vector3.down, transform.position);
            Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

            float planeDist;
            if (Physics.Raycast(camRay, out var hitInfo))
            {
                if (hitInfo.collider.gameObject != gameObject)
                {
                    selected = false;
                    Destroy(zCollider);
                    Destroy(xCollider);
                    Destroy(yCollider);
                }
            }
            else
            {
                selected = false;
            }
        }
    }

    void OnMouseDown()
    {

        dragPlane = new Plane(Vector3.down, transform.position);
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(camRay, out var hitInfo);

        if (hitInfo.collider == zCollider)
        {
            dragZ = true;
        }
        else if (hitInfo.collider == xCollider)
        {
            dragX = true;
        }
        else if (hitInfo.collider == yCollider)
        {
            dragY = true;
        }


        float planeDist;
        dragPlane.Raycast(camRay, out planeDist);
        offset = transform.position - hitInfo.point;
        selected = true;
    }

    private void OnMouseUp()
    {
        dragX = false;
        dragY = false;
        dragZ = false;
    }

    void OnMouseDrag()
    {
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

        float planeDist;

        if (dragZ)
        {
            dragPlane = new Plane(transform.right, transform.position);

            dragPlane.Raycast(camRay, out planeDist);
            transform.position = new Vector3(transform.position.x, transform.position.y,
                camRay.GetPoint(planeDist).z + offset.z);
        }

        if (dragX)
        {
            dragPlane = new Plane(transform.forward, transform.position);

            dragPlane.Raycast(camRay, out planeDist);
            transform.position = new Vector3(camRay.GetPoint(planeDist).x + offset.x, transform.position.y,
                transform.position.z);
        }

        if (dragY)
        {
            //dragPlane = new Plane(myMainCamera.transform.forward, transform.position);
            dragPlane = new Plane(transform.forward + transform.right, transform.position);
            dragPlane.Raycast(camRay, out planeDist);
            transform.position = new Vector3(transform.position.x, camRay.GetPoint(planeDist).y + offset.y,
                transform.position.z);
        }
        
    }

    // When added to an object, draws colored rays from the
    // transform position.


    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int) UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }


    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if (selected)
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            DrawArrows();
        }
    }

    void DrawArrows()
    {
        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);


        // Y vector
        GL.Color(Color.green);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0, 1, 0);
        GL.Vertex3(0, 1, 0);
        GL.Vertex3(-0.1f, 0.9f, 0);
        GL.Vertex3(0, 1, 0);
        GL.Vertex3(0.1f, 0.9f, 0);

        // X vector
        GL.Color(Color.red);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(1, 0, 0);
        GL.Vertex3(1, 0, 0);
        GL.Vertex3(0.9f, 0, 0.1f);
        GL.Vertex3(1, 0, 0);
        GL.Vertex3(0.9f, 0, -0.1f);

        // Z vector
        GL.Color(Color.blue);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0, 0, 1);
        GL.Vertex3(0, 0, 1);
        GL.Vertex3(0.1f, 0, 0.9f);
        GL.Vertex3(0, 0, 1);
        GL.Vertex3(-0.1f, 0, 0.9f);
        GL.End();
        GL.PopMatrix();
    }
}