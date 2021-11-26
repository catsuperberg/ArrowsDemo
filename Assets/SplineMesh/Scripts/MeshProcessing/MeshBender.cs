﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SplineMesh {
    /// <summary>
    /// A component that creates a deformed mesh from a given one along the given spline segment.
    /// The source mesh will always be bended along the X axis.
    /// It can work on a cubic bezier curve or on any interval of a given spline.
    /// On the given interval, the mesh can be place with original scale, stretched, or repeated.
    /// The resulting mesh is stored in a MeshFilter component and automaticaly updated on the next update if the spline segment change.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class MeshBender : MonoBehaviour {
        private bool isDirty = false;
        private Mesh result;
        private bool useSpline;
        private Spline spline;
        private float intervalStart, intervalEnd;
        private CubicBezierCurve curve;
        private Dictionary<float, CurveSample> sampleCache = new Dictionary<float, CurveSample>();

        private SourceMesh source;
        /// <summary>
        /// The source mesh to bend.
        /// </summary>
        public SourceMesh Source {
            get { return source; }
            set {
                if (value == source) return;
                SetDirty();
                source = value;
            }
        }
        
        private FillingMode mode = FillingMode.StretchToInterval;
        /// <summary>
        /// The scaling mode along the spline
        /// </summary>
        public FillingMode Mode {
            get { return mode; }
            set {
                if (value == mode) return;
                SetDirty();
                mode = value;
            }
        }

        /// <summary>
        /// Sets a curve along which the mesh will be bent.
        /// The mesh will be updated if the curve changes.
        /// </summary>
        /// <param name="curve">The <see cref="CubicBezierCurve"/> to bend the source mesh along.</param>
        public void SetInterval(CubicBezierCurve curve) {
            if (this.curve == curve) return;
            if (curve == null) throw new ArgumentNullException("curve");
            if (this.curve != null) {
                this.curve.Changed.RemoveListener(SetDirty);
            }
            this.curve = curve;
            spline = null;
            curve.Changed.AddListener(SetDirty);
            useSpline = false;
            SetDirty();
        }

        /// <summary>
        /// Sets a spline's interval along which the mesh will be bent.
        /// If interval end is absent or set to 0, the interval goes from start to spline length.
        /// The mesh will be update if any of the curve changes on the spline, including curves
        /// outside the given interval.
        /// </summary>
        /// <param name="spline">The <see cref="SplineMesh"/> to bend the source mesh along.</param>
        /// <param name="intervalStart">Distance from the spline start to place the mesh minimum X.<param>
        /// <param name="intervalEnd">Distance from the spline start to stop deforming the source mesh.</param>
        public void SetInterval(Spline spline, float intervalStart, float intervalEnd = 0) {
            if (this.spline == spline && this.intervalStart == intervalStart && this.intervalEnd == intervalEnd) return;
            if (spline == null) throw new ArgumentNullException("spline");
            if (intervalStart < 0 || intervalStart >= spline.Length) {
                throw new ArgumentOutOfRangeException("interval start must be 0 or greater and lesser than spline length (was " + intervalStart + ")");
            }
            if (intervalEnd != 0 && intervalEnd <= intervalStart || intervalEnd > spline.Length) {
                throw new ArgumentOutOfRangeException("interval end must be 0 or greater than interval start, and lesser than spline length (was " + intervalEnd + ")");
            }
            if (this.spline != null) {
                // unlistening previous spline
                this.spline.CurveChanged.RemoveListener(SetDirty);
            }
            this.spline = spline;
            // listening new spline
            spline.CurveChanged.AddListener(SetDirty);

            curve = null;
            this.intervalStart = intervalStart;
            this.intervalEnd = intervalEnd;
            useSpline = true;
            SetDirty();
        }

        private void OnEnable() {
            if(GetComponent<MeshFilter>().sharedMesh != null) {
                result = GetComponent<MeshFilter>().sharedMesh;
            } else {
                GetComponent<MeshFilter>().sharedMesh = result = new Mesh();
                result.name = "Generated by " + GetType().Name;
            }
        }

        private void LateUpdate() {
            ComputeIfNeeded();
        }

        public void ComputeIfNeeded() {
            if (isDirty) {
                Compute();
            }
        }

        private void SetDirty() {
            isDirty = true;
        }

        /// <summary>
        /// Bend the mesh. This method may take time and should not be called more than necessary.
        /// Consider using <see cref="ComputeIfNeeded"/> for faster result.
        /// </summary>
        private  void Compute() {
            isDirty = false;
            switch (Mode) {
                case FillingMode.Once:
                    FillOnce();
                    break;
                case FillingMode.Repeat:
                    FillRepeat();
                    break;
                case FillingMode.StretchToInterval:
                    FillStretch();
                    break;
            }
        }

        private void OnDestroy() {
            if(curve != null) {
                curve.Changed.RemoveListener(Compute);
            }
        }

        /// <summary>
        /// The mode used by <see cref="MeshBender"/> to bend meshes on the interval.
        /// </summary>
        public enum FillingMode {
            /// <summary>
            /// In this mode, source mesh will be placed on the interval by preserving mesh scale.
            /// Vertices that are beyond interval end will be placed on the interval end.
            /// </summary>
            Once,
            /// <summary>
            /// In this mode, the mesh will be repeated to fill the interval, preserving
            /// mesh scale.
            /// This filling process will stop when the remaining space is not enough to
            /// place a whole mesh, leading to an empty interval.
            /// </summary>
            Repeat,
            /// <summary>
            /// In this mode, the mesh is deformed along the X axis to fill exactly the interval.
            /// </summary>
            StretchToInterval
        }

        private void FillOnce() {
            sampleCache.Clear();
            var bentVertices = new List<MeshVertex>(source.Vertices.Count);
            // for each mesh vertex, we found its projection on the curve
            foreach (var vert in source.Vertices) {
                float distance = vert.position.x - source.MinX;
                CurveSample sample;
                if (!sampleCache.TryGetValue(distance, out sample)) {
                    if (!useSpline) {
                        if (distance > curve.Length) distance = curve.Length;
                        sample = curve.GetSampleAtDistance(distance);
                    } else {
                        float distOnSpline = intervalStart + distance;
                        if (distOnSpline > spline.Length) {
                            if (spline.IsLoop) {
                                while (distOnSpline > spline.Length) {
                                    distOnSpline -= spline.Length;
                                }
                            } else {
                                distOnSpline = spline.Length;
                            }
                        }
                        sample = spline.GetSampleAtDistance(distOnSpline);
                    }
                    sampleCache[distance] = sample;
                }

                bentVertices.Add(sample.GetBent(vert));
            }

            MeshUtility.Update(result,
                source.Mesh,
                source.Triangles,
                bentVertices.Select(b => b.position),
                bentVertices.Select(b => b.normal));
        }

        private void FillRepeat() {            
            float intervalLength = useSpline?
                (intervalEnd == 0 ? spline.Length : intervalEnd) - intervalStart :
                curve.Length;
            int repetitionCount = Mathf.FloorToInt(intervalLength / source.Length);    
            var scaleCoeff = intervalLength/(source.Length*repetitionCount); // MYCHANGE stretching repeated to full length

            // building triangles and UVs for the repeated mesh
            var triangles = new List<int>();
            var uv = new List<Vector2>();
            var uv2 = new List<Vector2>();
            var uv3 = new List<Vector2>();
            var uv4 = new List<Vector2>();
            var uv5 = new List<Vector2>();
            var uv6 = new List<Vector2>();
            var uv7 = new List<Vector2>();
            var uv8 = new List<Vector2>();
            for (int i = 0; i < repetitionCount; i++) {
                foreach (var index in source.Triangles) {
                    triangles.Add(index + source.Vertices.Count * i);
                }
                uv.AddRange(source.Mesh.uv);
                uv2.AddRange(source.Mesh.uv2);
                uv3.AddRange(source.Mesh.uv3);
                uv4.AddRange(source.Mesh.uv4);
#if UNITY_2018_2_OR_NEWER
                uv5.AddRange(source.Mesh.uv5);
                uv6.AddRange(source.Mesh.uv6);
                uv7.AddRange(source.Mesh.uv7);
                uv8.AddRange(source.Mesh.uv8);
#endif
            }

            // computing vertices and normals
            var bentVertices = new List<MeshVertex>(source.Vertices.Count);
            float offset = 0;
            for (int i = 0; i < repetitionCount; i++) {

                sampleCache.Clear();
                // for each mesh vertex, we found its projection on the curve
                foreach (var vert in source.Vertices) {
                    float distance = vert.position.x - source.MinX + offset;                    
                    distance *= scaleCoeff-0.0001f; // MYCHANGE stretching repeated to full length
                    CurveSample sample;
                    if (!sampleCache.TryGetValue(distance, out sample)) {
                        if (!useSpline) {
                            if (distance > curve.Length) continue;
                            sample = curve.GetSampleAtDistance(distance);
                        } else {
                            float distOnSpline = intervalStart + distance;
                            //if (true) { //spline.isLoop) {
                                while (distOnSpline > spline.Length) {
                                    distOnSpline -= spline.Length;
                                }
                            //} else if (distOnSpline > spline.Length) {
                            //    continue;
                            //}
                            sample = spline.GetSampleAtDistance(distOnSpline);
                        }
                        sampleCache[distance] = sample;
                    }
                    bentVertices.Add(sample.GetBent(vert));
                }
                offset += source.Length;
            }                

            MeshUtility.Update(result,
                source.Mesh,
                triangles,
                bentVertices.Select(b => b.position),
                bentVertices.Select(b => b.normal),
                uv,
                uv2,
                uv3,
                uv4,
                uv5,
                uv6,
                uv7,
                uv8);
        }

        private void FillStretch() {
            var bentVertices = new List<MeshVertex>(source.Vertices.Count);
            sampleCache.Clear();
            // for each mesh vertex, we found its projection on the curve
            foreach (var vert in source.Vertices) {
                float distanceRate = source.Length == 0 ? 0 : Math.Abs(vert.position.x - source.MinX) / source.Length;
                CurveSample sample;
                if (!sampleCache.TryGetValue(distanceRate, out sample)) {
                    if (!useSpline) {
                        sample = curve.GetSampleAtDistance(curve.Length * distanceRate);
                    } else {
                        float intervalLength = intervalEnd == 0 ? spline.Length - intervalStart : intervalEnd - intervalStart;
                        float distOnSpline = intervalStart + intervalLength * distanceRate;
                        if(distOnSpline > spline.Length) {
                            distOnSpline = spline.Length;
                            Debug.Log("dist " + distOnSpline + " spline length " + spline.Length + " start " + intervalStart);
                        }

                        sample = spline.GetSampleAtDistance(distOnSpline);
                    }
                    sampleCache[distanceRate] = sample;
                }

                bentVertices.Add(sample.GetBent(vert));
            }

            MeshUtility.Update(result,
                source.Mesh,
                source.Triangles,
                bentVertices.Select(b => b.position),
                bentVertices.Select(b => b.normal));
            if (TryGetComponent(out MeshCollider collider)) {
                collider.sharedMesh = result;
            }
        }
    }
}