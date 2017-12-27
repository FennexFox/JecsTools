﻿using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AbilityUser
{
    public class JobDriver_CastAbilityVerb : JobDriver
    {
        public AbilityContext Context => job.count == 1 ? AbilityContext.Player : AbilityContext.AI;

        private List<CompAbilityUser> CompAbilityUsers
        {
            get
            {
                var results = new List<CompAbilityUser>();
                var allCompAbilityUsers = pawn.GetComps<CompAbilityUser>();
                if (allCompAbilityUsers.TryRandomElement(out var comp))
                    foreach (var compy in allCompAbilityUsers)
                        results.Add(compy);
                return results;
            }
        }

        public override bool TryMakePreToilReservations()
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);

            var verb = pawn.CurJob.verbToUse as Verb_UseAbility;
            if (TargetA.HasThing)
            {
                var getInRangeToil = Toils_Combat.GotoCastPosition(TargetIndex.A, false);
                yield return getInRangeToil;
            }

            if (Context == AbilityContext.Player)
                Find.Targeter.targetingVerb = verb;

            yield return new Toil
            {
                initAction = delegate { verb.Ability.PostAbilityAttempt(); },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return Toils_Combat.CastVerb(TargetIndex.A, false);
        }
    }
}