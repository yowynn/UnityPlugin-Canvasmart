using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Canvasmart.Editor
{
    public class CanvasmartEditorWindow : EditorWindow
    {
        [MenuItem("Window/UIElements/CanvasmartEditorWindow")]
        public static CanvasmartEditorWindow ShowWindow()
        {
            CanvasmartEditorWindow wnd = GetWindow<CanvasmartEditorWindow>();
            wnd.titleContent = new GUIContent("Canvasmart");
            return wnd;
        }

        public static CanvasmartEditorWindow ShowWindow(Canvasmart layouter)
        {
            CanvasmartEditorWindow wnd = ShowWindow();
            wnd.SetCurrLayouter(layouter);
            return wnd;
        }

        private Canvasmart layouter;
        private GameObjectCell Root;

        private VisualElement HierarchyPanel;

        public void SetCurrLayouter(Canvasmart layouter)
        {
            this.layouter = layouter;
            HierarchyPanel.Clear();
            if (layouter != null){
                var go = layouter.gameObject;
                Root = new GameObjectCell(layouter, go);
                HierarchyPanel.Add(Root);
            }
            else
            {
                Root = null;
            }
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var ToolBar = new Toolbar();
            ToolBar.name = "ToolBar";
            root.Add(ToolBar);

            var Mode = new ToolbarMenu();
            Mode.name = "Mode";
            var mUniformExpand = new LayoutModeUniformExpand();
            Mode.menu.AppendAction(mUniformExpand.Name, action =>
            {
                layouter.SetLayoutMode(new LayoutModeUniformExpand());
            });
            var mCrossLine = new LayoutModeCrossLine();
            Mode.menu.AppendAction(mCrossLine.Name, action =>
            {
                layouter.SetLayoutMode(new LayoutModeCrossLine());
            });
            ToolBar.Add(Mode);

            var Run = new ToolbarButton();
            Run.text = "自动布局全部";
            Run.clicked += () => layouter.ToAll(go => layouter.SetBehaviour(go, layouter.InferAutoBehaviour(go)), true);
            ToolBar.Add(Run);

            var Content = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            root.Add(Content);

            HierarchyPanel = new VisualElement();
            Content.Add(HierarchyPanel);
        }

        public void OnGUI()
        {
            if (Root != null)
            {
                Root.UpdateCell(true);
            }
            (rootVisualElement.Q("ToolBar").Q("Mode") as ToolbarMenu).text = layouter.GetLayoutMode().Name;
        }

        public void OnHierarchyChange()
        {
            if (Root != null)
            {
                Root.ResetChildList(true);
            }
        }

        public void OnInspectorUpdate()
        {
            OnGUI();
        }
    }
}
