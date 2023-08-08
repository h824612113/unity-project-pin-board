#if !UNITY_2021_1_OR_NEWER
using System.Linq;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        #region Initialization

        /// <summary>
        /// 内容
        /// </summary>
        private VisualElement m_Content = null;

        /// <summary>
        /// 内容占位
        /// </summary>
        private VisualElement m_ContentPlaceholder = null;

        /// <summary>
        /// 内容分栏面板
        /// </summary>
        private TwoPaneSplitView m_ContentSplitView = null;

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitContent()
        {
            // 绑定视图
            m_Content = rootVisualElement.Q<VisualElement>("Content");
            {
                m_Content.style.flexBasis = Length.Percent(100);
                m_Content.style.marginTop = 0;
            }

            // 占位
            m_ContentPlaceholder = new VisualElement()
            {
                name = "Placeholder",
                style =
                {
                    flexBasis = Length.Percent(100),
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                }
            };
            m_Content.Add(m_ContentPlaceholder);
            {
                // 占位文本
                m_ContentPlaceholder.Add(new Label()
                {
                    text = "Empty...",
                    style =
                    {
                        fontSize = 22,
                        color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 0.8f),
                        unityFontStyleAndWeight = FontStyle.Bold,
                    }
                });
            }

            // 内容分栏
            m_ContentSplitView = new TwoPaneSplitView()
            {
                name = "ContentSplitView",
                fixedPaneIndex = 0,
                fixedPaneInitialDimension = ProjectPinBoardSettings.dragLinePos,
                orientation = TwoPaneSplitViewOrientation.Horizontal,
                style =
                {
                    flexBasis = Length.Percent(100),
                }
            };
            m_Content.Add(m_ContentSplitView);

            // 元素就绪后恢复预览区域状态
            {
                void Callback(GeometryChangedEvent evt)
                {
                    TogglePreview(ProjectPinBoardSettings.enablePreview);
                    m_ContentSplitView.UnregisterCallback<GeometryChangedEvent>(Callback);
                }

                m_ContentSplitView.RegisterCallback<GeometryChangedEvent>(Callback);
            }

            // 拖拽线
            {
                VisualElement dragLine = m_ContentSplitView.Q<VisualElement>("unity-dragline-anchor");
                IStyle dragLineStyle = dragLine.style;
                // 禁止拖拽线在Hover的时候变颜色
                Color color = dragLineColor;
                dragLine.RegisterCallback<MouseEnterEvent>((evt) => dragLineStyle.backgroundColor = color);
                // 拖动拖拽线后保存其位置
                dragLine.RegisterCallback<MouseUpEvent>((evt) => ProjectPinBoardSettings.dragLinePos = dragLineStyle.left.value.value);
            }

            // 初始化列表
            InitListView();
            // 初始化预览
            InitContentPreview();
        }

        /// <summary>
        /// 内容是否初始化
        /// </summary>
        /// <returns></returns>
        private bool IsContentInited()
        {
            return (m_Content != null);
        }

        #endregion

        /// <summary>
        /// 更新内容
        /// </summary>
        private void UpdateContent()
        {
            if (!IsContentInited()) return;

            // 取消选中
            ClearAssetListSelection();

            // 更新列表
            UpdateAssetList();

            // 恢复选中
            if (!string.IsNullOrEmpty(m_FirstSelectedItemGuid) && m_AssetListData.Count > 0)
            {
                SetAssetListSelection(m_FirstSelectedItemGuid);
                SetPreview(ProjectPinBoardData.GetItem(m_FirstSelectedItemGuid));
            }

            // 列表为空时展示占位
            m_ContentPlaceholder.style.display = (m_AssetListData.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None);
            m_ContentSplitView.style.display = (m_AssetListData.Count == 0 ? DisplayStyle.None : DisplayStyle.Flex);
        }

    }

}
