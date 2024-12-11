using Terraria.ModLoader.Core;
using System;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TwilightEgress.Content.Events
{
    public class EventHandlerManager : ModSystem
    {
        public static Dictionary<Type, EventHandler> Events;

        public override void OnModLoad()
        {
            Events = [];
            foreach (Type type in AssemblyManager.GetLoadableTypes(TwilightEgress.Instance.Code))
            {
                if (type.IsSubclassOf(typeof(EventHandler)) && !type.IsAbstract)
                {
                    EventHandler handler = Activator.CreateInstance(type) as EventHandler;
                    handler.OnModLoad();
                    Events[type] = handler;
                }
            }
        }

        public override void OnModUnload()
        {
            if (Events is not null)
            {
                foreach (EventHandler eventHandler in Events.Values)
                    eventHandler.OnModUnload();
            }
            Events = null;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (Events is not null)
            {
                foreach (EventHandler eventHandler in Events.Values)
                {
                    if (eventHandler.EventIsActive)
                        eventHandler.SaveWorldData(tag);
                }
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (Events is not null)
            {
                foreach (EventHandler eventHandler in Events.Values)
                {
                    if (eventHandler.EventIsActive)
                        eventHandler.SaveWorldData(tag);
                }
            }
        }

        public override void OnWorldLoad() => ResetAllEventStuff();

        public override void OnWorldUnload() => ResetAllEventStuff();

        public override void PostUpdateEverything()
        {
            if (Events is not null)
            {
                foreach (EventHandler eventHandler in Events.Values)
                {
                    if (eventHandler.EventIsActive)
                        eventHandler.HandlerUpdateEvent();
                }
            }
        }

        public override void ModifyLightingBrightness(ref float scale)
        {
            if (Events is not null)
            {
                foreach (EventHandler eventHandler in Events.Values)
                {
                    if (eventHandler.EventIsActive)
                        eventHandler.ModifyLightingBrightness(ref scale);
                }
            }
        }

        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (Events is not null)
            {
                foreach (EventHandler eventHandler in Events.Values)
                {
                    if (eventHandler.EventIsActive)
                        eventHandler.ModifySunLightColor(ref tileColor, ref backgroundColor);
                }
            }
        }

        /// <summary>
        /// Checks if the specified event is active or not.
        /// </summary>
        public static bool SpecificEventIsActive<T>() where T : EventHandler => (bool)(Events?[typeof(T)].EventIsActive);

        /// <summary>
        /// Starts the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void StartEvent<T>() where T : EventHandler
        {
            if (Events?.TryGetValue(typeof(T), out EventHandler worldEvent) == true)
                worldEvent.StartEvent();
        }

        /// <summary>
        /// Stops the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void StopEvent<T>() where T : EventHandler
        {
            if (Events?.TryGetValue(typeof(T), out EventHandler worldEvent) == true)
                worldEvent.StopEvent();
        }

        private static void ResetAllEventStuff()
        {
            if (Events is not null)
            {
                foreach (EventHandler eventHandler in Events.Values)
                {
                    if (eventHandler.EventIsActive)
                        eventHandler.ResetEventStuff();
                }
            }
        }
    }
}
