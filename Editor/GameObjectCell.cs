using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Canvasmart.Editor
{
    public class GameObjectCell : VisualElement
    {
        public static string[] Options = {
                "Custom",
                "Auto",
                "Auto (↕)",
                "Auto (↔)",
                "Auto (↔↕)",
            };
        private Canvasmart layouter;
        private GameObject gameObject;
        private LayoutBehaviour b;
        private bool isFold;
        private bool isIgnore;
        private bool isRoot;

        private VisualElement ChildList;


        public GameObjectCell(Canvasmart layouter, GameObject gameObject)
        {
            this.layouter = layouter;
            this.gameObject = gameObject;
            this.b = layouter.InferBehaviour(gameObject);
            this.isFold = false;
            this.isIgnore = layouter.IsIgnore(gameObject);
            this.isRoot = layouter.gameObject == gameObject;
            InitCell();
        }

        public void InitCell()
        {
            var Self = new VisualElement();
            Self.name = "Self";
            Self.style.flexDirection = FlexDirection.Row;
            Add(Self);

            var Fold = new Foldout();
            Fold.name = "Fold";
            Fold.value = !isFold;
            Fold.RegisterValueChangedCallback(evt =>
            {
                isFold = !evt.newValue;
                UpdateCell();
            });
            Self.Add(Fold);

            var Unlock = new Toggle();
            Unlock.value = !isIgnore;
            Unlock.RegisterValueChangedCallback(evt =>
            {
                isIgnore = !evt.newValue;
                bool lastIngore = layouter.IsIgnore(gameObject);
                if (lastIngore && !isIgnore)
                    layouter.RemoveFromIgnoredList(gameObject);
                else if (!lastIngore && isIgnore)
                    layouter.AddToIgnoredList(gameObject);
                UpdateCell();
            });
            Self.Add(Unlock);

            var Option = new ToolbarMenu();
            Option.name = "Option";
            Option.style.width = 110;
            Option.style.marginTop = 1;
            Option.style.marginBottom = 1;
            Option.style.marginRight = 0;
            Option.style.borderRightWidth = 0;
            Option.style.paddingBottom = 1;
            Option.style.paddingRight = 10;
            Option.style.unityFontStyleAndWeight = FontStyle.Bold;
            foreach (string op in Options)
                Option.menu.AppendAction(op, SetBehaviourOption);
            Self.Add(Option);

            var Name = new Label(gameObject.name);
            Name.name = "Name";
            Name.style.color = new Color(0.7843137f, 0.7843137f, 0.3921569f);
            Name.style.height = 17;
            Name.style.unityFontStyleAndWeight = FontStyle.Bold;
            Name.style.marginTop = 1;
            Name.style.borderTopWidth = 2;
            Name.style.borderLeftWidth = 0;
            Name.style.borderRightWidth = 6;
            Name.style.borderBottomWidth = 2;
            Name.style.paddingLeft = 5;
            Name.style.paddingRight = 5;
            Name.RegisterCallback<MouseDownEvent>(evt =>
            {
                Selection.activeGameObject = gameObject;
            });
            Self.Add(Name);

            var Children = new VisualElement();
            Children.name = "Children";
            Children.style.marginLeft = 18;
            Add(Children);

            ChildList = new VisualElement();
            ResetChildList(true);
            Children.Add(ChildList);

            UpdateCell();
        }

        public void UpdateCell(bool onChildren = false)
        {
            b = layouter.InferBehaviour(gameObject);
            var Self = this.Q("Self");
            var Option = Self.Q("Option") as ToolbarMenu;
            Option.text = GetBehaviourOption();

            UpdateColor();

            var Children = this.Q("Children");
            if (isFold && Children.Contains(ChildList))
                Children.Remove(ChildList);
            else
            {
                if (!Children.Contains(ChildList))
                {
                    ResetChildList(true);
                    Children.Add(ChildList);
                }
                if (onChildren)
                {
                    for (int i = 0; i < ChildList.childCount; i++)
                    {
                        var child = ChildList[i] as GameObjectCell;
                        child.UpdateCell(true);
                    }
                }
            }
        }

        public void UpdateColor()
        {
            Color color = Color.clear;
            if (isIgnore || !layouter.IsLegal(gameObject))
            {
                color = new Color(.176f, .169f, .082f);
            }
            else
            {
                switch (b)
                {
                    case LayoutBehaviour.Custom:
                        {
                            color = new Color(.16f, .125f, .263f);
                            break;
                        }
                    case LayoutBehaviour.Normal:
                        {
                            color = new Color(.169f, .361f, .0f);
                            break;
                        }
                    case LayoutBehaviour.StretchVertical:
                        {
                            color = new Color(.527f, .388f, .0f);
                            break;
                        }
                    case LayoutBehaviour.StretchHorizontal:
                        {
                            color = new Color(.404f, .247f, .0f);
                            break;
                        }
                    case LayoutBehaviour.StretchAll:
                        {
                            color = new Color(.357f, .0f, .114f);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            var Self = this.Q("Self");

            var Option = Self.Q("Option");
            Option.style.backgroundColor = color;

            var Name = Self.Q("Name");
            Name.style.borderTopColor = color;
            Name.style.borderLeftColor = color;
            Name.style.borderRightColor = color;
            Name.style.borderBottomColor = color;
        }

        public void ResetChildList(bool onChildren = false)
        {
            if (isFold)
                return;
            var map = new Dictionary<GameObject, GameObjectCell>();
            for (int i = 0; i < ChildList.childCount; i++)
            {
                var child = ChildList[i] as GameObjectCell;
                map.Add(child.gameObject, child);
            }
            ChildList.Clear();
            var Fold = this.Q("Self").Q("Fold");
            Transform t = gameObject.transform;
            int legelChildCount = 0;
            for (int i = 0; i < t.childCount; i++)
            {
                var go = t.GetChild(i).gameObject;
                if (layouter.IsLegal(go))
                {
                    GameObjectCell child;
                    if (!map.TryGetValue(go, out child))
                        child = new GameObjectCell(layouter, go);
                    else if (onChildren)
                        child.ResetChildList(true);
                    ChildList.Add(child);
                    legelChildCount++;
                }
            }
            Fold.visible = legelChildCount > 0;
        }

        public string GetBehaviourOption()
        {
            string op = "——";
            switch (b)
            {
                case LayoutBehaviour.Custom:
                    {
                        op = Options[0];
                        break;
                    }
                case LayoutBehaviour.Normal:
                    {
                        op = Options[1];
                        break;
                    }
                case LayoutBehaviour.StretchVertical:
                    {
                        op = Options[2];
                        break;
                    }
                case LayoutBehaviour.StretchHorizontal:
                    {
                        op = Options[3];
                        break;
                    }
                case LayoutBehaviour.StretchAll:
                    {
                        op = Options[4];
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return op;
        }

        public void SetBehaviourOption(DropdownMenuAction action)
        {
            string name = action.name;
            LayoutBehaviour behaviour = LayoutBehaviour.Illegal;
            if (name == Options[0])
                behaviour = LayoutBehaviour.Custom;
            else if (name == Options[1])
                behaviour = LayoutBehaviour.Normal;
            else if (name == Options[2])
                behaviour = LayoutBehaviour.StretchVertical;
            else if (name == Options[3])
                behaviour = LayoutBehaviour.StretchHorizontal;
            else if (name == Options[4])
                behaviour = LayoutBehaviour.StretchAll;
            b = layouter.SetBehaviour(gameObject, behaviour);
            UpdateCell();
        }
    }
}
