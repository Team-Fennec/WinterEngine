using log4net;
using System.Reflection;

namespace WinterEngine.Core;

public abstract class IHasLog<T> {
    protected static ILog log => LogManager.GetLogger(typeof(T));
}
