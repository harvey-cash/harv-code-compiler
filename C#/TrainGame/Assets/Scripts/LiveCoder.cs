using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiveCoder : MonoBehaviour {

    InputField input;
    public Text history;

    public delegate void Command(MachineType obj, string[] parameters);
    public delegate GameObject MachineType();

    public Dictionary<string, Command> commands = new Dictionary<string, Command>();
    public Dictionary<string, MachineType> objects = new Dictionary<string, MachineType>();

    private void Awake() {
        Command cmdNew = new Command(New);
        MachineType moverType = new MachineType(Mover.New);

        commands.Add("new", cmdNew);

        objects.Add("mover", moverType);
    }

    private void New(MachineType mType, string[] parameters) {
        GameObject gameObject = mType();
        Machine machine = gameObject.GetComponent<Machine>();
        Debug.Log(machine);
        gameObject.name = parameters[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputField>();
        input.onEndEdit.AddListener(delegate {            
            history.text += "> " + input.text + "\n";
            Parse(input.text);
            input.text = "";
            input.ActivateInputField();
        });
    }

    void Parse(string line) {
        string[] words = line.Split(new char[] { ' ' });

        string command = "", subject = "";
        string[] parameters = new string[0];
        
        if (words.Length > 0) command = words[0];
        if (words.Length > 1) subject = words[1];

        if (words.Length > 2) {
            parameters = new string[words.Length - 2];
            Array.Copy(words, 2, parameters, 0, words.Length - 2);
        }

        bool exists = commands.TryGetValue(command, out Command method);
        bool objExists = objects.TryGetValue(subject, out MachineType mType);
        if (exists && objExists) {
            method(mType, parameters);
        }
        else {
            history.text += "\"" + command + "\" is undefined for object type \"" + subject + "\".\n";
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
