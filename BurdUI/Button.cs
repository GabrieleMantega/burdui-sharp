using System.Xml.Serialization;
using Avalonia.Media;
using BurdUI.Utils;

namespace BurdUI;

[Serializable]
public class Button : View
{
    
    public Border? Border { get; set; }
    public AlignedText? Text { get; set; }
    


    public Button()
    {
        
    }
    public Button(string label = "")
    {
        this.Text = new AlignedText(label)
        {
            HorizontalAlignment = AlignedText.HorizontalTextAlignment.Center,
            VerticalAlignment = AlignedText.VerticalTextAlignment.Middle
        };
    }
    public override void Paint(DrawingContext g)
    {
        
        this.Border?.Fill(g, this.Bounds);
        this.Text?.Draw(g, this.Bounds);
        this.Border?.Draw(g, this.Bounds);
    }
}