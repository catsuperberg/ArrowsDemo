
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{  
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }
    
    public class FlexibleGridLayout : LayoutGroup
    {
        [SerializeField]
        FitType _fitType;
        [SerializeField]
        int _rows;
        [SerializeField]
        int _columns;
        [SerializeField]
        Vector2 _cellSize;
        [SerializeField]
        Vector2 _spacing;
        [SerializeField]
        RectTransform _parentRectAsLimits;
        [SerializeField]
        bool _preserveChildSize;
        
        bool _autoFit;
        
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            _autoFit = false;
            if ((_fitType & (FitType.Width | FitType.Height | FitType.Uniform)) != 0 && !_preserveChildSize)
            {
                _autoFit = true;
                var elementsPerDirection = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
                _rows = _columns = elementsPerDirection;
            }

            SetPrioritizedDimension();
            var containerTransform = GetComponent<RectTransform>();
            var parentSize = transform.parent.GetComponent<RectTransform>().rect;
            SetCellSize(containerTransform.rect);
            if(_parentRectAsLimits != null)
                UpdateContainerSizeAndPosition(containerTransform);
            ResizeAndPositionChildren();
        }

        void UpdateContainerSizeAndPosition( RectTransform container)
        {
            var parentSize = _parentRectAsLimits.rect;
            var sizeToFitRows = _cellSize.y * _rows;
            var containerHeight = (sizeToFitRows >= parentSize.height) ? sizeToFitRows : parentSize.height;
            container.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, containerHeight);
            container.ForceUpdateRectTransforms();
        }

        void SetCellSize(Rect containerSize)
        {
            var cellWidth = _autoFit ? GetCellDimension(containerSize.width, new List<float> { _spacing.x, padding.left, padding.right }, _columns) : _cellSize.x;
            var cellHeight = _autoFit ? GetCellDimension(containerSize.height, new List<float> { _spacing.y, padding.top, padding.bottom }, _rows) : _cellSize.y;
            _cellSize = new Vector2(cellWidth, cellHeight);
        }

        void ResizeAndPositionChildren()
        {
            if(rectChildren.Count <= 1)
                PositionSingleChildAndKeepSize();
            else
                foreach (var (childElement, i) in rectChildren.Select((childElement, i) => (childElement, i)))
                    ResizeAndPositionChild(childElement, i);
        }

        private void ResizeAndPositionChild(RectTransform childElement, int i)
        {
            var positionInRow = i / _columns;
            var positionInColumn = i % _columns;
            var offsetX = padding.left + (_cellSize.x + _spacing.x) * positionInColumn;
            var offsetY = padding.top + (_cellSize.y + _spacing.y) * positionInRow;
            SetChildAlongAxis(childElement, (int)RectTransform.Axis.Horizontal, offsetX, _cellSize.x);
            SetChildAlongAxis(childElement, (int)RectTransform.Axis.Vertical, offsetY, _cellSize.y);
        }

        void PositionSingleChildAndKeepSize()
        {
            var child = rectChildren.First();
            SetChildAlongAxis(child, (int)RectTransform.Axis.Horizontal, padding.left);
            SetChildAlongAxis(child, (int)RectTransform.Axis.Vertical, padding.top);
        }

        void SetPrioritizedDimension()
        {
            if((_fitType & (FitType.Width | FitType.FixedColumns)) != 0)
                SetRowsAccordingToColumns();
            if((_fitType & (FitType.Height | FitType.FixedRows)) != 0)
                SetColumnsAccordingToRaws();
        }
        
        void SetRowsAccordingToColumns() => _rows = Mathf.CeilToInt(transform.childCount / (float) _columns);
        void SetColumnsAccordingToRaws() => _columns = Mathf.CeilToInt(transform.childCount / (float) _rows);
        
        float GetCellDimension(float parentDimension, List<float> spacingAndPaddingPerAll, float elementsInThisDirection)
        {
            var dimensionWithoutPaddingAndSpacing = parentDimension - spacingAndPaddingPerAll.Sum() * (elementsInThisDirection - 1);
            var oneCellDimension = dimensionWithoutPaddingAndSpacing/elementsInThisDirection;
            return oneCellDimension;
        }
        
        public override void CalculateLayoutInputVertical()
        {
            
        }
        
        public override void SetLayoutHorizontal()
        {
            
        }
        
        public override void SetLayoutVertical()
        {
            
        }    
    }
}
