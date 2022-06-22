
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
        
        bool _fitWidth;
        bool _fitHeight;
        
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            
            _fitWidth = _fitHeight = false;
            if((_fitType & (FitType.Width | FitType.Height | FitType.Uniform)) != 0)
            {
                _fitWidth = _fitHeight = true;
                var elementsPerDirection = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
                _rows = _columns = elementsPerDirection;
            }
            
            SetPrioritizedDimension();
            var parentSize = base.rectTransform.rect;     
            var cellWidth = _fitWidth ? GetCellDimension(parentSize.width, new List<float>{_spacing.x, padding.left, padding.right}, _columns) : _cellSize.x;
            var cellHeight = _fitWidth ? GetCellDimension(parentSize.height, new List<float>{_spacing.y, padding.top, padding.bottom}, _rows) : _cellSize.y;  
            _cellSize = new Vector2(cellWidth,cellHeight);
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.SetPositionAndRotation(new Vector3(parentSize.width/2, -(_cellSize.y*_rows/2), 0), rectTransform.rotation);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _cellSize.y*_rows);
            rectTransform.ForceUpdateRectTransforms();
            
            foreach(var (childElement, i) in rectChildren.Select((childElement, i) => ( childElement, i )))
            {
                var positionInRow = i / _columns;
                var positionInColumn = i % _columns;
                var offsetX = padding.left + (_cellSize.x + _spacing.x) * positionInColumn;
                var offsetY = padding.top + (_cellSize.y + _spacing.y) * positionInRow;                
                SetChildAlongAxis(childElement, (int)RectTransform.Axis.Horizontal, offsetX, _cellSize.x);
                SetChildAlongAxis(childElement, (int)RectTransform.Axis.Vertical, offsetY, _cellSize.y);
            }
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
