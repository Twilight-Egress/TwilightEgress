using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilightEgress.Content.Actions.Interfaces
{
    public interface IHasHome
    {
        Vector2 HomePosition { get; set; }
    }
}
