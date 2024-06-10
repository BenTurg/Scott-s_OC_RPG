using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Player : BaseNotificationClass
    {

        #region Properties 
        //The #region and #endregion directives in C# are primarily for the benefit of the developer, not the machine.
        //They are used to organize code into collapsible sections in the code editor,
        //which can make it easier to navigate and manage your code.
        //in this case we divide this region to 'Properties' but it's for the developer's choise

        private string _name;
        private string _characterClass;
        private int _hitPoints;
        private int _experiencePoints;
        private int _level;
        private int _gold;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string CharacterClass
        {
            get { return _characterClass; }
            set
            {
                _characterClass = value;
                OnPropertyChanged(nameof(CharacterClass));
            }
        }
        public int HitPoints
        {
            get { return _hitPoints; }
            set
            {
                _hitPoints = value;
                OnPropertyChanged(nameof(HitPoints));
            }
        }
        public int ExperiencePoints
        {
            get { return _experiencePoints; }
            set
            {
                _experiencePoints = value;
                OnPropertyChanged(nameof(ExperiencePoints));
            }
        }
        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                OnPropertyChanged(nameof(Level));
            }
        }
        public int Gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                OnPropertyChanged(nameof(Gold));
            }
        }

        public ObservableCollection<GameItem> Inventory { get; set; }

        public List<GameItem> Weapons =>
            Inventory.Where(i => i is Weapon).ToList();//The Where method returns a sequence (more specifically, an IEnumerable<GameItem>)
                                                       //of elements from the Inventory that satisfy the condition specified
                                                       //by the lambda expression i => i is Weapon.
                                                       //The ToList() method is then called to convert this sequence into a List<GameItem>.
                                                       //This is done because working with a List can be more convenient and efficient than
                                                       //working with an IEnumerable, depending on what you plan to do with the collection

        public ObservableCollection<QuestStatus> Quests { get; set; }

        #endregion

        public Player()
        {
            Inventory = new ObservableCollection<GameItem>();
            Quests = new ObservableCollection<QuestStatus>();
        }

        public void AddItemToInventory(GameItem item)
        {
            Inventory.Add(item);

            OnPropertyChanged(nameof(Weapons));
        }
    }
}
