using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Actions;

namespace Engine.Factories
{
    public static class ItemFactory
    {
        private static readonly List<GameItem> _standardGameItems = new List<GameItem>();

        static ItemFactory()
        {
            BuildWeapon(1001, "Pointy Stick", 1, 1, 2);
            BuildWeapon(1002, "Rusty Sword", 5, 1, 3);
            BuildMiscellaneousItem(9001, "Snake fang", 1);
            BuildMiscellaneousItem(9002, "Snakeskin", 2);
            BuildMiscellaneousItem(9003, "Rat tail", 1);
            BuildMiscellaneousItem(9004, "Rat fur", 2);
            BuildMiscellaneousItem(9005, "Spider fang", 1);
            BuildMiscellaneousItem(9006, "Spider silk", 2);
        }


        public static GameItem CreateGameItem(int itemTypeID)
        {//use LINQ to find the first item matches the ID parameters. FirstOrDefault return the default value if isnt find. in this case null
            return _standardGameItems.FirstOrDefault(item => item.ItemTypeID == itemTypeID)?.Clone();
        }
        private static void BuildMiscellaneousItem(int id, string name, int price)
        {
            _standardGameItems.Add(new GameItem(GameItem.ItemCategory.Miscellaneous, id, name, price));
        }
        private static void BuildWeapon(int id, string name, int price, int minimumDamage, int maximumDamage)
        {
            //for the design pattern Command: we need to do it with two steps
            //first create GameItem weapon - and then create Action that get this weapon and belong to this weapon
            //P.S it little mess but we will try to make it cleaner later
            GameItem weapon = new GameItem(GameItem.ItemCategory.Weapon, id, name, price);
            weapon.Action = new AttackWithWeapon(weapon, minimumDamage, maximumDamage);

            _standardGameItems.Add(weapon);

            //and here we can see the desgin pattern composition over inheritance that create Genric object
            //and this Object it the same object but with different properties 
        }
    }
}
