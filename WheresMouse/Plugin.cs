using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Interface.Utility;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Common.Lua;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace WheresMouse
{

    public sealed class Plugin : IDalamudPlugin
    {

        public string Name => "Where's Mouse";

        private Service _svc = null;
        private Wherenator _where = new Wherenator();
        private Config _cfg;
        private float _adjusterX = 0.0f;
        private bool _drawingStarted = false;
        private DateTime _lastUpdate = DateTime.Now;
        private Vector2 _prevPos;
        private bool firstpos = true;
        private DateTime _loaded = DateTime.Now;

        private List<Maustrale> maustrales = new List<Maustrale>();

        public Plugin(IDalamudPluginInterface pluginInterface)
        {
            _svc = pluginInterface.Create<Service>();
            _cfg = _svc.pi.GetPluginConfig() as Config ?? new Config();
            _svc.pi.UiBuilder.Draw += DrawUI;
            _svc.pi.UiBuilder.OpenMainUi += DrawConfigUI;
            _svc.pi.UiBuilder.OpenConfigUi += DrawConfigUI;
            _svc.cm.AddHandler("/wheremouse", new CommandInfo(OnCommand)
            {
                HelpMessage = "Open Where's Mouse configuration"
            });
        }

        public void Dispose()
        {
            _svc.pi.UiBuilder.Draw -= DrawUI;
            _svc.pi.UiBuilder.OpenMainUi -= DrawConfigUI;
            _svc.pi.UiBuilder.OpenConfigUi -= DrawConfigUI;
            SaveConfig();
            _svc.cm.RemoveHandler("/wheremouse");
        }

        public void SaveConfig()
        {
            _svc.lo.Debug("Saving config");
            _svc.pi.SavePluginConfig(_cfg);
        }

        private void OnCommand(string command, string args)
        {
            _cfg.Opened = true;
        }

        private void StartDrawing()
        {
            if (_drawingStarted == true)
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
            _drawingStarted = true;
        }

        private void StopDrawing()
        {
            if (_drawingStarted == true)
            {
                ImGui.End();
                ImGui.PopStyleVar();
                _drawingStarted = false;
            }
        }

        private void Trales()
        {
            double delta = (DateTime.Now - _lastUpdate).TotalMilliseconds;
            _lastUpdate = DateTime.Now;
            List<Maustrale> graveyard = new List<Maustrale>();
            foreach (Maustrale m in maustrales)
            {
                m.TTL -= delta;
                if (m.TTL < 0.0f)
                {
                    graveyard.Add(m);
                    continue;
                }
                double op = m.TTL / m.TTLMax;
                ImGui.GetWindowDrawList().AddCircleFilled(
                    m.Position,
                    (float)(_cfg.TrailSize * op),
                    ImGui.GetColorU32(new Vector4(
                        m.Color.X,
                        m.Color.Y,
                        m.Color.Z,
                        m.Color.W * (float)op
                    )),
                    32
                );
            }
            foreach (Maustrale m in graveyard)
            {
                maustrales.Remove(m);
            }
        }

        private void DrawUI()
        {
            Vector2 pos = ImGui.GetMousePos();
            if (firstpos == true)
            {
                _prevPos = pos;
                firstpos = false;
            }
            Vector2 delta = Vector2.Subtract(pos, _prevPos);
            _prevPos = pos;
            _where.distDecayFactorActive = _cfg.ActiveDecayFactor;
            _where.distDecayFactorInactive = _cfg.DistanceDecayFactor;
            /*if (_cfg.DistanceHysteresis.X > _cfg.DistanceHysteresis.Y)
            {
                _cfg.DistanceHysteresis.X = _cfg.DistanceHysteresis.Y;
            }
            if (_cfg.DistanceHysteresis.Y < _cfg.DistanceHysteresis.X)
            {
                _cfg.DistanceHysteresis.Y = _cfg.DistanceHysteresis.X;
            }*/
            _where.distHystMin = _cfg.DistanceHysteresis.X;
            _where.distHystMax = _cfg.DistanceHysteresis.Y;
            _where.Update(pos);
            if (_cfg.Opened == true)
            {
                ImGui.PushStyleColor(ImGuiCol.TitleBgActive, new Vector4(0.496f, 0.058f, 0.323f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.TabActive, new Vector4(0.496f, 0.058f, 0.323f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.TabHovered, new Vector4(0.4f, 0.4f, 0.4f, 1.0f));
                ImGui.SetNextWindowSize(new Vector2(300, 500), ImGuiCond.FirstUseEver);
                bool open = true;
                if (ImGui.Begin(Name, ref open, ImGuiWindowFlags.NoCollapse) == false)
                {
                    ImGui.End();
                    ImGui.PopStyleColor(3);                    
                    return;
                }
                if (open == false)
                {
                    _cfg.Opened = false;
                    SaveConfig();
                    ImGui.End();
                    ImGui.PopStyleColor(3);
                    return;
                }
                ImGuiStylePtr style = ImGui.GetStyle();
                Vector2 fsz = ImGui.GetContentRegionAvail();
                fsz.Y -= ImGui.GetTextLineHeight() + (style.ItemSpacing.Y * 2) + style.WindowPadding.Y;
                ImGui.BeginChild("WhammyFrame", fsz);
                ImGui.BeginTabBar("Whammy_Main", ImGuiTabBarFlags.None);
                if (ImGui.BeginTabItem("Shake indicator"))
                {
                    ImGui.BeginChild("ShakeChild");
                    bool enabled = _cfg.Enabled;
                    if (ImGui.Checkbox("Enabled", ref enabled) == true)
                    {
                        _cfg.Enabled = enabled;
                    }
                    bool incombat = _cfg.OnlyShowInCombat;
                    if (ImGui.Checkbox("Only show in combat", ref incombat) == true)
                    {
                        _cfg.OnlyShowInCombat = incombat;
                    }
                    bool onscreen = _cfg.OnlyOnScreen;
                    if (ImGui.Checkbox("Only show if mouse is in window", ref onscreen) == true)
                    {
                        _cfg.OnlyOnScreen = onscreen;
                    }
                    ImGui.Separator();
                    Vector2 disthyst = _cfg.DistanceHysteresis;
                    ImGui.Text("Distance accumulation hysteresis");
                    if (ImGui.SliderFloat2("##Dah", ref disthyst, 100.0f, 10000.0f) == true)
                    {
                        _cfg.DistanceHysteresis = disthyst;
                    }
                    float distdecay = _cfg.DistanceDecayFactor;
                    ImGui.Text("Distance accumulation decay factor");
                    if (ImGui.SliderFloat("##Dadf", ref distdecay, 0.01f, 0.99f) == true)
                    {
                        _cfg.DistanceDecayFactor = distdecay;
                    }
                    float actdecay = _cfg.ActiveDecayFactor;
                    ImGui.Text("Active indicator decay factor");
                    if (ImGui.SliderFloat("##Aidf", ref actdecay, 0.01f, 0.99f) == true)
                    {
                        _cfg.ActiveDecayFactor = actdecay;
                    }
                    ImGui.Separator();
                    bool circle = _cfg.DrawIndicatorCircle;
                    if (ImGui.Checkbox("Draw circle", ref circle) == true)
                    {
                        _cfg.DrawIndicatorCircle = circle;
                    }
                    int circleradius = _cfg.IndicatorCircleRadius;
                    ImGui.Text("Circle radius in pixels");
                    if (ImGui.SliderInt("##Crip", ref circleradius, 10, 300) == true)
                    {
                        _cfg.IndicatorCircleRadius = circleradius;
                    }
                    Vector3 circlecol = _cfg.IndicatorCircleColor;
                    if (ImGui.ColorEdit3("Circle color", ref circlecol, ImGuiColorEditFlags.NoInputs) == true)
                    {
                        _cfg.IndicatorCircleColor = circlecol;
                    }
                    ImGui.Separator();
                    bool card = _cfg.DrawIndicatorCardinal;
                    if (ImGui.Checkbox("Draw cardinal lines", ref card) == true)
                    {
                        _cfg.DrawIndicatorCardinal = card;
                    }
                    int cardthick = _cfg.IndicatorCardinalThickness;
                    ImGui.Text("Cardinal line thickness in pixels");
                    if (ImGui.SliderInt("##Cltip", ref cardthick, 5, 50) == true)
                    {
                        _cfg.IndicatorCardinalThickness = cardthick;
                    }
                    Vector3 cardcolor = _cfg.IndicatorCardinalColor;
                    if (ImGui.ColorEdit3("Cardinal line color", ref cardcolor, ImGuiColorEditFlags.NoInputs) == true)
                    {
                        _cfg.IndicatorCardinalColor = cardcolor;
                    }
                    ImGui.Separator();
                    bool intercard = _cfg.DrawIndicatorIntercardinal;
                    if (ImGui.Checkbox("Draw intercardinal lines", ref intercard) == true)
                    {
                        _cfg.DrawIndicatorIntercardinal = intercard;
                    }
                    int intercardthick = _cfg.IndicatorIntercardinalThickness;
                    ImGui.Text("Intercardinal line thickness in pixels");
                    if (ImGui.SliderInt("##Iltip", ref intercardthick, 5, 50) == true)
                    {
                        _cfg.IndicatorIntercardinalThickness = intercardthick;
                    }
                    Vector3 intercardcolor = _cfg.IndicatorIntercardinalColor;
                    if (ImGui.ColorEdit3("Intercardinal line color", ref intercardcolor, ImGuiColorEditFlags.NoInputs) == true)
                    {
                        _cfg.IndicatorIntercardinalColor = intercardcolor;
                    }
                    ImGui.EndChild();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Persistent indicator"))
                {
                    ImGui.BeginChild("PerChild");
                    bool enabled = _cfg.PerEnabled;
                    if (ImGui.Checkbox("Enabled", ref enabled) == true)
                    {
                        _cfg.PerEnabled = enabled;
                    }
                    bool incombat = _cfg.PerOnlyShowInCombat;
                    if (ImGui.Checkbox("Only show in combat", ref incombat) == true)
                    {
                        _cfg.PerOnlyShowInCombat = incombat;
                    }
                    bool onscreen = _cfg.PerOnlyOnScreen;
                    if (ImGui.Checkbox("Only show if mouse is in window", ref onscreen) == true)
                    {
                        _cfg.PerOnlyOnScreen = onscreen;
                    }
                    ImGui.Separator();
                    bool percar = _cfg.PerCardinals;
                    if (ImGui.Checkbox("Show cardinal lines", ref percar) == true)
                    {
                        _cfg.PerCardinals = percar;
                    }
                    bool pericar = _cfg.PerIntercardinals;
                    if (ImGui.Checkbox("Show intercardinal lines", ref pericar) == true)
                    {
                        _cfg.PerIntercardinals = pericar;
                    }
                    bool percorner = _cfg.PerCorners;
                    if (ImGui.Checkbox("Show corner lines", ref percorner) == true)
                    {
                        _cfg.PerCorners = percorner;
                    }
                    int perthick = _cfg.PerIndicatorThickness;
                    ImGui.Text("Line thickness in pixels");
                    if (ImGui.SliderInt("##Cltip", ref perthick, 1, 50) == true)
                    {
                        _cfg.PerIndicatorThickness = perthick;
                    }
                    Vector4 percolor = _cfg.PerIndicatorColor;
                    if (ImGui.ColorEdit4("Line color", ref percolor, ImGuiColorEditFlags.NoInputs) == true)
                    {
                        _cfg.PerIndicatorColor = percolor;
                    }
                    ImGui.Separator();

                    bool perci = _cfg.PerCircle;
                    if (ImGui.Checkbox("Show circle", ref perci) == true)
                    {
                        _cfg.PerCircle = perci;
                    }
                    bool percifill = _cfg.PerCircleFilled;
                    if (ImGui.Checkbox("Filled circle", ref percifill) == true)
                    {
                        _cfg.PerCircleFilled = percifill;
                    }
                    int percirad = _cfg.PerCircleRadius;
                    ImGui.Text("Circle radius in pixels");
                    if (ImGui.SliderInt("##Cirad", ref percirad, 5, 50) == true)
                    {
                        _cfg.PerCircleRadius = percirad;
                    }
                    int percithick = _cfg.PerCircleThickness;
                    ImGui.Text("Line thickness in pixels");
                    if (ImGui.SliderInt("##Cithick", ref percithick, 1, 50) == true)
                    {
                        _cfg.PerCircleThickness = percithick;
                    }
                    Vector4 percicolor = _cfg.PerCircleColor;
                    if (ImGui.ColorEdit4("Circle color", ref percicolor, ImGuiColorEditFlags.NoInputs) == true)
                    {
                        _cfg.PerCircleColor = percicolor;
                    }

                    ImGui.EndChild();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Offscreen indicator"))
                {
                    ImGui.BeginChild("OfsChild");
                    bool enabled = _cfg.OfsEnabled;
                    if (ImGui.Checkbox("Enabled", ref enabled) == true)
                    {
                        _cfg.OfsEnabled = enabled;
                    }
                    bool incombat = _cfg.OfsOnlyShowInCombat;
                    if (ImGui.Checkbox("Only show in combat", ref incombat) == true)
                    {
                        _cfg.OfsOnlyShowInCombat = incombat;
                    }
                    ImGui.Separator();
                    bool ofsboin = _cfg.OfsBounce;
                    if (ImGui.Checkbox("Offscreen indicator bounce", ref ofsboin) == true)
                    {
                        _cfg.OfsBounce = ofsboin;
                    }
                    bool ofsblink = _cfg.OfsBlink;
                    if (ImGui.Checkbox("Offscreen indicator blink", ref ofsblink) == true)
                    {
                        _cfg.OfsBlink = ofsblink;
                    }
                    int ofssize = _cfg.OfsIndicatorSize;
                    ImGui.Text("Offscreen indicator size");
                    if (ImGui.SliderInt("##Tmt", ref ofssize, 50, 300) == true)
                    {
                        _cfg.OfsIndicatorSize = ofssize;
                    }
                    Vector4 ofscolor = _cfg.OfsIndicatorColor;
                    if (ImGui.ColorEdit4("Offscreen indicator color", ref ofscolor, ImGuiColorEditFlags.NoInputs) == true)
                    {
                        _cfg.OfsIndicatorColor = ofscolor;
                    }
                    ImGui.EndChild();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Trails"))
                {
                    ImGui.BeginChild("TrailChild");
                    bool enabled = _cfg.TrailEnabled;
                    if (ImGui.Checkbox("Enabled", ref enabled) == true)
                    {
                        _cfg.TrailEnabled = enabled;
                    }
                    bool incombat = _cfg.TrailOnlyShowInCombat;
                    if (ImGui.Checkbox("Only show in combat", ref incombat) == true)
                    {
                        _cfg.TrailOnlyShowInCombat = incombat;
                    }
                    ImGui.Separator();
                    int trailthr = _cfg.TrailThreshold;
                    ImGui.Text("Trail movement threshold");
                    if (ImGui.SliderInt("##Tmt", ref trailthr, 1, 50) == true)
                    {
                        _cfg.TrailThreshold = trailthr;
                    }
                    int trailttl = _cfg.TrailTTL;
                    ImGui.Text("Trail time-to-live in milliseconds");
                    if (ImGui.SliderInt("##Ttl", ref trailttl, 100, 5000) == true)
                    {
                        _cfg.TrailTTL = trailttl;
                    }
                    int trailsz = _cfg.TrailSize;
                    ImGui.Text("Trail size");
                    if (ImGui.SliderInt("##Trs", ref trailsz, 3, 50) == true)
                    {
                        _cfg.TrailSize = trailsz;
                    }
                    Vector4 trcolor = _cfg.TrailColor;
                    if (ImGui.ColorEdit4("Trail color", ref trcolor, ImGuiColorEditFlags.NoInputs) == true)
                    {
                        _cfg.TrailColor = trcolor;
                    }
                    ImGui.EndChild();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
                ImGui.EndChild();
                ImGui.Separator();
                Vector2 fp = ImGui.GetCursorPos();
                ImGui.SetCursorPos(new Vector2(_adjusterX, fp.Y));
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.496f, 0.058f, 0.323f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.496f, 0.058f, 0.323f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.4f, 0.4f, 0.4f, 1.0f));
                if (ImGui.Button("Discord") == true)
                {
                    Task tx = new Task(() =>
                    {
                        Process p = new Process();
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.FileName = @"https://discord.gg/6f9MY55";
                        p.Start();
                    });
                    tx.Start();
                }
                ImGui.SameLine();
                if (ImGui.Button("GitHub") == true)
                {
                    Task tx = new Task(() =>
                    {
                        Process p = new Process();
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.FileName = @"https://github.com/paissaheavyindustries/WheresMouse";
                        p.Start();
                    });
                    tx.Start();
                }
                ImGui.SameLine();
                _adjusterX += ImGui.GetContentRegionAvail().X;
                ImGui.PopStyleColor(3);
                ImGui.End();
                ImGui.PopStyleColor(3);
            }
            if (_cfg.Enabled == false && _cfg.PerEnabled == false && _cfg.TrailEnabled == false && _cfg.OfsEnabled == false)
            {
                return;
            }
            Vector2 disp = ImGui.GetIO().DisplaySize;
            bool offscreen = (
                (pos.X < 0.0f)
                ||
                (pos.X > disp.X)
                ||
                (pos.Y < 0.0f)
                ||
                (pos.Y > disp.Y)
            );
            bool inCombat = _svc.cd[ConditionFlag.InCombat];
            if (_cfg.Enabled == true && _where.active == true)
            {
                if ((inCombat == true || _cfg.OnlyShowInCombat == false) && (offscreen == false || _cfg.OnlyOnScreen == false))
                {
                    StartDrawing();
                    float opacity = (float)(_where.activePower / 100.0);
                    if (_cfg.DrawIndicatorCardinal == true)
                    {
                        float thickness = (float)(_cfg.IndicatorCardinalThickness * (_where.activePower / 100.0)) / 2.0f;
                        ImGui.GetWindowDrawList().AddQuadFilled(
                            new Vector2(pos.X - thickness, pos.Y - 20000),
                            new Vector2(pos.X + thickness, pos.Y - 20000),
                            new Vector2(pos.X + thickness, pos.Y + 20000),
                            new Vector2(pos.X - thickness, pos.Y + 20000),
                            ImGui.GetColorU32(new Vector4(
                                _cfg.IndicatorCardinalColor.X,
                                _cfg.IndicatorCardinalColor.Y,
                                _cfg.IndicatorCardinalColor.Z,
                                opacity
                            ))
                        );
                        ImGui.GetWindowDrawList().AddQuadFilled(
                            new Vector2(pos.X - 20000, pos.Y - thickness),
                            new Vector2(pos.X - 20000, pos.Y + thickness),
                            new Vector2(pos.X + 20000, pos.Y + thickness),
                            new Vector2(pos.X + 20000, pos.Y - thickness),
                            ImGui.GetColorU32(new Vector4(
                                _cfg.IndicatorCardinalColor.X,
                                _cfg.IndicatorCardinalColor.Y,
                                _cfg.IndicatorCardinalColor.Z,
                                opacity
                            ))
                        );
                    }
                    if (_cfg.DrawIndicatorIntercardinal == true)
                    {
                        float thickness = (float)(_cfg.IndicatorIntercardinalThickness * (_where.activePower / 100.0)) / 2.0f;
                        ImGui.GetWindowDrawList().AddQuadFilled(
                            new Vector2(pos.X - 20000 - thickness, pos.Y - 20000 + thickness),
                            new Vector2(pos.X - 20000 + thickness, pos.Y - 20000 - thickness),
                            new Vector2(pos.X + 20000 + thickness, pos.Y + 20000 - thickness),
                            new Vector2(pos.X + 20000 - thickness, pos.Y + 20000 + thickness),
                            ImGui.GetColorU32(new Vector4(
                                _cfg.IndicatorIntercardinalColor.X,
                                _cfg.IndicatorIntercardinalColor.Y,
                                _cfg.IndicatorIntercardinalColor.Z,
                                opacity
                            ))
                        );
                        ImGui.GetWindowDrawList().AddQuadFilled(
                            new Vector2(pos.X - 20000 - thickness, pos.Y + 20000 - thickness),
                            new Vector2(pos.X - 20000 + thickness, pos.Y + 20000 + thickness),
                            new Vector2(pos.X + 20000 + thickness, pos.Y - 20000 + thickness),
                            new Vector2(pos.X + 20000 - thickness, pos.Y - 20000 - thickness),
                            ImGui.GetColorU32(new Vector4(
                                _cfg.IndicatorIntercardinalColor.X,
                                _cfg.IndicatorIntercardinalColor.Y,
                                _cfg.IndicatorIntercardinalColor.Z,
                                opacity
                            ))
                        );
                    }
                    if (_cfg.DrawIndicatorCircle == true)
                    {
                        ImGui.GetWindowDrawList().AddCircleFilled(
                            new Vector2(pos.X, pos.Y),
                            (float)(_cfg.IndicatorCircleRadius * (_where.activePower / 100.0)),
                            ImGui.GetColorU32(new Vector4(
                                _cfg.IndicatorCircleColor.X,
                                _cfg.IndicatorCircleColor.Y,
                                _cfg.IndicatorCircleColor.Z,
                                opacity
                            )),
                            32
                        );
                    }
                }
            }
            if (_cfg.PerEnabled == true)
            {
                if ((inCombat == true || _cfg.PerOnlyShowInCombat == false) && (offscreen == false || _cfg.PerOnlyOnScreen == false))
                {
                    if (_cfg.PerCorners == true)
                    {
                        StartDrawing();
                        ImGui.GetWindowDrawList().AddLine(
                            new Vector2(0.0f),
                            new Vector2(pos.X, pos.Y),
                            ImGui.GetColorU32(_cfg.PerIndicatorColor),
                            _cfg.PerIndicatorThickness
                        );
                        ImGui.GetWindowDrawList().AddLine(
                            new Vector2(disp.X, 0.0f),
                            new Vector2(pos.X, pos.Y),
                            ImGui.GetColorU32(_cfg.PerIndicatorColor),
                            _cfg.PerIndicatorThickness
                        );
                        ImGui.GetWindowDrawList().AddLine(
                            new Vector2(0.0f, disp.Y),
                            new Vector2(pos.X, pos.Y),
                            ImGui.GetColorU32(_cfg.PerIndicatorColor),
                            _cfg.PerIndicatorThickness
                        );
                        ImGui.GetWindowDrawList().AddLine(
                            new Vector2(disp.X, disp.Y),
                            new Vector2(pos.X, pos.Y),
                            ImGui.GetColorU32(_cfg.PerIndicatorColor),
                            _cfg.PerIndicatorThickness
                        );
                    }
                    if (_cfg.PerCardinals == true)
                    {
                        StartDrawing();                        
                        ImGui.GetWindowDrawList().AddLine(
                            new Vector2(0.0f, pos.Y),
                            new Vector2(disp.X, pos.Y),
                            ImGui.GetColorU32(_cfg.PerIndicatorColor),
                            _cfg.PerIndicatorThickness
                        );
                        ImGui.GetWindowDrawList().AddLine(
                            new Vector2(pos.X, 0.0f),
                            new Vector2(pos.X, disp.Y),
                            ImGui.GetColorU32(_cfg.PerIndicatorColor),
                            _cfg.PerIndicatorThickness
                        );
                    }
                    if (_cfg.PerIntercardinals == true)
                    {
                        StartDrawing();
                        ImGui.GetWindowDrawList().AddLine(
                            new Vector2(pos.X - pos.Y, 0.0f),
                            new Vector2(pos.X + (disp.Y - pos.Y), disp.Y),
                            ImGui.GetColorU32(_cfg.PerIndicatorColor),
                            _cfg.PerIndicatorThickness
                        );
                        ImGui.GetWindowDrawList().AddLine(
                            new Vector2(pos.X + pos.Y, 0.0f),
                            new Vector2(pos.X - (disp.Y - pos.Y), disp.Y),
                            ImGui.GetColorU32(_cfg.PerIndicatorColor),
                            _cfg.PerIndicatorThickness
                        );
                    }
                    if (_cfg.PerCircle == true)
                    {
                        StartDrawing();
                        if (_cfg.PerCircleFilled == true)
                        {
                            ImGui.GetWindowDrawList().AddCircleFilled(
                                new Vector2(pos.X, pos.Y),
                                _cfg.PerCircleRadius,
                                ImGui.GetColorU32(_cfg.PerIndicatorColor),
                                20
                            );
                        }
                        else
                        {
                            ImGui.GetWindowDrawList().AddCircle(
                                new Vector2(pos.X, pos.Y),
                                _cfg.PerCircleRadius,
                                ImGui.GetColorU32(_cfg.PerIndicatorColor),
                                20,
                                _cfg.PerCircleThickness
                            );
                        }

                    }
                }
            }
            if (_cfg.TrailEnabled == true)
            {
                if (inCombat == true || _cfg.TrailOnlyShowInCombat == false)
                {
                    if (delta.Length() > _cfg.TrailThreshold)
                    {
                        maustrales.Add(new Maustrale() { Position = new Vector2(_prevPos.X, _prevPos.Y), TTL = _cfg.TrailTTL, TTLMax = _cfg.TrailTTL, Style = Maustrale.StyleEnum.Boingo, Color = _cfg.TrailColor });
                    }
                    if (maustrales.Count > 0)
                    {
                        StartDrawing();
                        Trales();
                    }
                }
            }
            if (_cfg.OfsEnabled == true)
            {
                if (inCombat == true || _cfg.OfsOnlyShowInCombat == false)
                {
                    Vector2 arrowtip = new Vector2();
                    float x = pos.X, y = pos.Y;
                    if (offscreen == true)
                    {
                        x = Math.Clamp(x, 30.0f, disp.X - 30.0f);
                        y = Math.Clamp(y, 30.0f, disp.Y - 30.0f);
                        float angle = (float)(Math.Atan2(pos.Y - y, pos.X - x) + Math.PI);
                        float time = (float)((DateTime.Now - _loaded).TotalMilliseconds / 200.0);
                        float bounce = (float)(_cfg.OfsBounce == true ? 40.0f * Math.Abs(Math.Cos(time)) : 0.0f);
                        arrowtip = new Vector2(
                            x + (float)(Math.Cos(angle) * bounce),
                            y + (float)(Math.Sin(angle) * bounce)
                        );
                        Vector2 arrowhead1 = new Vector2(
                            arrowtip.X + (float)(Math.Cos(angle) * (_cfg.OfsIndicatorSize * 0.3f)) + (float)(Math.Cos(angle + Math.PI / 2.0f) * (_cfg.OfsIndicatorSize * 0.3f)),
                            arrowtip.Y + (float)(Math.Sin(angle) * (_cfg.OfsIndicatorSize * 0.3f)) + (float)(Math.Sin(angle + Math.PI / 2.0f) * (_cfg.OfsIndicatorSize * 0.3f))
                        );
                        Vector2 arrowhead2 = new Vector2(
                            arrowtip.X + (float)(Math.Cos(angle) * (_cfg.OfsIndicatorSize * 0.3f)) - (float)(Math.Cos(angle + Math.PI / 2.0f) * (_cfg.OfsIndicatorSize * 0.3f)),
                            arrowtip.Y + (float)(Math.Sin(angle) * (_cfg.OfsIndicatorSize * 0.3f)) - (float)(Math.Sin(angle + Math.PI / 2.0f) * (_cfg.OfsIndicatorSize * 0.3f))
                        );
                        Vector2 arrowtail1 = new Vector2(
                            arrowtip.X + (float)(Math.Cos(angle) * (_cfg.OfsIndicatorSize * 0.3f)) + (float)(Math.Cos(angle + Math.PI / 2.0f) * (_cfg.OfsIndicatorSize * 0.1f)),
                            arrowtip.Y + (float)(Math.Sin(angle) * (_cfg.OfsIndicatorSize * 0.3f)) + (float)(Math.Sin(angle + Math.PI / 2.0f) * (_cfg.OfsIndicatorSize * 0.1f))
                        );
                        Vector2 arrowtail2 = new Vector2(
                            arrowtip.X + (float)(Math.Cos(angle) * (_cfg.OfsIndicatorSize * 0.3f)) - (float)(Math.Cos(angle + Math.PI / 2.0f) * (_cfg.OfsIndicatorSize * 0.1f)),
                            arrowtip.Y + (float)(Math.Sin(angle) * (_cfg.OfsIndicatorSize * 0.3f)) - (float)(Math.Sin(angle + Math.PI / 2.0f) * (_cfg.OfsIndicatorSize * 0.1f))
                        );
                        Vector2 arrowtail3 = new Vector2(
                            arrowtail1.X + (float)(Math.Cos(angle) * _cfg.OfsIndicatorSize),
                            arrowtail1.Y + (float)(Math.Sin(angle) * _cfg.OfsIndicatorSize)
                        );
                        Vector2 arrowtail4 = new Vector2(
                            arrowtail2.X + (float)(Math.Cos(angle) * _cfg.OfsIndicatorSize),
                            arrowtail2.Y + (float)(Math.Sin(angle) * _cfg.OfsIndicatorSize)
                        );
                        StartDrawing();
                        float alpha = _cfg.OfsBlink == true ? _cfg.OfsIndicatorColor.W * (float)Math.Abs(Math.Cos(time)) : _cfg.OfsIndicatorColor.W;
                        ImGui.GetWindowDrawList().PathLineTo(arrowtip);
                        ImGui.GetWindowDrawList().PathLineTo(arrowhead1);
                        ImGui.GetWindowDrawList().PathLineTo(arrowhead2);
                        ImGui.GetWindowDrawList().PathFillConvex(
                            ImGui.GetColorU32(new Vector4(
                                _cfg.OfsIndicatorColor.X,
                                _cfg.OfsIndicatorColor.Y,
                                _cfg.OfsIndicatorColor.Z,
                                alpha
                            ))
                        );
                        ImGui.GetWindowDrawList().PathLineTo(arrowtail1);
                        ImGui.GetWindowDrawList().PathLineTo(arrowtail2);
                        ImGui.GetWindowDrawList().PathLineTo(arrowtail4);
                        ImGui.GetWindowDrawList().PathLineTo(arrowtail3);
                        ImGui.GetWindowDrawList().PathFillConvex(
                            ImGui.GetColorU32(new Vector4(
                                _cfg.OfsIndicatorColor.X,
                                _cfg.OfsIndicatorColor.Y,
                                _cfg.OfsIndicatorColor.Z,
                                alpha
                            ))
                        );
                    }
                }
            }
            StopDrawing();
        }

        private void DrawConfigUI()
        {
            _cfg.Opened = true;
        }

    }

}
