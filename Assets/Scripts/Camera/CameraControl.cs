using FFT;
using Mesh_Gen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


//use old input system
//e and q to rotate?
//wasd to move around
//something to go up and down
//g to generate it
//maybe have the option to rotate


public class CameraControl : MonoBehaviour
{
    Camera Cam;
    public GameObject MG;
    float SpeedMultiplier = 1.5f;
    float RotateSpeed = 30.0f;

    bool FFTOn = false;



    // Start is called before the first frame update
    void Start()
    {
        Cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            ResetScene();
        }

        if (Input.GetKeyDown("f"))
        {
            SwitchTerrain();
        }

        if (Input.GetKeyDown("g"))
        {
            GenerateTerrain();
        }


        if (Input.GetKey("left shift"))
        {
            SpeedMultiplier = 3.0f;
        }
        else
        {
            SpeedMultiplier = 1.5f;
        }


        Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Elevation"), Input.GetAxis("Vertical")) * Time.deltaTime * SpeedMultiplier;
        transform.Translate(Movement);


        //transform.Rotate(new Vector3(Input.GetAxis("HorizontalRotation"),Input.GetAxis("VerticalRotation"),0));
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + Input.GetAxis("VerticalRotation") * -RotateSpeed * Time.deltaTime,
            transform.localEulerAngles.y + Input.GetAxis("HorizontalRotation") * RotateSpeed * Time.deltaTime, 0);
    }


    public void GenerateTerrain()
    {
        print(FFTOn);
        if (FFTOn)
        {
            MG.GetComponent<TerrainGenerator>().GenerateTerrain(); 
        }
        else
        {
            MG.GetComponent<MidpointDisplacement>().GenerateMidpointDisplacement();
        }
    }

    public void SwitchTerrain()
    {
        FFTOn = !FFTOn;

        print(FFTOn);
    }


    private void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
