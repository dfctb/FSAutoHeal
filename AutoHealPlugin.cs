using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace FSAutoHeal
{
    [BepInPlugin("com.conf.fsautoheal", "FS AutoHeal", "0.3.0")]
    public class AutoHealPlugin : BaseUnityPlugin
    {
        private ConfigEntry<KeyboardShortcut> _toggleKey;
        private ConfigEntry<bool> _enabledOnStart;
        private ConfigEntry<bool> _useStimpack;
        private ConfigEntry<int> _stimpackBelowPercent;
        private ConfigEntry<bool> _useRadAway;
        private ConfigEntry<int> _radAwayAbovePercent;
        private ConfigEntry<float> _interval;

        private bool _enabled;
        private float _timer;

        private MethodInfo _hasStimpack;
        private MethodInfo _hasRadAway;
        private MethodInfo _useRadAwayMethod;

        private void Awake()
        {
            _toggleKey = Config.Bind("Hotkeys", "ToggleKey", new KeyboardShortcut(KeyCode.F7),
                "Key that turns auto-healing on and off in game");

            _enabledOnStart = Config.Bind("General", "EnabledOnStart", true,
                "Is auto-healing on when the game starts");
            _interval = Config.Bind("General", "CheckInterval", 0.5f,
                "Seconds between checks");

            _useStimpack = Config.Bind("Stimpak", "Enabled", true,
                "Auto-use Stimpaks on hurt dwellers");
            _stimpackBelowPercent = Config.Bind("Stimpak", "UseWhenHealthBelowPercent", 50,
                new ConfigDescription("Use a Stimpak when a dweller's health is at or below this percent (1-99)",
                    new AcceptableValueRange<int>(1, 99)));

            _useRadAway = Config.Bind("RadAway", "Enabled", true,
                "Auto-use RadAway on irradiated dwellers");
            _radAwayAbovePercent = Config.Bind("RadAway", "UseWhenRadiationAbovePercent", 30,
                new ConfigDescription("Use RadAway when a dweller's radiation is at or above this percent (1-100)",
                    new AcceptableValueRange<int>(1, 100)));

            _enabled = _enabledOnStart.Value;

            // Letter case and visibility of these methods differ between builds,
            // so look them up by name, ignoring case, including non-public ones.
            _hasStimpack = FindMethod(typeof(DwellerHealth), "hasstimpack");
            _hasRadAway = FindMethod(typeof(DwellerHealth), "hasradaway");
            _useRadAwayMethod = FindMethod(typeof(DwellerHealth), "useradaway");

            if (_hasStimpack == null)
                Logger.LogWarning("HasStimpack not found: falling back to the game's own threshold for Stimpaks");
            if (_hasRadAway == null)
                Logger.LogWarning("HasRadAway not found: falling back to the game's own threshold for RadAway");
            if (_useRadAwayMethod == null)
                Logger.LogWarning("UseRadAway not found: RadAway auto-use disabled");

            Logger.LogInfo("FS AutoHeal loaded (" + (_enabled ? "ON" : "OFF") + ")");
        }

        private static MethodInfo FindMethod(Type type, string nameLower)
        {
            MethodInfo[] methods = type.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name.ToLowerInvariant() == nameLower && methods[i].GetParameters().Length == 0)
                    return methods[i];
            }
            return null;
        }

        private void Update()
        {
            if (_toggleKey.Value.IsDown())
            {
                _enabled = !_enabled;
                Logger.LogInfo("AutoHeal: " + (_enabled ? "ON" : "OFF"));
            }

            if (!_enabled) return;

            _timer += Time.unscaledDeltaTime;
            if (_timer < _interval.Value) return;
            _timer = 0f;

            try
            {
                HealAll();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        private void HealAll()
        {
            if (!MonoSingleton<Vault>.IsInstanceValid) return;

            List<Dweller> source = MonoSingleton<Vault>.Instance.GetAliveDwellers();
            if (source == null) return;

            // Copy the list: using items can change the vault's collections.
            List<Dweller> dwellers = new List<Dweller>(source);

            for (int i = 0; i < dwellers.Count; i++)
            {
                Dweller dweller = dwellers[i];
                if (dweller == null) continue;

                DwellerHealth health = dweller.Health;
                if (health == null) continue;

                if (_useStimpack.Value)
                    TryStimpack(health);

                if (_useRadAway.Value)
                    TryRadAway(health);
            }
        }

        private void TryStimpack(DwellerHealth health)
        {
            float max = health.HealthMax;
            if (max <= 0f) return;

            float percent = health.HealthValue / max * 100f;
            if (percent > _stimpackBelowPercent.Value) return;

            bool canUse;
            if (_hasStimpack != null)
                canUse = (bool)_hasStimpack.Invoke(health, null);
            else
                canUse = health.NeedStimpack;

            if (!canUse) return;

            health.UseStimpack();
            Logger.LogInfo("AutoHeal: Stimpak used at " + percent.ToString("0") + "% health");
        }

        private void TryRadAway(DwellerHealth health)
        {
            if (_useRadAwayMethod == null) return;

            float max = health.RadiationMax;
            if (max <= 0f) return;

            float percent = health.RadiationValue / max * 100f;
            if (percent < _radAwayAbovePercent.Value) return;

            bool canUse;
            if (_hasRadAway != null)
                canUse = (bool)_hasRadAway.Invoke(health, null);
            else
                canUse = health.NeedRadaway;

            if (!canUse) return;

            _useRadAwayMethod.Invoke(health, null);
            Logger.LogInfo("AutoHeal: RadAway used at " + percent.ToString("0") + "% radiation");
        }
    }
}
