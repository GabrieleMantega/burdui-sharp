using System.IO;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Media;
using BurdUI;
using BurdUI.Utils;
namespace AvaloniaApplication1.Views;

public partial class MainView : Avalonia.Controls.UserControl
{
    public MainView()
    {
        InitializeComponent();

        BurdUiApp.Root = BuildTree3();
    }
    
    private View BuildTree()
    {
        // write the code building the view tree here...
        var container = new View();
        container.Bounds = new Rect(10, 10, 1300, 300);
        var child1 = new View();
        child1.Bounds = new Rect(25, 10, 100, 50);
        
        var child2 = new View();
        child2.Bounds = new Rect(150, 10, 100, 50);
        
        container.Children.Add(child1);
        container.Children.Add(child2);
        return container;
    }
    
    private View BuildTree2()
    {
        // write the code building the view tree here...
        var container = new View();
        container.Bounds = new Rect(10, 10, 300, 300);
        var child1 = new BurdUI.Button("Hello World");
        child1.Bounds = new Rect(25, 10, 100, 50);
        child1.Border = new Border(Color.FromRgb(0,0,255), Color.FromRgb(255,255,255), 3, 5);
        
        
        var child2 = new Button("Click me!");
        child2.Bounds = new Rect(150, 10, 100, 50);
        child2.Border = new Border(Color.FromRgb(255,0,255), Color.FromRgb(255,255,255), 3, 5);
        
        container.Children.Add(child1);
        container.Children.Add(child2);
        return container;
    }

    private View BuildTree3()
    {
        // write the code building the view tree here...
        var container = new VerticalLayoutPanel();
        container.Origin = VerticalStackOrigin.Bottom;
        container.Spacing = 10;
        container.Bounds = new Rect(10, 10, 300, 300);
        var child1 = new Button("Hello World");
        child1.Bounds = new Rect(25, 10, 100, 50);
        child1.Border = new Border(Color.FromRgb(0,0,255), Color.FromRgb(255,255,255), 3, 5);
        
        
        var child2 = new Button("Click me!");
        child2.Bounds = new Rect(150, 10, 100, 50);
        child2.Border = new Border(Color.FromRgb(255,0,255), Color.FromRgb(255,255,255), 3, 5);
        
        container.Children.Add(child1);
        container.Children.Add(child2);
        
        var serializer = new XmlSerializer(typeof(VerticalLayoutPanel));
        
        using (var file = File.Create("ui.xml"))
        {
            serializer.Serialize(file, container);
        }

        return container;
    }
}