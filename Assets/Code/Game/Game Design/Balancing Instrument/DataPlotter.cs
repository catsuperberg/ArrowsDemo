using ExtensionMethods;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameDesign
{
    public class ChartDataPoint
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
        
        public Texture2D PlotXYLog(IEnumerable<ChartDataPoint> dataPoints, Vector2Int pictureSize)
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
                        Fill = new LinearGradientPaint(
                            new []{SKColors.Transparent, SKColors.Aquamarine}, 
                            new SKPoint(0.5f, 1), new SKPoint(0.5f,-0.8f),
                            tileMode: SKShaderTileMode.Clamp),
                        Stroke = new SolidColorPaint(SKColors.Beige) {StrokeThickness = 2},
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
                        
            Texture2D texture = new Texture2D(1,1);
            texture.LoadImage(Convert.FromBase64String(imageB64));  
            return texture; 
        } 
        
        public Texture2D PlotXY(IEnumerable<ChartDataPoint> dataPoints, Vector2Int pictureSize)
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
                        Fill = new LinearGradientPaint(
                            new []{SKColors.Transparent, SKColors.Aquamarine}, 
                            new SKPoint(0.5f, 1), new SKPoint(0.5f,-0.8f),
                            tileMode: SKShaderTileMode.Clamp),
                        Stroke = new SolidColorPaint(SKColors.Beige) {StrokeThickness = 2},
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
            var data = image.Encode();
            var imageB64 = Convert.ToBase64String(data.AsSpan());
                        
            Texture2D texture = new Texture2D(1,1);
            texture.LoadImage(Convert.FromBase64String(imageB64));  
            return texture; 
        }
        
    }
}