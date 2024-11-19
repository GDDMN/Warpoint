using System.Collections.Generic;

public class UIMainConteiner : MonoSingleton<UIMainConteiner>
{
    private List<UIBaseWindow> _allWindows = new List<UIBaseWindow>();
    
    public void Initialize()
    {
        _allWindows.Clear();

        GetAllWindows();
    }

    private void GetAllWindows()
    {
        var childCount = gameObject.transform.childCount;

        for (int i = 0; i < childCount; i++)
            AddWindowInList(i);
    }

    private void AddWindowInList(int index)
    {
        var childObject = transform.GetChild(index).gameObject;
        var childBaseWindow = childObject.GetComponent<UIBaseWindow>();

        _allWindows.Add(childBaseWindow);
    }

    public T GetWindowByType<T>() where T : UIBaseWindow 
    {
        return (T)_allWindows.Find(wnd => wnd.GetType() == typeof(T));
    }
}
