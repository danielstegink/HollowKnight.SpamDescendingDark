using System.Collections;
using System.Collections.Generic;
using DanielSteginkUtils.Utilities;
using Modding;
using UnityEngine;

namespace SpamDescendingDark
{
    public class SpamDescendingDark : Mod
    {
        internal static SpamDescendingDark Instance;

        public override string GetVersion() => "1.0.0.0";

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;
            On.HeroController.SetDamageModeFSM += OnSetDamageModeFSM;

            Log("Initialized");
        }

        /// <summary>
        /// Descending Dark applies invulnerability using SetDamageModeFSM, so that is the best point to start I-Frames
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="invincibilityType"></param>
        private void OnSetDamageModeFSM(On.HeroController.orig_SetDamageModeFSM orig, HeroController self, 
                                                        int invincibilityType)
        {
            orig(self, invincibilityType);

            if (invincibilityType == 1)
            {
                GameManager.instance.StartCoroutine(Invulnerable());
            }
        }

        /// <summary>
        /// Per my testing, a full cycle of Descending Dark takes about 1.2 seconds.
        /// 
        /// So when invincibility is applied, I will set I-Frames to last a little more than that, ensuring that
        /// the player stays invincible until DDark reapplies its normal invincibility.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Invulnerable()
        {
            // Invulnerable resets after its timer ends, so we need a check to 
            // ensure that 2 overlapping calls don't cancel each other out
            while (HeroController.instance.cState.invulnerable)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }

            yield return ClassIntegrations.CallFunction<HeroController, IEnumerator>(HeroController.instance, 
                                                                                        "Invulnerable", new object[] { 1.5f });
        }
    }
}