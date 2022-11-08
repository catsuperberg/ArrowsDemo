using ExtensionMethods;
using Game.GameDesign;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GameDesign
{
    public class ChartDataPoints
    {
        public readonly double X;
        public readonly double Y;

        public ChartDataPoints(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    
    public class DataPlotter 
    {
        public string PlotXYBase64(IEnumerable<ChartDataPoints> dataPoints, Vector2Int pictureSize)
        {
            var chart = new SKCartesianChart
            {                
                Width = pictureSize.x,
                Height = pictureSize.y,
                Series = new ISeries[]
                {
                    new LineSeries<ChartDataPoints> 
                    {                        
                        Mapping = (logPoint, chartPoint) =>
                        {
                            chartPoint.SecondaryValue = logPoint.X;
                            chartPoint.PrimaryValue = Math.Log(logPoint.Y, 10); 
                        }, 
                        
                        Values = dataPoints,
                        Fill = new LinearGradientPaint(
                            new []{SKColors.Transparent, SKColors.Aquamarine}, 
                            new SKPoint(0.4f, 1), new SKPoint(0.5f,-0.8f),
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
                        Labeler = value => new System.Numerics.BigInteger(Math.Pow(10, value)).ParseToReadable(),
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
            return Convert.ToBase64String(data.AsSpan());
        }
    }
}