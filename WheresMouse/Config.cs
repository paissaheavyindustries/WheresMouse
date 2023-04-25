using Dalamud.Configuration;
using System.Numerics;

namespace WheresMouse
{

    public class Config : IPluginConfiguration
    {

        public int Version { get; set; } = 0;

        public bool Opened { get; set; } = true;

        /// <summary>
        /// Indicators are enabled
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Indicators are meant to be drawn only in combat
        /// </summary>
        public bool OnlyShowInCombat { get; set; } = false;

        public bool OnlyOnScreen { get; set; } = true;

        /// <summary>
        /// Draws an indicator circle on the mouse position
        /// </summary>
        public bool DrawIndicatorCircle { get; set; } = true;

        /// <summary>
        /// Draws vertical and horizontal indicator lines from the mouse position
        /// </summary>
        public bool DrawIndicatorCardinal { get; set; } = true;

        /// <summary>
        /// Draws intercardinal (diagonal) indicator lines from the mouse position
        /// </summary>
        public bool DrawIndicatorIntercardinal { get; set; } = true;

        /// <summary>
        /// Radius of the indicator circle
        /// </summary>
        public int IndicatorCircleRadius { get; set; } = 50;

        /// <summary>
        /// Thickness of the cardinal indicator lines
        /// </summary>
        public int IndicatorCardinalThickness { get; set; } = 5;

        /// <summary>
        /// Thickness of the intercardinal (diagonal) indicator lines
        /// </summary>
        public int IndicatorIntercardinalThickness { get; set; } = 5;

        /// <summary>
        /// Color of the indicator circle
        /// </summary>
        public Vector3 IndicatorCircleColor { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);

        /// <summary>
        /// Color of the cardinal indicator lines
        /// </summary>
        public Vector3 IndicatorCardinalColor { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);

        /// <summary>
        /// Color of the intercardinal (diagonal) indicator lines
        /// </summary>
        public Vector3 IndicatorIntercardinalColor { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);

        /// <summary>
        /// Hysteresis limits for the distance accumulation that triggers the indicator effect on and off
        /// </summary>
        public Vector2 DistanceHysteresis { get; set; } = new Vector2(100.0f, 1500.0f);

        /// <summary>
        /// Decay factor the the distance accumulation when the indicator effect is not active
        /// </summary>
        public float DistanceDecayFactor { get; set; } = 0.95f;

        /// <summary>
        /// Decay factor the the distance accumulation when the indicator effect is active
        /// </summary>
        public float ActiveDecayFactor { get; set; } = 0.9f;

        public bool PerEnabled { get; set; } = false;
        public bool PerOnlyShowInCombat { get; set; } = false;
        public bool PerOnlyOnScreen { get; set; } = true;
        public int PerIndicatorThickness { get; set; } = 5;
        public Vector4 PerIndicatorColor { get; set; } = new Vector4(1.0f, 1.0f, 0.0f, 0.25f);

        public bool OfsEnabled { get; set; } = false;
        public bool OfsOnlyShowInCombat { get; set; } = false;
        public bool OfsBounce { get; set; } = true;
        public bool OfsBlink { get; set; } = true;
        public int OfsIndicatorSize { get; set; } = 20;
        public Vector4 OfsIndicatorColor { get; set; } = new Vector4(1.0f, 1.0f, 0.0f, 0.5f);

        public bool TrailEnabled { get; set; } = false;
        public bool TrailOnlyShowInCombat { get; set; } = false;
        public int TrailThreshold { get; set; } = 5;
        public int TrailTTL { get; set; } = 1000;
        public int TrailSize { get; set; } = 3;
        public Vector4 TrailColor { get; set; } = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);

    }

}
