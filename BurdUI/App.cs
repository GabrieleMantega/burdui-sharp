namespace BurdUI;

/// <summary>
/// This class represents a BurdUI-based application. It provides to the BurdUI
/// View Tree the raw information coming from the devices and manages the
/// Event loop. In a nutshell, this class contains the logic required to access
/// OS-level information about the screen output and device input.
/// In order to simplify our development, we do not access directly the OS information
/// but we use the WindowsForms representation (i.e., we pass through another UI toolkit). 
/// </summary>
public class App: Control
{
    /// <summary>
    /// This is the root of the BurdUI View Tree in the current application
    /// </summary>
    public View? Root { get; set; }
    
    public App()
    {
    }

    protected override void OnCreateControl()
    {
        
    }

    /// <summary>
    /// Paints the application in the WinForms window. Simulates the screen usage
    /// </summary>
    /// <param name="e">The WindowsForms event arguments (we need the graphics context)</param>
    protected override void OnPaint(PaintEventArgs e)
    {
        if (Root != null)
        {
            // we pass the graphics context down to our BurdUI View Tree
            this.Root.Paint(e.Graphics);
        }
    }
}