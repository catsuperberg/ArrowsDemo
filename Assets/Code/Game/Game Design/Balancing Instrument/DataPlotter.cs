using ExtensionMethods;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameDesign
{
    public struct ChartDataPoint
    {
        public readonly double X;
        public readonly double Y;

        public ChartDataPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    
    public class DataPlotter 
    {
        const int _logBase = 10;  
        IPaint<LiveChartsCore.SkiaSharpView.Drawing.SkiaSharpDrawingContext> _gradientPaint = new LinearGradientPaint(
                            new []{SKColors.Transparent, SKColors.Aquamarine}, 
                            new SKPoint(0.5f, 1), new SKPoint(0.5f,-0.8f),
                            tileMode: SKShaderTileMode.Clamp);
        IPaint<LiveChartsCore.SkiaSharpView.Drawing.SkiaSharpDrawingContext> _strokePaint = new SolidColorPaint(SKColors.Beige) {StrokeThickness = 2};
        
        public string PlotXYLog(IEnumerable<ChartDataPoint> dataPoints, Vector2Int pictureSize)
        {
            var chart = new SKCartesianChart
            {                
                Width = pictureSize.x,
                Height = pictureSize.y,
                Series = new ISeries[]
                {
                    new LineSeries<ChartDataPoint> 
                    {                        
                        Mapping = (logPoint, chartPoint) =>
                        {
                            chartPoint.SecondaryValue = logPoint.X;
                            chartPoint.PrimaryValue = Math.Log(logPoint.Y, _logBase); 
                        }, 
                        
                        Values = dataPoints,
                        Fill = _gradientPaint,
                        Stroke = _strokePaint,
                        GeometryFill = null,
                        GeometryStroke = null
                    }
                },
                
                YAxes = new List<Axis>
                {
                    new Axis
                    {
                        MinStep = 1,
                        Labeler = value => new System.Numerics.BigInteger(Math.Pow(_logBase, value)).ParseToReadable(),
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        SeparatorsPaint = new SolidColorPaint(SKColors.Beige) { StrokeThickness = 1 } 
                    }
                },
                
                XAxes = new List<Axis>
                {
                    new Axis
                    {
                        MinStep = 1,
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        TextSize = 12
                    }
                },
                Background = SKColor.FromHsv(170,20,11)
            };
            
            
            var image = chart.GetImage();            
            var data = image.Encode();
            var imageB64 = Convert.ToBase64String(data.AsSpan());
            chart = null;        
                       
            return imageB64;
        } 
        
        public string PlotXY(IEnumerable<ChartDataPoint> dataPoints, Vector2Int pictureSize)
        {
            var chart = new SKCartesianChart
            {                
                Width = pictureSize.x,
                Height = pictureSize.y,
                Series = new ISeries[]
                {
                    new LineSeries<ChartDataPoint> 
                    {                        
                        Mapping = (logPoint, chartPoint) =>
                        {
                            chartPoint.SecondaryValue = logPoint.X;
                            chartPoint.PrimaryValue = logPoint.Y; 
                        }, 
                        
                        Values = dataPoints,
                        Fill = _gradientPaint,
                        Stroke = _strokePaint,
                        GeometryFill = null,
                        GeometryStroke = null
                    }
                },
                
                YAxes = new List<Axis>
                {
                    new Axis
                    {
                        MinStep = 1,
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        SeparatorsPaint = new SolidColorPaint(SKColors.Beige) { StrokeThickness = 1 } 
                    }
                },
                
                XAxes = new List<Axis>
                {
                    new Axis
                    {
                        MinStep = 1,
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        TextSize = 12
                    }
                },
                Background = SKColor.FromHsv(170,20,11)
            };
            
            
            var image = chart.GetImage();  
            chart = null;          
            var data = image.Encode();
            var imageB64 = Convert.ToBase64String(data.AsSpan());
            
            return imageB64;
        }
        
    }
}