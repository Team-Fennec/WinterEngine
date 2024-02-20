using System;

namespace WinterEngine.Debug;

public interface IConCommand {
    public string Command => "";
    public string Description => "";

    public void Exec(string[] args);
}
