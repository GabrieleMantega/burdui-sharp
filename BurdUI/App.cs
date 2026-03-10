namespace BurdUI;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Input;


/// <summary>
/// This class represents a BurdUI-based application. It provides to the BurdUI
/// View Tree the raw information coming from the devices and manages the
/// Event loop. In a nutshell, this class contains the logic required to access
/// OS-level information about the screen output and device input.
/// In order to simplify our development, we do not access directly the OS information
/// but we use the Avalonia representation (i.e., we pass through another UI toolkit). 
/// </summary>

public class App : Control
{
    private RenderTargetBitmap _bitmap;
    private int _width = 800;
    private int _height = 600;
    private DrawingContext burdUIContext;
    
    // contesto logico per te (non grafico di Avalonia)
    public object? GraphicsContext { get; private set; }

    public View? Root { get; set; }

    public App()
    {
        // inizializza bitmap interna
        var size = new PixelSize(_width, _height);
        var dpi = new Vector(192, 192);

        _bitmap = new RenderTargetBitmap(size, dpi);
        burdUIContext = _bitmap.CreateDrawingContext();

        Focusable = true;

        // Eventi mouse/tastiera -> a te
        PointerPressed += (s, e) =>
        {
            var p = e.GetPosition(this);
            OnMouseDown(p.X, p.Y, e);
        };

        PointerMoved += (s, e) =>
        {
            var p = e.GetPosition(this);
            OnMouseMove(p.X, p.Y, e);
        };

        KeyDown += (s, e) =>
        {
            OnKey(e.Key);
        };

        this.SizeChanged += (s, e) =>
        {
            ResizeBitmapIfNeeded();
        };
    }

    // ==========================
    // EVENTI PASSATI A TE
    // ==========================

    protected virtual void OnMouseDown(double x, double y, PointerPressedEventArgs e)
    {
        // override se vuoi
    }

    protected virtual void OnMouseMove(double x, double y, PointerEventArgs e)
    {
        // override se vuoi
    }
    

    protected virtual void OnKey(Key key)
    {
        // override se vuoi
    }


    // ==========================
    // RENDER AVALONIA
    // ==========================

    public override void Render(DrawingContext ctx)
    {
        base.Render(ctx);
        
        // Chiama la tua paint logica
        Root?.Paint(burdUIContext);

        // disegna la bitmap interna sulla view
        ctx.DrawImage(
            _bitmap,
            sourceRect: new Rect(0, 0, _width, _height),
            destRect: new Rect(0, 0, Bounds.Width, Bounds.Height));
    }

    // ==========================
    // IL TUO METODO PAINT
    // ==========================
    //
    // Qui tu accedi alla bitmap e ci scrivi dentro i tuoi pixel
    // (o qualunque logica interna)
    // ==========================

   /* protected virtual void Paint()
    {
        // Esempio mini: riempio lo sfondo
        using var fb = _bitmap.Lock();
        unsafe
        {
            uint* ptr = (uint*)fb.Address;
            int count = _width * _height;
            for (int i = 0; i < count; i++)
                ptr[i] = 0xFF202020; // grigio scuro
        }

        // Qui puoi ottenere un puntatore ai pixel e implementare
        // il tuo sistema grafico custom (booleans, tile map, ecc.)
    }*/

    // ==========================
    // GESTIONE RESIZE
    // ==========================

    private void ResizeBitmapIfNeeded()
    {
        int newW = (int)Math.Max(1, Bounds.Width);
        int newH = (int)Math.Max(1, Bounds.Height);

        if (newW != _width || newH != _height)
        {
            _width = newW;
            _height = newH;

            var size = new PixelSize(_width, _height);
            var dpi = new Vector(96, 96);

            _bitmap = new RenderTargetBitmap(size, dpi);
            burdUIContext = _bitmap.CreateDrawingContext();
        }
    }
}