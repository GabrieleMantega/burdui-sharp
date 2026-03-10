
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using Avalonia;
using Avalonia.Media;
using System.Xml.Serialization;

namespace BurdUI.Utils
{
    public class Border : IXmlSerializable
    {
        public Color Color { get; set; } = Color.FromRgb(0,0,0);

        public Color BackgroudColor { get; set; } = Color.FromRgb(255,255,255);
        
        [XmlAttribute]
        public float StrokeThickness { get; set; } = 2f;

        [XmlAttribute]
        public int CornerRadius { get; set; } = 10;

        public Border() { }

        public Border(Color color, Color background, float thickness, int cornerRadius = 10)
        {
            Color = color;
            BackgroudColor =  background;
            StrokeThickness = thickness;
            CornerRadius = cornerRadius;
        }

        /// <summary>
        /// Draws a rounded rectangle border on the provided graphics context.
        /// </summary>
        public void Draw(DrawingContext g, Rect rect)
        {
            var pen = new Pen(new SolidColorBrush(Color), StrokeThickness);
            g.DrawRectangle(pen, rect, CornerRadius);
        }

        public void Fill(DrawingContext g, Rect rect)
        {
            g.FillRectangle(new SolidColorBrush(BackgroudColor), rect, CornerRadius);
        }
        

        // ------------------------------
        // IXmlSerializable implementation
        // ------------------------------

        public XmlSchema? GetSchema() => null;

        /// <summary>
        /// Writes attributes: color (#RRGGBB), strokeThickness (float), cornerRadius (int).
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("color", AlignedText.ColorToHexRgb(Color));
            writer.WriteAttributeString("background", AlignedText.ColorToHexRgb(BackgroudColor));
            writer.WriteAttributeString("strokeThickness", StrokeThickness.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("cornerRadius", CornerRadius.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Reads attributes and falls back to defaults if missing/invalid.
        /// Expects: color (#RRGGBB or #AARRGGBB), strokeThickness (float), cornerRadius (int).
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToAttribute("color"))
            {
                if (AlignedText.TryParseHexColor(reader.Value, out var c))
                    Color = c;
                else
                    Color = Color.FromRgb(0,0,0);
            }
            
            if (reader.MoveToAttribute("background"))
            {
                if (AlignedText.TryParseHexColor(reader.Value, out var c))
                    BackgroudColor = c;
                else
                    BackgroudColor = Color.FromRgb(255,255,255);
            }

            if (reader.MoveToAttribute("strokeThickness"))
            {
                if (float.TryParse(reader.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var t))
                    StrokeThickness = t;
            }

            if (reader.MoveToAttribute("cornerRadius"))
            {
                if (int.TryParse(reader.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var r))
                    CornerRadius = r;
            }

            // Return to the element and consume it
            reader.MoveToElement();

            if (reader.IsEmptyElement)
            {
                // <Border ... />
                reader.Read();
            }
            else
            {
                // <Border ...> ... </Border> — no inner content expected; skip safely
                reader.ReadStartElement();
                while (reader.NodeType != XmlNodeType.EndElement && !reader.EOF)
                    reader.Skip();
                if (!reader.EOF) reader.ReadEndElement();
            }
        }
        
    }
}