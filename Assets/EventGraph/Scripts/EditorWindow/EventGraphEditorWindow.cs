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

    const string SAVE_TYPE_KEY = "EventGraphSaveType";
    const string LOAD_TYPE_KEY = "EventGraphLoadType";
    private const string DEFAULT_FILE_NAME = "NewEventGraph";

    private EventGraphView _graphView;
    private string _fileName = DEFAULT_FILE_NAME;

    private EnumFlagsField _saveTypeFlagsField;
    public DataOperation saveTypeFlags => (DataOperation)_saveTypeFlagsField.value;

    private Toolbar _saveBar;
    private PopupField<string> _jsonFilesPopup;
    private Label _noFilesLabel;
    private TextField _fileNameTextField;
    private EnumField _loadTypeField;

    [MenuItem("Window/Event Graph")]
    static void Init()
    {
        var window = GetWindow<EventGraphEditorWindow>();
        window.titleContent = new GUIContent("Event Graph");
    }

    private void OnEnable()
    {
        CreateGraphView();
        CreateToolbars();
    }

    public void CreateToolbars()
    {
        CreateToolbar();
        CreateSaveBar();
    }

    void CreateSaveBar()
    {
        _saveBar = new Toolbar();

        // save button
        Button saveButton = EventGraphEditorUtils.CreateButton("Save", () =>
        {
            EventGraphSaver.Save(_graphView, _fileName);
        });
        _saveBar.Add(saveButton);

        // save type flags dropdown
        _saveTypeFlagsField = new EnumFlagsField(DataOperation.JSON);
        _saveTypeFlagsField.RegisterValueChangedCallback(evt =>
        {
            PlayerPrefs.SetInt(SAVE_TYPE_KEY, (int)(SaveType)evt.newValue);
            PlayerPrefs.Save();
        });
        _saveTypeFlagsField.value = (SaveType)PlayerPrefs.GetInt(SAVE_TYPE_KEY);
        _saveBar.Add(_saveTypeFlagsField);

        _saveBar.Add(new Label("   "));

        // load button
        Button loadButton = EventGraphEditorUtils.CreateButton("Load", () =>
        {
            switch ((DataOperation)_loadTypeField.value)
            {
                case DataOperation.JSON:
                    EventGraphSaver.LoadFromJson(_graphView, _jsonFilesPopup.value.RemoveString(".json"));
                    break;

                case DataOperation.ScriptableObject:
                    EventGraphSaver.LoadFromSo(_graphView, _jsonFilesPopup.value);
                    break;
            }

            

            _fileName = _jsonFilesPopup.value.RemoveString(".json");
            _fileNameTextField.value = _fileName;
        });
        _saveBar.Add(loadButton);

        SetupLoadTypeField();

        // TODO only show if loadTypeField is JSON
        //PopulateJSONDropdown();

        rootVisualElement.Add(_saveBar);
    }

    void SetupLoadTypeField()
    {
        _loadTypeField = new EnumField(DataOperation.JSON);

        _loadTypeField.RegisterValueChangedCallback(evt =>
        {
            PlayerPrefs.SetInt(LOAD_TYPE_KEY, (int)(DataOperation)evt.newValue);
            PlayerPrefs.Save();
            HandleLoadTypeChanged();
        });

        _loadTypeField.value = (DataOperation)PlayerPrefs.GetInt(LOAD_TYPE_KEY, (int)DataOperation.JSON);

        _saveBar.Add(_loadTypeField);

        // appear after DataOperation dropdown
        HandleLoadTypeChanged();
    }

    void HandleLoadTypeChanged()
    {
        RemoveJsonPopup();
        RemoveSoPopup();

        switch ((DataOperation)_loadTypeField.value)
        {
            case DataOperation.JSON:
                PopulateJsonPopup();
                break;

            case DataOperation.ScriptableObject:
                PopulateSoPopup();
                break;

            case DataOperation.SQLite:

                break;
        }
    }



    void RemoveSoPopup()
    {

    }

    void PopulateSoPopup()
    {
        var list = EventGraphEditorUtils.FindScriptableObjects<EventGraphDataObject>();
        if (list.Count == 0)
        {
            _noFilesLabel = new Label("No files found.");
            _saveBar.Add(_noFilesLabel);
        }
        else
        {
            List<string> concatFiles = new List<string>();
            foreach (var item in list)
            {
                concatFiles.Add(item.name);
            }

            _jsonFilesPopup = new PopupField<string>(concatFiles, 0);
            _saveBar.Add(_jsonFilesPopup);

        }
    }



    void RemoveJsonPopup()
    {
        if (_jsonFilesPopup != null && _saveBar.Contains(_jsonFilesPopup))
            _saveBar.Remove(_jsonFilesPopup);
        if (_noFilesLabel != null && _saveBar.Contains(_jsonFilesPopup))
            _saveBar.Remove(_noFilesLabel);
    }

    public void PopulateJsonPopup()
    {
        // load files dropdown list
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
            _noFilesLabel = new Label("No files found.");
            _saveBar.Add(_noFilesLabel);
        }
        else
        {
            _jsonFilesPopup = new PopupField<string>(concatFiles, 0);
            _saveBar.Add(_jsonFilesPopup);
        }
    }

    void CreateToolbar()
    {
        Toolbar toolbar = new Toolbar();

        // file name text field
        _fileNameTextField = EventGraphEditorUtils.CreateTextField(
            DEFAULT_FILE_NAME, "Filename:", (evt) => { _fileName = evt.newValue; });
        toolbar.Add(_fileNameTextField);

        // minimap toggle
        ToolbarToggle minimapToggle = new ToolbarToggle();
        minimapToggle.label = "Minimap";
        minimapToggle.RegisterValueChangedCallback(evt =>
        {
            _graphView.ToggleMinimap(evt.newValue);
        });
        minimapToggle.value = false;
        toolbar.Add(minimapToggle);

        // add the toolbar to editor window
        rootVisualElement.Add(toolbar);
    }

    void CreateGraphView()
    {
        _graphView = new EventGraphView(this);
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }


}

[Flags]
public enum DataOperation
{
    JSON = 1,
    ScriptableObject = 2,
    SQLite = 4,
}
