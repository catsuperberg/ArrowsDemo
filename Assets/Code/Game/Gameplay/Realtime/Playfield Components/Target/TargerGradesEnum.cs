using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Target
{
    public enum TargetGrades
    {
        Common,
        Rare,
        Legendary,
        Epic,
        ENUM_END
    }
    
    static class GradeOperations
    {
        public static float TargetDamagePointsMultiplier(this TargetGrades grade)
        {
            switch (grade)
            {
                case TargetGrades.Common:
                    return 1f;
                case TargetGrades.Rare:
                    return 2.5f;
                case TargetGrades.Legendary:
                    return 7f;
                case TargetGrades.Epic:
                    return 12f;
                default:
                    return 1f;
            }
        }
        
        public static float RewardMultiplier(this TargetGrades grade)
        {
            switch (grade)
            {
                case TargetGrades.Common:
                    return 1f;
                case TargetGrades.Rare:
                    return 1.1f;
                case TargetGrades.Legendary:
                    return 1.25f;
                case TargetGrades.Epic:
                    return 1.6f;
                default:
                    return 1f;
            }
        }
        
        public static Material Material(this TargetGrades grade)
        {                    
            switch (grade)
            {
                case TargetGrades.Common:
                    return Resources.Load("Materials/Enemies/Mat_Common", typeof(Material)) as Material;
                case TargetGrades.Rare:
                    return Resources.Load("Materials/Enemies/Mat_Rare", typeof(Material)) as Material;
                case TargetGrades.Legendary:
                    return Resources.Load("Materials/Enemies/Mat_Legendary", typeof(Material)) as Material;
                case TargetGrades.Epic:
                    return Resources.Load("Materials/Enemies/Mat_Epic", typeof(Material)) as Material;
                default:
                    return Resources.Load("Materials/Enemies/Mat_Common", typeof(Material)) as Material;
            }
        }
    }
}