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
        public static float TargetScoreMultiplier(this TargetGrades grade)
        {
            switch (grade)
            {
                case TargetGrades.Common:
                    return 1;
                case TargetGrades.Rare:
                    return 2.5f;
                case TargetGrades.Legendary:
                    return 7;
                case TargetGrades.Epic:
                    return 12;
                default:
                    return 1;
            }
        }
        
        public static float RevardMultiplier(this TargetGrades grade)
        {
            switch (grade)
            {
                case TargetGrades.Common:
                    return 1f;
                case TargetGrades.Rare:
                    return 1.5f;
                case TargetGrades.Legendary:
                    return 2f;
                case TargetGrades.Epic:
                    return 5f;
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