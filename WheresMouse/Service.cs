using Dalamud.Plugin.Services;
using Dalamud.Plugin;
using Dalamud.IoC;

namespace WheresMouse
{

    internal class Service
    {

        [PluginService] public IDalamudPluginInterface pi { get; private set; }
        [PluginService] public ICommandManager cm { get; private set; }
        [PluginService] public IClientState cs { get; private set; }
        [PluginService] public ICondition cd { get; private set; }
        [PluginService] public IPluginLog lo { get; private set; }

    }

}
