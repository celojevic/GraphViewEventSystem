using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Collections.Generic;
using System;
using EventGraph.Editor;

#if UNITY_EDITOR

using UnityEditor.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace EventGraph
{
    public class EventGraphEditorWindow : GraphViewEditorWindow
    {

        private const string SAVE_TYPE_KEY = "EventGraphSaveType";
        private const string LOAD_TYPE_KEY = "EventGraphLoadType";
        private const string DEFAULT_FILE_NAME = "NewEventGraph";

        private EventGraphView _graphView;
        private string _fileName = DEFAULT_FILE_NAME;

        private EnumFlagsField _saveTypeFlagsField;
        public DataType saveTypeFlags => (DataType)_saveTypeFlagsField.value;

        private Toolbar _saveBar;
        private PopupField<string> _fileListPopup;
        private Label _noFilesLabel;
        private TextField _fileNameTextField;
        private EnumField _loadTypeField;

        private string _fileListCurSelection;

        [MenuItem("Window/Event Graph")]
        private static void Init()
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

        private void CreateSaveBar()
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
                        if (_fileListPopup == null)
                        {
                            Debug.LogWarning("There are no saved JSON files to load.");
                            return;
                        }
                        EventGraphSaver.LoadFromJson(_graphView, _fileListPopup.value.RemoveString(".json"));
                        break;

                    case DataType.ScriptableObject:
                        EventGraphSaver.LoadFromSo(_graphView, _fileListPopup.value);
                        break;
                }

                _fileName = _fileListPopup.value.RemoveString(".json");
                _fileNameTextField.value = _fileName;
            });
            _saveBar.Add(loadButton);

            SetupLoadTypeField();

            rootVisualElement.Add(_saveBar);
        }

        private void SetupLoadTypeField()
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

        internal void HandleLoadTypeChanged()
        {
            RemoveFileListPopup();
            _fileListCurSelection = "";

            switch ((DataType)_loadTypeField.value)
            {
                case DataType.JSON:
                    PopulateJsonPopup();
                    break;

                case DataType.ScriptableObject:
                    PopulateSoPopup();
                    break;

                case DataType.SQLite:
                    // TODO
                    break;
            }
        }

        private void RemoveFileListPopup()
        {
            if (_fileListPopup != null && _saveBar.Contains(_fileListPopup))
                _saveBar.Remove(_fileListPopup);
            if (_noFilesLabel != null && _saveBar.Contains(_noFilesLabel))
                _saveBar.Remove(_noFilesLabel);
        }

        private void PopulateSoPopup()
        {
            RemoveFileListPopup();

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
                    concatFiles.Add(item.name);

                // create file list
                _fileListPopup = new PopupField<string>(concatFiles, 0);
                _fileListPopup.RegisterValueChangedCallback((evt) => 
                { 
                    _fileListCurSelection = evt.newValue; 
                    //Debug.Log(_fileListCurSelection); 
                });
                // TODO this will not switch back to selection after saving...
                if (!string.IsNullOrEmpty(_fileListCurSelection))
                    _fileListPopup.index = concatFiles.IndexOf(_fileListCurSelection);

                _saveBar.Add(_fileListPopup);
            }
        }

        public void PopulateJsonPopup()
        {
            RemoveFileListPopup();

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
                _fileListPopup = new PopupField<string>(concatFiles, 0);
                _saveBar.Add(_fileListPopup);
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

    [Flags]
    public enum DataType
    {
        JSON = 1,
        ScriptableObject = 2,
        SQLite = 4,
    }

}

#endif
