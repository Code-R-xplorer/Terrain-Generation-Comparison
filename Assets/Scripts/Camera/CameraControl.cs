using FFT;
using Mesh_Gen;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public float SpeedMultiplier = 1.5f;
    public float ShiftSpeedMultiplier = 3f;
    private float _speedMultiplier;
    public float RotateSpeed = 30.0f;

    bool FFTOn = false;

    public GameObject InputBlock;

    public TMP_InputField TerrainSizeField;
    public TMP_InputField RoughnessField;


    public TMP_Dropdown TerrainScaleDropdown;

    public TMP_Dropdown NoiseTypeDrop;

    public TMP_InputField GaussianStdDev;

    public TMP_InputField WhiteNoiseScale;

    public UnityEngine.UI.Slider PowerLawFilterBar;




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
            _speedMultiplier = ShiftSpeedMultiplier;
            // SpeedMultiplier = 3.0f;
        }
        else
        {
            // SpeedMultiplier = 1.5f;
            _speedMultiplier = SpeedMultiplier;
            
        }


        Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Elevation"), Input.GetAxis("Vertical")) * Time.deltaTime * _speedMultiplier;
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
            if (int.TryParse(TerrainSizeField.text, out int Size))
            {
                //set the map depth and width here
                //print(Size);
                MG.GetComponent<TerrainGenerator>().mapSize = Size;
            }

            var size = TerrainScaleDropdown.value switch
            {
                1 => 1f,
                2 => 0.5f,
                3 => 0.25f,
                4 => 0.125f,
                5 => 0.0625f,
                6 => 0.03125f,
                
                7 => 0.015625f,
                _ => 1f
            };
            
            MG.GetComponent<TerrainGenerator>().mapScale = size;

            if (NoiseTypeDrop.value == 0)
            {
                //gaussian noise
                MG.GetComponent<TerrainGenerator>().noiseType = Utils.NoiseTypes.Gaussian;
                if (float.TryParse(GaussianStdDev.text, out float S))
                {
                    MG.GetComponent<TerrainGenerator>().stdDev = S;
                }
            }
            else
            {
                //white noise
                MG.GetComponent<TerrainGenerator>().noiseType = Utils.NoiseTypes.White;
                if (float.TryParse(WhiteNoiseScale.text, out float S))
                {
                    MG.GetComponent<TerrainGenerator>().scale = S;
                }
            }


            MG.GetComponent<TerrainGenerator>().alpha = PowerLawFilterBar.value;


            MG.GetComponent<TerrainGenerator>().GenerateTerrain();


        }
        else
        {

            if (int.TryParse(TerrainSizeField.text, out int Size))
            {
                //set the map depth and width here
                print(Size);
                MG.GetComponent<MidpointDisplacement>().MapDepth = Size;
                MG.GetComponent<MidpointDisplacement>().MapWidth = Size;
            }

            if (float.TryParse(RoughnessField.text, out float R))
            {
                MG.GetComponent<MidpointDisplacement>().Roughness = R;
                print(R);
            }
            //print("G");
            MG.GetComponent<MidpointDisplacement>().GenerateMidpointDisplacement();
        }
    }

    public void SwitchTerrain()
    {
        FFTOn = !FFTOn;

        if (FFTOn)
        {
            InputBlock.transform.GetChild(0).gameObject.SetActive(false);
            InputBlock.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {

            InputBlock.transform.GetChild(0).gameObject.SetActive(true);
            InputBlock.transform.GetChild(1).gameObject.SetActive(false);
        }

        //print(FFTOn);
    }


    private void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void UpdateSlider()
    {
        float DisplayValue = Mathf.Round(PowerLawFilterBar.value * 100) / 100;
        PowerLawFilterBar.transform.GetChild(4).GetComponent<TextMeshProUGUI>().SetText(DisplayValue.ToString());
    }

    public void UpdateDropdown()
    {
        if (NoiseTypeDrop.value == 0)
        {

            GaussianStdDev.gameObject.SetActive(true);
            WhiteNoiseScale.gameObject.SetActive(false);

        }
        else
        {

            GaussianStdDev.gameObject.SetActive(false);
            WhiteNoiseScale.gameObject.SetActive(true);
        }
    }

}
