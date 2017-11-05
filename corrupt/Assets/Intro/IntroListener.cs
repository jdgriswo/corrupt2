using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class IntroListener : MonoBehaviour
{
    public Text textOne;
    public Text textTwo;

    public int flickerTime;

    private bool canAdvance = false;
	// Use this for initialization
	void Start ()
    {
        StartCoroutine(PromptFlicker());
        //playerID = DataConstants.headFolder.GetFolder("ProgramFiles").GetFileRep("unit_id");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Submit") && !canAdvance)
        {
            //Debug.Log("!" + File.ReadAllText(DataConstants.headFolder.GetFolder("ProgramFiles").GetFileRep("unit_id").file.Name));
            //Debug.Log("pressed");
            DataConstants.headFolder.UpdateFiles();
            //playerID = DataConstants.headFolder.GetFolder("ProgramFiles").GetFileRep("unit_id");

            foreach (FileVariable var in DataConstants.headFolder.GetFolder("ProgramFiles").GetFileRep("unit_id").currentVariables)
            {
                Debug.Log(var.varName + " " + var.varVal);

                if (var.varName == "unit_name")
                {
                    //textOne.text = var.varName + " " + var.varVal;
                    if (var.varVal.Trim() != "")
                    {
                        //Debug.Log(var.varVal.Trim() + ".");
                        if (var.varVal.Length > 16) //Don't let the player have the Bee Movie script as their name
                        {
                            textOne.text = "ERROR" + "\n" + "\n" + "OVERFLOW OF VARIABLE unit_id IN FILE unit_id";
                            break;
                        }
                        else
                        {
                            textOne.text = "ERROR RESOLVED";
                            StopCoroutine(PromptFlicker());
                            textTwo.text = "PRESS b TO BEGIN BOOT";
                            StartCoroutine(PromptFlicker());
                            canAdvance = true;
                        }
                    }
                }
            }
        }

        if (Input.GetButtonDown("Boot") && canAdvance) SceneManager.LoadScene(SceneIndexes.Main());
	}

    private IEnumerator PromptFlicker()
    {
        while(!canAdvance)
        {
            textTwo.color = new Color(textTwo.color.r, textTwo.color.g, textTwo.color.b, 1);

            yield return new WaitForSeconds(flickerTime);

            textTwo.color = new Color(textTwo.color.r, textTwo.color.g, textTwo.color.b, 0);

            yield return new WaitForSeconds(flickerTime);
        }
    }
}
