using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;

namespace FishingMachine
{
    [HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.performObjectDropInAction))]
    public class FishingPatch
    {
        static bool Prefix(StardewValley.Object __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            // Se probe for true, o jogo está apenas checando se o item PODE ser colocado
            // Não devemos consumir itens ou processar lógica se for apenas um teste

            // Verifica se a máquina é o Charcoal Kiln (ID "114")
            if (__instance.QualifiedItemId != "(BC)114")
                return true;

            // Verifica se o item colocado é uma Bait comum (ID "685")
            if (dropInItem?.QualifiedItemId != "(O)685")
                return true;

            if (probe)
            {
                __result = true;
                return false;
            }

            // Lógica de sorteio do resultado
            Random random = new Random();
            bool treasure = random.NextDouble() < 0.15;
            StardewValley.Object resultItem;

            if (treasure)
            {
                // Busca um tesouro da localização atual do jogador
                Item treasureItem = Utility.getTreasureFromLocation(who.currentLocation);
                // Converte para objeto se necessário ou define um padrão (ex: Geodo)
                resultItem = (treasureItem is StardewValley.Object obj) ? obj : new StardewValley.Object("390", 1);
            }
            else
            {
                // Tenta pegar um peixe da água local
                // O método getFish retorna o ID do peixe
                int fishId = who.currentLocation.getFish(0, "685", 1, who, 0, who.TilePoint);
                
                // Se não vier peixe (retornar -1), vira Alga (153)
                if (fishId == -1) fishId = 153;
                
                resultItem = new StardewValley.Object(fishId.ToString(), 1);
            }

            // Configura a máquina para processar
            __instance.heldObject.Value = resultItem;
            __instance.MinutesUntilReady = 30; // 30 minutos de jogo
            
            // Ativa a animação visual da máquina trabalhando
            who.currentLocation.playSound("Ship");
            
            __result = true;
            return false; // Pula a lógica original do Charcoal Kiln (que transformaria madeira em carvão)
        }
    }
}
