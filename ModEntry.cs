using HarmonyLib;
using StardewModdingAPI;

namespace FishingMachine
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            try 
            {
                harmony.PatchAll(typeof(ModEntry).Assembly);
                this.Monitor.Log("Lógica de Pesca aplicada com sucesso ao Charcoal Kiln!", LogLevel.Debug);
            }
            catch (System.Exception ex)
            {
                this.Monitor.Log($"Erro ao aplicar patches de harmonia: {ex}", LogLevel.Error);
            }
        }
    }
}
