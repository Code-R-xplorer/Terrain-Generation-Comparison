using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraButtons : MonoBehaviour
{
    public GameObject InstructionBlock;
    public GameObject InputBlock;
    public Button InstructionButton;
    public Button InputFieldButton;

    public GameObject quitButton;
        

    //GameObject MidpointInputBlock;
    //GameObject FFTInputBlock;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InputToggle()
    {
        //print("The input");
        InputBlock.SetActive(!InputBlock.activeSelf);

        if (!InputBlock.activeSelf)
        {
            InputFieldButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Show input field");
        }
        else
        {
            InputFieldButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Hide input field");
            
        }
    }

    public void InstructionToggle()
    {
        InstructionBlock.SetActive(!InstructionBlock.activeSelf);
        if (!InstructionBlock.activeSelf)
        {
            InstructionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Show instructions");
            quitButton.SetActive(false);
        }
        else
        {
            InstructionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Hide instructions");
            quitButton.SetActive(true);

        }
    }
    
    public void Quit()
    {
        Application.Quit();
    }

}
