using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class Camera
{
    private static readonly FloatRect DEFAULT_VIEW_PARAMS =
        new FloatRect(GameCycle.GetInstance().GetScreenCenter().X / 2, GameCycle.GetInstance().GetScreenCenter().Y / 2, GameConfig.RenderWindowWidth, GameConfig.RenderWindowHeight);
    
    public View View { get; private set; }
    public GameObject? FocusObject { get; set; }

    private Vector2f FocusPosition
    {
        get
        {
            if (FocusObject != null)
            {
                return FocusObject.Shape.Position;
            }
            
            return GameCycle.GetInstance().GetScreenCenter();
        }
    }

    private bool _zoomedOut = true;
    
    public Camera() : this(DEFAULT_VIEW_PARAMS)
    { }
    
    public Camera(FloatRect viewRect)
    {
        View = new View(viewRect);
    }

    public void Update()
    {
        if (FocusObject == null && _zoomedOut)
            ResetView();

        View.Center = FocusPosition;
    }

    public void ZoomOut(float zoom)
    {
        _zoomedOut = true;
        
        View.Zoom(zoom);
    }

    private void ResetView()
    {
        View = new View(DEFAULT_VIEW_PARAMS);
        
        _zoomedOut = false;
    }
}