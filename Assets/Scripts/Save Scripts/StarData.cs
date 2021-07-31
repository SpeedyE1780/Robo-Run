[System.Serializable]
public class StarData
{
    public string Name;
    public bool PickedUp;

    public StarData(string n , bool picked = false)
    {
        Name = n;
        PickedUp = picked;
    }
}