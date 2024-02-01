
public interface IDraggable
{
    bool CanBeDragged();
    void OnStartDrag();
    void OnDrag();
    void OnDrop();
}
