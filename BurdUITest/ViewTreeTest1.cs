namespace BurdUITest;
using BurdUI;

/// <summary>
/// First test for the BurdUI package
/// </summary>
public partial class ViewTreeTest1 : Form
{
    public ViewTreeTest1()
    {
        InitializeComponent();
        
        this.Text = "Burdui WinForms Demo"; this.ClientSize = new Size(900, 600);
        var host = new App{ Dock = DockStyle.Fill };
        host.Root = BuildTree();
        this.Controls.Add(host);
    }

    private View BuildTree()
    {
        // write the code building the view tree here...
        return new View();
    }
    
    
}