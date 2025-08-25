using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DecisionTree
{
    public class DecisionTreeEditorWindow : EditorWindow
    {
        private DecisionTreeAsset _treeAsset;
        private Vector2 _canvasOffset;
        private float _zoomLevel = 1f;

        private readonly Dictionary<DecisionNode, NodeView> _nodeViews = new();
        private NodeView _selectedNode;
        private NodeView _connectionStart;
        private bool _isConnecting;
        private bool _connectingFromTrue;

        private GUIStyle _nodeStyle;
        private GUIStyle _selectedNodeStyle;
        private GUIStyle _questionNodeStyle;
        private GUIStyle _actionNodeStyle;
        private GUIStyle _rootNodeStyle;

        private const float NODE_WIDTH = 200f;
        private const float NODE_HEIGHT = 120f;
        private const float GRID_SIZE = 20f;

        private readonly Dictionary<DecisionNode, bool> _nodeIsAction = new();

        [MenuItem("Window/Decision Tree Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<DecisionTreeEditorWindow>("Decision Tree Editor");
            window.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            InitializeStyles();
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            if (Selection.activeObject is DecisionTreeAsset asset)
            {
                LoadTree(asset);
            }
        }

        private void InitializeStyles()
        {
            _nodeStyle = new GUIStyle
            {
                normal =
                {
                    background = CreateColorTexture(new Color(0.3f, 0.3f, 0.3f)),
                    textColor = Color.white
                },
                border = new RectOffset(12, 12, 12, 12),
                padding = new RectOffset(10, 10, 10, 10),
                alignment = TextAnchor.UpperCenter
            };

            _selectedNodeStyle = new GUIStyle(_nodeStyle)
            {
                normal =
                {
                    background = CreateColorTexture(new Color(0.4f, 0.5f, 0.6f))
                }
            };

            _questionNodeStyle = new GUIStyle(_nodeStyle)
            {
                normal =
                {
                    background = CreateColorTexture(new Color(0.3f, 0.4f, 0.5f))
                }
            };

            _actionNodeStyle = new GUIStyle(_nodeStyle)
            {
                normal =
                {
                    background = CreateColorTexture(new Color(0.5f, 0.3f, 0.3f))
                }
            };

            _rootNodeStyle = new GUIStyle(_nodeStyle)
            {
                normal =
                {
                    background = CreateColorTexture(new Color(0.3f, 0.5f, 0.3f))
                }
            };
        }

        private Texture2D CreateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (!_treeAsset)
            {
                DrawNoAssetMessage();
                return;
            }

            DrawCanvas();

            if (GUI.changed)
            {
                Repaint();
            }
        }

        private void DrawCanvas()
        {
            var canvasRect = new Rect(0, 20, position.width, position.height - 20);

            DrawGrid(canvasRect);

            ProcessEvents(Event.current);

            var matrix = GUI.matrix;
            var pivotPoint = canvasRect.size / 2;
            GUIUtility.ScaleAroundPivot(new Vector2(_zoomLevel, _zoomLevel), pivotPoint);

            DrawConnections();
            DrawNodes();

            GUI.matrix = matrix;
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUI.BeginChangeCheck();
            var newAsset =
                EditorGUILayout.ObjectField(_treeAsset, typeof(DecisionTreeAsset), false, GUILayout.Width(200)) as
                    DecisionTreeAsset;
            if (EditorGUI.EndChangeCheck() && newAsset != _treeAsset)
            {
                LoadTree(newAsset);
            }

            if (_treeAsset)
            {
                GUILayout.Space(10);

                if (GUILayout.Button("New Tree", EditorStyles.toolbarButton, GUILayout.Width(70)))
                {
                    CreateNewTree();
                }

                if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    SaveTree();
                }

                GUILayout.Space(10);

                GUI.backgroundColor = new Color(0.6f, 0.8f, 1f);
                if (GUILayout.Button("+ Question", EditorStyles.toolbarButton, GUILayout.Width(100)))
                {
                    CreateNodeAtCenter(false);
                }

                GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
                if (GUILayout.Button("+ Action", EditorStyles.toolbarButton, GUILayout.Width(100)))
                {
                    CreateNodeAtCenter(true);
                }

                GUI.backgroundColor = Color.white;

                if (_selectedNode != null)
                {
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    if (GUILayout.Button("Delete Node", EditorStyles.toolbarButton, GUILayout.Width(100)))
                    {
                        DeleteSelectedNode();
                    }

                    GUI.backgroundColor = Color.white;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Center", EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    CenterView();
                }

                GUILayout.Label($"Zoom: {_zoomLevel:P0}", GUILayout.Width(70));
                _zoomLevel = GUILayout.HorizontalSlider(_zoomLevel, 0.5f, 2f, GUILayout.Width(100));

                if (GUILayout.Button("Reset", EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    _zoomLevel = 1f;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void CreateNewTree()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                "Save Decision Tree",
                "NewDecisionTree",
                "asset",
                "Save decision tree asset"
            );

            if (string.IsNullOrEmpty(path)) return;
            
            var newAsset = ScriptableObject.CreateInstance<DecisionTreeAsset>();
            AssetDatabase.CreateAsset(newAsset, path);
            AssetDatabase.SaveAssets();
            LoadTree(newAsset);
        }

        private void DrawNoAssetMessage()
        {
            GUILayout.BeginArea(new Rect(0, 20, position.width, position.height - 20));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label("No Decision Tree Asset Selected", EditorStyles.largeLabel);
            GUILayout.Space(20);
            if (GUILayout.Button("Create New Tree", GUILayout.Width(150), GUILayout.Height(30)))
            {
                CreateNewTree();
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }

        private void DrawGrid(Rect canvasRect)
        {
            Handles.BeginGUI();
            Handles.color = new Color(0.3f, 0.3f, 0.3f, 0.2f);

            var gridStep = GRID_SIZE * _zoomLevel;
            var offset = new Vector2(_canvasOffset.x % gridStep, _canvasOffset.y % gridStep);

            for (var x = offset.x; x < canvasRect.width; x += gridStep)
            {
                Handles.DrawLine(new Vector3(x, canvasRect.y, 0), new Vector3(x, canvasRect.height, 0));
            }

            for (var y = offset.y + canvasRect.y; y < canvasRect.height + canvasRect.y; y += gridStep)
            {
                Handles.DrawLine(new Vector3(0, y, 0), new Vector3(canvasRect.width, y, 0));
            }

            Handles.EndGUI();
        }

        private void DrawConnections()
        {
            Handles.BeginGUI();

            foreach (var (node, nodeView) in _nodeViews)
            {
                if (!IsActionNode(node))
                {
                    if (node.trueNode != null && _nodeViews.TryGetValue(node.trueNode, out var targetView))
                    {
                        DrawConnection(
                            nodeView.Rect.center,
                            targetView.Rect.center,
                            Color.green,
                            "TRUE"
                        );
                    }

                    if (node.falseNode != null && _nodeViews.TryGetValue(node.falseNode, out var targetView1))
                    {
                        DrawConnection(
                            nodeView.Rect.center,
                            targetView1.Rect.center,
                            Color.red,
                            "FALSE"
                        );
                    }
                }
            }

            if (_isConnecting && _connectionStart != null)
            {
                var color = _connectingFromTrue ? Color.green : Color.red;
                var label = _connectingFromTrue ? "TRUE" : "FALSE";
                var mousePos = GetMousePosInCanvas(Event.current.mousePosition);

                DrawConnection(
                    _connectionStart.Rect.center,
                    mousePos,
                    color * 0.7f,
                    label
                );
                Repaint();
            }

            Handles.EndGUI();
        }

        private void DrawConnection(Vector2 start, Vector2 end, Color color, string label)
        {
            Handles.color = color;

            var startTangent = start + Vector2.down * 50;
            var endTangent = end + Vector2.up * 50;

            Handles.DrawBezier(
                start, end,
                startTangent, endTangent,
                color, null, 3f
            );

            var midPoint = (start + end) / 2;
            var labelStyle = new GUIStyle(EditorStyles.label)
            {
                normal =
                {
                    textColor = color
                },
                fontStyle = FontStyle.Bold
            };

            var labelSize = labelStyle.CalcSize(new GUIContent(label));
            var labelRect = new Rect(midPoint.x - labelSize.x / 2, midPoint.y - labelSize.y / 2, labelSize.x,
                labelSize.y);

            GUI.Label(labelRect, label, labelStyle);
        }

        private void DrawNodes()
        {
            BeginWindows();

            var windowId = 0;
            foreach (var (node, nodeView) in _nodeViews.ToList())
            {
                if (!_nodeViews.ContainsKey(node)) continue;

                var style = GetNodeStyle(nodeView);
                var nodeTitle = GetNodeTitle(node);

                nodeView.Rect = GUI.Window(windowId++, nodeView.Rect, (id) => DrawNodeWindow(node, nodeView), nodeTitle,
                    style);
            }

            EndWindows();
        }

        private GUIStyle GetNodeStyle(NodeView nodeView)
        {
            if (nodeView == _selectedNode)
                return _selectedNodeStyle;

            if (nodeView.Node == _treeAsset.rootNode)
                return _rootNodeStyle;

            return IsActionNode(nodeView.Node) ? _actionNodeStyle : _questionNodeStyle;
        }

        private string GetNodeTitle(DecisionNode node)
        {
            if (node == _treeAsset.rootNode)
                return "ROOT NODE";

            if (IsActionNode(node))
            {
                return node.action ? node.action.name : "Action Node";
            }

            return node.question ? node.question.name : "Question Node";
        }

        private void DrawNodeWindow(DecisionNode node, NodeView nodeView)
        {
            EditorGUILayout.BeginVertical();

            var isAction = IsActionNode(node);

            EditorGUI.BeginChangeCheck();
            if (isAction)
            {
                var newAction = EditorGUILayout.ObjectField(
                    "Action",
                    node.action,
                    typeof(BaseDecisionResponseSO),
                    false,
                    GUILayout.Width(180)
                ) as BaseDecisionResponseSO;

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_treeAsset, "Change Action");
                    node.action = newAction;
                    EditorUtility.SetDirty(_treeAsset);
                }
            }
            else
            {
                var newQuestion = EditorGUILayout.ObjectField(
                    "Question",
                    node.question,
                    typeof(BaseDecisionQuestionSO),
                    false,
                    GUILayout.Width(180)
                ) as BaseDecisionQuestionSO;

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_treeAsset, "Change Question");
                    node.question = newQuestion;
                    EditorUtility.SetDirty(_treeAsset);
                }

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();

                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("True →", GUILayout.Height(25)))
                {
                    StartConnection(nodeView, true);
                }

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("False →", GUILayout.Height(25)))
                {
                    StartConnection(nodeView, false);
                }

                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            if (node == _treeAsset.rootNode)
            {
                EditorGUILayout.HelpBox("Root", MessageType.None);
            }

            EditorGUILayout.EndVertical();

            GUI.DragWindow();
        }

        private void ProcessEvents(Event e)
        {
            var mousePos = GetMousePosInCanvas(e.mousePosition);

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        OnLeftClick(mousePos, e);
                    }
                    else if (e.button == 1)
                    {
                        OnRightClick(mousePos, e);
                    }
                    else if (e.button == 2)
                    {
                        e.Use();
                    }

                    break;

                case EventType.MouseDrag:
                    if (e.button == 2)
                    {
                        var delta = e.delta / _zoomLevel;
                        _canvasOffset += delta;

                        foreach (var nodeView in _nodeViews.Values)
                        {
                            nodeView.Rect.position += delta;
                        }

                        GUI.changed = true;
                        e.Use();
                    }

                    break;

                case EventType.ScrollWheel:
                    var oldZoom = _zoomLevel;
                    _zoomLevel = Mathf.Clamp(_zoomLevel - e.delta.y * 0.01f, 0.5f, 2f);

                    if (!Mathf.Approximately(oldZoom, _zoomLevel))
                    {
                        var zoomDelta = _zoomLevel / oldZoom;
                        var pivot = new Vector2(position.width / 2, position.height / 2);

                        foreach (var nodeView in _nodeViews.Values)
                        {
                            var offset = nodeView.Rect.position - pivot;
                            nodeView.Rect.position = pivot + offset * zoomDelta;
                        }

                        GUI.changed = true;
                    }

                    e.Use();
                    break;

                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.Delete && _selectedNode != null)
                    {
                        DeleteSelectedNode();
                        e.Use();
                    }

                    break;
            }
        }

        private Vector2 GetMousePosInCanvas(Vector2 mousePos)
        {
            var center = new Vector2(position.width / 2, position.height / 2);
            return (mousePos - center) / _zoomLevel + center;
        }

        private void OnLeftClick(Vector2 mousePos, Event e)
        {
            if (_isConnecting)
            {
                var clickedNode = GetNodeViewAtPoint(mousePos);
                if (clickedNode != null && clickedNode != _connectionStart)
                {
                    CompleteConnection(clickedNode);
                }
                else
                {
                    CancelConnection();
                }

                e.Use();
            }
            else
            {
                _selectedNode = GetNodeViewAtPoint(mousePos);
                if (_selectedNode == null)
                {
                    GUI.FocusControl(null);
                }

                GUI.changed = true;
            }
        }

        private void OnRightClick(Vector2 mousePos, Event e)
        {
            var clickedNode = GetNodeViewAtPoint(mousePos);

            var menu = new GenericMenu();

            if (clickedNode != null)
            {
                _selectedNode = clickedNode;

                menu.AddItem(new GUIContent("Set as Root"), clickedNode.Node == _treeAsset.rootNode,
                    () => SetAsRoot(clickedNode));

                menu.AddSeparator("");

                if (IsActionNode(clickedNode.Node))
                {
                    menu.AddItem(new GUIContent("Convert to Question"), false,
                        () => ConvertNodeType(clickedNode, false));
                }
                else
                {
                    menu.AddItem(new GUIContent("Convert to Action"), false,
                        () => ConvertNodeType(clickedNode, true));

                    if (clickedNode.Node.trueNode != null)
                    {
                        menu.AddItem(new GUIContent("Clear True Connection"), false,
                            () => ClearConnection(clickedNode, true));
                    }

                    if (clickedNode.Node.falseNode != null)
                    {
                        menu.AddItem(new GUIContent("Clear False Connection"), false,
                            () => ClearConnection(clickedNode, false));
                    }
                }

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete Node"), false, () => DeleteNode(clickedNode));
            }
            else
            {
                menu.AddItem(new GUIContent("Add Question Node"), false,
                    () => CreateNodeAt(mousePos, false));
                menu.AddItem(new GUIContent("Add Action Node"), false,
                    () => CreateNodeAt(mousePos, true));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Center View"), false, CenterView);
            }

            menu.ShowAsContext();
            e.Use();
        }

        private void CreateNodeAtCenter(bool isAction)
        {
            var center = new Vector2(position.width / 2, position.height / 2);
            var canvasCenter = GetMousePosInCanvas(center);
            CreateNodeAt(canvasCenter, isAction);
        }

        private void CreateNodeAt(Vector2 creationPos, bool isAction)
        {
            Undo.RecordObject(_treeAsset, "Create Node");

            var node = new DecisionNode
            {
                editorPosition = creationPos
            };

            _nodeIsAction[node] = isAction;

            var nodeView = new NodeView
            {
                Node = node,
                Rect = new Rect(position.x - NODE_WIDTH / 2, position.y - NODE_HEIGHT / 2, NODE_WIDTH, NODE_HEIGHT)
            };

            _nodeViews[node] = nodeView;

            _treeAsset.rootNode ??= node;

            _selectedNode = nodeView;

            EditorUtility.SetDirty(_treeAsset);
            GUI.changed = true;
        }

        private void DeleteSelectedNode()
        {
            if (_selectedNode != null)
            {
                DeleteNode(_selectedNode);
            }
        }

        private void DeleteNode(NodeView nodeView)
        {
            if (nodeView.Node == _treeAsset.rootNode && _nodeViews.Count > 1)
            {
                if (!EditorUtility.DisplayDialog("Delete Root Node",
                        "This is the root node. Deleting it will make another node the root. Continue?",
                        "Delete", "Cancel"))
                {
                    return;
                }
            }

            Undo.RecordObject(_treeAsset, "Delete Node");

            foreach (var node in _nodeViews.Select(kvp => kvp.Key))
            {
                if (node.trueNode == nodeView.Node)
                    node.trueNode = null;
                if (node.falseNode == nodeView.Node)
                    node.falseNode = null;
            }

            _nodeViews.Remove(nodeView.Node);
            _nodeIsAction.Remove(nodeView.Node);

            if (nodeView.Node == _treeAsset.rootNode)
            {
                _treeAsset.rootNode = _nodeViews.Count > 0 ? _nodeViews.First().Key : null;
            }

            if (_selectedNode == nodeView)
            {
                _selectedNode = null;
            }

            EditorUtility.SetDirty(_treeAsset);
            GUI.changed = true;
        }

        private void SetAsRoot(NodeView nodeView)
        {
            Undo.RecordObject(_treeAsset, "Set Root");
            _treeAsset.rootNode = nodeView.Node;
            EditorUtility.SetDirty(_treeAsset);
            GUI.changed = true;
        }

        private void ConvertNodeType(NodeView nodeView, bool toAction)
        {
            Undo.RecordObject(_treeAsset, toAction ? "Convert to Action" : "Convert to Question");

            _nodeIsAction[nodeView.Node] = toAction;

            if (toAction)
            {
                nodeView.Node.question = null;
                nodeView.Node.trueNode = null;
                nodeView.Node.falseNode = null;
            }
            else
            {
                nodeView.Node.action = null;
            }

            EditorUtility.SetDirty(_treeAsset);
            GUI.changed = true;
        }

        private void ClearConnection(NodeView nodeView, bool clearTrue)
        {
            Undo.RecordObject(_treeAsset, "Clear Connection");

            if (clearTrue)
                nodeView.Node.trueNode = null;
            else
                nodeView.Node.falseNode = null;

            EditorUtility.SetDirty(_treeAsset);
            GUI.changed = true;
        }

        private void StartConnection(NodeView nodeView, bool fromTrue)
        {
            _connectionStart = nodeView;
            _isConnecting = true;
            _connectingFromTrue = fromTrue;
        }

        private void CompleteConnection(NodeView target)
        {
            if (_connectionStart == null || target == null || _connectionStart == target)
            {
                CancelConnection();
                return;
            }

            if (WouldCreateCircularDependency(_connectionStart.Node, target.Node))
            {
                EditorUtility.DisplayDialog("Invalid Connection",
                    "This would create a circular dependency!", "OK");
                CancelConnection();
                return;
            }

            Undo.RecordObject(_treeAsset, "Create Connection");

            if (_connectingFromTrue)
                _connectionStart.Node.trueNode = target.Node;
            else
                _connectionStart.Node.falseNode = target.Node;

            EditorUtility.SetDirty(_treeAsset);
            CancelConnection();
        }

        private bool WouldCreateCircularDependency(DecisionNode from, DecisionNode to)
        {
            if (from == to) return true;

            var visited = new HashSet<DecisionNode>();
            return CheckCircular(to, from, visited);
        }

        private bool CheckCircular(DecisionNode current, DecisionNode target, HashSet<DecisionNode> visited)
        {
            if (current == null || visited.Contains(current)) return false;
            if (current == target) return true;

            visited.Add(current);

            return CheckCircular(current.trueNode, target, visited) ||
                   CheckCircular(current.falseNode, target, visited);
        }

        private void CancelConnection()
        {
            _connectionStart = null;
            _isConnecting = false;
            _connectingFromTrue = false;
            GUI.changed = true;
        }

        private NodeView GetNodeViewAtPoint(Vector2 point)
        {
            return (from kvp in _nodeViews where kvp.Value.Rect.Contains(point) select kvp.Value).FirstOrDefault();
        }

        private void CenterView()
        {
            if (_nodeViews.Count == 0) return;
            
            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            foreach (var nodeView in _nodeViews.Values)
            {
                minX = Mathf.Min(minX, nodeView.Rect.xMin);
                maxX = Mathf.Max(maxX, nodeView.Rect.xMax);
                minY = Mathf.Min(minY, nodeView.Rect.yMin);
                maxY = Mathf.Max(maxY, nodeView.Rect.yMax);
            }

            var center = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
            var targetCenter = new Vector2(position.width / 2, position.height / 2) / _zoomLevel;
            var offset = targetCenter - center;

            foreach (var nodeView in _nodeViews.Values)
            {
                nodeView.Rect.position += offset;
            }

            _canvasOffset = Vector2.zero;
            GUI.changed = true;
        }

        private bool IsActionNode(DecisionNode node)
        {
            if (_nodeIsAction.TryGetValue(node, out var actionNode))
                return actionNode;

            return node.action || (!node.question && node.trueNode == null && node.falseNode == null);
        }

        private void LoadTree(DecisionTreeAsset asset)
        {
            _treeAsset = asset;
            _nodeViews.Clear();
            _nodeIsAction.Clear();
            _selectedNode = null;
            _connectionStart = null;
            _isConnecting = false;

            if (asset && asset.rootNode != null)
            {
                var processed = new HashSet<DecisionNode>();
                LoadNodeRecursive(asset.rootNode, new Vector2(position.width / 2, 100), 0, processed);
                CenterView();
            }

            Repaint();
        }

        private void LoadNodeRecursive(DecisionNode node, Vector2 nodePosition, int depth,
            HashSet<DecisionNode> processed)
        {
            while (true)
            {
                if (node == null || !processed.Add(node)) return;

                var nodePos = node.editorPosition != Vector2.zero ? node.editorPosition : nodePosition;

                var isAction = node.action || (!node.question && node.trueNode == null && node.falseNode == null);
                _nodeIsAction[node] = isAction;

                var nodeView = new NodeView
                {
                    Node = node,
                    Rect = new Rect(nodePos.x - NODE_WIDTH / 2, nodePos.y - NODE_HEIGHT / 2, NODE_WIDTH, NODE_HEIGHT)
                };

                _nodeViews[node] = nodeView;

                if (node.trueNode != null)
                {
                    LoadNodeRecursive(node.trueNode, new Vector2(position.x - 200, position.y + 150), depth + 1,
                        processed);
                }

                if (node.falseNode != null)
                {
                    node = node.falseNode;
                    nodePosition = new Vector2(position.x + 200, position.y + 150);
                    depth += 1;
                    continue;
                }

                break;
            }
        }

        private void SaveTree()
        {
            if (!_treeAsset) return;

            foreach (var kvp in _nodeViews)
            {
                kvp.Key.editorPosition = kvp.Value.Rect.center;
            }

            EditorUtility.SetDirty(_treeAsset);
            AssetDatabase.SaveAssets();
        }

        private class NodeView
        {
            public Rect Rect;
            public DecisionNode Node;
        }
    }
}