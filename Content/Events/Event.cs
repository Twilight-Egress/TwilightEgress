using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TwilightEgress.Core.Globals.GlobalNPCs;

namespace TwilightEgress.Content.Events
{
    public abstract class Event
    {
        /// <summary>
        /// Whether the event is active or not, private.
        /// </summary>
        private bool active;

        /// <summary>
        /// The condition which determins whether or not the event is active.
        /// </summary>
        public bool EventIsActive => active;

        /// <summary>
        /// The internal name of this. Similar to <see cref="ModType.Name"/>
        /// </summary>
        public virtual string Name => GetType().Name;

        /// <summary>
        /// Whether or not the event should remain active after leaving and rejoining the world.
        /// </summary>
        public virtual bool PersistAfterLeavingWorld { get; }

        /// <summary>
        /// Start the event
        /// </summary>
        public void StartEvent() => active = true;

        /// <summary>
        /// Stops the event
        /// </summary>
        public void StopEvent() => active = false;

        public void OnModLoad()
        {
            EventGlobalNPC.EditSpawnPoolEvent += EditSpawnPool;
            EventGlobalNPC.EditSpawnRateEvent += EditSpawnRate;
            SafeOnModLoad();
        }

        public void OnModUnload()
        {
            EventGlobalNPC.EditSpawnPoolEvent -= EditSpawnPool;
            EventGlobalNPC.EditSpawnRateEvent -= EditSpawnRate;
            SafeOnModUnload();
        }

        public void SaveWorldData(TagCompound tag)
        {
            if (PersistAfterLeavingWorld && active)
                tag[$"{Name}.Active"] = active;
            SafeSaveWorldData(tag);
        }

        public void LoadWorldData(TagCompound tag)
        {
            active = tag.GetBool($"{Name}.Active");
            SafeLoadWorldData(tag);
        }


        public void HandlerUpdateEvent()
        {
            if (active && PreUpdateEvent())
                UpdateEvent();
        }

        private void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!active)
                return;
            EditEventSpawnPool(pool, spawnInfo);
        }

        private void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (!active)
                return;
            EditEventSpawnRate(player, ref spawnRate, ref maxSpawns);
        }

        /// <summary>
        /// Runs in <see cref="ModSystem.OnModLoad"/>. Override this to perform specific loading for your event.
        /// </summary>
        public virtual void SafeOnModLoad() { }

        /// <summary>
        /// Runs in <see cref="ModSystem.OnModUnload"/> Override this to unload things that you loaded in <see cref="SafeOnModLoad"/>.
        /// </summary>
        public virtual void SafeOnModUnload() { }

        /// <summary>
        /// Runs in <see cref="ModSystem.SaveWorldData(TagCompound)"/>. Override this if you need any bits of data saved.
        /// </summary>
        /// <param name="tag"></param>
        public virtual void SafeSaveWorldData(TagCompound tag) { }

        /// <summary>
        /// Runs in <see cref="ModSystem.LoadWorldData(TagCompound)"/> Override this to load whatever you saved in <see cref="SafeSaveWorldData(TagCompound)"/>.
        /// </summary>
        /// <param name="tag"></param>
        public virtual void SafeLoadWorldData(TagCompound tag) { }

        /// <summary>
        /// Runs before your event is active. If this returns false, your event will never be updated.
        /// </summary>
        /// <returns>Whether or not this event updates.</returns>
        public virtual bool PreUpdateEvent() => true;

        /// <summary>
        /// Override to run any code related to your custom event.
        /// </summary>
        public virtual void UpdateEvent() { }

        /// <summary>
        /// Allows you to directly edit the spawn pool for this event.
        /// </summary>
        /// <param name="pool">The current spawn pool for the location. Use this in tandem with <see cref="NPCSpawnInfo"/>.</param>
        /// <param name="spawnInfo">Contains specific information regarding where an NPC spawns and the player it spawns around.</param>
        public virtual void EditEventSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) { }

        /// <summary>
        /// Allows you to directly edit the spawn rate of NPCs for this event.
        /// </summary>
        /// <param name="spawnRate">The rate at which NPCs spawn around the player.</param>
        /// <param name="maxSpawns">The maximum amount of NPCs that can be active during this time.</param>
        public virtual void EditEventSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) { }

        /// <summary>
        /// Allows you to midfy the overall brightness of lights during this event, allowing to make effects similar to what
        /// the Night Vision Buff or Darkness Debuff do to you. Values too high or low may result in glitches so beware.
        /// </summary>
        /// <param name="scale">The current brightness scale.</param>
        public virtual void ModifyLightingBrightness(ref float scale) { }

        /// <summary>
        /// Allows you to modify what color the sun gives off.
        /// </summary>
        /// <param name="tileColor">The color of tiles.</param>
        /// <param name="backgroundColor">The color of the background.</param>
        public virtual void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor) { }

        /// <summary>
        /// Override to place any code related to your custom event that should be reset upon loading and unloading a world. 
        /// </summary>
        public virtual void ResetEventStuff() { }
    }
}
