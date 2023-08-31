using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Judiva.Metaverse.Interactions
{
    public class PairingManagerTestFramework : JMonoBehaviour
    {

        public GameObject JumpTestPrefab;
        public GameObject TeleporterTestPrefab;

        private void Start()
        {
            PairJumps();
            //PairTeleporters();
        }

        private void PairJumps()
        {
            Jump[] jumps = FindObjectsOfType<Jump>();

            foreach (var jump in jumps)
            {
                TriggerJumpTestFramework jumpTestFramework = Instantiate(JumpTestPrefab).GetComponent<TriggerJumpTestFramework>();
                jumpTestFramework.SetTriggerJump(jump.verticalVelocity, jump.orientationOffset, jump.additionalSpeed, jump.transform);
            }
        }

        private void PairTeleporters()
        {
            Teleporter[] teleporters = FindObjectsOfType<Teleporter>();

            List<TeleporterTriggerTestFramework> teleporterTriggerTestFrameworks = new List<TeleporterTriggerTestFramework>();

            int index = 0;

            foreach (var teleporter in teleporters)
            {
                TeleporterTriggerTestFramework teleporterTriggerTestFramework = Instantiate(TeleporterTestPrefab).GetComponent<TeleporterTriggerTestFramework>();
                teleporter.gameObject.name = teleporter.gameObject.name +"$"+ index;
                teleporterTriggerTestFramework.gameObject.name = teleporter.gameObject.name;
                teleporterTriggerTestFrameworks.Add(teleporterTriggerTestFramework);
                index = index + 1;
            }

            foreach (var teleporterTriggerTestFramework in teleporterTriggerTestFrameworks)
            {
                foreach (var teleporter in teleporters)
                {
                    if (teleporter.connected.gameObject.name == teleporterTriggerTestFramework.gameObject.name)
                    {
                        teleporterTriggerTestFramework.SetupTeleport(teleporterTriggerTestFramework, teleporter.transform);
                    }
                }
                
            }
        }
    }
}
