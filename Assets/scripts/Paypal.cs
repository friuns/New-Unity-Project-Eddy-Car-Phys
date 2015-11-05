using System.Collections.Generic;
using System.Linq;

public static class Paypal
{
    private static List<string> paypalItems = new List<string>();
    public static bool m_haveCar;
    public static bool haveCar { get { return bs.isDebug ? bs.settings.haveCar : m_haveCar; } } ////todo Paypal.Paypal:8 
    public static void SetPaypalItems(List<string> list)
    {
        paypalItems = list.Select(a=>a.Trim()).ToList();
        m_haveCar = paypalItems.Contains("car");
    }
}