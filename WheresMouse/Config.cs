using Dalamud.Configuration;
using System.Numerics;

namespace WheresMouse
{
    
    public class Config : IPluginConfiguration
    {

        public int Version { get; set; } = 0;

        /// <summary>
        /// Indicators are enabled
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Indicators are meant to be drawn only in combat
        /// </summary>
        public bool OnlyShowInCombat { get; set; } = false;

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
        public int IndicatorCircleRadius { get; set; } = 100;

        /// <summary>
        /// Thickness of the cardinal indicator lines
        /// </summary>
        public int IndicatorCardinalThickness { get; set; } = 20;

        /// <summary>
        /// Thickness of the intercardinal (diagonal) indicator lines
        /// </summary>
        public int IndicatorIntercardinalThickness { get; set; } = 20;

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

    }

}
