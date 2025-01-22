using Agario.Game.Components;
using SFML.Graphics;

namespace Agario.Infrastructure;

public class Camera
{
    private static readonly FloatRect DEFAULT_VIEW_PARAMS =
        new FloatRect(GameCycle.GetInstance().GetScreenCenter().X / 2, GameCycle.GetInstance().GetScreenCenter().Y / 2, 1800, 900);
    
    private GameObject? _focusObject;
    public GameObject? FocusObject
    {
        get => _focusObject;
        set
        {
            if (value == null)
            {
                _focusObject = null;
                return;
            }
            
            if (_focusObject == value)
                return;
            
            if (_focusObject != null)
            {
                _focusObject.GetComponent<PlayerComponent>().SizeIncreased -= ZoomOut;
            }
        
            _focusObject = value;
            _focusObject.GetComponent<PlayerComponent>().SizeIncreased += ZoomOut;
        }
    }

    public View View;
    private bool _zoomedOut = true;
    
    public Camera() : this(DEFAULT_VIEW_PARAMS)
    { }
    
    public Camera(FloatRect viewRect)
    {
        View = new View(viewRect);
    }

    public void Update()
    {
        if (_focusObject != null)
        {
            View.Center = _focusObject.Shape.Position;
        }
        else
        {
            if (_zoomedOut) 
                ResetView();
            
            View.Center = GameCycle.GetInstance().GetScreenCenter();
        }
    }

    private void ZoomOut()
    {
        float zoom = FocusObject.GetComponent<PlayerComponent>().GetSizeModifier();

        _zoomedOut = true;
        
        View.Zoom(zoom);
    }

    private void ResetView()
    {
        View = new View(DEFAULT_VIEW_PARAMS);
        
        _zoomedOut = false;
    }
}