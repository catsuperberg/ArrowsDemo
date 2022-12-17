using ExtensionMethods;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GameDesign
{
    public class DataPlotter 
    {
        const int _logBase = 10;  
        IPaint<LiveChartsCore.SkiaSharpView.Drawing.SkiaSharpDrawingContext> _gradientPaint = new LinearGradientPaint(
                            new []{SKColors.Transparent, SKColors.Aquamarine}, 
                            new SKPoint(0.5f, 1), new SKPoint(0.5f,-0.8f),
                            tileMode: SKShaderTileMode.Clamp);
        IPaint<LiveChartsCore.SkiaSharpView.Drawing.SkiaSharpDrawingContext> _strokePaint = new SolidColorPaint(SKColors.Beige) {StrokeThickness = 2};
        SKColor _backgroundColor = SKColor.FromHsv(170,20,11);
        
        SKColor[] ColorSelection = {SKColors.Yellow, SKColors.SteelBlue, SKColors.LimeGreen, SKColors.Aqua, SKColors.Red, SKColors.Beige};
        
        static string DefaultLabeler(double value) => value.ToString();
        
        
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
                
                YAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        Labeler = value => new System.Numerics.BigInteger(Math.Pow(_logBase, value)).ParseToReadable(),
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        SeparatorsPaint = new SolidColorPaint(SKColors.Beige) { StrokeThickness = 1 } 
                    }},
                
                XAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        TextSize = 12
                    }},
                Background = _backgroundColor
            };
            
            return RenderChart(chart);
        } 
        
        public string PlotXYLogMultiple(Dictionary<string, IEnumerable<ChartDataPoint>> multiSeries, Vector2Int pictureSize)
        {     
            var maxDisplayableValue = double.Parse("1.00E250"); // because labeler gonna create labels with higher than max double value even when limiting by max double        
            multiSeries = multiSeries
                            .ToDictionary(series => series.Key, series => series.Value
                                .Where(dataPoints => dataPoints.Y < maxDisplayableValue));
                  
            var chart = new SKCartesianChart
            {                
                Width = pictureSize.x,
                Height = pictureSize.y,                
                
                Series = multiSeries
                            .Select((entry, index) => 
                                new LineSeries<ChartDataPoint> 
                                {            
                                    Name = entry.Key,  
                                    Mapping = (logPoint, chartPoint) =>
                                    {
                                        chartPoint.SecondaryValue = logPoint.X;
                                        chartPoint.PrimaryValue = Math.Log(logPoint.Y, _logBase); 
                                    }, 
                                    
                                    Values = entry.Value,
                                    Fill = null,
                                    Stroke = new SolidColorPaint(ColorSelection[index%ColorSelection.Length]) {StrokeThickness = 3},
                                    GeometryFill = null,
                                    GeometryStroke = null,
                                })
                            .ToArray(),
                
                YAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        Labeler = value => new System.Numerics.BigInteger(Math.Pow(_logBase, value)).ParseToReadable(),
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        SeparatorsPaint = new SolidColorPaint(SKColors.Beige) { StrokeThickness = 1 } 
                    }},
                
                XAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        TextSize = 12
                    }},
                Background = _backgroundColor,
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
                LegendFontPaint = new SolidColorPaint(SKColors.Beige)
            };
            
            return RenderChart(chart);            
        } 
        
        public string PlotXLogY(IEnumerable<ChartDataPoint> dataPoints, Vector2Int pictureSize)
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
                            chartPoint.SecondaryValue = Math.Log(logPoint.X, _logBase);
                            chartPoint.PrimaryValue = logPoint.Y; 
                        }, 
                        
                        Values = dataPoints,
                        Fill = _gradientPaint,
                        Stroke = _strokePaint,
                        GeometryFill = null,
                        GeometryStroke = null
                    }
                },
                
                YAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        SeparatorsPaint = new SolidColorPaint(SKColors.Beige) { StrokeThickness = 1 } 
                    }},
                
                XAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        Labeler = value => new System.Numerics.BigInteger(Math.Pow(_logBase, value)).ParseToReadable(),
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        TextSize = 12
                    }},
                Background = _backgroundColor
            };
            
            return RenderChart(chart);
        } 
        
        public string PlotXLogYLog(IEnumerable<ChartDataPoint> dataPoints, Vector2Int pictureSize)
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
                            chartPoint.SecondaryValue = Math.Log(logPoint.X, _logBase);
                            chartPoint.PrimaryValue = Math.Log(logPoint.Y, _logBase);
                        }, 
                        
                        Values = dataPoints,
                        Fill = _gradientPaint,
                        Stroke = _strokePaint,
                        GeometryFill = null,
                        GeometryStroke = null
                    }
                },
                
                YAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        Labeler = value => Math.Pow(_logBase, value).ToString(),
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        SeparatorsPaint = new SolidColorPaint(SKColors.Beige) { StrokeThickness = 1 } 
                    }},
                
                XAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        Labeler = value => new System.Numerics.BigInteger(Math.Pow(_logBase, value)).ParseToReadable(),
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        TextSize = 12
                    }},
                Background = _backgroundColor
            };
            
            return RenderChart(chart);
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
                
                YAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        SeparatorsPaint = new SolidColorPaint(SKColors.Beige) { StrokeThickness = 1 } 
                    }},
                
                XAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        TextSize = 12
                    }},
                Background = _backgroundColor
            };
            
            return RenderChart(chart);
        }
        
        public string PlotColumns(ColumnDataPoints dataPoints, Vector2Int pictureSize)
            => PlotColumns(dataPoints, pictureSize, DefaultLabeler);
        
        public string PlotColumns(ColumnDataPoints dataPoints, Vector2Int pictureSize, Func<double, string> yLabeler)
        {
            var chart = new SKCartesianChart
            {
                Width = pictureSize.x,
                Height = pictureSize.y,
                
                Series = new ISeries[] {new ColumnSeries<double>
                    {
                        Values = dataPoints.Values,
                        Fill = _gradientPaint,
                        Stroke = _strokePaint
                    }},                
                
                YAxes = new List<Axis> {new Axis
                    {
                        MinStep = 1,
                        Labeler = value => yLabeler(value),
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        SeparatorsPaint = new SolidColorPaint(SKColors.Beige) { StrokeThickness = 1 } 
                    }},
                
                XAxes = new List<Axis>{new Axis
                    {
                        Labels = dataPoints.Labels,
                        MinStep = 1,
                        LabelsPaint = new SolidColorPaint(SKColors.Beige),
                        TextSize = 12,
                        LabelsRotation = -45
                    }},
                Background = _backgroundColor
            };
            
            return RenderChart(chart);
        }
        
        string RenderChart(InMemorySkiaSharpChart chart)
        {            
            var image = chart.GetImage();  
            chart = null;          
            var data = image.Encode();
            return Convert.ToBase64String(data.AsSpan());
        }
    }
}