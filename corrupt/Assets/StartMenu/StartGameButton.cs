using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class StartGameButton : MonoBehaviour
{
    public Button thisButton;

    public TextAsset PersonalityFilePlayer;
    public TextAsset ID;
    public TextAsset sensors;
    public TextAsset motors;

	// Use this for initialization
	void Start ()
    {
        thisButton.onClick.AddListener(OnClickStart);
	}

    private void OnClickStart()
    {
        //DirectoryInfo mainFolder = Directory.CreateDirectory("YourData");
        //FileStream testFile = File.Create("YourData/testFile.txt");
        //testFile.Dispose();
        //File.WriteAllText("YourData/testFile.txt", testAsset.ToString());
        Setup();
        SceneManager.LoadScene(SceneIndexes.Intro());
    }

    private void Setup()
    {
        DataConstants.headFolder = new DataFolder("Data");
        DataFolder head = DataConstants.headFolder;
        head.AddFolder("ProgramFiles");
        head.AddFolder("ExternalFiles");
        DataFolder playerPrograms = head.GetFolder("ProgramFiles");
        //playerPrograms.AddFile(PersonalityFilePlayer);
        playerPrograms.AddFile(ID);
        playerPrograms.AddFile(sensors);
        playerPrograms.AddFile(motors);
    }
}

public class DataFolder
{
    public string name;
    public string filePath;

    public List<DataFolder> childFolders = new List<DataFolder>();
    public List<FilePlusBackup> childFiles = new List<FilePlusBackup>();

    public DataFolder parentFolder = null;

    public DataFolder(string _name, DataFolder _parentFolder = null)
    {
        name = _name;
        if (_parentFolder != null)
        {
            parentFolder = _parentFolder;
            filePath = parentFolder.filePath + "/" + name;
        }
        else filePath = name;

        Directory.CreateDirectory(filePath);
    }

    public void AddFolder(string _name)
    {
        childFolders.Add(new DataFolder(_name, this));
    }

    public void AddFile(TextAsset file)
    {
        FileStream addedFile = File.Create(filePath + "/" + file.name + ".txt");
        addedFile.Dispose();
        File.WriteAllText(addedFile.Name, file.ToString());

        childFiles.Add(new FilePlusBackup(file, addedFile));
    }

    public DataFolder GetFolder(string _name)
    {
        foreach(DataFolder child in childFolders)
        {
            if (child.name == _name) return child;
        }
        return null;
    }

    public FilePlusBackup GetFileRep(string _name)
    {
        foreach (FilePlusBackup child in childFiles)
        {
            if (child.asset.name == _name) return child;
        }

        return new FilePlusBackup(null, null);
    }

    public bool UpdateFiles() //Updates all files in the folder and has all children do the same
    {
        bool isInvalidated = false;

        //Debug.Log("updating");

        foreach(DataFolder child in childFolders)
        {
            child.UpdateFiles();
        }

        int index = 0;

        foreach(FilePlusBackup fileSet in childFiles)
        {
            fileSet.ParseFile(); //Update the files

            childFiles[index] = fileSet;
            index++;

            if (fileSet.currentVariables == null) isInvalidated = true;
        }

        return isInvalidated;
    }
}

public struct FilePlusBackup
{
    public TextAsset asset;
    public FileStream file;
    public List<FileVariable> baseVariables;
    public List<FileVariable> currentVariables;

    public FilePlusBackup(TextAsset _asset, FileStream _file)
    {
        asset = _asset;
        file = _file;
        baseVariables = new List<FileVariable>();
        currentVariables = new List<FileVariable>();
        ParseFile(); //Set all currentVariables
        baseVariables = currentVariables; //Keep a copy of original variables
    }


    public void ParseFile() //Updates/sets currentVariables
    {
        currentVariables = new List<FileVariable>();

        string fileString = File.ReadAllText(file.Name);

        Debug.Log(fileString);

        IEnumerator<char> stringEnum = fileString.GetEnumerator();
        stringEnum.MoveNext();

        //Debug.Log(DataConstants.headFolder.GetFileRep("ProgramFiles").currentVariables[0].varVal);
        while(ReadLine(stringEnum))
        {
            //Debug.Log(currentVariables[currentVariables.Count - 1].varName + ":" + currentVariables[currentVariables.Count - 1].varVal);
        }
    }

    //Reads a single line of the string
    //Returning true means there's a next string to read, false means there isn't
    private bool ReadLine(IEnumerator<char> stringEnum)
    {
        string name = "";
        string value = "";

        while(stringEnum.Current != ':') //Read variable name
        {
            if (stringEnum.Current == '\n') //Not valid file
            {
                currentVariables = null;
                return false;
            }

            name += stringEnum.Current;

            if (!stringEnum.MoveNext()) //Advance; if statement is true, not valid file
            {
                currentVariables = null;
                return false;
            }
        }

        if (!stringEnum.MoveNext()) //Move beyond colon
        {
            currentVariables.Add(new FileVariable(name, value));
            //Debug.Log(name + " " + value);
            return false; //valid line with no value for last unit, and done
        }

        //while(stringEnum.Current == ' ') //Skip spaces
        //{
        //    if (!stringEnum.MoveNext())
        //    {
        //        name.Trim();
        //        value.Trim();
        //        currentVariables.Add(new FileVariable(name, value));
        //        //Debug.Log(name + " " + value);
        //        return false; //valid line with no value for last unit, and done
        //    }
        //}

        if (stringEnum.Current == '\n') //See if there's no value for this line
        {
            currentVariables.Add(new FileVariable(name, value));
            //Debug.Log(name + " " + value);
            return true; //no value for this line, there is another line
        }

        while (stringEnum.Current != '\n') //Read value name
        {
            value += stringEnum.Current;

            if (!stringEnum.MoveNext())
            {
                currentVariables.Add(new FileVariable(name, value));
                //Debug.Log(name + " " + value);
                return false;
            }
        }

        if (!stringEnum.MoveNext())
        {
            currentVariables.Add(new FileVariable(name, value));
            //Debug.Log(name + " " + value);
            return false; //valid line and done
        }

        currentVariables.Add(new FileVariable(name, value));
        //Debug.Log(name + " " + value);
        return true;
    }
}

public struct FileVariable
{
    public string varName;
    public string varVal;

    public FileVariable(string name, string value)
    {
        varName = name;
        varVal = value;
    }
}

