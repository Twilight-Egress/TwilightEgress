using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.IO;

namespace TwilightEgress.Content.Events
{
    public class EventHandlerManager : ModSystem
    {
        public static Dictionary<Type, Event> Events;

        public override void OnModLoad()
        {
            Events = [];
            foreach (Type type in AssemblyManager.GetLoadableTypes(TwilightEgress.Instance.Code))
            {
                if (type.IsSubclassOf(typeof(Event)) && !type.IsAbstract)
                {
                    Event handler = Activator.CreateInstance(type) as Event;
                    handler.OnModLoad();
                    Events[type] = handler;
                }
            }
        }

        public override void OnModUnload()
        {
            if (Events is not null)
            {
                foreach (Event eventHandler in Events.Values)
                    eventHandler.OnModUnload();
            }
            Events = null;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (Events is not null)
            {
                foreach (Event eventHandler in Events.Values)
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
                foreach (Event eventHandler in Events.Values)
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
                foreach (Event eventHandler in Events.Values)
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
                foreach (Event eventHandler in Events.Values)
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
                foreach (Event eventHandler in Events.Values)
                {
                    if (eventHandler.EventIsActive)
                        eventHandler.ModifySunLightColor(ref tileColor, ref backgroundColor);
                }
            }
        }

        /// <summary>
        /// Checks if the specified event is active or not.
        /// </summary>
        public static bool SpecificEventIsActive<T>() where T : Event => (bool)(Events?[typeof(T)].EventIsActive);

        /// <summary>
        /// Starts the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void StartEvent<T>() where T : Event
        {
            if (Events?.TryGetValue(typeof(T), out Event worldEvent) == true)
                worldEvent.StartEvent();
        }

        /// <summary>
        /// Stops the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void StopEvent<T>() where T : Event
        {
            if (Events?.TryGetValue(typeof(T), out Event worldEvent) == true)
                worldEvent.StopEvent();
        }

        private static void ResetAllEventStuff()
        {
            if (Events is not null)
            {
                foreach (Event eventHandler in Events.Values)
                {
                    if (eventHandler.EventIsActive)
                        eventHandler.ResetEventStuff();
                }
            }
        }
    }
}
