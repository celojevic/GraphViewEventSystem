using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

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

        TextField fileName = EventGraphEditorUtils.CreateTextField(DEFAULT_FILE_NAME, "Filename:",
            (evt) => 
            {
                _fileName = evt.newValue;
            }
        );
        toolbar.Add(fileName);

        Button saveButton = EventGraphEditorUtils.CreateButton("Save", ()=> 
        {
            EventGraphSaver.Save(_graphView, _fileName);
        });
        toolbar.Add(saveButton);

        Button loadButton = EventGraphEditorUtils.CreateButton("Load", () =>
        {
            EventGraphSaver.Load(_graphView);
        });
        toolbar.Add(loadButton);

        rootVisualElement.Add(toolbar);
    }

    void CreateGraphView()
    {
        _graphView = new EventGraphView(this);
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }



}
