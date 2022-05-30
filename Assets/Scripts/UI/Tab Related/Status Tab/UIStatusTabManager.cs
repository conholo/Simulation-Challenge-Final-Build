using UnityEngine;

public sealed class UIStatusTabManager : Tab
{
    [SerializeField] private UIStatusViewer _statusViewer;

    public void Initialize()
    {
        _statusViewer.InitializeViewer();
    }
}