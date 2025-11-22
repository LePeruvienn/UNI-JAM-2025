using UnityEngine;

public class TestCallback : MonoBehaviour
{
    public void OnClapTest()
    {
        Debug.Log("[TestCallback] Reçu : Clap (UI ou clavier). Time=" + Time.time);
    }

    public void OnHiFiveTest()
    {
        Debug.Log("[TestCallback] Reçu : HiFive (UI ou clavier). Time=" + Time.time);
    }

    public void OnRiseHandsTest()
    {
        Debug.Log("[TestCallback] Reçu : RiseHands (UI ou clavier). Time=" + Time.time);
    }
}
