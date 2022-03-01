using System;
using System.Collections.Generic;
using IxMilia.Dxf.Collections;

namespace IxMilia.Dxf.Entities
{
    public partial class DxfPolyline : IDxfItemInternal
    {
        #region IDxfItemInternal
        IEnumerable<DxfPointer> IDxfItemInternal.GetPointers()
        {
            foreach (var pointer in _vertices.Pointers)
            {
                yield return pointer;
            }

            yield return _seqendPointer;
        }
        #endregion

        /// <summary>
        /// Creates a new polyline entity with the specified vertices.  NOTE, at least 2 vertices must be specified.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <param name="vertices">The vertices to add.</param>
        public DxfPolyline(IEnumerable<DxfVertex> vertices)
            : this(vertices, new DxfSeqend())
        {
        }

        /// <summary>
        /// Creates a new polyline entity with the specified vertices.  NOTE, at least 2 vertices must be specified.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <param name="vertices">The vertices to add.</param>
        /// <param name="seqend">The end sequence entity.</param>
        public DxfPolyline(IEnumerable<DxfVertex> vertices, DxfSeqend seqend)
        {
            Seqend = seqend;
            foreach (var vertex in vertices)
            {
                Vertices.Add(vertex);
            }

            _vertices.ValidateCount();
        }

        public new double Elevation
        {
            get { return Location.Z; }
            set { Location = Location.WithUpdatedZ(value); }
        }

        private DxfPointerList<DxfVertex> _vertices = new DxfPointerList<DxfVertex>(2);
        private DxfPointer _seqendPointer = new DxfPointer(new DxfSeqend());

        public IList<DxfVertex> Vertices { get { return _vertices; } }

        public DxfSeqend Seqend
        {
            get { return (DxfSeqend)_seqendPointer.Item; }
            set { _seqendPointer.Item = value; }
        }

        protected override void AddValuePairs(List<DxfCodePair> pairs, DxfAcadVersion version, bool outputHandles, bool writeXData)
        {
            base.AddValuePairs(pairs, version, outputHandles, writeXData: false);
            var subclassMarker = Is3DPolyline || Is3DPolygonMesh ? "AcDb3dPolyline" : "AcDb2dPolyline";
            pairs.Add(new DxfCodePair(100, subclassMarker));
            if (version <= DxfAcadVersion.R13)
            {
                pairs.Add(new DxfCodePair(66, BoolShort(ContainsVertices)));
            }

            if (version >= DxfAcadVersion.R12)
            {
                pairs.Add(new DxfCodePair(10, Location.X));
                pairs.Add(new DxfCodePair(20, Location.Y));
                pairs.Add(new DxfCodePair(30, Location.Z));
            }

            if (Thickness != 0.0)
            {
                pairs.Add(new DxfCodePair(39, Thickness));
            }

            if (Flags != 0)
            {
                pairs.Add(new DxfCodePair(70, (short)Flags));
            }

            if (DefaultStartingWidth != 0.0)
            {
                pairs.Add(new DxfCodePair(40, DefaultStartingWidth));
            }

            if (DefaultEndingWidth != 0.0)
            {
                pairs.Add(new DxfCodePair(41, DefaultEndingWidth));
            }

            if (PolygonMeshMVertexCount != 0)
            {
                pairs.Add(new DxfCodePair(71, (short)PolygonMeshMVertexCount));
            }

            if (PolygonMeshNVertexCount != 0)
            {
                pairs.Add(new DxfCodePair(72, (short)PolygonMeshNVertexCount));
            }

            if (SmoothSurfaceMDensity != 0)
            {
                pairs.Add(new DxfCodePair(73, (short)SmoothSurfaceMDensity));
            }

            if (SmoothSurfaceNDensity != 0)
            {
                pairs.Add(new DxfCodePair(74, (short)SmoothSurfaceNDensity));
            }

            if (SurfaceType != DxfPolylineCurvedAndSmoothSurfaceType.None)
            {
                pairs.Add(new DxfCodePair(75, (short)SurfaceType));
            }

            if (Normal != DxfVector.ZAxis)
            {
                pairs.Add(new DxfCodePair(210, Normal.X));
                pairs.Add(new DxfCodePair(220, Normal.Y));
                pairs.Add(new DxfCodePair(230, Normal.Z));
            }

            if (writeXData)
            {
                DxfXData.AddValuePairs(XData, pairs, version, outputHandles);
            }
        }

        protected override void AddTrailingCodePairs(List<DxfCodePair> pairs, DxfAcadVersion version, bool outputHandles)
        {
            foreach (var vertex in Vertices)
            {
                vertex.Is3DPolylineVertex = this.Is3DPolyline;
                vertex.Is3DPolygonMesh = this.Is3DPolygonMesh;
                pairs.AddRange(vertex.GetValuePairs(version, outputHandles));
            }

            if (Seqend != null)
            {
                pairs.AddRange(Seqend.GetValuePairs(version, outputHandles));
            }
        }
        
        private DxfEntity VertexPairToEntity(DxfVertex vertex1, DxfVertex vertex2)
        {
            if (Math.Abs(vertex1.Bulge) <= 1e-10)
            {
                return new DxfLine(vertex1.Location, vertex2.Location);
            }

            // the segment between `vertex.Location` and `next.Location` is an arc
            if (DxfArc.TryCreateFromVertices(vertex1, vertex2, out var arc))
            {
                return arc;
            }
            else 
            {
                // fallback if points are too close / bulge is tiny
                return new DxfLine(vertex1.Location, vertex2.Location);
            }
        }

        /// <summary>
        /// Converts DxfPolyline into a collection of DxfArc and DxfLine entities
        /// </summary>
        public IEnumerable<DxfEntity> AsSimpleEntities()
        {
            int n = Vertices.Count;

            for (var i = 0; i < n - 1; i++)
            {
                var result = VertexPairToEntity(Vertices[i], Vertices[i + 1]);
                result.CopyCommonPropertiesFrom(this);
                yield return result;
            }

            if (IsClosed)
            {
                var result = VertexPairToEntity(Vertices[n - 1], Vertices[0]);
                result.CopyCommonPropertiesFrom(this);
                yield return result;
            }
        } 

        private IEnumerable<DxfPoint> VertexPairToBoundingPoints(DxfVertex vertex1, DxfVertex vertex2)
        {
            if (Math.Abs(vertex1.Bulge) <= 1e-10)
            {
                yield return vertex1.Location;
            }
            else
            {
                // the segment between `vertex.Location` and `next.Location` is an arc
                if (TryGetArcBoundingBox(vertex1, vertex2, out var bbox))
                {
                    yield return bbox.MinimumPoint;
                    yield return bbox.MaximumPoint;
                }
                else
                {
                    // fallback if points are too close / bulge is tiny
                    yield return vertex1.Location;
                }
            }
        }

        protected override IEnumerable<DxfPoint> GetExtentsPoints()
        {
            int n = Vertices.Count;

            for (var i = 0; i < n - 1; i++)
            {
                foreach (var point in VertexPairToBoundingPoints(Vertices[i], Vertices[i + 1])) yield return point;
            }

            if (IsClosed)
            {
                foreach (var point in VertexPairToBoundingPoints(Vertices[n - 1], Vertices[0])) yield return point;
            }
        }

        private static bool TryGetArcBoundingBox(DxfVertex v1, DxfVertex v2, out DxfBoundingBox bbox)
        {
            if (!DxfArc.TryCreateFromVertices(v1, v2, out var arc))
            {
                bbox = default(DxfBoundingBox);
                return false;
            }

            var boundingBox = arc.GetBoundingBox();
            if (!boundingBox.HasValue)
            {
                bbox = default(DxfBoundingBox);
                return false;
            }

            bbox = boundingBox.Value;
            return true;
        }
    }
}
