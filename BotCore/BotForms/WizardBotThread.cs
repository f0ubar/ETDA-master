using BotCore;
using BotCore.Actions;
using BotCore.Components;
using BotCore.PathFinding;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Bot
{
    public class WizardBotThread
    {
        public WizardBotThread(Client Client)
        {
            this.Client = Client;
            Target = new Targeting(this);
            PathFinder = new SonarV2(Client);
            WalkingPath = new Pedometer(Client, PathFinder);
        }

        Client Client;

        Position Position { get { return Client.Attributes.ServerPosition; } }

        public SonarV2 PathFinder;

        public bool Paused = false;

        #region Auto Pickup
        public bool AutoPickUp = true;
        public DateTime LastAutoPickup = DateTime.Now.AddSeconds(5);
        public TimeSpan SpanSinceLastAutoPickup { get { return DateTime.Now - LastAutoPickup; } }
        public TimeSpan AutoPickupWait = new TimeSpan(0, 0, 1);
        public bool CanPickup { get { return AutoPickUp && SpanSinceLastAutoPickup > AutoPickupWait; } }
        #endregion

        #region Targeting
        public class Targeting
        {
            public Targeting(WizardBotThread WizardBotThread)
            {
                this.WizardBotThread = WizardBotThread;
            }

            public WizardBotThread WizardBotThread;
            public Client Client { get { return WizardBotThread.Client; } }
            public Position Position { get { return WizardBotThread.Position; } }
            public SonarV2 PathFinder { get { return WizardBotThread.PathFinder; } }

            #region Settings
            public int MaxDistance = 9;
            public bool TargetWithoutPath = true;
            public bool TargetHostileFirst = true, TargetOnlyHostile = false;
            public List<int> HostileSprites = new List<int>(new int[] { 52 });
            public List<int> NonHostileSprites = new List<int>(new int[] { 53, 7 });
            public bool TargetExplicitSpriteList = true;
            public bool SwitchToHostileTargetIfAvailable = true;
            #endregion

            #region Active Map Object List
            public List<MapObject> ActiveMapObjectList = new List<MapObject>();
            public Dictionary<int, MapObject> ActiveMapObjectDictionary = new Dictionary<int, MapObject>();
            private void RefreshMapObjectList()
            {
                ActiveMapObjectList.Clear();
                ActiveMapObjectList.AddRange(Client.ObjectSearcher.RetrieveMonsterTargets(o => o != null && o.Active));
                ActiveMapObjectList
                    .Where(o => !ActiveMapObjectDictionary.ContainsKey(o.Serial)).ToList()
                    .ForEach(o => ActiveMapObjectDictionary.Add(o.Serial, o));
                int[] activeMapObjectListSerials = ActiveMapObjectList.Select(o => o.Serial).ToArray();
                ActiveMapObjectDictionary
                    .Where(o => !activeMapObjectListSerials.Contains(o.Key)).ToList()
                    .ForEach(o => ActiveMapObjectDictionary.Remove(o.Key));
                if (HaveTarget && (!TargetExists || !TargetIsActive))
                    CurrentTarget = null;
            }
            #endregion

            #region TargetingLists
            private IEnumerable<MapObject> Targetable { get { return ActiveMapObjectList.Where(o => MaxDistance < 0 || Position.DistanceFrom(o.ServerPosition) <= MaxDistance); } }
            private IEnumerable<MapObject> HostileTargets { get { return Targetable.Where(o => HostileSprites.Contains(o.Sprite)); } }
            private IEnumerable<MapObject> HostileAndNonHostileTargets { get { return Targetable.Where(o => HostileSprites.Contains(o.Sprite) || NonHostileSprites.Contains(o.Sprite)); } }
            private IEnumerable<MapObject> AllTargets { get { return Targetable.Where(o => true); } }
            public List<MapObject> CurrentTargets = new List<MapObject>();
            private bool HaveTargets { get { return CurrentTargets.Count > 0; } }
            #endregion

            #region Current Target Info
            public MapObject CurrentTarget = null;
            public bool HaveTarget { get { return CurrentTarget != null; } }
            private bool TargetExists { get { return ActiveMapObjectDictionary.ContainsKey(CurrentTarget.Serial); } }
            private bool TargetIsActive { get { return CurrentTarget.Active; } }
            private bool TargetIsHostile { get { return HostileSprites.Contains(CurrentTarget.Sprite); } }
            #region Target Info Class
            public class Info
            {
                public Info(MapObject Value, Client Client, Targeting Targeting, SonarV2 PathFinder)
                {
                    this.Value = Value;
                    this.Client = Client;
                    this.Targeting = Targeting;
                    this.PathFinder = PathFinder;
                    UpdatePathToTarget();
                }
                public MapObject Value;
                private Client Client;
                private Targeting Targeting;
                private SonarV2 PathFinder;
                public bool BadTargetInfo { get { return Targeting.CurrentTarget != Value; } }
                private Position ClientPosition { get { return Client.Attributes.ServerPosition; } }
                Position Position { get { return Value.ServerPosition; } }
                public bool Active { get { return Value.Active; } }
                public bool Hostile { get { return Targeting.HostileSprites.Contains(Value.Sprite); } }
                public int Distance { get { return ClientPosition.DistanceFrom(Position); } }
                public SonarV2.Node PathToTarget;
                public bool PathToTargetValid
                {
                    get
                    {
                        if (PathToTarget != null)
                        {
                            SonarV2.Node currentNode = PathToTarget.FirstAtPosition(Client.Attributes.ServerPosition);
                            if (currentNode != null &&
                                currentNode.LastNode.Position.SameLocationAs(Value.ServerPosition))
                                return true;
                        }
                        PathToTarget = null;
                        return false;
                    }
                }
                public void UpdatePathToTarget() { PathToTarget = PathFinder.FindPathTo(Position); }
                public void CheckPathToTarget() { if (!PathToTargetValid) UpdatePathToTarget(); }
            }
            public Info CurrentTargetInfo { get { return new Info(CurrentTarget, Client, this, PathFinder); } }
            #endregion
            private IEnumerable<MapObject> WithoutPathFilter(IEnumerable<MapObject> Collection)
            {
                return
                    TargetWithoutPath ?
                        Collection :
                        Collection.Where(o =>
                        {
                            List<PathSolver.PathFinderNode> path = Client.FieldMap.Search(Position, o.ServerPosition);
                            return path != null && path.Count > 1;
                        });
            }
            public bool CheckTarget()
            {
                RefreshMapObjectList();
                CurrentTargets.Clear();
                if (TargetHostileFirst || TargetOnlyHostile)
                {
                    CurrentTargets.AddRange(WithoutPathFilter(HostileTargets));
                    if (CurrentTargets.Count > 0)
                        Console.WriteLine($"{CurrentTargets.Count} hostile units detected");
                    if (SwitchToHostileTargetIfAvailable && HaveTargets && HaveTarget && !TargetIsHostile)
                        CurrentTarget = CurrentTargets.OrderBy(o => Position.DistanceFrom(o.ServerPosition)).First();
                }
                if (!TargetOnlyHostile && !HaveTargets)
                        CurrentTargets.AddRange(WithoutPathFilter(TargetExplicitSpriteList ? HostileAndNonHostileTargets : AllTargets));
                if (HaveTargets && !HaveTarget)
                    CurrentTarget = CurrentTargets.OrderBy(o => Position.DistanceFrom(o.ServerPosition)).First();
                return HaveTarget;
            }
            #endregion

            public IEnumerable<MapObject> FloorObjects { get { return Client.ObjectSearcher.RetreiveAllTargets(o => o != null && o.Type == MapObjectType.Item); } }
        }
        public Targeting Target;
        #endregion

        #region Auto Walk
        #region HP / MP Variables
        public float MP { get { return (float)Client.Attributes.MP; } }
        public float MaxMP { get { return (float)Client.Attributes.MaxMP; } }
        public float MPPercent { get { return (MP / MaxMP) * 100f; } }

        public float HP { get { return (float)Client.Attributes.HP; } }
        public float MaxHP { get { return (float)Client.Attributes.MaxHP; } }
        public float HPPercent { get { return (HP / MaxHP) * 100f; } }
        #endregion

        #region Pedometer
        public class Pedometer
        {
            public Pedometer(Client Client, SonarV2 PathFinder)
            {
                this.Client = Client;
                this.PathFinder = PathFinder;
            }
            public Client Client;
            public bool PathExists { get { return CurrentPath != null; } }
            public SonarV2.Node CurrentPath = null;
            private RandomNumberGenerator Randomizer = RandomNumberGenerator.Create();
            private Position RandomMapPosition
            {
                get
                {
                    byte[] shortBytes = new byte[2];
                    Randomizer.GetBytes(shortBytes);
                    ushort randomValue = BitConverter.ToUInt16(shortBytes, 0);
                    ushort X = (ushort)(randomValue % Client.FieldMap.Width);
                    while (X == 0 || X >= Client.FieldMap.Width - 1)
                    {
                        Randomizer.GetBytes(shortBytes);
                        randomValue = BitConverter.ToUInt16(shortBytes, 0);
                        X = (ushort)(randomValue % Client.FieldMap.Width);
                    }
                    Randomizer.GetBytes(shortBytes);
                    randomValue = BitConverter.ToUInt16(shortBytes, 0);
                    ushort Y = (ushort)(randomValue % Client.FieldMap.Height);
                    while (Y == 0 || Y >= Client.FieldMap.Height - 1)
                    {
                        Randomizer.GetBytes(shortBytes);
                        randomValue = BitConverter.ToUInt16(shortBytes, 0);
                        Y = (ushort)(randomValue % Client.FieldMap.Height);
                    }
                    return new Position(X, Y);
                }
            }
            SonarV2 PathFinder;
            private SonarV2.Node RandomMapPath
            {
                get
                {
                    Position randomPosition = RandomMapPosition;
                    while (Client.Attributes.ServerPosition.DistanceFrom(randomPosition) < ((double)(Client.FieldMap.Width + Client.FieldMap.Height) * .5f))
                        randomPosition = RandomMapPosition;
                    SonarV2.Node path = PathFinder.FindPathTo(randomPosition);
                    while (path == null)
                    {
                        randomPosition = RandomMapPosition;
                        while (Client.Attributes.ServerPosition.DistanceFrom(randomPosition) < ((double)(Client.FieldMap.Width + Client.FieldMap.Height) * .5f))
                            randomPosition = RandomMapPosition;
                        path = PathFinder.FindPathTo(randomPosition);
                    }
                    return path;
                }
            }
            public SonarV2.Node CurrentPathLocation;
            public void CheckCurrentPath()
            {
                if (CurrentPath == null)
                    CurrentPath = RandomMapPath;
                CurrentPathLocation = CurrentPath != null ? CurrentPath.FirstAtPosition(Client.Attributes.ServerPosition) : null;
                if (CurrentPathLocation == null)
                    CurrentPath = null;
                else
                {
                    if (CurrentPathLocation.NextNode == null)
                        CurrentPath = CurrentPathLocation = null;
                    else
                    {
                        if (Client.FieldMap.MapObjects.Where(
                            o =>
                                (o.Type == MapObjectType.Aisling |
                                o.Type == MapObjectType.Monster |
                                o.Type == MapObjectType.NPC) &&
                                CurrentPathLocation.NextNode.Position.DistanceFrom(o.ServerPosition) == 0
                            ).Count() > 0)
                            CurrentPath = CurrentPathLocation = null;
                    }
                }
            }
        }
        public Pedometer WalkingPath;
        #endregion 
        #endregion

        public void ThreadLoop()
        {
            int i = 0;
            Targeting.Info TargetInfo = null;
            while (true)
            {
                if (!Paused &&
                    Client != null &&
                    Client.ClientReady &&
                    Client.IsInGame() &&
                    Client.Attributes != null &&
                    Client.ObjectSearcher != null &&
                    Client.MapLoaded)
                {
                    WalkingPath.CheckCurrentPath();
                    #region Auto Pickup
                    if (AutoPickUp)
                    {
                        MapObject pickupItem =
                            Target.FloorObjects.Where(o =>
                                o != null &&
                                !o.PickedUp &&
                                Client.Attributes.ServerPosition.DistanceFrom(o.ServerPosition) > 0 &&
                                (WalkingPath.CurrentPath == null || WalkingPath.CurrentPathLocation == null || WalkingPath.CurrentPathLocation.FirstNodeBackwardsWhere(node => node.Position.DistanceFrom(o.ServerPosition) == 0) == null) &&
                                Position.WithinSquare(o.ServerPosition, 2)).FirstOrDefault();
                        if (pickupItem != null)
                        {
                            pickupItem.PickedUp = true;
                            Console.WriteLine($"Picking up {pickupItem.ServerPosition}");
                            GameActions.Pickup(Client, pickupItem.ServerPosition);
                            continue;
                        }
                    }
                    #endregion
                    #region Targeting
                    if (Target.CheckTarget())
                    {
                        if (TargetInfo == null || TargetInfo.BadTargetInfo) TargetInfo = Target.CurrentTargetInfo;
                        TargetInfo.CheckPathToTarget();
                        if (!TargetInfo.BadTargetInfo)
                        {
                            string[] spells = new string[]
                            {
                                //"beag srad",
                                "beag sal",
                                //"beag athar"
                            };
                            string spell = spells[i = (++i % spells.Length)];
                            Console.WriteLine($"Casting on sprite {TargetInfo.Value.Sprite}");
                            Client.Utilities.CastSpell(spells[i], TargetInfo.Value);
                            //    System.Threading.Thread.Sleep(100);
                        }
                    }
                    #endregion
                    #region Auto Walk
                    else if (WalkingPath.CurrentPath != null && HPPercent > 50 && MPPercent > 50)
                    {
                        Direction pathDirection = WalkingPath.CurrentPathLocation.Direction;
                        SonarV2.Node nextNode = WalkingPath.CurrentPathLocation.NextNode;
                        bool Walkable = !Client.FieldMap.IsWall(nextNode.Position.X, nextNode.Position.Y);
                        if (!Walkable)
                        {
                            WalkingPath.CurrentPath = WalkingPath.CurrentPathLocation = null;
                            continue;
                        }
                        if (pathDirection == Direction.None || nextNode.Attempts > 4)
                        {
                            WalkingPath.CurrentPath = WalkingPath.CurrentPathLocation = null;
                            continue;
                        }
                        nextNode.Attempts++;
                        GameActions.Walk(Client, pathDirection);
                        System.Threading.Thread.Sleep(375);
                    } 
                    #endregion
                }
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
