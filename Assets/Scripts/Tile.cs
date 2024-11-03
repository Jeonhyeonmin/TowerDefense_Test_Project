using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsBuildTower
    {
        set; get;
    }

    private void Awake()
    {
        IsBuildTower = false;
    }
}
