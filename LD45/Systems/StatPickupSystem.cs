﻿using Artemis;
using Artemis.System;
using LD45.Components;
using Microsoft.Xna.Framework;

namespace LD45.Systems {
    public sealed class StatPickupSystem : EntityProcessingSystem {
        private const float _pickupDistance = 16f;
        private const float _pickupDistanceSqr = _pickupDistance * _pickupDistance;

        private readonly Aspect _statAspect = Aspect.All(typeof(StatDropComponent), typeof(BodyComponent));

        public StatPickupSystem() 
            : base(Aspect.All(typeof(CommanderComponent), typeof(BodyComponent))) {
        }

        public override void Process(Entity entity) {
            var commanderComponent = entity.GetComponent<CommanderComponent>();
            var bodyComponent = entity.GetComponent<BodyComponent>();

            foreach (Entity statEntity in EntityWorld.EntityManager.GetEntities(_statAspect)) {
                var statComponent = statEntity.GetComponent<StatDropComponent>();
                var statBodyComponent = statEntity.GetComponent<BodyComponent>();

                float distanceSqr = Vector2.DistanceSquared(bodyComponent.Position, statBodyComponent.Position);

                if (distanceSqr < _pickupDistanceSqr) {
                    commanderComponent.Strength += statComponent.Strength;
                    commanderComponent.Armor += statComponent.Armor;
                    commanderComponent.Magic += statComponent.Magic;
                    commanderComponent.Resistance += statComponent.Resistance;
                    commanderComponent.Force += statComponent.Force;
                    commanderComponent.Stability += statComponent.Stability;
                    commanderComponent.Range += statComponent.Range;
                    commanderComponent.Speed += statComponent.Speed;
                    commanderComponent.Accuracy += statComponent.Accuracy;

                    statEntity.Delete();
                    break;
                }
            }
        }
    }
}
