using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

namespace Canvasmart.Editor
{
    public class CanvasmartEditorWindow : EditorWindow
    {
        [MenuItem("Window/UIElements/CanvasmartEditorWindow")]
        public static CanvasmartEditorWindow ShowWindow()
        {
            CanvasmartEditorWindow wnd = GetWindow<CanvasmartEditorWindow>();
            wnd.titleContent = new GUIContent("Canvasmart");
            wnd.SetCurrLayouter(null);
            return wnd;
        }

        public static CanvasmartEditorWindow ShowWindow(Canvasmart layouter)
        {
            CanvasmartEditorWindow wnd = ShowWindow();
            wnd.SetCurrLayouter(layouter);
            return wnd;
        }

        public List<Canvasmart> GetAllCanvasmarts()
        {
            List<Canvasmart> list = new List<Canvasmart>();
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                foreach(var go in scene.GetRootGameObjects())
                {
                    var array = go.GetComponentsInChildren<Canvasmart>(true);
                    foreach(var c in array)
                    {
                        list.Add(c);
                    }
                }
            }
            var CanvasSelector = rootVisualElement.Q("ToolBar").Q("CanvasSelector") as ToolbarMenu;
            CanvasSelector.menu.MenuItems().Clear();
            foreach (var c in list)
            {
                CanvasSelector.menu.AppendAction(c.name, action =>
                {
                    SetCurrLayouter(c);
                });
            }

            return list;
        }

        private Canvasmart layouter;
        private GameObjectCell Root;

        private VisualElement HierarchyPanel;

        public void SetCurrLayouter(Canvasmart layouter = null)
        {
            if (layouter != null)
            {
                var layouters = GetAllCanvasmarts();
                if (layouters.Contains(layouter))
                {
                    if (layouter == this.layouter)
                        return;
                }
                else
                {
                    layouter = null;
                }
            }
            if (layouter == null)
            {
                var layouters = GetAllCanvasmarts();
                if (layouters.Count > 0)
                {
                    layouter = layouters[0];
                }
            }
            this.layouter = layouter;
            HierarchyPanel.Clear();
            var Mode = rootVisualElement.Q("ToolBar").Q("Mode") as ToolbarMenu;
            if (layouter != null)
            {
                var go = layouter.gameObject;
                Root = new GameObjectCell(layouter, go);
                HierarchyPanel.Add(Root);
                Canvasmart.EnumLayoutMode(m =>
                {
                    LayoutMode mode = layouter.GetLayoutMode(m);
                    Mode.menu.AppendAction(mode.Name, action =>
                    {
                        layouter.SetLayoutMode(m);
                    });
                });
                Mode.SetEnabled(true);
            }
            else
            {
                Root = null;
                Mode.SetEnabled(false);
            }
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var ToolBar = new Toolbar();
            ToolBar.name = "ToolBar";
            root.Add(ToolBar);

            var CanvasSelector = new ToolbarMenu();
            CanvasSelector.name = "CanvasSelector";
            CanvasSelector.style.width = 250;
            ToolBar.Add(CanvasSelector);

            var Mode = new ToolbarMenu();
            Mode.name = "Mode";
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
            SetCurrLayouter(layouter);
            if (Root != null)
            {
                Root.UpdateCell(true);
            }
            var CanvasSelector = rootVisualElement.Q("ToolBar").Q("CanvasSelector") as ToolbarMenu;
            if (layouter != null)
            {
                (rootVisualElement.Q("ToolBar").Q("Mode") as ToolbarMenu).text = layouter.GetLayoutMode().Name;
                CanvasSelector.text = layouter.name;
            }
            else
            {
                CanvasSelector.text = "——";
            }

        }

        public void OnHierarchyChange()
        {
            SetCurrLayouter(layouter);
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
