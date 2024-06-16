﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Monster : LivingEntity
    {

        public string ImageName { get; }

        public int RewardExperiencePoints { get; }

        public Monster(string name, string imageName,
                       int maximumHitPoints, int currentHitPoints,
                       int rewardExperiencePoints, int gold) :
            base(name, maximumHitPoints, currentHitPoints, gold)
        {
            ImageName = $"/Engine;component/Images/Monsters/{imageName}";
            //ImageName = string.Format("pack://application:,,,/Engine;component/Images/Monsters/{0}", imageName);
            RewardExperiencePoints = rewardExperiencePoints;
        }
    }
}
