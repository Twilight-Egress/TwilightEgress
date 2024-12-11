using System.Linq;

namespace TwilightEgress.Content.EntityOverrides.Items
{
    public abstract class ItemOverride : GlobalItem
    {
        public abstract int TypeToOverride { get; }

        // I'll be damned if this isn't enough.
        public virtual int[] AdditionalOverrideTypes => new int[100];

        public override sealed bool InstancePerEntity => true;

        public override sealed bool AppliesToEntity(Item entity, bool lateInstantiation)
            => lateInstantiation && (entity.type == TypeToOverride || AdditionalOverrideTypes.Contains(entity.type));
    }
}
