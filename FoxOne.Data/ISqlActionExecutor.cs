using System.Collections.Generic;

namespace FoxOne.Data
{
    public interface ISqlAction
    {
        string Name { get; }
        string Text { get; }
    }

    public interface ISqlActionExecutor
    {
        string Prefix { get; }

        string Execute(ISqlAction action, ISqlParameters inParams,IDictionary<string,object> outParams);
    }
}