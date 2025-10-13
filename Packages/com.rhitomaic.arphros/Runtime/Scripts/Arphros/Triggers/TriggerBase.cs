using UnityEngine;

// Welcome to inheritance hell

public interface ITriggerEnter
{
    public void OnTriggerEnter(Collider other);
}

public interface ITriggerExit {
    public void OnTriggerExit(Collider other);
}

public interface ITriggerSerialize {
    public string OnTriggerSerialize();
}

public interface ITriggerDeserialize {
    public void OnTriggerDeserialize(string data);
}

public interface IPlaymodeListener
{
    public void OnPlay();
    public void OnPause();
    public void OnStop();
}