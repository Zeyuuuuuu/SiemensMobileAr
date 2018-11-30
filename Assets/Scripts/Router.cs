public class RouterInfo
{
    public string _id;
    public string _name;
    public bool _signalHide;

    public RouterInfo(string id, string name)
    {
        _id = id;
        _name = name;
        _signalHide = false;
    }

}