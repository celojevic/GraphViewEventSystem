using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System.Collections.Generic;
using System;

public class EventGraphEditorWindow : GraphViewEditorWindow
{

    private EventGraphView _graphView;

    private const string DEFAULT_FILE_NAME = "NewEventGraph";
    private string _fileName = DEFAULT_FILE_NAME;

    [MenuItem("Window/Event Graph")]
    static void Init()
    {
        var window = GetWindow<EventGraphEditorWindow>();
        window.titleContent = new GUIContent("Event Graph");
    }

    private void OnEnable()
    {
        CreateGraphView();
        CreateToolbar();
    }

    void CreateToolbar()
    {
        Toolbar toolbar = new Toolbar();

        TextField fileName = EventGraphEditorUtils.CreateTextField(DEFAULT_FILE_NAME, "Filename:", (evt) => { _fileName = evt.newValue; });
        toolbar.Add(fileName);

        Button saveButton = EventGraphEditorUtils.CreateButton("Save", ()=> 
        {
            EventGraphSaver.Save(_graphView, _fileName);
        });
        toolbar.Add(saveButton);

        EnumFlagsField saveTypePopup = new EnumFlagsField(SaveType.JSON);
        // TODO callback for type and save as selected type when implemented
        toolbar.Add(saveTypePopup);

        Button loadButton = EventGraphEditorUtils.CreateButton("Load", () =>
        {
            EventGraphSaver.Load(_graphView, _fileName);
        });
        toolbar.Add(loadButton);

        string savePath = $"{Application.persistentDataPath}/EventGraphs";
        string[] files = Directory.GetFiles(savePath);
        List<string> concatFiles = new List<string>();
        foreach (var item in files)
        {
            // +1 to remove last '/'
            concatFiles.Add(item.Remove(0, savePath.Length + 1));
        }
        if (files.Length == 0)
        {
            Label label = new Label("No files found.");
            toolbar.Add(label);
        }
        else
        {
            PopupField<string> filesPopup = new PopupField<string>(concatFiles, 0);
            // TODO callback for index and load that file
            // TODO change fileName to be selected file
            toolbar.Add(filesPopup);
        }

        rootVisualElement.Add(toolbar);
    }

    void CreateGraphView()
    {
        _graphView = new EventGraphView(this);
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    [Flags]
    public enum SaveType
    {
        JSON=1,
        ScriptableObject=2,
        SQLite=4,
    }

}
