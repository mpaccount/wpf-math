﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WpfMath.Rendering.Transformations;

namespace WpfMath.Rendering
{
    /// <summary>A renderer that renders the elements to a provided <see cref="GeometryGroup"/> instance.</summary>
    public class GeometryRenderer : IElementRenderer
    {
        private readonly double _scale;
        private readonly GeometryGroup _geometry;

        public GeometryRenderer(double scale, GeometryGroup geometry)
        {
            _scale = scale;
            _geometry = geometry;
        }

        public void RenderElement(Box box, double x, double y) => box.RenderTo(this, x, y);

        public void RenderGlyphRun(Func<double, GlyphRun> scaledGlyphFactory, double x, double y, Brush foreground)
        {
            var glyph = scaledGlyphFactory(_scale);
            var glyphGeometry = glyph.BuildGeometry();
            _geometry.Children.Add(glyphGeometry);
        }

        public void RenderRectangle(Rect rectangle, Brush foreground)
        {
            var rectangleGeometry = new RectangleGeometry(GeometryHelper.ScaleRectangle(_scale, rectangle));
            _geometry.Children.Add(rectangleGeometry);
        }

        public void RenderTransformed(Box box, Transformation[] transforms, double x, double y)
        {
            var group = new GeometryGroup();
            var scaledTransforms = transforms.Select(t => t.Scale(_scale));
            ApplyTransformations(scaledTransforms, group);
            var nestedRenderer = new GeometryRenderer(_scale, group);
            nestedRenderer.RenderElement(box, x, y);
            _geometry.Children.Add(group);
        }

        private static void ApplyTransformations(IEnumerable<Transformation> transformations, GeometryGroup geometry)
        {
            foreach (var transformation in transformations)
            {
                switch (transformation.Kind)
                {
                    case TransformationKind.Translate:
                        var tt = (Transformation.Translate) transformation;
                        geometry.Transform.Value.Translate(tt.X, tt.Y);
                        break;
                    case TransformationKind.Rotate:
                        var rt = (Transformation.Rotate) transformation;
                        geometry.Transform.Value.Rotate(rt.RotationDegrees);
                        break;
                    default:
                        throw new NotSupportedException(
                            $"Unknown {nameof(Transformation)} kind: {transformation.Kind}");
                }
            }
        }
    }
}
