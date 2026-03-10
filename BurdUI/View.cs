using System.Globalization;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace BurdUI;
    
[Serializable]
public class View
{
    [XmlIgnore]
    public Rect Bounds { get; set; }
    
    [XmlArray("Children")]   
    [XmlArrayItem("Button", typeof(Button))]
    [XmlArrayItem("VerticalLayoutPanel", typeof(VerticalLayoutPanel))]

    public List<View> Children { get; internal set; }
    
    
    [XmlAttribute("Bounds")]
    public string BoundsAttr
    {
        get => $"{Bounds.TopLeft.X.ToString(CultureInfo.InvariantCulture)}," +
               $"{Bounds.TopLeft.Y.ToString(CultureInfo.InvariantCulture)}," +
               $"{Bounds.Width.ToString(CultureInfo.InvariantCulture)}," +
               $"{Bounds.Height.ToString(CultureInfo.InvariantCulture)}";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Bounds = new Rect();
                return;
            }

            var parts = value.Split(',');
            if (parts.Length != 4)
                throw new FormatException("Bounds must be 'x,y,width,height'.");

            int x  = int.Parse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
            int y  = int.Parse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
            int w  = int.Parse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            int h  = int.Parse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture);

            Bounds = new Rect(x, y, w, h);
        }
    }


    public View()
    {
        this.Children = new List<View>();
    }
    

    public virtual void Paint(DrawingContext ctx)
    {
        using (ctx.PushTransform(Matrix.CreateTranslation(Bounds.X, Bounds.Y)))
        {
            foreach (var child in Children)
            {
                child.Paint(ctx);
            }
        }    
        // Draw final border rectangle
        var pen = new Pen(Brushes.Red, 1);    
        ctx.DrawRectangle(pen, Bounds);
       
    }

    public void AddChild(View child)
    {
        this.Children.Add(child);
    }
}