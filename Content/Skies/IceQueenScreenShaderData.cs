namespace TwilightEgress.Content.Skies
{
    public class IceQueenScreenShaderData : ScreenShaderData
    {
        public IceQueenScreenShaderData(string passName) : base(passName) { }

        public override void Apply()
        {
            base.Apply();
            int FrostLadyIndex = NPC.FindFirstNPC(NPCID.IceQueen);
            if (FrostLadyIndex < 0)
                return;

            UseTargetPosition(Main.LocalPlayer.Center);
        }
    }
}
