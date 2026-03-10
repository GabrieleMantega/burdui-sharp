using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Media;

namespace BurdUI.Utils
{
    public class AlignedText : IXmlSerializable
    {
        public enum VerticalTextAlignment
        {
            Top,
            Middle,
            Bottom
        }

        public enum HorizontalTextAlignment
        {
            Left,
            Center,
            Right
        }

        [XmlAttribute]
        public string Value { get; set; } = "";

        [XmlAttribute]
        public VerticalTextAlignment VerticalAlignment { get; set; } = VerticalTextAlignment.Top;

        [XmlAttribute]
        public HorizontalTextAlignment HorizontalAlignment { get; set; } = HorizontalTextAlignment.Left;

        public Color Color { get; set; } = Color.FromRgb(0,0,0);

        [XmlIgnore]
        public Typeface Font { get; set; } = new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal);
        
        public double FontSize { get; set; } = 12;

        public AlignedText() { }

        public AlignedText(string value)
        {
            Value = value;
        }

        public AlignedText(
            string value, Typeface font, Color color,
            HorizontalTextAlignment hAlign = HorizontalTextAlignment.Left,
            VerticalTextAlignment vAlign = VerticalTextAlignment.Top)
        {
            Value = value;
            Font = font;
            Color = color;
            HorizontalAlignment = hAlign;
            VerticalAlignment = vAlign;
        }

        /// <summary>
        /// Draws the aligned text inside the given rectangle.
        /// </summary>
        public void Draw(DrawingContext g, Rect bounds)
        {
            var formatted = new FormattedText(
                Value,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                Font,
                FontSize,
                new SolidColorBrush(Color));
            
            double x = bounds.X;
            double y = bounds.Y;

            // Horizontal alignment
            switch (HorizontalAlignment)
            {
                case HorizontalTextAlignment.Left:
                    x = bounds.Left;
                    break;

                case HorizontalTextAlignment.Center:
                    x = bounds.Left + (bounds.Width - formatted.Width) / 2;
                    break;

                case HorizontalTextAlignment.Right:
                    x = bounds.Right - formatted.Width;
                    break;
            }

            // Vertical alignment
            switch (VerticalAlignment)
            {
                case VerticalTextAlignment.Top:
                    y = bounds.Top;
                    break;

                case VerticalTextAlignment.Middle:
                    y = bounds.Top + (bounds.Height - formatted.Height) / 2;
                    break;

                case VerticalTextAlignment.Bottom:
                    y = bounds.Bottom - formatted.Height;
                    break;
            }

            g.DrawText(formatted, new Point(x, y));
        }

        // ------------------------------
        // IXmlSerializable implementation
        // ------------------------------

        public XmlSchema? GetSchema() => null;

        /// <summary>
        /// Writes this object as attributes:
        /// vertical, horizontal, value, color (#RRGGBB), font (FontConverter invariant).
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("vertical", VerticalAlignment.ToString());
            writer.WriteAttributeString("horizontal", HorizontalAlignment.ToString());
            writer.WriteAttributeString("value", Value ?? string.Empty);
            writer.WriteAttributeString("color", ColorToHexRgb(Color));

            // Use invariant string for reliable round-tripping regardless of locale.
            
            
            string fontStr = SerializeTypeface(Font);
            writer.WriteAttributeString("font", fontStr);
            
            writer.WriteAttributeString("font-size", FontSize.ToString(CultureInfo.InvariantCulture));
        }
        
        

        /// <summary>
        /// Reads attributes: vertical, horizontal, value, color (#RRGGBB or #AARRGGBB), font.
        /// Missing or invalid values fall back to sensible defaults.
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToAttribute("vertical"))
            {
                if (Enum.TryParse(reader.Value, true, out VerticalTextAlignment v))
                    VerticalAlignment = v;
            }

            if (reader.MoveToAttribute("horizontal"))
            {
                if (Enum.TryParse(reader.Value, true, out HorizontalTextAlignment h))
                    HorizontalAlignment = h;
            }

            if (reader.MoveToAttribute("value"))
            {
                Value = reader.Value ?? string.Empty;
            }

            if (reader.MoveToAttribute("color"))
            {
                var parsed = TryParseHexColor(reader.Value, out var c);
                Color = parsed ? c : Color.FromRgb(0,0,0);
            }

            if (reader.MoveToAttribute("font"))
            {
                try
                {
                    if (reader.Value != null) Font = DeserializeTypeface(reader.Value);
                }
                catch
                {
                    Font = new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal);
                }
            }

            if (reader.MoveToAttribute("font-size"))
            {
                var parsed = int.TryParse(reader.Value, out var f);
                FontSize = parsed ? f : FontSize;
            }

            // Move back to the element and consume it properly.
            reader.MoveToElement();

            if (reader.IsEmptyElement)
            {
                reader.Read(); // <AlignedText ... />
            }
            else
            {
                // <AlignedText ...> ... </AlignedText>
                reader.ReadStartElement();
                // No inner content expected; skip anything unexpected safely
                while (reader.NodeType != XmlNodeType.EndElement && !reader.EOF)
                {
                    reader.Skip();
                }
                if (!reader.EOF) reader.ReadEndElement();
            }
        }

        // ------------------------------
        // Helpers
        // ------------------------------

        /// <summary>
        /// Returns #RRGGBB (no alpha) uppercase.
        /// </summary>
        public static string ColorToHexRgb(Color color) =>
            $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        /// <summary>
        /// Parses #RRGGBB or #AARRGGBB. If alpha present, it will be applied.
        /// Returns false on invalid input.
        /// </summary>
        public static bool TryParseHexColor(string? text, out Color color)
        {
            color = Avalonia.Media.Color.FromRgb(0,0,0);
            if (string.IsNullOrWhiteSpace(text)) return false;

            var t = text.Trim();
            if (t.StartsWith("#", StringComparison.Ordinal)) t = t.Substring(1);

            if (t.Length == 6)
            {
                if (byte.TryParse(t.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) &&
                    byte.TryParse(t.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) &&
                    byte.TryParse(t.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                {
                    color = Color.FromRgb(r, g, b);
                    return true;
                }
            }
            else if (t.Length == 8)
            {
                if (byte.TryParse(t.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var a) &&
                    byte.TryParse(t.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) &&
                    byte.TryParse(t.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) &&
                    byte.TryParse(t.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                {
                    color = Color.FromArgb(a, r, g, b);
                    return true;
                }
            }

            return false;
        }
        
        private static Typeface DeserializeTypeface(string value)
        {
            var parts = value.Split(';');

            return new Typeface(
                new FontFamily(parts[0]),
                Enum.Parse<FontStyle>(parts[1]),
                Enum.Parse<FontWeight>(parts[2]),
                Enum.Parse<FontStretch>(parts[3])
            );
        }
        
        private static string SerializeTypeface(Typeface typeface)
        {
            return $"{typeface.FontFamily.Name};{typeface.Style};{typeface.Weight};{typeface.Stretch}";
        }
    }
}