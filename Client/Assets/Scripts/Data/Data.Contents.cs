using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    // Stat정보를 굳이 clinet에서 들고 있어야할 필요없다.
    // 모든 정보들은 server쪽에서 보내주고 있다.
    //#region Stat
    //[Serializable]
    //public class Stat
    //{
    //    public int level;
    //    public int maxHp;
    //    public int attack;
    //    public int totalExp;
    //}

    //[Serializable]
    //public class StatData : ILoader<int, Stat>
    //{
    //    public List<Stat> stats = new List<Stat>();

    //    public Dictionary<int, Stat> MakeDict()
    //    {
    //        Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
    //        foreach (Stat stat in stats)
    //            dict.Add(stat.level, stat);
    //        return dict;
    //    }
    //}
    //#endregion

    // SKill도 딱히 여기서 들고 있지 않고
    #region Skill
    [Serializable]
    public class Skill
    {
        public int id;
        public string name;
        public float cooldown;
        public int damage;
        // enum을 parsing할 수 있는지? - json에서는 string이 들어가겠지만
        // 이름이 matching된다면 정상적으로 parsing이 이루어질 것이다.
        public SkillType skillTpye;
        // SkillType이 SKILL_PROJECTILE인 경우에만 존재해야하지만
        // 입력을 안해주면 null로 들어가기 때문에 상관없다.
        public ProjectileInfo projectile;
    }

    public class ProjectileInfo
    {
        public string name;
        public float speed;
        public int range;
        public string prefab;
    }

    [Serializable]
    public class SkillData : ILoader<int, Skill>
    {
        public List<Skill> skills = new List<Skill>();

        public Dictionary<int, Skill> MakeDict()
        {
            Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
            foreach (Skill skill in skills)
                dict.Add(skill.id, skill);
            return dict;
        }
    }

    #endregion
}