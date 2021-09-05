using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Main.Scenes;
using Assets.Scripts.Models;
using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Powers;
using Assets.Scripts.Models.Profile;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Abilities;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using Assets.Scripts.Models.Towers.Filters;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Models.TowerSets;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Display;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Assets.Scripts.Unity.UI_New.Upgrade;
using Assets.Scripts.Utils;
using Harmony;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

using UnhollowerBaseLib;
using UnityEngine;
using BTD_Mod_Helper.Extensions;
using Assets.Scripts.Models.Towers.Weapons.Behaviors;
using Assets.Scripts.Models.Towers.Weapons;
using System.Net;
using Assets.Scripts.Unity.UI_New.Popups;
using TMPro;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.TowerFilters;
using Assets.Scripts.Unity.UI_New.Main.MonkeySelect;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Towers;

namespace ZapMonkey
{

    class Main : BloonsMod
    {


        public class ZapMonkey : ModTower
        {
            public override string Name => "ZapMonkey";
            public override string DisplayName => "ZapMonkey";
            public override string Description => "Uses electricity to pop bloons. Good fire rate damage and pierce.";
            public override string BaseTower => "EngineerMonkey";
            public override int Cost => 600;
            public override int TopPathUpgrades => 5;
            public override int MiddlePathUpgrades => 5;
            public override int BottomPathUpgrades => 5;
            public override string TowerSet => "Primary";
            public override void ModifyBaseTowerModel(TowerModel towerModel)
            {
                //balance stuff
                towerModel.display = "423e0ab45d98fdb43b7584c60f3ecf79";
                towerModel.GetBehavior<DisplayModel>().display = "423e0ab45d98fdb43b7584c60f3ecf79";
                var attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].projectile.GetBehavior<TravelStraitModel>().Lifespan = 99;
                attackModel.weapons[0].projectile.display = "ffed377b3e146f649b3e6d5767726a44";
                //pierce and damage
                attackModel.weapons[0].projectile.pierce = 4;
                attackModel.weapons[0].projectile.GetBehavior<DamageModel>().damage = 3;

                //change radius to 75% of 100 mortar
                attackModel.weapons[0].projectile.radius = 28 * 0.75f;



                //how many seconds until it shoots
                attackModel.weapons[0].Rate = 1.65f;

                MelonLogger.Msg("Base Loaded");
            }
            public override string Icon => "ZapMonkey_Icon";
            public override string Portrait => "ZapMonkey_Icon";
        }
        public class EnlargedShots : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Electric Shock";
            public override string DisplayName => "Electric Shock";
            public override string Description => "Shocked bloons get shocked into popping 1 second after being hit.";
            public override int Cost => 800;
            public override int Path => TOP;
            public override int Tier => 1;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();

                var lasershock = Game.instance.model.GetTowerFromId("DartlingGunner-200").Duplicate<TowerModel>().GetBehavior<AttackModel>().weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>();
                lasershock.lifespan = 1;
                attackModel.weapons[0].Rate /= 1.3f;
                attackModel.weapons[0].projectile.AddBehavior(lasershock);
                attackModel.weapons[0].projectile.AddBehavior(lasershock);
                attackModel.weapons[0].projectile.collisionPasses = new int[] { 0, 1 };

                MelonLogger.Msg("1st upgrade Loaded");

            }
            public override string Icon => "Electricshock_icon";
            public override string Portrait => "Electricshock";
        }
        public class NadeOptimizer : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Jumper Cables";
            public override string DisplayName => "Jumper Cables";
            public override string Description => "Allows electricity to jump from bloon to bloon.";
            public override int Cost => 900;
            public override int Path => TOP;
            public override int Tier => 2;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].projectile.pierce += 2;
                attackModel.weapons[0].Rate /= 1.3f;
                var seeking = Game.instance.model.GetTowerFromId("NinjaMonkey-001").GetBehavior<AttackModel>().weapons[0].projectile.GetBehavior<TrackTargetWithinTimeModel>().Duplicate<TrackTargetWithinTimeModel>();
                attackModel.weapons[0].projectile.AddBehavior<TrackTargetWithinTimeModel>(seeking);

            }
            public override string Icon => "Jumpercables_icon";
            public override string Portrait => "Jumpercables";
        }
        public class HeavyBlasts : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Provoker";
            public override string DisplayName => "Provoker";
            public override string Description => "Deals much more damage to fortified bloons. Base damage and attack speed also increased";
            public override int Cost => 1500;
            public override int Path => TOP;
            public override int Tier => 3;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                towerModel.display = "0878b4519529a0242a73b360c00f67ef";
                towerModel.GetBehavior<DisplayModel>().display = "0878b4519529a0242a73b360c00f67ef";
                attackModel.weapons[0].projectile.GetDamageModel().damage++;
                attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("aaa", "Fortified", 1.7f, 0, false, false) { tags = new string[] { "Fortified" }, collisionPass = 0 });
                attackModel.weapons[0].projectile.GetBehavior<DamageModel>().damage += 3;
                attackModel.weapons[0].Rate /= 1.5f;
                attackModel.weapons[0].projectile.pierce += 3;

            }
            public override string Icon => "Provoker_icon";
            public override string Portrait => "Provoker";
        }
        public class T101Feldhaubitz : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Lightning Spike";
            public override string DisplayName => "Lightning Spike";
            public override string Description => "Fast firing lightning wreaks havoc on bloons.";
            public override int Cost => 3500;
            public override int Path => TOP;
            public override int Tier => 4;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].Rate /= 3f;
                attackModel.weapons[0].projectile.pierce += 3;
                attackModel.weapons[0].projectile.GetBehavior<DamageModel>().damage += 3;
            }
            public override string Icon => "Lightning Spike_icon";
            public override string Portrait => "Lightning Spike";
        }
        public class TheBigBang : ModUpgrade<ZapMonkey>
        {
            public override string Name => "The Beacon";
            public override string DisplayName => "The Beacon";
            public override string Description => "A true master of lightning.";
            public override int Cost => 30000;
            public override int Path => TOP;
            public override int Tier => 5;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                towerModel.display = "682a2ffa86ca5d04897383aa83ee80cf";
                towerModel.GetBehavior<DisplayModel>().display = "682a2ffa86ca5d04897383aa83ee80cf";
                attackModel.weapons[0].Rate /= 2.5f;
                attackModel.weapons[0].projectile.pierce += 2;
                attackModel.weapons[0].projectile.GetBehavior<DamageModel>().damage += 4;
            }
            public override string Icon => "Thebeacon_icon";
            public override string Portrait => "Thebeacon";
        }
        public class QuickdrawSight : ModUpgrade<ZapMonkey>
        {
            public override string Name => "BlackLight";
            public override string DisplayName => "BlackLight";
            public override string Description => "Gives Zap Monkey camo detection.";
            public override int Cost => 350;
            public override int Path => MIDDLE;
            public override int Tier => 1;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
               towerModel.AddBehavior(new OverrideCamoDetectionModel("OverrideCamoDetectionModel_", true));
            }
            public override string Icon => "Blacklight_icon";
            public override string Portrait => "Blacklight";
        }
        public class Tenacious : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Double Prongs";
            public override string DisplayName => "Double Prongs";
            public override string Description => "Using double the prongs ups its attack speed.";
            public override int Cost => 500;
            public override int Path => MIDDLE;
            public override int Tier => 2;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].Rate /= 1.5f;
            }
            public override string Icon => "Doubleprongs_icon";
            public override string Portrait => "Doubleprongs";
        }
        public class Adaptive : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Negative Energy";
            public override string DisplayName => "Negative Energy";
            public override string Description => "Attack becomes a bolt of negative energy knocking back bloons.";
            public override int Cost => 2000;
            public override int Path => MIDDLE;
            public override int Tier => 3;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                towerModel.display = "be87fb5b6c96c1b46b6f82f66be29a44";
                towerModel.GetBehavior<DisplayModel>().display = "be87fb5b6c96c1b46b6f82f66be29a44";
                attackModel.weapons[0].projectile.GetBehavior<DamageModel>().damage += 1;
                WindModel wind = new WindModel("WindModel_",30f,50f,100f,false,null,0);
                attackModel.weapons[0].projectile.AddBehavior(wind);

            }
            public override string Icon => "Negativeenergy_icon";
            public override string Portrait => "Negativeenergy";
        }
        public class BouncyShots : ModUpgrade<ZapMonkey>
        {
            public override string Name => "BlackHole Generator";
            public override string DisplayName => "BlackHole Generator";
            public override string Description => "Ability: Casts a black hole in range that stops bloons from getting through.";
            public override int Cost => 4000;
            public override int Path => MIDDLE;
            public override int Tier => 4;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].Rate /= 2f;
                towerModel.AddBehavior(Game.instance.model.GetTowerFromId("ObynGreenfoot 3").GetAbility().Duplicate());
                towerModel.GetAbility().Cooldown = 75.0f;
                var activate = towerModel.GetAbility().GetBehavior<ActivateAttackModel>().attacks[0];
                activate.weapons[0].projectile.GetBehavior<AgeModel>().Lifespan = 15f;
                activate.weapons[0].projectile.display = "24e47e3d6dd51f74baa99537277c39e0";
                activate.weapons[0].projectile.RemoveBehavior<DamageModel>();
                activate.weapons[0].projectile.pierce = 9999999999.0f;
                activate.weapons[0].projectile.AddBehavior(new KnockbackModel("KnockbackModel_", 0.0f, 1.0f, 1.0f, 0.1f, "Knockback"));


            }
            public override string Icon => "Blackholegenerator_icon";
            public override string Portrait => "Blackholegenerator";
        }
        public class ExplosivesSpecialist : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Rift Master General";
            public override string DisplayName => "Rift Master General";
            public override string Description => "Spawns a rift that can suck up an INFINITE amount of bloons!";
            public override int Cost => 50000;
            public override int Path => MIDDLE;
            public override int Tier => 5;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                towerModel.display = "363079d873f805b4c8c842501d492747";
                towerModel.GetBehavior<DisplayModel>().display = "363079d873f805b4c8c842501d492747";
                attackModel.weapons[0].Rate /= 1.5f;
                attackModel.weapons[0].projectile.pierce += 3;
                attackModel.weapons[0].projectile.GetBehavior<DamageModel>().damage += 3;
                var activate = towerModel.GetAbility().GetBehavior<ActivateAttackModel>().attacks[0];
                activate.weapons[0].projectile.RemoveBehavior<DamageModel>();
                activate.weapons[0].projectile.GetBehavior<AgeModel>().Lifespan = 15f;
                activate.weapons[0].projectile.AddBehavior(new DamageModel("DamageModel_", 99999f, 99999f, false, false, false, BloonProperties.None));
                activate.weapons[0].projectile.pierce = 999999.0f;



            }
            public override string Icon => "Riftmastergeneral_icon";
            public override string Portrait => "Riftmastergeneral";
        }
        public class NimbleHands : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Bigger Batteries";
            public override string DisplayName => "Bigger Batteries";
            public override string Description => "Zaps can now pop 5 layers of bloon.";
            public override int Cost => 400;
            public override int Path => BOTTOM;
            public override int Tier => 1;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].projectile.GetBehavior<DamageModel>().damage += 2;
            }
            public override string Icon => "Biggerbatteries_icon";
            public override string Portrait => "Biggerbatteries";
        }
        public class TacticalLobber : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Tactical Charging";
            public override string DisplayName => "Tactical Charging";
            public override string Description => "ZapMonkey can shoot through walls.";
            public override int Cost => 650;
            public override int Path => BOTTOM;
            public override int Tier => 2;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                towerModel.ignoreBlockers = true;
                attackModel.weapons[0].Rate /= 1.15f;
                attackModel.weapons[0].projectile.ignoreBlockers = true;
                attackModel.weapons[0].projectile.canCollisionBeBlockedByMapLos = false;
                attackModel.attackThroughWalls = true;
            }
            public override string Icon => "Charging_icon";
            public override string Portrait => "Charging";
        }
        public class EMB : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Buzzsaw";
            public override string DisplayName => "Buzzsaw";
            public override string Description => "Attack turns into a saw that rips up balloons.";
            public override int Cost => 1500;
            public override int Path => BOTTOM;
            public override int Tier => 3;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                towerModel.display = "6f2cf506ac8f424459411f7e89a4e12b";
                towerModel.GetBehavior<DisplayModel>().display = "6f2cf506ac8f424459411f7e89a4e12b";
                attackModel.weapons[0].projectile.pierce += 3;
                attackModel.weapons[0].projectile.GetBehavior<DamageModel>().damage += 2;
                attackModel.weapons[0].projectile.display = "33d6310eb4321284e869c1c289be7a39";
            }
            public override string Icon => "Buzzsaw_icon";
            public override string Portrait => "Buzzsaw";
        }
        public class T102Jagdfaust : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Atomic Shredder";
            public override string DisplayName => "Atomic Shredder";
            public override string Description => "Atomic Shredder excels at ripping up MOAB class bloons.";
            public override int Cost => 4500;
            public override int Path => BOTTOM;
            public override int Tier => 4;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].projectile.pierce += 3f;
                attackModel.weapons[0].projectile.GetBehavior<DamageModel>().damage += 30;
                attackModel.weapons[0].Rate /= 1.5f;
            }
            public override string Icon => "atomicshredder_icon";
            public override string Portrait => "Atomicshredder";
        }
        public class Zerfallen : ModUpgrade<ZapMonkey>
        {
            public override string Name => "Particle Dissasembler";
            public override string DisplayName => "Particle Dissasembler";
            public override string Description => "The stronger the bloon is the more damage it does!";
            public override int Cost => 54000;
            public override int Path => BOTTOM;
            public override int Tier => 5;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("aaa", "Moab", 1f, 0, false, false) { tags = new string[] { "Fortified" }, collisionPass = 0 });
                attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("aaa", "Bfb", 1.2f, 0, false, false) { tags = new string[] { "Fortified" }, collisionPass = 0 });
                attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("aaa", "Zomg", 1.5f, 0, false, false) { tags = new string[] { "Fortified" }, collisionPass = 0 });
                attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("aaa", "Bad", 1.6f, 0, false, false) { tags = new string[] { "Fortified" }, collisionPass = 0 });
                attackModel.weapons[0].projectile.GetBehavior<DamageModel>().damage += 150;
                attackModel.weapons[0].Rate /= 3f;

            }
            public override string Icon => "Particledissasembler_icon";
            public override string Portrait => "Particledissasembler";
        }



        [HarmonyLib.HarmonyPatch(typeof(InGame), "Update")]
        public class Update_Patch
        {
            [HarmonyLib.HarmonyPostfix]
            public static void Postfix()
            {
                if (!(InGame.instance != null && InGame.instance.bridge != null)) return;
                try
                {
                    foreach (var tts in InGame.Bridge.GetAllTowers())
                    {

                        if (!tts.namedMonkeyKey.ToLower().Contains("zapmonkey")) continue;
                        if (tts?.tower?.Node?.graphic?.transform != null)
                        {
                            tts.tower.Node.graphic.transform.localScale = new UnityEngine.Vector3(1f, 1f, 1f);

                        }

                    }
                }
                catch
                {

                }


            }
        }
    }
}


    