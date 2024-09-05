using UnityEngine;

public abstract class MenuWithView<TView> : Menu, IMenu  where TView: MonoBehaviour
{
    protected readonly TView _view;
    
    public override bool IsActive => _view.gameObject.activeSelf;

    protected MenuWithView(Transform parent, string menuViewResourceName)
    {
        _view = ResourcesLoader.InstantiateLoadComponent<TView>(menuViewResourceName);
        _view.transform.SetParent(parent, false);
    }

    public override void SetAsLastSibling()
    {
        _view.transform.SetAsLastSibling();
    }

    public override void SetMenuActive(bool state, bool affectCursor = true)
    {
        base.SetMenuActive(state, affectCursor);
        
        _view.gameObject.SetActive(state);
    }

    public void ChangeMenuActive(bool active)
    {
        _view.gameObject.SetActive(active);
    }

    public void ChangeMenuActive()
    {
        _view.gameObject.SetActive(!_view.gameObject.activeSelf);
    }
}