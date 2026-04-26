using UnityEngine;

namespace ACT.Scripts
{
    public abstract class UnitModifierSO : ScriptableObject
    {
        [Header("Health points"), SerializeField] private int _hp;
        [Header("Attack power"), SerializeField] private int _atk;
        [Header("Move speed"), SerializeField] private float _speed;
        [Header("Attack speed"), SerializeField] private float _atkspeed;
        [Header("Knockback power"), SerializeField] private float _knockbackPower;
        
        //Геттеры для получения значений модификаторов. 
        //Редактирование только из инспектора, чтобы не допустить сложноуловимые баги.
        public int HP => _hp;
        public int ATK => _atk;
        public float SPEED => _speed;
        public float ATKSPD => _atkspeed;
        public float Knockback => _knockbackPower;
    }
}
