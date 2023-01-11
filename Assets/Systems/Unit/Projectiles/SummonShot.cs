using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;
using TwoBears.Waves;

namespace TwoBears.Unit
{
    public class SummonShot : ProjectileAbility
    {
        [SerializeField] private GameObject summon;
        [SerializeField] private float knockback = 4;
        [SerializeField] private float recovery = 0.4f;

        public override void Trigger(BaseUnit launcher, Faction faction, Collision2D collision)
        {
            //Summon unit
            FormationSpawn spawn = launcher.GetComponentInParent<FormationSpawn>();
            SummonUnit(transform.position + (transform.up * -recovery), transform.up, knockback, spawn, faction);

            //Destroy projectile
            Destroy(gameObject);
        }

        private void SummonUnit(Vector3 position, Vector3 direction, float knockback, FormationSpawn spawner, Faction faction)
        {
            //Spawn unit
            GameObject minion = Instantiate(summon, position, Quaternion.LookRotation(Vector3.forward, direction), spawner.transform);

            //Set faction
            Perceivable perceivable = minion.GetComponent<Perceivable>();
            perceivable.Faction = faction;

            Debug.Log(-direction * knockback);
            Debug.DrawRay(position, direction.normalized, Color.cyan, 100.0f);

            //Knock unit backward
            BaseUnit unit = minion.GetComponent<BaseUnit>();
            unit.KnockBack(-direction * knockback);

            //Register to formation
            spawner.RegisterSpawn(unit);

        }
    }
}