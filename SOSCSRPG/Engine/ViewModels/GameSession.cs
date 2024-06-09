using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Factories;
using System.ComponentModel;

namespace Engine.ViewModels
{
    public class GameSession : BaseNotificationClass
    {
        private Location _currentLoaction;
        private Monster _currentMonster;
        public World CurrentWorld { get; set; }
        public Player CurrentPlayer { get; set; }
        public Location CurrentLocation
        {
            get { return _currentLoaction; }
            set
            {
                _currentLoaction = value;
                OnPropertyChanged(nameof(CurrentLocation));
                OnPropertyChanged(nameof(HasLocationToNorth));
                OnPropertyChanged(nameof(HasLocationToSouth));
                OnPropertyChanged(nameof(HasLocationToWest));
                OnPropertyChanged(nameof(HasLocationToEast));
                
                GivePlayerQuestAtLocation();
                GetMonsterAtLocation();

            }
        }
        public Monster CurrentMonster
        {
            get { return _currentMonster; }
            set
            {
                _currentMonster = value;
                OnPropertyChanged(nameof(CurrentMonster));
                OnPropertyChanged(nameof(HasMonster));
            }
        }
        public bool HasLocationToNorth
        { 
            get
            {
                return CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate +1) != null;
            }
        }
        public bool HasLocationToEast
        {
            get
            {
                return CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) != null;
            }
        }
        public bool HasLocationToSouth
        {
            get
            {
                return CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1) != null;
            }
        }
        public bool HasLocationToWest
        {
            get
            {
                return CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) != null;
            }
        }

        public bool HasMonster => CurrentMonster != null; // feature of C# called expression-bodied members
                                                          // => lambada operator (separates the input parameters on the left side to the lambda body on the right
                                                          // another example of using it `public bool IsEven => Value % 2 == 0;`

                                                          // In this case:
                                                          // The expression CurrentMonster != null checks if CurrentMonster is not null.
                                                          // If CurrentMonster is not null, it means there is a monster,
                                                          // so HasMonster will return true.
                                                          // If CurrentMonster is null, it means there is no monster, so HasMonster will return false.

        public GameSession()
        {
            CurrentPlayer = new Player 
                            { 
                                Name = "Ben", 
                                CharacterClass = "Fighter",
                                HitPoints = 10,
                                Gold = 10000000,
                                ExperiencePoints = 0,
                                Level = 1,
                            };

            CurrentWorld = WorldFactory.CreateWorld();

            CurrentLocation = CurrentWorld.LocationAt(0, 0);
        }

        public void MoveNorth()
        {
            if(HasLocationToNorth)
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);
        }
        public void MoveWest()
        {
            if(HasLocationToWest)
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate -1, CurrentLocation.YCoordinate);
        }
        public void MoveEast()
        {
            if(HasLocationToEast)
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate +1, CurrentLocation.YCoordinate);
        }
        public void MoveSouth()
        {
            if(HasLocationToSouth)
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate -1);
        }

        private void GivePlayerQuestAtLocation()
        {
            foreach (Quest quest in CurrentLocation.QuestsAvailableHere)
                if (!CurrentPlayer.Quests.Any(q => q.PlayerQuest.ID == quest.ID))
                    CurrentPlayer.Quests.Add(new QuestStatus(quest));
        }
        
        private void GetMonsterAtLocation()
        {
            CurrentMonster = CurrentLocation.GetMonster();
        }
    }
}
