// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// The contents of this file are automatically generated by a tool, and should not be directly modified.

using System;
using System.Collections.Generic;
using System.Linq;

namespace IxMilia.Dxf.Entities
{

    /// <summary>
    /// DxfUnderlay class
    /// </summary>
    public partial class DxfUnderlay : DxfEntity
    {
        public override DxfEntityType EntityType { get { return DxfEntityType.Underlay; } }
        protected override DxfAcadVersion MinVersion { get { return DxfAcadVersion.R2007; } }

        public uint ObjectHandle { get; set; }
        public DxfPoint InsertionPoint { get; set; }
        public double XScale { get; set; }
        public double YScale { get; set; }
        public double ZScale { get; set; }
        public double RotationAngle { get; set; }
        public DxfVector Normal { get; set; }
        public int Flags { get; set; }
        public short Contrast { get; set; }
        public short Fade { get; set; }
        private List<double> PointX { get; set; }
        private List<double> PointY { get; set; }

        // Flags flags

        public bool IsClippingOn
        {
            get { return DxfHelpers.GetFlag(Flags, 1); }
            set
            {
                var flags = Flags;
                DxfHelpers.SetFlag(value, ref flags, 1);
                Flags = flags;
            }
        }

        public bool IsUnderlayOn
        {
            get { return DxfHelpers.GetFlag(Flags, 2); }
            set
            {
                var flags = Flags;
                DxfHelpers.SetFlag(value, ref flags, 2);
                Flags = flags;
            }
        }

        public bool IsMonochrome
        {
            get { return DxfHelpers.GetFlag(Flags, 4); }
            set
            {
                var flags = Flags;
                DxfHelpers.SetFlag(value, ref flags, 4);
                Flags = flags;
            }
        }

        public bool AdjustForBackground
        {
            get { return DxfHelpers.GetFlag(Flags, 8); }
            set
            {
                var flags = Flags;
                DxfHelpers.SetFlag(value, ref flags, 8);
                Flags = flags;
            }
        }

        public bool IsClipInsideMode
        {
            get { return DxfHelpers.GetFlag(Flags, 16); }
            set
            {
                var flags = Flags;
                DxfHelpers.SetFlag(value, ref flags, 16);
                Flags = flags;
            }
        }

        internal DxfUnderlay()
            : base()
        {
        }

        protected DxfUnderlay(DxfUnderlay other)
            : base(other)
        {
            this.ObjectHandle = other.ObjectHandle;
            this.InsertionPoint = other.InsertionPoint;
            this.XScale = other.XScale;
            this.YScale = other.YScale;
            this.ZScale = other.ZScale;
            this.RotationAngle = other.RotationAngle;
            this.Normal = other.Normal;
            this.Flags = other.Flags;
            this.Contrast = other.Contrast;
            this.Fade = other.Fade;
            this.PointX = other.PointX;
            this.PointY = other.PointY;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.ObjectHandle = 0;
            this.InsertionPoint = DxfPoint.Origin;
            this.XScale = 1.0;
            this.YScale = 1.0;
            this.ZScale = 1.0;
            this.RotationAngle = 0.0;
            this.Normal = DxfVector.ZAxis;
            this.Flags = 0;
            this.Contrast = 100;
            this.Fade = 0;
            this.PointX = new List<double>();
            this.PointY = new List<double>();
        }

        protected override void AddValuePairs(List<DxfCodePair> pairs, DxfAcadVersion version, bool outputHandles)
        {
            base.AddValuePairs(pairs, version, outputHandles);
            pairs.Add(new DxfCodePair(100, "AcDbUnderlayReference"));
            pairs.Add(new DxfCodePair(340, UIntHandle(this.ObjectHandle)));
            pairs.Add(new DxfCodePair(10, InsertionPoint.X));
            pairs.Add(new DxfCodePair(20, InsertionPoint.Y));
            pairs.Add(new DxfCodePair(30, InsertionPoint.Z));
            pairs.Add(new DxfCodePair(41, (this.XScale)));
            pairs.Add(new DxfCodePair(42, (this.YScale)));
            pairs.Add(new DxfCodePair(43, (this.ZScale)));
            pairs.Add(new DxfCodePair(50, (this.RotationAngle)));
            pairs.Add(new DxfCodePair(210, Normal.X));
            pairs.Add(new DxfCodePair(220, Normal.Y));
            pairs.Add(new DxfCodePair(230, Normal.Z));
            pairs.Add(new DxfCodePair(280, (short)(this.Flags)));
            pairs.Add(new DxfCodePair(281, (this.Contrast)));
            pairs.Add(new DxfCodePair(282, (this.Fade)));
            foreach (var item in BoundaryPoints)
            {
                pairs.Add(new DxfCodePair(11, item.X));
                pairs.Add(new DxfCodePair(12, item.Y));
            }

        }

        internal override bool TrySetPair(DxfCodePair pair)
        {
            switch (pair.Code)
            {
                case 10:
                    this.InsertionPoint.X = pair.DoubleValue;
                    break;
                case 20:
                    this.InsertionPoint.Y = pair.DoubleValue;
                    break;
                case 30:
                    this.InsertionPoint.Z = pair.DoubleValue;
                    break;
                case 11:
                    this.PointX.Add((pair.DoubleValue));
                    break;
                case 21:
                    this.PointY.Add((pair.DoubleValue));
                    break;
                case 41:
                    this.XScale = (pair.DoubleValue);
                    break;
                case 42:
                    this.YScale = (pair.DoubleValue);
                    break;
                case 43:
                    this.ZScale = (pair.DoubleValue);
                    break;
                case 50:
                    this.RotationAngle = (pair.DoubleValue);
                    break;
                case 210:
                    this.Normal.X = pair.DoubleValue;
                    break;
                case 220:
                    this.Normal.Y = pair.DoubleValue;
                    break;
                case 230:
                    this.Normal.Z = pair.DoubleValue;
                    break;
                case 280:
                    this.Flags = (int)(pair.ShortValue);
                    break;
                case 281:
                    this.Contrast = (pair.ShortValue);
                    break;
                case 282:
                    this.Fade = (pair.ShortValue);
                    break;
                case 340:
                    this.ObjectHandle = UIntHandle(pair.StringValue);
                    break;
                default:
                    return base.TrySetPair(pair);
            }

            return true;
        }
    }

}
