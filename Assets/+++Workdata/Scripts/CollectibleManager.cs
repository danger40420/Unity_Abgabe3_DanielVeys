using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    private int orbCount = 0;
    private int wispCount = 0;

    [SerializeField] private UIManager uiManager; 

    
    public void AddOrb()
    {
        orbCount++;
        uiManager.UpdateOrbText(orbCount); 

        if (orbCount >= 2)
        {
            uiManager.ShowPanelWin();
        }
    }

    
    public void AddWisp()
    {
        wispCount++;
        uiManager.UpdateWispText(wispCount); 
    }

    
    public void ResetCollectibles()
    {
        orbCount = 0;
        wispCount = 0;
        uiManager.UpdateOrbText(orbCount);
        uiManager.UpdateWispText(wispCount);
    }

    public int GetOrbCount() => orbCount;
    public int GetWispCount() => wispCount;
}