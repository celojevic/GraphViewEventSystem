using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System.Collections.Generic;
using System;
using EventGraph.Editor;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace EventGraph
{
    public class EventGraphEditorWindow : GraphViewEditorWindow
    {

        const string SAVE_TYPE_KEY = "EventGraphSaveType";
        const string LOAD_TYPE_KEY = "EventGraphLoadType";
        private const string DEFAULT_FILE_NAME = "NewEventGraph";

        private EventGraphView _graphView;
        private string _fileName = DEFAULT_FILE_NAME;

        private EnumFlagsField _saveTypeFlagsField;
        public DataType saveTypeFlags => (DataType)_saveTypeFlagsField.value;

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
            _saveTypeFlagsField = new EnumFlagsField(DataType.JSON);
            _saveTypeFlagsField.RegisterValueChangedCallback(evt =>
            {
                EditorPrefs.SetInt(SAVE_TYPE_KEY, (int)(SaveType)evt.newValue);
            });
            _saveTypeFlagsField.value = (SaveType)EditorPrefs.GetInt(SAVE_TYPE_KEY);
            _saveBar.Add(_saveTypeFlagsField);

            _saveBar.Add(new Label("   "));

            // load button
            Button loadButton = EventGraphEditorUtils.CreateButton("Load", () =>
            {
                switch ((DataType)_loadTypeField.value)
                {
                    case DataType.JSON:
                        // TODO disable the load button if no files
                        if (_jsonFilesPopup == null)
                        {
                            Debug.LogWarning("There are no saved JSON files to load.");
                            return;
                        }
                        EventGraphSaver.LoadFromJson(_graphView, _jsonFilesPopup.value.RemoveString(".json"));
                        break;

                    case DataType.ScriptableObject:
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
            _loadTypeField = new EnumField(DataType.JSON);

            _loadTypeField.RegisterValueChangedCallback(evt =>
            {
                EditorPrefs.SetInt(LOAD_TYPE_KEY, (int)(DataType)evt.newValue);
                HandleLoadTypeChanged();
            });

            _loadTypeField.value = (DataType)EditorPrefs.GetInt(LOAD_TYPE_KEY, (int)DataType.JSON);

            _saveBar.Add(_loadTypeField);

            // appear after DataOperation dropdown
            HandleLoadTypeChanged();
        }

        void HandleLoadTypeChanged()
        {
            RemoveJsonPopup();
            RemoveSoPopup();

            switch ((DataType)_loadTypeField.value)
            {
                case DataType.JSON:
                    PopulateJsonPopup();
                    break;

                case DataType.ScriptableObject:
                    PopulateSoPopup();
                    break;

                case DataType.SQLite:

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
            RemoveJsonPopup();

            // load files dropdown list
            string savePath = $"{Application.persistentDataPath}/EventGraphs";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
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
            minimapToggle.label = "  Minimap";
            minimapToggle.RegisterValueChangedCallback(evt =>
            {
                _graphView.ToggleMinimap(evt.newValue);
            });
            minimapToggle.value = false;
            toolbar.Add(minimapToggle);

            // clear graph button
            Button clearGraphButton = EventGraphEditorUtils.CreateButton("Clear Graph", () =>
            {
                _graphView.ClearGraph();
            });
            toolbar.Add(clearGraphButton);

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

#endif

    [Flags]
    public enum DataType
    {
        JSON = 1,
        ScriptableObject = 2,
        SQLite = 4,
    }
}