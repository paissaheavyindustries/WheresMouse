using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.IoC;
using Dalamud.Plugin;
using ImGuiNET;
using System.Numerics;

namespace WheresMouse
{

    public sealed class Plugin : IDalamudPlugin
    {

        public string Name => "Where's Mouse";

        private DalamudPluginInterface _pi { get; init; }
        private CommandManager _cm { get; init; }
        private ClientState _cs { get; init; }
        private Condition _cd { get; init; }

        private Wherenator _where = new Wherenator();
        private bool _configOpen = false;
        private Config _cfg = new Config();

        private bool _cfgEnabled;        
        private bool _cfgOnlyShowInCombat;
        private bool _cfgDrawIndicatorCircle;
        private bool _cfgDrawIndicatorCardinal;
        private bool _cfgDrawIndicatorIntercardinal;
        private int _cfgIndicatorCircleRadius;
        private int _cfgIndicatorCardinalThickness;
        private int _cfgIndicatorIntercardinalThickness;
        private Vector3 _cfgIndicatorCircleColor;
        private Vector3 _cfgIndicatorCardinalColor;
        private Vector3 _cfgIndicatorIntercardinalColor;
        private Vector2 _cfgDistanceHysteresis;
        private float _cfgDistanceDecayFactor;
        private float _cfgActiveDecayFactor;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            [RequiredVersion("1.0")] ClientState clientState,
            [RequiredVersion("1.0")] Framework framework,
            [RequiredVersion("1.0")] GameGui gameGui,
            [RequiredVersion("1.0")] Condition condition
        ) {
            _pi = pluginInterface;
            _cm = commandManager;
            _cs = clientState;
            _cd = condition;            
            _cfg = _pi.GetPluginConfig() as Config ?? new Config();
            _cfgEnabled = _cfg.Enabled;
            _cfgOnlyShowInCombat = _cfg.OnlyShowInCombat;
            _cfgDrawIndicatorCircle = _cfg.DrawIndicatorCircle;
            _cfgDrawIndicatorCardinal = _cfg.DrawIndicatorCardinal;
            _cfgDrawIndicatorIntercardinal = _cfg.DrawIndicatorIntercardinal;
            _cfgIndicatorCircleRadius = _cfg.IndicatorCircleRadius;
            _cfgIndicatorCardinalThickness = _cfg.IndicatorCardinalThickness;
            _cfgIndicatorIntercardinalThickness = _cfg.IndicatorIntercardinalThickness;
            _cfgIndicatorCircleColor = _cfg.IndicatorCircleColor;
            _cfgIndicatorCardinalColor = _cfg.IndicatorCardinalColor;
            _cfgIndicatorIntercardinalColor = _cfg.IndicatorIntercardinalColor;
            _cfgDistanceHysteresis = _cfg.DistanceHysteresis;
            _cfgDistanceDecayFactor = _cfg.DistanceDecayFactor;
            _cfgActiveDecayFactor = _cfg.ActiveDecayFactor;
            _pi.UiBuilder.Draw += DrawUI;
            _pi.UiBuilder.OpenConfigUi += DrawConfigUI;
            _cm.AddHandler("/wheremouse", new CommandInfo(OnCommand)
            {
                HelpMessage = "Open Where's Mouse configuration"
            });
        }

        public void Dispose()
        {
            _pi.UiBuilder.Draw -= DrawUI;
            _pi.UiBuilder.OpenConfigUi -= DrawConfigUI;
            _cm.RemoveHandler("/wheremouse");
        }

        private void OnCommand(string command, string args)
        {
            _cfgEnabled = _cfg.Enabled;
            _cfgOnlyShowInCombat = _cfg.OnlyShowInCombat;
            _cfgDrawIndicatorCircle = _cfg.DrawIndicatorCircle;
            _cfgDrawIndicatorCardinal = _cfg.DrawIndicatorCardinal;
            _cfgDrawIndicatorIntercardinal = _cfg.DrawIndicatorIntercardinal;
            _cfgIndicatorCircleRadius = _cfg.IndicatorCircleRadius;
            _cfgIndicatorCardinalThickness = _cfg.IndicatorCardinalThickness;
            _cfgIndicatorIntercardinalThickness = _cfg.IndicatorIntercardinalThickness;
            _cfgIndicatorCircleColor = _cfg.IndicatorCircleColor;
            _cfgIndicatorCardinalColor = _cfg.IndicatorCardinalColor;
            _cfgIndicatorIntercardinalColor = _cfg.IndicatorIntercardinalColor;
            _cfgDistanceHysteresis = _cfg.DistanceHysteresis;
            _cfgDistanceDecayFactor = _cfg.DistanceDecayFactor;
            _cfgActiveDecayFactor = _cfg.ActiveDecayFactor;
            _configOpen = true;
        }

        private void DrawUI()
        {
            Vector2 pos = ImGui.GetMousePos();
            _where.distDecayFactorActive = _cfgActiveDecayFactor;
            _where.distDecayFactorInactive = _cfgDistanceDecayFactor;
            if (_cfgDistanceHysteresis.X > _cfgDistanceHysteresis.Y)
            {
                _cfgDistanceHysteresis.X = _cfgDistanceHysteresis.Y;
            }
            if (_cfgDistanceHysteresis.Y < _cfgDistanceHysteresis.X)
            {
                _cfgDistanceHysteresis.Y = _cfgDistanceHysteresis.X;
            }
            _where.distHystMin = _cfgDistanceHysteresis.X;
            _where.distHystMax = _cfgDistanceHysteresis.Y;
            _where.Update(pos);
            if (_configOpen == true)
            {
                ImGui.SetNextWindowSize(new Vector2(300, 500), ImGuiCond.FirstUseEver);
                ImGui.Begin("Where's Mouse?", ref _configOpen);
                ImGui.Checkbox("Indicator enabled", ref _cfgEnabled);
                ImGui.Checkbox("Only show in combat", ref _cfgOnlyShowInCombat);
                ImGui.Separator();
                ImGui.DragFloat2("Distance accumulation hysteresis", ref _cfgDistanceHysteresis, 100.0f, 100, 10000);
                ImGui.DragFloat("Distance accumulation decay factor", ref _cfgDistanceDecayFactor, 0.01f, 0.1f, 0.99f);
                ImGui.DragFloat("Active indicator decay factor", ref _cfgActiveDecayFactor, 0.01f, 0.1f, 0.99f);
                ImGui.Separator();
                ImGui.Checkbox("Draw circle", ref _cfgDrawIndicatorCircle);
                ImGui.DragInt("Circle radius in pixels", ref _cfgIndicatorCircleRadius, 1.0f, 10, 300);
                ImGui.ColorEdit3("Circle color", ref _cfgIndicatorCircleColor, ImGuiColorEditFlags.NoInputs);
                ImGui.Separator();
                ImGui.Checkbox("Draw cardinal lines", ref _cfgDrawIndicatorCardinal);
                ImGui.DragInt("Cardinal line thickness in pixels", ref _cfgIndicatorCardinalThickness, 1.0f, 5, 50);
                ImGui.ColorEdit3("Cardinal line color", ref _cfgIndicatorCardinalColor, ImGuiColorEditFlags.NoInputs);
                ImGui.Separator();
                ImGui.Checkbox("Draw intercardinal lines", ref _cfgDrawIndicatorIntercardinal);
                ImGui.DragInt("Intercardinal line thickness in pixels", ref _cfgIndicatorIntercardinalThickness, 1.0f, 5, 50);
                ImGui.ColorEdit3("Intercardinal line color", ref _cfgIndicatorIntercardinalColor, ImGuiColorEditFlags.NoInputs);
                ImGui.Separator();
                if (ImGui.Button("Save changes"))
                {
                    SaveConfig();
                }
                if (ImGui.Button("Revert changes"))
                {
                    RevertConfig(_cfg);
                }
                if (ImGui.Button("Restore defaults"))
                {
                    RestoreConfig();
                }
                if (ImGui.Button("Close"))
                {
                    _configOpen = false;
                }
                ImGui.End();
            }
            if (_cfgEnabled == false)
            {
                return;
            }
            if (_cfgOnlyShowInCombat)
            {
                if (!_cd[ConditionFlag.InCombat])
                {
                    return;
                }
            }
            if (_where.active == false)
            {
                return;
            }
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Vector2(0, 0));
            ImGui.Begin("WhereIndicator",
                ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);
            ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
            float opacity = (float)(_where.activePower / 100.0);
            if (_cfgDrawIndicatorCardinal == true)
            {
                float thickness = (float)(_cfgIndicatorCardinalThickness * (_where.activePower / 100.0)) / 2.0f;
                ImGui.GetWindowDrawList().AddQuadFilled(
                    new Vector2(pos.X - thickness, pos.Y - 20000),
                    new Vector2(pos.X + thickness, pos.Y - 20000),
                    new Vector2(pos.X + thickness, pos.Y + 20000),
                    new Vector2(pos.X - thickness, pos.Y + 20000),
                    ImGui.GetColorU32(new Vector4(
                        _cfgIndicatorCardinalColor.X,
                        _cfgIndicatorCardinalColor.Y,
                        _cfgIndicatorCardinalColor.Z,
                        opacity
                    ))
                );
                ImGui.GetWindowDrawList().AddQuadFilled(
                    new Vector2(pos.X - 20000, pos.Y - thickness),
                    new Vector2(pos.X - 20000, pos.Y + thickness),
                    new Vector2(pos.X + 20000, pos.Y + thickness),
                    new Vector2(pos.X + 20000, pos.Y - thickness),
                    ImGui.GetColorU32(new Vector4(
                        _cfgIndicatorCardinalColor.X,
                        _cfgIndicatorCardinalColor.Y,
                        _cfgIndicatorCardinalColor.Z,
                        opacity
                    ))
                );
            }
            if (_cfgDrawIndicatorIntercardinal == true)
            {
                float thickness = (float)(_cfgIndicatorIntercardinalThickness * (_where.activePower / 100.0)) / 2.0f;
                ImGui.GetWindowDrawList().AddQuadFilled(
                    new Vector2(pos.X - 20000 - thickness, pos.Y - 20000 + thickness),
                    new Vector2(pos.X - 20000 + thickness, pos.Y - 20000 - thickness),
                    new Vector2(pos.X + 20000 + thickness, pos.Y + 20000 - thickness),
                    new Vector2(pos.X + 20000 - thickness, pos.Y + 20000 + thickness),
                    ImGui.GetColorU32(new Vector4(
                        _cfgIndicatorIntercardinalColor.X,
                        _cfgIndicatorIntercardinalColor.Y,
                        _cfgIndicatorIntercardinalColor.Z,
                        opacity
                    ))
                );
                ImGui.GetWindowDrawList().AddQuadFilled(
                    new Vector2(pos.X - 20000 - thickness, pos.Y + 20000 - thickness),
                    new Vector2(pos.X - 20000 + thickness, pos.Y + 20000 + thickness),
                    new Vector2(pos.X + 20000 + thickness, pos.Y - 20000 + thickness),
                    new Vector2(pos.X + 20000 - thickness, pos.Y - 20000 - thickness),
                    ImGui.GetColorU32(new Vector4(
                        _cfgIndicatorIntercardinalColor.X,
                        _cfgIndicatorIntercardinalColor.Y,
                        _cfgIndicatorIntercardinalColor.Z,
                        opacity
                    ))
                );
            }
            if (_cfgDrawIndicatorCircle == true)
            {
                ImGui.GetWindowDrawList().AddCircleFilled(
                    new Vector2(pos.X, pos.Y),
                    (float)(_cfgIndicatorCircleRadius * (_where.activePower / 100.0)),
                    ImGui.GetColorU32(new Vector4(
                        _cfgIndicatorCircleColor.X,
                        _cfgIndicatorCircleColor.Y,
                        _cfgIndicatorCircleColor.Z,
                        opacity
                    )),
                    100
                );
            }
            ImGui.End();
            ImGui.PopStyleVar();
        }

        private void DrawConfigUI()
        {
            _configOpen = true;
        }

        private void SaveConfig()
        {
            _cfg.Enabled = _cfgEnabled;
            _cfg.OnlyShowInCombat = _cfgOnlyShowInCombat;
            _cfg.DrawIndicatorCircle = _cfgDrawIndicatorCircle;
            _cfg.DrawIndicatorCardinal = _cfgDrawIndicatorCardinal;
            _cfg.DrawIndicatorIntercardinal = _cfgDrawIndicatorIntercardinal;
            _cfg.IndicatorCircleRadius = _cfgIndicatorCircleRadius;
            _cfg.IndicatorCardinalThickness = _cfgIndicatorCardinalThickness;
            _cfg.IndicatorIntercardinalThickness = _cfgIndicatorIntercardinalThickness;
            _cfg.IndicatorCircleColor = _cfgIndicatorCircleColor;
            _cfg.IndicatorCardinalColor = _cfgIndicatorCardinalColor;
            _cfg.IndicatorIntercardinalColor = _cfgIndicatorIntercardinalColor;
            _cfg.DistanceHysteresis = _cfgDistanceHysteresis;
            _cfg.DistanceDecayFactor = _cfgDistanceDecayFactor;
            _cfg.ActiveDecayFactor = _cfgActiveDecayFactor;
            _pi.SavePluginConfig(_cfg);
        }

        private void RevertConfig(Config cfg)
        {
            _cfgEnabled = cfg.Enabled;
            _cfgOnlyShowInCombat = cfg.OnlyShowInCombat;
            _cfgDrawIndicatorCircle = cfg.DrawIndicatorCircle;
            _cfgDrawIndicatorCardinal = cfg.DrawIndicatorCardinal;
            _cfgDrawIndicatorIntercardinal = cfg.DrawIndicatorIntercardinal;
            _cfgIndicatorCircleRadius = cfg.IndicatorCircleRadius;
            _cfgIndicatorCardinalThickness = cfg.IndicatorCardinalThickness;
            _cfgIndicatorIntercardinalThickness = cfg.IndicatorIntercardinalThickness;
            _cfgIndicatorCircleColor = cfg.IndicatorCircleColor;
            _cfgIndicatorCardinalColor = cfg.IndicatorCardinalColor;
            _cfgIndicatorIntercardinalColor = cfg.IndicatorIntercardinalColor;
            _cfgDistanceHysteresis = cfg.DistanceHysteresis;
            _cfgDistanceDecayFactor = cfg.DistanceDecayFactor;
            _cfgActiveDecayFactor = cfg.ActiveDecayFactor;
        }

        private void RestoreConfig()
        {
            RevertConfig(new Config());
        }

    }

}
