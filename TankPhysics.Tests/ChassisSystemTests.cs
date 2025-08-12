using NUnit.Framework;
using SecuredSpace.Important.Raven;
using SecuredSpace.Important.TPhysics;

namespace TankPhysics.Tests {
    public class ChassisSystemTests {
        [Test]
        public void InitTankChassis_AddsComponentsToEntity() {
            var chassis = new ChassisSystem();
            var entity = new Entity();
            var init = new ChassisSystem.ChassisInitNode {
                Entity = entity,
                rigidbody = new RigidbodyComponent(),
                speed = new SpeedComponent(),
                tankColliders = new TankCollidersComponent(),
                chassisConfig = new ChassisConfigComponent(),
                damping = new DampingComponent(),
                weight = new WeightComponent(),
                chassisSmoother = new ChassisSmootherComponent()
            };

            chassis.InitTankChassis(init);

            Assert.That(entity.storage.Count, Is.EqualTo(7));
        }

        [Test]
        public void HotApplyPhysics_WiresValues() {
            var chassisSystem = new ChassisSystem();
            var manager = new TankChassisManager {
                MainChassisSystem = chassisSystem,
                chassisNode = new ChassisSystem.ChassisNode {
                    speedConfig = new SpeedConfigComponent(),
                    speed = new SpeedComponent(),
                    track = new TrackComponent { LeftTrack = new Track(), RightTrack = new Track() },
                    chassis = new ChassisComponent()
                }
            };

            manager.TurnSpeed = 1f;
            manager.Acceleration = 2f;
            manager.Speed = 3f;
            manager.ReverseAcceleration = 4f;
            manager.ReverseTurnAcceleration = 5f;
            manager.SideAcceleration = 6f;
            manager.TurnAcceleration = 7f;
            manager.SpringScaler = 8f;
            manager.SpringCoef = 9f;
            manager.SideDamperScaler = 10f;
            manager.SideSprindDamperDelta = 11f;
            manager.FrontSpringDamperDelta = 12f;
            manager.SwingCoef = 13f;

            manager.HotApplyPhysics();

            Assert.AreEqual(8f, chassisSystem.SpringScaler);
            Assert.AreEqual(9f, chassisSystem.SpringCoefManual);
            Assert.AreEqual(10f, chassisSystem.SideDamperScaler);
            Assert.AreEqual(11f, chassisSystem.SideSprindDamperDelta);
            Assert.AreEqual(12f, chassisSystem.FrontSpringDamperDelta);
            Assert.AreEqual(13f, chassisSystem.SwingCoef);
            Assert.AreEqual(1f, manager.chassisNode.speed.TurnSpeed);
            Assert.AreEqual(2f, manager.chassisNode.speed.Acceleration);
            Assert.AreEqual(3f, manager.chassisNode.speed.Speed);
            Assert.AreEqual(4f, manager.chassisNode.speedConfig.ReverseAcceleration);
            Assert.AreEqual(5f, manager.chassisNode.speedConfig.ReverseTurnAcceleration);
            Assert.AreEqual(6f, manager.chassisNode.speedConfig.SideAcceleration);
            Assert.AreEqual(7f, manager.chassisNode.speedConfig.TurnAcceleration);
        }

        [Test]
        public void FixedUpdate_WithNode_DoesNotThrow() {
            var chassis = new ChassisSystem();
            var node = new ChassisSystem.ChassisNode {
                chassis = new ChassisComponent(),
                effectiveSpeed = new EffectiveSpeedComponent(),
                track = new TrackComponent { LeftTrack = new Track(), RightTrack = new Track() },
                speed = new SpeedComponent(),
                chassisConfig = new ChassisConfigComponent(),
                chassisSmoother = new ChassisSmootherComponent(),
                rigidbody = new RigidbodyComponent(),
                tankColliders = new TankCollidersComponent(),
                speedConfig = new SpeedConfigComponent()
            };
            var jump = new TankJumpComponent();
            var settings = new GameTankSettingsComponent();

            Assert.DoesNotThrow(() => chassis.FixedUpdate(node, jump, settings, true));
        }
    }
}
