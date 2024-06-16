using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Factories;
using System.ComponentModel;
using Engine.EventArgs;

namespace Engine.ViewModels
{
    public class GameSession : BaseNotificationClass
    {
        public event EventHandler<GameMessageEventArgs> OnMessageRaised;

        private Player _currentPlayer;
        private Location _currentLoaction;
        private Monster _currentMonster;
        private Trader _currentTrader;

        public World CurrentWorld { get; }
        public Player CurrentPlayer
        {
            get { return _currentPlayer; }
            set
            {
                if (_currentPlayer != null)
                {
                    _currentPlayer.OnActionPreformed -= OnCurrentPlayerPerformedAction;
                    _currentPlayer.OnLeveledUp -= OnCurrentPlayerLeveledUp;
                    _currentPlayer.OnKilled -= OnCurrentPlayerKilled; // meaning of 'object.event -= func' unsubscribe from the event of runnin 'func'
                }
                
                _currentPlayer = value;

                if (_currentPlayer != null)
                {
                    _currentPlayer.OnActionPreformed += OnCurrentPlayerPerformedAction;
                    _currentPlayer.OnLeveledUp += OnCurrentPlayerLeveledUp;
                    _currentPlayer.OnKilled += OnCurrentPlayerKilled;
                }
            }
        }

        public Location CurrentLocation
        {
            get { return _currentLoaction; }
            set
            {
                _currentLoaction = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasLocationToNorth));//on this OnPropertyChanged we need nameof("func")
                OnPropertyChanged(nameof(HasLocationToSouth));//beceause we call it from another function
                OnPropertyChanged(nameof(HasLocationToWest));
                OnPropertyChanged(nameof(HasLocationToEast));

                CompleteQuestsAtLocation();
                GivePlayerQuestAtLocation();
                GetMonsterAtLocation();
                CurrentTrader = CurrentLocation.TradeHere;

            }
        }
        public Monster CurrentMonster
        {
            get { return _currentMonster; }
            set
            {
                if(_currentMonster != null)
                    _currentMonster.OnKilled -= OnCurrentMonsterKilled; // meaning of 'object.event -= func' unsubscribe from the event of runnin 'func'

                _currentMonster = value;

                if(CurrentMonster != null)
                {
                    _currentMonster.OnKilled += OnCurrentMonsterKilled;

                    RaiseMessage("");
                    RaiseMessage($"You see a {CurrentMonster.Name} here!");
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasMonster));
            }
        }

        public Trader CurrentTrader
        {
            get { return _currentTrader; }
            set
            {
                _currentTrader = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasTrader));
            }
        }

        public bool HasLocationToNorth =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate +1) != null;
        public bool HasLocationToEast =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) != null;
        public bool HasLocationToSouth => 
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1) != null;
        public bool HasLocationToWest =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) != null;

        // feature of C# called expression-bodied members
        // => lambada operator (separates the input parameters on the left side to the lambda body on the right
        // another example of using it `public bool IsEven => Value % 2 == 0;`

        // In this case:
        // The expression CurrentMonster != null checks if CurrentMonster is not null.
        // If CurrentMonster is not null, it means there is a monster,
        // so HasMonster will return true.
        // If CurrentMonster is null, it means there is no monster, so HasMonster will return false.
        public bool HasMonster => CurrentMonster != null; 
        public bool HasTrader => CurrentTrader != null;

        public GameSession()
        {
            CurrentPlayer = new Player("Ben10", "Fighter", 0, 10, 10, 100000);

            if (!CurrentPlayer.Weapons.Any())
            {
                CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(1001));//pointy_stick as defualt
            }

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

        private void CompleteQuestsAtLocation()
        {
            foreach (Quest quest in CurrentLocation.QuestsAvailableHere)
            {
                QuestStatus questToComplete =
                    CurrentPlayer.Quests.FirstOrDefault(q => q.PlayerQuest.ID == quest.ID && !q.IsCompleted);
                if (questToComplete != null)
                {
                    if (CurrentPlayer.HasAllTheseItems(quest.ItemsToComplete))
                    {
                        // Remove the quest completion items from the player's inventory
                        foreach (ItemQuantity itemQuantity in quest.ItemsToComplete)
                        {
                            for (int i = 0; i < itemQuantity.Quantity; i++)
                            {
                                CurrentPlayer.RemoveItemFromInventory(CurrentPlayer.Inventory.First(item => item.ItemTypeID == itemQuantity.ItemID));
                            }
                        }
                        RaiseMessage("");
                        RaiseMessage($"You completed the '{quest.Name}' quest");

                        // Give the player the quest rewards
                        RaiseMessage($"You receive {quest.RewardExperiencePoints} experience points");
                        CurrentPlayer.AddExperience(quest.RewardExperiencePoints);

                        RaiseMessage($"You receive {quest.RewardGold} gold");
                        CurrentPlayer.ReceiveGold(quest.RewardGold);

                        foreach (ItemQuantity itemQuantity in quest.RewardItems)
                        {
                            GameItem rewardItem = ItemFactory.CreateGameItem(itemQuantity.ItemID);
                            CurrentPlayer.AddItemToInventory(rewardItem);
                            RaiseMessage($"You receive a {rewardItem.Name}");
                        }

                        // Mark the Quest as completed
                        questToComplete.IsCompleted = true;
                    }
                }
            }
        }

        private void GivePlayerQuestAtLocation()
        {
            foreach (Quest quest in CurrentLocation.QuestsAvailableHere)
            {
                if (!CurrentPlayer.Quests.Any(q => q.PlayerQuest.ID == quest.ID))
                {
                    CurrentPlayer.Quests.Add(new QuestStatus(quest));
                    RaiseMessage("");
                    RaiseMessage($"You receive the '{quest.Name}' quest");
                    RaiseMessage(quest.Description);
                    RaiseMessage("Return with:");
                    foreach (ItemQuantity itemQuantity in quest.ItemsToComplete)
                    {
                        RaiseMessage($"   {itemQuantity.Quantity} {ItemFactory.CreateGameItem(itemQuantity.ItemID).Name}");
                    }
                    RaiseMessage("And you will receive:");
                    RaiseMessage($"   {quest.RewardExperiencePoints} experience points");
                    RaiseMessage($"   {quest.RewardGold} gold");
                    foreach (ItemQuantity itemQuantity in quest.RewardItems)
                    {
                        RaiseMessage($"   {itemQuantity.Quantity} {ItemFactory.CreateGameItem(itemQuantity.ItemID).Name}");
                    }
                }
            }
        }
        
        private void GetMonsterAtLocation()
        {
            CurrentMonster = CurrentLocation.GetMonster();
        }

        public void AttackCurrentMonster()
        {
            if (CurrentPlayer.CurrentWeapon == null)
            {
                RaiseMessage("You must select a weapon, to attack.");
                return;
            }

            CurrentPlayer.UseCurrentWeaponOn(CurrentMonster);

            if (CurrentMonster.IsDead)
            {
                //get another monster to fight with (on this location)
                
                GetMonsterAtLocation();
            }
            else
            {
                // Let the monster attack
                int damageToPlayer =
                    RandomNumberGenerator.NumberBetween(CurrentMonster.MinimumDamage, CurrentMonster.MaximumDamage);
                if (damageToPlayer == 0)
                {
                    RaiseMessage($"The {CurrentMonster.Name} attacks, but misses you.");
                }
                else
                {
                    RaiseMessage($"The {CurrentMonster.Name} hit you for {damageToPlayer} points.");
                    CurrentPlayer.TakeDamage(damageToPlayer);
                }
            }
        
        }

        private void OnCurrentPlayerPerformedAction(object sender, string result)
        {
            RaiseMessage(result);
        }

        private void OnCurrentPlayerKilled(object sender, System.EventArgs eventArgs)
        {
            RaiseMessage("");
            RaiseMessage("You have been killed");
            //RaiseMessage($"The {CurrentMonster.Name} killed you."); //create error on my unit test -> because in the test there isnt monster that kill you
            
            CurrentLocation = CurrentWorld.LocationAt(0, -1);
            CurrentPlayer.CompletelyHeal();
        }

        private void OnCurrentMonsterKilled(object sender, System.EventArgs eventArgs)
        {
            RaiseMessage("");
            RaiseMessage($"You defeated the {CurrentMonster.Name}!");

            RaiseMessage($"You receive {CurrentMonster.RewardExperiencePoints} experience points.");
            CurrentPlayer.AddExperience(CurrentMonster.RewardExperiencePoints);

            RaiseMessage($"You receive {CurrentMonster.Gold} gold.");
            CurrentPlayer.ReceiveGold(CurrentMonster.Gold);

            foreach (GameItem gameItem in CurrentMonster.Inventory)
            {
                RaiseMessage($"You receive one {gameItem.Name}.");
                CurrentPlayer.AddItemToInventory(gameItem);
            }
        }

        private void OnCurrentPlayerLeveledUp(object sender, System.EventArgs eventArgs)
        {
            RaiseMessage($"You are now level {CurrentPlayer.Level}!");
        }

        private void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new GameMessageEventArgs(message));
        }
    }
}
